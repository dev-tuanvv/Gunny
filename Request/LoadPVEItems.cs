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
    public class LoadPVEItems : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string Build(HttpContext context)
        {
            bool flag = false;
            string str = "Fail";
            XElement result = new XElement("Result");
            try
            {
                using (PveBussiness bussiness = new PveBussiness())
                {
                    PveInfo[] allPveInfos = bussiness.GetAllPveInfos();
                    foreach (PveInfo info in allPveInfos)
                    {
                        result.Add(FlashUtils.CreatePveInfo(info));
                    }
                }
                flag = true;
                str = "Success!";
            }
            catch (Exception exception)
            {
                log.Error("LoadPVEItems", exception);
            }
            result.Add(new XAttribute("value", flag));
            result.Add(new XAttribute("message", str));
            return csFunction.CreateCompressXml(context, result, "LoadPVEItems", true);
        }

        public void ProcessRequest(HttpContext context)
        {
            if (csFunction.ValidAdminIP(context.Request.UserHostAddress))
            {
                context.Response.Write(Build(context));
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

