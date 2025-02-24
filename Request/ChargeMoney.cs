namespace Tank.Request
{
    using Bussiness;
    using Bussiness.CenterService;
    using Bussiness.Interface;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using System.Web.UI;

    public class ChargeMoney : Page
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            int num = 1;
            try
            {
                string userHostAddress = this.Context.Request.UserHostAddress;
                if (ValidLoginIP(userHostAddress))
                {
                    string str2 = HttpUtility.UrlDecode(base.Request["content"]);
                    string str3 = (base.Request["site"] == null) ? "" : HttpUtility.UrlDecode(base.Request["site"]).ToLower();
                    string str4 = (base.Request["nickname"] == null) ? "" : HttpUtility.UrlDecode(base.Request["nickname"]);
                    string[] strArray = BaseInterface.CreateInterface().UnEncryptCharge(str2, ref num, str3);
                    if (strArray.Length > 5)
                    {
                        string str5 = strArray[0];
                        string nameBySite = strArray[1].Trim();
                        int money = int.Parse(strArray[2]);
                        string payway = strArray[3];
                        decimal needMoney = decimal.Parse(strArray[4]);
                        if (!string.IsNullOrEmpty(nameBySite))
                        {
                            nameBySite = BaseInterface.GetNameBySite(nameBySite, str3);
                            if (money > 0)
                            {
                                using (PlayerBussiness bussiness = new PlayerBussiness())
                                {
                                    int num4 =0;
                                    DateTime now = DateTime.Now;
                                    if (bussiness.AddChargeMoney(str5, nameBySite, money, payway, needMoney, ref num4, ref num, now, userHostAddress, str4))
                                    {
                                        num = 0;
                                        using (CenterServiceClient client = new CenterServiceClient())
                                        {
                                            client.ChargeMoney(num4, str5);
                                            using (PlayerBussiness bussiness2 = new PlayerBussiness())
                                            {
                                                PlayerInfo userSingleByUserID = bussiness2.GetUserSingleByUserID(num4);
                                                if (userSingleByUserID != null)
                                                {
                                                    StaticsMgr.Log(now, nameBySite, userSingleByUserID.Sex, money, payway, needMoney);
                                                }
                                                else
                                                {
                                                    StaticsMgr.Log(now, nameBySite, true, money, payway, needMoney);
                                                    log.Error("ChargeMoney_StaticsMgr:Player is null!");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                num = 3;
                            }
                        }
                        else
                        {
                            num = 2;
                        }
                    }
                }
                else
                {
                    num = 5;
                }
            }
            catch (Exception exception)
            {
                log.Error("ChargeMoney:", exception);
            }
            base.Response.Write(num);
        }

        public static bool ValidLoginIP(string ip)
        {
            string getChargeIP = GetChargeIP;
            return (string.IsNullOrEmpty(getChargeIP) || getChargeIP.Split(new char[] { '|' }).Contains<string>(ip));
        }

        public static string GetChargeIP
        {
            get
            {
                return ConfigurationSettings.AppSettings["ChargeIP"];
            }
        }
    }
}

