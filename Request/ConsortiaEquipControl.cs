namespace Tank.Request
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Reflection;
    using System.Web;
    using System.Web.Services;
    using System.Xml.Linq;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class ConsortiaEquipControl : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            bool flag = false;
            string str = "Fail!";
            XElement element = new XElement("Result");
            int num = 0;
            try
            {
                int num2 = int.Parse(context.Request["consortiaID"]);
                using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                {
                    for (int i = 1; i < 3; i++)
                    {
                        for (int j = 1; j < 11; j++)
                        {
                            ConsortiaEquipControlInfo info = bussiness.GetConsortiaEuqipRiches(num2, j, i);
                            if (info != null)
                            {
                                object[] content = new object[] { new XAttribute("type", info.Type), new XAttribute("level", info.Level), new XAttribute("riches", info.Riches) };
                                element.Add(new XElement("Item", content));
                                num++;
                            }
                        }
                    }
                    flag = true;
                    str = "Success!";
                }
            }
            catch (Exception exception)
            {
                log.Error("ConsortiaEventList", exception);
            }
            element.Add(new XAttribute("total", num));
            element.Add(new XAttribute("value", flag));
            element.Add(new XAttribute("message", str));
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

