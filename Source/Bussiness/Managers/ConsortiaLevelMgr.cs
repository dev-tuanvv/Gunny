using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.Managers
{
	public class ConsortiaLevelMgr
	{
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, ConsortiaLevelInfo> _consortiaLevel;
		private static ReaderWriterLock m_lock;
		private static ThreadSafeRandom rand;
		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, ConsortiaLevelInfo> tempConsortiaLevel = new Dictionary<int, ConsortiaLevelInfo>();
				if (ConsortiaLevelMgr.Load(tempConsortiaLevel))
				{
					ConsortiaLevelMgr.m_lock.AcquireWriterLock(-1);
					try
					{
						ConsortiaLevelMgr._consortiaLevel = tempConsortiaLevel;
						return true;
					}
					catch
					{
					}
					finally
					{
						ConsortiaLevelMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception e)
			{
				if (ConsortiaLevelMgr.log.IsErrorEnabled)
				{
					ConsortiaLevelMgr.log.Error("ConsortiaLevelMgr", e);
				}
			}
			return false;
		}
		public static bool Init()
		{
			bool result;
			try
			{
				ConsortiaLevelMgr.m_lock = new ReaderWriterLock();
				ConsortiaLevelMgr._consortiaLevel = new Dictionary<int, ConsortiaLevelInfo>();
				ConsortiaLevelMgr.rand = new ThreadSafeRandom();
				result = ConsortiaLevelMgr.Load(ConsortiaLevelMgr._consortiaLevel);
			}
			catch (Exception e)
			{
				if (ConsortiaLevelMgr.log.IsErrorEnabled)
				{
					ConsortiaLevelMgr.log.Error("ConsortiaLevelMgr", e);
				}
				result = false;
			}
			return result;
		}
		private static bool Load(Dictionary<int, ConsortiaLevelInfo> consortiaLevel)
		{
			using (ConsortiaBussiness db = new ConsortiaBussiness())
			{
				ConsortiaLevelInfo[] infos = db.GetAllConsortiaLevel();
				ConsortiaLevelInfo[] array = infos;
				for (int i = 0; i < array.Length; i++)
				{
					ConsortiaLevelInfo info = array[i];
					if (!consortiaLevel.ContainsKey(info.Level))
					{
						consortiaLevel.Add(info.Level, info);
					}
				}
			}
			return true;
		}
		public static ConsortiaLevelInfo FindConsortiaLevelInfo(int level)
		{
			ConsortiaLevelMgr.m_lock.AcquireReaderLock(-1);
			try
			{
				if (ConsortiaLevelMgr._consortiaLevel.ContainsKey(level))
				{
					return ConsortiaLevelMgr._consortiaLevel[level];
				}
			}
			catch
			{
			}
			finally
			{
				ConsortiaLevelMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
	}
}
