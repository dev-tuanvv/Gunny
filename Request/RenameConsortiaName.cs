namespace Tank.Request
{
    using Bussiness;
    using Bussiness.Interface;
    using log4net;
    using Road.Flash;
    using System;
    using System.Text;
    using System.Web;
    using System.Web.Services;
    using System.Xml.Linq;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class RenameConsortiaName : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string translation = LanguageMgr.GetTranslation("Tank.Request.RenameConsortiaName.Fail1", new object[0]);
            XElement element = new XElement("Result");
            try
            {
                BaseInterface interface2 = BaseInterface.CreateInterface();
                string str2 = context.Request["p"];
                string str3 = (context.Request["site"] == null) ? "" : HttpUtility.UrlDecode(context.Request["site"]);
                string userHostAddress = context.Request.UserHostAddress;
                if (!string.IsNullOrEmpty(str2))
                {
                    byte[] bytes = CryptoHelper.RsaDecryt2(StaticFunction.RsaCryptor, str2);
                    char[] separator = new char[] { ',' };
                    string[] strArray = Encoding.UTF8.GetString(bytes, 7, bytes.Length - 7).Split(separator);
                    if (strArray.Length == 5)
                    {
                        string name = strArray[0];
                        string pass = strArray[1];
                        string str7 = strArray[2];
                        string str8 = strArray[3];
                        string str9 = strArray[4];
                        if (PlayerManager.Login(name, pass))
                        {
                            using (PlayerBussiness bussiness = new PlayerBussiness())
                            {
                                //if (bussiness.RenameConsortiaName(name, str8, str9, ref translation)) //bt90 bo
                                //{
                                //    PlayerManager.Update(name, str7);
                                //    flag = true;
                                //    translation = LanguageMgr.GetTranslation("Tank.Request.RenameConsortiaName.Success", new object[0]);
                                //}
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error("RenameConsortiaName", exception);
                flag = false;
                translation = LanguageMgr.GetTranslation("Tank.Request.RenameConsortiaName.Fail2", new object[0]);
            }
            element.Add(new XAttribute("value", flag));
            element.Add(new XAttribute("message", translation));
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

