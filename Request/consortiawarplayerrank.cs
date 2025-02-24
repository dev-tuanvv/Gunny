namespace Tank.Request
{
    using Bussiness;
    using System;
    using System.Web;
    using System.Xml.Linq;

    public class consortiawarplayerrank : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string str = context.Request["ConsortiaID"];
            string str2 = context.Request["UserID"];
            bool flag = false;
            string str3 = "fail!";
            XElement node = new XElement("Result");
            if (!(string.IsNullOrEmpty(str) || string.IsNullOrEmpty(str2)))
            {
                XElement content = new XElement("Item", new object[] { new XAttribute("Rank", 1), new XAttribute("ConsortiaID", str), new XAttribute("Name", "Ủn ỉn để thương"), new XAttribute("Score", 0x63), new XAttribute("UserID", str2), new XAttribute("ZoneName", "Ủn ỉn"), new XAttribute("ZoneID", 4) });
                node.Add(content);
                flag = true;
                str3 = "Success!";
            }
            node.Add(new XAttribute("value", flag));
            node.Add(new XAttribute("message", str3));
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

