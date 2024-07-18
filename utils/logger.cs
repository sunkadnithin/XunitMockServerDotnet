using System;
using System.IO;

namespace MockServer.Utils;
public class Logger
{
    private static string _logFilePath = "./logs/logs.log";
    private static bool isLogFileCreated = false;


    public static bool CreateLogFile(string logFilePath)
    {
        try
        {
            if(!Directory.Exists("./logs"))
            {
                Directory.CreateDirectory("./logs");
            }
            if (!File.Exists(logFilePath))
            {
                File.Create(logFilePath);
                Console.WriteLine($"Log File is created at --- {Path.GetFullPath(logFilePath)}");
                isLogFileCreated = true;
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Log File is NOT created, catched Exception  --- {ex.Message} \n ");
            return false;
        }
        return true;
    }

    public static void Log(string message)
    {
        if (!isLogFileCreated)
        {
            isLogFileCreated = Logger.CreateLogFile(_logFilePath);
            if (isLogFileCreated != true)
            {
                Console.WriteLine($"Not Able to Log the below messsage {DateTime.Now: HH:mm:ss dd-MM-yyyy} - \n {message} \n\n");
                return;
            }
        }
        string logMessage = $"{DateTime.Now:HH:mm:ss dd-MM-yyyy} - \n {message}";
        streamWrite(logMessage);
    }

    public static void LogResponse(string resHeader, string resSOAP)
    {
        string logMessage = $"{DateTime.Now:HH:mm:ss dd-MM-yyyy} - \nResponse: \n {resHeader}\n {resSOAP}";
        streamWrite(logMessage);
    }

    public static void streamWrite(string logMessage)
    {
        try
        {
            using (StreamWriter writer = File.AppendText(_logFilePath))
            {
                writer.WriteLine(logMessage + Environment.NewLine);
            }
        }
        catch (Exception ex)
        {
            Logger.catchedException(ex, logMessage);
        }
    }

    // Create Empty Line
    public static void streamWrite()
    {
        try
        {
            using (StreamWriter writer = File.AppendText(_logFilePath))
            {
                writer.WriteLine(Environment.NewLine);
            }
        }
        catch (Exception ex)
        {
            Logger.catchedException(ex, "Not able to create EMPTY LINE");
        }
    }

    public static void catchedException(Exception ex, string logMessage)
    {
        Console.WriteLine($"Not Able to Log the below messsage {DateTime.Now: HH:mm:ss dd-MM-yyyy} - \n {logMessage} \n\n");
    }
}

