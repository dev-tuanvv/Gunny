namespace Tank.Request
{
    using Bussiness;
    using Road.Flash;
    using SqlDataProvider.Data;
    using System;
    using System.Web;
    using System.Web.Services;
    using System.Xml.Linq;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class bombconfig : IHttpHandler
    {
        public static string Bulid(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            XElement result = new XElement("Result");
            try
            {
                using (ProduceBussiness bussiness = new ProduceBussiness())
                {
                    BallConfigInfo[] allBallConfig = bussiness.GetAllBallConfig();
                    foreach (BallConfigInfo info in allBallConfig)
                    {
                        result.Add(FlashUtils.CreateBallConfigInfo(info));
                    }
                }
                flag = true;
                str = "Success!";
            }
            catch (Exception)
            {
            }
            result.Add(new XAttribute("value", flag));
            result.Add(new XAttribute("message", str));
            return csFunction.CreateCompressXml(context, result, "bombconfig", true);
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

