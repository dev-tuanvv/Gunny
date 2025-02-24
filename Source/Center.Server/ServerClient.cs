namespace Center.Server
{
    using Bussiness;
    using Bussiness.Protocol;
    using Center.Server.Managers;
    using Game.Base;
    using Game.Base.Packets;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Text;
    using System.Reflection;

    public class ServerClient : BaseClient
    {
        private RSACryptoServiceProvider _rsa;
        private CenterServer _svr;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public bool NeedSyncMacroDrop;

        public ServerClient(CenterServer svr) : base(new byte[0x2000], new byte[0x2000])
        {
            this._svr = svr;
        }

        public void HandkeItemStrengthen(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }

        public void HandleBigBugle(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }

        public void HandleBuyBadge(GSPacketIn pkg)
        {
            pkg.ReadInt();
            this._svr.SendToALL(pkg, null);
        }

        public void HandleConsortiaCreate(GSPacketIn pkg)
        {
            pkg.ReadInt();
            pkg.ReadInt();
            this._svr.SendToALL(pkg, null);
        }

        public void HandleConsortiaFight(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg);
        }

        public void HandleConsortiaOffer(GSPacketIn pkg)
        {
            pkg.ReadInt();
            pkg.ReadInt();
            pkg.ReadInt();
        }

        public void HandleConsortiaResponse(GSPacketIn pkg)
        {
            int num = pkg.ReadByte();
            this._svr.SendToALL(pkg, null);
        }

        public void HandleConsortiaUpGrade(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }

        public void HandleChatConsortia(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }

        public void HandleChatPersonal(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg);
        }

        public void HandleChatScene(GSPacketIn pkg)
        {
            if (pkg.ReadByte() == 3)
            {
                this.HandleChatConsortia(pkg);
            }
        }

        public void HandleEventRank(GSPacketIn pkg)
        {
            GSPacketIn @in = new GSPacketIn(90);
            switch (pkg.ReadByte())
            {
                case 1:
                {
                    RankingLightriddleInfo info = null;
                    string str = pkg.ReadString();
                    int val = pkg.ReadInt();
                    @in.WriteByte(1);
                    List<RankingLightriddleInfo> list = WorldMgr.SelectTopEight();
                    @in.WriteInt(list.Count);
                    foreach (RankingLightriddleInfo info2 in list)
                    {
                        @in.WriteInt(info2.Rank);
                        @in.WriteString(info2.NickName);
                        @in.WriteByte((byte) info2.TypeVIP);
                        @in.WriteInt(info2.Integer);
                        if (str == info2.NickName)
                        {
                            info = info2;
                        }
                    }
                    if (info == null)
                    {
                        @in.WriteInt(0);
                    }
                    else
                    {
                        @in.WriteInt(info.Rank);
                    }
                    @in.WriteInt(val);
                    this._svr.SendToALL(@in);
                    break;
                }
                case 2:
                {
                    string nickName = pkg.ReadString();
                    int typeVip = pkg.ReadInt();
                    int integer = pkg.ReadInt();
                    int playerId = pkg.ReadInt();
                    WorldMgr.UpdateLightriddleRank(integer, typeVip, nickName, playerId);
                    break;
                }
                case 4:
                {
                    int playerID = pkg.ReadInt();
                    string str3 = pkg.ReadString();
                    int templateID = pkg.ReadInt();
                    int count = pkg.ReadInt();
                    int isVip = pkg.ReadInt();
                    WorldMgr.UpdateLuckStarRewardRecord(playerID, str3, templateID, count, isVip);
                    break;
                }
            }
        }

        public void HandleFirendResponse(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }

        public void HandleFriend(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }

        public void HandleFriendState(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }

        public void HandleIPAndPort(GSPacketIn pkg)
        {
        }

        public void HandleLogin(GSPacketIn pkg)
        {
            byte[] rgb = pkg.ReadBytes();
            string[] strArray = Encoding.UTF8.GetString(this._rsa.Decrypt(rgb, false)).Split(new char[] { ',' });
            if (strArray.Length != 2)
            {
                log.ErrorFormat("Error Login Packet from {0}", base.TcpEndpoint);
                this.Disconnect();
            }
            else
            {
                this._rsa = null;
                int id = int.Parse(strArray[0]);
                this.Info = ServerMgr.GetServerInfo(id);
                if ((this.Info == null) || (this.Info.State != 1))
                {
                    log.ErrorFormat("Error Login Packet from {0} want to login serverid:{1}", base.TcpEndpoint, id);
                    this.Disconnect();
                }
                else
                {
                    base.Strict = false;
                    CenterServer.Instance.SendConfigState();
                    CenterServer.Instance.SendUpdateWorldEvent();
                    this.Info.Online = 0;
                    this.Info.State = 2;
                }
            }
        }

        public void HandleMacroDrop(GSPacketIn pkg)
        {
            Dictionary<int, int> temp = new Dictionary<int, int>();
            int num = pkg.ReadInt();
            for (int i = 0; i < num; i++)
            {
                int key = pkg.ReadInt();
                int num4 = pkg.ReadInt();
                temp.Add(key, num4);
            }
            MacroDropMgr.DropNotice(temp);
            this.NeedSyncMacroDrop = true;
        }

        public void HandleMailResponse(GSPacketIn pkg)
        {
            int playerid = pkg.ReadInt();
            this.HandleUserPrivateMsg(pkg, playerid);
        }

        public void HandleMarryRoomInfoToPlayer(GSPacketIn pkg)
        {
            Player player = LoginMgr.GetPlayer(pkg.ReadInt());
            if ((player != null) && (player.CurrentServer != null))
            {
                player.CurrentServer.SendTCP(pkg);
            }
        }

        public void HandlePing(GSPacketIn pkg)
        {
            this.Info.Online = pkg.ReadInt();
            this.Info.State = ServerMgr.GetState(this.Info.Online, this.Info.Total);
        }

        public void HandleQuestUserState(GSPacketIn pkg)
        {
            int playerId = pkg.ReadInt();
            if (LoginMgr.GetServerClient(playerId) == null)
            {
                this.SendUserState(playerId, false);
            }
            else
            {
                this.SendUserState(playerId, true);
            }
        }

        public void HandleRecvConsortiaBossAdd(GSPacketIn pkg)
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
                LastOpenBoss = pkg.ReadDateTime()
            };
            if (!ConsortiaBossMgr.AddConsortia(consortia.ConsortiaID, consortia))
            {
                consortia = ConsortiaBossMgr.GetConsortiaById(consortia.ConsortiaID);
            }
            this.HandleSendConsortiaBossInfo(consortia, 180);
        }

        public void HandleRecvConsortiaBossCreate(GSPacketIn pkg)
        {
            int consortiaId = pkg.ReadInt();
            byte bossState = pkg.ReadByte();
            DateTime endTime = pkg.ReadDateTime();
            DateTime lastOpenBoss = pkg.ReadDateTime();
            long maxBlood = pkg.ReadInt();
            if (ConsortiaBossMgr.UpdateConsortia(consortiaId, bossState, endTime, lastOpenBoss, maxBlood))
            {
                ConsortiaInfo consortiaById = ConsortiaBossMgr.GetConsortiaById(consortiaId);
                if (consortiaById != null)
                {
                    this.HandleSendConsortiaBossInfo(consortiaById, 0xb7);
                }
            }
        }

        public void HandleRecvConsortiaBossExtendAvailable(GSPacketIn pkg)
        {
            int consortiaId = pkg.ReadInt();
            int riches = pkg.ReadInt();
            if (ConsortiaBossMgr.ExtendAvailable(consortiaId, riches))
            {
                ConsortiaInfo consortiaById = ConsortiaBossMgr.GetConsortiaById(consortiaId);
                if (consortiaById != null)
                {
                    this.HandleSendConsortiaBossInfo(consortiaById, 0xb6);
                }
            }
            else
            {
                ConsortiaInfo consortia = ConsortiaBossMgr.GetConsortiaById(consortiaId);
                if (consortia != null)
                {
                    this.HandleSendConsortiaBossInfo(consortia, 0xb8);
                }
            }
        }

        public void HandleRecvConsortiaBossReload(GSPacketIn pkg)
        {
            ConsortiaInfo consortiaById = ConsortiaBossMgr.GetConsortiaById(pkg.ReadInt());
            if (consortiaById != null)
            {
                if ((consortiaById.bossState == 2) && consortiaById.SendToClient)
                {
                    if (consortiaById.IsBossDie)
                    {
                        this.HandleSendConsortiaBossInfo(consortiaById, 0xbc);
                    }
                    else
                    {
                        this.HandleSendConsortiaBossInfo(consortiaById, 0xbb);
                    }
                    ConsortiaBossMgr.UpdateSendToClient(consortiaById.ConsortiaID);
                }
                else
                {
                    this.HandleSendConsortiaBossInfo(consortiaById, 0xb8);
                }
            }
        }

        public void HandleRecvConsortiaBossUpdateBlood(GSPacketIn pkg)
        {
            int consortiaId = pkg.ReadInt();
            int damage = pkg.ReadInt();
            ConsortiaBossMgr.UpdateBlood(consortiaId, damage);
        }

        public void HandleRecvConsortiaBossUpdateRank(GSPacketIn pkg)
        {
            int consortiaId = pkg.ReadInt();
            int damage = pkg.ReadInt();
            int richer = pkg.ReadInt();
            int honor = pkg.ReadInt();
            string nickName = pkg.ReadString();
            int userID = pkg.ReadInt();
            ConsortiaBossMgr.UpdateRank(consortiaId, damage, richer, honor, nickName, userID);
        }

        public void HandleReload(GSPacketIn pkg)
        {
            eReloadType type = (eReloadType) pkg.ReadInt();
            int num = pkg.ReadInt();
            bool flag = pkg.ReadBoolean();
            Console.WriteLine(string.Concat(new object[] { num, " ", type.ToString(), " is reload ", flag ? "succeed!" : "fail" }));
        }

        public void HandleSendConsortiaBossInfo(ConsortiaInfo consortia, byte code)
        {
            GSPacketIn pkg = new GSPacketIn(180);
            pkg.WriteInt(consortia.ConsortiaID);
            pkg.WriteInt(consortia.ChairmanID);
            pkg.WriteByte((byte) consortia.bossState);
            pkg.WriteDateTime(consortia.endTime);
            pkg.WriteInt(consortia.extendAvailableNum);
            pkg.WriteInt(consortia.callBossLevel);
            pkg.WriteInt(consortia.Level);
            pkg.WriteInt(consortia.SmithLevel);
            pkg.WriteInt(consortia.StoreLevel);
            pkg.WriteInt(consortia.SkillLevel);
            pkg.WriteInt(consortia.Riches);
            pkg.WriteDateTime(consortia.LastOpenBoss);
            pkg.WriteLong(consortia.MaxBlood);
            pkg.WriteLong(consortia.TotalAllMemberDame);
            pkg.WriteBoolean(consortia.IsBossDie);
            List<RankingPersonInfo> list = ConsortiaBossMgr.SelectRank(consortia.ConsortiaID);
            pkg.WriteInt(list.Count);
            int val = 1;
            foreach (RankingPersonInfo info in list)
            {
                pkg.WriteString(info.Name);
                pkg.WriteInt(val);
                pkg.WriteInt(info.TotalDamage);
                pkg.WriteInt(info.Honor);
                pkg.WriteInt(info.Damage);
                val++;
            }
            pkg.WriteByte(code);
            this._svr.SendToALL(pkg);
        }

        public void HandleShutdown(GSPacketIn pkg)
        {
            int num = pkg.ReadInt();
            if (pkg.ReadBoolean())
            {
                Console.WriteLine(num + "  begin stoping !");
            }
            else
            {
                Console.WriteLine(num + "  is stoped !");
            }
        }

        public void HandleUpdatePlayerState(GSPacketIn pkg)
        {
            Player player = LoginMgr.GetPlayer(pkg.ReadInt());
            if ((player != null) && (player.CurrentServer != null))
            {
                player.CurrentServer.SendTCP(pkg);
            }
        }

        private void HandleUserLogin(GSPacketIn pkg)
        {
            int id = pkg.ReadInt();
            if (LoginMgr.TryLoginPlayer(id, this))
            {
                this.SendAllowUserLogin(id, true);
            }
            else
            {
                this.SendAllowUserLogin(id, false);
            }
        }

        private void HandleUserOffline(GSPacketIn pkg)
        {
            new List<int>();
            int num = pkg.ReadInt();
            for (int i = 0; i < num; i++)
            {
                int id = pkg.ReadInt();
                pkg.ReadInt();
                LoginMgr.PlayerLoginOut(id, this);
            }
            this._svr.SendToALL(pkg);
        }

        private void HandleUserOnline(GSPacketIn pkg)
        {
            int num = pkg.ReadInt();
            for (int i = 0; i < num; i++)
            {
                int id = pkg.ReadInt();
                pkg.ReadInt();
                LoginMgr.PlayerLogined(id, this);
            }
            this._svr.SendToALL(pkg, this);
        }

        private void HandleUserPrivateMsg(GSPacketIn pkg, int playerid)
        {
            ServerClient serverClient = LoginMgr.GetServerClient(playerid);
            if (serverClient != null)
            {
                serverClient.SendTCP(pkg);
            }
        }

        public void HandleUserPublicMsg(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }

        public void HandleWorldBossFightOver(GSPacketIn pkg)
        {
            WorldMgr.WorldBossFightOver();
            this._svr.SendWorldBossFightOver();
        }

        public void HandleWorldBossPrivateInfo(GSPacketIn pkg)
        {
            string name = pkg.ReadString();
            this._svr.SendPrivateInfo(name);
        }

        public void HandleWorldBossRank(GSPacketIn pkg, bool update)
        {
            if (update)
            {
                int damage = pkg.ReadInt();
                int honor = pkg.ReadInt();
                string nickName = pkg.ReadString();
                WorldMgr.UpdateRank(damage, honor, nickName);
            }
            this._svr.SendUpdateRank(false);
        }

        public void HandleWorldBossRoomClose(GSPacketIn pkg)
        {
            WorldMgr.WorldBossRoomClose();
            this._svr.SendRoomClose(0);
        }

        public void HandleWorldBossUpdateBlood(GSPacketIn pkg)
        {
            int num = pkg.ReadInt();
            if (num > 0)
            {
                WorldMgr.ReduceBlood(num);
            }
            this._svr.SendUpdateWorldBlood();
        }

        public void HandleWorldEvent(GSPacketIn pkg)
        {
            GSPacketIn @in = new GSPacketIn(0x5b);
            switch (pkg.ReadByte())
            {
                case 1:
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
                    WorldMgr.AddOrUpdateLanternriddles(lanternriddles.PlayerID, lanternriddles);
                    return;
                }
                case 2:
                {
                    LanternriddlesInfo info2 = WorldMgr.GetLanternriddles(pkg.ReadInt());
                    @in.WriteByte(2);
                    if (info2 != null)
                    {
                        @in.WriteInt(1);
                        @in.WriteInt(info2.PlayerID);
                        @in.WriteInt(info2.QuestionIndex);
                        @in.WriteInt(info2.QuestionView);
                        @in.WriteDateTime(info2.EndDate);
                        @in.WriteInt(info2.DoubleFreeCount);
                        @in.WriteInt(info2.DoublePrice);
                        @in.WriteInt(info2.HitFreeCount);
                        @in.WriteInt(info2.HitPrice);
                        @in.WriteInt(info2.MyInteger);
                        @in.WriteInt(info2.QuestionNum);
                        @in.WriteInt(info2.Option);
                        @in.WriteBoolean(info2.IsHint);
                        @in.WriteBoolean(info2.IsDouble);
                        break;
                    }
                    @in.WriteInt(0);
                    break;
                }
                default:
                    return;
            }
            this._svr.SendToALL(@in);
        }

        protected override void OnConnect()
        {
            base.OnConnect();
            this._rsa = new RSACryptoServiceProvider();
            RSAParameters parameters = this._rsa.ExportParameters(false);
            this.SendRSAKey(parameters.Modulus, parameters.Exponent);
        }

        protected override void OnDisconnect()
        {
            base.OnDisconnect();
            this._rsa = null;
            List<Player> serverPlayers = LoginMgr.GetServerPlayers(this);
            LoginMgr.RemovePlayer(serverPlayers);
            this.SendUserOffline(serverPlayers);
            if (this.Info != null)
            {
                this.Info.State = 1;
                this.Info.Online = 0;
                this.Info = null;
            }
        }

        public override void OnRecvPacket(GSPacketIn pkg)
        {
            short code = pkg.Code;
            if (code <= 0x5b)
            {
                if (code <= 0x25)
                {
                    switch (code)
                    {
                        case 1:
                            this.HandleLogin(pkg);
                            return;

                        case 2:
                        case 7:
                        case 8:
                        case 9:
                        case 0x10:
                        case 0x11:
                        case 0x12:
                            return;

                        case 3:
                            this.HandleUserLogin(pkg);
                            return;

                        case 4:
                            this.HandleUserOffline(pkg);
                            return;

                        case 5:
                            this.HandleUserOnline(pkg);
                            return;

                        case 6:
                            this.HandleQuestUserState(pkg);
                            return;

                        case 10:
                            this.HandkeItemStrengthen(pkg);
                            return;

                        case 11:
                            this.HandleReload(pkg);
                            return;

                        case 12:
                            this.HandlePing(pkg);
                            return;

                        case 13:
                            this.HandleUpdatePlayerState(pkg);
                            return;

                        case 14:
                            this.HandleMarryRoomInfoToPlayer(pkg);
                            return;

                        case 15:
                            this.HandleShutdown(pkg);
                            return;

                        case 0x13:
                            this.HandleChatScene(pkg);
                            return;
                    }
                    if (code == 0x25)
                    {
                        this.HandleChatPersonal(pkg);
                    }
                }
                else
                {
                    switch (code)
                    {
                        case 0x51:
                            this.HandleWorldBossRank(pkg, true);
                            break;

                        case 0x52:
                            this.HandleWorldBossFightOver(pkg);
                            break;

                        case 0x53:
                            this.HandleWorldBossRoomClose(pkg);
                            break;

                        case 0x54:
                            this.HandleWorldBossUpdateBlood(pkg);
                            break;

                        case 0x55:
                            this.HandleWorldBossPrivateInfo(pkg);
                            break;

                        case 0x56:
                            this.HandleWorldBossRank(pkg, false);
                            break;

                        case 90:
                            this.HandleEventRank(pkg);
                            break;

                        case 0x5b:
                            this.HandleWorldEvent(pkg);
                            break;

                        case 0x48:
                            this.HandleBigBugle(pkg);
                            break;
                    }
                }
            }
            else if (code <= 130)
            {
                switch (code)
                {
                    case 0x80:
                        this.HandleConsortiaResponse(pkg);
                        break;

                    case 130:
                        this.HandleConsortiaCreate(pkg);
                        break;

                    case 0x75:
                        this.HandleMailResponse(pkg);
                        break;
                }
            }
            else
            {
                switch (code)
                {
                    case 0x9c:
                        this.HandleConsortiaOffer(pkg);
                        return;

                    case 0x9d:
                    case 0x9f:
                    case 0xb3:
                    case 0xb9:
                        return;

                    case 0x9e:
                        this.HandleConsortiaFight(pkg);
                        return;

                    case 160:
                        this.HandleFriend(pkg);
                        return;

                    case 0xb2:
                        this.HandleMacroDrop(pkg);
                        return;

                    case 180:
                        this.HandleRecvConsortiaBossAdd(pkg);
                        return;

                    case 0xb5:
                        this.HandleRecvConsortiaBossUpdateRank(pkg);
                        return;

                    case 0xb6:
                        this.HandleRecvConsortiaBossExtendAvailable(pkg);
                        return;

                    case 0xb7:
                        this.HandleRecvConsortiaBossCreate(pkg);
                        return;

                    case 0xb8:
                        this.HandleRecvConsortiaBossReload(pkg);
                        return;

                    case 0xba:
                        this.HandleRecvConsortiaBossUpdateBlood(pkg);
                        return;
                }
                if (code == 240)
                {
                    this.HandleIPAndPort(pkg);
                }
            }
        }

        public void SendAllowUserLogin(int playerid, bool allow)
        {
            GSPacketIn pkg = new GSPacketIn(3);
            pkg.WriteInt(playerid);
            pkg.WriteBoolean(allow);
            this.SendTCP(pkg);
        }

        public void SendASS(bool state)
        {
            GSPacketIn pkg = new GSPacketIn(7);
            pkg.WriteBoolean(state);
            this.SendTCP(pkg);
        }

        public void SendChargeMoney(int player, string chargeID)
        {
            GSPacketIn pkg = new GSPacketIn(9, player);
            pkg.WriteString(chargeID);
            this.SendTCP(pkg);
        }

        public void SendKitoffUser(int playerid)
        {
            this.SendKitoffUser(playerid, LanguageMgr.GetTranslation("Center.Server.SendKitoffUser", new object[0]));
        }

        public void SendKitoffUser(int playerid, string msg)
        {
            GSPacketIn pkg = new GSPacketIn(2);
            pkg.WriteInt(playerid);
            pkg.WriteString(msg);
            this.SendTCP(pkg);
        }

        public void SendRSAKey(byte[] m, byte[] e)
        {
            GSPacketIn pkg = new GSPacketIn(0);
            pkg.Write(m);
            pkg.Write(e);
            this.SendTCP(pkg);
        }

        public void SendUserOffline(List<Player> users)
        {
            for (int i = 0; i < users.Count; i += 100)
            {
                int val = ((i + 100) > users.Count) ? (users.Count - i) : 100;
                GSPacketIn pkg = new GSPacketIn(4);
                pkg.WriteInt(val);
                for (int j = i; j < (i + val); j++)
                {
                    pkg.WriteInt(users[j].Id);
                    pkg.WriteInt(0);
                }
                this.SendTCP(pkg);
                this._svr.SendToALL(pkg, this);
            }
        }

        public void SendUserState(int player, bool state)
        {
            GSPacketIn pkg = new GSPacketIn(6, player);
            pkg.WriteBoolean(state);
            this.SendTCP(pkg);
        }

        public ServerInfo Info { get; set; }
    }
}

