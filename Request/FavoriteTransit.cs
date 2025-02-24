namespace Tank.Request
{
    using log4net;
    using System;
    using System.Configuration;
    using System.Web;
    using System.Web.Services;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class FavoriteTransit : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            try
            {
                string str = (context.Request["username"] == null) ? "" : HttpUtility.UrlDecode(context.Request["username"]);
                string str2 = (context.Request["site"] == null) ? "" : HttpUtility.UrlDecode(context.Request["site"]).ToLower();
                string getFavoriteUrl = string.Empty;
                if (!string.IsNullOrEmpty(str2))
                {
                    getFavoriteUrl = ConfigurationSettings.AppSettings[string.Format("FavoriteUrl_{0}", str2)];
                    int index = str.IndexOf('_');
                    if (index != -1)
                    {
                        str = str.Substring(index + 1, (str.Length - index) - 1);
                    }
                }
                if (string.IsNullOrEmpty(getFavoriteUrl))
                {
                    getFavoriteUrl = GetFavoriteUrl;
                }
                context.Response.Redirect(string.Format(getFavoriteUrl, str, str2), false);
            }
            catch (Exception exception)
            {
                log.Error("FavoriteTransit:", exception);
            }
        }

        public static string GetFavoriteUrl
        {
            get
            {
                return ConfigurationSettings.AppSettings["FavoriteUrl"];
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

