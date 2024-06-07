
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
            Console.WriteLine("Listener stopped");
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
            string url = parts.Length > 1 ? parts[1] : "";

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
        client.Close();
    }

    static void HandleEndpoint1Request(NetworkStream stream)
    {
        // Read the XML file
        string xmlFilePath = "reqLDAPAuth.xml"; // Replace this with the actual path to your XML file
        string soapResponse;
        string xmlContent = File.ReadAllText(xmlFilePath);
        Console.WriteLine("Reading XML from file");

        // Craft the SOAP response using the XML content
        soapResponse = xmlContent;

        try
        {
            soapResponse = xmlContent;

            // Add custom headers
            string customHeaders = "HTTP_CACHE_CONTROL: no-cache, max-age=0\r\n" +
                                   "HTTP_CONTENT_LENGTH: 823\r\n" +
                                   "HTTP_CONTENT_TYPE: text/xml; charset=utf-8\r\n" +
                                   "HTTP_ACCEPT_ENCODING: gzip, deflate\r\n" +
                                   "HTTP_HOST: 172.29.242.170\r\n" +
                                   "HTTP_SOAPACTION: \"urn:schemas-sc-jp:mfp:osa-2-1/ActLDAPAuthResult\"\r\n" +
                                   "ALL_HTTP: HTTP_CACHE_CONTROL:no-cache, max-age=0 HTTP_CONTENT_LENGTH:823 HTTP_CONTENT_TYPE:text/xml; charset=utf-8 HTTP_ACCEPT_ENCODING:gzip, deflate HTTP_HOST:172.29.242.170 HTTP_SOAPACTION:\"urn:schemas-sc-jp:mfp:osa-2-1/ActLDAPAuthResult\"\r\n" +
                                   "ALL_RAW: Cache-Control: no-cache, max-age=0 Content-Length: 823 Content-Type: text/xml; charset=utf-8 Accept-Encoding: gzip, deflate Host: 172.29.242.170 SOAPAction: \"urn:schemas-sc-jp:mfp:osa-2-1/ActLDAPAuthResult\"\r\n" +
                                   "APPL_MD_PATH: /LM/W3SVC/1/ROOT/eOSAEAChecker\r\n" +
                                   "APPL_PHYSICAL_PATH: D:\\SSDI\\Projects\\OSA\\open-systems\\test\\eOSAEAChecker\\eOSAEAChecker\\\r\n" +
                                   "CONTENT_LENGTH: 823\r\n" +
                                   "CONTENT_TYPE: text/xml; charset=utf-8\r\n" +
                                   "GATEWAY_INTERFACE: CGI/1.1\r\n" +
                                   "HTTPS: off\r\n" +
                                   "INSTANCE_ID: 1\r\n" +
                                   "INSTANCE_META_PATH: /LM/W3SVC/1\r\n" +
                                   "LOCAL_ADDR: 172.29.242.170\r\n" +
                                   "PATH_INFO: /eOSAEAChecker/Mfpsink.asmx\r\n" +
                                   "PATH_TRANSLATED: D:\\SSDI\\Projects\\OSA\\open-systems\\test\\eOSAEAChecker\\eOSAEAChecker\\Mfpsink.asmx\r\n" +
                                   "REMOTE_ADDR: 192.168.81.65\r\n" +
                                   "REMOTE_HOST: 192.168.81.65\r\n" +
                                   "REMOTE_PORT: 34004\r\n" +
                                   "REQUEST_METHOD: POST\r\n" +
                                   "SCRIPT_NAME: /eOSAEAChecker/Mfpsink.asmx\r\n" +
                                   "SERVER_NAME: 172.29.242.170\r\n" +
                                   "SERVER_PORT: 80\r\n" +
                                   "SERVER_PORT_SECURE: 0\r\n" +
                                   "SERVER_PROTOCOL: HTTP/1.1\r\n" +
                                   "SERVER_SOFTWARE: Microsoft-IIS/10.0\r\n" +
                                   "URL: /eOSAEAChecker/Mfpsink.asmx";

            // Send the SOAP response with headers
            string responseHeaders = "HTTP/1.1 200 OK\r\n" +
                                     "Content-Type: text/xml; charset=utf-8\r\n" +
                                     customHeaders + "\r\n\r\n";
            byte[] responseBytes = Encoding.UTF8.GetBytes(responseHeaders + soapResponse);
            // byte[] responseBytes = Encoding.UTF8.GetBytes(soapResponse);
            stream.Write(responseBytes, 0, responseBytes.Length);
            Console.WriteLine("stream.Write ended");
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception caught");

            // If there's an error reading the XML file, send an error SOAP response
            soapResponse = $@"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
            <soap:Body>
                <soap:Fault>
                    <faultcode>500</faultcode>
                    <faultstring>{e.Message}</faultstring>
                </soap:Fault>
            </soap:Body>
        </soap:Envelope>";

            string responseHeaders = "HTTP/1.1 500 Internal Server Error\r\n" +
                "Content-Type: text/xml; charset=utf-8\r\n\r\n";
            byte[] responseBytes = Encoding.UTF8.GetBytes(responseHeaders + soapResponse);
            stream.Write(responseBytes, 0, responseBytes.Length);
        }
    }

    static void HandleEndpoint2Request(NetworkStream stream)
    {
        // Handle request for endpoint 2
        // Craft and send SOAP response for endpoint 2
        string soapResponse = @"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
            <soap:Body>
                <Response>Endpoint 2 Response</Response>
            </soap:Body>
        </soap:Envelope>";

        string responseHeaders = "HTTP/1.1 200 OK\r\n" +
            "Content-Type: text/xml; charset=utf-8\r\n\r\n";
        byte[] responseBytes = Encoding.UTF8.GetBytes(responseHeaders + soapResponse);
        stream.Write(responseBytes, 0, responseBytes.Length);
    }

    static void HandleUnknownRequest(NetworkStream stream)
    {
        // Handle unknown request
        // Send appropriate response or error message
        string soapResponse = @"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
            <soap:Body>
                <soap:Fault>
                    <faultcode>400</faultcode>
                    <faultstring>Bad Request</faultstring>
                </soap:Fault>
            </soap:Body>
        </soap:Envelope>";

        string responseHeaders = "HTTP/1.1 400 Bad Request\r\n" +
            "Content-Type: text/xml; charset=utf-8\r\n\r\n";
        byte[] responseBytes = Encoding.UTF8.GetBytes(responseHeaders + soapResponse);
        stream.Write(responseBytes, 0, responseBytes.Length);
    }
}
