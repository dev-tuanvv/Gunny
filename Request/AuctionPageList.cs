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
    public class AuctionPageList : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            int num = 0;
            XElement element = new XElement("Result");
            try
            {
                int num2 = int.Parse(context.Request["page"]);
                string str2 = csFunction.ConvertSql(HttpUtility.UrlDecode(context.Request["name"]));
                int num3 = int.Parse(context.Request["type"]);
                int num4 = int.Parse(context.Request["pay"]);
                int num5 = int.Parse(context.Request["userID"]);
                int num6 = int.Parse(context.Request["buyID"]);
                int num7 = int.Parse(context.Request["order"]);
                bool flag2 = bool.Parse(context.Request["sort"]);
                string str3 = csFunction.ConvertSql(HttpUtility.UrlDecode(context.Request["Auctions"]));
                str3 = string.IsNullOrEmpty(str3) ? "0" : str3;
                int num8 = 50;
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    AuctionInfo[] infoArray = bussiness.GetAuctionPage(num2, str2, num3, num4, ref num, num5, num6, num7, flag2, num8, str3);
                    foreach (AuctionInfo info in infoArray)
                    {
                        XElement content = FlashUtils.CreateAuctionInfo(info);
                        using (PlayerBussiness bussiness2 = new PlayerBussiness())
                        {
                            ItemInfo userItemSingle = bussiness2.GetUserItemSingle(info.ItemID);
                            if (userItemSingle != null)
                            {
                                content.Add(FlashUtils.CreateGoodsInfo(userItemSingle));
                            }
                            element.Add(content);
                        }
                    }
                    flag = true;
                    str = "Success!";
                }
            }
            catch (Exception exception)
            {
                log.Error("AuctionPageList", exception);
            }
            element.Add(new XAttribute("total", num));
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

