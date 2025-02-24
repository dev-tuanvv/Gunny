namespace Tank.Request
{
    using Bussiness;
    using Road.Flash;
    using SqlDataProvider.Data;
    using System;
    using System.Web;
    using System.Xml.Linq;

    public class LoadBoxTemp : IHttpHandler
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
                    ItemBoxInfo[] itemBoxInfos = bussiness.GetItemBoxInfos();
                    foreach (ItemBoxInfo info in itemBoxInfos)
                    {
                        result.Add(FlashUtils.CreateItemBoxInfo(info));
                    }
                    flag = true;
                    str = "Success!";
                }
            }
            catch
            {
            }
            result.Add(new XAttribute("value", flag));
            result.Add(new XAttribute("message", str));
            return csFunction.CreateCompressXml(context, result, "LoadBoxTemp", true);
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

