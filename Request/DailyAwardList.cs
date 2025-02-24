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
    public class DailyAwardList : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string Bulid(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            XElement result = new XElement("Result");
            try
            {
                using (ProduceBussiness bussiness = new ProduceBussiness())
                {
                    DailyAwardInfo[] allDailyAward = bussiness.GetAllDailyAward();
                    foreach (DailyAwardInfo info in allDailyAward)
                    {
                        result.Add(FlashUtils.CreateActiveInfo(info));
                    }
                    flag = true;
                    str = "Success!";
                }
            }
            catch (Exception exception)
            {
                log.Error("Load DailyAwardList is fail!", exception);
            }
            result.Add(new XAttribute("value", flag));
            result.Add(new XAttribute("message", str));
            return csFunction.CreateCompressXml(context, result, "DailyAwardList", true);
        }

        public void ProcessRequest(HttpContext context)
        {
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

