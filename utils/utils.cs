


using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Xml;

namespace Utility;

class Helper
{

    public static int GetContentLength(string headers)
    {
        string[] data = headers.Split(" ");
        bool flag = int.TryParse(data[1], out int parsedValue);
        if (flag)
        {
            return parsedValue;
        }
        return 0; // Default value if Content-Length header not found or parsing failed
    }

    public static async Task<string> ReadByteDataAsync(StreamReader reader, int startPoint, int bufferSize)
    {

        char[] buffer = new char[bufferSize];
        int bytesRead = await reader.ReadAsync(buffer, 0, bufferSize);
        return new string(buffer, 0, bytesRead); ;
    }


    public static XmlNode getXmlNode(XmlDocument xmlDoc, string name)
    {

        XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
        nsmgr.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
        nsmgr.AddNamespace("mfp", "urn:schemas-sc-jp:mfp:osa-2-1");

        XmlNode node = xmlDoc.SelectSingleNode("//mfp:" + name, nsmgr);
        return node;
        //example
        // XmlNode authTypeNode = xmlDoc.SelectSingleNode("//mfp:authType", nsmgr);
        // XmlNode loginNameNode = xmlDoc.SelectSingleNode("//mfp:property[@sys-name='LoginName']", nsmgr);
    }

    public static string getXmlNodeValue(XmlDocument xmlDoc, string name)
    {
        XmlNode node = Helper.getXmlNode(xmlDoc, name);
        string value = null;
        // string value = string.Empty;
        if (node != null)
        {
            value = node.InnerText;
        }
        return value;
    }

    public static string getSOAPActionVlaue(string soapAction)
    {
        // Extract the actual URL part
        string url = soapAction.Split('"')[1];

        // Get the last part after the last '/'
        string soapActionValue = url.Substring(url.LastIndexOf('/') + 1);

        Console.WriteLine(soapActionValue);
        return soapActionValue;
    }

    public static bool saveXml(XmlDocument xmlDoc)
    {
        try
        {
            // Save the Last XML Request
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "lastRequest.xml");
            xmlDoc.Save(filePath);
            return true;
        }
        catch (XmlException ex)
        {
            return false;
        }
    }

    public static string GetLocalIPv4Address()
    {
        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.OperationalStatus == OperationalStatus.Up &&
                ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            {
                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.Address.ToString();
                    }
                }
            }
        }
        Logger.Log("Not able get the Ip address, Running on localHost");
        return "localhost"; // Return null if no IPv4 address is found
    }

    public static void Send500InternalServerErr(NetworkStream stream, string exMessage)
    {
        Console.WriteLine($"Exception caught ---- {exMessage}");
        string soapResponse = $@"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                                        <soap:Body>
                                            <soap:Fault>
                                                <faultcode>{500}</faultcode>
                                                <faultstring>{exMessage}</faultstring>
                                            </soap:Fault>
                                        </soap:Body>
                                    </soap:Envelope>";

        string responseHeaders = Constants.RESPONSE_HEADER_HTTP_500_INTERNAL_SERVER_ERR;
        byte[] responseBytes = Encoding.UTF8.GetBytes(responseHeaders + soapResponse);
        stream.Write(responseBytes, 0, responseBytes.Length);
        Logger.LogResponse(responseHeaders, soapResponse);
    }

    public static async Task Send200_ReadXmlFromFileAsync(NetworkStream stream, string filePath)
    {
        string xmlFilePath = filePath;
        string xmlContent = File.ReadAllText(xmlFilePath);
        byte[] responseBytes = Encoding.UTF8.GetBytes(Constants.RESPONSE_HEADER_HTTP_200_OK + xmlContent);
        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
        Console.WriteLine("stream.Write ended");
        Logger.LogResponse(Constants.RESPONSE_HEADER_HTTP_200_OK, xmlContent);
    }
}

