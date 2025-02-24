namespace Tank.Request
{
    using Bussiness;
    using log4net;
    using Road.Flash;
    using System;
    using System.Web;
    using System.Xml.Linq;

    public class scenecollecrandomnpc : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "fail!";
            XElement node = new XElement("Result");
            try
            {
                using (new PlayerBussiness())
                {
                    node.Add(FlashUtils.CreatescenecollecrandomnpcInfo());
                    flag = true;
                    str = "Success!";
                }
            }
            catch (Exception exception)
            {
                log.Error("subactivelist", exception);
            }
            node.Add(new XAttribute("value", flag));
            node.Add(new XAttribute("message", str));
            context.Response.ContentType = "text/plain";
            context.Response.BinaryWrite(StaticFunction.Compress(node.ToString(false)));
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

