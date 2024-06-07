#region Copyright SHARP Corporation
//  Copyright (c) 2024 SHARP CORPORATION. All rights reserved.
//
//  SHARP OSA SDK
//
//  This software is protected under the Copyright Laws of the United States,
//  Title 17 USC, by the Berne Convention, and the copyright laws of other
//  countries.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDER ``AS IS'' AND ANY EXPRESS
//  OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
//  OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
//
//==============================================================================
#endregion
#region FileHeader
//===========================================================================
//  FILE          : HttpHelper.cs
//                  HttpHelper   - Helper class for WsHttpProxy used for Writing data into Blocks
//                                 and Reading Chunked Data.
//
//  MODULE        : OSA Simulator
//
//  OWNER         : Sharp Software Development India
//===========================================================================
#endregion
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Collections.Specialized;
// using Sharp.OsaSimulator;
// using Sharp.OsaSimulator.StandardLibrary;
namespace Utility
{
    public class HttpHelper
    {
        /// <summary>
        /// Read all the chunked body using Binary Reader
        /// </summary>
        /// <param name="reader">The instance of binary reader.</param>
        /// <returns>A byte array containing the assembled chunks.</returns>
        public static byte[] ReadToEndChunked(BinaryReader reader, out int[] chunks)
        {
            if (null == reader)
            {
                throw new ArgumentNullException();
            }
            ArrayList list = new ArrayList();
            long totalBytes = 0;
            byte[] buffer;
            long len;
            long offset = 0;
            chunks = null;
            while (true)
            {
                string pre = HttpHelper.ReadLine(reader);
                pre = pre.Replace("\r\n", string.Empty);
                if (string.IsNullOrEmpty(pre))
                    break;
                if (!Int64.TryParse(pre, System.Globalization.NumberStyles.HexNumber,
                    System.Globalization.CultureInfo.CurrentCulture, out len))
                    return null;
                if (0 == len)
                    break;
                buffer = new byte[len];
                offset = 0;
                while (offset < len)
                {
                    int bytesRead = reader.Read(buffer, (int)offset, (int)(len - offset));
                    if (bytesRead == 0)
                        return null;
                    offset += bytesRead;
                }
                totalBytes += len;
                list.Add(buffer);
                HttpHelper.ReadLine(reader);
            }
            // Assemble all the blocks
            byte[] data = new byte[totalBytes];
            chunks = new int[list.Count];
            offset = 0;
            for (int x = 0; x < list.Count; x++)
            {
                buffer = list[x] as byte[];
                Array.Copy(buffer, 0, data, offset, buffer.Length);
                offset += buffer.Length;
                chunks[x] = buffer.Length;
            }
            return data;
        }
        /// <summary>
        /// Reads a string line using BinaryReader using ASCII encoding.
        /// </summary>
        /// <param name="reader">The instance of BinaryReader</param>
        /// <returns>A text line as a string.</returns>
        public static string ReadLine(BinaryReader reader)
        {
            if (null == reader)
            {
                throw new ArgumentNullException();
            }
            const int BUFFER_SIZE = 128;
            byte prev = 0;
            byte data;
            char[] buffer = new char[BUFFER_SIZE];
            int offset = 0;
            string ret = string.Empty;
            do
            {
                data = reader.ReadByte();
                char s = Convert.ToChar(data);
                if (offset >= BUFFER_SIZE)
                {
                    ret += new string(buffer);
                    offset = 0;
                }
                buffer[offset] = s;
                offset++;
                if (prev == '\r' && data == '\n')
                {
                    ret += new string(buffer, 0, offset);
                    return ret;
                }
                prev = data;
            } while (true);
        }
        /// <summary>
        /// Read the headers
        /// </summary>
        /// <param name="reader">The stream reader instance.</param>
        public static string ReadHeaders(BinaryReader reader)
        {
            int lineCount = 0;
            StringBuilder headers = new StringBuilder();
            string line;
            try
            {
                do
                {
                    line = ReadLine(reader);
                    if (!string.IsNullOrEmpty(line))
                    {
                        headers.Append(line);
                    }
                    lineCount++;
                } while ("\r\n" != line);
            }
            catch (IOException)
            {
                // Ignore
            }
            return headers.ToString();
        }
        /// <summary>
        /// Get the headers as NameValueCollection from the raw header string
        /// </summary>
        /// <param name="headers">The raw header string</param>
        /// <returns></returns>
        public static NameValueCollection GetHeaders(string headers)
        {
            NameValueCollection coll = new NameValueCollection();
            if (headers != null)
            {
                string[] lines = headers.Split(new char[] { '\r', '\n' },
                    StringSplitOptions.RemoveEmptyEntries);
                for (int i = 1; i < lines.Length; i++)
                {
                    int colon = lines[i].IndexOf(":");
                    string key = lines[i].Substring(0, colon);
                    string val = lines[i].Substring(colon + 1).Trim();
                    coll[key] = val;
                }
            }
            return coll;
        }
        /// <summary>
        /// Write as blocks
        /// </summary>
        /// <param name="stm">The stream to write to</param>
        /// <param name="data">The byte array containing the data</param>
        /// <param name="offset">The offset of data inside the byte array</param>
        /// <param name="length">The number of bytes to write</param>
        public static void WriteAsBlocks(Stream stm, byte[] data, int offset, int length)
        {
            const int BLOCK_SIZE = 16 * 1024;
            int remaining = length;
            do
            {
                int toWrite = BLOCK_SIZE;
                if (remaining < BLOCK_SIZE)
                {
                    toWrite = remaining;
                }
                remaining = remaining - toWrite;
                WriteChunked(stm, data, offset, toWrite);
                offset += toWrite;
            } while (remaining > 0);
        }
        /// <summary>
        /// Write as blocks
        /// </summary>
        /// <param name="stm">The stream to write to</param>
        /// <param name="data">The byte array containing the data</param>
        /// <param name="offset">The offset of data inside the byte array</param>
        /// <param name="length">The number of bytes to write</param>
        /// <param name="chunks">The number of chunks</param>
        public static void WriteAsBlocks(Stream stm, byte[] data, int offset, int length, int[] chunks)
        {
            if (null == chunks)
            {
                throw new ArgumentNullException();
            }
            const int BLOCK_SIZE = 16 * 1024;
            int remaining = length;
            int index = 0;
            do
            {
                int toWrite = BLOCK_SIZE;
                if (null != chunks && index < chunks.Length)
                {
                    toWrite = chunks[index];
                }
                if (remaining < toWrite)
                {
                    toWrite = remaining;
                }
                remaining = remaining - toWrite;
                WriteChunked(stm, data, offset, toWrite);
                offset += toWrite;
            } while (remaining > 0);
            WriteChunked(stm, null, 0, 0);
        }
        /// <summary>
        /// Writes chunked data to the stream
        /// </summary>
        /// <param name="stm">The stream to write to</param>
        /// <param name="data">The byte array containing the data</param>
        /// <param name="offset">The offset of data inside the byte array</param>
        /// <param name="length">The number of bytes to write</param>
        public static void WriteChunked(Stream stm, byte[] data, int offset, int length)
        {
            if (null == stm)
            {
                throw new ArgumentNullException();
            }
            byte[] CRLF = new byte[] { 13, 10 };
            byte[] dataSize;
            if (null != data)
            {
                string hex = length.ToString("x", CultureInfo.InvariantCulture);
                dataSize = Encoding.ASCII.GetBytes(hex);
                // Write Chunk
                stm.Write(dataSize, 0, dataSize.Length);
                stm.Write(CRLF, 0, 2);
                stm.Write(data, offset, length);
            }
            else
            {
                dataSize = Encoding.ASCII.GetBytes("0");
                // Write Empty chunk
                stm.Write(dataSize, 0, dataSize.Length);
                stm.Write(CRLF, 0, 2);
            }
            stm.Write(CRLF, 0, 2);
            stm.Flush();
        }
    }
}