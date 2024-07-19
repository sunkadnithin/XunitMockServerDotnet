using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Xml;
using MockServer.Utils;

namespace MockServer;

public class ActAuthResultClass
{
    string test_data_path = Constants.TESTDATA_PATH_ActAuthenticateResponse;
    List<string> defaultLoginName = new List<string> { "1", "2", "3", "4", "5" };
    public async Task ActAuthResult(NetworkStream stream, XmlDocument xmlDoc, string url)
    {
        try
        {

            bool retResult = false;
            string authType = Helper.getXmlNodeValue(xmlDoc, "authType");
            string loginName = Helper.getXmlNodeValue(xmlDoc, "property[@sys-name='LoginName']");
            string authResult = Helper.getXmlNodeValue(xmlDoc, "authResult");
            string passWord = Helper.getXmlNodeValue(xmlDoc, "property[@sys-name='Password']");

            // Exception logins, This are used to send the xml, which are cannot cover in the normal execution
            // 11 - 15 loginNames are used in the simulator as default values for GROUP_TYPE
            // Remaining are used to send xml data which cannot be covered in normal execution  like
            // Server Side setting : System.Web.Configuration.WebConfigurationManager.AppSettings["ACT_AUTHENTICATERES_OMIT_ACL_ELEMENT"]
            List<string> exceptionLoginName = new List<string> { "11", "12", "13", "14", "15", "100", "101", "102", "103", "104", "105", "106", "107", "108", "1000" };

            Logger.Log($"ActAuthResult Input\n authResult : {authResult}\tauthType : {authType}\tloginName : {loginName} \tpassWord : {passWord}");

            switch (authType)
            {

                case "walk_up":

                    retResult = commonOperations.checkPassWord(loginName, passWord);
                    // switch (loginName)
                    // {
                    //     case "1":
                    //         retResult = (passWord == "1");
                    //         break;
                    //     case "2":
                    //         retResult = (passWord == "2");
                    //         break;
                    //     case "3":
                    //         retResult = (passWord == "3");
                    //         break;
                    //     case "4":
                    //         retResult = (passWord == "4");
                    //         break;
                    //     case "5":
                    //         retResult = (passWord == "5");
                    //         break;
                    //     case "10":
                    //         retResult = (passWord == "10");
                    //         break;

                    //     case "11":
                    //         retResult = (passWord == "11");
                    //         break;

                    //     case "12":
                    //         retResult = (passWord == "12");
                    //         break;


                    //     case "13":
                    //         retResult = (passWord == "13");
                    //         break;

                    //     case "14":
                    //         retResult = (passWord == "14");
                    //         break;
                    //     case "100":
                    //         retResult = (passWord == "100");
                    //         break;
                    //     case "101":
                    //         retResult = (passWord == "101");
                    //         break;
                    //     case "102":
                    //         retResult = (passWord == "102");
                    //         break;
                    //     case "103":
                    //         retResult = (passWord == "103");
                    //         break;

                    //     case "104":
                    //         retResult = (passWord == "104");
                    //         break;
                    //     case "105":
                    //         retResult = (passWord == "105");
                    //         break;

                    //     case "106":
                    //         retResult = (passWord == "106");
                    //         break;
                    //     case "107":
                    //         retResult = (passWord == "107");
                    //         break;
                    //     case "108":
                    //         retResult = (passWord == "108");
                    //         break;

                    //     case "1000":
                    //         retResult = (passWord == "1000");
                    //         break;

                    //     default:
                    //         retResult = false;
                    //         break;
                    // }

                    if (retResult)
                    {
                        // if (defaultLoginName.Contains(loginName))
                        // {
                        //     string xmlFilePath = test_data_path + "/true.xml";
                        //     await Helper.Send200_ReadXmlFromFileAsync(stream, xmlFilePath);

                        // }
                        // else
                        // {
                        //     await SendResponseBasesOnLoginName(stream, loginName);
                        // }
                        await SendResponseBasesOnLoginName(stream, loginName);

                    }
                    else
                    {
                        //  await SendResponseBasesOnLoginName(stream, loginName);
                        await Helper.Send200_ReadXmlFromFileAsync(stream, test_data_path + "/failed.xml");
                    }
                    // }
                    break;
                // ----------------------- walk_up Ended---------------------------------------

                case "card_hid":
                    string cardValue = Helper.getXmlNodeValue(xmlDoc, "property[@sys-name='CardValue']");
                    string cardName = commonOperations.getCard(cardValue);
                    if (!string.IsNullOrEmpty(cardName))
                    {
                        // string xmlFilePath = test_data_path + $"/{cardName}.xml";
                        // await Helper.Send200_ReadXmlFromFileAsync(stream, xmlFilePath);
                        await SendResponseBasesOnLoginName(stream, cardName);
                    }
                    else
                    {
                        Logger.Log($"cardValue : {cardValue} -- not found");
                        await Helper.Send200_ReadXmlFromFileAsync(stream, test_data_path + "/failed.xml");
                    }
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


    public async Task SendResponseBasesOnLoginName(NetworkStream stream, string name)
    {
        switch (name)
        {
            case "1":
            case "2":
            case "3":
            case "4":
            case "5":
                await Helper.Send200_ReadXmlFromFileAsync(stream, test_data_path + "/true.xml");
                break;

                // await Helper.Send200_ReadXmlFromFileAsync(stream, test_data_path + $"/{name}.xml");
                // break;

                // await Helper.Send200_ReadXmlFromFileAsync(stream, test_data_path + $"/{name}.xml");
                // break;

                // await Helper.Send200_ReadXmlFromFileAsync(stream, test_data_path + $"/{name}.xml");
                // break;

                // await Helper.Send200_ReadXmlFromFileAsync(stream, test_data_path + $"/{name}.xml");
                // break;

            case "11":
            case "12":
            case "13":
            case "14":
            case "card01":
            case "card02":
            case "card03":
            case "card04":
                await Helper.Send200_ReadXmlFromFileAsync(stream, test_data_path + $"/{name}.xml");
                break;

            case "100":
                await Helper.Send200_ReadXmlFromFileAsync(stream, test_data_path + "/100_ACL_TYPE_CUSTOM.xml");
                break;
            case "101":
                await Helper.Send200_ReadXmlFromFileAsync(stream, test_data_path + "/101_ACL_GROUP.xml");
                break;
            case "102":
                await Helper.Send200_ReadXmlFromFileAsync(stream, test_data_path + "/103_LCL_CUSTOM.xml");
                break;
            case "103":
                await Helper.Send200_ReadXmlFromFileAsync(stream, test_data_path + "/103_LCL_CUSTOM.xml");
                break;

            case "104":
                await Helper.Send200_ReadXmlFromFileAsync(stream, test_data_path + "/104_LCL_GROUP.xml");
                break;

            case "105":
                await Helper.Send200_ReadXmlFromFileAsync(stream, test_data_path + "/105_SCREEN_APP_TYPE.xml");
                break;

            case "106":
                await Helper.Send200_ReadXmlFromFileAsync(stream, test_data_path + "/106_SCREEN_TYPE.xml");
                break;

            case "107":
                await Helper.Send200_ReadXmlFromFileAsync(stream, test_data_path + "/107_SCREEN_TYPE_NULL_APP_ADDRESS.xml");
                break;

            case "108":
                await Helper.Send200_ReadXmlFromFileAsync(stream, test_data_path + "/108_SCREEN_TYPE_MAINMODE.xml");
                break;

            case "1000":
                await Helper.Send200_ReadXmlFromFileAsync(stream, test_data_path + "/1000_USERINFO_ACL_LCL_NULL.xml");
                break;

            // case "MQ==":
            //     await Helper.Send200_ReadXmlFromFileAsync(stream, test_data_path + "/card01.xml");
            //     break;

            // case "OQ==":
            //     await Helper.Send200_ReadXmlFromFileAsync(stream, test_data_path + "/card02.xml");
            //     break;

            // case "MTAwMDA=":
            //     await Helper.Send200_ReadXmlFromFileAsync(stream, test_data_path + "/card03.xml");
            //     break;

            // case "MzIwMDA=":
            //     await Helper.Send200_ReadXmlFromFileAsync(stream, test_data_path + "/card04.xml");
            //     break;
        }
    }
}