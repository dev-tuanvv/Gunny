namespace Tank.Request
{
    using Bussiness;
    using System;
    using System.Web;
    using System.Xml.Linq;

    public class GiftRecieveLog : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            XElement node = new XElement("Result");
            try
            {
                flag = true;
                str = "Success!";
            }
            catch (Exception)
            {
            }
            finally
            {
                node.Add(new XAttribute("value", flag));
                node.Add(new XAttribute("message", str));
                context.Response.ContentType = "text/plain";
                context.Response.BinaryWrite(StaticFunction.Compress(node.ToString(false)));
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

