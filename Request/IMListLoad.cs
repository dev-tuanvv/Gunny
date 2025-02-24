namespace Tank.Request
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Reflection;
    using System.Web;
    using System.Web.Services;
    using System.Xml.Linq;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class IMListLoad : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            XElement element = new XElement("Result");
            try
            {
                int num = int.Parse(context.Request["id"]);
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    FriendInfo[] friendsAll = bussiness.GetFriendsAll(num);
                    object[] content = new object[] { new XAttribute("ID", 0), new XAttribute("Name", "Bạn b\x00e8") };
                    XElement element2 = new XElement("customList", content);
                    element.Add(element2);
                    foreach (FriendInfo info in friendsAll)
                    {
                        object[] objArray2 = new object[] { 
                            new XAttribute("ID", info.FriendID), new XAttribute("NickName", info.NickName), new XAttribute("Birthday", DateTime.Now), new XAttribute("ApprenticeshipState", 0), new XAttribute("LoginName", info.UserName), new XAttribute("Style", info.Style), new XAttribute("Sex", info.Sex == 1), new XAttribute("Colors", info.Colors), new XAttribute("Grade", info.Grade), new XAttribute("Hide", info.Hide), new XAttribute("ConsortiaName", info.ConsortiaName), new XAttribute("TotalCount", info.Total), new XAttribute("EscapeCount", info.Escape), new XAttribute("WinCount", info.Win), new XAttribute("Offer", info.Offer), new XAttribute("Relation", info.Relation), 
                            new XAttribute("Repute", info.Repute), new XAttribute("State", (info.State == 1) ? 1 : 0), new XAttribute("Nimbus", info.Nimbus), new XAttribute("DutyName", info.DutyName)
                         };
                        XElement element3 = new XElement("Item", objArray2);
                        element.Add(element3);
                    }
                }
                flag = true;
                str = "Success!";
            }
            catch (Exception exception)
            {
                log.Error("IMListLoad", exception);
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

