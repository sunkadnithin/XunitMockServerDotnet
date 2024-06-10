namespace Utility;

public class Constants
{

    // Update Might be required for other API's and differnet machines (PATH)
    public static string ALL_HEADERS_WITH_DEFAULT = "HTTP_CACHE_CONTROL: no-cache, max-age=0\r\n" +
                       "HTTP_CONTENT_LENGTH: 823\r\n" +
                       "HTTP_CONTENT_TYPE: text/xml; charset=utf-8\r\n" +
                       "HTTP_ACCEPT_ENCODING: gzip, deflate\r\n" +
                       "HTTP_HOST: 172.29.242.170\r\n" +
                       "HTTP_SOAPACTION: \"urn:schemas-sc-jp:mfp:osa-2-1/ActLDAPAuthResult\"\r\n" +
                       "ALL_HTTP: HTTP_CACHE_CONTROL:no-cache, max-age=0 HTTP_CONTENT_LENGTH:823 HTTP_CONTENT_TYPE:text/xml; charset=utf-8 HTTP_ACCEPT_ENCODING:gzip, deflate HTTP_HOST:172.29.242.170 HTTP_SOAPACTION:\"urn:schemas-sc-jp:mfp:osa-2-1/ActLDAPAuthResult\"\r\n" + // Check This one
                       "ALL_RAW: Cache-Control: no-cache, max-age=0 Content-Length: 823 Content-Type: text/xml; charset=utf-8 Accept-Encoding: gzip, deflate Host: 172.29.242.170 SOAPAction: \"urn:schemas-sc-jp:mfp:osa-2-1/ActLDAPAuthResult\"\r\n" + // Check This one
                       "APPL_MD_PATH: /LM/W3SVC/1/ROOT/eOSAEAChecker\r\n" +
                       "APPL_PHYSICAL_PATH: D:\\SSDI\\Projects\\OSA\\open-systems\\test\\eOSAEAChecker\\eOSAEAChecker\\\r\n" + // Check This one
                       "CONTENT_LENGTH: 823\r\n" +
                       "CONTENT_TYPE: text/xml; charset=utf-8\r\n" +
                       "GATEWAY_INTERFACE: CGI/1.1\r\n" +
                       "HTTPS: off\r\n" +
                       "INSTANCE_ID: 1\r\n" +
                       "INSTANCE_META_PATH: /LM/W3SVC/1\r\n" +
                       "LOCAL_ADDR: 172.29.242.170\r\n" +
                       "PATH_INFO: /eOSAEAChecker/Mfpsink.asmx\r\n" +
                       "PATH_TRANSLATED: D:\\SSDI\\Projects\\OSA\\open-systems\\test\\eOSAEAChecker\\eOSAEAChecker\\Mfpsink.asmx\r\n" + // Check This one
                       "REMOTE_ADDR: 192.168.81.65\r\n" +
                       "REMOTE_HOST: 192.168.81.65\r\n" +
                       "REMOTE_PORT: 34004\r\n" +
                       "REQUEST_METHOD: POST\r\n" +
                       "SCRIPT_NAME: /eOSAEAChecker/Mfpsink.asmx\r\n" + // Check This one
                       "SERVER_NAME: 172.29.242.170\r\n" +
                       "SERVER_PORT: 80\r\n" +
                       "SERVER_PORT_SECURE: 0\r\n" +
                       "SERVER_PROTOCOL: HTTP/1.1\r\n" +
                       "SERVER_SOFTWARE: Microsoft-IIS/10.0\r\n" +
                       "URL: /eOSAEAChecker/Mfpsink.asmx"; // Check This one

    public static string RESPONSE_HEADER_HTTP_200_OK = "HTTP/1.1 200 OK\r\n" +
                                     "Content-Type: text/xml; charset=utf-8\r\n\r\n";
}
