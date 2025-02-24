namespace Tank.Request
{
    using Bussiness;
    using log4net;
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Web;
    using System.Web.Services;
    using System.Xml.Linq;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class VisualizeItemLoad : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            bool flag2 = bool.Parse(context.Request["sex"]);
            XElement element = new XElement("Result");
            try
            {
                string str2 = ConfigurationSettings.AppSettings[flag2 ? "BoyVisualizeItem" : "GrilVisualizeItem"];
                element.Add(new XAttribute("content", str2));
                flag = true;
                str = "Success!";
            }
            catch (Exception exception)
            {
                log.Error("VisualizeItemLoad", exception);
            }
            element.Add(new XAttribute("value", flag));
            element.Add(new XAttribute("message", str));
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

