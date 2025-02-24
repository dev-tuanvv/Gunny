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
    public class LoadUserMail : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void AddAnnex(XElement node, string value)
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                if (!string.IsNullOrEmpty(value))
                {
                    ItemInfo userItemSingle = bussiness.GetUserItemSingle(int.Parse(value));
                    if (userItemSingle != null)
                    {
                        node.Add(FlashUtils.CreateGoodsInfo(userItemSingle));
                    }
                }
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            XElement element = new XElement("Result");
            try
            {
                int num = int.Parse(context.Request.QueryString["selfid"]);
                if (num > 0)
                {
                    using (PlayerBussiness bussiness = new PlayerBussiness())
                    {
                        MailInfo[] mailByUserID = bussiness.GetMailByUserID(num);
                        foreach (MailInfo info in mailByUserID)
                        {
                            object[] content = new object[] { new XAttribute("ID", info.ID), new XAttribute("Title", info.Title), new XAttribute("Content", info.Content), new XAttribute("Sender", info.Sender), new XAttribute("SendTime", info.SendTime.ToString("yyyy-MM-dd HH:mm:ss")), new XAttribute("Gold", info.Gold), new XAttribute("Money", info.Money), new XAttribute("Annex1ID", (info.Annex1 == null) ? "" : info.Annex1), new XAttribute("Annex2ID", (info.Annex2 == null) ? "" : info.Annex2), new XAttribute("Annex3ID", (info.Annex3 == null) ? "" : info.Annex3), new XAttribute("Annex4ID", (info.Annex4 == null) ? "" : info.Annex4), new XAttribute("Annex5ID", (info.Annex5 == null) ? "" : info.Annex5), new XAttribute("Type", info.Type), new XAttribute("ValidDate", info.ValidDate), new XAttribute("IsRead", info.IsRead) };
                            XElement node = new XElement("Item", content);
                            AddAnnex(node, info.Annex1);
                            AddAnnex(node, info.Annex2);
                            AddAnnex(node, info.Annex3);
                            AddAnnex(node, info.Annex4);
                            AddAnnex(node, info.Annex5);
                            element.Add(node);
                        }
                    }
                    flag = true;
                    str = "Success!";
                }
            }
            catch (Exception exception)
            {
                log.Error("LoadUserMail", exception);
            }
            element.Add(new XAttribute("value", flag));
            element.Add(new XAttribute("message", str));
            context.Response.ContentType = "text/plain";
            context.Response.BinaryWrite(StaticFunction.Compress(XmlExtends.ToString(element, false)));
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

