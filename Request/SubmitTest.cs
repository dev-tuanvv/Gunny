namespace Tank.Request
{
    using Bussiness;
    using System;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    public class SubmitTest : Page
    {
        protected Button Button1;
        protected HtmlForm form1;
        protected TextBox TextBox1;

        protected void Button1_Click(object sender, EventArgs e)
        {
            base.Response.Redirect("/LoginTest.aspx?name=" + this.TextBox1.Text);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            using (new ConsortiaBussiness())
            {
            }
        }
    }
}

