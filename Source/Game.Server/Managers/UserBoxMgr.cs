namespace Game.Server.Managers
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Reflection;

    public class UserBoxMgr
    {
        private static Dictionary<int, UserBoxInfo> dictionary_0;
        private static readonly ILog ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock readerWriterLock_0;

        public static List<UserBoxInfo> GetGradeBoxAward()
        {
            List<UserBoxInfo> list = new List<UserBoxInfo>();
            readerWriterLock_0.AcquireReaderLock(-1);
            try
            {
                foreach (UserBoxInfo info in dictionary_0.Values)
                {
                    if (info.Type == 1)
                    {
                        list.Add(info);
                    }
                }
                return list;
            }
            catch
            {
            }
            finally
            {
                readerWriterLock_0.ReleaseReaderLock();
            }
            return list;
        }

        public static List<UserBoxInfo> GetTimeBoxAward()
        {
            List<UserBoxInfo> list = new List<UserBoxInfo>();
            readerWriterLock_0.AcquireReaderLock(-1);
            try
            {
                foreach (UserBoxInfo info in dictionary_0.Values)
                {
                    if (info.Type == 0)
                    {
                        list.Add(info);
                    }
                }
                return list;
            }
            catch
            {
            }
            finally
            {
                readerWriterLock_0.ReleaseReaderLock();
            }
            return list;
        }

        public static UserBoxInfo GetTimeBoxWithCondition(int condition)
        {
            foreach (UserBoxInfo info in GetTimeBoxAward())
            {
                if (info.Condition == condition)
                {
                    return info;
                }
            }
            return null;
        }

        public static bool Init()
        {
            try
            {
                readerWriterLock_0 = new ReaderWriterLock();
                dictionary_0 = new Dictionary<int, UserBoxInfo>();
                return smethod_0(dictionary_0);
            }
            catch (Exception exception)
            {
                if (ilog_0.IsErrorEnabled)
                {
                    ilog_0.Error("UserBoxMgr", exception);
                }
                return false;
            }
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, UserBoxInfo> dictionary = new Dictionary<int, UserBoxInfo>();
                if (smethod_0(dictionary))
                {
                    readerWriterLock_0.AcquireWriterLock(-1);
                    try
                    {
                        dictionary_0 = dictionary;
                        return true;
                    }
                    catch
                    {
                    }
                    finally
                    {
                        readerWriterLock_0.ReleaseWriterLock();
                    }
                }
            }
            catch (Exception exception)
            {
                if (ilog_0.IsErrorEnabled)
                {
                    ilog_0.Error("UserBoxMgr", exception);
                }
            }
            return false;
        }

        private static bool smethod_0(Dictionary<int, UserBoxInfo> VrpYTyJcS3Y149eS09k)
        {
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                foreach (UserBoxInfo info in bussiness.GetAllUserBox())
                {
                    if (!VrpYTyJcS3Y149eS09k.ContainsKey(info.ID))
                    {
                        VrpYTyJcS3Y149eS09k.Add(info.ID, info);
                    }
                }
            }
            return true;
        }
    }
}

