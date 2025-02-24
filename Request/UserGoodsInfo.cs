namespace Tank.Request
{
    using Bussiness;
    using log4net;
    using Road.Flash;
    using SqlDataProvider.Data;
    using System;
    using System.Reflection;
    using System.Web;
    using System.Web.Services;
    using System.Xml.Linq;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class UserGoodsInfo : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            XElement element = new XElement("Result");
            try
            {
                int num = int.Parse(context.Request.Params["ID"]);
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    ItemInfo userItemSingle = bussiness.GetUserItemSingle(num);
                    element.Add(FlashUtils.CreateGoodsInfo(userItemSingle));
                }
                flag = true;
                str = "Success!";
            }
            catch (Exception exception)
            {
                log.Error("UserGoodsInfo", exception);
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

