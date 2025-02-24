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
    public class LoadUserEquip : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            XElement element = new XElement("Result");
            try
            {
                int num = int.Parse(context.Request["ID"]);
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    PlayerInfo userSingleByUserID = bussiness.GetUserSingleByUserID(num);
                    object[] content = new object[] { 
                        new XAttribute("Agility", userSingleByUserID.Agility), new XAttribute("Attack", userSingleByUserID.Attack), new XAttribute("Colors", userSingleByUserID.Colors), new XAttribute("Skin", userSingleByUserID.Skin), new XAttribute("Defence", userSingleByUserID.Defence), new XAttribute("GP", userSingleByUserID.GP), new XAttribute("Grade", userSingleByUserID.Grade), new XAttribute("Luck", userSingleByUserID.Luck), new XAttribute("Hide", userSingleByUserID.Hide), new XAttribute("Repute", userSingleByUserID.Repute), new XAttribute("Offer", userSingleByUserID.Offer), new XAttribute("NickName", userSingleByUserID.NickName), new XAttribute("ConsortiaName", userSingleByUserID.ConsortiaName), new XAttribute("ConsortiaID", userSingleByUserID.ConsortiaID), new XAttribute("ReputeOffer", userSingleByUserID.ReputeOffer), new XAttribute("ConsortiaHonor", userSingleByUserID.ConsortiaHonor), 
                        new XAttribute("ConsortiaLevel", userSingleByUserID.ConsortiaLevel), new XAttribute("ConsortiaRepute", userSingleByUserID.ConsortiaRepute), new XAttribute("WinCount", userSingleByUserID.Win), new XAttribute("TotalCount", userSingleByUserID.Total), new XAttribute("EscapeCount", userSingleByUserID.Escape), new XAttribute("Sex", userSingleByUserID.Sex), new XAttribute("Style", userSingleByUserID.Style), new XAttribute("FightPower", userSingleByUserID.FightPower)
                     };
                    element.Add(content);
                    ItemInfo[] infoArray = bussiness.GetUserEuqip(num).ToArray();
                    foreach (ItemInfo info2 in infoArray)
                    {
                        element.Add(FlashUtils.CreateGoodsInfo(info2));
                    }
                }
                flag = true;
                str = "Success!";
            }
            catch (Exception exception)
            {
                log.Error("LoadUserEquip", exception);
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

