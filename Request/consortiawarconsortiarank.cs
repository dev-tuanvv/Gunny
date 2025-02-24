namespace Tank.Request
{
    using Bussiness;
    using System;
    using System.Web;
    using System.Xml.Linq;

    public class consortiawarconsortiarank : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "fail!";
            XElement node = new XElement("Result");
            XElement content = new XElement("Item", new object[] { new XAttribute("Rank", 1), new XAttribute("ConsortiaID", 1), new XAttribute("Name", "Ủn ỉn Guild"), new XAttribute("Score", 0x270f) });
            node.Add(content);
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

