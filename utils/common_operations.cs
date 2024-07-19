namespace MockServer.Utils;

class commonOperations
{
    public static bool checkPassWord(string loginName, string passWord)
    {
        bool retResult = false;
        switch (loginName)
        {
            case "1":
                retResult = (passWord == "1");
                break;
            case "2":
                retResult = (passWord == "2");
                break;
            case "3":
                retResult = (passWord == "3");
                break;
            case "4":
                retResult = (passWord == "4");
                break;
            case "5":
                retResult = (passWord == "5");
                break;
            case "10":
                retResult = (passWord == "10");
                break;

            case "11":
                retResult = (passWord == "11");
                break;

            case "12":
                retResult = (passWord == "12");
                break;


            case "13":
                retResult = (passWord == "13");
                break;

            case "14":
                retResult = (passWord == "14");
                break;
            case "100":
                retResult = (passWord == "100");
                break;
            case "101":
                retResult = (passWord == "101");
                break;
            case "102":
                retResult = (passWord == "102");
                break;
            case "103":
                retResult = (passWord == "103");
                break;

            case "104":
                retResult = (passWord == "104");
                break;
            case "105":
                retResult = (passWord == "105");
                break;

            case "106":
                retResult = (passWord == "106");
                break;
            case "107":
                retResult = (passWord == "107");
                break;
            case "108":
                retResult = (passWord == "108");
                break;

            case "1000":
                retResult = (passWord == "1000");
                break;

            default:
                retResult = false;
                break;
        }

        return retResult;
    }

    public static string getCard(string cardValue)
    {
        string cardNumer = string.Empty;
        switch (cardValue)
        {
            case "MQ==":
                cardNumer = "card01";
                break;

            case "OQ==":
                cardNumer = "card02";
                break;

            case "MTAwMDA=":
                cardNumer = "card03";
                break;

            case "MzIwMDA=":
                cardNumer = "card04";
                break;

            case "11":
                cardNumer = "11";
                break;

            case "12":
                cardNumer = "12";
                break;

            case "13":
                cardNumer = "13";
                break;

            case "14":
                cardNumer = "14";
                break;

            case "100":
                cardNumer = "100";
                break;

            case "101":
                cardNumer = "101";
                break;

            case "102":
                cardNumer = "102";
                break;

            case "103":
                cardNumer = "103";
                break;

            case "104":
                cardNumer = "104";
                break;

            case "105":
                cardNumer = "105";
                break;

            case "106":
                cardNumer = "106";
                break;

            case "107":
                cardNumer = "107";
                break;

            case "108":
                cardNumer = "108";
                break;

            case "1000":
                cardNumer = "1000";
                break;

        }
        return cardNumer;
    }
}