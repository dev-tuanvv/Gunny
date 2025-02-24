namespace Game.Base.Packets
{
    using Bussiness;
    using Game.Server;
    using Game.Server.Achievement;
    using Game.Server.Buffer;
    using Game.Server.GameObjects;
    using Game.Server.GameUtils;
    using Game.Server.Managers;
    using Game.Server.Packets;
    using Game.Server.Quests;
    using Game.Server.Rooms;
    using Game.Server.SceneMarryRooms;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    [PacketLib(1)]
    public class AbstractPacketLib : IPacketLib
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected readonly GameClient m_gameClient;

        public AbstractPacketLib(GameClient client)
        {
            this.m_gameClient = client;
        }

        public static IPacketLib CreatePacketLibForVersion(int rawVersion, GameClient client)
        {
            foreach (Type type in ScriptMgr.GetDerivedClasses(typeof(IPacketLib)))
            {
                foreach (PacketLibAttribute attribute in type.GetCustomAttributes(typeof(PacketLibAttribute), false))
                {
                    if (attribute.RawVersion == rawVersion)
                    {
                        try
                        {
                            return (IPacketLib) Activator.CreateInstance(type, new object[] { client });
                        }
                        catch (Exception exception)
                        {
                            if (log.IsErrorEnabled)
                            {
                                log.Error(string.Concat(new object[] { "error creating packetlib (", type.FullName, ") for raw version ", rawVersion }), exception);
                            }
                        }
                    }
                }
            }
            return null;
        }

        public GSPacketIn SendAASControl(bool result, bool IsAASInfo, bool IsMinor)
        {
            GSPacketIn packet = new GSPacketIn(0xe3);
            packet.WriteBoolean(true);
            packet.WriteInt(1);
            packet.WriteBoolean(true);
            packet.WriteBoolean(IsMinor);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendAASInfoSet(bool result)
        {
            GSPacketIn packet = new GSPacketIn(0xe0);
            packet.WriteBoolean(result);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendAASState(bool result)
        {
            GSPacketIn packet = new GSPacketIn(0xe0);
            packet.WriteBoolean(result);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendActivityList(int ID)
        {
            GSPacketIn packet = new GSPacketIn(0x9b, ID);
            packet.WriteByte(1);
            packet.WriteInt(0);
            packet.WriteInt(0);
            packet.WriteInt(1);
            for (int i = 0; i < 1; i++)
            {
                packet.WriteInt(0);
                packet.WriteInt(1);
            }
            packet.WriteInt(0);
            packet.WriteInt(1);
            packet.WriteInt(6);
            packet.WriteBoolean(false);
            packet.WriteBoolean(false);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendAchievementDataInfo(List<AchievementData> data)
        {
            GSPacketIn packet = new GSPacketIn(0xe7);
            packet.WriteInt(data.Count);
            foreach (AchievementData data2 in data)
            {
                packet.WriteInt(data2.AchievementID);
                packet.WriteInt(data2.DateComplete.Year);
                packet.WriteInt(data2.DateComplete.Month);
                packet.WriteInt(data2.DateComplete.Day);
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendAchievementDatas(GamePlayer player, BaseAchievement[] infos)
        {
            if ((player == null) || (infos == null))
            {
                return null;
            }
            int val = 0x7d7;
            int num2 = 7;
            int num3 = 7;
            List<string> list = new List<string>();
            GSPacketIn packet = new GSPacketIn(0xe7, player.PlayerCharacter.ID);
            packet.WriteInt(infos.Length);
            for (int i = 0; i < infos.Length; i++)
            {
                BaseAchievement achievement = infos[i];
                packet.WriteInt(achievement.Data.AchievementID);
                packet.WriteInt(val);
                packet.WriteInt(num2);
                packet.WriteInt(num3);
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendAchievementSuccess(AchievementData d)
        {
            GSPacketIn packet = new GSPacketIn(230);
            packet.WriteInt(d.AchievementID);
            packet.WriteInt(d.DateComplete.Year);
            packet.WriteInt(d.DateComplete.Month);
            packet.WriteInt(d.DateComplete.Day);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendAddFriend(PlayerInfo user, int relation, bool state)
        {
            GSPacketIn packet = new GSPacketIn(160, user.ID);
            packet.WriteByte(160);
            packet.WriteBoolean(state);
            packet.WriteInt(user.ID);
            packet.WriteString(user.NickName);
            packet.WriteByte(user.typeVIP);
            packet.WriteInt(user.VIPLevel);
            packet.WriteBoolean(user.Sex);
            packet.WriteString(user.Style);
            packet.WriteString(user.Colors);
            packet.WriteString(user.Skin);
            packet.WriteInt((user.State == 1) ? 1 : 0);
            packet.WriteInt(user.Grade);
            packet.WriteInt(user.Hide);
            packet.WriteString(user.ConsortiaName);
            packet.WriteInt(user.Total);
            packet.WriteInt(user.Escape);
            packet.WriteInt(user.Win);
            packet.WriteInt(user.Offer);
            packet.WriteInt(user.Repute);
            packet.WriteInt(relation);
            packet.WriteString(user.UserName);
            packet.WriteInt(user.Nimbus);
            packet.WriteInt(user.FightPower);
            packet.WriteInt(0);
            packet.WriteInt(0);
            packet.WriteString("");
            packet.WriteInt(0);
            packet.WriteString("");
            packet.WriteInt(user.AchievementPoint);
            packet.WriteString(user.Honor);
            packet.WriteBoolean(user.IsMarried);
            packet.WriteBoolean(user.IsOldPlayer);
            packet.WriteDateTime(user.LastDate);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendAuctionRefresh(AuctionInfo info, int auctionID, bool isExist, SqlDataProvider.Data.ItemInfo item)
        {
            GSPacketIn packet = new GSPacketIn(0xc3);
            packet.WriteInt(auctionID);
            packet.WriteBoolean(isExist);
            if (isExist)
            {
                packet.WriteInt(info.AuctioneerID);
                packet.WriteString(info.AuctioneerName);
                packet.WriteDateTime(info.BeginDate);
                packet.WriteInt(info.BuyerID);
                packet.WriteString(info.BuyerName);
                packet.WriteInt(info.ItemID);
                packet.WriteInt(info.Mouthful);
                packet.WriteInt(info.PayType);
                packet.WriteInt(info.Price);
                packet.WriteInt(info.Rise);
                packet.WriteInt(info.ValidDate);
                packet.WriteBoolean(item != null);
                if (item != null)
                {
                    packet.WriteInt(item.Count);
                    packet.WriteInt(item.TemplateID);
                    packet.WriteInt(item.AttackCompose);
                    packet.WriteInt(item.DefendCompose);
                    packet.WriteInt(item.AgilityCompose);
                    packet.WriteInt(item.LuckCompose);
                    packet.WriteInt(item.StrengthenLevel + item.LianGrade);
                    packet.WriteBoolean(item.IsBinds);
                    packet.WriteBoolean(item.IsJudge);
                    packet.WriteDateTime(item.BeginDate);
                    packet.WriteInt(item.ValidDate);
                    packet.WriteString(item.Color);
                    packet.WriteString(item.Skin);
                    packet.WriteBoolean(item.IsUsed);
                    packet.WriteInt(item.Hole1);
                    packet.WriteInt(item.Hole2);
                    packet.WriteInt(item.Hole3);
                    packet.WriteInt(item.Hole4);
                    packet.WriteInt(item.Hole5);
                    packet.WriteInt(item.Hole6);
                    packet.WriteString(item.Pic);
                    packet.WriteInt(item.RefineryLevel);
                    packet.WriteDateTime(DateTime.Now);
                    packet.WriteByte((byte) item.Hole5Level);
                    packet.WriteInt(item.Hole5Exp);
                    packet.WriteByte((byte) item.Hole6Level);
                    packet.WriteInt(item.Hole6Exp);
                }
            }
            packet.Compress();
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendAvatarCollect(PlayerAvatarCollection avtCollect)
        {
            GSPacketIn packet = new GSPacketIn(0x192);
            packet.WriteByte(5);
            packet.WriteInt(avtCollect.AvatarCollect.Count);
            if (avtCollect.AvatarCollect.Count > 0)
            {
                foreach (UserAvatarCollectionInfo info in avtCollect.AvatarCollect)
                {
                    packet.WriteInt(info.AvatarID);
                    packet.WriteInt(info.Sex);
                    if (info.Items == null)
                    {
                        info.UpdateItems();
                    }
                    packet.WriteInt(info.Items.Count);
                    if (info.Items.Count > 0)
                    {
                        foreach (UserAvatarCollectionDataInfo info2 in info.Items)
                        {
                            packet.WriteInt(info2.TemplateID);
                        }
                    }
                    packet.WriteDateTime(info.TimeEnd);
                }
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendBattleGoundOpen(int ID)
        {
            GSPacketIn packet = new GSPacketIn(0x84, ID);
            packet.WriteByte(1);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendBattleGoundOver(int ID)
        {
            GSPacketIn packet = new GSPacketIn(0x84, ID);
            packet.WriteByte(2);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendBufferList(GamePlayer player, List<AbstractBuffer> infos)
        {
            GSPacketIn packet = new GSPacketIn(0xba, player.PlayerId);
            packet.WriteInt(infos.Count);
            foreach (AbstractBuffer buffer in infos)
            {
                BufferInfo info = buffer.Info;
                packet.WriteInt(info.Type);
                packet.WriteBoolean(info.IsExist);
                packet.WriteDateTime(info.BeginDate);
                packet.WriteInt(info.ValidDate);
                packet.WriteInt(info.Value);
                packet.WriteInt(info.ValidCount);
                packet.WriteInt(info.TemplateID);
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn sendBuyBadge(int BadgeID, int ValidDate, bool result, string BadgeBuyTime, int playerid)
        {
            GSPacketIn packet = new GSPacketIn(0xa4, playerid);
            packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.ConsortiaID);
            packet.WriteInt(BadgeID);
            packet.WriteInt(ValidDate);
            packet.WriteDateTime(Convert.ToDateTime(BadgeBuyTime));
            packet.WriteBoolean(result);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendCampBattleOpenClose(int ID, bool result)
        {
            GSPacketIn packet = new GSPacketIn(0x92, ID);
            packet.WriteByte(10);
            packet.WriteBoolean(result);
            packet.WriteDateTime(DateTime.Now);
            packet.WriteDateTime(DateTime.Now.AddMinutes(90.0));
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendCardReset()
        {
            GSPacketIn @in = new GSPacketIn(0xc4);
            @in.WriteInt(1);
            @in.WriteInt(1);
            return @in;
        }

        public void SendCatchBeastOpen(int playerID, bool isOpen)
        {
            GSPacketIn packet = new GSPacketIn(0x91, playerID);
            packet.WriteByte(0x20);
            packet.WriteBoolean(isOpen);
            this.SendTCP(packet);
        }

        public GSPacketIn SendCollectInfor(int id, byte state)
        {
            GSPacketIn packet = new GSPacketIn(0x20, id);
            packet.WriteByte(state);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn sendCompose(GamePlayer Player)
        {
            GSPacketIn packet = new GSPacketIn(0x51, Player.PlayerCharacter.ID);
            packet.WriteByte(5);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendConsortia(int money, bool result, string msg, int playerid)
        {
            GSPacketIn packet = new GSPacketIn(0x81, playerid);
            packet.WriteByte(6);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn sendConsortiaApplyStatusOut(bool state, bool result, int playerid)
        {
            GSPacketIn packet = new GSPacketIn(0x81, playerid);
            packet.WriteByte(7);
            packet.WriteBoolean(result);
            packet.WriteBoolean(result);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendConsortiaBattleOpenClose(int ID, bool result)
        {
            GSPacketIn packet = new GSPacketIn(0x99, ID);
            packet.WriteByte(1);
            packet.WriteBoolean(result);
            if (result)
            {
                packet.WriteDateTime(DateTime.Now);
                packet.WriteDateTime(DateTime.Now.AddMinutes(90.0));
                packet.WriteBoolean(result);
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendConsortiaCreate(string name1, bool result, int id, string name2, string msg, int dutyLevel, string DutyName, int dutyRight, int playerid)
        {
            GSPacketIn packet = new GSPacketIn(0x81, playerid);
            packet.WriteByte(1);
            packet.WriteString(name1);
            packet.WriteBoolean(result);
            packet.WriteInt(id);
            packet.WriteString(name2);
            packet.WriteString(msg);
            packet.WriteInt(dutyLevel);
            packet.WriteString((DutyName == null) ? "" : DutyName);
            packet.WriteInt(dutyRight);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn sendConsortiaChangeChairman(string nick, bool result, string msg, int playerid)
        {
            GSPacketIn packet = new GSPacketIn(0x81, playerid);
            packet.WriteByte(14);
            packet.WriteString(nick);
            packet.WriteBoolean(result);
            packet.WriteString(msg);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn sendConsortiaEquipConstrol(bool result, List<int> Riches, int playerid)
        {
            GSPacketIn packet = new GSPacketIn(0x81, playerid);
            packet.WriteByte(0x18);
            packet.WriteBoolean(result);
            for (int i = 0; i < Riches.Count; i++)
            {
                packet.WriteInt(Riches[i]);
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendConsortiaInvite(string username, bool result, string msg, int playerid)
        {
            GSPacketIn packet = new GSPacketIn(0x81, playerid);
            packet.WriteByte(11);
            packet.WriteString(username);
            packet.WriteBoolean(result);
            packet.WriteString(msg);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn sendConsortiaInviteDel(int id, bool result, string msg, int playerid)
        {
            GSPacketIn packet = new GSPacketIn(0x81, playerid);
            packet.WriteByte(13);
            packet.WriteInt(id);
            packet.WriteBoolean(result);
            packet.WriteString(msg);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn sendConsortiaInvitePass(int id, bool result, int consortiaid, string consortianame, string msg, int playerid)
        {
            GSPacketIn packet = new GSPacketIn(0x81, playerid);
            packet.WriteByte(12);
            packet.WriteInt(id);
            packet.WriteBoolean(result);
            packet.WriteString(msg);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendConsortiaLevelUp(byte type, byte level, bool result, string msg, int playerid)
        {
            GSPacketIn packet = new GSPacketIn(0x81, playerid);
            packet.WriteByte(0x15);
            packet.WriteByte(type);
            packet.WriteByte(level);
            packet.WriteBoolean(result);
            packet.WriteString(msg);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendConsortiaMail(bool result, int playerid)
        {
            GSPacketIn packet = new GSPacketIn(0x81, playerid);
            packet.WriteByte(0x1d);
            packet.WriteBoolean(result);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendConsortiaMemberGrade(int id, bool update, bool result, string msg, int playerid)
        {
            GSPacketIn packet = new GSPacketIn(0x81, playerid);
            packet.WriteByte(0x12);
            packet.WriteInt(id);
            packet.WriteBoolean(update);
            packet.WriteBoolean(result);
            packet.WriteString(msg);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn sendConsortiaOut(int id, bool result, string msg, int playerid)
        {
            GSPacketIn packet = new GSPacketIn(0x81, playerid);
            packet.WriteByte(3);
            packet.WriteInt(id);
            packet.WriteBoolean(result);
            packet.WriteString(msg);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendConsortiaRichesOffer(int money, bool result, string msg, int playerid)
        {
            GSPacketIn packet = new GSPacketIn(0x81, playerid);
            packet.WriteByte(6);
            packet.WriteInt(money);
            packet.WriteBoolean(result);
            packet.WriteString(msg);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn sendConsortiaTryIn(int id, bool result, string msg, int playerid)
        {
            GSPacketIn packet = new GSPacketIn(0x81, playerid);
            packet.WriteByte(0);
            packet.WriteInt(id);
            packet.WriteBoolean(result);
            packet.WriteString(msg);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn sendConsortiaTryInDel(int id, bool result, string msg, int playerid)
        {
            GSPacketIn packet = new GSPacketIn(0x81, playerid);
            packet.WriteByte(5);
            packet.WriteInt(id);
            packet.WriteBoolean(result);
            packet.WriteString(msg);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn sendConsortiaTryInPass(int id, bool result, string msg, int playerid)
        {
            GSPacketIn packet = new GSPacketIn(0x81, playerid);
            packet.WriteByte(4);
            packet.WriteInt(id);
            packet.WriteBoolean(result);
            packet.WriteString(msg);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn sendConsortiaUpdateDescription(string description, bool result, string msg, int playerid)
        {
            GSPacketIn packet = new GSPacketIn(0x81, playerid);
            packet.WriteByte(14);
            packet.WriteString(description);
            packet.WriteBoolean(result);
            packet.WriteString(msg);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn sendConsortiaUpdatePlacard(string description, bool result, string msg, int playerid)
        {
            GSPacketIn packet = new GSPacketIn(0x81, playerid);
            packet.WriteByte(15);
            packet.WriteString(description);
            packet.WriteBoolean(result);
            packet.WriteString(msg);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendContinuation(GamePlayer player, HotSpringRoomInfo hotSpringRoomInfo)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendContinuation(GamePlayer player, MarryRoomInfo info)
        {
            GSPacketIn packet = new GSPacketIn(0xf9, player.PlayerCharacter.ID);
            packet.WriteByte(3);
            packet.WriteInt(info.AvailTime);
            this.SendTCP(packet);
            return packet;
        }

        public void SendCSMBox(int UserID)
        {
            GSPacketIn packet = new GSPacketIn(360, UserID);
            packet.WriteInt(1);
            packet.WriteInt(60);
            this.SendTCP(packet);
        }

        public void SendCurrentDressModel(PlayerInfo player)
        {
            GSPacketIn packet = new GSPacketIn(0xed);
            packet.WriteByte(2);
            packet.WriteInt(player.CurrentDressModel);
            this.SendTCP(packet);
        }

        public void SendCheckCode()
        {
            if ((this.m_gameClient.Player != null) && (this.m_gameClient.Player.PlayerCharacter.CheckCount >= GameProperties.CHECK_MAX_FAILED_COUNT))
            {
                if (this.m_gameClient.Player.PlayerCharacter.CheckError == 0)
                {
                    PlayerInfo playerCharacter = this.m_gameClient.Player.PlayerCharacter;
                    playerCharacter.CheckCount += 0x2710;
                }
                GSPacketIn packet = new GSPacketIn(200, this.m_gameClient.Player.PlayerCharacter.ID, 0x2800);
                if (this.m_gameClient.Player.PlayerCharacter.CheckError < 1)
                {
                    packet.WriteByte(0);
                }
                else
                {
                    packet.WriteByte(2);
                }
                packet.WriteBoolean(true);
                this.m_gameClient.Player.PlayerCharacter.CheckCode = CheckCode.GenerateCheckCode();
                packet.Write(CheckCode.CreateImage(this.m_gameClient.Player.PlayerCharacter.CheckCode));
                this.SendTCP(packet);
            }
        }

        public GSPacketIn SendChickenBoxOpen(int ID, int flushPrice, int[] openCardPrice, int[] eagleEyePrice)
        {
            int num;
            GSPacketIn packet = new GSPacketIn(0x57, ID);
            packet.WriteInt(1);
            packet.WriteInt(openCardPrice.Length);
            for (num = openCardPrice.Length; num > 0; num--)
            {
                packet.WriteInt(openCardPrice[num - 1]);
            }
            packet.WriteInt(eagleEyePrice.Length);
            for (num = eagleEyePrice.Length; num > 0; num--)
            {
                packet.WriteInt(eagleEyePrice[num - 1]);
            }
            packet.WriteInt(flushPrice);
            packet.WriteDateTime(DateTime.Parse(GameProperties.NewChickenEndTime));
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendDailyAward(PlayerInfo player)
        {
            bool val = false;
            DateTime date = DateTime.Now.Date;
            DateTime time3 = player.LastAward.Date;
            if (date != time3)
            {
                val = true;
            }
            GSPacketIn packet = new GSPacketIn(13, player.ID);
            packet.WriteBoolean(val);
            packet.WriteInt(0);
            this.SendTCP(packet);
            return packet;
        }

        public void SendDateTime()
        {
            GSPacketIn packet = new GSPacketIn(5);
            packet.WriteDateTime(DateTime.Now);
            this.SendTCP(packet);
        }

        public GSPacketIn SendDiceActiveClose(int ID)
        {
            GSPacketIn packet = new GSPacketIn(0x86, ID);
            packet.WriteByte(2);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendDiceActiveOpen(PlayerDice Dice)
        {
            GSPacketIn packet = new GSPacketIn(0x86, Dice.Data.UserID);
            packet.WriteByte(1);
            packet.WriteInt(Dice.Data.FreeCount);
            packet.WriteInt(Dice.refreshPrice);
            packet.WriteInt(Dice.commonDicePrice);
            packet.WriteInt(Dice.doubleDicePrice);
            packet.WriteInt(Dice.bigDicePrice);
            packet.WriteInt(Dice.smallDicePrice);
            packet.WriteInt(Dice.MAX_LEVEL);
            for (int i = 0; i < Dice.MAX_LEVEL; i++)
            {
                List<DiceLevelAwardInfo> list = Dice.LevelAward[i];
                packet.WriteInt(Dice.IntegralPoint[i]);
                packet.WriteInt(list.Count);
                for (int j = 0; j < list.Count; j++)
                {
                    packet.WriteInt(list[j].TemplateID);
                    packet.WriteInt(list[j].Count);
                }
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendDiceReceiveData(PlayerDice Dice)
        {
            GSPacketIn packet = new GSPacketIn(0x86, Dice.Data.UserID);
            packet.WriteByte(3);
            packet.WriteBoolean(Dice.Data.UserFirstCell);
            packet.WriteInt(Dice.Data.CurrentPosition);
            packet.WriteInt(Dice.Data.LuckIntegralLevel);
            packet.WriteInt(Dice.Data.LuckIntegral);
            packet.WriteInt(Dice.Data.FreeCount);
            packet.WriteInt(Dice.RewardItem.Count);
            for (int i = 0; i < Dice.RewardItem.Count; i++)
            {
                packet.WriteInt(Dice.RewardItem[i].TemplateID);
                packet.WriteInt(i);
                packet.WriteInt(Dice.RewardItem[i].StrengthenLevel);
                packet.WriteInt(Dice.RewardItem[i].Count);
                packet.WriteInt(Dice.RewardItem[i].ValidDate);
                packet.WriteBoolean(Dice.RewardItem[i].IsBinds);
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendDiceReceiveResult(PlayerDice Dice)
        {
            GSPacketIn packet = new GSPacketIn(0x86, Dice.Data.UserID);
            packet.WriteByte(4);
            packet.WriteInt(Dice.Data.CurrentPosition);
            packet.WriteInt(Dice.result);
            packet.WriteInt(Dice.Data.LuckIntegral);
            packet.WriteInt(Dice.Data.Level);
            packet.WriteInt(Dice.Data.FreeCount);
            packet.WriteString(Dice.RewardName);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SenddoMature(GamePlayer Player)
        {
            GSPacketIn packet = new GSPacketIn(0x51, Player.PlayerCharacter.ID);
            packet.WriteByte(3);
            this.SendTCP(packet);
            return packet;
        }

        public void SendDragonBoat(PlayerInfo info)
        {
            GSPacketIn packet = new GSPacketIn(100, info.ID);
            packet.WriteByte(1);
            packet.WriteInt(0);
            packet.WriteInt(ActiveSystemMgr.periodType);
            packet.WriteInt(ActiveSystemMgr.boatCompleteExp);
            this.SendTCP(packet);
        }

        public GSPacketIn SendDressModelInfo(PlayerDressModel dressModel)
        {
            GSPacketIn packet = new GSPacketIn(0xed);
            packet.WriteByte(1);
            packet.WriteInt(3);
            for (int i = 0; i < 3; i++)
            {
                packet.WriteInt(i);
                List<UserDressModelInfo> dressModelWithSlotID = dressModel.GetDressModelWithSlotID(i);
                packet.WriteInt(dressModelWithSlotID.Count);
                if (dressModelWithSlotID.Count > 0)
                {
                    foreach (UserDressModelInfo info in dressModelWithSlotID)
                    {
                        packet.WriteInt(info.ItemID);
                        packet.WriteInt(info.TemplateID);
                    }
                }
            }
            this.SendTCP(packet);
            return packet;
        }

        public void SendEdictumVersion()
        {
            EdictumInfo[] allEdictumVersion = WorldMgr.GetAllEdictumVersion();
            Random random = new Random();
            if (allEdictumVersion.Length != 0)
            {
                GSPacketIn packet = new GSPacketIn(0x4b);
                packet.WriteInt(allEdictumVersion.Length);
                foreach (EdictumInfo info in allEdictumVersion)
                {
                    packet.WriteInt(info.ID + random.Next(0x2710));
                }
                this.SendTCP(packet);
            }
        }

        public void SendEditionError(string msg)
        {
            GSPacketIn packet = new GSPacketIn(12);
            packet.WriteString(msg);
            this.SendTCP(packet);
        }

        public GSPacketIn SendEnterFarm(PlayerInfo Player, UserFarmInfo farm, UserFieldInfo[] fields)
        {
            GSPacketIn packet = new GSPacketIn(0x51, Player.ID);
            packet.WriteByte(1);
            packet.WriteInt(farm.FarmID);
            packet.WriteBoolean(farm.isFarmHelper);
            packet.WriteInt(farm.isAutoId);
            packet.WriteDateTime(farm.AutoPayTime);
            packet.WriteInt(farm.AutoValidDate);
            packet.WriteInt(farm.GainFieldId);
            packet.WriteInt(farm.KillCropId);
            packet.WriteInt(fields.Length);
            for (int i = 0; i < fields.Length; i++)
            {
                UserFieldInfo info = fields[i];
                packet.WriteInt(info.FieldID);
                packet.WriteInt(info.SeedID);
                packet.WriteDateTime(info.PayTime);
                packet.WriteDateTime(info.PlantTime);
                packet.WriteInt(info.GainCount);
                packet.WriteInt(info.FieldValidDate);
                packet.WriteInt(info.AccelerateTime);
            }
            if (farm.FarmID == Player.ID)
            {
                packet.WriteInt(0x8235);
                packet.WriteString(farm.PayFieldMoney);
                packet.WriteString(farm.PayAutoMoney);
                packet.WriteDateTime(farm.AutoPayTime);
                packet.WriteInt(farm.AutoValidDate);
                packet.WriteInt(Player.VIPLevel);
                packet.WriteInt(farm.buyExpRemainNum);
            }
            else
            {
                packet.WriteBoolean(farm.isArrange);
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendEnterHotSpringRoom(GamePlayer player)
        {
            if (player.CurrentHotSpringRoom == null)
            {
                return null;
            }
            GSPacketIn packet = new GSPacketIn(0xca, player.PlayerCharacter.ID);
            packet.WriteInt(player.CurrentHotSpringRoom.Info.roomID);
            packet.WriteInt(player.CurrentHotSpringRoom.Info.roomNumber);
            packet.WriteString(player.CurrentHotSpringRoom.Info.roomName);
            packet.WriteString(player.CurrentHotSpringRoom.Info.roomPassword);
            packet.WriteInt(player.CurrentHotSpringRoom.Info.effectiveTime);
            packet.WriteInt(player.CurrentHotSpringRoom.Count);
            packet.WriteInt(player.CurrentHotSpringRoom.Info.playerID);
            packet.WriteString(player.CurrentHotSpringRoom.Info.playerName);
            packet.WriteDateTime(player.CurrentHotSpringRoom.Info.startTime);
            packet.WriteString(player.CurrentHotSpringRoom.Info.roomIntroduction);
            packet.WriteInt(player.CurrentHotSpringRoom.Info.roomType);
            packet.WriteInt(player.CurrentHotSpringRoom.Info.maxCount);
            packet.WriteDateTime(player.Extra.Info.LastTimeHotSpring);
            packet.WriteInt(player.Extra.Info.MinHotSpring);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendEquipChange(GamePlayer player, int place, int goodsID, string style)
        {
            GSPacketIn packet = new GSPacketIn(0x42, player.PlayerCharacter.ID);
            packet.WriteByte((byte) place);
            packet.WriteInt(goodsID);
            packet.WriteString(style);
            this.SendTCP(packet);
            return packet;
        }

        public void SendExpBlessedData(int PlayerId)
        {
            GSPacketIn packet = new GSPacketIn(0x9b, PlayerId);
            packet.WriteByte(8);
            packet.WriteInt(0);
            this.SendTCP(packet);
        }

        public GSPacketIn SendFightFootballTimeOpenClose(int ID, bool result)
        {
            GSPacketIn packet = new GSPacketIn(0x97, ID);
            packet.WriteBoolean(result);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendFindBackIncome(int ID)
        {
            GSPacketIn packet = new GSPacketIn(0x93, ID);
            packet.WriteInt(6);
            packet.WriteBoolean(false);
            packet.WriteBoolean(false);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendFriendRemove(int FriendID)
        {
            GSPacketIn packet = new GSPacketIn(160, FriendID);
            packet.WriteByte(0xa1);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendFriendState(int playerID, int state, byte typeVip, int viplevel)
        {
            GSPacketIn packet = new GSPacketIn(160, playerID);
            packet.WriteByte(0xa5);
            packet.WriteInt(state);
            packet.WriteInt(typeVip);
            packet.WriteInt(viplevel);
            packet.WriteBoolean(true);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendFusionPreview(GamePlayer player, Dictionary<int, double> previewItemList, bool isbind, int MinValid)
        {
            GSPacketIn packet = new GSPacketIn(0x4e, player.PlayerCharacter.ID);
            packet.WriteInt(3);
            packet.WriteInt(previewItemList.Count);
            foreach (KeyValuePair<int, double> pair in previewItemList)
            {
                packet.WriteInt(pair.Key);
                packet.WriteInt(MinValid);
                int num = Convert.ToInt32(pair.Value);
                packet.WriteInt((num > 100) ? 100 : ((num < 0) ? 0 : num));
            }
            packet.WriteBoolean(isbind);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendFusionResult(GamePlayer player, bool result)
        {
            GSPacketIn packet = new GSPacketIn(0x4e, player.PlayerCharacter.ID);
            packet.WriteBoolean(result);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendGameMissionPrepare()
        {
            GSPacketIn packet = new GSPacketIn(0x74);
            packet.WriteBoolean(true);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendGameMissionStart()
        {
            GSPacketIn packet = new GSPacketIn(0x52);
            packet.WriteBoolean(true);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendGameRoomSetupChange(BaseRoom room)
        {
            GSPacketIn packet = new GSPacketIn(0x6b);
            packet.WriteInt(room.MapId);
            packet.WriteByte((byte) room.RoomType);
            packet.WriteString(room.Password);
            packet.WriteString(room.Name);
            packet.WriteByte(room.TimeMode);
            packet.WriteByte((byte) room.HardLevel);
            packet.WriteInt(room.LevelLimits);
            packet.WriteBoolean(false);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendGetBoxTime(int ID, int receiebox, bool result)
        {
            GSPacketIn packet = new GSPacketIn(0x35, ID);
            packet.WriteBoolean(result);
            packet.WriteInt(receiebox);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendGetCard(PlayerInfo player, UsersCardInfo card)
        {
            GSPacketIn packet = new GSPacketIn(0xd8, player.ID);
            packet.WriteInt(player.ID);
            packet.WriteInt(1);
            packet.WriteInt(card.Place);
            packet.WriteBoolean(true);
            packet.WriteInt(card.CardID);
            packet.WriteInt(card.UserID);
            packet.WriteInt(card.Count);
            packet.WriteInt(card.Place);
            packet.WriteInt(card.TemplateID);
            packet.WriteInt(card.Attack);
            packet.WriteInt(card.Defence);
            packet.WriteInt(card.Agility);
            packet.WriteInt(card.Luck);
            packet.WriteInt(card.Damage);
            packet.WriteInt(card.Guard);
            packet.WriteInt(card.Level);
            packet.WriteInt(card.CardGP);
            packet.WriteBoolean(card.isFirstGet);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendGetPlayerCard(int playerId)
        {
            GSPacketIn packet = new GSPacketIn(0x12, playerId);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendGetSpree(PlayerInfo player)
        {
            GSPacketIn packet = new GSPacketIn(0x9d, player.ID);
            packet.WriteBoolean(true);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendGrowthPackageOpen(int ID, int isBuy)
        {
            GSPacketIn packet = new GSPacketIn(0x54, ID);
            packet.WriteInt(1);
            packet.WriteInt(1);
            packet.WriteInt(isBuy);
            for (int i = 0; i < 9; i++)
            {
                if (i < isBuy)
                {
                    packet.WriteInt(1);
                }
                else
                {
                    packet.WriteInt(0);
                }
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendGrowthPackageUpadte(int ID, int isBuy)
        {
            GSPacketIn packet = new GSPacketIn(0x54, ID);
            packet.WriteInt(1);
            packet.WriteInt(2);
            packet.WriteInt(isBuy);
            for (int i = 0; i < 9; i++)
            {
                if (i < isBuy)
                {
                    packet.WriteInt(1);
                }
                else
                {
                    packet.WriteInt(0);
                }
            }
            this.SendTCP(packet);
            return packet;
        }

        public void SendGuildMemberWeekOpenClose(PyramidConfigInfo info)
        {
            GSPacketIn packet = new GSPacketIn(0x91, info.UserID);
            packet.WriteByte(7);
            packet.WriteBoolean(info.isOpen);
            packet.WriteString(info.beginTime.ToString());
            packet.WriteString(info.endTime.ToString());
            this.SendTCP(packet);
        }

        public GSPacketIn SendHelperSwitchField(PlayerInfo Player, UserFarmInfo farm)
        {
            GSPacketIn packet = new GSPacketIn(0x51, Player.ID);
            packet.WriteByte(9);
            packet.WriteBoolean(farm.isFarmHelper);
            packet.WriteInt(farm.isAutoId);
            packet.WriteDateTime(farm.AutoPayTime);
            packet.WriteInt(farm.AutoValidDate);
            packet.WriteInt(farm.GainFieldId);
            packet.WriteInt(farm.KillCropId);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendHotSpringUpdateTime(GamePlayer player, int expAdd)
        {
            if (player.CurrentHotSpringRoom == null)
            {
                return null;
            }
            GSPacketIn packet = new GSPacketIn(0xbf, player.PlayerCharacter.ID);
            packet.WriteByte(7);
            packet.WriteInt(player.Extra.Info.MinHotSpring);
            packet.WriteInt(expAdd);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendIDNumberCheck(bool result)
        {
            GSPacketIn packet = new GSPacketIn(0xe2);
            packet.WriteBoolean(result);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendKillCropField(PlayerInfo Player, UserFieldInfo field)
        {
            GSPacketIn packet = new GSPacketIn(0x51, Player.ID);
            packet.WriteByte(7);
            packet.WriteBoolean(true);
            packet.WriteInt(field.FieldID);
            packet.WriteInt(field.SeedID);
            packet.WriteInt(field.AccelerateTime);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendKingBlessMain(PlayerExtra Extra)
        {
            GSPacketIn packet = new GSPacketIn(0x8e, Extra.Info.UserID);
            packet.WriteInt(3);
            packet.WriteDateTime(Extra.Info.KingBlessEnddate);
            packet.WriteInt(Extra.KingBlessInfo.Count);
            foreach (int num in Extra.KingBlessInfo.Keys)
            {
                packet.WriteInt(num);
                packet.WriteInt(Extra.KingBlessInfo[num]);
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendKingBlessUpdateBuffData(int UserID, int data, int value)
        {
            GSPacketIn packet = new GSPacketIn(0x8f, UserID);
            packet.WriteInt(data);
            packet.WriteInt(value);
            this.SendTCP(packet);
            return packet;
        }

        public void SendKitoff(string msg)
        {
            GSPacketIn packet = new GSPacketIn(2);
            packet.WriteString(msg);
            this.SendTCP(packet);
        }

        public GSPacketIn SendLabyrinthUpdataInfo(int ID, UserLabyrinthInfo laby)
        {
            GSPacketIn packet = new GSPacketIn(0x83, ID);
            packet.WriteByte(2);
            packet.WriteInt(laby.myProgress);
            packet.WriteInt(laby.currentFloor);
            packet.WriteBoolean(laby.completeChallenge);
            packet.WriteInt(laby.remainTime);
            packet.WriteInt(laby.accumulateExp);
            packet.WriteInt(laby.cleanOutAllTime);
            packet.WriteInt(laby.cleanOutGold);
            packet.WriteInt(laby.myRanking);
            packet.WriteBoolean(laby.isDoubleAward);
            packet.WriteBoolean(laby.isInGame);
            packet.WriteBoolean(laby.isCleanOut);
            packet.WriteBoolean(laby.serverMultiplyingPower);
            this.SendTCP(packet);
            return packet;
        }

        public void SendLanternriddlesOpen(int playerID, bool isOpen)
        {
            GSPacketIn packet = new GSPacketIn(0x91, playerID);
            packet.WriteByte(0x25);
            packet.WriteBoolean(isOpen);
            this.SendTCP(packet);
        }

        public void SendLeagueNotice(int id, int restCount, int maxCount, byte type)
        {
            GSPacketIn packet = new GSPacketIn(0x2a, id);
            packet.WriteByte(type);
            if (type == 1)
            {
                packet.WriteInt(restCount);
                packet.WriteInt(maxCount);
            }
            else
            {
                packet.WriteInt(restCount);
            }
            this.SendTCP(packet);
        }

        public void SendLittleGameActived()
        {
            GSPacketIn packet = new GSPacketIn(80);
            packet.WriteBoolean(true);
            this.SendTCP(packet);
        }

        public void SendLoginFailed(string msg)
        {
            GSPacketIn packet = new GSPacketIn(1);
            packet.WriteByte(1);
            packet.WriteString(msg);
            this.SendTCP(packet);
        }

        public void SendLoginSuccess()
        {
            if (this.m_gameClient.Player != null)
            {
                GSPacketIn packet = new GSPacketIn(1, this.m_gameClient.Player.PlayerCharacter.ID);
                packet.WriteByte(0);
                packet.WriteInt(4);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Attack);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Defence);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Agility);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Luck);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.GP);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Repute);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Gold);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Money);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.GiftToken);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Score);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Hide);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.FightPower);
                packet.WriteInt(0);
                packet.WriteInt(0);
                packet.WriteString("");
                packet.WriteInt(0);
                packet.WriteString("");
                packet.WriteDateTime(DateTime.Now.AddDays(50.0));
                packet.WriteByte(this.m_gameClient.Player.PlayerCharacter.typeVIP);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.VIPLevel);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.VIPExp);
                packet.WriteDateTime(this.m_gameClient.Player.PlayerCharacter.VIPExpireDay);
                packet.WriteDateTime(this.m_gameClient.Player.PlayerCharacter.LastDate);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.VIPNextLevelDaysNeeded);
                packet.WriteDateTime(DateTime.Now);
                packet.WriteBoolean(this.m_gameClient.Player.PlayerCharacter.CanTakeVipReward);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.OptionOnOff);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.AchievementPoint);
                packet.WriteString(this.m_gameClient.Player.PlayerCharacter.Honor);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.OnlineTime);
                packet.WriteBoolean(this.m_gameClient.Player.PlayerCharacter.Sex);
                packet.WriteString(this.m_gameClient.Player.PlayerCharacter.Style + "&" + this.m_gameClient.Player.PlayerCharacter.Colors);
                packet.WriteString(this.m_gameClient.Player.PlayerCharacter.Skin);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.ConsortiaID);
                packet.WriteString(this.m_gameClient.Player.PlayerCharacter.ConsortiaName);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.badgeID);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.DutyLevel);
                packet.WriteString(this.m_gameClient.Player.PlayerCharacter.DutyName);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Right);
                packet.WriteString(this.m_gameClient.Player.PlayerCharacter.ChairmanName);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.ConsortiaHonor);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.ConsortiaRiches);
                packet.WriteBoolean(this.m_gameClient.Player.PlayerCharacter.HasBagPassword);
                packet.WriteString(this.m_gameClient.Player.PlayerCharacter.PasswordQuest1);
                packet.WriteString(this.m_gameClient.Player.PlayerCharacter.PasswordQuest2);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.FailedPasswordAttemptCount);
                packet.WriteString(this.m_gameClient.Player.PlayerCharacter.UserName);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Nimbus);
                packet.WriteString(this.m_gameClient.Player.PlayerCharacter.PvePermission);
                packet.WriteString("1-2-3-4-5-6-7-8-9-10-11-12-13");
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.WeaklessGuildProgressStr.Length);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.receiebox);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.receieGrade);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.needGetBoxTime);
                packet.WriteDateTime(DateTime.Now.AddDays(0.0));
                packet.WriteDateTime(DateTime.Now);
                packet.WriteInt(0);
                packet.WriteInt(0);
                packet.WriteInt(0);
                packet.WriteInt(0);
                packet.WriteInt(0);
                packet.WriteInt(0);
                packet.WriteInt(0);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Texp.spdTexpExp);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Texp.attTexpExp);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Texp.defTexpExp);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Texp.hpTexpExp);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Texp.lukTexpExp);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Texp.texpTaskCount);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.Texp.texpCount);
                packet.WriteDateTime(this.m_gameClient.Player.PlayerCharacter.Texp.texpTaskDate);
                packet.WriteBoolean(this.m_gameClient.Player.PlayerCharacter.isOldPlayerHasValidEquitAtLogin);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.badLuckNumber);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.luckyNum);
                packet.WriteDateTime(this.m_gameClient.Player.PlayerCharacter.lastLuckyNumDate);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.lastLuckNum);
                packet.WriteInt(0);
                packet.WriteInt(0);
                packet.WriteBoolean(false);
                this.SendTCP(packet);
            }
        }

        public GSPacketIn SendLuckStarOpen(int ID)
        {
            GSPacketIn packet = new GSPacketIn(0x57, ID);
            packet.WriteInt(0x19);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendLuckStoneEnable(int ID)
        {
            GSPacketIn packet = new GSPacketIn(0xa5, ID);
            packet.WriteDateTime(DateTime.Now);
            packet.WriteDateTime(DateTime.Now);
            packet.WriteBoolean(false);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendMagicStonePoint(PlayerInfo player)
        {
            GSPacketIn packet = new GSPacketIn(0x102);
            packet.WriteByte(4);
            packet.WriteInt(player.MagicStonePoint);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendMailResponse(int playerID, eMailRespose type)
        {
            GSPacketIn packet = new GSPacketIn(0x75);
            packet.WriteInt(playerID);
            packet.WriteInt((int) type);
            GameServer.Instance.LoginServer.SendPacket(packet);
            return packet;
        }

        public GSPacketIn SendMarryApplyReply(GamePlayer player, int UserID, string UserName, bool result, bool isApplicant, int id)
        {
            GSPacketIn packet = new GSPacketIn(250, player.PlayerCharacter.ID);
            packet.WriteInt(UserID);
            packet.WriteBoolean(result);
            packet.WriteString(UserName);
            packet.WriteBoolean(isApplicant);
            packet.WriteInt(id);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendMarryInfo(GamePlayer player, MarryInfo info)
        {
            GSPacketIn packet = new GSPacketIn(0xeb, player.PlayerCharacter.ID);
            packet.WriteString(info.Introduction);
            packet.WriteBoolean(info.IsPublishEquip);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendMarryInfoRefresh(MarryInfo info, int ID, bool isExist)
        {
            GSPacketIn packet = new GSPacketIn(0xef);
            packet.WriteInt(ID);
            packet.WriteBoolean(isExist);
            if (isExist)
            {
                packet.WriteInt(info.UserID);
                packet.WriteBoolean(info.IsPublishEquip);
                packet.WriteString(info.Introduction);
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendMarryProp(GamePlayer player, MarryProp info)
        {
            GSPacketIn packet = new GSPacketIn(0xea, player.PlayerCharacter.ID);
            packet.WriteBoolean(info.IsMarried);
            packet.WriteInt(info.SpouseID);
            packet.WriteString(info.SpouseName);
            packet.WriteBoolean(info.IsCreatedMarryRoom);
            packet.WriteInt(info.SelfMarryRoomID);
            packet.WriteBoolean(info.IsGotRing);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendMarryRoomInfo(GamePlayer player, MarryRoom room)
        {
            GSPacketIn packet = new GSPacketIn(0xf1, player.PlayerCharacter.ID);
            bool val = room != null;
            packet.WriteBoolean(val);
            if (val)
            {
                packet.WriteInt(room.Info.ID);
                packet.WriteBoolean(room.Info.IsHymeneal);
                packet.WriteString(room.Info.Name);
                packet.WriteBoolean(!(room.Info.Pwd == ""));
                packet.WriteInt(room.Info.MapIndex);
                packet.WriteInt(room.Info.AvailTime);
                packet.WriteInt(room.Count);
                packet.WriteInt(room.Info.PlayerID);
                packet.WriteString(room.Info.PlayerName);
                packet.WriteInt(room.Info.GroomID);
                packet.WriteString(room.Info.GroomName);
                packet.WriteInt(room.Info.BrideID);
                packet.WriteString(room.Info.BrideName);
                packet.WriteDateTime(room.Info.BeginTime);
                packet.WriteByte((byte) room.RoomState);
                packet.WriteString(room.Info.RoomIntroduction);
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendMarryRoomInfoToPlayer(GamePlayer player, bool state, MarryRoomInfo info)
        {
            GSPacketIn packet = new GSPacketIn(0xfc, player.PlayerCharacter.ID);
            packet.WriteInt(info.ID);
            packet.WriteBoolean(state);
            if (state)
            {
                packet.WriteInt(info.ID);
                packet.WriteString(info.Name);
                packet.WriteInt(info.MapIndex);
                packet.WriteInt(info.AvailTime);
                packet.WriteInt(info.PlayerID);
                packet.WriteInt(info.GroomID);
                packet.WriteInt(info.BrideID);
                packet.WriteDateTime(info.BeginTime);
                packet.WriteBoolean(info.IsGunsaluteUsed);
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendMarryRoomLogin(GamePlayer player, bool result)
        {
            GSPacketIn packet = new GSPacketIn(0xf2, player.PlayerCharacter.ID);
            packet.WriteBoolean(result);
            if (result)
            {
                packet.WriteInt(player.CurrentMarryRoom.Info.ID);
                packet.WriteString(player.CurrentMarryRoom.Info.Name);
                packet.WriteInt(player.CurrentMarryRoom.Info.MapIndex);
                packet.WriteInt(player.CurrentMarryRoom.Info.AvailTime);
                packet.WriteInt(player.CurrentMarryRoom.Count);
                packet.WriteInt(player.CurrentMarryRoom.Info.PlayerID);
                packet.WriteString(player.CurrentMarryRoom.Info.PlayerName);
                packet.WriteInt(player.CurrentMarryRoom.Info.GroomID);
                packet.WriteString(player.CurrentMarryRoom.Info.GroomName);
                packet.WriteInt(player.CurrentMarryRoom.Info.BrideID);
                packet.WriteString(player.CurrentMarryRoom.Info.BrideName);
                packet.WriteDateTime(player.CurrentMarryRoom.Info.BeginTime);
                packet.WriteBoolean(player.CurrentMarryRoom.Info.IsHymeneal);
                packet.WriteByte((byte) player.CurrentMarryRoom.RoomState);
                packet.WriteString(player.CurrentMarryRoom.Info.RoomIntroduction);
                packet.WriteBoolean(player.CurrentMarryRoom.Info.GuestInvite);
                packet.WriteInt(player.MarryMap);
                packet.WriteBoolean(player.CurrentMarryRoom.Info.IsGunsaluteUsed);
            }
            this.SendTCP(packet);
            return packet;
        }

        public virtual GSPacketIn SendMessage(eMessageType type, string message)
        {
            GSPacketIn packet = new GSPacketIn(3);
            packet.WriteInt((int) type);
            packet.WriteString(message);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendMissionEnergy(UsersExtraInfo extra)
        {
            GSPacketIn packet = new GSPacketIn(0x69, extra.UserID);
            packet.WriteInt(extra.MissionEnergy);
            packet.WriteInt(extra.buyEnergyCount);
            this.SendTCP(packet);
            return packet;
        }

        public void SendMysteriousActivity(PyramidConfigInfo info)
        {
            GSPacketIn packet = new GSPacketIn(110, info.UserID);
            packet.WriteByte(0x10);
            packet.WriteInt(1);
            packet.WriteInt(2);
            packet.WriteInt(3);
            packet.WriteDateTime(DateTime.Now);
            packet.WriteDateTime(DateTime.Now.AddDays(1.0));
            this.SendTCP(packet);
        }

        public GSPacketIn SendNecklaceStrength(PlayerInfo player)
        {
            GSPacketIn packet = new GSPacketIn(0x5f, player.ID);
            packet.WriteInt(player.necklaceExp);
            packet.WriteInt(player.necklaceExpAdd);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendNetWork(GamePlayer player, long delay)
        {
            GSPacketIn packet = new GSPacketIn(6, player.PlayerId);
            packet.WriteInt((((int) delay) / 0x3e8) / 10);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendNewPacket(GamePlayer Player)
        {
            GSPacketIn packet = new GSPacketIn(0x66, Player.PlayerCharacter.ID);
            packet.WriteByte(0);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn sendOneOnOneTalk(int receiverID, bool isAutoReply, string SenderNickName, string msg, int playerid)
        {
            GSPacketIn packet = new GSPacketIn(160, playerid);
            packet.WriteByte(0x33);
            packet.WriteInt(receiverID);
            packet.WriteString(SenderNickName);
            packet.WriteDateTime(DateTime.Now);
            packet.WriteString(msg);
            packet.WriteBoolean(isAutoReply);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendOpenBoguAdventure()
        {
            GSPacketIn packet = new GSPacketIn(0x91);
            packet.WriteByte(0x59);
            packet.WriteBoolean(true);
            this.SendTCP(packet);
            return packet;
        }

        public void SendOpenDDPlay(PlayerInfo player)
        {
            DateTime date = DateTime.Parse(GameProperties.DDPlayActivityBeginDate);
            DateTime time2 = DateTime.Parse(GameProperties.DDPlayActivityEndDate);
            int dDPlayActivityMoney = GameProperties.DDPlayActivityMoney;
            GSPacketIn packet = new GSPacketIn(0x91);
            packet.WriteByte(0x4a);
            if ((DateTime.Now >= date) && (DateTime.Now <= time2))
            {
                packet.WriteBoolean(true);
            }
            else
            {
                packet.WriteBoolean(false);
            }
            packet.WriteDateTime(date);
            packet.WriteDateTime(time2);
            packet.WriteInt(dDPlayActivityMoney);
            packet.WriteInt(0);
            this.SendTCP(packet);
        }

        public GSPacketIn SendOpenGrowthPackageOpen(int ID)
        {
            GSPacketIn packet = new GSPacketIn(0x54, ID);
            packet.WriteInt(1);
            packet.WriteInt(3);
            packet.WriteBoolean(true);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendOpenHoleComplete(GamePlayer player, int type, bool result)
        {
            GSPacketIn packet = new GSPacketIn(0xd9, player.PlayerCharacter.ID);
            packet.WriteInt(type);
            packet.WriteBoolean(result);
            this.SendTCP(packet);
            return packet;
        }

        public void SendOpenLightRoad()
        {
            GSPacketIn packet = new GSPacketIn(0x91);
            packet.WriteByte(0x40);
            packet.WriteBoolean(true);
            this.SendTCP(packet);
        }

        public void SendOpenNoviceActive(int channel, int activeId, int condition, int awardGot, DateTime startTime, DateTime endTime)
        {
            GSPacketIn packet = new GSPacketIn(0x102);
            packet.WriteInt(channel);
            if (channel != 0)
            {
                if (channel == 1)
                {
                    packet.WriteBoolean(false);
                }
            }
            else
            {
                packet.WriteInt(activeId);
                packet.WriteInt(condition);
                packet.WriteInt(awardGot);
                packet.WriteDateTime(startTime);
                packet.WriteDateTime(endTime);
            }
            this.SendTCP(packet);
        }

        public void SendOpenOrCloseChristmas(int lastPacks, bool isOpen)
        {
            GSPacketIn packet = new GSPacketIn(0x91);
            packet.WriteByte(0x10);
            packet.WriteBoolean(isOpen);
            if (isOpen)
            {
                DateTime date = DateTime.Parse(GameProperties.ChristmasBeginDate);
                DateTime time2 = DateTime.Parse(GameProperties.ChristmasEndDate);
                packet.WriteDateTime(date);
                packet.WriteDateTime(time2);
                string[] strArray = GameProperties.ChristmasGifts.Split(new char[] { '|' });
                packet.WriteInt(strArray.Length);
                for (int i = 0; i < strArray.Length; i++)
                {
                    string[] strArray2 = strArray[i].Split(new char[] { ',' });
                    packet.WriteInt(int.Parse(strArray2[0]));
                    packet.WriteInt(int.Parse(strArray2[1]));
                }
                packet.WriteInt(lastPacks);
                packet.WriteInt(GameProperties.ChristmasBuildSnowmanDoubleMoney);
            }
            this.SendTCP(packet);
        }

        public GSPacketIn SendOpenTimeBox(int condtion, bool isSuccess)
        {
            GSPacketIn packet = new GSPacketIn(0x35);
            packet.WriteBoolean(isSuccess);
            packet.WriteInt(condtion);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendOpenVIP(PlayerInfo Player)
        {
            GSPacketIn packet = new GSPacketIn(0x5c, Player.ID);
            packet.WriteByte(Player.typeVIP);
            packet.WriteInt(Player.VIPLevel);
            packet.WriteInt(Player.VIPExp);
            packet.WriteDateTime(Player.VIPExpireDay);
            packet.WriteDateTime(Player.LastVIPPackTime);
            packet.WriteInt(Player.VIPNextLevelDaysNeeded);
            packet.WriteBoolean(Player.CanTakeVipReward);
            this.SendTCP(packet);
            return packet;
        }

        public void SendOpenWorldBoss(int pX, int pY)
        {
        }

        public GSPacketIn SendPayFields(GamePlayer Player, List<int> fieldIds)
        {
            GSPacketIn packet = new GSPacketIn(0x51, Player.PlayerCharacter.ID);
            packet.WriteByte(6);
            packet.WriteInt(Player.PlayerCharacter.ID);
            packet.WriteInt(fieldIds.Count);
            foreach (int num in fieldIds)
            {
                UserFieldInfo fieldAt = Player.Farm.GetFieldAt(num);
                packet.WriteInt(fieldAt.FieldID);
                packet.WriteInt(fieldAt.SeedID);
                packet.WriteDateTime(fieldAt.PayTime);
                packet.WriteDateTime(fieldAt.PlantTime);
                packet.WriteInt(fieldAt.GainCount);
                packet.WriteInt(fieldAt.FieldValidDate);
                packet.WriteInt(fieldAt.AccelerateTime);
            }
            this.SendTCP(packet);
            return packet;
        }

        public void SendPetGuildOptionChange()
        {
            GSPacketIn packet = new GSPacketIn(0x9e);
            packet.WriteBoolean(true);
            packet.WriteInt(8);
            this.SendTCP(packet);
        }

        public GSPacketIn SendPetInfo(PlayerInfo info, UsersPetinfo[] pets)
        {
            GSPacketIn packet = new GSPacketIn(0x44, info.ID);
            packet.WriteByte(1);
            packet.WriteInt(info.ID);
            packet.WriteInt(pets.Length);
            for (int i = 0; i < pets.Length; i++)
            {
                UsersPetinfo petinfo = pets[i];
                packet.WriteInt(petinfo.Place);
                packet.WriteBoolean(true);
                packet.WriteInt(petinfo.ID);
                packet.WriteInt(petinfo.TemplateID);
                packet.WriteString(petinfo.Name);
                packet.WriteInt(petinfo.UserID);
                packet.WriteInt(petinfo.TotalAttack);
                packet.WriteInt(petinfo.TotalDefence);
                packet.WriteInt(petinfo.TotalLuck);
                packet.WriteInt(petinfo.TotalAgility);
                packet.WriteInt(petinfo.TotalBlood);
                packet.WriteInt(petinfo.TotalDamage);
                packet.WriteInt(petinfo.TotalGuard);
                packet.WriteInt(petinfo.AttackGrow);
                packet.WriteInt(petinfo.DefenceGrow);
                packet.WriteInt(petinfo.LuckGrow);
                packet.WriteInt(petinfo.AgilityGrow);
                packet.WriteInt(petinfo.BloodGrow);
                packet.WriteInt(petinfo.DamageGrow);
                packet.WriteInt(petinfo.GuardGrow);
                packet.WriteInt(petinfo.Level);
                packet.WriteInt(petinfo.GP);
                packet.WriteInt(petinfo.MaxGP);
                packet.WriteInt(petinfo.Hunger);
                packet.WriteInt(petinfo.PetHappyStar);
                packet.WriteInt(petinfo.MP);
                List<string> skill = petinfo.GetSkill();
                List<string> skillEquip = petinfo.GetSkillEquip();
                packet.WriteInt(skill.Count);
                foreach (string str in skill)
                {
                    packet.WriteInt(int.Parse(str.Split(new char[] { ',' })[0]));
                    packet.WriteInt(int.Parse(str.Split(new char[] { ',' })[1]));
                }
                packet.WriteInt(skillEquip.Count);
                foreach (string str in skillEquip)
                {
                    packet.WriteInt(int.Parse(str.Split(new char[] { ',' })[1]));
                    packet.WriteInt(int.Parse(str.Split(new char[] { ',' })[0]));
                }
                packet.WriteBoolean(petinfo.IsEquip);
                List<PetEquipDataInfo> equip = petinfo.GetEquip();
                packet.WriteInt(equip.Count);
                for (int j = 0; j < equip.Count; j++)
                {
                    packet.WriteInt(equip[j].eqType);
                    packet.WriteInt(equip[j].eqTemplateID);
                    packet.WriteDateTime(equip[j].startTime);
                    packet.WriteInt(equip[j].ValidDate);
                }
                packet.WriteInt(petinfo.currentStarExp);
            }
            this.SendTCP(packet);
            return packet;
        }

        public void SendPingTime(GamePlayer player)
        {
            GSPacketIn packet = new GSPacketIn(4);
            player.PingStart = DateTime.Now.Ticks;
            packet.WriteInt(player.PlayerCharacter.AntiAddiction);
            this.SendTCP(packet);
        }

        public void SendPlayerCardEquip(PlayerInfo player, List<UsersCardInfo> cards)
        {
            if (this.m_gameClient.Player != null)
            {
                GSPacketIn packet = new GSPacketIn(0xd8, this.m_gameClient.Player.PlayerCharacter.ID);
                packet.WriteInt(player.ID);
                packet.WriteInt(cards.Count);
                foreach (UsersCardInfo info in cards)
                {
                    packet.WriteInt(1);
                    packet.WriteBoolean(true);
                    packet.WriteInt(info.CardID);
                    packet.WriteInt(info.UserID);
                    packet.WriteInt(info.Count);
                    packet.WriteInt(info.Place);
                    packet.WriteInt(info.TemplateID);
                    packet.WriteInt(info.Attack);
                    packet.WriteInt(info.Defence);
                    packet.WriteInt(info.Agility);
                    packet.WriteInt(info.Luck);
                    packet.WriteInt(info.Damage);
                    packet.WriteInt(info.Guard);
                    packet.WriteInt(info.Level);
                    packet.WriteInt(info.CardGP);
                    packet.WriteBoolean(info.isFirstGet);
                }
                this.SendTCP(packet);
            }
        }

        public void SendPlayerCardInfo(CardInventory bag, int[] updatedSlots)
        {
            if (this.m_gameClient.Player != null)
            {
                GSPacketIn packet = new GSPacketIn(0xd8, this.m_gameClient.Player.PlayerCharacter.ID);
                packet.WriteInt(this.m_gameClient.Player.PlayerCharacter.ID);
                packet.WriteInt(updatedSlots.Length);
                for (int i = 0; i < updatedSlots.Length; i++)
                {
                    int val = updatedSlots[i];
                    packet.WriteInt(val);
                    UsersCardInfo itemAt = bag.GetItemAt(val);
                    if (itemAt == null)
                    {
                        packet.WriteBoolean(false);
                    }
                    else
                    {
                        packet.WriteBoolean(true);
                        packet.WriteInt(itemAt.CardID);
                        packet.WriteInt(itemAt.UserID);
                        packet.WriteInt(itemAt.Count);
                        packet.WriteInt(itemAt.Place);
                        packet.WriteInt(itemAt.TemplateID);
                        packet.WriteInt(itemAt.Attack);
                        packet.WriteInt(itemAt.Defence);
                        packet.WriteInt(itemAt.Agility);
                        packet.WriteInt(itemAt.Luck);
                        packet.WriteInt(itemAt.Damage);
                        packet.WriteInt(itemAt.Guard);
                        packet.WriteInt(itemAt.Level);
                        packet.WriteInt(itemAt.CardGP);
                        packet.WriteBoolean(itemAt.isFirstGet);
                    }
                }
                this.SendTCP(packet);
            }
        }

        public GSPacketIn SendPlayerCardReset(PlayerInfo player, List<int> points)
        {
            GSPacketIn packet = new GSPacketIn(0xc4, player.ID);
            packet.WriteInt(points.Count);
            foreach (int num in points)
            {
                packet.WriteInt(num);
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendPlayerCardSlot(PlayerInfo player, UsersCardInfo card)
        {
            GSPacketIn packet = new GSPacketIn(170, player.ID);
            packet.WriteInt(player.ID);
            packet.WriteInt(player.CardSoul);
            packet.WriteInt(1);
            packet.WriteInt(card.Place);
            packet.WriteInt(card.Level);
            packet.WriteInt(card.CardGP);
            this.SendTCP(packet);
            return packet;
        }

        public void SendPlayerCardSlot(PlayerInfo player, List<UsersCardInfo> cardslots)
        {
            if (this.m_gameClient.Player != null)
            {
                GSPacketIn packet = new GSPacketIn(0xd8, this.m_gameClient.Player.PlayerCharacter.ID);
                List<UsersCardInfo> list = new List<UsersCardInfo>();
                foreach (UsersCardInfo info in cardslots)
                {
                    packet.WriteInt(info.CardID);
                    packet.WriteInt(info.UserID);
                    packet.WriteInt(info.Count);
                    packet.WriteInt(info.Place);
                    packet.WriteInt(info.TemplateID);
                    packet.WriteInt(info.Attack);
                    packet.WriteInt(info.Defence);
                    packet.WriteInt(info.Agility);
                    packet.WriteInt(info.Luck);
                    packet.WriteInt(info.Damage);
                    packet.WriteInt(info.Guard);
                    packet.WriteInt(info.Level);
                    packet.WriteInt(info.CardGP);
                    packet.WriteBoolean(info.isFirstGet);
                    if (info.TemplateID > 0)
                    {
                        list.Add(info);
                    }
                }
                this.SendTCP(packet);
            }
        }

        public GSPacketIn SendPlayerCardSoul(PlayerInfo player, bool isSoul, int soul)
        {
            GSPacketIn packet = new GSPacketIn(0xd0, player.ID);
            packet.WriteBoolean(isSoul);
            if (isSoul)
            {
                packet.WriteInt(soul);
                packet.WriteInt(player.GetSoulCount);
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendPlayerDivorceApply(GamePlayer player, bool result, bool isProposer)
        {
            GSPacketIn packet = new GSPacketIn(0xf8, player.PlayerCharacter.ID);
            packet.WriteBoolean(result);
            packet.WriteBoolean(isProposer);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendPlayerDrill(int ID, Dictionary<int, UserDrillInfo> drills)
        {
            GSPacketIn packet = new GSPacketIn(0x79, ID);
            packet.WriteByte(6);
            packet.WriteInt(ID);
            packet.WriteInt(drills[0].HoleExp);
            packet.WriteInt(drills[1].HoleExp);
            packet.WriteInt(drills[2].HoleExp);
            packet.WriteInt(drills[0].HoleExp);
            packet.WriteInt(drills[1].HoleExp);
            packet.WriteInt(drills[2].HoleExp);
            packet.WriteInt(drills[0].HoleLv);
            packet.WriteInt(drills[1].HoleLv);
            packet.WriteInt(drills[2].HoleLv);
            packet.WriteInt(drills[0].HoleLv);
            packet.WriteInt(drills[1].HoleLv);
            packet.WriteInt(drills[2].HoleLv);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendPlayerEnterMarryRoom(GamePlayer player)
        {
            GSPacketIn packet = new GSPacketIn(0xf3, player.PlayerCharacter.ID);
            packet.WriteInt(player.PlayerCharacter.Grade);
            packet.WriteInt(player.PlayerCharacter.Hide);
            packet.WriteInt(player.PlayerCharacter.Repute);
            packet.WriteInt(player.PlayerCharacter.ID);
            packet.WriteString(player.PlayerCharacter.NickName);
            packet.WriteByte(player.PlayerCharacter.typeVIP);
            packet.WriteInt(player.PlayerCharacter.VIPLevel);
            packet.WriteBoolean(player.PlayerCharacter.Sex);
            packet.WriteString(player.PlayerCharacter.Style);
            packet.WriteString(player.PlayerCharacter.Colors);
            packet.WriteString(player.PlayerCharacter.Skin);
            packet.WriteInt(player.X);
            packet.WriteInt(player.Y);
            packet.WriteInt(player.PlayerCharacter.FightPower);
            packet.WriteInt(player.PlayerCharacter.Win);
            packet.WriteInt(player.PlayerCharacter.Total);
            packet.WriteInt(player.PlayerCharacter.Offer);
            packet.WriteBoolean(player.PlayerCharacter.IsOldPlayer);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendPlayerFigSpiritinit(int ID, List<UserGemStone> gems)
        {
            GSPacketIn packet = new GSPacketIn(0xd1, ID);
            packet.WriteByte(1);
            packet.WriteBoolean(true);
            packet.WriteInt(gems.Count);
            foreach (UserGemStone stone in gems)
            {
                packet.WriteInt(stone.UserID);
                packet.WriteInt(stone.FigSpiritId);
                packet.WriteString(stone.FigSpiritIdValue);
                packet.WriteInt(stone.EquipPlace);
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendPlayerFigSpiritUp(int ID, UserGemStone gem, bool isUp, bool isMaxLevel, bool isFall, int num, int dir)
        {
            GSPacketIn packet = new GSPacketIn(0xd1, ID);
            packet.WriteByte(2);
            string[] strArray = gem.FigSpiritIdValue.Split(new char[] { '|' });
            packet.WriteBoolean(isUp);
            packet.WriteBoolean(isMaxLevel);
            packet.WriteBoolean(isFall);
            packet.WriteInt(num);
            packet.WriteInt(strArray.Length);
            for (int i = 0; i < strArray.Length; i++)
            {
                string str = strArray[i];
                packet.WriteInt(gem.FigSpiritId);
                packet.WriteInt(Convert.ToInt32(str.Split(new char[] { ',' })[0]));
                packet.WriteInt(Convert.ToInt32(str.Split(new char[] { ',' })[1]));
                packet.WriteInt(Convert.ToInt32(str.Split(new char[] { ',' })[2]));
            }
            packet.WriteInt(gem.EquipPlace);
            packet.WriteInt(dir);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendPlayerLeaveMarryRoom(GamePlayer player)
        {
            GSPacketIn packet = new GSPacketIn(0xf4, player.PlayerCharacter.ID);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendPlayerMarryApply(GamePlayer player, int userID, string userName, string loveProclamation, int id)
        {
            GSPacketIn packet = new GSPacketIn(0xf7, player.PlayerCharacter.ID);
            packet.WriteInt(userID);
            packet.WriteString(userName);
            packet.WriteString(loveProclamation);
            packet.WriteInt(id);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendPlayerMarryStatus(GamePlayer player, int userID, bool isMarried)
        {
            GSPacketIn packet = new GSPacketIn(0xf6, player.PlayerCharacter.ID);
            packet.WriteInt(userID);
            packet.WriteBoolean(isMarried);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendPlayerRefreshTotem(PlayerInfo player)
        {
            GSPacketIn packet = new GSPacketIn(0x88, player.ID);
            packet.WriteInt(1);
            packet.WriteInt(player.myHonor);
            packet.WriteInt(player.totemId);
            this.SendTCP(packet);
            return packet;
        }

        public void SendPyramidOpenClose(PyramidConfigInfo info)
        {
            GSPacketIn packet = new GSPacketIn(0x91, info.UserID);
            packet.WriteByte(0);
            packet.WriteBoolean(info.isOpen);
            packet.WriteBoolean(info.isScoreExchange);
            packet.WriteDateTime(info.beginTime);
            packet.WriteDateTime(info.endTime);
            packet.WriteInt(info.freeCount);
            packet.WriteInt(info.turnCardPrice);
            packet.WriteInt(info.revivePrice.Length);
            for (int i = 0; i < info.revivePrice.Length; i++)
            {
                packet.WriteInt(info.revivePrice[i]);
            }
            this.SendTCP(packet);
        }

        public void SendQQtips(int UserID, QQtipsMessagesInfo QQTips)
        {
            if (QQTips != null)
            {
                GSPacketIn packet = new GSPacketIn(0x63, UserID);
                packet.WriteString(QQTips.title);
                packet.WriteString(QQTips.content);
                packet.WriteInt(QQTips.maxLevel);
                packet.WriteInt(QQTips.minLevel);
                packet.WriteInt(QQTips.outInType);
                if (QQTips.outInType == 0)
                {
                    packet.WriteInt(QQTips.moduleType);
                    packet.WriteInt(QQTips.inItemID);
                }
                else
                {
                    packet.WriteString(QQTips.url);
                }
                this.SendTCP(packet);
            }
        }

        public void SendReady()
        {
            this.SendTCP(new GSPacketIn(0));
        }

        public GSPacketIn SendRefineryPreview(GamePlayer player, int templateid, bool isbind, SqlDataProvider.Data.ItemInfo item)
        {
            GSPacketIn packet = new GSPacketIn(0x6f, player.PlayerCharacter.ID);
            packet.WriteInt(templateid);
            packet.WriteInt(item.ValidDate);
            packet.WriteBoolean(isbind);
            packet.WriteInt(item.AgilityCompose);
            packet.WriteInt(item.AttackCompose);
            packet.WriteInt(item.DefendCompose);
            packet.WriteInt(item.LuckCompose);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendRefreshPet(GamePlayer player, UsersPetinfo[] pets, SqlDataProvider.Data.ItemInfo[] items, bool refreshBtn)
        {
            int num4;
            UsersPetinfo petinfo;
            List<string> skill;
            GSPacketIn packet = new GSPacketIn(0x44, player.PlayerCharacter.ID);
            packet.WriteByte(5);
            int val = 10;
            int num2 = 10;
            int num3 = 100;
            if (!player.PlayerCharacter.IsFistGetPet)
            {
                packet.WriteBoolean(refreshBtn);
                packet.WriteInt(pets.Length);
                for (num4 = 0; num4 < pets.Length; num4++)
                {
                    petinfo = pets[num4];
                    packet.WriteInt(petinfo.Place);
                    packet.WriteInt(petinfo.TemplateID);
                    packet.WriteString(petinfo.Name);
                    packet.WriteInt(petinfo.Attack);
                    packet.WriteInt(petinfo.Defence);
                    packet.WriteInt(petinfo.Luck);
                    packet.WriteInt(petinfo.Agility);
                    packet.WriteInt(petinfo.Blood);
                    packet.WriteInt(petinfo.Damage);
                    packet.WriteInt(petinfo.Guard);
                    packet.WriteInt(petinfo.AttackGrow);
                    packet.WriteInt(petinfo.DefenceGrow);
                    packet.WriteInt(petinfo.LuckGrow);
                    packet.WriteInt(petinfo.AgilityGrow);
                    packet.WriteInt(petinfo.BloodGrow);
                    packet.WriteInt(petinfo.DamageGrow);
                    packet.WriteInt(petinfo.GuardGrow);
                    packet.WriteInt(petinfo.Level);
                    packet.WriteInt(petinfo.GP);
                    packet.WriteInt(petinfo.MaxGP);
                    packet.WriteInt(petinfo.Hunger);
                    packet.WriteInt(petinfo.MP);
                    skill = petinfo.GetSkill();
                    packet.WriteInt(skill.Count);
                    foreach (string str in skill)
                    {
                        packet.WriteInt(int.Parse(str.Split(new char[] { ',' })[0]));
                        packet.WriteInt(int.Parse(str.Split(new char[] { ',' })[1]));
                    }
                    packet.WriteInt(val);
                    packet.WriteInt(num2);
                    packet.WriteInt(num3);
                }
                packet.WriteInt(0);
            }
            else
            {
                packet.WriteBoolean(refreshBtn);
                packet.WriteInt(pets.Length);
                for (num4 = 0; num4 < pets.Length; num4++)
                {
                    petinfo = pets[num4];
                    packet.WriteInt(petinfo.Place);
                    packet.WriteInt(petinfo.TemplateID);
                    packet.WriteString(petinfo.Name);
                    packet.WriteInt(petinfo.Attack);
                    packet.WriteInt(petinfo.Defence);
                    packet.WriteInt(petinfo.Luck);
                    packet.WriteInt(petinfo.Agility);
                    packet.WriteInt(petinfo.Blood);
                    packet.WriteInt(petinfo.Damage);
                    packet.WriteInt(petinfo.Guard);
                    packet.WriteInt(petinfo.AttackGrow);
                    packet.WriteInt(petinfo.DefenceGrow);
                    packet.WriteInt(petinfo.LuckGrow);
                    packet.WriteInt(petinfo.AgilityGrow);
                    packet.WriteInt(petinfo.BloodGrow);
                    packet.WriteInt(petinfo.DamageGrow);
                    packet.WriteInt(petinfo.GuardGrow);
                    packet.WriteInt(petinfo.Level);
                    packet.WriteInt(petinfo.GP);
                    packet.WriteInt(petinfo.MaxGP);
                    packet.WriteInt(petinfo.Hunger);
                    packet.WriteInt(petinfo.MP);
                    skill = petinfo.GetSkill();
                    packet.WriteInt(skill.Count);
                    foreach (string str in skill)
                    {
                        packet.WriteInt(int.Parse(str.Split(new char[] { ',' })[0]));
                        packet.WriteInt(int.Parse(str.Split(new char[] { ',' })[1]));
                    }
                    packet.WriteInt(val);
                    packet.WriteInt(num2);
                    packet.WriteInt(num3);
                }
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendRoomCreate(BaseRoom room)
        {
            GSPacketIn packet = new GSPacketIn(0x5e);
            packet.WriteInt(room.RoomId);
            packet.WriteByte((byte) room.RoomType);
            packet.WriteByte((byte) room.HardLevel);
            packet.WriteByte(room.TimeMode);
            packet.WriteByte((byte) room.PlayerCount);
            packet.WriteByte((byte) room.viewerCnt);
            packet.WriteByte((byte) room.PlacesCount);
            packet.WriteBoolean(!string.IsNullOrEmpty(room.Password));
            packet.WriteInt(room.MapId);
            packet.WriteBoolean(room.IsPlaying);
            packet.WriteString(room.Name);
            packet.WriteByte((byte) room.GameType);
            packet.WriteInt(room.LevelLimits);
            packet.WriteBoolean(room.isCrosszone);
            packet.WriteBoolean(room.isWithinLeageTime);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendRoomLoginResult(bool result)
        {
            GSPacketIn packet = new GSPacketIn(0x51);
            packet.WriteBoolean(result);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendRoomPairUpCancel(BaseRoom room)
        {
            GSPacketIn packet = new GSPacketIn(210);
            packet.WriteByte(11);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendRoomPairUpStart(BaseRoom room)
        {
            GSPacketIn packet = new GSPacketIn(0xd0);
            packet.WriteByte(13);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendRoomPlayerAdd(GamePlayer player)
        {
            GSPacketIn packet = new GSPacketIn(0x52, player.PlayerId);
            bool val = false;
            if (player.CurrentRoom.Game != null)
            {
                val = true;
            }
            packet.WriteBoolean(val);
            packet.WriteByte((byte) player.CurrentRoomIndex);
            packet.WriteByte((byte) player.CurrentRoomTeam);
            packet.WriteBoolean(false);
            packet.WriteInt(player.PlayerCharacter.Grade);
            packet.WriteInt(player.PlayerCharacter.Offer);
            packet.WriteInt(player.PlayerCharacter.Hide);
            packet.WriteInt(player.PlayerCharacter.Repute);
            packet.WriteInt((((int) player.PingTime) / 0x3e8) / 10);
            packet.WriteInt(player.ZoneId);
            packet.WriteInt(player.PlayerCharacter.ID);
            packet.WriteString(player.PlayerCharacter.NickName);
            packet.WriteByte(player.PlayerCharacter.typeVIP);
            packet.WriteInt(player.PlayerCharacter.VIPLevel);
            packet.WriteBoolean(player.PlayerCharacter.Sex);
            packet.WriteString(player.PlayerCharacter.Style);
            packet.WriteString(player.PlayerCharacter.Colors);
            packet.WriteString(player.PlayerCharacter.Skin);
            SqlDataProvider.Data.ItemInfo itemAt = player.EquipBag.GetItemAt(6);
            packet.WriteInt((itemAt == null) ? 0x1b60 : itemAt.TemplateID);
            if (player.SecondWeapon == null)
            {
                packet.WriteInt(0);
            }
            else
            {
                packet.WriteInt(player.SecondWeapon.TemplateID);
            }
            packet.WriteInt(player.PlayerCharacter.ConsortiaID);
            packet.WriteString(player.PlayerCharacter.ConsortiaName);
            packet.WriteInt(player.PlayerCharacter.badgeID);
            packet.WriteInt(player.PlayerCharacter.Win);
            packet.WriteInt(player.PlayerCharacter.Total);
            packet.WriteInt(player.PlayerCharacter.Escape);
            packet.WriteInt(0);
            packet.WriteInt(0);
            packet.WriteBoolean(player.PlayerCharacter.IsMarried);
            if (player.PlayerCharacter.IsMarried)
            {
                packet.WriteInt(player.PlayerCharacter.SpouseID);
                packet.WriteString(player.PlayerCharacter.SpouseName);
            }
            packet.WriteString(player.PlayerCharacter.UserName);
            packet.WriteInt(player.PlayerCharacter.Nimbus);
            packet.WriteInt(player.PlayerCharacter.FightPower);
            packet.WriteInt(0);
            packet.WriteInt(0);
            packet.WriteString("Master");
            packet.WriteInt(0);
            packet.WriteString("HonorOfMaster");
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendRoomPlayerChangedTeam(GamePlayer player)
        {
            GSPacketIn packet = new GSPacketIn(0x66, player.PlayerId);
            packet.WriteByte((byte) player.CurrentRoomTeam);
            packet.WriteByte((byte) player.CurrentRoomIndex);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendRoomPlayerRemove(GamePlayer player)
        {
            GSPacketIn packet = new GSPacketIn(0x53, player.PlayerId) {
                Parameter1 = player.PlayerId
            };
            packet.WriteInt(4);
            packet.WriteInt(4);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendRoomType(GamePlayer player, BaseRoom game)
        {
            GSPacketIn packet = new GSPacketIn(0xd3);
            packet.WriteByte((byte) game.GameStyle);
            packet.WriteInt((int) game.GameType);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendRoomUpdatePlacesStates(int[] states)
        {
            GSPacketIn packet = new GSPacketIn(100);
            for (int i = 0; i < states.Length; i++)
            {
                packet.WriteInt(states[i]);
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendRoomUpdatePlayerStates(byte[] states)
        {
            GSPacketIn packet = new GSPacketIn(0x57);
            for (int i = 0; i < states.Length; i++)
            {
                packet.WriteByte(states[i]);
            }
            this.SendTCP(packet);
            return packet;
        }

        public void SendRSAKey(byte[] m, byte[] e)
        {
            GSPacketIn packet = new GSPacketIn(7);
            packet.Write(m);
            packet.Write(e);
            this.SendTCP(packet);
        }

        public GSPacketIn SendRuneOpenPackage(GamePlayer player, int rand)
        {
            GSPacketIn packet = new GSPacketIn(0x79, player.PlayerCharacter.ID);
            packet.WriteByte(3);
            packet.WriteInt(rand);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendSceneAddPlayer(GamePlayer player)
        {
            GSPacketIn packet = new GSPacketIn(0x12, player.PlayerCharacter.ID);
            packet.WriteInt(player.PlayerCharacter.Grade);
            packet.WriteBoolean(player.PlayerCharacter.Sex);
            packet.WriteString(player.PlayerCharacter.NickName);
            packet.WriteByte(player.PlayerCharacter.typeVIP);
            packet.WriteInt(player.PlayerCharacter.VIPLevel);
            packet.WriteString(player.PlayerCharacter.ConsortiaName);
            packet.WriteInt(player.PlayerCharacter.Offer);
            packet.WriteInt(player.PlayerCharacter.Win);
            packet.WriteInt(player.PlayerCharacter.Total);
            packet.WriteInt(player.PlayerCharacter.Escape);
            packet.WriteInt(player.PlayerCharacter.ConsortiaID);
            packet.WriteInt(player.PlayerCharacter.Repute);
            packet.WriteBoolean(player.PlayerCharacter.IsMarried);
            if (player.PlayerCharacter.IsMarried)
            {
                packet.WriteInt(player.PlayerCharacter.SpouseID);
                packet.WriteString(player.PlayerCharacter.SpouseName);
            }
            packet.WriteString(player.PlayerCharacter.UserName);
            packet.WriteInt(player.PlayerCharacter.FightPower);
            packet.WriteInt(5);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendSceneRemovePlayer(GamePlayer player)
        {
            GSPacketIn packet = new GSPacketIn(0x15, player.PlayerCharacter.ID);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendSeeding(PlayerInfo Player, UserFieldInfo field)
        {
            GSPacketIn packet = new GSPacketIn(0x51, Player.ID);
            packet.WriteByte(2);
            packet.WriteInt(field.FieldID);
            packet.WriteInt(field.SeedID);
            packet.WriteDateTime(field.PlantTime);
            packet.WriteDateTime(field.PayTime);
            packet.WriteInt(field.GainCount);
            packet.WriteInt(field.FieldValidDate);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendSevenDoubleOpenClose(int ID, bool result)
        {
            GSPacketIn packet = new GSPacketIn(0x94, ID);
            packet.WriteByte(1);
            packet.WriteBoolean(result);
            if (result)
            {
                int num;
                packet.WriteBoolean(false);
                packet.WriteInt(3);
                packet.WriteInt(0x21);
                packet.WriteInt(0);
                packet.WriteInt(3);
                for (num = 0; num < 3; num++)
                {
                    packet.WriteInt(num);
                    packet.WriteInt(0xbb8);
                    packet.WriteInt(3 * num);
                    packet.WriteInt(1);
                    packet.WriteInt(0x18704);
                    packet.WriteInt(10 * num);
                }
                packet.WriteInt(11);
                packet.WriteInt(0x16);
                packet.WriteInt(0x21);
                packet.WriteInt(5);
                for (num = 0; num < 5; num++)
                {
                    packet.WriteInt(num);
                }
                packet.WriteInt(1);
                packet.WriteInt(1);
                packet.WriteInt(0x2c);
                packet.WriteString("10:20|11:10");
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendSingleRoomCreate(BaseRoom room)
        {
            GSPacketIn packet = new GSPacketIn(0x5e);
            packet.WriteByte(0x12);
            packet.WriteInt(room.RoomId);
            packet.WriteByte((byte) room.RoomType);
            packet.WriteBoolean(room.IsPlaying);
            packet.WriteByte((byte) room.GameType);
            packet.WriteInt(room.MapId);
            packet.WriteBoolean(room.isCrosszone);
            packet.WriteInt(room.ZoneId);
            packet.WriteBoolean(false);
            this.SendTCP(packet);
            return packet;
        }

        public void SendSuperWinnerOpen(int playerID, bool isOpen)
        {
            GSPacketIn packet = new GSPacketIn(0x91, playerID);
            packet.WriteByte(0x30);
            packet.WriteBoolean(isOpen);
            this.SendTCP(packet);
        }

        public void SendTCP(GSPacketIn packet)
        {
            this.m_gameClient.SendTCP(packet);
        }

        public GSPacketIn SendtoGather(PlayerInfo Player, UserFieldInfo field)
        {
            GSPacketIn packet = new GSPacketIn(0x51, Player.ID);
            packet.WriteByte(4);
            packet.WriteBoolean(true);
            packet.WriteInt(field.FieldID);
            packet.WriteInt(field.SeedID);
            packet.WriteDateTime(field.PlantTime);
            packet.WriteInt(field.GainCount);
            packet.WriteInt(field.AccelerateTime);
            this.SendTCP(packet);
            return packet;
        }

        public void SendTreasureHunting(PyramidConfigInfo info)
        {
            GSPacketIn packet = new GSPacketIn(110, info.UserID);
            packet.WriteByte(1);
            packet.WriteBoolean(true);
            packet.WriteBoolean(true);
            packet.WriteDateTime(DateTime.Now);
            packet.WriteDateTime(DateTime.Now.AddDays(1.0));
            packet.WriteInt(0x2710);
            this.SendTCP(packet);
            this.SendMysteriousActivity(info);
        }

        public GSPacketIn SendTrusteeshipStart(int ID)
        {
            GSPacketIn packet = new GSPacketIn(140, ID);
            packet.WriteByte(1);
            packet.WriteInt(0);
            packet.WriteInt(0);
            for (int i = 0; i < 0; i++)
            {
                packet.WriteInt(0);
                packet.WriteDateTime(DateTime.Now);
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendUpdateAchievementInfo(List<AchievementProcessInfo> process)
        {
            GSPacketIn packet = new GSPacketIn(0xe5);
            packet.WriteInt(process.Count);
            foreach (AchievementProcessInfo info in process)
            {
                packet.WriteInt(info.CondictionType);
                packet.WriteInt(info.Value);
                info.IsDirty = false;
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendUpdateAchievements(GamePlayer player, BaseAchievement[] infos)
        {
            if ((player == null) || (infos == null))
            {
                return null;
            }
            GSPacketIn packet = new GSPacketIn(0xe4, player.PlayerCharacter.ID);
            packet.WriteInt(infos.Length);
            for (int i = 0; i < infos.Length; i++)
            {
                BaseAchievement achievement = infos[i];
                packet.WriteInt(achievement.Data.AchievementID);
                packet.WriteInt(1);
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendUpdateAllData(PlayerInfo player)
        {
            GSPacketIn packet = new GSPacketIn(0x8e, player.ID);
            packet.WriteInt(0);
            packet.WriteDateTime(DateTime.Now.AddDays(7.0));
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendUpdateBuffer(GamePlayer player, BufferInfo[] infos)
        {
            GSPacketIn packet = new GSPacketIn(0xb9, player.PlayerId);
            packet.WriteInt(infos.Length);
            for (int i = 0; i < infos.Length; i++)
            {
                BufferInfo info = infos[i];
                packet.WriteInt(info.Type);
                packet.WriteBoolean(info.IsExist);
                packet.WriteDateTime(info.BeginDate);
                packet.WriteInt(info.ValidDate);
                packet.WriteInt(info.Value);
                packet.WriteInt(info.ValidCount);
                packet.WriteInt(info.TemplateID);
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendUpdateConsotiaBuffer(GamePlayer player, Dictionary<int, BufferInfo> bufflist)
        {
            List<ConsortiaBuffTempInfo> allConsortiaBuff = ConsortiaExtraMgr.GetAllConsortiaBuff();
            GSPacketIn packet = new GSPacketIn(0x81, player.PlayerId);
            packet.WriteByte(0x1a);
            packet.WriteInt(allConsortiaBuff.Count);
            foreach (ConsortiaBuffTempInfo info in allConsortiaBuff)
            {
                if (bufflist.ContainsKey(info.id))
                {
                    BufferInfo info2 = bufflist[info.id];
                    packet.WriteInt(info2.TemplateID);
                    packet.WriteBoolean(true);
                    packet.WriteDateTime(info2.BeginDate);
                    packet.WriteInt((info2.ValidDate / 0x18) / 60);
                }
                else
                {
                    packet.WriteInt(info.id);
                    packet.WriteBoolean(false);
                    packet.WriteDateTime(DateTime.Now);
                    packet.WriteInt(0);
                }
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendUpdateGoodsCount(PlayerInfo player, ShopItemInfo[] BagList, ShopItemInfo[] ConsoList)
        {
            GSPacketIn packet = new GSPacketIn(0xa8, player.ID);
            packet.WriteInt(0);
            packet.WriteInt(player.ConsortiaID);
            packet.WriteInt(0);
            packet.WriteInt(0);
            this.SendTCP(packet);
            return packet;
        }

        public void SendUpdateInventorySlot(PlayerInventory bag, int[] updatedSlots)
        {
            if (this.m_gameClient.Player != null)
            {
                GSPacketIn packet = new GSPacketIn(0x40, this.m_gameClient.Player.PlayerCharacter.ID, 0x2800);
                packet.WriteInt(bag.BagType);
                packet.WriteInt(updatedSlots.Length);
                foreach (int num in updatedSlots)
                {
                    packet.WriteInt(num);
                    SqlDataProvider.Data.ItemInfo itemAt = bag.GetItemAt(num);
                    if (itemAt == null)
                    {
                        packet.WriteBoolean(false);
                    }
                    else
                    {
                        packet.WriteBoolean(true);
                        packet.WriteInt(itemAt.UserID);
                        packet.WriteInt(itemAt.ItemID);
                        packet.WriteInt(itemAt.Count);
                        packet.WriteInt(itemAt.Place);
                        packet.WriteInt(itemAt.TemplateID);
                        packet.WriteInt(itemAt.AttackCompose);
                        packet.WriteInt(itemAt.DefendCompose);
                        packet.WriteInt(itemAt.AgilityCompose);
                        packet.WriteInt(itemAt.LuckCompose);
                        packet.WriteInt(itemAt.StrengthenLevel);
                        packet.WriteBoolean(itemAt.IsBinds);
                        packet.WriteBoolean(itemAt.IsJudge);
                        packet.WriteDateTime(itemAt.BeginDate);
                        packet.WriteInt(itemAt.ValidDate);
                        packet.WriteString((itemAt.Color == null) ? "" : itemAt.Color);
                        packet.WriteString((itemAt.Skin == null) ? "" : itemAt.Skin);
                        packet.WriteBoolean(itemAt.IsUsed);
                        packet.WriteInt(itemAt.Hole1);
                        packet.WriteInt(itemAt.Hole2);
                        packet.WriteInt(itemAt.Hole3);
                        packet.WriteInt(itemAt.Hole4);
                        packet.WriteInt(itemAt.Hole5);
                        packet.WriteInt(itemAt.Hole6);
                        packet.WriteString(itemAt.Template.Pic);
                        packet.WriteInt(5);
                        packet.WriteDateTime(DateTime.Now.AddDays(5.0));
                        packet.WriteInt(0);
                        packet.WriteByte(0);
                        packet.WriteInt(0);
                        packet.WriteByte(0);
                        packet.WriteInt(0);
                        packet.WriteInt(itemAt.LianExp);
                        packet.WriteInt(itemAt.LianGrade);
                    }
                }
                this.SendTCP(packet);
            }
        }

        public GSPacketIn SendUpdateOneKeyFinish(PlayerInfo player)
        {
            GSPacketIn packet = new GSPacketIn(0x56, player.ID);
            packet.WriteInt(0x2710);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendUpdatePlayerProperty(PlayerInfo info, PlayerProperty PlayerProp)
        {
            GSPacketIn packet = new GSPacketIn(0xa4, info.ID);
            packet.WriteInt(info.ID);
            for (int i = 0; i < 4; i++)
            {
                packet.WriteInt(0);
                packet.WriteInt(0);
                packet.WriteInt(0);
            }
            packet.WriteInt(0);
            packet.WriteInt(0);
            packet.WriteInt(0);
            packet.WriteInt(0);
            this.SendTCP(packet);
            return packet;
        }

        public void SendUpdatePrivateInfo(PlayerInfo info)
        {
            GSPacketIn packet = new GSPacketIn(0x26, info.ID);
            packet.WriteInt(info.Money);
            packet.WriteInt(1);
            packet.WriteInt(9);
            packet.WriteInt(info.Gold);
            packet.WriteInt(info.GiftToken);
            packet.WriteInt(0);
            this.SendTCP(packet);
        }

        public GSPacketIn SendUpdatePublicPlayer(PlayerInfo info, UserMatchInfo matchInfo)
        {
            GSPacketIn packet = new GSPacketIn(0x43, info.ID);
            packet.WriteInt(info.GP);
            packet.WriteInt(info.Offer);
            packet.WriteInt(info.RichesOffer);
            packet.WriteInt(info.RichesRob);
            packet.WriteInt(info.Win);
            packet.WriteInt(info.Total);
            packet.WriteInt(info.Escape);
            packet.WriteInt(info.Attack);
            packet.WriteInt(info.Defence);
            packet.WriteInt(info.Agility);
            packet.WriteInt(info.Luck);
            packet.WriteInt(info.hp);
            packet.WriteInt(info.Hide);
            packet.WriteString(info.Style);
            packet.WriteString(info.Colors);
            packet.WriteString(info.Skin);
            packet.WriteBoolean(false);
            packet.WriteInt(info.ConsortiaID);
            packet.WriteString(info.ConsortiaName);
            packet.WriteInt(info.badgeID);
            packet.WriteInt(info.ConsortiaLevel);
            packet.WriteInt(info.ConsortiaRepute);
            packet.WriteInt(info.Nimbus);
            packet.WriteString(info.PvePermission);
            packet.WriteString("1");
            packet.WriteInt(info.FightPower);
            packet.WriteInt(0);
            packet.WriteInt(0);
            packet.WriteString("");
            packet.WriteInt(0);
            packet.WriteString("");
            packet.WriteInt(info.AchievementPoint);
            packet.WriteString(info.Honor);
            packet.WriteDateTime(info.LastSpaDate);
            packet.WriteInt(0);
            packet.WriteInt(0);
            packet.WriteDateTime(DateTime.Now);
            packet.WriteInt(info.RichesOffer);
            packet.WriteInt(0);
            packet.WriteInt(0);
            packet.WriteInt(0);
            packet.WriteInt(0);
            packet.WriteInt(0);
            packet.WriteInt(info.Texp.spdTexpExp);
            packet.WriteInt(info.Texp.attTexpExp);
            packet.WriteInt(info.Texp.defTexpExp);
            packet.WriteInt(info.Texp.hpTexpExp);
            packet.WriteInt(info.Texp.lukTexpExp);
            packet.WriteInt(info.Texp.texpTaskCount);
            packet.WriteInt(info.Texp.texpCount);
            packet.WriteDateTime(info.Texp.texpTaskDate);
            packet.WriteInt(0);
            packet.WriteInt(0);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendUpdateQuests(GamePlayer player, byte[] states, BaseQuest[] infos)
        {
            if (((player == null) || (states == null)) || (infos == null))
            {
                return null;
            }
            GSPacketIn packet = new GSPacketIn(0xb2, player.PlayerCharacter.ID);
            packet.WriteInt(infos.Length);
            for (int i = 0; i < infos.Length; i++)
            {
                BaseQuest quest = infos[i];
                packet.WriteInt(quest.Data.QuestID);
                packet.WriteBoolean(quest.Data.IsComplete);
                packet.WriteInt(quest.Data.Condition1);
                packet.WriteInt(quest.Data.Condition2);
                packet.WriteInt(quest.Data.Condition3);
                packet.WriteInt(quest.Data.Condition4);
                packet.WriteDateTime(quest.Data.CompletedDate);
                packet.WriteInt(quest.Data.RepeatFinish);
                packet.WriteInt(quest.Data.RandDobule);
                packet.WriteBoolean(quest.Data.IsExist);
            }
            for (int j = 0; j < states.Length; j++)
            {
                packet.WriteByte(states[j]);
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendUpdateRoomList(List<BaseRoom> roomlist)
        {
            GSPacketIn packet = new GSPacketIn(0x5f);
            packet.WriteInt(roomlist.Count);
            int val = (roomlist.Count < 8) ? roomlist.Count : 8;
            packet.WriteInt(val);
            for (int i = 0; i < val; i++)
            {
                BaseRoom room = roomlist[i];
                packet.WriteInt(room.RoomId);
                packet.WriteByte((byte) room.RoomType);
                packet.WriteByte(room.TimeMode);
                packet.WriteByte((byte) room.PlayerCount);
                packet.WriteByte((byte) room.viewerCnt);
                packet.WriteByte((byte) room.maxViewerCnt);
                packet.WriteByte((byte) room.PlacesCount);
                packet.WriteBoolean(!string.IsNullOrEmpty(room.Password));
                packet.WriteInt(room.MapId);
                packet.WriteBoolean(room.IsPlaying);
                packet.WriteString(room.Name);
                packet.WriteByte((byte) room.GameType);
                packet.WriteByte((byte) room.HardLevel);
                packet.WriteInt(room.LevelLimits);
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendUpdateUpCount(PlayerInfo player)
        {
            GSPacketIn packet = new GSPacketIn(0x60, player.ID);
            packet.WriteInt(player.MaxBuyHonor);
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendUpdateUserPet(PetInventory bag, int[] slots)
        {
            if (this.m_gameClient.Player == null)
            {
                return null;
            }
            int iD = this.m_gameClient.Player.PlayerCharacter.ID;
            GSPacketIn packet = new GSPacketIn(0x44, iD);
            packet.WriteByte(1);
            packet.WriteInt(iD);
            packet.WriteInt(slots.Length);
            for (int i = 0; i < slots.Length; i++)
            {
                int val = slots[i];
                packet.WriteInt(val);
                UsersPetinfo petAt = bag.GetPetAt(val);
                if (petAt == null)
                {
                    packet.WriteBoolean(false);
                }
                else
                {
                    packet.WriteBoolean(true);
                    packet.WriteInt(petAt.ID);
                    packet.WriteInt(petAt.TemplateID);
                    packet.WriteString(petAt.Name);
                    packet.WriteInt(petAt.UserID);
                    packet.WriteInt(petAt.TotalAttack);
                    packet.WriteInt(petAt.TotalDefence);
                    packet.WriteInt(petAt.TotalLuck);
                    packet.WriteInt(petAt.TotalAgility);
                    packet.WriteInt(petAt.TotalBlood);
                    packet.WriteInt(petAt.TotalDamage);
                    packet.WriteInt(petAt.TotalGuard);
                    packet.WriteInt(petAt.AttackGrow);
                    packet.WriteInt(petAt.DefenceGrow);
                    packet.WriteInt(petAt.LuckGrow);
                    packet.WriteInt(petAt.AgilityGrow);
                    packet.WriteInt(petAt.BloodGrow);
                    packet.WriteInt(petAt.DamageGrow);
                    packet.WriteInt(petAt.GuardGrow);
                    packet.WriteInt(petAt.Level);
                    packet.WriteInt(petAt.GP);
                    packet.WriteInt(petAt.MaxGP);
                    packet.WriteInt(petAt.Hunger);
                    packet.WriteInt(petAt.PetHappyStar);
                    packet.WriteInt(petAt.MP);
                    List<string> skill = petAt.GetSkill();
                    List<string> skillEquip = petAt.GetSkillEquip();
                    packet.WriteInt(skill.Count);
                    foreach (string str in skill)
                    {
                        packet.WriteInt(int.Parse(str.Split(new char[] { ',' })[0]));
                        packet.WriteInt(int.Parse(str.Split(new char[] { ',' })[1]));
                    }
                    packet.WriteInt(skillEquip.Count);
                    foreach (string str in skillEquip)
                    {
                        packet.WriteInt(int.Parse(str.Split(new char[] { ',' })[1]));
                        packet.WriteInt(int.Parse(str.Split(new char[] { ',' })[0]));
                    }
                    packet.WriteBoolean(petAt.IsEquip);
                    List<PetEquipDataInfo> equip = petAt.GetEquip();
                    packet.WriteInt(equip.Count);
                    for (int j = 0; j < equip.Count; j++)
                    {
                        packet.WriteInt(equip[j].eqType);
                        packet.WriteInt(equip[j].eqTemplateID);
                        packet.WriteDateTime(equip[j].startTime);
                        packet.WriteInt(equip[j].ValidDate);
                    }
                    packet.WriteInt(petAt.currentStarExp);
                }
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendUserEquip(PlayerInfo player, List<SqlDataProvider.Data.ItemInfo> items, List<UserGemStone> UserGemStone, List<SqlDataProvider.Data.ItemInfo> beadItems, List<SqlDataProvider.Data.ItemInfo> magicStoneItems)
        {
            GSPacketIn packet = new GSPacketIn(0x4a, player.ID, 0x2800);
            packet.WriteInt(player.ID);
            packet.WriteString(player.NickName);
            packet.WriteInt(player.Agility);
            packet.WriteInt(player.Attack);
            packet.WriteString(player.Colors);
            packet.WriteString(player.Skin);
            packet.WriteInt(player.Defence);
            packet.WriteInt(player.GP);
            packet.WriteInt(player.Grade);
            packet.WriteInt(player.Luck);
            packet.WriteInt(player.hp);
            packet.WriteInt(player.Hide);
            packet.WriteInt(player.Repute);
            packet.WriteBoolean(player.Sex);
            packet.WriteString(player.Style);
            packet.WriteInt(player.Offer);
            packet.WriteByte(player.typeVIP);
            packet.WriteInt(player.VIPLevel);
            packet.WriteInt(player.Win);
            packet.WriteInt(player.Total);
            packet.WriteInt(player.Escape);
            packet.WriteInt(player.ConsortiaID);
            packet.WriteString(player.ConsortiaName);
            packet.WriteInt(player.badgeID);
            packet.WriteInt(player.RichesOffer);
            packet.WriteInt(player.RichesRob);
            packet.WriteBoolean(player.IsMarried);
            packet.WriteInt(player.SpouseID);
            packet.WriteString(player.SpouseName);
            packet.WriteString(player.DutyName);
            packet.WriteInt(player.Nimbus);
            packet.WriteInt(player.FightPower);
            packet.WriteInt(0);
            packet.WriteInt(0);
            packet.WriteString("");
            packet.WriteInt(0);
            packet.WriteString("");
            packet.WriteInt(player.AchievementPoint);
            packet.WriteString(player.Honor);
            packet.WriteDateTime(DateTime.Now.AddDays(-2.0));
            packet.WriteInt(player.Texp.spdTexpExp);
            packet.WriteInt(player.Texp.attTexpExp);
            packet.WriteInt(player.Texp.defTexpExp);
            packet.WriteInt(player.Texp.hpTexpExp);
            packet.WriteInt(player.Texp.lukTexpExp);
            packet.WriteInt(items.Count);
            foreach (SqlDataProvider.Data.ItemInfo info in items)
            {
                packet.WriteByte((byte) info.BagType);
                packet.WriteInt(info.UserID);
                packet.WriteInt(info.ItemID);
                packet.WriteInt(info.Count);
                packet.WriteInt(info.Place);
                packet.WriteInt(info.TemplateID);
                packet.WriteInt(info.AttackCompose);
                packet.WriteInt(info.DefendCompose);
                packet.WriteInt(info.AgilityCompose);
                packet.WriteInt(info.LuckCompose);
                packet.WriteInt(info.StrengthenLevel + info.LianGrade);
                packet.WriteBoolean(info.IsBinds);
                packet.WriteBoolean(info.IsJudge);
                packet.WriteDateTime(info.BeginDate);
                packet.WriteInt(info.ValidDate);
                packet.WriteString(info.Color);
                packet.WriteString(info.Skin);
                packet.WriteBoolean(info.IsUsed);
                packet.WriteInt(info.Hole1);
                packet.WriteInt(info.Hole2);
                packet.WriteInt(info.Hole3);
                packet.WriteInt(info.Hole4);
                packet.WriteInt(info.Hole5);
                packet.WriteInt(info.Hole6);
                packet.WriteString(info.Pic);
                packet.WriteInt(info.RefineryLevel);
                packet.WriteDateTime(DateTime.Now);
                packet.WriteByte((byte) info.Hole5Level);
                packet.WriteInt(info.Hole5Exp);
                packet.WriteByte((byte) info.Hole6Level);
                packet.WriteInt(info.Hole6Exp);
            }
            packet.WriteInt(beadItems.Count);
            foreach (SqlDataProvider.Data.ItemInfo info in beadItems)
            {
                packet.WriteByte((byte) info.BagType);
                packet.WriteInt(info.UserID);
                packet.WriteInt(info.ItemID);
                packet.WriteInt(info.Count);
                packet.WriteInt(info.Place);
                packet.WriteInt(info.TemplateID);
                packet.WriteInt(info.AttackCompose);
                packet.WriteInt(info.DefendCompose);
                packet.WriteInt(info.AgilityCompose);
                packet.WriteInt(info.LuckCompose);
                packet.WriteInt(info.StrengthenLevel);
                packet.WriteBoolean(info.IsBinds);
                packet.WriteBoolean(info.IsJudge);
                packet.WriteDateTime(info.BeginDate);
                packet.WriteInt(info.ValidDate);
                packet.WriteString(info.Color);
                packet.WriteString(info.Skin);
                packet.WriteBoolean(info.IsUsed);
                packet.WriteInt(info.Hole1);
                packet.WriteInt(info.Hole2);
                packet.WriteInt(info.Hole3);
                packet.WriteInt(info.Hole4);
                packet.WriteInt(info.Hole5);
                packet.WriteInt(info.Hole6);
                packet.WriteString(info.Pic);
                packet.WriteInt(info.RefineryLevel);
                packet.WriteDateTime(DateTime.Now);
                packet.WriteByte((byte) info.Hole5Level);
                packet.WriteInt(info.Hole5Exp);
                packet.WriteByte((byte) info.Hole6Level);
                packet.WriteInt(info.Hole6Exp);
                packet.WriteBoolean(info.IsGold);
            }
            packet.Compress();
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendUserRanks(List<UserRankInfo> rankList)
        {
            GSPacketIn packet = new GSPacketIn(0x22);
            packet.WriteInt(rankList.Count);
            foreach (UserRankInfo info in rankList)
            {
                packet.WriteString(info.UserRank);
            }
            this.SendTCP(packet);
            return packet;
        }

        public GSPacketIn SendUserRanks(int Id, List<UserRankInfo> ranks)
        {
            GSPacketIn packet = new GSPacketIn(0x22, Id);
            packet.WriteInt(ranks.Count);
            foreach (UserRankInfo info in ranks)
            {
                packet.WriteString(info.UserRank);
            }
            this.SendTCP(packet);
            return packet;
        }

        public void SendWaitingRoom(bool result)
        {
            GSPacketIn packet = new GSPacketIn(0x10);
            packet.WriteByte(result ? ((byte) 1) : ((byte) 0));
            this.SendTCP(packet);
        }

        public void SendWeaklessGuildProgress(PlayerInfo player)
        {
            GSPacketIn packet = new GSPacketIn(15, player.ID);
            packet.WriteInt(player.weaklessGuildProgress.Length);
            for (int i = 0; i < player.weaklessGuildProgress.Length; i++)
            {
                packet.WriteByte(player.weaklessGuildProgress[i]);
            }
            this.SendTCP(packet);
        }
    }
}

