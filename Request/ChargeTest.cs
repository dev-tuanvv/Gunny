namespace Tank.Request
{
    using Bussiness.Interface;
    using System;
    using System.Configuration;
    using System.Web;
    using System.Web.Services;

    [WebService(Namespace="http://tempuri.org/"), WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
    public class ChargeTest : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string str = context.Request["chargeID"];
                string str2 = context.Request["userName"];
                int num = int.Parse(context.Request["money"]);
                string str3 = context.Request["payWay"];
                decimal num2 = decimal.Parse(context.Request["needMoney"]);
                string str4 = (context.Request["nickname"] == null) ? "" : HttpUtility.UrlDecode(context.Request["nickname"]);
                string str5 = "";
                QYInterface interface2 = new QYInterface();
                string str6 = string.Empty;
                if (!string.IsNullOrEmpty(str5))
                {
                    str6 = ConfigurationSettings.AppSettings[string.Format("ChargeKey_{0}", str5)];
                }
                else
                {
                    str6 = BaseInterface.GetChargeKey;
                }
                string str7 = BaseInterface.md5(string.Concat(new object[] { str, str2, num, str3, num2, str6 }));
                string str8 = (string.Concat(new object[] { "http://192.168.0.4:828/ChargeMoney.aspx?content=", str, "|", str2, "|", num, "|", str3, "|", num2, "|", str7 }) + "&site=" + str5) + "&nickname=" + HttpUtility.UrlEncode(str4);
                context.Response.Write(BaseInterface.RequestContent(str8));
            }
            catch
            {
                context.Response.Write("false");
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

