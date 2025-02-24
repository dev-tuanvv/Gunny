namespace Tank.Request
{
    using Bussiness;
    using System;
    using System.Web;
    using System.Xml.Linq;

    public class teambuyactiveawardinfo : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "Success!";
            XElement node = new XElement("Result");
            node.Add(new XAttribute("value", flag));
            node.Add(new XAttribute("message", str));
            context.Response.ContentType = "text/plain";
            context.Response.Write(node.ToString(false));
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

