namespace Bussiness
{
    using DAL;
    using log4net;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class CountBussiness
    {
        private static int _appID;
        private static string _connectionString;
        private static bool _conutRecord;
        private static int _serverID;
        private static int _subID;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void InsertContentCount(Dictionary<string, string> clientInfos)
        {
            try
            {
                if (CountRecord)
                {
                    SqlHelper.BeginExecuteNonQuery(ConnectionString, "Modify_Count_Content", new object[] { 
                        clientInfos["Application_Id"], clientInfos["Cpu"], clientInfos["OperSystem"], clientInfos["IP"], clientInfos["IPAddress"], clientInfos["NETCLR"], clientInfos["Browser"], clientInfos["ActiveX"], clientInfos["Cookies"], clientInfos["CSS"], clientInfos["Language"], clientInfos["Computer"], clientInfos["Platform"], clientInfos["Win16"], clientInfos["Win32"], clientInfos["Referry"], 
                        clientInfos["Redirect"], clientInfos["TimeSpan"], clientInfos["ScreenWidth"] + clientInfos["ScreenHeight"], clientInfos["Color"], clientInfos["Flash"], "Insert"
                     });
                }
            }
            catch (Exception exception)
            {
                log.Error("Insert Log Error!!!!", exception);
            }
        }

        public static void InsertGameInfo(DateTime begin, int mapID, int money, int gold, string users)
        {
            InsertGameInfo(AppID, SubID, ServerID, begin, DateTime.Now, users.Split(new char[] { ',' }).Length, mapID, money, gold, users);
        }

        public static void InsertGameInfo(int appid, int subid, int serverid, DateTime begin, DateTime end, int usercount, int mapID, int money, int gold, string users)
        {
            try
            {
                if (CountRecord)
                {
                    SqlHelper.BeginExecuteNonQuery(ConnectionString, "SP_Insert_Count_FightInfo", new object[] { appid, subid, serverid, begin, end, usercount, mapID, money, gold, users });
                }
            }
            catch (Exception exception)
            {
                log.Error("Insert Log Error!", exception);
            }
        }

        public static void InsertServerInfo(int usercount, int gamecount)
        {
            InsertServerInfo(AppID, SubID, ServerID, usercount, gamecount, DateTime.Now);
        }

        public static void InsertServerInfo(int appid, int subid, int serverid, int usercount, int gamecount, DateTime time)
        {
            try
            {
                if (CountRecord)
                {
                    SqlHelper.BeginExecuteNonQuery(ConnectionString, "SP_Insert_Count_Server", new object[] { appid, subid, serverid, usercount, gamecount, time });
                }
            }
            catch (Exception exception)
            {
                log.Error("Insert Log Error!!", exception);
            }
        }

        public static void InsertSystemPayCount(int consumerid, int money, int gold, int consumertype, int subconsumertype)
        {
            InsertSystemPayCount(AppID, SubID, consumerid, money, gold, consumertype, subconsumertype, DateTime.Now);
        }

        public static void InsertSystemPayCount(int appid, int subid, int consumerid, int money, int gold, int consumertype, int subconsumertype, DateTime datime)
        {
            try
            {
                if (CountRecord)
                {
                    SqlHelper.BeginExecuteNonQuery(ConnectionString, "SP_Insert_Count_SystemPay", new object[] { appid, subid, consumerid, money, gold, consumertype, subconsumertype, datime });
                }
            }
            catch (Exception exception)
            {
                log.Error("InsertSystemPayCount Log Error!!!", exception);
            }
        }

        public static void SetConfig(string connectionString, int appID, int subID, int serverID, bool countRecord)
        {
            _connectionString = connectionString;
            _appID = appID;
            _subID = subID;
            _serverID = serverID;
            _conutRecord = countRecord;
        }

        public static int AppID
        {
            get
            {
                return _appID;
            }
        }

        public static string ConnectionString
        {
            get
            {
                return _connectionString;
            }
        }

        public static bool CountRecord
        {
            get
            {
                return _conutRecord;
            }
        }

        public static int ServerID
        {
            get
            {
                return _serverID;
            }
        }

        public static int SubID
        {
            get
            {
                return _subID;
            }
        }
    }
}

