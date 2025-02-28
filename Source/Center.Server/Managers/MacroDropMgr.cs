﻿namespace Center.Server.Managers
{
    using Bussiness;
    using Center.Server;
    using Game.Base.Packets;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Timers;
    using System.Threading;
    using System.Reflection;

    public class MacroDropMgr
    {
        private static int counter;
        private static string FilePath;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<int, DropInfo> m_DropInfo;
        private static ReaderWriterLock m_lock;

        public static void DropNotice(Dictionary<int, int> temp)
        {
            m_lock.AcquireWriterLock(-1);
            try
            {
                foreach (KeyValuePair<int, int> pair in temp)
                {
                    if (m_DropInfo.ContainsKey(pair.Key))
                    {
                        DropInfo info = m_DropInfo[pair.Key];
                        if (info.Count > 0)
                        {
                            info.Count -= pair.Value;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("DropInfoMgr CanDrop", exception);
                }
            }
            finally
            {
                m_lock.ReleaseWriterLock();
            }
        }

        public static bool Init()
        {
            m_lock = new ReaderWriterLock();
            FilePath = Directory.GetCurrentDirectory() + @"\macrodrop\macroDrop.ini";
            return Reload();
        }

        private static Dictionary<int, DropInfo> LoadDropInfo()
        {
            Dictionary<int, DropInfo> dictionary = new Dictionary<int, DropInfo>();
            if (File.Exists(FilePath))
            {
                IniReader reader = new IniReader(FilePath);
                for (int i = 1; reader.GetIniString(i.ToString(), "TemplateId") != ""; i++)
                {
                    string section = i.ToString();
                    int id = Convert.ToInt32(reader.GetIniString(section, "TemplateId"));
                    int time = Convert.ToInt32(reader.GetIniString(section, "Time"));
                    int count = Convert.ToInt32(reader.GetIniString(section, "Count"));
                    DropInfo info = new DropInfo(id, time, count, count);
                    dictionary.Add(info.ID, info);
                }
                return dictionary;
            }
            return null;
        }

        private static void MacroDropReset()
        {
            m_lock.AcquireWriterLock(-1);
            try
            {
                foreach (KeyValuePair<int, DropInfo> pair in m_DropInfo)
                {
                    int key = pair.Key;
                    DropInfo info = pair.Value;
                    if (((counter > info.Time) && (info.Time > 0)) && ((counter % info.Time) == 0))
                    {
                        info.Count = info.MaxCount;
                    }
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("DropInfoMgr MacroDropReset", exception);
                }
            }
            finally
            {
                m_lock.ReleaseWriterLock();
            }
        }

        private static void MacroDropSync()
        {
            bool flag = true;
            ServerClient[] allClients = CenterServer.Instance.GetAllClients();
            foreach (ServerClient client in allClients)
            {
                if (!client.NeedSyncMacroDrop)
                {
                    flag = false;
                    break;
                }
            }
            if ((allClients.Length > 0) && flag)
            {
                GSPacketIn pkg = new GSPacketIn(0xb2);
                int count = m_DropInfo.Count;
                pkg.WriteInt(count);
                m_lock.AcquireReaderLock(-1);
                try
                {
                    foreach (KeyValuePair<int, DropInfo> pair in m_DropInfo)
                    {
                        DropInfo info = pair.Value;
                        pkg.WriteInt(info.ID);
                        pkg.WriteInt(info.Count);
                        pkg.WriteInt(info.MaxCount);
                    }
                }
                catch (Exception exception)
                {
                    if (log.IsErrorEnabled)
                    {
                        log.Error("DropInfoMgr MacroDropReset", exception);
                    }
                }
                finally
                {
                    m_lock.ReleaseReaderLock();
                }
                foreach (ServerClient client2 in allClients)
                {
                    client2.NeedSyncMacroDrop = false;
                    client2.SendTCP(pkg);
                }
            }
        }

        private static void OnTimeEvent(object source, ElapsedEventArgs e)
        {
            counter++;
            if ((counter % 12) == 0)
            {
                MacroDropReset();
            }
            MacroDropSync();
        }

        public static bool Reload()
        {
            try
            {
                Dictionary<int, DropInfo> dictionary = new Dictionary<int, DropInfo>();
                m_DropInfo = new Dictionary<int, DropInfo>();
                dictionary = LoadDropInfo();
                if ((dictionary != null) && (dictionary.Count > 0))
                {
                    Interlocked.Exchange<Dictionary<int, DropInfo>>(ref m_DropInfo, dictionary);
                }
                return true;
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("DropInfoMgr", exception);
                }
            }
            return false;
        }

        public static void Start()
        {
            counter = 0;
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(MacroDropMgr.OnTimeEvent);
            timer.Interval = 300000.0;
            timer.Enabled = true;
        }
    }
}

