namespace Tank.Request
{
    using Bussiness;
    using log4net;
    using System;
    using System.Web;
    using System.Web.Services;
    using System.Xml.Linq;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class AdvanceQuestTime : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            XElement element = new XElement("Result");
            try
            {
                int num = int.Parse(context.Request["useid"]);
                using (new PlayerBussiness())
                {
                }
                flag = true;
                str = "Success!";
            }
            catch (Exception exception)
            {
                log.Error("IMListLoad", exception);
            }
            element.Add(new XAttribute("value", flag));
            element.Add(new XAttribute("message", str));
            context.Response.ContentType = "text/plain";
            context.Response.Write(string.Format("0,{0},0", DateTime.Now));
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

