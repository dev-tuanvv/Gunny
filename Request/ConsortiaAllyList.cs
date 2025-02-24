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
    public class ConsortiaAllyList : IHttpHandler
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
                int num6 = int.Parse(context.Request["state"]);
                string str2 = csFunction.ConvertSql(HttpUtility.UrlDecode((context.Request["name"] == null) ? "" : context.Request["name"]));
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    ConsortiaAllyInfo[] infoArray = bussiness.GetConsortiaAllyPage(num2, num3, ref num, num4, num5, num6, str2);
                    foreach (ConsortiaAllyInfo info in infoArray)
                    {
                        element.Add(FlashUtils.CreateConsortiaAllyInfo(info));
                    }
                    flag = true;
                    str = "Success!";
                }
            }
            catch (Exception exception)
            {
                log.Error("ConsortiaAllyList", exception);
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

