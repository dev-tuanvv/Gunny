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
    public class ItemStrengthenList : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string Bulid(HttpContext context)
        {
            bool flag = false;
            string str = "Fail";
            XElement result = new XElement("Result");
            try
            {
                using (ProduceBussiness bussiness = new ProduceBussiness())
                {
                    StrengthenInfo[] allStrengthen = bussiness.GetAllStrengthen();
                    foreach (StrengthenInfo info in allStrengthen)
                    {
                        result.Add(FlashUtils.CreateStrengthenInfo(info));
                    }
                }
                flag = true;
                str = "Success!";
            }
            catch (Exception exception)
            {
                log.Error("ItemStrengthenList", exception);
            }
            result.Add(new XAttribute("value", flag));
            result.Add(new XAttribute("message", str));
            return csFunction.CreateCompressXml(context, result, "ItemStrengthenList", true);
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

