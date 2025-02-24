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
    public class MarryInfoPageList : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            int num = 0;
            XElement element = new XElement("Result");
            try
            {
                int num2 = int.Parse(context.Request["page"]);
                string str2 = null;
                if (context.Request["name"] != null)
                {
                    str2 = csFunction.ConvertSql(HttpUtility.UrlDecode(context.Request["name"]));
                }
                bool flag2 = bool.Parse(context.Request["sex"]);
                int num3 = 12;
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    MarryInfo[] infoArray = bussiness.GetMarryInfoPage(num2, str2, flag2, num3, ref num);
                    foreach (MarryInfo info in infoArray)
                    {
                        XElement content = FlashUtils.CreateMarryInfo(info);
                        element.Add(content);
                    }
                    flag = true;
                    str = "Success!";
                }
            }
            catch (Exception exception)
            {
                log.Error("MarryInfoPageList", exception);
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

