namespace Tank.Request
{
    using Bussiness;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Xml.Linq;

    public class eventrewarditemlist : IHttpHandler
    {
        public static string Bulid(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            XElement result = new XElement("Result");
            try
            {
                using (ProduceBussiness bussiness = new ProduceBussiness())
                {
                    Dictionary<int, Dictionary<int, EventRewardInfo>> dictionary = new Dictionary<int, Dictionary<int, EventRewardInfo>>();
                    EventRewardInfo[] allEventRewardInfo = bussiness.GetAllEventRewardInfo();
                    EventRewardGoodsInfo[] allEventRewardGoods = bussiness.GetAllEventRewardGoods();
                    Dictionary<int, EventRewardInfo> dictionary2 = null;
                    foreach (EventRewardInfo info in allEventRewardInfo)
                    {
                        info.AwardLists = (new List<EventRewardGoodsInfo>());
                        if (!dictionary.ContainsKey(info.ActivityType))
                        {
                            dictionary2 = new Dictionary<int, EventRewardInfo>();
                            dictionary2.Add(info.SubActivityType, info);
                            dictionary.Add(info.ActivityType, dictionary2);
                        }
                        else if (!dictionary[info.ActivityType].ContainsKey(info.SubActivityType))
                        {
                            dictionary[info.ActivityType].Add(info.SubActivityType, info);
                        }
                    }
                    foreach (EventRewardGoodsInfo info2 in allEventRewardGoods)
                    {
                        if (dictionary.ContainsKey(info2.ActivityType) && dictionary[info2.ActivityType].ContainsKey(info2.SubActivityType))
                        {
                            dictionary[info2.ActivityType][info2.SubActivityType].AwardLists.Add(info2);
                        }
                    }
                    XElement content = null;
                    foreach (Dictionary<int, EventRewardInfo> dictionary3 in dictionary.Values)
                    {
                        foreach (EventRewardInfo info3 in dictionary3.Values)
                        {
                            if (content == null)
                            {
                                content = new XElement("ActivityType", new XAttribute("value", info3.ActivityType));
                            }
                            object[] objArray1 = new object[] { new XAttribute("SubActivityType", info3.SubActivityType), new XAttribute("Condition", info3.Condition) };
                            XElement element3 = new XElement("Items", objArray1);
                            foreach (EventRewardGoodsInfo info4 in info3.AwardLists)
                            {
                                object[] objArray2 = new object[] { new XAttribute("TemplateId", info4.TemplateId), new XAttribute("StrengthLevel", info4.StrengthLevel), new XAttribute("AttackCompose", info4.AttackCompose), new XAttribute("DefendCompose", info4.DefendCompose), new XAttribute("LuckCompose", info4.LuckCompose), new XAttribute("AgilityCompose", info4.AgilityCompose), new XAttribute("IsBind", info4.IsBind), new XAttribute("ValidDate", info4.ValidDate), new XAttribute("Count", info4.Count) };
                                XElement element4 = new XElement("Item", objArray2);
                                element3.Add(element4);
                            }
                            content.Add(element3);
                        }
                        result.Add(content);
                        content = null;
                    }
                    flag = true;
                    str = "Success!";
                }
            }
            catch
            {
            }
            result.Add(new XAttribute("value", flag));
            result.Add(new XAttribute("message", str));
            return csFunction.CreateCompressXml(context, result, "eventrewarditemlist", true);
        }

        public void ProcessRequest(HttpContext context)
        {
            if (csFunction.ValidAdminIP(context.Request.UserHostAddress))
            {
                context.Response.Write(Bulid(context));
            }
            else
            {
                context.Response.Write("IP is not valid!");
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

