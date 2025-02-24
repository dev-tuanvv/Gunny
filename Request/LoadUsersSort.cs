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
    public class LoadUsersSort : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            XElement element = new XElement("Result");
            int num = 0;
            try
            {
                int num2 = 1;
                int num3 = 10;
                int num4 = int.Parse(context.Request["order"]);
                int num5 = -1;
                bool flag2 = false;
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    PlayerInfo[] infoArray = bussiness.GetPlayerPage(num2, num3, ref num, num4, num5, ref flag2);
                    if (flag2)
                    {
                        foreach (PlayerInfo info in infoArray)
                        {
                            object[] content = new object[] { 
                                new XAttribute("ID", info.ID), new XAttribute("NickName", (info.NickName == null) ? "" : info.NickName), new XAttribute("Grade", info.Grade), new XAttribute("Colors", (info.Colors == null) ? "" : info.Colors), new XAttribute("Skin", (info.Skin == null) ? "" : info.Skin), new XAttribute("Sex", info.Sex), new XAttribute("Style", (info.Style == null) ? "" : info.Style), new XAttribute("ConsortiaName", (info.ConsortiaName == null) ? "" : info.ConsortiaName), new XAttribute("Hide", info.Hide), new XAttribute("Offer", info.Offer), new XAttribute("ReputeOffer", info.ReputeOffer), new XAttribute("ConsortiaHonor", info.ConsortiaHonor), new XAttribute("ConsortiaLevel", info.ConsortiaLevel), new XAttribute("ConsortiaRepute", info.ConsortiaRepute), new XAttribute("WinCount", info.Win), new XAttribute("TotalCount", info.Total), 
                                new XAttribute("EscapeCount", info.Escape), new XAttribute("Repute", info.Repute), new XAttribute("GP", info.GP)
                             };
                            XElement element2 = new XElement("Item", content);
                            element.Add(element2);
                        }
                        flag = true;
                        str = "Success!";
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error("LoadUsersSort", exception);
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

