namespace Tank.Request
{
    using Bussiness;
    using log4net;
    using Road.Flash;
    using SqlDataProvider.Data;
    using System;
    using System.Reflection;
    using System.Web;
    using System.Xml.Linq;

    public class searchgoodstemp : IHttpHandler
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
                    SearchGoodsTempInfo[] allSearchGoodsTemp = bussiness.GetAllSearchGoodsTemp();
                    foreach (SearchGoodsTempInfo info in allSearchGoodsTemp)
                    {
                        result.Add(FlashUtils.CreateSearchGoodsTemp(info));
                    }
                    flag = true;
                    str = "Success!";
                }
            }
            catch (Exception exception)
            {
                log.Error("searchgoodstemp", exception);
            }
            result.Add(new XAttribute("value", flag));
            result.Add(new XAttribute("message", str));
            return csFunction.CreateCompressXml(context, result, "searchgoodstemp", false);
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

