namespace Tank.Request
{
    using Bussiness;
    using log4net;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Services;
    using System.Xml.Linq;
    using Tank.Request.Illegalcharacters;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class NickNameCheck : IHttpHandler
    {
        private static string CharacterAllow = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789\x00e1\x00e0ạả\x00e3\x00e2ấầậẩẫăắằặẳẵ\x00c1\x00c0ẠẢ\x00c3\x00c2ẤẦẬẨẪĂẮẰẶẲẴ\x00e9\x00e8ẹẻẽ\x00eaếềệểễ\x00c9\x00c8ẸẺẼ\x00caẾỀỆỂỄ\x00f3\x00f2ọỏ\x00f5\x00f4ốồộổỗơớờợởỡ\x00d3\x00d2ỌỎ\x00d5\x00d4ỐỒỘỔỖƠỚỜỢỞỠ\x00fa\x00f9ụủũưứừựửữ\x00da\x00d9ỤỦŨƯỨỪỰỬỮ\x00ed\x00ecịỉĩ\x00cd\x00ccỊỈĨđĐ\x00fdỳỵỷỹ\x00ddỲỴỶỸ.-_";
        private static FileSystem fileIllegal = new FileSystem(HttpContext.Current.Server.MapPath(IllegalCharacters), HttpContext.Current.Server.MapPath(IllegalDirectory), "*.txt");
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool CheckCharacterAllow(string text)
        {
            foreach (char ch in text.ToCharArray())
            {
                if (!CharacterAllow.ToCharArray().Contains<char>(ch))
                {
                    return false;
                }
            }
            return true;
        }

        public void ProcessRequest(HttpContext context)
        {
            LanguageMgr.Setup(ConfigurationManager.AppSettings["ReqPath"]);
            bool flag = false;
            string translation = LanguageMgr.GetTranslation("Tank.Request.NickNameCheck.Exist", new object[0]);
            XElement element = new XElement("Result");
            try
            {
                string s = csFunction.ConvertSql(HttpUtility.UrlDecode(context.Request["NickName"]));
                if (Encoding.Default.GetByteCount(s) <= 14)
                {
                    if (!fileIllegal.checkIllegalChar(s))
                    {
                        if (this.CheckCharacterAllow(s))
                        {
                            if (!string.IsNullOrEmpty(s))
                            {
                                using (PlayerBussiness bussiness = new PlayerBussiness())
                                {
                                    if (bussiness.GetUserSingleByNickName(s) == null)
                                    {
                                        flag = true;
                                        translation = LanguageMgr.GetTranslation("Tank.Request.NickNameCheck.Right", new object[0]);
                                    }
                                }
                            }
                        }
                        else
                        {
                            translation = "T\x00ean chỉ được ph\x00e9p chứa Tiếng Việt v\x00e0 số";
                        }
                    }
                    else
                    {
                        translation = LanguageMgr.GetTranslation("Tank.Request.VisualizeRegister.Illegalcharacters", new object[0]);
                    }
                }
                else
                {
                    translation = LanguageMgr.GetTranslation("Tank.Request.NickNameCheck.Long", new object[0]);
                }
            }
            catch (Exception exception)
            {
                log.Error("NickNameCheck", exception);
                flag = false;
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

