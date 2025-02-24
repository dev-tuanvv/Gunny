namespace Tank.Request
{
    using Bussiness;
    using log4net;
    using Road.Flash;
    using SqlDataProvider.Data;
    using System;
    using System.Reflection;
    using System.Web;
    using System.Web.Services;
    using System.Xml.Linq;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class MailSenderList : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            XElement element = new XElement("Result");
            try
            {
                int num = int.Parse(context.Request.QueryString["selfID"]);
                if (num > 0)
                {
                    using (PlayerBussiness bussiness = new PlayerBussiness())
                    {
                        MailInfo[] mailBySenderID = bussiness.GetMailBySenderID(num);
                        foreach (MailInfo info in mailBySenderID)
                        {
                            element.Add(FlashUtils.CreateMailInfo(info, "Item"));
                        }
                    }
                    flag = true;
                    str = "Success!";
                }
            }
            catch (Exception exception)
            {
                log.Error("MailSenderList", exception);
            }
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

