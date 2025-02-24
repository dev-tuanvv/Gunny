namespace Tank.Request
{
    using Bussiness.Interface;
    using System;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    public class LoginTest : Page
    {
        protected HtmlForm form1;

        protected void Page_Load(object sender, EventArgs e)
        {
            string str = "onelife";
            string str2 = "733789";
            int num = 0x4ad04d57;
            string str4 = BaseInterface.md5(str + str2 + num.ToString() + "yk-MotL-qhpAo88-7road-mtl55dantang-login-logddt777");
            string[] textArray1 = new string[] { str, "|", str2, "|", num.ToString(), "|", str4 };
            string str5 = "content=" + HttpUtility.UrlEncode(string.Concat(textArray1));
            string[] textArray2 = new string[] { str, "|", str2, "|", num.ToString(), "|", str4 };
            string s = BaseInterface.RequestContent("http://localhost:728/CreateLogin.aspx?content=" + HttpUtility.UrlEncode(string.Concat(textArray2)));
            base.Response.Write(s);
        }
    }
}

