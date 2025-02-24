namespace Tank.Request
{
    using Bussiness;
    using Bussiness.Interface;
    using log4net;
    using Road.Flash;
    using SqlDataProvider.Data;
    using System;
    using System.Text;
    using System.Web;
    using System.Web.Services;
    using System.Web.SessionState;
    using System.Xml.Linq;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class Login : IHttpHandler, IRequiresSessionState
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string translation = LanguageMgr.GetTranslation("Tank.Request.Login.Fail1", new object[0]);
            string str2 = "";
            bool flag2 = false;
            XElement element = new XElement("Result");
            try
            {
                BaseInterface interface2 = BaseInterface.CreateInterface();
                string str3 = context.Request["p"];
                string str4 = (context.Request["site"] == null) ? "" : HttpUtility.UrlDecode(context.Request["site"]);
                string userHostAddress = context.Request.UserHostAddress;
                if (!string.IsNullOrEmpty(str3))
                {
                    byte[] bytes = CryptoHelper.RsaDecryt2(StaticFunction.RsaCryptor, str3);
                    char[] separator = new char[] { ',' };
                    string[] strArray = Encoding.UTF8.GetString(bytes, 7, bytes.Length - 7).Split(separator);
                    if (strArray.Length == 4)
                    {
                        string name = strArray[0];
                        string pass = strArray[1];
                        string str8 = strArray[2];
                        string str9 = strArray[3];
                        if (PlayerManager.Login(name, pass))
                        {
                            int num = 0;
                            bool flag6 = false;
                            bool byUserIsFirst = PlayerManager.GetByUserIsFirst(name);
                            PlayerInfo info = interface2.CreateLogin(name, str8, ref translation, ref num, userHostAddress, ref flag2, byUserIsFirst, ref flag6, str4, str9);
                            if (flag6)
                            {
                                StaticsMgr.RegCountAdd();
                            }
                            if ((info != null) && !flag2)
                            {
                                if (num == 0)
                                {
                                    PlayerManager.Update(name, str8);
                                }
                                else
                                {
                                    PlayerManager.Remove(name);
                                }
                                str2 = string.IsNullOrEmpty(info.Style) ? ",,,,,,,," : info.Style;
                                info.Colors = string.IsNullOrEmpty(info.Colors) ? ",,,,,,,," : info.Colors;
                                object[] content = new object[] { 
                                    new XAttribute("ID", info.ID), new XAttribute("IsFirst", num), new XAttribute("NickName", info.NickName), new XAttribute("Date", ""), new XAttribute("IsConsortia", 0), new XAttribute("ConsortiaID", info.ConsortiaID), new XAttribute("Sex", info.Sex), new XAttribute("WinCount", info.Win), new XAttribute("TotalCount", info.Total), new XAttribute("EscapeCount", info.Escape), new XAttribute("DutyName", (info.DutyName == null) ? "" : info.DutyName), new XAttribute("GP", info.GP), new XAttribute("Honor", ""), new XAttribute("Style", str2), new XAttribute("Gold", info.Gold), new XAttribute("Colors", (info.Colors == null) ? "" : info.Colors), 
                                    new XAttribute("Attack", info.Attack), new XAttribute("Defence", info.Defence), new XAttribute("Agility", info.Agility), new XAttribute("Luck", info.Luck), new XAttribute("Grade", info.Grade), new XAttribute("Hide", info.Hide), new XAttribute("Repute", info.Repute), new XAttribute("ConsortiaName", (info.ConsortiaName == null) ? "" : info.ConsortiaName), new XAttribute("Offer", info.Offer), new XAttribute("Skin", (info.Skin == null) ? "" : info.Skin), new XAttribute("ReputeOffer", info.ReputeOffer), new XAttribute("ConsortiaHonor", info.ConsortiaHonor), new XAttribute("ConsortiaLevel", info.ConsortiaLevel), new XAttribute("ConsortiaRepute", info.ConsortiaRepute), new XAttribute("Money", info.Money), new XAttribute("AntiAddiction", info.AntiAddiction), 
                                    new XAttribute("IsMarried", info.IsMarried), new XAttribute("SpouseID", info.SpouseID), new XAttribute("SpouseName", (info.SpouseName == null) ? "" : info.SpouseName), new XAttribute("MarryInfoID", info.MarryInfoID), new XAttribute("IsCreatedMarryRoom", info.IsCreatedMarryRoom), new XAttribute("IsGotRing", info.IsGotRing), new XAttribute("LoginName", (info.UserName == null) ? "" : info.UserName), new XAttribute("Nimbus", info.Nimbus), new XAttribute("FightPower", info.FightPower), new XAttribute("AnswerSite", info.AnswerSite), new XAttribute("WeaklessGuildProgressStr", (info.WeaklessGuildProgressStr == null) ? "" : info.WeaklessGuildProgressStr), new XAttribute("IsOldPlayer", false)
                                 };
                                XElement element2 = new XElement("Item", content);
                                element.Add(element2);
                                flag = true;
                                translation = LanguageMgr.GetTranslation("Tank.Request.Login.Success", new object[0]);
                            }
                            else
                            {
                                PlayerManager.Remove(name);
                            }
                        }
                        else
                        {
                            log.Error("name:" + name + "-pwd:" + pass);
                            translation = LanguageMgr.GetTranslation("BaseInterface.LoginAndUpdate.Try", new object[0]);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error(string.Concat(new object[] { "User Login error: (--", StaticFunction.RsaCryptor.KeySize, "--)", exception.ToString() }));
                flag = false;
                translation = LanguageMgr.GetTranslation("Tank.Request.Login.Fail2", new object[0]);
            }
            finally
            {
                element.Add(new XAttribute("value", flag));
                element.Add(new XAttribute("message", translation));
                context.Response.ContentType = "text/plain";
                context.Response.Write(XmlExtends.ToString(element, false));
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

