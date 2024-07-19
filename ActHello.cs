using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using MockServer.Utils;

namespace MockServer
{
    public class ActHelloClass
    {
        string modifiedXml = String.Empty;

        public async Task ActHelloResponse(NetworkStream stream, XmlDocument xmlDoc, string url)
        {
            XmlNode extensionsNode = Helper.getXmlNode(xmlDoc, "extensions");

            XmlNode setExtensionsNode = xmlDoc.CreateElement("setExtensions");

            if (extensionsNode != null)
            {
                foreach (XmlNode authExtensionNode in extensionsNode.ChildNodes)
                {
                    XmlElement newAuthExtensionNode = xmlDoc.CreateElement("authExtension");

                    XmlElement dataTypeNode = xmlDoc.CreateElement("dataType");
                    dataTypeNode.InnerText = authExtensionNode["dataType"]?.InnerText;

                    if (dataTypeNode.InnerText == "ldap" && url == "/CloudEA/Mfpsink.asmx")
                    {
                        XmlNode metadataNode = authExtensionNode["metadata"];
                        if (metadataNode != null)
                        {
                            XmlElement newMetadataNode = xmlDoc.CreateElement("metadata");
                            foreach (XmlNode propertyNode in metadataNode.ChildNodes)
                            {
                                if (propertyNode.InnerText == "sample1")
                                {
                                    XmlElement lserverPropertyNode = xmlDoc.CreateElement("property");
                                    lserverPropertyNode.SetAttribute("sys-name", "lserver");
                                    lserverPropertyNode.InnerText = propertyNode.InnerText;
                                    newMetadataNode.AppendChild(lserverPropertyNode);

                                    XmlElement timeoutPropertyNode = xmlDoc.CreateElement("property");
                                    timeoutPropertyNode.SetAttribute("sys-name", "timeout");
                                    timeoutPropertyNode.InnerText = "60";
                                    newMetadataNode.AppendChild(timeoutPropertyNode);
                                }
                            }
                            newAuthExtensionNode.AppendChild(dataTypeNode);
                            newAuthExtensionNode.AppendChild(newMetadataNode);
                        }
                    }
                    else
                    {
                        newAuthExtensionNode.AppendChild(dataTypeNode);
                    }

                    setExtensionsNode.AppendChild(newAuthExtensionNode);
                }
            }

            XmlDocument responseXmlDoc = new XmlDocument();
            string responseXmlString = @"<?xml version='1.0' encoding='utf-8'?>
                                <soap:Envelope xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
                                    <soap:Header>
                                        <HeaderActHelloRes xmlns='urn:schemas-sc-jp:mfp:osa-2-1'>
                                            <vkey>9IzTHMjuyoIgHMpOglmXRskrdDF8uz9PXncJgezGf/QC9tWpZllhVO8IsTMA013O79qTRTL0+LK6NqO7L8bftvXwDZl9t7RDEskcCOqvQizDbTk2MYBkLvk9yI6hORK0</vkey>
                                        </HeaderActHelloRes>
                                    </soap:Header>
                                    <soap:Body>
                                        <ActHelloResponse xmlns='urn:schemas-sc-jp:mfp:osa-2-1'>
                                            <operation>
                                                <complex sys-name='EVENT'>
                                                    <property sys-name='completedUrl'>http://172.29.241.99/CloudEA/MfpSink.asmx</property>
                                                    <property sys-name='logoutUrl'>http://172.29.241.99/CloudEA/MfpSink.asmx</property>
                                                </complex>
                                            </operation>
                                            <setExtensions>
                                                <authExtension>
                                                    <dataType>offline_support</dataType>
                                                </authExtension>
                                                <authExtension>
                                                    <dataType>card_hid</dataType>
                                                </authExtension>
                                                <authExtension>
                                                    <dataType>ldap</dataType>
                                                    <metadata>
                                                        <property sys-name='lserver'>sample1</property>
                                                        <property sys-name='timeout'>60</property>
                                                    </metadata>
                                                </authExtension>
                                                <authExtension>
                                                    <dataType>active_directory</dataType>
                                                </authExtension>
                                            </setExtensions>
                                        </ActHelloResponse>
                                    </soap:Body>
                                </soap:Envelope>";

            responseXmlDoc.LoadXml(responseXmlString);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(responseXmlDoc.NameTable);
            nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("ns", "urn:schemas-sc-jp:mfp:osa-2-1");
            try
            {

                XmlNode oldSetExtensionsNode = responseXmlDoc.SelectSingleNode("//ns:setExtensions", nsmgr);
                if (oldSetExtensionsNode != null)
                {
                    oldSetExtensionsNode.ParentNode.ReplaceChild(responseXmlDoc.ImportNode(setExtensionsNode, true), oldSetExtensionsNode);
                }

                using (StringWriter writer = new StringWriter())
                {
                    responseXmlDoc.Save(writer);
                    modifiedXml = writer.ToString();
                    Console.WriteLine(modifiedXml);
                }

                Helper.saveXml(responseXmlDoc);

// modifiedXml : value


// <?xml version="1.0" encoding="utf-16"?>
// <soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
//   <soap:Header>
//     <HeaderActHelloRes xmlns="urn:schemas-sc-jp:mfp:osa-2-1">
//       <vkey>9IzTHMjuyoIgHMpOglmXRskrdDF8uz9PXncJgezGf/QC9tWpZllhVO8IsTMA013O79qTRTL0+LK6NqO7L8bftvXwDZl9t7RDEskcCOqvQizDbTk2MYBkLvk9yI6hORK0</vkey>
//     </HeaderActHelloRes>
//   </soap:Header>
//   <soap:Body>
//     <ActHelloResponse xmlns="urn:schemas-sc-jp:mfp:osa-2-1">
//       <operation>
//         <complex sys-name="EVENT">
//           <property sys-name="completedUrl">http://172.29.241.99/CloudEA/MfpSink.asmx</property>
//           <property sys-name="logoutUrl">http://172.29.241.99/CloudEA/MfpSink.asmx</property>
//         </complex>
//       </operation>
//       <setExtensions xmlns="">                                                                                                   // this should be <setExtensions>
//         <authExtension>
//           <dataType>billing_code</dataType>
//         </authExtension>
//         <authExtension>
//           <dataType>ldap</dataType>
//           <metadata>
//             <property sys-name="lserver">sample1</property>
//             <property sys-name="timeout">60</property>
//           </metadata>
//         </authExtension>
//       </setExtensions>
//     </ActHelloResponse>
//   </soap:Body>
// </soap:Envelope>

                Helper.Send200_StringXml(stream, modifiedXml);
            }
            catch (Exception ex)
            {
                Helper.Send500InternalServerErr(stream, ex.Message);
            }
        }
    }
}
