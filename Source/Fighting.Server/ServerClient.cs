namespace Fighting.Server
{
    using Bussiness;
    using Bussiness.Managers;
    using Fighting.Server.GameObjects;
    using Fighting.Server.Games;
    using Fighting.Server.Rooms;
    using Game.Base;
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Logic.Phy.Object;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;

    public class ServerClient : BaseClient
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Dictionary<int, ProxyRoom> m_rooms;
        private RSACryptoServiceProvider m_rsa;
        private FightServer m_svr;

        public ServerClient(FightServer svr) : base(new byte[0x2000], new byte[0x2000])
        {
            this.m_rooms = new Dictionary<int, ProxyRoom>();
            this.m_svr = svr;
        }

        public void HandleConsortiaAlly(GSPacketIn pkg)
        {
            BaseGame game = GameMgr.FindGame(pkg.ClientID);
            if (game != null)
            {
                game.ConsortiaAlly = pkg.ReadInt();
                game.RichesRate = pkg.ReadInt();
            }
        }

        public void HandleGameRoomCancel(GSPacketIn pkg)
        {
            ProxyRoom room = null;
            lock (this.m_rooms)
            {
                if (this.m_rooms.ContainsKey(pkg.Parameter1))
                {
                    room = this.m_rooms[pkg.Parameter1];
                }
            }
            if (room != null)
            {
                ProxyRoomMgr.RemoveRoom(room);
            }
        }

        public void HandleGameRoomCreate(GSPacketIn pkg)
        {
            Dictionary<int, ProxyRoom> dictionary;
            int orientRoomId = pkg.ReadInt();
            int npcId = pkg.ReadInt();
            bool isAutoBot = pkg.ReadBoolean();
            bool flag2 = pkg.ReadBoolean();
            bool flag3 = pkg.ReadBoolean();
            int num3 = pkg.ReadInt();
            int num4 = pkg.ReadInt();
            int num5 = pkg.ReadInt();
            int num6 = pkg.ReadInt();
            int totallevel = 0;
            int totalFightPower = 0;
            int iD = 0;
            IGamePlayer[] players = new IGamePlayer[num6];
            for (int i = 0; i < num6; i++)
            {
                PlayerInfo character = new PlayerInfo();
                ProxyPlayerInfo proxyPlayer = new ProxyPlayerInfo {
                    ZoneId = pkg.ReadInt(),
                    ZoneName = pkg.ReadString()
                };
                character.ID = pkg.ReadInt();
                character.NickName = pkg.ReadString();
                character.Sex = pkg.ReadBoolean();
                character.typeVIP = pkg.ReadByte();
                character.VIPLevel = pkg.ReadInt();
                character.Hide = pkg.ReadInt();
                character.Style = pkg.ReadString();
                proxyPlayer.FightFootballStyle = pkg.ReadString();
                character.Colors = pkg.ReadString();
                character.Skin = pkg.ReadString();
                character.Offer = pkg.ReadInt();
                character.GP = pkg.ReadInt();
                character.Grade = pkg.ReadInt();
                character.Repute = pkg.ReadInt();
                character.ConsortiaID = pkg.ReadInt();
                character.ConsortiaName = pkg.ReadString();
                character.ConsortiaLevel = pkg.ReadInt();
                character.ConsortiaRepute = pkg.ReadInt();
                character.badgeID = pkg.ReadInt();
                character.weaklessGuildProgress = Base64.decodeToByteArray(pkg.ReadString());
                character.Honor = pkg.ReadString();
                character.Attack = pkg.ReadInt();
                character.Defence = pkg.ReadInt();
                character.Agility = pkg.ReadInt();
                character.Luck = pkg.ReadInt();
                character.hp = pkg.ReadInt();
                character.FightPower = pkg.ReadInt();
                character.IsMarried = pkg.ReadBoolean();
                if (character.IsMarried)
                {
                    character.SpouseID = pkg.ReadInt();
                    character.SpouseName = pkg.ReadString();
                }
                character.IsAutoBot = isAutoBot;
                totalFightPower += character.FightPower;
                proxyPlayer.BaseAttack = pkg.ReadDouble();
                proxyPlayer.BaseDefence = pkg.ReadDouble();
                proxyPlayer.BaseAgility = pkg.ReadDouble();
                proxyPlayer.BaseBlood = pkg.ReadDouble();
                proxyPlayer.TemplateId = pkg.ReadInt();
                proxyPlayer.CanUserProp = pkg.ReadBoolean();
                proxyPlayer.SecondWeapon = pkg.ReadInt();
                proxyPlayer.StrengthLevel = pkg.ReadInt();
                proxyPlayer.Healstone = pkg.ReadInt();
                proxyPlayer.HealstoneCount = pkg.ReadInt();
                proxyPlayer.GPAddPlus = pkg.ReadDouble();
                proxyPlayer.OfferAddPlus = pkg.ReadDouble();
                proxyPlayer.AntiAddictionRate = pkg.ReadDouble();
                proxyPlayer.ServerId = pkg.ReadInt();
                proxyPlayer.CanX2Exp = pkg.ReadBoolean();
                proxyPlayer.CanX3Exp = pkg.ReadBoolean();
                UsersPetinfo pet = new UsersPetinfo();
                if (pkg.ReadInt() == 1)
                {
                    pet.Place = pkg.ReadInt();
                    pet.TemplateID = pkg.ReadInt();
                    pet.ID = pkg.ReadInt();
                    pet.Name = pkg.ReadString();
                    pet.UserID = pkg.ReadInt();
                    pet.Level = pkg.ReadInt();
                    pet.Skill = pkg.ReadString();
                    pet.SkillEquip = pkg.ReadString();
                }
                else
                {
                    pet = null;
                }
                List<BufferInfo> infos = new List<BufferInfo>();
                int num12 = pkg.ReadInt();
                for (int j = 0; j < num12; j++)
                {
                    BufferInfo item = new BufferInfo {
                        Type = pkg.ReadInt(),
                        IsExist = pkg.ReadBoolean(),
                        BeginDate = pkg.ReadDateTime(),
                        ValidDate = pkg.ReadInt(),
                        Value = pkg.ReadInt(),
                        ValidCount = pkg.ReadInt(),
                        TemplateID = pkg.ReadInt()
                    };
                    if (character != null)
                    {
                        infos.Add(item);
                    }
                }
                List<SqlDataProvider.Data.ItemInfo> euipEffects = new List<SqlDataProvider.Data.ItemInfo>();
                int num14 = pkg.ReadInt();
                for (int k = 0; k < num14; k++)
                {
                    int templateId = pkg.ReadInt();
                    int num17 = pkg.ReadInt();
                    SqlDataProvider.Data.ItemInfo info4 = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(templateId), 1, 1);
                    info4.Hole1 = num17;
                    euipEffects.Add(info4);
                }
                UserMatchInfo matchInfo = new UserMatchInfo {
                    restCount = pkg.ReadInt(),
                    maxCount = pkg.ReadInt()
                };
                List<BufferInfo> fightBuffs = new List<BufferInfo>();
                int num18 = pkg.ReadInt();
                for (int m = 0; m < num18; m++)
                {
                    BufferInfo info6 = new BufferInfo {
                        Type = pkg.ReadInt(),
                        Value = pkg.ReadInt()
                    };
                    if (character != null)
                    {
                        fightBuffs.Add(info6);
                    }
                }
                totallevel += character.Grade;
                iD = character.ID;
                players[i] = new ProxyPlayer(this, character, matchInfo, proxyPlayer, pet, infos, euipEffects, fightBuffs);
                players[i].CanUseProp = proxyPlayer.CanUserProp;
                players[i].CanX2Exp = proxyPlayer.CanX2Exp;
                players[i].CanX3Exp = proxyPlayer.CanX3Exp;
            }
            ProxyRoom room = new ProxyRoom(ProxyRoomMgr.NextRoomId(), orientRoomId, players, this, totallevel, totalFightPower, npcId, isAutoBot) {
                GuildId = num5,
                selfId = iD,
                startWithNpc = flag2,
                IsFreedom = flag3,
                RoomType = (eRoomType) num3,
                GameType = (eGameType) num4
            };
            foreach (IGamePlayer player in room.GetPlayers())
            {
                player.PlayerCharacter.RoomId = room.RoomId;
            }
            ProxyRoom room2 = null;
            lock ((dictionary = this.m_rooms))
            {
                if (this.m_rooms.ContainsKey(orientRoomId))
                {
                    room2 = this.m_rooms[orientRoomId];
                    this.m_rooms.Remove(orientRoomId);
                }
            }
            if (room2 != null)
            {
                ProxyRoomMgr.RemoveRoom(room2);
            }
            lock ((dictionary = this.m_rooms))
            {
                if (!this.m_rooms.ContainsKey(orientRoomId))
                {
                    this.m_rooms.Add(orientRoomId, room);
                    this.SendFightRoomID(orientRoomId, room.RoomId);
                }
                else
                {
                    log.Error(string.Concat(new object[] { "Room exists: RoomType: ", this.m_rooms[orientRoomId].RoomType, " - isAutoBot: ", this.m_rooms[orientRoomId].isAutoBot, " - PickUpCount: ", this.m_rooms[orientRoomId].PickUpCount, " - IsPlaying: ", this.m_rooms[orientRoomId].IsPlaying, " - CountPlayer: ", this.m_rooms[orientRoomId].PlayerCount }));
                    log.Error(string.Concat(new object[] { "Fucking Room: RoomType: ", room.RoomType, " - isAutoBot: ", room.isAutoBot, " - PickUpCount: ", room.PickUpCount, " - CountPlayer: ", room.PlayerCount }));
                    room = null;
                }
            }
            if (room != null)
            {
                ProxyRoomMgr.AddRoom(room);
            }
            else
            {
                log.WarnFormat("Room already exists:{0}", orientRoomId);
            }
        }

        public void HandleLogin(GSPacketIn pkg)
        {
            byte[] rgb = pkg.ReadBytes();
            string[] strArray = Encoding.UTF8.GetString(this.m_rsa.Decrypt(rgb, false)).Split(new char[] { ',' });
            if (strArray.Length == 2)
            {
                this.m_rsa = null;
                int.Parse(strArray[0]);
                base.Strict = false;
            }
            else
            {
                log.ErrorFormat("Error Login Packet from {0}", base.TcpEndpoint);
                this.Disconnect();
            }
        }

        private void HandlePlayerExit(GSPacketIn pkg)
        {
            BaseGame game = GameMgr.FindGame(pkg.ClientID);
            if (game != null)
            {
                Player player = game.FindPlayer(pkg.Parameter1);
                if (player != null)
                {
                    GSPacketIn @in = new GSPacketIn(0x53, player.PlayerDetail.PlayerCharacter.ID);
                    game.SendToAll(@in);
                    game.RemovePlayer(player.PlayerDetail, false);
                    ProxyRoom roomUnsafe = ProxyRoomMgr.GetRoomUnsafe((game as BattleGame).Red.RoomId);
                    if ((roomUnsafe != null) && !roomUnsafe.RemovePlayer(player.PlayerDetail))
                    {
                        ProxyRoom room2 = ProxyRoomMgr.GetRoomUnsafe((game as BattleGame).Blue.RoomId);
                        if (room2 != null)
                        {
                            room2.RemovePlayer(player.PlayerDetail);
                        }
                    }
                }
            }
        }

        private void HandlePlayerMessage(GSPacketIn pkg)
        {
            BaseGame game = GameMgr.FindGame(pkg.ClientID);
            if (game != null)
            {
                Player player = game.FindPlayer(pkg.ReadInt());
                bool val = pkg.ReadBoolean();
                string str = pkg.ReadString();
                if (player != null)
                {
                    GSPacketIn @in = new GSPacketIn(0x13) {
                        ClientID = player.PlayerDetail.PlayerCharacter.ID
                    };
                    @in.WriteInt(4);
                    @in.WriteByte(5);
                    @in.WriteBoolean(val);
                    @in.WriteString(player.PlayerDetail.PlayerCharacter.NickName);
                    @in.WriteString(str);
                    if (val)
                    {
                        game.SendToTeam(pkg, player.Team);
                    }
                    else
                    {
                        game.SendToAll(@in);
                    }
                }
            }
        }

        private void HandlePlayerUsingProp(GSPacketIn pkg)
        {
            BaseGame game = GameMgr.FindGame(pkg.ClientID);
            if (game != null)
            {
                game.Resume();
                if (pkg.ReadBoolean())
                {
                    Player player = game.FindPlayer(pkg.Parameter1);
                    ItemTemplateInfo item = ItemMgr.FindItemTemplate(pkg.Parameter2);
                    if ((player != null) && (item != null))
                    {
                        player.UseItem(item);
                    }
                }
            }
        }

        private void HandleSysNotice(GSPacketIn pkg)
        {
            BaseGame game = GameMgr.FindGame(pkg.ClientID);
            if (game != null)
            {
                Player player = game.FindPlayer(pkg.Parameter1);
                GSPacketIn @in = new GSPacketIn(3);
                @in.WriteInt(3);
                @in.WriteString(LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", new object[] { player.PlayerDetail.PlayerCharacter.Grade * 12, 15 }));
                player.PlayerDetail.SendTCP(@in);
                @in.ClearContext();
                @in.WriteInt(3);
                @in.WriteString(LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", new object[] { player.PlayerDetail.PlayerCharacter.NickName, player.PlayerDetail.PlayerCharacter.Grade * 12, 15 }));
                game.SendToAll(@in, player.PlayerDetail);
            }
        }

        public void HanleSendToGame(GSPacketIn pkg)
        {
            BaseGame game = GameMgr.FindGame(pkg.ClientID);
            if (game != null)
            {
                GSPacketIn @in = pkg.ReadPacket();
                game.ProcessData(@in);
            }
        }

        protected override void OnConnect()
        {
            base.OnConnect();
            this.m_rsa = new RSACryptoServiceProvider();
            RSAParameters parameters = this.m_rsa.ExportParameters(false);
            this.SendRSAKey(parameters.Modulus, parameters.Exponent);
        }

        protected override void OnDisconnect()
        {
            base.OnDisconnect();
            this.m_rsa = null;
        }

        public override void OnRecvPacket(GSPacketIn pkg)
        {
            short code = pkg.Code;
            if (code <= 0x24)
            {
                switch (code)
                {
                    case 1:
                        this.HandleLogin(pkg);
                        return;

                    case 2:
                        this.HanleSendToGame(pkg);
                        return;

                    case 3:
                        this.HandleSysNotice(pkg);
                        return;

                    case 0x13:
                        this.HandlePlayerMessage(pkg);
                        return;
                }
                if (code == 0x24)
                {
                    this.HandlePlayerUsingProp(pkg);
                }
            }
            else
            {
                switch (code)
                {
                    case 0x40:
                        this.HandleGameRoomCreate(pkg);
                        return;

                    case 0x41:
                        this.HandleGameRoomCancel(pkg);
                        return;

                    case 0x4d:
                        this.HandleConsortiaAlly(pkg);
                        return;
                }
                if (code == 0x53)
                {
                    this.HandlePlayerExit(pkg);
                }
            }
        }

        public void RemoveRoom(int orientId, ProxyRoom room)
        {
            bool flag = false;
            lock (this.m_rooms)
            {
                if (this.m_rooms.ContainsKey(orientId) && (this.m_rooms[orientId] == room))
                {
                    flag = this.m_rooms.Remove(orientId);
                }
            }
            if (flag)
            {
                this.SendRemoveRoom(orientId);
            }
        }

        public void SendBeginFightNpc(int playerId, int RoomType, int GameType, int OrientRoomId)
        {
            GSPacketIn pkg = new GSPacketIn(0x58) {
                Parameter1 = playerId
            };
            pkg.WriteInt(RoomType);
            pkg.WriteInt(GameType);
            pkg.WriteInt(OrientRoomId);
            this.SendTCP(pkg);
        }

        public void SendConsortiaAlly(int Consortia1, int Consortia2, int GameId)
        {
            GSPacketIn pkg = new GSPacketIn(0x4d);
            pkg.WriteInt(Consortia1);
            pkg.WriteInt(Consortia2);
            pkg.WriteInt(GameId);
            this.SendTCP(pkg);
        }

        public void SendDisconnectPlayer(int playerId)
        {
            GSPacketIn pkg = new GSPacketIn(0x22, playerId);
            this.SendTCP(pkg);
        }

        public void SendFightRoomID(int roomId, int fightRoomId)
        {
            GSPacketIn pkg = new GSPacketIn(0x45, roomId);
            pkg.WriteInt(fightRoomId);
            this.SendTCP(pkg);
        }

        public void SendFootballTakeOut(int playerId, bool isWin)
        {
            GSPacketIn pkg = new GSPacketIn(0x57, playerId);
            pkg.WriteBoolean(isWin);
            this.SendTCP(pkg);
        }

        public void SendGamePlayerId(IGamePlayer player)
        {
            GSPacketIn pkg = new GSPacketIn(0x21) {
                Parameter1 = player.PlayerCharacter.ID,
                Parameter2 = player.GamePlayerId
            };
            this.SendTCP(pkg);
        }

        public void SendPacketToPlayer(int playerId, GSPacketIn pkg)
        {
            GSPacketIn @in = new GSPacketIn(0x20, playerId);
            @in.WritePacket(pkg);
            this.SendTCP(@in);
        }

        public void SendPlayerAddActiveMoney(int playerId, int value)
        {
            GSPacketIn pkg = new GSPacketIn(0x59, playerId) {
                Parameter1 = value
            };
            this.SendTCP(pkg);
        }

        public void SendPlayerAddGold(int playerId, int value)
        {
            GSPacketIn pkg = new GSPacketIn(0x26, playerId) {
                Parameter1 = value
            };
            this.SendTCP(pkg);
        }

        public void SendPlayerAddGoXu(int playerId, int value)
        {
            GSPacketIn pkg = new GSPacketIn(100, playerId) {
                Parameter1 = value
            };
            this.SendTCP(pkg);
        }

        public void SendPlayerAddGP(int playerId, int value)
        {
            GSPacketIn pkg = new GSPacketIn(0x27, playerId) {
                Parameter1 = value
            };
            this.SendTCP(pkg);
        }

        public void SendPlayerAddGiftToken(int playerId, int value)
        {
            GSPacketIn pkg = new GSPacketIn(0x4b, playerId) {
                Parameter1 = value
            };
            this.SendTCP(pkg);
        }

        public void SendPlayerAddLeagueMoney(int playerId, int value)
        {
            GSPacketIn pkg = new GSPacketIn(0x54, playerId) {
                Parameter1 = value
            };
            this.SendTCP(pkg);
        }

        public void SendPlayerAddMedal(int playerId, int value)
        {
            GSPacketIn pkg = new GSPacketIn(0x4c, playerId) {
                Parameter1 = value
            };
            this.SendTCP(pkg);
        }

        public void SendPlayerAddMoney(int playerId, int value)
        {
            GSPacketIn pkg = new GSPacketIn(0x4a, playerId) {
                Parameter1 = value
            };
            this.SendTCP(pkg);
        }

        public void SendPlayerAddPrestige(int playerId, bool isWin)
        {
            GSPacketIn pkg = new GSPacketIn(0x55, playerId);
            pkg.WriteBoolean(isWin);
            this.SendTCP(pkg);
        }

        public void SendPlayerAddTemplate(int playerId, SqlDataProvider.Data.ItemInfo cloneItem, eBageType bagType, int count)
        {
            if (cloneItem != null)
            {
                GSPacketIn pkg = new GSPacketIn(0x30, playerId);
                pkg.WriteInt(cloneItem.TemplateID);
                pkg.WriteByte((byte) bagType);
                pkg.WriteInt(count);
                pkg.WriteInt(cloneItem.ValidDate);
                pkg.WriteBoolean(cloneItem.IsBinds);
                pkg.WriteBoolean(cloneItem.IsUsed);
                this.SendTCP(pkg);
            }
        }

        public void SendPlayerConsortiaFight(int playerId, int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth)
        {
            GSPacketIn pkg = new GSPacketIn(0x2a, playerId);
            pkg.WriteInt(consortiaWin);
            pkg.WriteInt(consortiaLose);
            pkg.WriteInt(players.Count);
            for (int i = 0; i < players.Count; i++)
            {
                pkg.WriteInt(players[i].PlayerDetail.PlayerCharacter.ID);
            }
            pkg.WriteByte((byte) roomType);
            pkg.WriteByte((byte) gameClass);
            pkg.WriteInt(totalKillHealth);
            this.SendTCP(pkg);
        }

        public void SendPlayerOnGameOver(int playerId, int gameId, bool isWin, int gainXp)
        {
            GSPacketIn pkg = new GSPacketIn(0x23, playerId) {
                Parameter1 = gameId
            };
            pkg.WriteBoolean(isWin);
            pkg.WriteInt(gainXp);
            this.SendTCP(pkg);
        }

        public void SendPlayerOnKillingLiving(int playerId, AbstractGame game, int type, int id, bool isLiving, int demage)
        {
            GSPacketIn pkg = new GSPacketIn(40, playerId);
            pkg.WriteInt(type);
            pkg.WriteBoolean(isLiving);
            pkg.WriteInt(demage);
            this.SendTCP(pkg);
        }

        public void SendPlayerOnMissionOver(int playerId, AbstractGame game, bool isWin, int MissionID, int turnNum)
        {
            GSPacketIn pkg = new GSPacketIn(0x29, playerId);
            pkg.WriteBoolean(isWin);
            pkg.WriteInt(MissionID);
            pkg.WriteInt(turnNum);
            this.SendTCP(pkg);
        }

        public void SendPlayerRemoveGold(int playerId, int value)
        {
            GSPacketIn pkg = new GSPacketIn(0x2c, playerId);
            pkg.WriteInt(value);
            this.SendTCP(pkg);
        }

        public void SendPlayerRemoveGP(int playerId, int value)
        {
            GSPacketIn pkg = new GSPacketIn(0x31, playerId) {
                Parameter1 = value
            };
            this.SendTCP(pkg);
        }

        public void SendPlayerRemoveHealstone(int playerId)
        {
            GSPacketIn pkg = new GSPacketIn(0x49, playerId);
            this.SendTCP(pkg);
        }

        public void SendPlayerRemoveMoney(int playerId, int value)
        {
            GSPacketIn pkg = new GSPacketIn(0x2d, playerId);
            pkg.WriteInt(value);
            this.SendTCP(pkg);
        }

        public void SendPlayerRemoveOffer(int playerId, int value)
        {
            GSPacketIn pkg = new GSPacketIn(50, playerId);
            pkg.WriteInt(value);
            this.SendTCP(pkg);
        }

        public void SendPlayerSendConsortiaFight(int playerId, int consortiaID, int riches, string msg)
        {
            GSPacketIn pkg = new GSPacketIn(0x2b, playerId);
            pkg.WriteInt(consortiaID);
            pkg.WriteInt(riches);
            pkg.WriteString(msg);
            this.SendTCP(pkg);
        }

        public void SendPlayerUsePropInGame(int playerId, int bag, int place, int templateId, bool isLiving)
        {
            GSPacketIn pkg = new GSPacketIn(0x24, playerId) {
                Parameter1 = bag,
                Parameter2 = place
            };
            pkg.WriteInt(templateId);
            pkg.WriteBoolean(isLiving);
            this.SendTCP(pkg);
        }

        public void SendRemoveRoom(int roomId)
        {
            GSPacketIn pkg = new GSPacketIn(0x41, roomId);
            this.SendTCP(pkg);
        }

        public void SendRSAKey(byte[] m, byte[] e)
        {
            GSPacketIn pkg = new GSPacketIn(0);
            pkg.Write(m);
            pkg.Write(e);
            this.SendTCP(pkg);
        }

        public void SendStartGame(int roomId, AbstractGame game)
        {
            GSPacketIn pkg = new GSPacketIn(0x42) {
                Parameter1 = roomId,
                Parameter2 = game.Id
            };
            pkg.WriteInt((int) game.RoomType);
            pkg.WriteInt((int) game.GameType);
            pkg.WriteInt(game.TimeType);
            this.SendTCP(pkg);
        }

        public void SendStopGame(int oldRoomId, int gameId, int roomId)
        {
            GSPacketIn pkg = new GSPacketIn(0x44) {
                Parameter1 = oldRoomId,
                Parameter2 = gameId
            };
            pkg.WriteInt(roomId);
            this.SendTCP(pkg);
        }

        public void SendToRoom(int roomId, GSPacketIn pkg, IGamePlayer except)
        {
            GSPacketIn @in = new GSPacketIn(0x43, roomId);
            if (except != null)
            {
                @in.Parameter1 = except.PlayerCharacter.ID;
                @in.Parameter2 = except.GamePlayerId;
            }
            else
            {
                @in.Parameter1 = 0;
                @in.Parameter2 = 0;
            }
            @in.WritePacket(pkg);
            this.SendTCP(@in);
        }

        public void SendUpdateRestCount(int playerId)
        {
            GSPacketIn pkg = new GSPacketIn(0x56, playerId);
            this.SendTCP(pkg);
        }

        public override string ToString()
        {
            return string.Format("Server Client: {0} IsConnected:{1}  RoomCount:{2}", 0, base.IsConnected, this.m_rooms.Count);
        }
    }
}

