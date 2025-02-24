namespace Tank.Request
{
    using Bussiness;
    using Bussiness.CenterService;
    using log4net;
    using Road.Flash;
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Services;
    using System.Xml.Linq;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class ServerList : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            int num = 0;
            XElement element = new XElement("Result");
            try
            {
                using (CenterServiceClient client = new CenterServiceClient())
                {
                    IList<ServerData> serverList = client.GetServerList();
                    foreach (ServerData data in serverList)
                    {
                        if (data.State != -1)
                        {
                            num += data.Online;
                            element.Add(FlashUtils.CreateServerInfo(data.Id, data.Name, data.Ip, data.Port, data.State, data.MustLevel, data.LowestLevel, data.Online));
                        }
                    }
                }
                flag = true;
                str = "Success!";
            }
            catch (Exception exception)
            {
                log.Error("Load server list error:", exception);
            }
            element.Add(new XAttribute("value", flag));
            element.Add(new XAttribute("message", str));
            element.Add(new XAttribute("total", num));
            context.Response.ContentType = "text/plain";
            context.Response.Write(XmlExtends.ToString(element, false));
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

