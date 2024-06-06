using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    static void Main()
    {
        // Specify the IP address and port number to listen on
        IPAddress ipAddress = IPAddress.Parse("192.168.81.65"); // Change this to your desired IP address
        int port = 3000; // Change this to your desired port number

        // Create a TCP listener
        TcpListener listener = new TcpListener(ipAddress, port);

        try
        {
            // Start listening for incoming connections
            listener.Start();
            Console.WriteLine($"Listening for SOAP requests on {ipAddress}:{port}...");

            // Accept connections asynchronously
            while (true)
            {
                TcpClient client = listener.AcceptTcpClientAsync().Result;
                HandleClient(client);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
        }
        finally
        {
            // Stop listening for new connections
            listener.Stop();
        }
    }

    static void HandleClient(TcpClient client)
    {
        using (NetworkStream stream = client.GetStream())
        {
            StreamReader reader = new StreamReader(stream, Encoding.ASCII);

            // Read the HTTP request headers
            string requestHeaders = reader.ReadLine();
            Console.WriteLine($"Received request: {requestHeaders}");

            // Extract the URL path from the request headers
            string[] parts = requestHeaders.Split(' ');
            string url = parts[1];

            // Check if the request is for endpoint 1
            if (url.StartsWith("/exOSAEAChecker/Mfpsink.asmx"))
            {
                // Handle request for endpoint 1
                HandleEndpoint1Request(stream);
            }
            // Check if the request is for endpoint 2
            else if (url.StartsWith("/endpoint2"))
            {
                // Handle request for endpoint 2
                HandleEndpoint2Request(stream);
            }
            else
            {
                // Handle unknown request
                HandleUnknownRequest(stream);
            }
        }

        // Close the client connection
        // client.Close();
    }

    static void HandleEndpoint1Request(NetworkStream stream)
    {

        // Read the XML file
        string xmlFilePath = "reqLDAPAuth.xml"; // Replace this with the actual path to your XML file
        string soapResponse;
        string xmlContent = File.ReadAllText(xmlFilePath);

        // Craft the SOAP response using the XML content
        soapResponse = xmlContent;

        try
        {
            // Read the contents of the XML file
            // string xmlContent = File.ReadAllText(xmlFilePath);

            // // Craft the SOAP response using the XML content
            soapResponse = xmlContent;
            // //     soapResponse = $@"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
            // <soap:Body>
            //     {xmlContent}
            // </soap:Body>
            // </soap:Envelope>";
        }
        catch (Exception e)
        {
            // If there's an error reading the XML file, send an error SOAP response
            soapResponse = $@"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
            <soap:Body>
                <soap:Fault>
                    <faultcode>500</faultcode>
                    <faultstring>{e.Message}</faultstring>
                </soap:Fault>
            </soap:Body>
        </soap:Envelope>";
        }

        // Send the SOAP response
        byte[] responseBytes = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\nContent-Type: text/xml; charset=utf-8\r\n\r\n" + soapResponse);
        stream.Write(responseBytes, 0, responseBytes.Length);

    }

    static void HandleEndpoint2Request(NetworkStream stream)
    {
        // Handle request for endpoint 2
        // Craft and send SOAP response for endpoint 2
    }

    static void HandleUnknownRequest(NetworkStream stream)
    {
        // Handle unknown request
        // Send appropriate response or error message
    }
}
