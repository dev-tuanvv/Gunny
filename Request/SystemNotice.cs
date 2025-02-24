namespace Tank.Request
{
    using Bussiness.CenterService;
    using log4net;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using System.Web.UI;

    public class SystemNotice : Page
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            int num = 1;
            try
            {
                if (ValidLoginIP(this.Context.Request.UserHostAddress))
                {
                    string str = HttpUtility.UrlDecode(base.Request["content"]);
                    if (!string.IsNullOrEmpty(str))
                    {
                        using (CenterServiceClient client = new CenterServiceClient())
                        {
                            if (client.SystemNotice(str))
                            {
                                num = 0;
                            }
                        }
                    }
                }
                else
                {
                    num = 2;
                }
            }
            catch (Exception exception)
            {
                log.Error("SystemNotice:", exception);
            }
            base.Response.Write(num);
        }

        public static bool ValidLoginIP(string ip)
        {
            string getChargeIP = GetChargeIP;
            return (string.IsNullOrEmpty(getChargeIP) || getChargeIP.Split(new char[] { '|' }).Contains<string>(ip));
        }

        public static string GetChargeIP
        {
            get
            {
                return ConfigurationSettings.AppSettings["AdminIP"];
            }
        }
    }
}

