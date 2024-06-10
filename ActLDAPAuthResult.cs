using System.Net.Sockets;
using System.Text;
using System.Xml;
using Utility;

namespace ExtendedApplications;

public class ActLDAPAuthResultClass
{
    //     // public void main()
    //     public async Task main(NetworkStream stream, XmlDocument xmlDoc, string SOAPAction)
    //     {

    //         switch(Helper.getSOAPActionVlaue(SOAPAction))
    //         {
    //             case "ActLDAPAuthResult":
    //                 {

    //                 }
    //             default:
    //         }
    //         if (Helper.getSOAPActionVlaue(SOAPAction) == "ActLDAPAuthResult")
    //         {

    //         }


    //     }


    public async Task ActLDAPAuthResult(NetworkStream stream, XmlDocument xmlDoc, string url)
    {
        // if (Helper.getSOAPActionVlaue(SOAPAction) == "ActLDAPAuthResult")
        // {
        bool retResult = true;
        string authResult = Helper.getXmlNodeValue(xmlDoc, "authResult");
        string authType = Helper.getXmlNodeValue(xmlDoc, "authType");
        string loginName = Helper.getXmlNodeValue(xmlDoc, "property[@sys-name='LoginName']");

        string customHeaders = Constants.ALL_HEADERS_WITH_DEFAULT;

        // Send the response
        string responseHeaders = Constants.RESPONSE_HEADER_HTTP_200_OK;

        if (authResult != "1")
        {
            string errMsg = (authResult == "0") ?
                    "LoginName or Password is incorrect." :
                    "Cannot connect to the LDAP Server.";
            Console.Write("[MfpSink.ActLDAPAuthResult] LDAP Authentication error : " + errMsg);
            Logger.Log("[MfpSink.ActLDAPAuthResult] LDAP Authentication error : " + errMsg);
            retResult = false;
        }
        if (retResult == true)
        {
            try
            {
                string xmlFilePath = "reqLDAPAuth.xml";
                string xmlContent = File.ReadAllText(xmlFilePath);

                // Add custom headers

                byte[] responseBytes = Encoding.UTF8.GetBytes(responseHeaders + xmlContent);
                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                Console.WriteLine("stream.Write ended");
            }

            catch (Exception e)
            {
                Console.WriteLine("Exception caught");

                // If there's an error reading the XML file, send an error SOAP response
                string soapResponse = $@"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                                        <soap:Body>
                                            <soap:Fault>
                                                <faultcode>500</faultcode>
                                                <faultstring>{e.Message}</faultstring>
                                            </soap:Fault>
                                        </soap:Body>
                                    </soap:Envelope>";

                responseHeaders = "HTTP/1.1 500 Internal Server Error\r\n" +
                    "Content-Type: text/xml; charset=utf-8\r\n\r\n";
                byte[] responseBytes = Encoding.UTF8.GetBytes(responseHeaders + soapResponse);
                stream.Write(responseBytes, 0, responseBytes.Length);
                Logger.LogResponse(responseHeaders, soapResponse);
            }
        }
        else
        {

            string xmlFilePath = "reqLDAPAuth_failed.xml";
            string xmlContent = File.ReadAllText(xmlFilePath);
            byte[] responseBytesErr = Encoding.UTF8.GetBytes(responseHeaders + xmlContent);
            await stream.WriteAsync(responseBytesErr, 0, responseBytesErr.Length);
            Logger.LogResponse(responseHeaders, xmlContent);
        }
    }


    // }
}