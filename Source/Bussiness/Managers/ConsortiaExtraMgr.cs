using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.Managers
{
	public class ConsortiaExtraMgr
	{
		private static readonly ILog log;
		private static Dictionary<int, ConsortiaLevelInfo> _consortiaLevel;
		private static Dictionary<int, ConsortiaBuffTempInfo> _consortiaBuffTemp;
		private static ReaderWriterLock m_lock;
		private static ThreadSafeRandom rand;
		static ConsortiaExtraMgr()
		{
			ConsortiaExtraMgr.log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}
		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, ConsortiaLevelInfo> consortiaLevel = new Dictionary<int, ConsortiaLevelInfo>();
				Dictionary<int, ConsortiaBuffTempInfo> consortiaBuffTemp = new Dictionary<int, ConsortiaBuffTempInfo>();
				if (ConsortiaExtraMgr.Load(consortiaLevel, consortiaBuffTemp))
				{
					ConsortiaExtraMgr.m_lock.AcquireWriterLock(-1);
					try
					{
						ConsortiaExtraMgr._consortiaLevel = consortiaLevel;
						ConsortiaExtraMgr._consortiaBuffTemp = consortiaBuffTemp;
						return true;
					}
					catch
					{
					}
					finally
					{
						ConsortiaExtraMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception ex)
			{
				if (ConsortiaExtraMgr.log.IsErrorEnabled)
				{
					ConsortiaExtraMgr.log.Error("ConsortiaLevelMgr", ex);
				}
			}
			return false;
		}
		public static bool Init()
		{
			bool result;
			try
			{
				ConsortiaExtraMgr.m_lock = new ReaderWriterLock();
				ConsortiaExtraMgr._consortiaLevel = new Dictionary<int, ConsortiaLevelInfo>();
				ConsortiaExtraMgr._consortiaBuffTemp = new Dictionary<int, ConsortiaBuffTempInfo>();
				ConsortiaExtraMgr.rand = new ThreadSafeRandom();
				result = ConsortiaExtraMgr.Load(ConsortiaExtraMgr._consortiaLevel, ConsortiaExtraMgr._consortiaBuffTemp);
			}
			catch (Exception ex)
			{
				if (ConsortiaExtraMgr.log.IsErrorEnabled)
				{
					ConsortiaExtraMgr.log.Error("ConsortiaLevelMgr", ex);
				}
				result = false;
			}
			return result;
		}
		private static bool Load(Dictionary<int, ConsortiaLevelInfo> consortiaLevel, Dictionary<int, ConsortiaBuffTempInfo> consortiaBuffTemp)
		{
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				ConsortiaLevelInfo[] allConsortiaLevel = consortiaBussiness.GetAllConsortiaLevel();
				for (int i = 0; i < allConsortiaLevel.Length; i++)
				{
					ConsortiaLevelInfo consortiaLevelInfo = allConsortiaLevel[i];
					if (!consortiaLevel.ContainsKey(consortiaLevelInfo.Level))
					{
						consortiaLevel.Add(consortiaLevelInfo.Level, consortiaLevelInfo);
					}
				}
				ConsortiaBuffTempInfo[] allConsortiaBuffTemp = consortiaBussiness.GetAllConsortiaBuffTemp();
				for (int j = 0; j < allConsortiaBuffTemp.Length; j++)
				{
					ConsortiaBuffTempInfo consortiaBuffTempInfo = allConsortiaBuffTemp[j];
					if (!consortiaBuffTemp.ContainsKey(consortiaBuffTempInfo.id))
					{
						consortiaBuffTemp.Add(consortiaBuffTempInfo.id, consortiaBuffTempInfo);
					}
				}
			}
			return true;
		}
		public static ConsortiaLevelInfo FindConsortiaLevelInfo(int level)
		{
			ConsortiaExtraMgr.m_lock.AcquireReaderLock(-1);
			try
			{
				if (ConsortiaExtraMgr._consortiaLevel.ContainsKey(level))
				{
					return ConsortiaExtraMgr._consortiaLevel[level];
				}
			}
			catch
			{
			}
			finally
			{
				ConsortiaExtraMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static ConsortiaBuffTempInfo FindConsortiaBuffInfo(int id)
		{
			ConsortiaExtraMgr.m_lock.AcquireReaderLock(-1);
			try
			{
				if (ConsortiaExtraMgr._consortiaBuffTemp.ContainsKey(id))
				{
					return ConsortiaExtraMgr._consortiaBuffTemp[id];
				}
			}
			catch
			{
			}
			finally
			{
				ConsortiaExtraMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static List<ConsortiaBuffTempInfo> GetAllConsortiaBuff()
		{
			ConsortiaExtraMgr.m_lock.AcquireReaderLock(-1);
			List<ConsortiaBuffTempInfo> list = new List<ConsortiaBuffTempInfo>();
			try
			{
				foreach (ConsortiaBuffTempInfo consortiaBuffTempInfo in ConsortiaExtraMgr._consortiaBuffTemp.Values)
				{
					list.Add(consortiaBuffTempInfo);
				}
			}
			catch
			{
			}
			finally
			{
				ConsortiaExtraMgr.m_lock.ReleaseReaderLock();
			}
			return list;
		}
	}
}
