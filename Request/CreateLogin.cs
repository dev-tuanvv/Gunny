namespace Tank.Request
{
    using Bussiness.Interface;
    using log4net;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using System.Web.UI;

    public class CreateLogin : Page
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            int num = 1;
            try
            {
                string str = HttpUtility.UrlDecode(base.Request["content"]);
                string str2 = (base.Request["site"] == null) ? "" : HttpUtility.UrlDecode(base.Request["site"]).ToLower();
                string[] strArray = BaseInterface.CreateInterface().UnEncryptLogin(str, ref num, str2);
                if (strArray.Length > 3)
                {
                    string str3 = strArray[0].Trim().ToLower();
                    string str4 = strArray[1].Trim().ToLower();
                    if (!string.IsNullOrEmpty(str3) && !string.IsNullOrEmpty(str4))
                    {
                        PlayerManager.Add(BaseInterface.GetNameBySite(str3, str2), str4);
                        num = 0;
                    }
                    else
                    {
                        num = -91010;
                    }
                }
                else
                {
                    num = -1900;
                }
            }
            catch (Exception exception)
            {
                log.Error("CreateLogin:", exception);
            }
            base.Response.Write(num);
        }

        public static bool ValidLoginIP(string ip)
        {
            string getLoginIP = GetLoginIP;
            return (string.IsNullOrEmpty(getLoginIP) || getLoginIP.Split(new char[] { '|' }).Contains<string>(ip));
        }

        public static string GetLoginIP
        {
            get
            {
                return ConfigurationSettings.AppSettings["LoginIP"];
            }
        }
    }
}

