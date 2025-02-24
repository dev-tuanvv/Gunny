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
    public class ConsortiaEquipControlList : IHttpHandler
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
                int num2 = 1;
                int num3 = 10;
                int num4 = 1;
                int num5 = int.Parse(context.Request["consortiaID"]);
                int num6 = int.Parse(context.Request["level"]);
                int num7 = int.Parse(context.Request["type"]);
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    ConsortiaEquipControlInfo[] infoArray = bussiness.GetConsortiaEquipControlPage(num2, num3, ref num, num4, num5, num6, num7);
                    foreach (ConsortiaEquipControlInfo info in infoArray)
                    {
                        element.Add(FlashUtils.CreateConsortiaEquipControlInfo(info));
                    }
                    flag = true;
                    str = "Success!";
                }
            }
            catch (Exception exception)
            {
                log.Error("ConsortiaList", exception);
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

