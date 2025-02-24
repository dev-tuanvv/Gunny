namespace Tank.Request
{
    using log4net;
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Web;
    using System.Web.Services;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class PayTransit : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string site = "";

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string str = "";
            string payURL = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(context.Request["username"]))
                {
                    str = HttpUtility.UrlDecode(context.Request["username"].Trim());
                }
                this.site = (context.Request["site"] == null) ? "" : HttpUtility.UrlDecode(context.Request["site"]).ToLower();
                if (!string.IsNullOrEmpty(this.site))
                {
                    payURL = this.PayURL;
                    int index = str.IndexOf('_');
                    if (index != -1)
                    {
                        str = str.Substring(index + 1, (str.Length - index) - 1);
                    }
                }
                if (string.IsNullOrEmpty(payURL))
                {
                    payURL = ConfigurationSettings.AppSettings["PayURL"];
                }
                context.Response.Redirect(string.Format(payURL, str, this.site), false);
            }
            catch (Exception exception)
            {
                log.Error("PayTransit:", exception);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public string PayURL
        {
            get
            {
                string str = "PayURL_" + this.site;
                return ConfigurationSettings.AppSettings[str];
            }
        }
    }
}

