namespace Tank.Request
{
    using Bussiness;
    using log4net;
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Web;
    using System.Web.Services;
    using System.Xml.Linq;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class IMFriendsGood : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            XElement element = new XElement("Result");
            try
            {
                string str2 = context.Request["UserName"];
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    ArrayList friendsGood = bussiness.GetFriendsGood(str2);
                    for (int i = 0; i < friendsGood.Count; i++)
                    {
                        XElement content = new XElement("Item", new XAttribute("UserName", friendsGood[i].ToString()));
                        element.Add(content);
                    }
                }
                flag = true;
                str = "Success!";
            }
            catch (Exception exception)
            {
                log.Error("IMFriendsGood", exception);
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

