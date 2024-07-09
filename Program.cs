
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using ExtendedApplications;
using Utility;
class Program
{
    static void Main()
    {
        // string ip = Helper.GetLocalIPv4Address(); // to get the Actual IP Address
        string ip = "127.0.0.1"; // To Run in localhost
        IPAddress ipAddress = IPAddress.Parse(ip);
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
                Helper.Send500InternalServerErr(stream, "Not able to save the Request XML");
            }

            // Extract the URL path from the request headers
            string[] parts = requestHeaders.Split(' ');
            string url = parts.Length > 1 ? parts[1] : "";

            string api = Helper.getSOAPActionVlaue(SOAPAction);
            switch (api)
            {
                case "ActLDAPAuthResult":
                    {
                        ActLDAPAuthResultClass actLDAP = new ActLDAPAuthResultClass();
                        actLDAP.ActLDAPAuthResult(stream, xmlDoc, url);
                        break;
                    }
                case "ActADAuthResult":
                    {
                        ActADAuthResultClass actLDAP = new();
                        await actLDAP.ActADAuthResult(stream, xmlDoc, url);
                        break;
                    }
                case "ActAuthenticate":
                    {
                        ActAuthenticateResponseClass actAuthenticate = new ActAuthenticateResponseClass();
                        actAuthenticate.ActAuthenticateResponse(stream, xmlDoc, url);
                        break;
                    }
                default:
                    {
                        Logger.Log($"API {api} not able to find in the mockServer");
                        Helper.Send500InternalServerErr(stream, "API {api} not able to find in the mockServer");
                        break;
                    }
            }
        }
        // Close the client connection
        client.Close();
    }
}
