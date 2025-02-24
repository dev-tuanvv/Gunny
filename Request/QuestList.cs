namespace Tank.Request
{
    using Bussiness;
    using log4net;
    using Road.Flash;
    using SqlDataProvider.Data;
    using System;
    using System.Collections;
    using System.Linq;
    using System.Web;
    using System.Web.Services;
    using System.Xml;
    using System.Xml.Linq;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class QuestList : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static void AppendAttribute(XmlDocument doc, XmlNode node, string attr, string value)
        {
            XmlAttribute attribute = doc.CreateAttribute(attr);
            attribute.Value = value;
            node.Attributes.Append(attribute);
        }

        public static string Bulid(HttpContext context)
        {
            bool value = false;
            string message = "Fail!";
            XElement result = new XElement("Result");

            try
            {
                using (ProduceBussiness db = new ProduceBussiness())
                {
                    QuestInfo[] quests = db.GetALlQuest();
                    QuestAwardInfo[] questgoods = db.GetAllQuestGoods();
                    QuestConditionInfo[] questcondiction = db.GetAllQuestCondiction();
                    QuestRateInfo[] questrate = db.GetAllQuestRate();
                    foreach (QuestInfo quest in quests)
                    {
                        //添加任务模板
                        XElement temp_xml = FlashUtils.CreateQuestInfo(quest);

                        //添加任何条件
                        IEnumerable temp_questcondiction = questcondiction.Where(s => s.QuestID == quest.ID);
                        foreach (QuestConditionInfo item1 in temp_questcondiction)
                        {
                            temp_xml.Add(FlashUtils.CreateQuestCondiction(item1));
                        }

                        //添加任务奖励
                        IEnumerable temp_questgoods = questgoods.Where(s => s.QuestID == quest.ID);
                        foreach (QuestAwardInfo item2 in temp_questgoods)
                        {
                            temp_xml.Add(FlashUtils.CreateQuestGoods(item2));
                        }
                        //<Rate BindMoneyRate="1|1|1|1" ExpRate="1.5|2|2.5|3" GoldRate="1.5|2|2.5|3" ExploitRate="1.5|2|2.5|3" CanOneKeyFinishTime="3" />
                        result.Add(temp_xml);
                    }
                    foreach (QuestRateInfo quest in questrate)
                    {
                        XElement temp_xml1 = FlashUtils.CreateQuestRate(quest);
                        result.Add(temp_xml1);
                    }
                    value = true;
                    message = "Success!";
                }
            }
            catch (Exception ex)
            {
                log.Error("QuestList", ex);
            }

            result.Add(new XAttribute("value", value));
            result.Add(new XAttribute("message", message));

            //return result.ToString(false);
            return csFunction.CreateCompressXml(context, result, "QuestList", true);
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

