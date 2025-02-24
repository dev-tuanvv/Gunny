namespace Game.Server.Managers
{
    using Bussiness;
    using Bussiness.Managers;
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Server;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Reflection;

    public class ConsortiaBossMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_clientLocker = new ReaderWriterLock();
        private static Dictionary<int, ConsortiaInfo> m_consortias = new Dictionary<int, ConsortiaInfo>();

        public static bool AddConsortia(ConsortiaInfo consortia)
        {
            GSPacketIn packet = new GSPacketIn(180);
            packet.WriteInt(consortia.ConsortiaID);
            packet.WriteInt(consortia.ChairmanID);
            packet.WriteByte((byte) consortia.bossState);
            packet.WriteDateTime(consortia.endTime);
            packet.WriteInt(consortia.extendAvailableNum);
            packet.WriteInt(consortia.callBossLevel);
            packet.WriteInt(consortia.Level);
            packet.WriteInt(consortia.SmithLevel);
            packet.WriteInt(consortia.StoreLevel);
            packet.WriteInt(consortia.SkillLevel);
            packet.WriteInt(consortia.Riches);
            packet.WriteDateTime(consortia.LastOpenBoss);
            GameServer.Instance.LoginServer.SendPacket(packet);
            return true;
        }

        public static bool AddConsortia(int consortiaId, ConsortiaInfo consortia)
        {
            m_clientLocker.AcquireWriterLock(-1);
            try
            {
                if (m_consortias.ContainsKey(consortiaId))
                {
                    return false;
                }
                m_consortias.Add(consortiaId, consortia);
            }
            finally
            {
                m_clientLocker.ReleaseWriterLock();
            }
            return true;
        }

        public static void CreateBoss(ConsortiaInfo consortia, int npcId)
        {
            GSPacketIn packet = new GSPacketIn(0xb7);
            packet.WriteInt(consortia.ConsortiaID);
            packet.WriteByte((byte) consortia.bossState);
            packet.WriteDateTime(consortia.endTime);
            packet.WriteDateTime(consortia.LastOpenBoss);
            int val = 0xe4e1c0;
            NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(npcId);
            if (npcInfoById != null)
            {
                val = npcInfoById.Blood;
            }
            packet.WriteInt(val);
            GameServer.Instance.LoginServer.SendPacket(packet);
        }

        public static void ExtendAvailable(int consortiaId, int riches)
        {
            GSPacketIn packet = new GSPacketIn(0xb6);
            packet.WriteInt(consortiaId);
            packet.WriteInt(riches);
            GameServer.Instance.LoginServer.SendPacket(packet);
        }

        public static long GetConsortiaBossTotalDame(int consortiaId)
        {
            if (m_consortias.ContainsKey(consortiaId))
            {
                long totalAllMemberDame = m_consortias[consortiaId].TotalAllMemberDame;
                long maxBlood = m_consortias[consortiaId].MaxBlood;
                if (totalAllMemberDame > maxBlood)
                {
                    totalAllMemberDame = maxBlood - 0x3e8L;
                }
                return totalAllMemberDame;
            }
            return 0L;
        }

        public static ConsortiaInfo GetConsortiaById(int consortiaId)
        {
            ConsortiaInfo info = null;
            m_clientLocker.AcquireReaderLock(0x2710);
            try
            {
                if (m_consortias.ContainsKey(consortiaId))
                {
                    info = m_consortias[consortiaId];
                }
            }
            finally
            {
                m_clientLocker.ReleaseReaderLock();
            }
            return info;
        }

        public static bool GetConsortiaExit(int consortiaId)
        {
            bool flag;
            m_clientLocker.AcquireReaderLock(0x2710);
            try
            {
                flag = m_consortias.ContainsKey(consortiaId);
            }
            finally
            {
                m_clientLocker.ReleaseReaderLock();
            }
            return flag;
        }

        public static void reload(int consortiaId)
        {
            GSPacketIn packet = new GSPacketIn(0xb8);
            packet.WriteInt(consortiaId);
            GameServer.Instance.LoginServer.SendPacket(packet);
        }

        public static void SendConsortiaAward(int consortiaId)
        {
            m_clientLocker.AcquireWriterLock(-1);
            try
            {
                if (m_consortias.ContainsKey(consortiaId))
                {
                    ConsortiaInfo info = m_consortias[consortiaId];
                    int copyId = 0xc350 + info.callBossLevel;
                    List<SqlDataProvider.Data.ItemInfo> list = null;
                    DropInventory.CopyAllDrop(copyId, ref list);
                    int riches = 0;
                    if (info.RankList != null)
                    {
                        foreach (RankingPersonInfo info2 in info.RankList.Values)
                        {
                            if (info.IsBossDie)
                            {
                                string title = "Phần thưởng tham gia Boss Guild";
                                if (list != null)
                                {
                                    WorldEventMgr.SendItemsToMail(list, info2.UserID, info2.Name, title);
                                }
                                else
                                {
                                    Console.WriteLine("Boss Guild award error dropID {0} ", copyId);
                                }
                            }
                            riches += info2.Damage;
                        }
                        using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
                        {
                            bussiness.ConsortiaRichAdd(consortiaId, ref riches, 5, "Boss Guild");
                        }
                    }
                }
            }
            finally
            {
                m_clientLocker.ReleaseWriterLock();
            }
        }

        public static void UpdateBlood(int consortiaId, int damage)
        {
            GSPacketIn packet = new GSPacketIn(0xba);
            packet.WriteInt(consortiaId);
            packet.WriteInt(damage);
            GameServer.Instance.LoginServer.SendPacket(packet);
        }

        public static bool UpdateConsortia(ConsortiaInfo info)
        {
            m_clientLocker.AcquireWriterLock(-1);
            try
            {
                int consortiaID = info.ConsortiaID;
                if (m_consortias.ContainsKey(consortiaID))
                {
                    m_consortias[consortiaID] = info;
                }
            }
            finally
            {
                m_clientLocker.ReleaseWriterLock();
            }
            return true;
        }

        public static void UpdateRank(int consortiaId, int damage, int richer, int honor, string Nickname, int userID)
        {
            GSPacketIn packet = new GSPacketIn(0xb5);
            packet.WriteInt(consortiaId);
            packet.WriteInt(damage);
            packet.WriteInt(richer);
            packet.WriteInt(honor);
            packet.WriteString(Nickname);
            packet.WriteInt(userID);
            GameServer.Instance.LoginServer.SendPacket(packet);
        }
    }
}

