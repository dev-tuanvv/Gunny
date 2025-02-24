namespace Tank.Request
{
    using Bussiness.Interface;
    using System;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    public class SentRewardTest : Page
    {
        protected HtmlForm form1;

        protected void Page_Load(object sender, EventArgs e)
        {
            string str = "大幅度是";
            string str2 = "大幅度是";
            string str3 = "watson";
            string str4 = "6666";
            string str5 = "99999";
            string str6 = "11020,4,0,0,0,0,0,0,1|7014,2,9,400,400,400,400,400,0";
            string[] textArray1 = new string[] { str, "#", str2, "#", str3, "#", str4, "#", str5, "#", str6, "#" };
            string s = string.Concat(textArray1);
            DateTime now = DateTime.Now;
            string str8 = "asdfgh";
            string str9 = BaseInterface.md5(string.Concat(new object[] { str3, str4, str5, str6, BaseInterface.ConvertDateTimeInt(now), str8 }));
            object[] objArray2 = new object[] { s, BaseInterface.ConvertDateTimeInt(now), "#", str9 };
            s = string.Concat(objArray2);
            base.Response.Redirect("http://192.168.0.4:828/SentReward.ashx?content=" + base.Server.UrlEncode(s));
        }
    }
}

