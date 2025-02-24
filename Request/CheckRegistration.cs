namespace Tank.Request
{
    using log4net;
    using System;
    using System.IO;
    using System.Web;
    using System.Web.Services;
    using System.Web.SessionState;
    using System.Xml.Linq;
    using zlib;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class CheckRegistration : IHttpHandler, IRequiresSessionState
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static byte[] Compress(byte[] data)
        {
            MemoryStream stream = new MemoryStream();
            ZOutputStream stream2 = new ZOutputStream(stream, 3);
            stream2.Write(data, 0, data.Length);
            stream2.Flush();
            stream2.Close();
            return stream.ToArray();
        }

        public void ProcessRequest(HttpContext context)
        {
            bool flag = true;
            string str = "Registered!";
            XElement element = new XElement("Result");
            int num = 1;
            element.Add(new XAttribute("value", flag));
            element.Add(new XAttribute("message", str));
            element.Add(new XAttribute("status", num));
            context.Response.ContentType = "text/plain";
            context.Response.BinaryWrite(StaticFunction.Compress(element.ToString()));
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

