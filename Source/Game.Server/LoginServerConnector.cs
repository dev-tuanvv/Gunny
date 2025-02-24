namespace Game.Server
{
    using Bussiness;
    using Bussiness.Managers;
    using Bussiness.Protocol;
    using Game.Base;
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using Game.Server.Rooms;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Reflection;

    public class LoginServerConnector : BaseConnector
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private string m_loginKey;
        private int m_serverId;

        public LoginServerConnector(string ip, int port, int serverid, string name, byte[] readBuffer, byte[] sendBuffer) : base(ip, port, true, readBuffer, sendBuffer)
        {
            this.m_serverId = serverid;
            this.m_loginKey = string.Format("{0},{1}", serverid, name);
            base.Strict = true;
        }

        protected void AsynProcessPacket(object state)
        {
            try
            {
                GSPacketIn packet = state as GSPacketIn;
                int code = packet.Code;
                if (code <= 0x75)
                {
                    if (code <= 0x26)
                    {
                        switch (code)
                        {
                            case 0:
                                this.HandleRSAKey(packet);
                                return;

                            case 1:
                            case 6:
                            case 11:
                            case 12:
                            case 0x10:
                            case 0x11:
                            case 0x12:
                                return;

                            case 2:
                                this.HandleKitoffPlayer(packet);
                                return;

                            case 3:
                                this.HandleAllowUserLogin(packet);
                                return;

                            case 4:
                                this.HandleUserOffline(packet);
                                return;

                            case 5:
                                this.HandleUserOnline(packet);
                                return;

                            case 7:
                                this.HandleASSState(packet);
                                return;

                            case 8:
                                this.HandleConfigState(packet);
                                return;

                            case 9:
                                this.HandleChargeMoney(packet);
                                return;

                            case 10:
                                this.HandleSystemNotice(packet);
                                return;

                            case 13:
                                this.HandleUpdatePlayerMarriedState(packet);
                                return;

                            case 14:
                                this.HandleMarryRoomInfoToPlayer(packet);
                                return;

                            case 15:
                                this.HandleShutdown(packet);
                                return;

                            case 0x13:
                                this.HandleChatConsortia(packet);
                                return;

                            case 0x25:
                                this.HandleChatPersonal(packet);
                                return;

                            case 0x26:
                                this.HandleSysMess(packet);
                                return;
                        }
                    }
                    else
                    {
                        switch (code)
                        {
                            case 0x48:
                                this.HandleBigBugle(packet);
                                return;

                            case 0x49:
                            case 0x4a:
                            case 0x4b:
                            case 0x4c:
                            case 0x4d:
                            case 0x4e:
                            case 0x54:
                            case 0x56:
                                return;

                            case 0x4f:
                                this.HandleWorldBossUpdateBlood(packet);
                                return;

                            case 80:
                                this.HandleWorldBossUpdate(packet);
                                return;

                            case 0x51:
                                this.HandleWorldBossRank(packet);
                                return;

                            case 0x52:
                                this.HandleWorldBossFightOver(packet);
                                return;

                            case 0x53:
                                this.HandleWorldBossRoomClose(packet);
                                return;

                            case 0x55:
                                this.HandleWorldBossPrivateInfo(packet);
                                return;

                            case 0x57:
                                this.HandleLeagueOpenClose(packet);
                                return;

                            case 0x58:
                                this.HandleBattleGoundOpenClose(packet);
                                return;

                            case 0x59:
                                this.HandleFightFootballTime(packet);
                                return;

                            case 90:
                                this.HandleEventRank(packet);
                                return;

                            case 0x5b:
                                this.HandleWorldEvent(packet);
                                return;
                        }
                        if (code == 0x75)
                        {
                            this.HandleMailResponse(packet);
                        }
                    }
                }
                else if (code <= 160)
                {
                    switch (code)
                    {
                        case 0x80:
                            this.HandleConsortiaResponse(packet);
                            return;

                        case 0x81:
                        case 0x9f:
                            return;

                        case 130:
                            this.HandleConsortiaCreate(packet);
                            return;

                        case 0x9e:
                            this.HandleConsortiaFight(packet);
                            return;

                        case 160:
                            this.HandleFriend(packet);
                            return;
                    }
                }
                else
                {
                    switch (code)
                    {
                        case 0xb1:
                            this.HandleRate(packet);
                            return;

                        case 0xb2:
                            this.HandleMacroDrop(packet);
                            return;

                        case 0xb3:
                            return;

                        case 180:
                            this.HandleConsortiaBossInfo(packet);
                            return;
                    }
                    if (code == 0xb9)
                    {
                        this.HandleConsortiaBossSendAward(packet);
                    }
                }
            }
            catch (Exception exception)
            {
                GameServer.log.Error("AsynProcessPacket", exception);
            }
        }

        protected void HandleAllowUserLogin(object stateInfo)
        {
            try
            {
                GSPacketIn @in = (GSPacketIn) stateInfo;
                int playerId = @in.ReadInt();
                if (@in.ReadBoolean())
                {
                    GamePlayer player = LoginMgr.LoginClient(playerId);
                    if (player != null)
                    {
                        if (player.Login())
                        {
                            this.SendUserOnline(playerId, player.PlayerCharacter.ConsortiaID);
                            WorldMgr.OnPlayerOnline(playerId, player.PlayerCharacter.ConsortiaID);
                        }
                        else
                        {
                            player.Client.Disconnect();
                            this.SendUserOffline(playerId, 0);
                        }
                    }
                    else
                    {
                        this.SendUserOffline(playerId, 0);
                    }
                }
            }
            catch (Exception exception)
            {
                GameServer.log.Error("HandleAllowUserLogin", exception);
            }
        }

        public void HandleASSState(GSPacketIn packet)
        {
            bool aSSState = packet.ReadBoolean();
            AntiAddictionMgr.SetASSState(aSSState);
            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
            {
                player.Out.SendAASControl(aSSState, player.IsAASInfo, player.IsMinor);
            }
        }

        public void HandleBattleGoundOpenClose(GSPacketIn pkg)
        {
            ActiveSystemMgr.UpdateIsBattleGoundOpen(pkg.ReadBoolean());
        }

        protected void HandleBigBugle(GSPacketIn packet)
        {
            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
            {
                player.Out.SendTCP(packet);
            }
        }

        public void HandleConfigState(GSPacketIn packet)
        {
            bool aSSState = packet.ReadBoolean();
            AwardMgr.DailyAwardState = packet.ReadBoolean();
            AntiAddictionMgr.SetASSState(aSSState);
            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
            {
                player.Out.SendAASControl(aSSState, player.IsAASInfo, player.IsMinor);
            }
        }

        public void HandleConsortiaAlly(GSPacketIn packet)
        {
            int num = packet.ReadInt();
            int num2 = packet.ReadInt();
            int state = packet.ReadInt();
            ConsortiaMgr.UpdateConsortiaAlly(num, num2, state);
            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
            {
                if ((player.PlayerCharacter.ConsortiaID == num) || (player.PlayerCharacter.ConsortiaID == num2))
                {
                    player.Out.SendTCP(packet);
                }
            }
        }

        public void HandleConsortiaBanChat(GSPacketIn packet)
        {
            bool flag = packet.ReadBoolean();
            int num = packet.ReadInt();
            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
            {
                if (player.PlayerCharacter.ID == num)
                {
                    player.PlayerCharacter.IsBanChat = flag;
                    player.Out.SendTCP(packet);
                    break;
                }
            }
        }

        public void HandleConsortiaBossClose(ConsortiaInfo consortia)
        {
            this.SendToAllConsortiaMember(consortia, 1);
        }

        public void HandleConsortiaBossCreateBoss(ConsortiaInfo consortia)
        {
            this.SendToAllConsortiaMember(consortia, 0);
        }

        public void HandleConsortiaBossDie(ConsortiaInfo consortia)
        {
            this.SendToAllConsortiaMember(consortia, 2);
        }

        public void HandleConsortiaBossExtendAvailable(ConsortiaInfo consortia)
        {
            this.SendToAllConsortiaMember(consortia, 3);
        }

        public void HandleConsortiaBossInfo(GSPacketIn pkg)
        {
            ConsortiaInfo consortia = new ConsortiaInfo {
                ConsortiaID = pkg.ReadInt(),
                ChairmanID = pkg.ReadInt(),
                bossState = pkg.ReadByte(),
                endTime = pkg.ReadDateTime(),
                extendAvailableNum = pkg.ReadInt(),
                callBossLevel = pkg.ReadInt(),
                Level = pkg.ReadInt(),
                SmithLevel = pkg.ReadInt(),
                StoreLevel = pkg.ReadInt(),
                SkillLevel = pkg.ReadInt(),
                Riches = pkg.ReadInt(),
                LastOpenBoss = pkg.ReadDateTime(),
                MaxBlood = pkg.ReadLong(),
                TotalAllMemberDame = pkg.ReadLong(),
                IsBossDie = pkg.ReadBoolean(),
                RankList = new Dictionary<string, RankingPersonInfo>()
            };
            int num = pkg.ReadInt();
            for (int i = 0; i < num; i++)
            {
                RankingPersonInfo info2 = new RankingPersonInfo {
                    Name = pkg.ReadString(),
                    ID = pkg.ReadInt(),
                    TotalDamage = pkg.ReadInt(),
                    Honor = pkg.ReadInt(),
                    Damage = pkg.ReadInt()
                };
                consortia.RankList.Add(info2.Name, info2);
            }
            switch (pkg.ReadByte())
            {
                case 180:
                    this.SendToAllConsortiaMember(consortia, -1);
                    break;

                case 0xb6:
                    this.HandleConsortiaBossExtendAvailable(consortia);
                    break;

                case 0xb7:
                    this.HandleConsortiaBossCreateBoss(consortia);
                    break;

                case 0xb8:
                    this.HandleConsortiaBossReload(consortia);
                    break;

                case 0xbb:
                    this.HandleConsortiaBossClose(consortia);
                    break;

                case 0xbc:
                    this.HandleConsortiaBossDie(consortia);
                    break;
            }
        }

        public void HandleConsortiaBossReload(ConsortiaInfo consortia)
        {
            this.SendToAllConsortiaMember(consortia, -1);
        }

        public void HandleConsortiaBossSendAward(GSPacketIn pkg)
        {
            int num = pkg.ReadInt();
            for (int i = 0; i < num; i++)
            {
                ConsortiaBossMgr.SendConsortiaAward(pkg.ReadInt());
            }
        }

        public void HandleConsortiaCreate(GSPacketIn packet)
        {
            int consortiaID = packet.ReadInt();
            packet.ReadInt();
            ConsortiaMgr.AddConsortia(consortiaID);
        }

        public void HandleConsortiaDelete(GSPacketIn packet)
        {
            int num = packet.ReadInt();
            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
            {
                if (player.PlayerCharacter.ConsortiaID == num)
                {
                    player.ClearConsortia();
                    player.AddRobRiches(-player.PlayerCharacter.RichesRob);
                    player.Out.SendTCP(packet);
                }
            }
        }

        public void HandleConsortiaDuty(GSPacketIn packet)
        {
            int num = packet.ReadByte();
            int num2 = packet.ReadInt();
            int num3 = packet.ReadInt();
            packet.ReadString();
            int num4 = packet.ReadInt();
            string str = packet.ReadString();
            int num5 = packet.ReadInt();
            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
            {
                if (player.PlayerCharacter.ConsortiaID == num2)
                {
                    if ((num == 2) && (player.PlayerCharacter.DutyLevel == num4))
                    {
                        player.PlayerCharacter.DutyName = str;
                    }
                    else if ((player.PlayerCharacter.ID == num3) && ((((num == 5) || (num == 6)) || ((num == 7) || (num == 8))) || (num == 9)))
                    {
                        player.PlayerCharacter.DutyLevel = num4;
                        player.PlayerCharacter.DutyName = str;
                        player.PlayerCharacter.Right = num5;
                    }
                    player.Out.SendTCP(packet);
                }
            }
        }

        public void HandleConsortiaFight(GSPacketIn packet)
        {
            int num = packet.ReadInt();
            packet.ReadInt();
            string message = packet.ReadString();
            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
            {
                if (player.PlayerCharacter.ConsortiaID == num)
                {
                    player.Out.SendMessage(eMessageType.ChatNormal, message);
                }
            }
        }

        protected void HandleConsortiaResponse(GSPacketIn packet)
        {
            switch (packet.ReadByte())
            {
                case 1:
                    this.HandleConsortiaUserPass(packet);
                    break;

                case 2:
                    this.HandleConsortiaDelete(packet);
                    break;

                case 3:
                    this.HandleConsortiaUserDelete(packet);
                    break;

                case 4:
                    this.HandleConsortiaUserInvite(packet);
                    break;

                case 5:
                    this.HandleConsortiaBanChat(packet);
                    break;

                case 6:
                    this.HandleConsortiaUpGrade(packet);
                    break;

                case 7:
                    this.HandleConsortiaAlly(packet);
                    break;

                case 8:
                    this.HandleConsortiaDuty(packet);
                    break;

                case 9:
                    this.HandleConsortiaRichesOffer(packet);
                    break;

                case 10:
                    this.HandleConsortiaShopUpGrade(packet);
                    break;

                case 11:
                    this.HandleConsortiaSmithUpGrade(packet);
                    break;

                case 12:
                    this.HandleConsortiaStoreUpGrade(packet);
                    break;
            }
        }

        public void HandleConsortiaRichesOffer(GSPacketIn packet)
        {
            int num = packet.ReadInt();
            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
            {
                if (player.PlayerCharacter.ConsortiaID == num)
                {
                    player.Out.SendTCP(packet);
                }
            }
        }

        public void HandleConsortiaShopUpGrade(GSPacketIn packet)
        {
            int consortiaID = packet.ReadInt();
            packet.ReadString();
            int shopLevel = packet.ReadInt();
            ConsortiaMgr.ConsortiaShopUpGrade(consortiaID, shopLevel);
            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
            {
                if (player.PlayerCharacter.ConsortiaID == consortiaID)
                {
                    player.PlayerCharacter.ShopLevel = shopLevel;
                    player.Out.SendTCP(packet);
                }
            }
        }

        public void HandleConsortiaSmithUpGrade(GSPacketIn packet)
        {
            int consortiaID = packet.ReadInt();
            packet.ReadString();
            int smithLevel = packet.ReadInt();
            ConsortiaMgr.ConsortiaSmithUpGrade(consortiaID, smithLevel);
            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
            {
                if (player.PlayerCharacter.ConsortiaID == consortiaID)
                {
                    player.PlayerCharacter.SmithLevel = smithLevel;
                    player.Out.SendTCP(packet);
                }
            }
        }

        public void HandleConsortiaStoreUpGrade(GSPacketIn packet)
        {
            int consortiaID = packet.ReadInt();
            packet.ReadString();
            int storeLevel = packet.ReadInt();
            ConsortiaMgr.ConsortiaStoreUpGrade(consortiaID, storeLevel);
            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
            {
                if (player.PlayerCharacter.ConsortiaID == consortiaID)
                {
                    player.PlayerCharacter.StoreLevel = storeLevel;
                    player.Out.SendTCP(packet);
                }
            }
        }

        public void HandleConsortiaUpGrade(GSPacketIn packet)
        {
            int consortiaID = packet.ReadInt();
            packet.ReadString();
            int consortiaLevel = packet.ReadInt();
            ConsortiaMgr.ConsortiaUpGrade(consortiaID, consortiaLevel);
            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
            {
                if (player.PlayerCharacter.ConsortiaID == consortiaID)
                {
                    player.PlayerCharacter.ConsortiaLevel = consortiaLevel;
                    player.Out.SendTCP(packet);
                }
            }
        }

        public void HandleConsortiaUserDelete(GSPacketIn packet)
        {
            int num = packet.ReadInt();
            int num2 = packet.ReadInt();
            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
            {
                if ((player.PlayerCharacter.ConsortiaID == num2) || (player.PlayerCharacter.ID == num))
                {
                    if (player.PlayerCharacter.ID == num)
                    {
                        player.ClearConsortia();
                    }
                    player.Out.SendTCP(packet);
                }
            }
        }

        public void HandleConsortiaUserInvite(GSPacketIn packet)
        {
            packet.ReadInt();
            int num = packet.ReadInt();
            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
            {
                if (player.PlayerCharacter.ID == num)
                {
                    player.Out.SendTCP(packet);
                    break;
                }
            }
        }

        public void HandleConsortiaUserPass(GSPacketIn packet)
        {
            packet.ReadInt();
            packet.ReadBoolean();
            int consortiaID = packet.ReadInt();
            string str = packet.ReadString();
            int num2 = packet.ReadInt();
            packet.ReadString();
            packet.ReadInt();
            packet.ReadString();
            packet.ReadInt();
            string str2 = packet.ReadString();
            packet.ReadInt();
            packet.ReadInt();
            packet.ReadInt();
            packet.ReadDateTime();
            packet.ReadInt();
            int num3 = packet.ReadInt();
            packet.ReadInt();
            packet.ReadBoolean();
            int num4 = packet.ReadInt();
            packet.ReadInt();
            packet.ReadInt();
            packet.ReadInt();
            int num5 = packet.ReadInt();
            packet.ReadString();
            packet.ReadInt();
            packet.ReadInt();
            packet.ReadString();
            packet.ReadInt();
            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
            {
                if (player.PlayerCharacter.ID == num2)
                {
                    player.BeginChanges();
                    player.PlayerCharacter.ConsortiaID = consortiaID;
                    player.PlayerCharacter.ConsortiaName = str;
                    player.PlayerCharacter.DutyName = str2;
                    player.PlayerCharacter.DutyLevel = num3;
                    player.PlayerCharacter.Right = num4;
                    player.PlayerCharacter.ConsortiaRepute = num5;
                    ConsortiaInfo info = ConsortiaMgr.FindConsortiaInfo(consortiaID);
                    if (info != null)
                    {
                        player.PlayerCharacter.ConsortiaLevel = info.Level;
                    }
                    player.CommitChanges();
                }
                if (player.PlayerCharacter.ConsortiaID == consortiaID)
                {
                    player.Out.SendTCP(packet);
                }
            }
        }

        public void HandleChargeMoney(GSPacketIn packet)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(packet.ClientID);
            if (playerById != null)
            {
                playerById.ChargeToUser();
            }
        }

        protected void HandleChatConsortia(GSPacketIn packet)
        {
            packet.ReadByte();
            packet.ReadBoolean();
            packet.ReadString();
            packet.ReadString();
            int num = packet.ReadInt();
            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
            {
                if (player.PlayerCharacter.ConsortiaID == num)
                {
                    player.Out.SendTCP(packet);
                }
            }
        }

        protected void HandleChatPersonal(GSPacketIn packet)
        {
            int receiverID = packet.ReadInt();
            string nickName = packet.ReadString();
            string str2 = packet.ReadString();
            string msg = packet.ReadString();
            bool isAutoReply = packet.ReadBoolean();
            int playerID = 0;
            GamePlayer clientByPlayerNickName = WorldMgr.GetClientByPlayerNickName(nickName);
            GamePlayer player2 = WorldMgr.GetClientByPlayerNickName(str2);
            if (player2 != null)
            {
                playerID = player2.PlayerCharacter.ID;
            }
            if (!((clientByPlayerNickName == null) || clientByPlayerNickName.IsBlackFriend(playerID)))
            {
                receiverID = clientByPlayerNickName.PlayerCharacter.ID;
                clientByPlayerNickName.SendPrivateChat(receiverID, nickName, str2, msg, isAutoReply);
            }
        }

        public void HandleEventRank(GSPacketIn pkg)
        {
            if (pkg.ReadByte() == 1)
            {
                List<RankingLightriddleInfo> list = new List<RankingLightriddleInfo>();
                int num3 = pkg.ReadInt();
                for (int i = 0; i < num3; i++)
                {
                    RankingLightriddleInfo item = new RankingLightriddleInfo {
                        Rank = pkg.ReadInt(),
                        NickName = pkg.ReadString(),
                        TypeVIP = pkg.ReadByte(),
                        Integer = pkg.ReadInt()
                    };
                    list.Add(item);
                }
                int myRank = pkg.ReadInt();
                GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ReadInt());
                if (playerById != null)
                {
                    playerById.Actives.SendLightriddleRank(myRank, list);
                }
            }
        }

        public void HandleFightFootballTime(GSPacketIn pkg)
        {
            ActiveSystemMgr.UpdateIsFightFootballTime(pkg.ReadBoolean());
        }

        public void HandleFirendResponse(GSPacketIn packet)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(packet.ReadInt());
            if (playerById != null)
            {
                playerById.Out.SendTCP(packet);
            }
        }

        public void HandleFriend(GSPacketIn pkg)
        {
            switch (pkg.ReadByte())
            {
                case 0xa5:
                    this.HandleFriendState(pkg);
                    break;

                case 0xa6:
                    this.HandleFirendResponse(pkg);
                    break;
            }
        }

        public void HandleFriendState(GSPacketIn pkg)
        {
            WorldMgr.ChangePlayerState(pkg.ClientID, pkg.ReadInt(), pkg.ReadInt());
        }

        protected void HandleKitoffPlayer(object stateInfo)
        {
            try
            {
                GSPacketIn @in = (GSPacketIn) stateInfo;
                int playerId = @in.ReadInt();
                GamePlayer playerById = WorldMgr.GetPlayerById(playerId);
                if (playerById != null)
                {
                    string msg = @in.ReadString();
                    playerById.Out.SendKitoff(msg);
                    playerById.Client.Disconnect();
                }
                else
                {
                    this.SendUserOffline(playerId, 0);
                }
            }
            catch (Exception exception)
            {
                GameServer.log.Error("HandleKitoffPlayer", exception);
            }
        }

        public void HandleLeagueOpenClose(GSPacketIn pkg)
        {
            ActiveSystemMgr.UpdateIsLeagueOpen(pkg.ReadBoolean());
        }

        public void HandleMacroDrop(GSPacketIn pkg)
        {
            Dictionary<int, MacroDropInfo> temp = new Dictionary<int, MacroDropInfo>();
            int num = pkg.ReadInt();
            for (int i = 0; i < num; i++)
            {
                int key = pkg.ReadInt();
                int dropCount = pkg.ReadInt();
                int maxDropCount = pkg.ReadInt();
                MacroDropInfo info = new MacroDropInfo(dropCount, maxDropCount);
                temp.Add(key, info);
            }
            MacroDropMgr.UpdateDropInfo(temp);
        }

        public void HandleMailResponse(GSPacketIn packet)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(packet.ReadInt());
            if (playerById != null)
            {
                playerById.Out.SendTCP(packet);
            }
        }

        public void HandleMarryRoomInfoToPlayer(GSPacketIn packet)
        {
            int playerId = packet.ReadInt();
            GamePlayer playerById = WorldMgr.GetPlayerById(playerId);
            if (playerById != null)
            {
                packet.Code = 0xfc;
                packet.ClientID = playerId;
                playerById.Out.SendTCP(packet);
            }
        }

        public void HandleRate(GSPacketIn packet)
        {
            RateMgr.ReLoad();
        }

        public void HandleReload(GSPacketIn packet)
        {
            eReloadType type = (eReloadType) packet.ReadInt();
            bool val = false;
            switch (type)
            {
                case eReloadType.ball:
                    val = BallMgr.ReLoad();
                    break;

                case eReloadType.map:
                    val = MapMgr.ReLoadMap();
                    break;

                case eReloadType.mapserver:
                    val = MapMgr.ReLoadMapServer();
                    break;

                case eReloadType.item:
                    val = ItemMgr.ReLoad();
                    break;

                case eReloadType.quest:
                    val = QuestMgr.ReLoad();
                    break;

                case eReloadType.fusion:
                    val = FusionMgr.ReLoad();
                    break;

                case eReloadType.server:
                    GameServer.Instance.Configuration.Refresh();
                    break;

                case eReloadType.rate:
                    val = RateMgr.ReLoad();
                    break;

                case eReloadType.consortia:
                    val = ConsortiaMgr.ReLoad();
                    break;

                case eReloadType.shop:
                    val = ShopMgr.ReLoad();
                    break;

                case eReloadType.fight:
                    val = FightRateMgr.ReLoad();
                    break;

                case eReloadType.dailyaward:
                    val = AwardMgr.ReLoad();
                    break;

                case eReloadType.language:
                    val = LanguageMgr.Reload("");
                    break;
            }
            packet.WriteInt(GameServer.Instance.Configuration.ServerID);
            packet.WriteBoolean(val);
            this.SendTCP(packet);
        }

        protected void HandleRSAKey(GSPacketIn packet)
        {
            RSAParameters parameters = new RSAParameters {
                Modulus = packet.ReadBytes(0x80),
                Exponent = packet.ReadBytes()
            };
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(parameters);
            this.SendRSALogin(rsa, this.m_loginKey);
            this.SendListenIPPort(IPAddress.Parse(GameServer.Instance.Configuration.Ip), GameServer.Instance.Configuration.Port);
        }

        public void HandleShutdown(GSPacketIn pkg)
        {
            GameServer.Instance.Shutdown();
        }

        public void HandleSysMess(GSPacketIn packet)
        {
            if (packet.ReadInt() == 1)
            {
                int playerId = packet.ReadInt();
                string str = packet.ReadString();
                GamePlayer playerById = WorldMgr.GetPlayerById(playerId);
                if (playerById != null)
                {
                    playerById.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("LoginServerConnector.HandleSysMess.Msg1", new object[] { str }));
                }
            }
        }

        public void HandleSystemNotice(GSPacketIn packet)
        {
            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
            {
                player.Out.SendTCP(packet);
            }
        }

        public void HandleUpdatePlayerMarriedState(GSPacketIn packet)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(packet.ReadInt());
            if (playerById != null)
            {
                playerById.LoadMarryProp();
                playerById.LoadMarryMessage();
                playerById.QuestInventory.ClearMarryQuest();
            }
        }

        protected void HandleUserOffline(GSPacketIn packet)
        {
            int num = packet.ReadInt();
            for (int i = 0; i < num; i++)
            {
                int playerId = packet.ReadInt();
                int consortiaID = packet.ReadInt();
                if (LoginMgr.ContainsUser(playerId))
                {
                    this.SendAllowUserLogin(playerId);
                }
                WorldMgr.OnPlayerOffline(playerId, consortiaID);
            }
        }

        protected void HandleUserOnline(GSPacketIn packet)
        {
            int num = packet.ReadInt();
            for (int i = 0; i < num; i++)
            {
                int playerId = packet.ReadInt();
                int consortiaID = packet.ReadInt();
                LoginMgr.ClearLoginPlayer(playerId);
                GamePlayer playerById = WorldMgr.GetPlayerById(playerId);
                if (playerById != null)
                {
                    GameServer.log.Error("Player hang in server!!!");
                    playerById.Out.SendKitoff(LanguageMgr.GetTranslation("Game.Server.LoginNext", new object[0]));
                    playerById.Client.Disconnect();
                }
                WorldMgr.OnPlayerOnline(playerId, consortiaID);
            }
        }

        public void HandleWorldBossFightOver(GSPacketIn pkg)
        {
            RoomMgr.WorldBossRoom.SendFightOver();
        }

        public void HandleWorldBossPrivateInfo(GSPacketIn pkg)
        {
            string name = pkg.ReadString();
            int damage = pkg.ReadInt();
            int honor = pkg.ReadInt();
            RoomMgr.WorldBossRoom.SendPrivateInfo(name, damage, honor);
        }

        public void HandleWorldBossRank(GSPacketIn pkg)
        {
            RoomMgr.WorldBossRoom.UpdateWorldBossRankCrosszone(pkg);
        }

        public void HandleWorldBossRoomClose(GSPacketIn pkg)
        {
            if (pkg.ReadByte() == 0)
            {
                RoomMgr.WorldBossRoom.SendRoomClose();
            }
            else
            {
                RoomMgr.WorldBossRoom.WorldBossClose();
            }
        }

        public void HandleWorldBossUpdate(GSPacketIn pkg)
        {
            RoomMgr.WorldBossRoom.UpdateWorldBoss(pkg);
        }

        public void HandleWorldBossUpdateBlood(GSPacketIn pkg)
        {
            RoomMgr.WorldBossRoom.SendUpdateBlood(pkg);
        }

        public void HandleWorldEvent(GSPacketIn pkg)
        {
            if ((pkg.ReadByte() == 2) && (pkg.ReadInt() == 1))
            {
                LanternriddlesInfo lanternriddles = new LanternriddlesInfo {
                    PlayerID = pkg.ReadInt(),
                    QuestionIndex = pkg.ReadInt(),
                    QuestionView = pkg.ReadInt(),
                    EndDate = pkg.ReadDateTime(),
                    DoubleFreeCount = pkg.ReadInt(),
                    DoublePrice = pkg.ReadInt(),
                    HitFreeCount = pkg.ReadInt(),
                    HitPrice = pkg.ReadInt(),
                    MyInteger = pkg.ReadInt(),
                    QuestionNum = pkg.ReadInt(),
                    Option = pkg.ReadInt(),
                    IsHint = pkg.ReadBoolean(),
                    IsDouble = pkg.ReadBoolean()
                };
                ActiveSystemMgr.AddOrUpdateLanternriddles(lanternriddles.PlayerID, lanternriddles);
            }
        }

        protected override void OnDisconnect()
        {
            base.OnDisconnect();
        }

        public override void OnRecvPacket(GSPacketIn pkg)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.AsynProcessPacket), pkg);
        }

        public void SendAllowUserLogin(int playerid)
        {
            GSPacketIn pkg = new GSPacketIn(3);
            pkg.WriteInt(playerid);
            this.SendTCP(pkg);
        }

        public void SendConsortiaAlly(int consortiaID1, int consortiaID2, int state)
        {
            GSPacketIn pkg = new GSPacketIn(0x80);
            pkg.WriteByte(7);
            pkg.WriteInt(consortiaID1);
            pkg.WriteInt(consortiaID2);
            pkg.WriteInt(state);
            this.SendTCP(pkg);
            ConsortiaMgr.UpdateConsortiaAlly(consortiaID1, consortiaID2, state);
        }

        public void SendConsortiaBanChat(int playerid, string playerName, int handleID, string handleName, bool isBan)
        {
            GSPacketIn pkg = new GSPacketIn(0x80);
            pkg.WriteByte(5);
            pkg.WriteBoolean(isBan);
            pkg.WriteInt(playerid);
            pkg.WriteString(playerName);
            pkg.WriteInt(handleID);
            pkg.WriteString(handleName);
            this.SendTCP(pkg);
        }

        public void SendConsortiaCreate(int consortiaID, int offer, string consotiaName)
        {
            GSPacketIn pkg = new GSPacketIn(130);
            pkg.WriteInt(consortiaID);
            pkg.WriteInt(offer);
            pkg.WriteString(consotiaName);
            this.SendTCP(pkg);
        }

        public void SendConsortiaDelete(int consortiaID)
        {
            GSPacketIn pkg = new GSPacketIn(0x80);
            pkg.WriteByte(2);
            pkg.WriteInt(consortiaID);
            this.SendTCP(pkg);
        }

        public void SendConsortiaDuty(ConsortiaDutyInfo info, int updateType, int consortiaID)
        {
            this.SendConsortiaDuty(info, updateType, consortiaID, 0, "", 0, "");
        }

        public void SendConsortiaDuty(ConsortiaDutyInfo info, int updateType, int consortiaID, int playerID, string playerName, int handleID, string handleName)
        {
            GSPacketIn pkg = new GSPacketIn(0x80);
            pkg.WriteByte(8);
            pkg.WriteByte((byte) updateType);
            pkg.WriteInt(consortiaID);
            pkg.WriteInt(playerID);
            pkg.WriteString(playerName);
            pkg.WriteInt(info.Level);
            pkg.WriteString(info.DutyName);
            pkg.WriteInt(info.Right);
            pkg.WriteInt(handleID);
            pkg.WriteString(handleName);
            this.SendTCP(pkg);
        }

        public void SendConsortiaFight(int consortiaID, int riches, string msg)
        {
            GSPacketIn pkg = new GSPacketIn(0x9e);
            pkg.WriteInt(consortiaID);
            pkg.WriteInt(riches);
            pkg.WriteString(msg);
            this.SendTCP(pkg);
        }

        public void SendConsortiaInvite(int ID, int playerid, string playerName, int inviteID, string intviteName, string consortiaName, int consortiaID)
        {
            GSPacketIn pkg = new GSPacketIn(0x80);
            pkg.WriteByte(4);
            pkg.WriteInt(ID);
            pkg.WriteInt(playerid);
            pkg.WriteString(playerName);
            pkg.WriteInt(inviteID);
            pkg.WriteString(intviteName);
            pkg.WriteInt(consortiaID);
            pkg.WriteString(consortiaName);
            this.SendTCP(pkg);
        }

        public void SendConsortiaKillUpGrade(ConsortiaInfo info)
        {
            GSPacketIn pkg = new GSPacketIn(0x80);
            pkg.WriteByte(13);
            pkg.WriteInt(info.ConsortiaID);
            pkg.WriteString(info.ConsortiaName);
            pkg.WriteInt(info.SkillLevel);
            this.SendTCP(pkg);
        }

        public void SendConsortiaOffer(int consortiaID, int offer, int riches)
        {
            GSPacketIn pkg = new GSPacketIn(0x9c);
            pkg.WriteInt(consortiaID);
            pkg.WriteInt(offer);
            pkg.WriteInt(riches);
            this.SendTCP(pkg);
        }

        public void SendConsortiaRichesOffer(int consortiaID, int playerID, string playerName, int riches)
        {
            GSPacketIn pkg = new GSPacketIn(0x80);
            pkg.WriteByte(9);
            pkg.WriteInt(consortiaID);
            pkg.WriteInt(playerID);
            pkg.WriteString(playerName);
            pkg.WriteInt(riches);
            this.SendTCP(pkg);
        }

        public void SendConsortiaShopUpGrade(ConsortiaInfo info)
        {
            GSPacketIn pkg = new GSPacketIn(0x80);
            pkg.WriteByte(10);
            pkg.WriteInt(info.ConsortiaID);
            pkg.WriteString(info.ConsortiaName);
            pkg.WriteInt(info.ShopLevel);
            this.SendTCP(pkg);
        }

        public void SendConsortiaSmithUpGrade(ConsortiaInfo info)
        {
            GSPacketIn pkg = new GSPacketIn(0x80);
            pkg.WriteByte(11);
            pkg.WriteInt(info.ConsortiaID);
            pkg.WriteString(info.ConsortiaName);
            pkg.WriteInt(info.SmithLevel);
            this.SendTCP(pkg);
        }

        public void SendConsortiaStoreUpGrade(ConsortiaInfo info)
        {
            GSPacketIn pkg = new GSPacketIn(0x80);
            pkg.WriteByte(12);
            pkg.WriteInt(info.ConsortiaID);
            pkg.WriteString(info.ConsortiaName);
            pkg.WriteInt(info.StoreLevel);
            this.SendTCP(pkg);
        }

        public void SendConsortiaUpGrade(ConsortiaInfo info)
        {
            GSPacketIn pkg = new GSPacketIn(0x80);
            pkg.WriteByte(6);
            pkg.WriteInt(info.ConsortiaID);
            pkg.WriteString(info.ConsortiaName);
            pkg.WriteInt(info.Level);
            this.SendTCP(pkg);
        }

        public void SendConsortiaUserDelete(int playerid, int consortiaID, bool isKick, string nickName, string kickName)
        {
            GSPacketIn pkg = new GSPacketIn(0x80);
            pkg.WriteByte(3);
            pkg.WriteInt(playerid);
            pkg.WriteInt(consortiaID);
            pkg.WriteBoolean(isKick);
            pkg.WriteString(nickName);
            pkg.WriteString(kickName);
            this.SendTCP(pkg);
        }

        public void SendConsortiaUserPass(int playerid, string playerName, ConsortiaUserInfo info, bool isInvite, int consortiaRepute, string loginName, int fightpower, int Offer)
        {
            GSPacketIn pkg = new GSPacketIn(0x80, playerid);
            pkg.WriteByte(1);
            pkg.WriteInt(info.ID);
            pkg.WriteBoolean(isInvite);
            pkg.WriteInt(info.ConsortiaID);
            pkg.WriteString(info.ConsortiaName);
            pkg.WriteInt(info.UserID);
            pkg.WriteString(info.UserName);
            pkg.WriteInt(playerid);
            pkg.WriteString(playerName);
            pkg.WriteInt(info.DutyID);
            pkg.WriteString(info.DutyName);
            pkg.WriteInt(info.Offer);
            pkg.WriteInt(info.RichesOffer);
            pkg.WriteInt(info.RichesRob);
            pkg.WriteDateTime(info.LastDate);
            pkg.WriteInt(info.Grade);
            pkg.WriteInt(info.Level);
            pkg.WriteInt(info.State);
            pkg.WriteBoolean(info.Sex);
            pkg.WriteInt(info.Right);
            pkg.WriteInt(info.Win);
            pkg.WriteInt(info.Total);
            pkg.WriteInt(info.Escape);
            pkg.WriteInt(consortiaRepute);
            pkg.WriteString(loginName);
            pkg.WriteInt(fightpower);
            pkg.WriteInt(0);
            pkg.WriteString("Honor");
            pkg.WriteInt(info.RichesOffer);
            ConsortiaInfo info2 = ConsortiaMgr.FindConsortiaInfo(info.ConsortiaID);
            pkg.WriteInt(info2.Level);
            this.SendTCP(pkg);
        }

        public GSPacketIn SendGetLightriddleInfo(int playerID)
        {
            GSPacketIn pkg = new GSPacketIn(0x5b);
            pkg.WriteByte(2);
            pkg.WriteInt(playerID);
            this.SendTCP(pkg);
            return pkg;
        }

        public void SendLightriddleInfo(LanternriddlesInfo Lanternriddles)
        {
            if (Lanternriddles != null)
            {
                GSPacketIn pkg = new GSPacketIn(0x5b);
                pkg.WriteByte(1);
                pkg.WriteInt(Lanternriddles.PlayerID);
                pkg.WriteInt(Lanternriddles.QuestionIndex);
                pkg.WriteInt(Lanternriddles.QuestionView);
                pkg.WriteDateTime(Lanternriddles.EndDate);
                pkg.WriteInt(Lanternriddles.DoubleFreeCount);
                pkg.WriteInt(Lanternriddles.DoublePrice);
                pkg.WriteInt(Lanternriddles.HitFreeCount);
                pkg.WriteInt(Lanternriddles.HitPrice);
                pkg.WriteInt(Lanternriddles.MyInteger);
                pkg.WriteInt(Lanternriddles.QuestionNum);
                pkg.WriteInt(Lanternriddles.Option);
                pkg.WriteBoolean(Lanternriddles.IsHint);
                pkg.WriteBoolean(Lanternriddles.IsDouble);
                this.SendTCP(pkg);
            }
        }

        public GSPacketIn SendLightriddleRank(string Nickname, int PlayeID)
        {
            GSPacketIn pkg = new GSPacketIn(90);
            pkg.WriteByte(1);
            pkg.WriteString(Nickname);
            pkg.WriteInt(PlayeID);
            this.SendTCP(pkg);
            return pkg;
        }

        public GSPacketIn SendLightriddleUpateRank(int Integer, PlayerInfo player)
        {
            GSPacketIn pkg = new GSPacketIn(90);
            pkg.WriteByte(2);
            pkg.WriteString(player.NickName);
            pkg.WriteInt(player.typeVIP);
            pkg.WriteInt(Integer);
            pkg.WriteInt(player.ID);
            this.SendTCP(pkg);
            return pkg;
        }

        public void SendListenIPPort(IPAddress ip, int port)
        {
            GSPacketIn pkg = new GSPacketIn(240);
            pkg.Write(ip.GetAddressBytes());
            pkg.WriteInt(port);
            this.SendTCP(pkg);
        }

        public GSPacketIn SendLuckStarRewardRecord(int PlayerID, string nickName, int TemplateID, int Count, int isVip)
        {
            GSPacketIn pkg = new GSPacketIn(90);
            pkg.WriteByte(4);
            pkg.WriteInt(PlayerID);
            pkg.WriteString(nickName);
            pkg.WriteInt(TemplateID);
            pkg.WriteInt(Count);
            pkg.WriteInt(isVip);
            this.SendTCP(pkg);
            return pkg;
        }

        public void SendMailResponse(int playerid)
        {
            GSPacketIn pkg = new GSPacketIn(0x75);
            pkg.WriteInt(playerid);
            this.SendTCP(pkg);
        }

        public void SendMarryRoomDisposeToPlayer(int roomId)
        {
            GSPacketIn pkg = new GSPacketIn(0xf1);
            pkg.WriteInt(roomId);
            this.SendTCP(pkg);
        }

        public void SendMarryRoomInfoToPlayer(int playerId, bool state, MarryRoomInfo info)
        {
            GSPacketIn pkg = new GSPacketIn(14);
            pkg.WriteInt(playerId);
            pkg.WriteBoolean(state);
            if (state)
            {
                pkg.WriteInt(info.ID);
                pkg.WriteString(info.Name);
                pkg.WriteInt(info.MapIndex);
                pkg.WriteInt(info.AvailTime);
                pkg.WriteInt(info.PlayerID);
                pkg.WriteInt(info.GroomID);
                pkg.WriteInt(info.BrideID);
                pkg.WriteDateTime(info.BeginTime);
                pkg.WriteBoolean(info.IsGunsaluteUsed);
            }
            this.SendTCP(pkg);
        }

        public void SendPacket(GSPacketIn packet)
        {
            this.SendTCP(packet);
        }

        public void SendPingCenter()
        {
            GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
            int val = (allPlayers == null) ? 0 : allPlayers.Length;
            GSPacketIn pkg = new GSPacketIn(12);
            pkg.WriteInt(val);
            this.SendTCP(pkg);
        }

        public void SendRSALogin(RSACryptoServiceProvider rsa, string key)
        {
            GSPacketIn pkg = new GSPacketIn(1);
            pkg.Write(rsa.Encrypt(Encoding.UTF8.GetBytes(key), false));
            this.SendTCP(pkg);
        }

        public void SendShutdown(bool isStoping)
        {
            GSPacketIn pkg = new GSPacketIn(15);
            pkg.WriteInt(this.m_serverId);
            pkg.WriteBoolean(isStoping);
            this.SendTCP(pkg);
        }

        public void SendToAllConsortiaMember(ConsortiaInfo consortia, int type)
        {
            if (!ConsortiaBossMgr.AddConsortia(consortia.ConsortiaID, consortia))
            {
                ConsortiaBossMgr.UpdateConsortia(consortia);
            }
            foreach (GamePlayer player in WorldMgr.GetAllPlayers())
            {
                if (player.PlayerCharacter.ConsortiaID == consortia.ConsortiaID)
                {
                    player.SendConsortiaBossInfo(consortia);
                    switch (type)
                    {
                        case 0:
                            player.SendConsortiaBossOpenClose(0);
                            break;

                        case 1:
                            player.SendConsortiaBossOpenClose(1);
                            break;

                        case 2:
                            player.SendConsortiaBossOpenClose(2);
                            break;

                        case 3:
                            player.SendConsortiaBossOpenClose(3);
                            break;
                    }
                }
            }
        }

        public void SendUpdatePlayerMarriedStates(int playerId)
        {
            GSPacketIn pkg = new GSPacketIn(13);
            pkg.WriteInt(playerId);
            this.SendTCP(pkg);
        }

        public GSPacketIn SendUserOffline(int playerid, int consortiaID)
        {
            GSPacketIn pkg = new GSPacketIn(4);
            pkg.WriteInt(1);
            pkg.WriteInt(playerid);
            pkg.WriteInt(consortiaID);
            this.SendTCP(pkg);
            return pkg;
        }

        public GSPacketIn SendUserOnline(Dictionary<int, int> users)
        {
            GSPacketIn pkg = new GSPacketIn(5);
            pkg.WriteInt(users.Count);
            foreach (KeyValuePair<int, int> pair in users)
            {
                pkg.WriteInt(pair.Key);
                pkg.WriteInt(pair.Value);
            }
            this.SendTCP(pkg);
            return pkg;
        }

        public GSPacketIn SendUserOnline(int playerid, int consortiaID)
        {
            GSPacketIn pkg = new GSPacketIn(5);
            pkg.WriteInt(1);
            pkg.WriteInt(playerid);
            pkg.WriteInt(consortiaID);
            this.SendTCP(pkg);
            return pkg;
        }
    }
}

