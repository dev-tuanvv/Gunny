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

    [WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1), WebService(Namespace="http://tempuri.org/")]
    public class run : IHttpHandler
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
                    RuneTemplateInfo[] all = bussiness.GetAllRuneTemplate();
                    foreach (RuneTemplateInfo info in all)
                    {
                        result.Add(FlashUtils.CreateRun(info));
                    }
                    //BallInfo[] allBall = bussiness.GetAllBall();
                    //foreach (BallInfo info in allBall)
                    //{
                    //    result.Add(FlashUtils.CreateBallInfo(info));
                    //}
                }
                flag = true;
                str = "Success!";
            }
            catch (Exception exception)
            {
                log.Error("runetemplatelist", exception);
            }
            result.Add(new XAttribute("value", flag));
            result.Add(new XAttribute("message", str));
            return csFunction.CreateCompressXml(context, result, "runetemplatelist", true);
        }

        public void ProcessRequest(HttpContext context)
        {
            if (csFunction.ValidAdminIP(context.Request.UserHostAddress))
            {
                context.Response.Write(Bulid(context));
            }
            else
            {
                context.Response.Write("IP is not valid!");
            }
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

