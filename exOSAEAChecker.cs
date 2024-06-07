// using System.Net.Sockets;
// using System.Text;
// using System.Xml;
// using Utility;

// namespace ExtendedApplications;

// class exOSAEAChecker
// {
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

//     public async Task ActLDAPAuthResultAsync(NetworkStream stream, XmlDocument xmlDoc)
//     {
//         bool retResult = true;
//             string authResult = Helper.getXmlNodeValue(xmlDoc, "authResult");
//             string authType = Helper.getXmlNodeValue(xmlDoc, "authType");
//             string loginName = Helper.getXmlNodeValue(xmlDoc, "property[@sys-name='LoginName']");

//             string xmlFilePath = "reqLDAPAuth.xml";
//             string xmlContent = File.ReadAllText(xmlFilePath);

//             if (authResult != "1")
//             {
//                 string errMsg = (authResult == "0") ?
//                         "LoginName or Password is incorrect." :
//                         "Cannot connect to the LDAP Server.";
//                 Console.Write("[MfpSink.ActLDAPAuthResult] LDAP Authentication error : " + errMsg);
//                 retResult = false;
//             }
            
//             if (retResult == true)
//                 try
//                 {

//                     // Add custom headers
//                     string customHeaders = "HTTP_CACHE_CONTROL: no-cache, max-age=0\r\n" +
//                                            "HTTP_CONTENT_LENGTH: 823\r\n" +
//                                            "HTTP_CONTENT_TYPE: text/xml; charset=utf-8\r\n" +
//                                            "HTTP_ACCEPT_ENCODING: gzip, deflate\r\n" +
//                                            "HTTP_HOST: 172.29.242.170\r\n" +
//                                            "HTTP_SOAPACTION: \"urn:schemas-sc-jp:mfp:osa-2-1/ActLDAPAuthResult\"\r\n" +
//                                            "ALL_HTTP: HTTP_CACHE_CONTROL:no-cache, max-age=0 HTTP_CONTENT_LENGTH:823 HTTP_CONTENT_TYPE:text/xml; charset=utf-8 HTTP_ACCEPT_ENCODING:gzip, deflate HTTP_HOST:172.29.242.170 HTTP_SOAPACTION:\"urn:schemas-sc-jp:mfp:osa-2-1/ActLDAPAuthResult\"\r\n" +
//                                            "ALL_RAW: Cache-Control: no-cache, max-age=0 Content-Length: 823 Content-Type: text/xml; charset=utf-8 Accept-Encoding: gzip, deflate Host: 172.29.242.170 SOAPAction: \"urn:schemas-sc-jp:mfp:osa-2-1/ActLDAPAuthResult\"\r\n" +
//                                            "APPL_MD_PATH: /LM/W3SVC/1/ROOT/eOSAEAChecker\r\n" +
//                                            "APPL_PHYSICAL_PATH: D:\\SSDI\\Projects\\OSA\\open-systems\\test\\eOSAEAChecker\\eOSAEAChecker\\\r\n" +
//                                            "CONTENT_LENGTH: 823\r\n" +
//                                            "CONTENT_TYPE: text/xml; charset=utf-8\r\n" +
//                                            "GATEWAY_INTERFACE: CGI/1.1\r\n" +
//                                            "HTTPS: off\r\n" +
//                                            "INSTANCE_ID: 1\r\n" +
//                                            "INSTANCE_META_PATH: /LM/W3SVC/1\r\n" +
//                                            "LOCAL_ADDR: 172.29.242.170\r\n" +
//                                            "PATH_INFO: /eOSAEAChecker/Mfpsink.asmx\r\n" +
//                                            "PATH_TRANSLATED: D:\\SSDI\\Projects\\OSA\\open-systems\\test\\eOSAEAChecker\\eOSAEAChecker\\Mfpsink.asmx\r\n" +
//                                            "REMOTE_ADDR: 192.168.81.65\r\n" +
//                                            "REMOTE_HOST: 192.168.81.65\r\n" +
//                                            "REMOTE_PORT: 34004\r\n" +
//                                            "REQUEST_METHOD: POST\r\n" +
//                                            "SCRIPT_NAME: /eOSAEAChecker/Mfpsink.asmx\r\n" +
//                                            "SERVER_NAME: 172.29.242.170\r\n" +
//                                            "SERVER_PORT: 80\r\n" +
//                                            "SERVER_PORT_SECURE: 0\r\n" +
//                                            "SERVER_PROTOCOL: HTTP/1.1\r\n" +
//                                            "SERVER_SOFTWARE: Microsoft-IIS/10.0\r\n" +
//                                            "URL: /eOSAEAChecker/Mfpsink.asmx";

//                     // Send the response
//                     string responseHeaders = "HTTP/1.1 200 OK\r\n" +
//                                              "Content-Type: text/xml; charset=utf-8\r\n\r\n";
//                     byte[] responseBytes = Encoding.UTF8.GetBytes(responseHeaders + xmlContent);
//                     await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
//                     Console.WriteLine("stream.Write ended");
//                 }

//                 catch (Exception e)
//                 {
//                     Console.WriteLine("Exception caught");

//                     // If there's an error reading the XML file, send an error SOAP response
//                     string soapResponse = $@"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
//                                         <soap:Body>
//                                             <soap:Fault>
//                                                 <faultcode>500</faultcode>
//                                                 <faultstring>{e.Message}</faultstring>
//                                             </soap:Fault>
//                                         </soap:Body>
//                                     </soap:Envelope>";

//                     string responseHeaders = "HTTP/1.1 500 Internal Server Error\r\n" +
//                         "Content-Type: text/xml; charset=utf-8\r\n\r\n";
//                     byte[] responseBytes = Encoding.UTF8.GetBytes(responseHeaders + soapResponse);
//                     stream.Write(responseBytes, 0, responseBytes.Length);
//                 }
//     }

// }