namespace Tank.Request
{
    using Bussiness;
    using Bussiness.CenterService;
    using log4net;
    using Road.Flash;
    using System;
    using System.Configuration;
    using System.Text;
    using System.Web;
    using System.Xml.Linq;

    public class ActivePullDown : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            LanguageMgr.Setup(ConfigurationManager.AppSettings["ReqPath"]);
            int num = Convert.ToInt32(context.Request["selfid"]);
            int num2 = Convert.ToInt32(context.Request["activeID"]);
            string str = context.Request["key"];
            string src = context.Request["activeKey"];
            bool flag = false;
            string translation = "ActivePullDownHandler.Fail";
            string str4 = "";
            XElement element = new XElement("Result");
            if (src != "")
            {
                byte[] bytes = CryptoHelper.RsaDecryt2(StaticFunction.RsaCryptor, src);
                str4 = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            }
            try
            {
                using (ActiveBussiness bussiness = new ActiveBussiness())
                {
                    if (bussiness.PullDown(num2, str4, num, ref translation) == 0)
                    {
                        using (CenterServiceClient client = new CenterServiceClient())
                        {
                            client.MailNotice(num);
                        }
                    }
                }
                flag = true;
                translation = LanguageMgr.GetTranslation(translation, new object[0]);
            }
            catch (Exception exception)
            {
                log.Error("ActivePullDown", exception);
            }
            element.Add(new XAttribute("value", flag));
            element.Add(new XAttribute("message", translation));
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

