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
    public class dailyloglist : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool flag = false;
            string str = "Fail!";
            XElement element = new XElement("Result");
            try
            {
                string str2 = context.Request["key"];
                int num = int.Parse(context.Request["selfid"]);
                using (ProduceBussiness bussiness = new ProduceBussiness())
                {
                    DailyLogListInfo dailyLogListSingle = bussiness.GetDailyLogListSingle(num);
                    if (dailyLogListSingle == null)
                    {
                        dailyLogListSingle = new DailyLogListInfo {
                            UserID = num,
                            DayLog = "",
                            UserAwardLog = 0,
                            LastDate = DateTime.Now
                        };
                    }
                    string dayLog = dailyLogListSingle.DayLog;
                    int userAwardLog = dailyLogListSingle.UserAwardLog;
                    DateTime lastDate = dailyLogListSingle.LastDate;
                    char[] separator = new char[] { ',' };
                    int length = dayLog.Split(separator).Length;
                    int month = DateTime.Now.Month;
                    int year = DateTime.Now.Year;
                    int day = DateTime.Now.Day;
                    int num7 = DateTime.DaysInMonth(year, month);
                    if ((month != lastDate.Month) || (year != lastDate.Year))
                    {
                        dayLog = "";
                        userAwardLog = 0;
                        lastDate = DateTime.Now;
                    }
                    if (length < num7)
                    {
                        if (string.IsNullOrEmpty(dayLog) && (length > 1))
                        {
                            dayLog = "False";
                        }
                        for (int i = length; i < (day - 1); i++)
                        {
                            dayLog = dayLog + ",False";
                        }
                    }
                    dailyLogListSingle.DayLog = dayLog;
                    dailyLogListSingle.UserAwardLog = userAwardLog;
                    dailyLogListSingle.LastDate = lastDate;
                    bussiness.UpdateDailyLogList(dailyLogListSingle);
                    object[] content = new object[] { new XAttribute("UserAwardLog", userAwardLog), new XAttribute("DayLog", dayLog), new XAttribute("luckyNum", 0), new XAttribute("myLuckyNum", 0) };
                    XElement element2 = new XElement("DailyLogList", content);
                    element.Add(element2);
                }
                flag = true;
                str = "Success!";
            }
            catch (Exception exception)
            {
                log.Error("dailyloglist", exception);
            }
            element.Add(new XAttribute("value", flag));
            element.Add(new XAttribute("message", str));
            element.Add(new XAttribute("nowDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            context.Response.ContentType = "text/plain";
            context.Response.BinaryWrite(StaticFunction.Compress(XmlExtends.ToString(element, false)));
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

