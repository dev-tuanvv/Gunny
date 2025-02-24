namespace Tank.Request
{
    using Bussiness;
    using Bussiness.CenterService;
    using log4net;
    using Road.Flash;
    using System;
    using System.Reflection;
    using System.Web.UI;
    using System.Xml.Linq;

    public class ServerList1 : Page
    {
        private static DateTime date = DateTime.Now;
        private static ServerData[] infos;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static int OnlineTotal = 0;
        private static string xml = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            int num = (base.Request["id"] == null) ? -1 : int.Parse(base.Request["id"]);
            if ((infos == null) || (date.AddMinutes(5.0).CompareTo(DateTime.Now) < 0))
            {
                bool flag2 = false;
                string str2 = "Fail!";
                int num2 = 0;
                XElement element = new XElement("Result");
                try
                {
                    using (CenterServiceClient client = new CenterServiceClient())
                    {
                        infos = client.GetServerList();
                        date = DateTime.Now;
                    }
                    foreach (ServerData data in infos)
                    {
                        if (data.State != -1)
                        {
                            num2 += data.Online;
                            element.Add(FlashUtils.CreateServerInfo(data.Id, data.Name, data.Ip, data.Port, data.State, data.MustLevel, data.LowestLevel, data.Online));
                        }
                    }
                    flag2 = true;
                    str2 = "Success!";
                }
                catch (Exception exception)
                {
                    log.Error("ServerList1 error:", exception);
                }
                OnlineTotal = num2;
                element.Add(new XAttribute("value", flag2));
                element.Add(new XAttribute("message", str2));
                element.Add(new XAttribute("total", num2));
                xml = XmlExtends.ToString(element, false);
            }
            string s = "0";
            if (num == 0)
            {
                s = OnlineTotal.ToString();
            }
            else if (num > 0)
            {
                foreach (ServerData data2 in infos)
                {
                    if (data2.Id == num)
                    {
                        s = data2.Online.ToString();
                        break;
                    }
                }
            }
            else
            {
                s = xml;
            }
            base.Response.Write(s);
        }
    }
}

