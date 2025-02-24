namespace Tank.Request
{
    using log4net;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Web;
    using Tank.Request.CelebList;

    public static class StaticsMgr
    {
        private static long _interval;
        private static List<string> _list = new List<string>();
        private static object _locker = new object();
        private static string _path;
        private static Timer _timer;
        private static int CelebBuildDay;
        public static string CurrentPath;
        private static int did;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static int pid;
        private static int RegCount = 0;
        private static int sid;

        public static void Log(DateTime dt, string username, bool sex, int money, string payway, decimal needMoney)
        {
            object[] args = new object[] { pid, did, sid, dt.ToString("yyyy-MM-dd HH:mm:ss"), username, sex ? 1 : 0, money, payway, needMoney };
            string item = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", args);
            object obj2 = _locker;
            lock (obj2)
            {
                _list.Add(item);
            }
        }

        private static void OnTimer(object state)
        {
            try
            {
                object obj2 = _locker;
                lock (obj2)
                {
                    if (_list.Count > 0)
                    {
                        object[] args = new object[] { _path, pid, did, sid, DateTime.Now };
                        using (FileStream stream = File.Open(string.Format(@"{0}\payment-{1:D2}{2:D2}{3:D2}-{4:yyyyMMdd}.log", args), FileMode.Append))
                        {
                            using (StreamWriter writer = new StreamWriter(stream))
                            {
                                while (_list.Count > 0)
                                {
                                    writer.WriteLine(_list[0]);
                                    _list.RemoveAt(0);
                                }
                            }
                        }
                    }
                    if (RegCount > 0)
                    {
                        object[] objArray2 = new object[] { _path, pid, did, sid, DateTime.Now };
                        using (FileStream stream2 = File.Open(string.Format(@"{0}\reg-{1:D2}{2:D2}{3:D2}-{4:yyyyMMdd}.log", objArray2), FileMode.Append))
                        {
                            using (StreamWriter writer2 = new StreamWriter(stream2))
                            {
                                object[] objArray3 = new object[] { pid, did, sid, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), RegCount };
                                string str3 = string.Format("{0},{1},{2},{3},{4}", objArray3);
                                writer2.WriteLine(str3);
                                RegCount = 0;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error("Save log error", exception);
            }
            if (((CelebBuildDay != DateTime.Now.Day) && (DateTime.Now.Hour > 2)) && (DateTime.Now.Hour < 6))
            {
                CelebBuildDay = DateTime.Now.Day;
                StringBuilder builder = new StringBuilder();
                try
                {
                    builder.Append(CelebByGpList.Build());
                    builder.Append(CelebByDayGPList.Build());
                    builder.Append(CelebByWeekGPList.Build());
                    builder.Append(CelebByOfferList.Build());
                    builder.Append(CelebByDayOfferList.Build());
                    builder.Append(CelebByWeekOfferList.Build());
                    builder.Append(CelebByDayFightPowerList.Build());
                    builder.Append(CelebByConsortiaRiches.Build());
                    builder.Append(CelebByConsortiaDayRiches.Build());
                    builder.Append(CelebByConsortiaWeekRiches.Build());
                    builder.Append(CelebByConsortiaHonor.Build());
                    builder.Append(CelebByConsortiaDayHonor.Build());
                    builder.Append(CelebByConsortiaWeekHonor.Build());
                    builder.Append(CelebByConsortiaLevel.Build());
                    builder.Append(CelebByDayBestEquip.Build());
                    log.Info("Complete auto update Celeb in " + DateTime.Now.ToString());
                }
                catch (Exception exception2)
                {
                    builder.Append("CelebByList is Error!");
                    log.Error(builder.ToString(), exception2);
                }
            }
        }

        public static void RegCountAdd()
        {
            object obj2 = _locker;
            lock (obj2)
            {
                RegCount++;
            }
        }

        public static void Setup()
        {
            CurrentPath = HttpContext.Current.Server.MapPath("~");
            CelebBuildDay = DateTime.Now.Day;
            pid = int.Parse(ConfigurationManager.AppSettings["PID"]);
            did = int.Parse(ConfigurationManager.AppSettings["DID"]);
            sid = int.Parse(ConfigurationManager.AppSettings["SID"]);
            _path = ConfigurationManager.AppSettings["LogPath"];
            _interval = (int.Parse(ConfigurationManager.AppSettings["LogInterval"]) * 60) * 0x3e8;
            _timer = new Timer(new TimerCallback(StaticsMgr.OnTimer), null, 0L, _interval);
        }

        public static void Stop()
        {
            _timer.Dispose();
            OnTimer(null);
        }
    }
}

