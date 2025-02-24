namespace Bussiness.Interface
{
    using Bussiness;
    using Bussiness.CenterService;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Configuration;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web.Security;
    using System.Reflection;

    public abstract class BaseInterface
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected BaseInterface()
        {
        }

        public static int ConvertDateTimeInt(DateTime time)
        {
            DateTime time2 = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(0x7b2, 1, 1));
            TimeSpan span = (TimeSpan) (time - time2);
            return (int) span.TotalSeconds;
        }

        public static DateTime ConvertIntDateTime(double d)
        {
            return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(0x7b2, 1, 1)).AddSeconds(d);
        }

        public static BaseInterface CreateInterface()
        {
            switch (GetInterName)
            {
                case "qunying":
                    return new QYInterface();

                case "sevenroad":
                    return new SRInterface();

                case "duowan":
                    return new DWInterface();
            }
            return null;
        }

        public virtual PlayerInfo CreateLogin(string name, string password, ref string message, ref int isFirst, string IP, ref bool isError, bool firstValidate, ref bool isActive, string site, string nickname)
        {
            try
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    bool isExist = true;
                    DateTime now = DateTime.Now;
                    PlayerInfo player = bussiness.LoginGame(name, ref isFirst, ref isExist, ref isError, firstValidate, ref now, nickname);
                    if (player == null)
                    {
                        if (!bussiness.ActivePlayer(ref player, name, password, true, this.ActiveGold, this.ActiveMoney, IP, site))
                        {
                            player = null;
                            message = LanguageMgr.GetTranslation("BaseInterface.LoginAndUpdate.Fail", new object[0]);
                            return player;
                        }
                        isActive = true;
                        using (CenterServiceClient client = new CenterServiceClient())
                        {
                            client.ActivePlayer(true);
                            return player;
                        }
                    }
                    if (isExist)
                    {
                        using (CenterServiceClient client2 = new CenterServiceClient())
                        {
                            client2.CreatePlayer(player.ID, name, password, isFirst == 0);
                            return player;
                        }
                    }
                    message = LanguageMgr.GetTranslation("ManageBussiness.Forbid1", new object[] { now.Year, now.Month, now.Day, now.Hour, now.Minute });
                    return null;
                }
            }
            catch (Exception exception)
            {
                log.Error("LoginAndUpdate", exception);
            }
            return null;
        }

        public virtual PlayerInfo CreateLogin(string name, string password, ref string message, ref int isFirst, string IP, ref bool isError, bool firstValidate, ref bool isActive, string site, ref string nickname)
        {
            try
            {
                using (PlayerBussiness bussiness = new PlayerBussiness())
                {
                    bool isExist = true;
                    DateTime now = DateTime.Now;
                    PlayerInfo player = bussiness.LoginGame(name, ref isFirst, ref isExist, ref isError, firstValidate, ref now, ref nickname, IP);
                    if (player == null)
                    {
                        if (!bussiness.ActivePlayer(ref player, name, password, true, this.ActiveGold, this.ActiveMoney, IP, site))
                        {
                            player = null;
                            message = LanguageMgr.GetTranslation("BaseInterface.LoginAndUpdate.Fail", new object[0]);
                            return player;
                        }
                        isActive = true;
                        using (CenterServiceClient client = new CenterServiceClient())
                        {
                            client.ActivePlayer(true);
                            return player;
                        }
                    }
                    if (isExist)
                    {
                        using (CenterServiceClient client2 = new CenterServiceClient())
                        {
                            client2.CreatePlayer(player.ID, name, password, isFirst == 0);
                            return player;
                        }
                    }
                    message = LanguageMgr.GetTranslation("ManageBussiness.Forbid1", new object[] { now.Year, now.Month, now.Day, now.Hour, now.Minute });
                    return null;
                }
            }
            catch (Exception exception)
            {
                log.Error("LoginAndUpdate", exception);
            }
            return null;
        }

        public static string GetNameBySite(string user, string site)
        {
            if (!string.IsNullOrEmpty(site))
            {
                string str = ConfigurationManager.AppSettings[string.Format("LoginKey_{0}", site)];
                if (!string.IsNullOrEmpty(str))
                {
                    user = string.Format("{0}_{1}", site, user);
                }
            }
            return user;
        }

        public virtual bool GetUserSex(string name)
        {
            return true;
        }

        public virtual PlayerInfo LoginGame(string name, string pass, ref bool isFirst)
        {
            try
            {
                using (CenterServiceClient client = new CenterServiceClient())
                {
                    int userID = 0;
                    if (client.ValidateLoginAndGetID(name, pass, ref userID, ref isFirst))
                    {
                        return new PlayerInfo { ID = userID, UserName = name };
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error("LoginGame", exception);
            }
            return null;
        }

        public static string md5(string str)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5").ToLower();
        }

        public static string RequestContent(string Url)
        {
            return RequestContent(Url, 0xa00);
        }

        public static string RequestContent(string Url, int byteLength)
        {
            byte[] buffer = new byte[byteLength];
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(Url);
            request.ContentType = "text/plain";
            Stream responseStream = ((HttpWebResponse) request.GetResponse()).GetResponseStream();
            int count = responseStream.Read(buffer, 0, buffer.Length);
            string str = Encoding.UTF8.GetString(buffer, 0, count);
            responseStream.Close();
            return str;
        }

        public static string RequestContent(string Url, string param, string code)
        {
            Encoding encoding = Encoding.GetEncoding(code);
            byte[] bytes = encoding.GetBytes(param);
            encoding.GetString(bytes);
            byte[] buffer = new byte[0xa00];
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(Url);
            request.ServicePoint.Expect100Continue = false;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            using (WebResponse response = request.GetResponse())
            {
                HttpWebResponse response2 = (HttpWebResponse) response;
                int count = response2.GetResponseStream().Read(buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer, 0, count);
            }
        }

        public virtual string[] UnEncryptCharge(string content, ref int result, string site)
        {
            try
            {
                string getChargeKey = string.Empty;
                if (!string.IsNullOrEmpty(site))
                {
                    getChargeKey = ConfigurationManager.AppSettings[string.Format("ChargeKey_{0}", site)];
                }
                if (string.IsNullOrEmpty(getChargeKey))
                {
                    getChargeKey = GetChargeKey;
                }
                if (!string.IsNullOrEmpty(getChargeKey))
                {
                    string[] strArray = content.Split(new char[] { '|' });
                    string str2 = md5(strArray[0] + strArray[1] + strArray[2] + strArray[3] + strArray[4] + getChargeKey);
                    if (strArray.Length > 5)
                    {
                        if (str2 == strArray[5].ToLower())
                        {
                            return strArray;
                        }
                        result = 7;
                    }
                    else
                    {
                        result = 8;
                    }
                }
                else
                {
                    result = 6;
                }
            }
            catch (Exception exception)
            {
                log.Error("UnEncryptCharge", exception);
            }
            return new string[0];
        }

        public virtual string[] UnEncryptLogin(string content, ref int result, string site)
        {
            try
            {
                string getLoginKey = string.Empty;
                if (!string.IsNullOrEmpty(site))
                {
                    getLoginKey = ConfigurationManager.AppSettings[string.Format("LoginKey_{0}", site)];
                }
                if (string.IsNullOrEmpty(getLoginKey))
                {
                    getLoginKey = GetLoginKey;
                }
                if (!string.IsNullOrEmpty(getLoginKey))
                {
                    if (content != null)
                    {
                        string[] strArray = content.Split(new char[] { '|' });
                        if (strArray.Length > 3)
                        {
                            if (md5(strArray[0] + strArray[1] + strArray[2] + getLoginKey) == strArray[3].ToLower())
                            {
                                return strArray;
                            }
                            result = 5;
                        }
                        else
                        {
                            result = 2;
                        }
                    }
                }
                else
                {
                    result = 4;
                }
            }
            catch (Exception exception)
            {
                log.Error("UnEncryptLogin", exception);
            }
            return new string[0];
        }

        public virtual string[] UnEncryptSentReward(string content, ref int result, string key)
        {
            try
            {
                string[] strArray = content.Split(new char[] { '#' });
                if (strArray.Length == 8)
                {
                    string str = ConfigurationManager.AppSettings["SentRewardTimeSpan"];
                    int num = int.Parse(string.IsNullOrEmpty(str) ? "1" : str);
                    TimeSpan span = string.IsNullOrEmpty(strArray[6]) ? new TimeSpan(1, 1, 1) : ((TimeSpan) (DateTime.Now - ConvertIntDateTime(double.Parse(strArray[6]))));
                    if (((span.Days == 0) && (span.Hours == 0)) && (span.Minutes < num))
                    {
                        if (string.IsNullOrEmpty(key))
                        {
                            return strArray;
                        }
                        if (md5(strArray[2] + strArray[3] + strArray[4] + strArray[5] + strArray[6] + key) == strArray[7].ToLower())
                        {
                            return strArray;
                        }
                        result = 5;
                    }
                    else
                    {
                        result = 7;
                    }
                }
                else
                {
                    result = 6;
                }
            }
            catch (Exception exception)
            {
                log.Error("UnEncryptSentReward", exception);
            }
            return new string[0];
        }

        public virtual int ActiveGold
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["DefaultGold"]);
            }
        }

        public virtual int ActiveMoney
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["DefaultMoney"]);
            }
        }

        public static string GetChargeKey
        {
            get
            {
                return ConfigurationManager.AppSettings["ChargeKey"];
            }
        }

        public static string GetInterName
        {
            get
            {
                return ConfigurationManager.AppSettings["InterName"].ToLower();
            }
        }

        public static string GetLoginKey
        {
            get
            {
                return ConfigurationManager.AppSettings["LoginKey"];
            }
        }

        public static string LoginUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["LoginUrl"];
            }
        }
    }
}

