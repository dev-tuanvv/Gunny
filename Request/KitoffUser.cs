namespace Tank.Request
{
    using log4net;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Reflection;
    using System.Web.UI;

    public class KitoffUser : Page
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            bool flag = false;
            try
            {
                if (ValidLoginIP(this.Context.Request.UserHostAddress))
                {
                }
            }
            catch (Exception exception)
            {
                log.Error("GetAdminIP:", exception);
            }
            base.Response.Write(flag);
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

