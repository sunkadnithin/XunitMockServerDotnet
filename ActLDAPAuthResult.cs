using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using Utility;

namespace ExtendedApplications;

public class ActLDAPAuthResultClass
{
    public async Task ActLDAPAuthResult(NetworkStream stream, XmlDocument xmlDoc, string url)
    {
        try
        {
            bool retResult = true;
            string authType = Helper.getXmlNodeValue(xmlDoc, "authType");
            string loginName = Helper.getXmlNodeValue(xmlDoc, "property[@sys-name='LoginName']");
            string authResult = Helper.getXmlNodeValue(xmlDoc, "authResult");
            string passWord = Helper.getXmlNodeValue(xmlDoc, "property[@sys-name='Password']");

            // Exception logins, This are used to send the xml, which are cannot cover in the normal execution
            // 11 - 15 loginNames are used in the simulator as default values for GROUP_TYPE
            // Remaining are used to send xml data which cannot be covered in normal execution  like
            // Server Side setting : System.Web.Configuration.WebConfigurationManager.AppSettings["ACT_AUTHENTICATERES_OMIT_ACL_ELEMENT"]
            List<string> exceptionLoginName = new List<string> { "11", "12", "13", "14", "15", "100", "101", "102", "103", "104", "105", "106", "107", "108", "1000" };

            Logger.Log($"ActLDAPAuthResult Input\n authResult : {authResult}\tauthType : {authType}\tloginName : {loginName} \tpassWord : {passWord}");

            switch (authType)
            {

                case "walk_up":
                    // Get the authResult and LoginName values from the Request
                    Logger.Log($"ActLDAPAuthResult Input\n authResult : {authResult}\tauthType : {authType}\tloginName : {loginName}");

                    if (authResult != null)
                    {
                        // 1 = Credentials correct
                        // 0 = Credentials incorrect
                        // -1 = Wrong Server
                        if (authResult != "1")
                        {
                            string errMsg = (authResult == "0") ? "LoginName or Password is incorrect." : "Cannot connect to the LDAP Server.";
                            Console.Write("[MfpSink.ActLDAPAuthResult] LDAP Authentication error : " + errMsg);
                            Logger.Log("[MfpSink.ActLDAPAuthResult] LDAP Authentication error : " + errMsg);
                            retResult = false;
                        }
                        if (exceptionLoginName.Contains(loginName))
                        {
                            await ActLDAPAuthResult_exceptionLoginNames(stream, loginName);
                        }
                         else if (retResult == false)
                        {
                            string xmlFilePath = "ActLDAPAuthResultResponse_failed.xml";
                            await Helper.Send200_ReadXmlFromFileAsync(stream, xmlFilePath);
                        }
                        else if (retResult == true)
                        {
                            try
                            {
                                string xmlFilePath = Constants.TESTDATA_PATH_ActLDAPAuthResultResponse + "/ActLDAPAuthResultResponse_true.xml";
                                await Helper.Send200_ReadXmlFromFileAsync(stream, xmlFilePath);
                            }

                            catch (Exception ex)
                            {
                                Helper.Send500InternalServerErr(stream, ex.Message);
                            }
                        }

                        else
                        {
                            Helper.Send500InternalServerErr(stream, "Unkown Error");

                        }
                    }
                    else
                    {
                        // get password from the request xml
                        Logger.Log($"ActLDAPAuthResult Input\n authResult : {authResult}\tauthType : {authType}\tloginName : {loginName}\t passWord : {passWord}");

                        // ACL Group Types
                        switch (loginName)
                        {
                            case "10":
                                retResult = (passWord == "10") ? true : false;
                                break;

                            case "11":
                                retResult = (passWord == "11") ? true : false;
                                break;

                            case "12":
                                retResult = (passWord == "12") ? true : false;
                                break;


                            case "13":
                                retResult = (passWord == "13") ? true : false;
                                break;


                            case "14":
                                retResult = (passWord == "14") ? true : false;
                                break;

                            default:
                                retResult = false;
                                break;
                        }

                        if (retResult)
                        {
                            await Helper.Send200_ReadXmlFromFileAsync(stream, Constants.TESTDATA_PATH_ActLDAPAuthResultResponse + $"/ActLDAPAuthResultResponse_{loginName}_true");

                        }
                        else
                        {
                            await Helper.Send200_ReadXmlFromFileAsync(stream, Constants.TESTDATA_PATH_ActLDAPAuthResultResponse + "/ActLDAPAuthResultResponse_failed.xml");
                        }
                    }
                    break;
                // ----------------------- walk_up Ended---------------------------------------

                case "card_hid":
                    Helper.Send500InternalServerErr(stream, "authType : card_hid --- Not Implemented Yet");
                    break;

                default:
                    Helper.Send500InternalServerErr(stream, "authType Not Found");
                    return;

            }
        }
        catch (Exception ex)
        {
            {
                Helper.Send500InternalServerErr(stream, ex.Message);

            }
        }
    }
 

    public async Task ActLDAPAuthResult_exceptionLoginNames(NetworkStream stream, string loginName )
    {
        switch (loginName)
        {
            case "11":
                await Helper.Send200_ReadXmlFromFileAsync(stream, Constants.TESTDATA_PATH_ActLDAPAuthResultResponse + $"/ActLDAPAuthResultResponse_{loginName}_true.xml");
                break ;
            
            case "12":
                await Helper.Send200_ReadXmlFromFileAsync(stream, Constants.TESTDATA_PATH_ActLDAPAuthResultResponse + $"/ActLDAPAuthResultResponse_{loginName}_true.xml");
                break ;
            
            case "13":
                await Helper.Send200_ReadXmlFromFileAsync(stream, Constants.TESTDATA_PATH_ActLDAPAuthResultResponse + $"/ActLDAPAuthResultResponse_{loginName}_true.xml");
                break ;
            
            case "14":
                await Helper.Send200_ReadXmlFromFileAsync(stream, Constants.TESTDATA_PATH_ActLDAPAuthResultResponse + $"/ActLDAPAuthResultResponse_{loginName}_true.xml");
                break ;
                
            case "100":
                await Helper.Send200_ReadXmlFromFileAsync(stream, Constants.TESTDATA_PATH_ActLDAPAuthResultResponse + "/ActLDAPAuthResultResponse_100_ACL_TYPE_CUSTOM.xml");
                break ;
            case "101":
                await Helper.Send200_ReadXmlFromFileAsync(stream, Constants.TESTDATA_PATH_ActLDAPAuthResultResponse + "/ActLDAPAuthResultResponse_101_ACL_GROUP.xml");
                break ;
            case "102":
                await Helper.Send200_ReadXmlFromFileAsync(stream, Constants.TESTDATA_PATH_ActLDAPAuthResultResponse + "/ActLDAPAuthResultResponse_103_LCL_CUSTOM.xml");
                break ;
            case "103":
                await Helper.Send200_ReadXmlFromFileAsync(stream, Constants.TESTDATA_PATH_ActLDAPAuthResultResponse + "/ActLDAPAuthResultResponse_103_LCL_CUSTOM.xml");
                break ;
            
            case "104":
                await Helper.Send200_ReadXmlFromFileAsync(stream, Constants.TESTDATA_PATH_ActLDAPAuthResultResponse + "/ActLDAPAuthResultResponse_104_LCL_GROUP.xml");
                break ;
            
            case "105":
                await Helper.Send200_ReadXmlFromFileAsync(stream, Constants.TESTDATA_PATH_ActLDAPAuthResultResponse + "/ActLDAPAuthResultResponse_105_SCREEN_APP_TYPE.xml");
                break ;
            
            case "106":
                await Helper.Send200_ReadXmlFromFileAsync(stream, Constants.TESTDATA_PATH_ActLDAPAuthResultResponse + "/ActLDAPAuthResultResponse_106_SCREEN_TYPE.xml");
                break ;
            
            case "107":
                await Helper.Send200_ReadXmlFromFileAsync(stream, Constants.TESTDATA_PATH_ActLDAPAuthResultResponse + "/ActLDAPAuthResultResponse_107_SCREEN_TYPE_NULL_APP_ADDRESS.xml");
                break ;
            
            case "108":
                await Helper.Send200_ReadXmlFromFileAsync(stream, Constants.TESTDATA_PATH_ActLDAPAuthResultResponse + "/ActLDAPAuthResultResponse_108_SCREEN_TYPE_MAINMODE.xml");
                break ;
            
            case "1000":
                await Helper.Send200_ReadXmlFromFileAsync(stream, Constants.TESTDATA_PATH_ActLDAPAuthResultResponse + "/ActLDAPAuthResultResponse_1000_USERINFO_ACL_LCL_NULL.xml");
                break ;
        }
    }


}