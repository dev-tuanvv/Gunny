namespace Tank.Request
{
    using Bussiness;
    using log4net;
    using Road.Flash;
    using SqlDataProvider.Data;
    using System;
    using System.Web;
    using System.Web.Services;
    using System.Xml.Linq;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class ConsortiaEventList : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            XElement element = new XElement("Result");
            int num = 0;
            try
            {
                int num2 = int.Parse(context.Request["page"]);
                int num3 = int.Parse(context.Request["size"]);
                int num4 = int.Parse(context.Request["order"]);
                int num5 = int.Parse(context.Request["consortiaID"]);
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    ConsortiaEventInfo[] infoArray = bussiness.GetConsortiaEventPage(num2, num3, ref num, num4, num5);
                    foreach (ConsortiaEventInfo info in infoArray)
                    {
                        element.Add(FlashUtils.CreateConsortiaEventInfo(info));
                    }
                    flag = true;
                    str = "Success!";
                }
            }
            catch (Exception exception)
            {
                log.Error("ConsortiaEventList", exception);
            }
            element.Add(new XAttribute("total", num));
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

