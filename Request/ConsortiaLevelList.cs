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
    public class ConsortiaLevelList : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string Bulid(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            XElement result = new XElement("Result");
            try
            {
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    ConsortiaLevelInfo[] allConsortiaLevel = bussiness.GetAllConsortiaLevel();
                    foreach (ConsortiaLevelInfo info in allConsortiaLevel)
                    {
                        result.Add(FlashUtils.CreateConsortiLevelInfo(info));
                    }
                }
                flag = true;
                str = "Success!";
            }
            catch (Exception exception)
            {
                log.Error("ConsortiaLevelList", exception);
            }
            result.Add(new XAttribute("value", flag));
            result.Add(new XAttribute("message", str));
            return csFunction.CreateCompressXml(context, result, "ConsortiaLevelList", true);
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(Bulid(context));
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

