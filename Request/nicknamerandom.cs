namespace Tank.Request
{
    using Bussiness;
    using System;
    using System.Web;
    using System.Xml.Linq;

    public class nicknamerandoms : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string singleRandomName = "unknown";
                XElement element = new XElement("Result");
                int sex = Convert.ToInt32(context.Request["sex"]);
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    singleRandomName = bussiness.GetSingleRandomName(sex).Replace(" ", "");
                }
                Random random = new Random();
                singleRandomName = singleRandomName + " " + random.Next(0x6f, 0x3e7);
                element.Add(new XAttribute("name", singleRandomName));
                context.Response.ContentType = "text/plain";
                context.Response.Write(element.ToString());
            }
            catch (Exception)
            {
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

