namespace Bussiness.Managers
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Reflection;

    public class TotemMgr
    {
        private static Dictionary<int, TotemInfo> _totem;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;
        private static ThreadSafeRandom rand;

        public static TotemInfo FindTotemInfo(int ID)
        {
            if (ID < 0x2710)
            {
                ID = 0x2711;
            }
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_totem.ContainsKey(ID))
                {
                    return _totem[ID];
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

        public static int GetTotemProp(int id, string typeOf)
        {
            int num = 0;
            for (int i = 0x2711; i <= id; i++)
            {
                TotemInfo info = FindTotemInfo(i);
                switch (typeOf)
                {
                    case "att":
                        num += info.AddAttack;
                        break;

                    case "agi":
                        num += info.AddAgility;
                        break;

                    case "def":
                        num += info.AddDefence;
                        break;

                    case "luc":
                        num += info.AddLuck;
                        break;

                    case "blo":
                        num += info.AddBlood;
                        break;

                    case "dam":
                        num += info.AddDamage;
                        break;

                    case "gua":
                        num += info.AddGuard;
                        break;
                }
            }
            return num;
        }

        public static bool Init()
        {
            try
            {
                m_lock = new ReaderWriterLock();
                _totem = new Dictionary<int, TotemInfo>();
                rand = new ThreadSafeRandom();
                return Load(_totem);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("TotemMgr", exception);
                }
                return false;
            }
        }

        private static bool Load(Dictionary<int, TotemInfo> totem)
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                foreach (TotemInfo info in bussiness.GetAllTotem())
                {
                    if (!totem.ContainsKey(info.ID))
                    {
                        totem.Add(info.ID, info);
                    }
                }
            }
            return true;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, TotemInfo> totem = new Dictionary<int, TotemInfo>();
                if (Load(totem))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        _totem = totem;
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
                    log.Error("TotemMgr", exception);
                }
            }
            return false;
        }
    }
}

