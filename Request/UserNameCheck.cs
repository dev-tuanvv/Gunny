namespace Tank.Request
{
    using Bussiness;
    using Bussiness.Interface;
    using log4net;
    using System;
    using System.Reflection;
    using System.Web;
    using System.Web.UI;

    public class UserNameCheck : Page
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            int num = 1;
            try
            {
                string nameBySite = HttpUtility.UrlDecode(base.Request["username"]);
                string str2 = (base.Request["site"] == null) ? "" : HttpUtility.UrlDecode(base.Request["site"]);
                if (!string.IsNullOrEmpty(nameBySite))
                {
                    nameBySite = BaseInterface.GetNameBySite(nameBySite, str2);
                    using (PlayerBussiness bussiness = new PlayerBussiness())
                    {
                        if (bussiness.GetUserSingleByUserName(nameBySite) != null)
                        {
                            num = 0;
                        }
                        else
                        {
                            num = 2;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error("UserNameCheck:", exception);
            }
            base.Response.Write(num);
        }
    }
}

