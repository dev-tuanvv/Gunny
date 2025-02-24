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
    public class ConsortiaList : IHttpHandler
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
                string str2 = csFunction.ConvertSql(HttpUtility.UrlDecode(context.Request["name"]));
                int num6 = int.Parse(context.Request["level"]);
                int num7 = int.Parse(context.Request["openApply"]);
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    ConsortiaInfo[] infoArray = bussiness.GetConsortiaPage(num2, num3, ref num, num4, str2, num5, num6, num7);
                    foreach (ConsortiaInfo info in infoArray)
                    {
                        element.Add(FlashUtils.CreateConsortiaInfo(info));
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
            context.Response.BinaryWrite(StaticFunction.Compress(XmlExtends.ToString(element, false)));
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

