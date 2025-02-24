namespace Game.Server.Managers
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Reflection;

    public class RankMgr
    {
        private static Dictionary<int, UserMatchInfo> _matchs;
        protected static Timer _timer;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock = new ReaderWriterLock();

        public static void BeginTimer()
        {
            int dueTime = 0x36ee80;
            if (_timer == null)
            {
                _timer = new Timer(new TimerCallback(RankMgr.TimeCheck), null, dueTime, dueTime);
            }
            else
            {
                _timer.Change(dueTime, dueTime);
            }
        }

        public static UserMatchInfo FindRank(int UserID)
        {
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_matchs.ContainsKey(UserID))
                {
                    return _matchs[UserID];
                }
            }
            catch
            {
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

        public static bool Init()
        {
            try
            {
                m_lock = new ReaderWriterLock();
                _matchs = new Dictionary<int, UserMatchInfo>();
                BeginTimer();
                return ReLoad();
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("RankMgr", exception);
                }
                return false;
            }
        }

        private static bool LoadData(Dictionary<int, UserMatchInfo> Match)
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                foreach (UserMatchInfo info in bussiness.GetAllUserMatchInfo())
                {
                    if (!Match.ContainsKey(info.UserID))
                    {
                        Match.Add(info.UserID, info);
                    }
                }
            }
            return true;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, UserMatchInfo> match = new Dictionary<int, UserMatchInfo>();
                if (LoadData(match))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        _matchs = match;
                        return true;
                    }
                    catch
                    {
                    }
                    finally
                    {
                        m_lock.ReleaseWriterLock();
                    }
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("RankMgr", exception);
                }
            }
            return false;
        }

        public void StopTimer()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        protected static void TimeCheck(object sender)
        {
            try
            {
                int tickCount = Environment.TickCount;
                ThreadPriority priority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                ReLoad();
                Thread.CurrentThread.Priority = priority;
                tickCount = Environment.TickCount - tickCount;
            }
            catch (Exception exception)
            {
                Console.WriteLine("TimeCheck Rank: " + exception);
            }
        }
    }
}

