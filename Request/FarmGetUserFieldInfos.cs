namespace Tank.Request
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Web;
    using System.Web.Services;
    using System.Xml.Linq;

    [WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1), WebService(Namespace="http://tempuri.org/")]
    public class FarmGetUserFieldInfos : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static int AccelerateTimeFields(UserFieldInfo m_field)
        {
            int num = 0;
            if ((m_field != null) && (m_field.SeedID > 0))
            {
                DateTime plantTime = m_field.PlantTime;
                int fieldValidDate = m_field.FieldValidDate;
                num = AccelerateTimeFields(plantTime, fieldValidDate);
            }
            return num;
        }

        private static int AccelerateTimeFields(DateTime PlantTime, int FieldValidDate)
        {
            DateTime now = DateTime.Now;
            int num = now.Hour - PlantTime.Hour;
            int num2 = now.Minute - PlantTime.Minute;
            int num3 = 0;
            if (num < 0)
            {
                num = 0x18 + num;
            }
            if (num2 < 0)
            {
                num2 = 60 + num2;
            }
            num3 = (num * 60) + num2;
            if (num3 > FieldValidDate)
            {
                num3 = FieldValidDate;
            }
            return num3;
        }

        public void ProcessRequest(HttpContext context)
        {
            int userID = Convert.ToInt32(context.Request["selfid"]);
            string str = context.Request["key"];
            bool flag = true;
            string str2 = "Success!";
            XElement node = new XElement("Result");
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                FriendInfo[] friendsAll = bussiness.GetFriendsAll(userID);
                foreach (FriendInfo info in friendsAll)
                {
                    XElement content = new XElement("Item");
                    UserFieldInfo[] singleFields = bussiness.GetSingleFields(info.FriendID);
                    foreach (UserFieldInfo info2 in singleFields)
                    {
                        XElement element3 = new XElement("Item", new object[] { new XAttribute("SeedID", info2.SeedID), new XAttribute("AcclerateDate", AccelerateTimeFields(info2)), new XAttribute("GrowTime", info2.PlantTime.ToString("yyyy-MM-ddTHH:mm:ss")) });
                        content.Add(element3);
                    }
                    content.Add(new XAttribute("UserID", info.FriendID));
                    node.Add(content);
                }
            }
            node.Add(new XAttribute("value", flag));
            node.Add(new XAttribute("message", str2));
            context.Response.ContentType = "text/plain";
            context.Response.Write(node.ToString(false));
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

