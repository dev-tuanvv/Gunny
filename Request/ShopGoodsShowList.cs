namespace Tank.Request
{
    using Bussiness;
    using Road.Flash;
    using SqlDataProvider.Data;
    using System;
    using System.Web;
    using System.Xml.Linq;

    public class ShopGoodsShowList : IHttpHandler
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
                    XElement content = new XElement("Store");
                    ShopGoodsShowListInfo[] allShopGoodsShowList = bussiness.GetAllShopGoodsShowList();
                    foreach (ShopGoodsShowListInfo info in allShopGoodsShowList)
                    {
                        content.Add(FlashUtils.CreateShopShowInfo(info));
                    }
                    result.Add(content);
                    flag = true;
                    str = "Success!";
                }
            }
            catch
            {
            }
            result.Add(new XAttribute("value", flag));
            result.Add(new XAttribute("message", str));
            return csFunction.CreateCompressXml(context, result, "ShopGoodsShowList", true);
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

