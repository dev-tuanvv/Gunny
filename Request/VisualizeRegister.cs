namespace Tank.Request
{
    using Bussiness;
    using log4net;
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Text;
    using System.Web;
    using System.Web.Services;
    using System.Xml.Linq;
    using Tank.Request.Illegalcharacters;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class VisualizeRegister : IHttpHandler
    {
        private static FileSystem fileIllegal = new FileSystem(HttpContext.Current.Server.MapPath(IllegalCharacters), HttpContext.Current.Server.MapPath(IllegalDirectory), "*.txt");
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string translation = LanguageMgr.GetTranslation("Tank.Request.VisualizeRegister.Fail1", new object[0]);
            XElement element = new XElement("Result");
            try
            {
                NameValueCollection @params = context.Request.Params;
                string str2 = @params["Name"];
                string str3 = @params["Pass"];
                string s = @params["NickName"].Trim().Replace(",", "");
                string str5 = @params["Arm"];
                string str6 = @params["Hair"];
                string str7 = @params["Face"];
                string str8 = @params["Cloth"];
                string str9 = @params["Cloth"];
                string str10 = @params["ArmID"];
                string str11 = @params["HairID"];
                string str12 = @params["FaceID"];
                string str13 = @params["ClothID"];
                string str14 = @params["ClothID"];
                int num = -1;
                if (bool.Parse(ConfigurationManager.AppSettings["MustSex"]))
                {
                    num = bool.Parse(@params["Sex"]) ? 1 : 0;
                }
                if (Encoding.Default.GetByteCount(s) <= 14)
                {
                    if (!fileIllegal.checkIllegalChar(s))
                    {
                        if ((!string.IsNullOrEmpty(str2) && !string.IsNullOrEmpty(str3)) && !string.IsNullOrEmpty(s))
                        {
                            string[] strArray = (num == 1) ? ConfigurationManager.AppSettings["BoyVisualizeItem"].Split(new char[] { ';' }) : ConfigurationManager.AppSettings["GrilVisualizeItem"].Split(new char[] { ';' });
                            char[] separator = new char[] { ',' };
                            str10 = strArray[0].Split(separator)[0];
                            char[] chArray4 = new char[] { ',' };
                            str11 = strArray[0].Split(chArray4)[1];
                            char[] chArray5 = new char[] { ',' };
                            str12 = strArray[0].Split(chArray5)[2];
                            char[] chArray6 = new char[] { ',' };
                            str13 = strArray[0].Split(chArray6)[3];
                            char[] chArray7 = new char[] { ',' };
                            str14 = strArray[0].Split(chArray7)[4];
                            str5 = "";
                            str6 = "";
                            str7 = "";
                            str8 = "";
                            str9 = "";
                            using (PlayerBussiness bussiness = new PlayerBussiness())
                            {
                                string[] textArray1 = new string[] { str10, ",", str11, ",", str12, ",", str13, ",", str14 };
                                string str15 = string.Concat(textArray1);
                                if (bussiness.RegisterPlayer(str2, str3, s, str15, str15, str5, str6, str7, str8, str9, num, ref translation, int.Parse(ConfigurationManager.AppSettings["ValidDate"])))
                                {
                                    flag = true;
                                    translation = LanguageMgr.GetTranslation("Tank.Request.VisualizeRegister.Success", new object[0]);
                                }
                            }
                        }
                        else
                        {
                            translation = LanguageMgr.GetTranslation("!string.IsNullOrEmpty(name) && !", new object[0]);
                        }
                    }
                    else
                    {
                        translation = LanguageMgr.GetTranslation("Tank.Request.VisualizeRegister.Illegalcharacters", new object[0]);
                    }
                }
                else
                {
                    translation = LanguageMgr.GetTranslation("Tank.Request.VisualizeRegister.Long", new object[0]);
                }
            }
            catch (Exception exception)
            {
                log.Error("VisualizeRegister", exception);
            }
            element.Add(new XAttribute("value", flag));
            element.Add(new XAttribute("message", translation));
            context.Response.ContentType = "text/plain";
            context.Response.Write(XmlExtends.ToString(element, false));
        }

        public static string IllegalCharacters
        {
            get
            {
                return ConfigurationManager.AppSettings["IllegalCharacters"];
            }
        }

        public static string IllegalDirectory
        {
            get
            {
                return ConfigurationManager.AppSettings["IllegalDirectory"];
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

