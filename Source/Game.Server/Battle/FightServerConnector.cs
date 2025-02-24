namespace Game.Server.Battle
{
    using Bussiness.Managers;
    using Game.Base;
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Protocol;
    using Game.Logic.Phy.Object;
    using Game.Server;
    using Game.Server.Buffer;
    using Game.Server.GameObjects;
    using Game.Server.Managers;
    using Game.Server.RingStation;
    using Game.Server.Rooms;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Reflection;

    public class FightServerConnector : BaseConnector
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private string m_key;
        private BattleServer m_server;

        public FightServerConnector(BattleServer server, string ip, int port, string key) : base(ip, port, true, new byte[0x2000], new byte[0x2000])
        {
            this.m_server = server;
            this.m_key = key;
            base.Strict = true;
        }

        protected void AsynProcessPacket(object state)
        {
            try
            {
                GSPacketIn pkg = state as GSPacketIn;
                int code = pkg.Code;
                switch (code)
                {
                    case 0x13:
                        this.HandlePlayerChatSend(pkg);
                        return;

                    case 0x20:
                        this.HandleSendToPlayer(pkg);
                        return;

                    case 0x21:
                        this.HandleUpdatePlayerGameId(pkg);
                        return;

                    case 0x22:
                        this.HandleDisconnectPlayer(pkg);
                        return;

                    case 0x23:
                        this.HandlePlayerOnGameOver(pkg);
                        return;

                    case 0x24:
                        this.HandlePlayerOnUsingItem(pkg);
                        return;

                    case 0x26:
                        this.HandlePlayerAddGold(pkg);
                        return;

                    case 0x27:
                        this.HandlePlayerAddGP(pkg);
                        return;

                    case 40:
                        this.HandlePlayerOnKillingLiving(pkg);
                        return;

                    case 0x29:
                        this.HandlePlayerOnMissionOver(pkg);
                        return;

                    case 0x2a:
                        this.HandlePlayerConsortiaFight(pkg);
                        return;

                    case 0x2b:
                        this.HandlePlayerSendConsortiaFight(pkg);
                        return;

                    case 0x2c:
                        this.HandlePlayerRemoveGold(pkg);
                        return;

                    case 0x2d:
                        this.HandlePlayerRemoveMoney(pkg);
                        return;

                    case 0x30:
                        this.HandlePlayerAddTemplate1(pkg);
                        return;

                    case 0x31:
                        this.HandlePlayerRemoveGP(pkg);
                        return;

                    case 50:
                        this.HandlePlayerRemoveOffer(pkg);
                        return;

                    case 0:
                        this.HandleRSAKey(pkg);
                        return;

                    case 0x41:
                        this.HandleRoomRemove(pkg);
                        return;

                    case 0x42:
                        this.HandleStartGame(pkg);
                        return;

                    case 0x43:
                        this.HandleSendToRoom(pkg);
                        return;

                    case 0x44:
                        this.HandleStopGame(pkg);
                        return;

                    case 0x45:
                        this.HandleUpdateRoomId(pkg);
                        break;

                    case 0x49:
                        this.HandlePlayerHealstone(pkg);
                        return;

                    case 0x4a:
                        this.HandlePlayerAddMoney(pkg);
                        return;

                    case 0x4b:
                        this.HandlePlayerAddGiftToken(pkg);
                        return;

                    case 0x4c:
                        this.HandlePlayerAddMedal(pkg);
                        return;

                    case 0x4d:
                        this.HandleFindConsortiaAlly(pkg);
                        return;

                    case 0x54:
                        this.HandlePlayerAddLeagueMoney(pkg);
                        return;

                    case 0x55:
                        this.HandlePlayerAddPrestige(pkg);
                        return;

                    case 0x56:
                        this.HandlePlayerUpdateRestCount(pkg);
                        return;

                    case 0x57:
                        this.HandleFootballTakeOut(pkg);
                        return;

                    case 0x58:
                        this.HandleFightNPC(pkg);
                        return;

                    case 0x59:
                        this.HandlePlayerAddActiveMoney(pkg);
                        return;

                    case 0xcb:
                        this.HandleFightNPC(pkg);
                        return;

                    case 100:
                        this.HandlePlayerAddGoXu(pkg);
                        return;
                }
                Console.WriteLine("??????????LoginServerConnector: " + ((eFightPackageType) code));
            }
            catch (Exception exception)
            {
                GameServer.log.Error("AsynProcessPacket", exception);
            }
        }

        private void HandleDisconnectPlayer(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            if (playerById != null)
            {
                playerById.Disconnect();
            }
        }

        private void HandleFightNPC(GSPacketIn packet)
        {
            int roomtype = packet.ReadInt();
            int gametype = packet.ReadInt();
            int npcId = packet.ReadInt();
            GamePlayer playerById = WorldMgr.GetPlayerById(packet.Parameter1);
            if (playerById == null)
            {
                RingStationMgr.CreateAutoBot(roomtype, gametype, npcId);
            }
            else
            {
                RingStationMgr.CreateAutoBot(playerById, roomtype, gametype, npcId);
            }
        }

        public void HandleFindConsortiaAlly(GSPacketIn pkg)
        {
            int state = ConsortiaMgr.FindConsortiaAlly(pkg.ReadInt(), pkg.ReadInt());
            this.SendFindConsortiaAlly(state, pkg.ReadInt());
        }

        private void HandleFootballTakeOut(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            if (playerById != null)
            {
                bool isWin = pkg.ReadBoolean();
                playerById.FootballTakeOut(isWin);
            }
        }

        private void HandlePlayerAddActiveMoney(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            if (playerById != null)
            {
                playerById.AddActiveMoney(pkg.Parameter1);
            }
        }

        private void HandlePlayerAddGold(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            if (playerById != null)
            {
                playerById.AddGold(pkg.Parameter1);
            }
        }

        private void HandlePlayerAddGoXu(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            if (playerById != null)
            {
                playerById.AddGoXu(pkg.Parameter1);
            }
        }

        private void HandlePlayerAddGP(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            if (playerById != null)
            {
                playerById.AddGP(pkg.Parameter1);
            }
        }

        private void HandlePlayerAddGiftToken(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            if (playerById != null)
            {
                playerById.AddGiftToken(pkg.Parameter1);
            }
        }

        private void HandlePlayerAddLeagueMoney(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            if (playerById != null)
            {
                playerById.AddLeagueMoney(pkg.Parameter1);
            }
        }

        private void HandlePlayerAddMedal(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            if (playerById != null)
            {
                playerById.AddMedal(pkg.Parameter1);
            }
        }

        private void HandlePlayerAddMoney(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            if (playerById != null)
            {
                playerById.AddMoney(pkg.Parameter1);
            }
        }

        private void HandlePlayerAddPrestige(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            if (playerById != null)
            {
                bool isWin = pkg.ReadBoolean();
                playerById.AddPrestige(isWin);
            }
        }

        private void HandlePlayerAddTemplate1(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            if (playerById != null)
            {
                ItemTemplateInfo goods = ItemMgr.FindItemTemplate(pkg.ReadInt());
                eBageType bagType = (eBageType) pkg.ReadByte();
                if (goods != null)
                {
                    int count = pkg.ReadInt();
                    SqlDataProvider.Data.ItemInfo cloneItem = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(goods, count, 0x76);
                    cloneItem.Count = count;
                    cloneItem.ValidDate = pkg.ReadInt();
                    cloneItem.IsBinds = pkg.ReadBoolean();
                    cloneItem.IsUsed = pkg.ReadBoolean();
                    playerById.AddTemplate(cloneItem, bagType, cloneItem.Count, eGameView.BatleTypeGet);
                }
            }
        }

        private void HandlePlayerConsortiaFight(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            Dictionary<int, Player> players = new Dictionary<int, Player>();
            int consortiaWin = pkg.ReadInt();
            int consortiaLose = pkg.ReadInt();
            int count = pkg.ReadInt();
            for (int i = 0; i < count; i++)
            {
                GamePlayer player = WorldMgr.GetPlayerById(pkg.ReadInt());
                if (player != null)
                {
                    Player player3 = new Player(player, 0, null, 0, player.PlayerCharacter.hp);
                    players.Add(i, player3);
                }
            }
            eRoomType roomType = (eRoomType) pkg.ReadByte();
            eGameType gameClass = (eGameType) pkg.ReadByte();
            int totalKillHealth = pkg.ReadInt();
            if (playerById != null)
            {
                int num6 = playerById.ConsortiaFight(consortiaWin, consortiaLose, players, roomType, gameClass, totalKillHealth, count);
            }
        }

        private void HandlePlayerChatSend(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            if (playerById != null)
            {
                playerById.SendMessage(pkg.ReadString());
            }
        }

        private void HandlePlayerHealstone(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            if (playerById != null)
            {
                playerById.RemoveHealstone();
            }
        }

        private void HandlePlayerOnGameOver(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            if (((playerById != null) && (playerById.CurrentRoom != null)) && (playerById.CurrentRoom.Game != null))
            {
                playerById.OnGameOver(playerById.CurrentRoom.Game, pkg.ReadBoolean(), pkg.ReadInt());
            }
        }

        private void HandlePlayerOnKillingLiving(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            AbstractGame game = playerById.CurrentRoom.Game;
            if (playerById != null)
            {
                playerById.OnKillingLiving(game, pkg.ReadInt(), pkg.ClientID, pkg.ReadBoolean(), pkg.ReadInt());
            }
        }

        private void HandlePlayerOnMissionOver(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            AbstractGame game = playerById.CurrentRoom.Game;
            if (playerById != null)
            {
                playerById.OnMissionOver(game, pkg.ReadBoolean(), pkg.ReadInt(), pkg.ReadInt());
            }
        }

        private void HandlePlayerOnUsingItem(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            if (playerById != null)
            {
                int templateId = pkg.ReadInt();
                bool result = playerById.UsePropItem(null, pkg.Parameter1, pkg.Parameter2, templateId, pkg.ReadBoolean());
                this.SendUsingPropInGame(playerById.CurrentRoom.Game.Id, playerById.GamePlayerId, templateId, result);
            }
        }

        private void HandlePlayerRemoveGold(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            if (playerById != null)
            {
                playerById.RemoveGold(pkg.ReadInt());
            }
        }

        private void HandlePlayerRemoveGP(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            if (playerById != null)
            {
                playerById.RemoveGP(pkg.Parameter1);
            }
        }

        private void HandlePlayerRemoveMoney(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            if (playerById != null)
            {
                playerById.RemoveMoney(pkg.ReadInt());
            }
        }

        private void HandlePlayerRemoveOffer(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            if (playerById != null)
            {
                playerById.RemoveOffer(pkg.ReadInt());
            }
        }

        private void HandlePlayerSendConsortiaFight(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            if (playerById != null)
            {
                playerById.SendConsortiaFight(pkg.ReadInt(), pkg.ReadInt(), pkg.ReadString());
            }
        }

        private void HandlePlayerUpdateRestCount(GSPacketIn pkg)
        {
            GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
            if (playerById != null)
            {
                playerById.UpdateRestCount();
            }
        }

        protected void HandleRoomRemove(GSPacketIn packet)
        {
            this.m_server.RemoveRoomImp(packet.ClientID);
        }

        protected void HandleRSAKey(GSPacketIn packet)
        {
            RSAParameters parameters = new RSAParameters {
                Modulus = packet.ReadBytes(0x80),
                Exponent = packet.ReadBytes()
            };
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(parameters);
            this.SendRSALogin(rsa, this.m_key);
        }

        protected void HandleSendToPlayer(GSPacketIn pkg)
        {
            int clientID = pkg.ClientID;
            try
            {
                GSPacketIn @in = pkg.ReadPacket();
                this.m_server.SendToUser(clientID, @in);
            }
            catch (Exception exception)
            {
                log.Error(string.Format("pkg len:{0}", pkg.Length), exception);
                log.Error(Marshal.ToHexDump("pkg content:", pkg.Buffer, 0, pkg.Length));
            }
        }

        protected void HandleSendToRoom(GSPacketIn pkg)
        {
            int clientID = pkg.ClientID;
            GSPacketIn @in = pkg.ReadPacket();
            this.m_server.SendToRoom(clientID, @in, pkg.Parameter1, pkg.Parameter2);
        }

        protected void HandleStartGame(GSPacketIn pkg)
        {
            ProxyGame game = new ProxyGame(pkg.Parameter2, this, (eRoomType) pkg.ReadInt(), (eGameType) pkg.ReadInt(), pkg.ReadInt());
            this.m_server.StartGame(pkg.Parameter1, game);
        }

        protected void HandleStopGame(GSPacketIn pkg)
        {
            int roomId = pkg.Parameter1;
            int gameId = pkg.Parameter2;
            this.m_server.StopGame(roomId, gameId, pkg.ReadInt());
        }

        private void HandleUpdatePlayerGameId(GSPacketIn pkg)
        {
            this.m_server.UpdatePlayerGameId(pkg.Parameter1, pkg.Parameter2);
        }

        private void HandleUpdateRoomId(GSPacketIn pkg)
        {
            this.m_server.UpdateRoomId(pkg.ClientID, pkg.ReadInt());
        }

        protected override void OnDisconnect()
        {
            base.OnDisconnect();
        }

        public override void OnRecvPacket(GSPacketIn pkg)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.AsynProcessPacket), pkg);
        }

        public void SendAddRoom(BaseRoom room)
        {
            GSPacketIn pkg = new GSPacketIn(0x40);
            pkg.WriteInt(room.RoomId);
            pkg.WriteInt(room.PickUpNpcId);
            pkg.WriteBoolean(false);
            pkg.WriteBoolean(room.StartWithNpc);
            pkg.WriteBoolean(false);
            pkg.WriteInt((int) room.RoomType);
            pkg.WriteInt((int) room.GameType);
            pkg.WriteInt(room.GuildId);
            bool flag = room.GameType == eGameType.BattleGame;
            List<GamePlayer> players = room.GetPlayers();
            pkg.WriteInt(players.Count);
            foreach (GamePlayer player in players)
            {
                pkg.WriteInt(player.ZoneId);
                pkg.WriteString(player.ZoneName);
                pkg.WriteInt(player.PlayerCharacter.ID);
                pkg.WriteString(player.PlayerCharacter.NickName);
                pkg.WriteBoolean(player.PlayerCharacter.Sex);
                pkg.WriteByte(player.PlayerCharacter.typeVIP);
                pkg.WriteInt(player.PlayerCharacter.VIPLevel);
                pkg.WriteInt(player.PlayerCharacter.Hide);
                pkg.WriteString(player.PlayerCharacter.Style);
                pkg.WriteString(player.CreateFightFootballStyle());
                pkg.WriteString(player.PlayerCharacter.Colors);
                pkg.WriteString(player.PlayerCharacter.Skin);
                pkg.WriteInt(player.PlayerCharacter.Offer);
                pkg.WriteInt(player.PlayerCharacter.GP);
                pkg.WriteInt(player.PlayerCharacter.Grade);
                pkg.WriteInt(player.PlayerCharacter.Repute);
                pkg.WriteInt(player.PlayerCharacter.ConsortiaID);
                pkg.WriteString(player.PlayerCharacter.ConsortiaName);
                pkg.WriteInt(player.PlayerCharacter.ConsortiaLevel);
                pkg.WriteInt(player.PlayerCharacter.ConsortiaRepute);
                pkg.WriteInt(player.PlayerCharacter.badgeID);
                pkg.WriteString(player.PlayerCharacter.WeaklessGuildProgressStr);
                pkg.WriteString(player.PlayerCharacter.Honor);
                if (flag)
                {
                    pkg.WriteInt(player.BattleData.Attack);
                    pkg.WriteInt(player.BattleData.Defend);
                    pkg.WriteInt(player.BattleData.Agility);
                    pkg.WriteInt(player.BattleData.Lucky);
                    pkg.WriteInt(player.BattleData.Blood);
                }
                else
                {
                    pkg.WriteInt(player.PlayerCharacter.Attack);
                    pkg.WriteInt(player.PlayerCharacter.Defence);
                    pkg.WriteInt(player.PlayerCharacter.Agility);
                    pkg.WriteInt(player.PlayerCharacter.Luck);
                    pkg.WriteInt(player.PlayerCharacter.hp);
                }
                pkg.WriteInt(player.PlayerCharacter.FightPower);
                pkg.WriteBoolean(player.PlayerCharacter.IsMarried);
                if (player.PlayerCharacter.IsMarried)
                {
                    pkg.WriteInt(player.PlayerCharacter.SpouseID);
                    pkg.WriteString(player.PlayerCharacter.SpouseName);
                }
                if (flag)
                {
                    pkg.WriteDouble((double) player.BattleData.Damage);
                    pkg.WriteDouble((double) player.BattleData.Guard);
                    double val = 1.0 - (player.BattleData.Agility * 0.001);
                    pkg.WriteDouble(val);
                    pkg.WriteDouble((double) player.BattleData.Blood);
                }
                else
                {
                    pkg.WriteDouble(player.GetBaseAttack());
                    pkg.WriteDouble(player.GetBaseDefence());
                    pkg.WriteDouble(player.GetBaseAgility());
                    pkg.WriteDouble(player.GetBaseBlood());
                }
                pkg.WriteInt(player.MainWeapon.TemplateID);
                pkg.WriteBoolean(player.CanUseProp);
                if (!((player.SecondWeapon == null) || flag))
                {
                    pkg.WriteInt(player.SecondWeapon.TemplateID);
                    pkg.WriteInt(player.SecondWeapon.StrengthenLevel);
                }
                else
                {
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                }
                if (!((player.Healstone == null) || flag))
                {
                    pkg.WriteInt(player.Healstone.TemplateID);
                    pkg.WriteInt(player.Healstone.Count);
                }
                else
                {
                    pkg.WriteInt(0);
                    pkg.WriteInt(0);
                }
                pkg.WriteDouble((RateMgr.GetRate(eRateType.Experience_Rate) * AntiAddictionMgr.GetAntiAddictionCoefficient(player.PlayerCharacter.AntiAddiction)) * ((player.GPAddPlus == 0.0) ? 1.0 : player.GPAddPlus));
                pkg.WriteDouble(AntiAddictionMgr.GetAntiAddictionCoefficient(player.PlayerCharacter.AntiAddiction) * ((player.OfferAddPlus == 0.0) ? 1.0 : player.OfferAddPlus));
                pkg.WriteDouble((double) RateMgr.GetRate(eRateType.Experience_Rate));
                pkg.WriteInt(GameServer.Instance.Configuration.ServerID);
                pkg.WriteBoolean(player.CanX2Exp);
                pkg.WriteBoolean(player.CanX3Exp);
                if ((player.Pet == null) || flag)
                {
                    pkg.WriteInt(0);
                }
                else
                {
                    pkg.WriteInt(1);
                    pkg.WriteInt(player.Pet.Place);
                    pkg.WriteInt(player.Pet.TemplateID);
                    pkg.WriteInt(player.Pet.ID);
                    pkg.WriteString(player.Pet.Name);
                    pkg.WriteInt(player.Pet.UserID);
                    pkg.WriteInt(player.Pet.Level);
                    pkg.WriteString(player.Pet.Skill);
                    pkg.WriteString(player.Pet.SkillEquip);
                }
                if (flag)
                {
                    pkg.WriteInt(0);
                }
                else
                {
                    List<AbstractBuffer> allBufferByTemplate = player.BufferList.GetAllBufferByTemplate();
                    pkg.WriteInt(allBufferByTemplate.Count);
                    foreach (AbstractBuffer buffer in allBufferByTemplate)
                    {
                        BufferInfo info = buffer.Info;
                        pkg.WriteInt(info.Type);
                        pkg.WriteBoolean(info.IsExist);
                        pkg.WriteDateTime(info.BeginDate);
                        pkg.WriteInt(info.ValidDate);
                        pkg.WriteInt(info.Value);
                        pkg.WriteInt(info.ValidCount);
                        pkg.WriteInt(info.TemplateID);
                    }
                }
                if (flag)
                {
                    pkg.WriteInt(0);
                }
                else
                {
                    pkg.WriteInt(player.EquipEffect.Count);
                    foreach (SqlDataProvider.Data.ItemInfo info2 in player.EquipEffect)
                    {
                        pkg.WriteInt(info2.TemplateID);
                        pkg.WriteInt(info2.Hole1);
                    }
                }
                pkg.WriteInt(player.BattleData.MatchInfo.restCount);
                pkg.WriteInt(player.BattleData.MatchInfo.maxCount);
                pkg.WriteInt(player.FightBuffs.Count);
                foreach (BufferInfo info3 in player.FightBuffs)
                {
                    pkg.WriteInt(info3.Type);
                    pkg.WriteInt(info3.Value);
                }
            }
            this.SendTCP(pkg);
        }

        public void SendChangeGameType(BaseRoom room)
        {
            GSPacketIn pkg = new GSPacketIn(0x48) {
                Parameter1 = room.RoomId
            };
            pkg.WriteInt((int) room.GameType);
            this.SendTCP(pkg);
        }

        public void SendChatMessage(string msg, GamePlayer player, bool team)
        {
            GSPacketIn pkg = new GSPacketIn(0x13, player.CurrentRoom.Game.Id);
            pkg.WriteInt(player.GamePlayerId);
            pkg.WriteBoolean(team);
            pkg.WriteString(msg);
            this.SendTCP(pkg);
        }

        public void SendFightNotice(GamePlayer player, int GameId)
        {
            GSPacketIn pkg = new GSPacketIn(3, GameId) {
                Parameter1 = player.GamePlayerId
            };
            this.SendTCP(pkg);
        }

        public void SendFindConsortiaAlly(int state, int gameid)
        {
            GSPacketIn pkg = new GSPacketIn(0x4d, gameid);
            pkg.WriteInt(state);
            pkg.WriteInt((int) RateMgr.GetRate(eRateType.Riches_Rate));
            this.SendTCP(pkg);
        }

        public void SendKitOffPlayer(int playerid)
        {
            GSPacketIn pkg = new GSPacketIn(4);
            pkg.WriteInt(playerid);
            this.SendTCP(pkg);
        }

        public void SendPlayerDisconnet(int gameId, int playerId, int roomid)
        {
            GSPacketIn pkg = new GSPacketIn(0x53, gameId) {
                Parameter1 = playerId
            };
            this.SendTCP(pkg);
        }

        public void SendRemoveRoom(BaseRoom room)
        {
            GSPacketIn pkg = new GSPacketIn(0x41) {
                Parameter1 = room.RoomId
            };
            this.SendTCP(pkg);
        }

        public void SendRSALogin(RSACryptoServiceProvider rsa, string key)
        {
            GSPacketIn pkg = new GSPacketIn(1);
            pkg.Write(rsa.Encrypt(Encoding.UTF8.GetBytes(key), false));
            this.SendTCP(pkg);
        }

        public void SendToGame(int gameId, GSPacketIn pkg)
        {
            GSPacketIn @in = new GSPacketIn(2, gameId);
            @in.WritePacket(pkg);
            this.SendTCP(@in);
        }

        private void SendUsingPropInGame(int gameId, int playerId, int templateId, bool result)
        {
            GSPacketIn pkg = new GSPacketIn(0x24, gameId) {
                Parameter1 = playerId,
                Parameter2 = templateId
            };
            pkg.WriteBoolean(result);
            this.SendTCP(pkg);
        }
    }
}

