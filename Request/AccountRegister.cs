namespace Tank.Request
{
    using Bussiness;
    using log4net;
    using System;
    using System.Web;
    using System.Web.Services;
    using System.Xml.Linq;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class AccountRegister : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            XElement element = new XElement("Result");
            bool flag = false;
            try
            {
                string str = HttpUtility.UrlDecode(context.Request["username"]);
                string str2 = HttpUtility.UrlDecode(context.Request["password"]);
                string str3 = HttpUtility.UrlDecode(context.Request["password"]);
                bool flag2 = false;
                int num = 100;
                int num2 = 100;
                int num3 = 100;
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    flag = bussiness.RegisterUser(str, str2, str3, flag2, num, num2, num3);
                }
            }
            catch (Exception exception)
            {
                log.Error("RegisterResult", exception);
            }
            finally
            {
                element.Add(new XAttribute("value", "vl"));
                element.Add(new XAttribute("message", flag));
                context.Response.ContentType = "text/plain";
                context.Response.Write(XmlExtends.ToString(element, false));
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

