namespace Tank.Request
{
    using System;
    using System.Web;
    using System.Xml.Linq;

    public class SavePlayerActions : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            int num = Convert.ToInt32(context.Request["selfid"]);
            string str = context.Request["key"];
            bool flag = false;
            string str2 = "fail!";
            XElement element = new XElement("Result");
            flag = true;
            str2 = "Success!";
            element.Add(new XAttribute("value", flag));
            element.Add(new XAttribute("message", str2));
            context.Response.ContentType = "text/plain";
            context.Response.Write(element.ToString());
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

