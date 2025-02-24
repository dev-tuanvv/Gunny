namespace Tank.Request
{
    using System;
    using System.Web;
    using System.Web.UI;

    public class SendItemTest : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie cookie = base.Request.Cookies["userInfo"];
            string s = cookie.Value;
            string str2 = cookie.Values["bd_sig_user"];
            base.Response.Write(s);
        }
    }
}

