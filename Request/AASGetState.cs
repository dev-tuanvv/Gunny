namespace Tank.Request
{
    using Bussiness.CenterService;
    using log4net;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Web.UI;

    public class AASGetState : Page
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            int num = 2;
            try
            {
                if (ValidLoginIP(this.Context.Request.UserHostAddress))
                {
                    using (CenterServiceClient client = new CenterServiceClient())
                    {
                        num = client.AASGetState();
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error("ASSGetState:", exception);
            }
            base.Response.Write(num);
        }

        public static bool ValidLoginIP(string ip)
        {
            string getAdminIP = GetAdminIP;
            return (string.IsNullOrEmpty(getAdminIP) || getAdminIP.Split(new char[] { '|' }).Contains<string>(ip));
        }

        public static string GetAdminIP
        {
            get
            {
                return ConfigurationSettings.AppSettings["AdminIP"];
            }
        }
    }
}

