namespace Tank.Request
{
    using Bussiness;
    using log4net;
    using System;
    using System.Reflection;
    using System.Web;
    using System.Web.Services;
    using System.Xml.Linq;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class UserApprenticeshipInfoList : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool flag = true;
            string str = "true!";
            bool flag2 = false;
            bool flag3 = false;
            XElement element = new XElement("Result");
            element.Add(new XAttribute("total", 0));
            element.Add(new XAttribute("value", flag));
            element.Add(new XAttribute("message", str));
            element.Add(new XAttribute("isPlayerRegeisted", flag2));
            element.Add(new XAttribute("isSelfPublishEquip", flag3));
            context.Response.ContentType = "text/plain";
            context.Response.Write(XmlExtends.ToString(element, false));
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

