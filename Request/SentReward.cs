namespace Tank.Request
{
    using Bussiness;
    using Bussiness.Interface;
    using log4net;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using System.Web.Services;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class SentReward : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool checkParam(ref string param)
        {
            int num = 0;
            string str = "1";
            int num2 = 9;
            int num3 = 0;
            string str2 = "0";
            string str3 = "10";
            string str4 = "20";
            string str5 = "30";
            string str6 = "40";
            string str7 = "1";
            string str8 = "0";
            if (!string.IsNullOrEmpty(param))
            {
                char[] separator = new char[] { '|' };
                string[] strArray = param.Split(separator);
                int length = strArray.Length;
                if (length > 0)
                {
                    param = "";
                    int index = 0;
                    foreach (string str9 in strArray)
                    {
                        char[] chArray2 = new char[] { ',' };
                        string[] strArray3 = str9.Split(chArray2);
                        if (strArray3.Length > 0)
                        {
                            strArray[index] = "";
                            strArray3[2] = ((int.Parse(strArray3[2]) < num) || string.IsNullOrEmpty(strArray3[2].ToString())) ? str : strArray3[2];
                            strArray3[3] = (((int.Parse(strArray3[3].ToString()) < num3) || (int.Parse(strArray3[3].ToString()) > num2)) || string.IsNullOrEmpty(strArray3[3].ToString())) ? num3.ToString() : strArray3[3];
                            strArray3[4] = ((((strArray3[4] == str2) || (strArray3[4] == str3)) || ((strArray3[4] == str4) || (strArray3[4] == str5))) || ((strArray3[4] == str6) && !string.IsNullOrEmpty(strArray3[4].ToString()))) ? strArray3[4] : str2;
                            strArray3[5] = ((((strArray3[5] == str2) || (strArray3[5] == str3)) || ((strArray3[5] == str4) || (strArray3[5] == str5))) || ((strArray3[5] == str6) && !string.IsNullOrEmpty(strArray3[5].ToString()))) ? strArray3[5] : str2;
                            strArray3[6] = ((((strArray3[6] == str2) || (strArray3[6] == str3)) || ((strArray3[6] == str4) || (strArray3[6] == str5))) || ((strArray3[6] == str6) && !string.IsNullOrEmpty(strArray3[6].ToString()))) ? strArray3[6] : str2;
                            strArray3[7] = ((((strArray3[7] == str2) || (strArray3[7] == str3)) || ((strArray3[7] == str4) || (strArray3[7] == str5))) || ((strArray3[7] == str6) && !string.IsNullOrEmpty(strArray3[7].ToString()))) ? strArray3[7] : str2;
                            strArray3[8] = ((strArray3[8] == str7) || ((strArray3[8] == str8) && !string.IsNullOrEmpty(strArray3[8]))) ? strArray3[8] : str7;
                        }
                        for (int j = 0; j < 9; j++)
                        {
                            strArray[index] = strArray[index] + strArray3[j] + ",";
                        }
                        strArray[index] = strArray[index].Remove(strArray[index].Length - 1, 1);
                        index++;
                    }
                    for (int i = 0; i < length; i++)
                    {
                        param = param + strArray[i] + "|";
                    }
                    param = param.Remove(param.Length - 1, 1);
                    return true;
                }
            }
            return false;
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            try
            {
                int num = 1;
                if (ValidSentRewardIP(context.Request.UserHostAddress))
                {
                    string str = HttpUtility.UrlDecode(context.Request["content"]);
                    string getSentRewardKey = GetSentRewardKey;
                    string[] strArray = BaseInterface.CreateInterface().UnEncryptSentReward(str, ref num, getSentRewardKey);
                    if ((((strArray.Length == 8) && (num != 5)) && (num != 6)) && (num != 7))
                    {
                        string str3 = strArray[0];
                        string str4 = strArray[1];
                        string str5 = strArray[2];
                        int num2 = int.Parse(strArray[3]);
                        int num3 = int.Parse(strArray[4]);
                        string param = strArray[5];
                        if (this.checkParam(ref param))
                        {
                            num = new PlayerBussiness().SendMailAndItemByUserName(str3, str4, str5, num2, num3, param);
                        }
                        else
                        {
                            num = 4;
                        }
                    }
                }
                else
                {
                    num = 3;
                }
                context.Response.Write(num);
            }
            catch (Exception exception)
            {
                log.Error("SentReward", exception);
            }
        }

        public static bool ValidSentRewardIP(string ip)
        {
            string getSentRewardIP = GetSentRewardIP;
            return (string.IsNullOrEmpty(getSentRewardIP) || getSentRewardIP.Split(new char[] { '|' }).Contains<string>(ip));
        }

        public static string GetSentRewardIP
        {
            get
            {
                return ConfigurationSettings.AppSettings["SentRewardIP"];
            }
        }

        public static string GetSentRewardKey
        {
            get
            {
                return ConfigurationSettings.AppSettings["SentRewardKey"];
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}

