
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ExtendedApplications;
using Utility;
class Program
{
    static void Main()
    {
        string ip = "192.168.81.65";
        // ToDo -- Not working when 
        // Specify the IP address and port number to listen on
        IPAddress ipAddress = IPAddress.Parse(ip); // Change this to your desired IP address
        int port = 3000; // Change this to your desired port number

        // Create a TCP listener
        TcpListener listener = new TcpListener(ipAddress, port);

        try
        {
            // Start listening for incoming connections
            listener.Start();
            Console.WriteLine($"Listening for SOAP requests on {ipAddress}:{port}...");
            Logger.Log($"Listening for SOAP requests on {ipAddress}:{port}...");

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

    static async void HandleClient(TcpClient client)
    {
        using (NetworkStream stream = client.GetStream())
        {
            StreamReader reader = new StreamReader(stream, Encoding.ASCII);
            // Read the HTTP request headers
            string requestHeaders = await reader.ReadLineAsync();
            string host = await reader.ReadLineAsync();
            string cacheControl = await reader.ReadLineAsync();
            string SOAPAction = await reader.ReadLineAsync();
            string acceptEncoding = await reader.ReadLineAsync();
            string contentType = await reader.ReadLineAsync();
            string contentLength = await reader.ReadLineAsync();
            string extraLine = await reader.ReadLineAsync();
            int contentLengthInt = Utility.Helper.GetContentLength(contentLength);
            string xmlData = await Utility.Helper.ReadByteDataAsync(reader, 0, contentLengthInt);
            Console.WriteLine($"Received request: from {host} {requestHeaders} -- {SOAPAction}");
            Logger.Log($"{requestHeaders}\n{host}\n{cacheControl}\n{SOAPAction}\n{acceptEncoding}\n{contentType}\n {contentLength}\n XML Data: \n {xmlData}");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlData);
            if (!Helper.saveXml(xmlDoc))
            {
                // Handle XML parsing errors
                string soapResponse = $@"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                <soap:Body>
                    <soap:Fault>
                        <faultcode>500</faultcode>
                        <faultstring>{"Not able to save the xml"}</faultstring>
                    </soap:Fault>
                    </soap:Body>
                </soap:Envelope>";

                string responseHeadersErr = "HTTP/1.1 500 Internal Server Error\r\n" +
                    "Content-Type: text/xml; charset=utf-8\r\n\r\n";
                byte[] responseBytesErr = Encoding.UTF8.GetBytes(responseHeadersErr + soapResponse);
                await stream.WriteAsync(responseBytesErr, 0, responseBytesErr.Length);
                Logger.LogResponse(responseHeadersErr, soapResponse);
            }



            // Extract the URL path from the request headers
            string[] parts = requestHeaders.Split(' ');
            string url = parts.Length > 1 ? parts[1] : "";

            // Check if the request is for endpoint ActLDAPAuthResult
            if (Helper.getSOAPActionVlaue(SOAPAction) == "ActLDAPAuthResult")
            {
                ActLDAPAuthResultClass actLDAP = new ActLDAPAuthResultClass();
                // Handle request for endpoint exOSAEAChecker
                actLDAP.ActLDAPAuthResult(stream, xmlDoc, url);
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



    // static async Task ActLDAPAuthResult(NetworkStream stream, XmlDocument xmlDoc, string url)
    // {
    //     // if (Helper.getSOAPActionVlaue(SOAPAction) == "ActLDAPAuthResult")
    //     // {
    //         bool retResult = true;
    //         string authResult = Helper.getXmlNodeValue(xmlDoc, "authResult");
    //         string authType = Helper.getXmlNodeValue(xmlDoc, "authType");
    //         string loginName = Helper.getXmlNodeValue(xmlDoc, "property[@sys-name='LoginName']");

    //         string customHeaders = Constants.ALL_HEADERS_WITH_DEFAULT;

    //         // Send the response
    //         string responseHeaders = Constants.RESPONSE_HEADER_HTTP_200_OK;

    //         if (authResult != "1")
    //         {
    //             string errMsg = (authResult == "0") ?
    //                     "LoginName or Password is incorrect." :
    //                     "Cannot connect to the LDAP Server.";
    //             Console.Write("[MfpSink.ActLDAPAuthResult] LDAP Authentication error : " + errMsg);
    //             retResult = false;
    //         }
    //         if (retResult == true)
    //         {
    //             try
    //             {
    //                 string xmlFilePath = "reqLDAPAuth.xml";
    //                 string xmlContent = File.ReadAllText(xmlFilePath);

    //                 // Add custom headers

    //                 byte[] responseBytes = Encoding.UTF8.GetBytes(responseHeaders + xmlContent);
    //                 await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
    //                 Console.WriteLine("stream.Write ended");
    //             }

    //             catch (Exception e)
    //             {
    //                 Console.WriteLine("Exception caught");

    //                 // If there's an error reading the XML file, send an error SOAP response
    //                 string soapResponse = $@"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
    //                                     <soap:Body>
    //                                         <soap:Fault>
    //                                             <faultcode>500</faultcode>
    //                                             <faultstring>{e.Message}</faultstring>
    //                                         </soap:Fault>
    //                                     </soap:Body>
    //                                 </soap:Envelope>";

    //                 responseHeaders = "HTTP/1.1 500 Internal Server Error\r\n" +
    //                     "Content-Type: text/xml; charset=utf-8\r\n\r\n";
    //                 byte[] responseBytes = Encoding.UTF8.GetBytes(responseHeaders + soapResponse);
    //                 stream.Write(responseBytes, 0, responseBytes.Length);
    //                 Logger.LogResponse(responseHeaders, soapResponse);
    //             }
    //         }
    //         else
    //         {

    //             string xmlFilePath = "reqLDAPAuth_failed.xml";
    //             string xmlContent = File.ReadAllText(xmlFilePath);
    //             byte[] responseBytesErr = Encoding.UTF8.GetBytes(responseHeaders + xmlContent);
    //             await stream.WriteAsync(responseBytesErr, 0, responseBytesErr.Length);
    //         }
    //     }


    // }

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
        Logger.LogResponse(responseHeaders, soapResponse);

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
        Logger.LogResponse(responseHeaders, soapResponse);
    }
}
