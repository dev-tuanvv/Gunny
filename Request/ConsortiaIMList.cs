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
    public class ConsortiaIMList : IHttpHandler
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
                int num2 = int.Parse(context.Request["id"]);
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    ConsortiaInfo consortiaSingle = bussiness.GetConsortiaSingle(num2);
                    if (consortiaSingle != null)
                    {
                        element.Add(new XAttribute("Level", consortiaSingle.Level));
                        element.Add(new XAttribute("Repute", consortiaSingle.Repute));
                    }
                }
                using (ConsortiaBussiness bussiness2 = new ConsortiaBussiness())
                {
                    ConsortiaUserInfo[] infoArray = bussiness2.GetConsortiaUsersPage(1, 0x3e8, ref num, -1, num2, -1, -1);
                    foreach (ConsortiaUserInfo info2 in infoArray)
                    {
                        element.Add(FlashUtils.CreateConsortiaIMInfo(info2));
                    }
                    flag = true;
                    str = "Success!";
                }
            }
            catch (Exception exception)
            {
                log.Error("ConsortiaIMList", exception);
            }
            element.Add(new XAttribute("value", flag));
            element.Add(new XAttribute("message", str));
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

