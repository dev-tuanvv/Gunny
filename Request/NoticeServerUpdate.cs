namespace Tank.Request
{
    using Bussiness.CenterService;
    using Bussiness.Interface;
    using log4net;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using System.Web.UI;

    public class NoticeServerUpdate : Page
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private int HandleServerMapUpdate()
        {
            if (BaseInterface.RequestContent("http://" + HttpContext.Current.Request.Url.Authority.ToString() + "/MapServerList.ashx").Contains("Success"))
            {
                return 0;
            }
            return 3;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int num = 2;
            try
            {
                int num2 = int.Parse(this.Context.Request["serverID"]);
                int num3 = int.Parse(this.Context.Request["type"]);
                if (ValidLoginIP(this.Context.Request.UserHostAddress))
                {
                    using (CenterServiceClient client = new CenterServiceClient())
                    {
                        num = client.NoticeServerUpdate(num2, num3);
                    }
                    int num4 = num3;
                    if ((num4 == 5) && (num == 0))
                    {
                        num = this.HandleServerMapUpdate();
                    }
                }
                else
                {
                    num = 5;
                }
            }
            catch (Exception exception)
            {
                log.Error("ExperienceRateUpdate:", exception);
                num = 4;
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

