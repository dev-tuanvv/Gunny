namespace Tank.Request
{
    using Bussiness;
    using log4net;
    using Road.Flash;
    using SqlDataProvider.Data;
    using System;
    using System.Reflection;
    using System.Web;
    using System.Web.Services;
    using System.Xml.Linq;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class LoginSelectList : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            XElement element = new XElement("Result");
            try
            {
                string str2 = HttpUtility.UrlDecode(context.Request["username"]);
                string str3 = HttpUtility.UrlDecode(context.Request["password"]);
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    PlayerInfo[] userLoginList = bussiness.GetUserLoginList(str2);
                    if (userLoginList.Length > 0)
                    {
                        foreach (PlayerInfo info in userLoginList)
                        {
                            if (!(string.IsNullOrEmpty(info.NickName) && (info.Password != str3)))
                            {
                                UserVIPInfo userVIP = bussiness.GetUserVIP(info.ID);
                                element.Add(FlashUtils.CreateUserLoginList(info, userVIP));
                            }
                        }
                        flag = true;
                        str = "Success!";
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error("LoginSelectList", exception);
            }
            finally
            {
                if (flag)
                {
                    element.Add(new XAttribute("value", flag));
                    element.Add(new XAttribute("message", str));
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(XmlExtends.ToString(element, false));
                }
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

