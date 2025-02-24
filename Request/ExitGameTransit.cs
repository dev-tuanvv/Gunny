namespace Tank.Request
{
    using log4net;
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Web;
    using System.Web.Services;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class ExitGameTransit : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string site = "";

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string str = "";
            string loginURL = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(context.Request["username"]))
                {
                    str = HttpUtility.UrlDecode(context.Request["username"]).Trim();
                }
                this.site = (context.Request["site"] == null) ? "" : HttpUtility.UrlDecode(context.Request["site"]).ToLower();
                if (!string.IsNullOrEmpty(this.site))
                {
                    loginURL = this.LoginURL;
                    int index = str.IndexOf('_');
                    if (index != -1)
                    {
                        str = str.Substring(index + 1, (str.Length - index) - 1);
                    }
                }
                if (string.IsNullOrEmpty(loginURL))
                {
                    loginURL = ConfigurationSettings.AppSettings["ExitURL"];
                }
                context.Response.Redirect(string.Format(loginURL, str, this.site), false);
            }
            catch (Exception exception)
            {
                log.Error("ExitGameTransit:", exception);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public string LoginURL
        {
            get
            {
                string str = "ExitURL_" + this.site;
                return ConfigurationSettings.AppSettings[str];
            }
        }
    }
}

