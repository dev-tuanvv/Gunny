namespace Game.Server.Managers
{
    using Bussiness;
    using Game.Base.Packets;
    using Game.Server;
    using Game.Server.GameObjects;
    using Game.Server.GameUtils;
    using Game.Server.Packets;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Reflection;

    public sealed class WorldMgr
    {
        public static Scene _hotSpring;
        public static Scene _hotSpringScene;
        public static Scene _marryScene;
        private static Dictionary<int, EdictumInfo> dictionary_1 = new Dictionary<int, EdictumInfo>();
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_clientLocker = new ReaderWriterLock();
        private static Dictionary<int, Dictionary<int, SqlDataProvider.Data.ItemInfo>> m_playerMailBags = new Dictionary<int, Dictionary<int, SqlDataProvider.Data.ItemInfo>>();
        private static Dictionary<int, GamePlayer> m_players = new Dictionary<int, GamePlayer>();
        private static RSACryptoServiceProvider m_rsa;

        public static void AddItemToMailBag(int playerId, List<SqlDataProvider.Data.ItemInfo> infos)
        {
            m_clientLocker.AcquireWriterLock(-1);
            try
            {
                if (m_playerMailBags.ContainsKey(playerId))
                {
                    Dictionary<int, SqlDataProvider.Data.ItemInfo> dictionary = m_playerMailBags[playerId];
                    foreach (SqlDataProvider.Data.ItemInfo info in infos)
                    {
                        if (!dictionary.ContainsKey(info.TemplateID))
                        {
                            dictionary.Add(info.TemplateID, info);
                        }
                        else
                        {
                            SqlDataProvider.Data.ItemInfo local1 = dictionary[info.TemplateID];
                            local1.Count += info.Count;
                        }
                    }
                }
                else
                {
                    Dictionary<int, SqlDataProvider.Data.ItemInfo> dictionary2 = new Dictionary<int, SqlDataProvider.Data.ItemInfo>();
                    foreach (SqlDataProvider.Data.ItemInfo info2 in infos)
                    {
                        if (!dictionary2.ContainsKey(info2.TemplateID))
                        {
                            dictionary2.Add(info2.TemplateID, info2);
                        }
                        else
                        {
                            SqlDataProvider.Data.ItemInfo local2 = dictionary2[info2.TemplateID];
                            local2.Count += info2.Count;
                        }
                    }
                    m_playerMailBags.Add(playerId, dictionary2);
                }
            }
            finally
            {
                m_clientLocker.ReleaseWriterLock();
            }
        }

        public static bool AddPlayer(int playerId, GamePlayer player)
        {
            m_clientLocker.AcquireWriterLock(-1);
            try
            {
                if (m_players.ContainsKey(playerId))
                {
                    return false;
                }
                m_players.Add(playerId, player);
            }
            finally
            {
                m_clientLocker.ReleaseWriterLock();
            }
            return true;
        }

        public static void ChangePlayerState(int playerID, int state, int consortiaID)
        {
            GSPacketIn pkg = null;
            foreach (GamePlayer player in GetAllPlayers())
            {
                if ((((player.Friends != null) && player.Friends.ContainsKey(playerID)) && (player.Friends[playerID] == 0)) || ((player.PlayerCharacter.ConsortiaID != 0) && (player.PlayerCharacter.ConsortiaID == consortiaID)))
                {
                    if (pkg == null)
                    {
                        pkg = player.Out.SendFriendState(playerID, state, player.PlayerCharacter.typeVIP, player.PlayerCharacter.VIPLevel);
                    }
                    else
                    {
                        player.SendTCP(pkg);
                    }
                }
            }
        }

        public static string DisconnectPlayerByName(string nickName)
        {
            foreach (GamePlayer player in GetAllPlayers())
            {
                if (player.PlayerCharacter.NickName == nickName)
                {
                    player.Disconnect();
                    return "OK";
                }
            }
            return (nickName + " is not online!");
        }

        public static Dictionary<int, Dictionary<int, SqlDataProvider.Data.ItemInfo>> GetAllBagMails()
        {
            Dictionary<int, Dictionary<int, SqlDataProvider.Data.ItemInfo>> dictionary = new Dictionary<int, Dictionary<int, SqlDataProvider.Data.ItemInfo>>();
            m_clientLocker.AcquireReaderLock(0x2710);
            try
            {
                foreach (KeyValuePair<int, Dictionary<int, SqlDataProvider.Data.ItemInfo>> pair in m_playerMailBags)
                {
                    dictionary.Add(pair.Key, pair.Value);
                }
            }
            finally
            {
                m_clientLocker.ReleaseReaderLock();
            }
            return dictionary;
        }

        public static EdictumInfo[] GetAllEdictumVersion()
        {
            List<EdictumInfo> list = new List<EdictumInfo>();
            foreach (EdictumInfo info in dictionary_1.Values)
            {
                if (info.EndDate.Date > DateTime.Now.Date)
                {
                    list.Add(info);
                }
            }
            return list.ToArray();
        }

        public static GamePlayer[] GetAllPlayers()
        {
            List<GamePlayer> list = new List<GamePlayer>();
            m_clientLocker.AcquireReaderLock(0x2710);
            try
            {
                foreach (GamePlayer player in m_players.Values)
                {
                    if ((player != null) && (player.PlayerCharacter != null))
                    {
                        list.Add(player);
                    }
                }
            }
            finally
            {
                m_clientLocker.ReleaseReaderLock();
            }
            return list.ToArray();
        }

        public static GamePlayer[] GetAllPlayersNoGame()
        {
            List<GamePlayer> list = new List<GamePlayer>();
            m_clientLocker.AcquireReaderLock(0x2710);
            try
            {
                foreach (GamePlayer player in GetAllPlayers())
                {
                    if (player.CurrentRoom == null)
                    {
                        list.Add(player);
                    }
                }
            }
            finally
            {
                m_clientLocker.ReleaseReaderLock();
            }
            return list.ToArray();
        }

        public static GamePlayer GetClientByPlayerNickName(string nickName)
        {
            foreach (GamePlayer player in GetAllPlayers())
            {
                if (player.PlayerCharacter.NickName == nickName)
                {
                    return player;
                }
            }
            return null;
        }

        public static GamePlayer GetPlayerById(int playerId)
        {
            GamePlayer player = null;
            m_clientLocker.AcquireReaderLock(0x2710);
            try
            {
                if (m_players.ContainsKey(playerId))
                {
                    player = m_players[playerId];
                }
            }
            finally
            {
                m_clientLocker.ReleaseReaderLock();
            }
            return player;
        }

        public static string GetPlayerStringByPlayerNickName(string nickName)
        {
            foreach (GamePlayer player in GetAllPlayers())
            {
                if (player.PlayerCharacter.NickName == nickName)
                {
                    return player.ToString();
                }
            }
            return (nickName + " is not online!");
        }

        public static bool Init()
        {
            bool flag = false;
            try
            {
                m_rsa = new RSACryptoServiceProvider();
                m_rsa.FromXmlString(GameServer.Instance.Configuration.PrivateKey);
                m_players.Clear();
                using (ServiceBussiness bussiness = new ServiceBussiness())
                {
                    ServerInfo serviceSingle = bussiness.GetServiceSingle(GameServer.Instance.Configuration.ServerID);
                    if (serviceSingle != null)
                    {
                        _marryScene = new Scene(serviceSingle);
                        _hotSpringScene = new Scene(serviceSingle);
                        flag = true;
                    }
                }
                Dictionary<int, EdictumInfo> dictionary = smethod_0();
                if (dictionary.Values.Count > 0)
                {
                    Interlocked.Exchange<Dictionary<int, EdictumInfo>>(ref dictionary_1, dictionary);
                }
            }
            catch (Exception exception)
            {
                log.Error("WordMgr Init", exception);
            }
            return flag;
        }

        public static void OnPlayerOffline(int playerid, int consortiaID)
        {
            ChangePlayerState(playerid, 0, consortiaID);
        }

        public static void OnPlayerOnline(int playerid, int consortiaID)
        {
            ChangePlayerState(playerid, 1, consortiaID);
        }

        public static bool ReloadEdictum()
        {
            bool flag = false;
            try
            {
                Dictionary<int, EdictumInfo> dictionary = smethod_0();
                if (dictionary.Values.Count > 0)
                {
                    Interlocked.Exchange<Dictionary<int, EdictumInfo>>(ref dictionary_1, dictionary);
                }
                flag = true;
            }
            catch (Exception exception)
            {
                log.Error("WordMgr ReloadEdictum Init", exception);
            }
            return flag;
        }

        public static bool RemovePlayer(int playerId)
        {
            m_clientLocker.AcquireWriterLock(-1);
            GamePlayer player = null;
            try
            {
                if (m_players.ContainsKey(playerId))
                {
                    player = m_players[playerId];
                    m_players.Remove(playerId);
                }
            }
            finally
            {
                m_clientLocker.ReleaseWriterLock();
            }
            if (player == null)
            {
                return false;
            }
            GameServer.Instance.LoginServer.SendUserOffline(playerId, player.PlayerCharacter.ConsortiaID);
            return true;
        }

        public static void ScanBagMail()
        {
            Dictionary<int, Dictionary<int, SqlDataProvider.Data.ItemInfo>> allBagMails = GetAllBagMails();
            foreach (KeyValuePair<int, Dictionary<int, SqlDataProvider.Data.ItemInfo>> pair in allBagMails)
            {
                if (pair.Value != null)
                {
                    SendItemsToMail(pair.Value, pair.Key);
                }
            }
        }

        public static void SendItemsToMail(Dictionary<int, SqlDataProvider.Data.ItemInfo> infos, int PlayerId)
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
                foreach (SqlDataProvider.Data.ItemInfo info in infos.Values)
                {
                    if (info.Template.MaxCount == 1)
                    {
                        for (int j = 0; j < info.Count; j++)
                        {
                            SqlDataProvider.Data.ItemInfo item = SqlDataProvider.Data.ItemInfo.CloneFromTemplate(info.Template, info);
                            item.Count = 1;
                            list.Add(item);
                        }
                    }
                    else
                    {
                        list.Add(info);
                    }
                }
                for (int i = 0; i < list.Count; i += 5)
                {
                    SqlDataProvider.Data.ItemInfo info4;
                    MailInfo mail = new MailInfo {
                        Title = "Vật phẩm chuyển về từ -T\x00fai ẩn-",
                        Gold = 0,
                        IsExist = true,
                        Money = 0,
                        Receiver = "T\x00fai ẩn",
                        ReceiverID = PlayerId,
                        Sender = "Hệ thống",
                        SenderID = 0,
                        Type = 9,
                        GiftToken = 0
                    };
                    StringBuilder builder = new StringBuilder();
                    StringBuilder builder2 = new StringBuilder();
                    builder.Append(LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.AnnexRemark", new object[0]));
                    int num3 = i;
                    if (list.Count > num3)
                    {
                        info4 = list[num3];
                        if (info4.ItemID == 0)
                        {
                            bussiness.AddGoods(info4);
                        }
                        mail.Annex1 = info4.ItemID.ToString();
                        mail.Annex1Name = info4.Template.Name;
                        builder.Append(string.Concat(new object[] { "1、", mail.Annex1Name, "x", info4.Count, ";" }));
                        builder2.Append(string.Concat(new object[] { "1、", mail.Annex1Name, "x", info4.Count, ";" }));
                    }
                    num3 = i + 1;
                    if (list.Count > num3)
                    {
                        info4 = list[num3];
                        if (info4.ItemID == 0)
                        {
                            bussiness.AddGoods(info4);
                        }
                        mail.Annex2 = info4.ItemID.ToString();
                        mail.Annex2Name = info4.Template.Name;
                        builder.Append(string.Concat(new object[] { "2、", mail.Annex2Name, "x", info4.Count, ";" }));
                        builder2.Append(string.Concat(new object[] { "2、", mail.Annex2Name, "x", info4.Count, ";" }));
                    }
                    num3 = i + 2;
                    if (list.Count > num3)
                    {
                        info4 = list[num3];
                        if (info4.ItemID == 0)
                        {
                            bussiness.AddGoods(info4);
                        }
                        mail.Annex3 = info4.ItemID.ToString();
                        mail.Annex3Name = info4.Template.Name;
                        builder.Append(string.Concat(new object[] { "3、", mail.Annex3Name, "x", info4.Count, ";" }));
                        builder2.Append(string.Concat(new object[] { "3、", mail.Annex3Name, "x", info4.Count, ";" }));
                    }
                    num3 = i + 3;
                    if (list.Count > num3)
                    {
                        info4 = list[num3];
                        if (info4.ItemID == 0)
                        {
                            bussiness.AddGoods(info4);
                        }
                        mail.Annex4 = info4.ItemID.ToString();
                        mail.Annex4Name = info4.Template.Name;
                        builder.Append(string.Concat(new object[] { "4、", mail.Annex4Name, "x", info4.Count, ";" }));
                        builder2.Append(string.Concat(new object[] { "4、", mail.Annex4Name, "x", info4.Count, ";" }));
                    }
                    num3 = i + 4;
                    if (list.Count > num3)
                    {
                        info4 = list[num3];
                        if (info4.ItemID == 0)
                        {
                            bussiness.AddGoods(info4);
                        }
                        mail.Annex5 = info4.ItemID.ToString();
                        mail.Annex5Name = info4.Template.Name;
                        builder.Append(string.Concat(new object[] { "5、", mail.Annex5Name, "x", info4.Count, ";" }));
                        builder2.Append(string.Concat(new object[] { "5、", mail.Annex5Name, "x", info4.Count, ";" }));
                    }
                    mail.AnnexRemark = builder.ToString();
                    mail.Content = builder2.ToString();
                    if (bussiness.SendMail(mail))
                    {
                        m_playerMailBags.Remove(PlayerId);
                    }
                }
            }
        }

        public static GSPacketIn SendSysNotice(string msg)
        {
            GSPacketIn pkg = new GSPacketIn(10);
            pkg.WriteInt(2);
            pkg.WriteString(msg);
            SendToAll(pkg);
            return pkg;
        }

        public static GSPacketIn SendSysNotice(eMessageType type, string msg, int ItemID, int TemplateID, string key, int zoneID)
        {
            int index = msg.IndexOf("@");
            GSPacketIn pkg = new GSPacketIn(10);
            pkg.WriteInt((int) type);
            pkg.WriteString(msg.Replace("@", ""));
            if (type == eMessageType.CROSS_NOTICE)
            {
                pkg.WriteInt(zoneID);
            }
            if (ItemID > 0)
            {
                pkg.WriteByte(1);
                pkg.WriteInt(index);
                pkg.WriteInt(TemplateID);
                pkg.WriteInt(ItemID);
                pkg.WriteString(key);
            }
            SendToAll(pkg);
            return pkg;
        }

        public static void SendToAll(GSPacketIn pkg)
        {
            foreach (GamePlayer player in GetAllPlayers())
            {
                player.SendTCP(pkg);
            }
        }

        private static Dictionary<int, EdictumInfo> smethod_0()
        {
            Dictionary<int, EdictumInfo> dictionary = new Dictionary<int, EdictumInfo>();
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                foreach (EdictumInfo info in bussiness.GetAllEdictum())
                {
                    if (!dictionary.ContainsKey(info.ID))
                    {
                        dictionary.Add(info.ID, info);
                    }
                }
            }
            return dictionary;
        }

        public static Scene HotSpring
        {
            get
            {
                return _hotSpring;
            }
        }

        public static Scene HotSpringScene
        {
            get
            {
                return _hotSpringScene;
            }
        }

        public static Scene MarryScene
        {
            get
            {
                return _marryScene;
            }
        }

        public static RSACryptoServiceProvider RsaCryptor
        {
            get
            {
                return m_rsa;
            }
        }
    }
}

