namespace Tank.Request
{
    using Bussiness;
    using log4net;
    using System;
    using System.Configuration;
    using System.Text;
    using System.Web;
    using System.Web.Services;
    using System.Xml.Linq;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class consortianamecheck : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            LanguageMgr.Setup(ConfigurationManager.AppSettings["ReqPath"]);
            bool flag = false;
            string str = "T\x00ean hội đ\x00e3 c\x00f3 người sử dụng.";
            XElement element = new XElement("Result");
            try
            {
                string s = csFunction.ConvertSql(HttpUtility.UrlDecode(context.Request["NickName"]));
                if (Encoding.Default.GetByteCount(s) <= 20)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                        {
                            if (bussiness.GetConsortiaByName(s) == null)
                            {
                                flag = true;
                                str = "Ch\x00fac mừng! T\x00ean hội đ\x00e3 c\x00f3 thể sử dụng.";
                            }
                        }
                    }
                }
                else
                {
                    str = "T\x00ean hội qu\x00e1 d\x00e0i";
                }
            }
            catch (Exception exception)
            {
                log.Error("NickNameCheck", exception);
                flag = false;
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

