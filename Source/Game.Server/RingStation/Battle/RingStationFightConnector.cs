namespace Game.Server.RingStation.Battle
{
    using Game.Base;
    using Game.Base.Packets;
    using Game.Logic;
    using Game.Server.RingStation;
    using log4net;
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Reflection;

    public class RingStationFightConnector : BaseConnector
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private string m_key;
        private RingStationBattleServer m_server;

        public RingStationFightConnector(RingStationBattleServer server, string ip, int port, string key) : base(ip, port, true, new byte[0x2000], new byte[0x2000])
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
                switch (pkg.Code)
                {
                    case 0x13:
                        this.HandlePlayerChatSend(pkg);
                        return;

                    case 20:
                    case 0x15:
                    case 0x16:
                    case 0x17:
                    case 0x18:
                    case 0x19:
                    case 0x1a:
                    case 0x1b:
                    case 0x1c:
                    case 0x1d:
                    case 30:
                    case 0x1f:
                    case 0x25:
                    case 0x2e:
                    case 0x2f:
                    case 0x33:
                    case 0x36:
                    case 0x37:
                    case 0x38:
                    case 0x39:
                    case 0x3a:
                    case 0x3b:
                    case 60:
                    case 0x3d:
                    case 0x3e:
                    case 0x3f:
                    case 0x40:
                    case 0x47:
                    case 0x48:
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

                    case 0x34:
                        this.HandPlayerAddRobRiches(pkg);
                        return;

                    case 0x35:
                        this.HandleClearBag(pkg);
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
                        return;

                    case 70:
                        this.HandlePlayerRemove(pkg);
                        return;

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

                    case 0:
                        this.HandleRSAKey(pkg);
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
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void HandleClearBag(GSPacketIn pkg)
        {
        }

        private void HandleDisconnectPlayer(GSPacketIn pkg)
        {
        }

        public void HandleFindConsortiaAlly(GSPacketIn pkg)
        {
        }

        private void HandlePlayerAddGold(GSPacketIn pkg)
        {
        }

        private void HandlePlayerAddGP(GSPacketIn pkg)
        {
        }

        private void HandlePlayerAddGiftToken(GSPacketIn pkg)
        {
        }

        private void HandlePlayerAddLeagueMoney(GSPacketIn pkg)
        {
        }

        private void HandlePlayerAddMedal(GSPacketIn pkg)
        {
        }

        private void HandlePlayerAddMoney(GSPacketIn pkg)
        {
        }

        private void HandlePlayerAddPrestige(GSPacketIn pkg)
        {
        }

        private void HandlePlayerAddTemplate1(GSPacketIn pkg)
        {
        }

        private void HandlePlayerConsortiaFight(GSPacketIn pkg)
        {
        }

        private void HandlePlayerChatSend(GSPacketIn pkg)
        {
        }

        private void HandlePlayerHealstone(GSPacketIn pkg)
        {
        }

        private void HandlePlayerOnGameOver(GSPacketIn pkg)
        {
        }

        private void HandlePlayerOnKillingLiving(GSPacketIn pkg)
        {
        }

        private void HandlePlayerOnMissionOver(GSPacketIn pkg)
        {
        }

        private void HandlePlayerOnUsingItem(GSPacketIn pkg)
        {
        }

        private void HandlePlayerRemove(GSPacketIn pkg)
        {
        }

        private void HandlePlayerRemoveGold(GSPacketIn pkg)
        {
        }

        private void HandlePlayerRemoveGP(GSPacketIn pkg)
        {
        }

        private void HandlePlayerRemoveMoney(GSPacketIn pkg)
        {
        }

        private void HandlePlayerRemoveOffer(GSPacketIn pkg)
        {
        }

        private void HandlePlayerSendConsortiaFight(GSPacketIn pkg)
        {
        }

        private void HandlePlayerUpdateRestCount(GSPacketIn pkg)
        {
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
        }

        protected void HandleSendToRoom(GSPacketIn pkg)
        {
            int clientID = pkg.ClientID;
            GSPacketIn @in = pkg.ReadPacket();
            this.m_server.SendToRoom(clientID, @in, pkg.Parameter1, pkg.Parameter2);
        }

        protected void HandleStartGame(GSPacketIn pkg)
        {
            ProxyRingStationGame game = new ProxyRingStationGame(pkg.Parameter2, this, (eRoomType) pkg.ReadInt(), (eGameType) pkg.ReadInt(), pkg.ReadInt());
            this.m_server.StartGame(pkg.Parameter1, game);
        }

        protected void HandleStopGame(GSPacketIn pkg)
        {
            int roomId = pkg.Parameter1;
            int gameId = pkg.Parameter2;
            this.m_server.StopGame(roomId, gameId);
        }

        private void HandleUpdatePlayerGameId(GSPacketIn pkg)
        {
            this.m_server.UpdatePlayerGameId(pkg.Parameter1, pkg.Parameter2);
        }

        private void HandleUpdateRoomId(GSPacketIn pkg)
        {
        }

        private void HandPlayerAddRobRiches(GSPacketIn pkg)
        {
        }

        protected override void OnDisconnect()
        {
            base.OnDisconnect();
        }

        public override void OnRecvPacket(GSPacketIn pkg)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.AsynProcessPacket), pkg);
        }

        public void SendAddRoom(BaseRoomRingStation room)
        {
            GSPacketIn pkg = new GSPacketIn(0x40);
            pkg.WriteInt(room.RoomId);
            pkg.WriteInt(room.PickUpNpcId);
            pkg.WriteBoolean(room.IsAutoBot);
            pkg.WriteBoolean(false);
            pkg.WriteBoolean(room.IsFreedom);
            pkg.WriteInt(room.RoomType);
            pkg.WriteInt(room.GameType);
            pkg.WriteInt(room.GuildId);
            List<RingStationGamePlayer> players = room.GetPlayers();
            pkg.WriteInt(players.Count);
            foreach (RingStationGamePlayer player in players)
            {
                pkg.WriteInt(RingStationConfiguration.ServerID);
                pkg.WriteString(RingStationConfiguration.ServerName);
                pkg.WriteInt(player.ID);
                pkg.WriteString(player.NickName);
                pkg.WriteBoolean(player.Sex);
                pkg.WriteByte(player.typeVIP);
                pkg.WriteInt(player.VIPLevel);
                pkg.WriteInt(player.Hide);
                pkg.WriteString(player.Style);
                pkg.WriteString(player.Style);
                pkg.WriteString(player.Colors);
                pkg.WriteString(player.Skin);
                pkg.WriteInt(player.Offer);
                pkg.WriteInt(player.GP);
                pkg.WriteInt(player.Grade);
                pkg.WriteInt(player.Repute);
                pkg.WriteInt(player.ConsortiaID);
                pkg.WriteString(player.ConsortiaName);
                pkg.WriteInt(player.ConsortiaLevel);
                pkg.WriteInt(player.ConsortiaRepute);
                pkg.WriteInt(player.badgeID);
                pkg.WriteString(player.WeaklessGuildProgressStr);
                pkg.WriteString(player.Honor);
                pkg.WriteInt(player.Attack);
                pkg.WriteInt(player.Defence);
                pkg.WriteInt(player.Agility);
                pkg.WriteInt(player.Luck);
                pkg.WriteInt(player.hp);
                pkg.WriteInt(player.FightPower);
                pkg.WriteBoolean(false);
                pkg.WriteDouble(player.BaseAttack);
                pkg.WriteDouble(player.BaseDefence);
                pkg.WriteDouble(player.BaseAgility);
                pkg.WriteDouble(player.BaseBlood);
                pkg.WriteInt(player.TemplateID);
                pkg.WriteBoolean(player.CanUserProp);
                pkg.WriteInt(player.SecondWeapon);
                pkg.WriteInt(player.StrengthLevel);
                pkg.WriteInt(player.Healstone);
                pkg.WriteInt(player.HealstoneCount);
                pkg.WriteDouble(player.GPAddPlus);
                pkg.WriteDouble((double) player.GMExperienceRate);
                pkg.WriteDouble((double) player.AuncherExperienceRate);
                pkg.WriteInt(RingStationConfiguration.ServerID);
                pkg.WriteBoolean(false);
                pkg.WriteBoolean(false);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
            }
            this.SendTCP(pkg);
        }

        public void SendChangeGameType()
        {
        }

        public void SendChatMessage()
        {
        }

        public void SendFightNotice()
        {
        }

        public void SendFindConsortiaAlly(int state, int gameid)
        {
        }

        public void SendKitOffPlayer(int playerid)
        {
        }

        public void SendPlayerDisconnet(int gameId, int playerId, int roomid)
        {
        }

        public void SendRemoveRoom(BaseRoomRingStation room)
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
        }
    }
}

