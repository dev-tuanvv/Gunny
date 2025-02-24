namespace Tank.Request
{
    using Bussiness;
    using log4net;
    using Road.Flash;
    using SqlDataProvider.Data;
    using System;
    using System.Reflection;
    using System.Web;
    using System.Xml.Linq;

    public class subactivelist : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "fail!";
            XElement node = new XElement("Result");
            try
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    SubActiveInfo[] allSubActive = bussiness.GetAllSubActive();
                    foreach (SubActiveInfo info in allSubActive)
                    {
                        node.Add(FlashUtils.CreateActiveInfo(info));
                        SubActiveConditionInfo[] allSubActiveCondition = bussiness.GetAllSubActiveCondition(info.ActiveID);
                        foreach (SubActiveConditionInfo info2 in allSubActiveCondition)
                        {
                            node.Add(FlashUtils.CreateActiveConditionInfo(info2));
                        }
                    }
                    flag = true;
                    str = "Success!";
                }
            }
            catch (Exception exception)
            {
                log.Error("subactivelist", exception);
            }
            node.Add(new XAttribute("value", flag));
            node.Add(new XAttribute("nowTime", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")));
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

