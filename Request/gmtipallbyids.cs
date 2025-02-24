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

    public class gmtipallbyids : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            XElement element = new XElement("Result");
            try
            {
                string str2 = context.Request["ids"];
                string[] strArray = null;
                if (!string.IsNullOrEmpty(str2))
                {
                    char[] separator = new char[] { ',' };
                    strArray = str2.Split(separator);
                }
                if (strArray != null)
                {
                    using (ProduceBussiness bussiness = new ProduceBussiness())
                    {
                        EdictumInfo[] allEdictum = bussiness.GetAllEdictum();
                        foreach (EdictumInfo info in allEdictum)
                        {
                            info.ID = (int.Parse(strArray[0]));
                            if (info.EndDate.Date > DateTime.Now.Date)
                            {
                                element.Add(FlashUtils.CreateEdictum(info));
                            }
                        }
                        flag = true;
                        str = "Success!";
                    }
                }
            }
            catch (Exception exception)
            {
                str = exception.ToString();
            }
            finally
            {
                element.Add(new XAttribute("value", flag));
                element.Add(new XAttribute("message", str));
                context.Response.ContentType = "text/plain";
                context.Response.Write(XmlExtends.ToString(element, false));
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

