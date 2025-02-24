namespace Bussiness
{
    using Bussiness.CenterService;
    using Bussiness.Managers;
    using SqlDataProvider.Data;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.InteropServices;

    public class PlayerBussiness : BaseBussiness
    {
        public bool ActivePlayer(ref PlayerInfo player, string userName, string passWord, bool sex, int gold, int money, string IP, string site)
        {
            bool flag = false;
            try
            {
                player = new PlayerInfo();
                player.Agility = 0;
                player.Attack = 0;
                player.Colors = ",,,,,,";
                player.Skin = "";
                player.ConsortiaID = 0;
                player.Defence = 0;
                player.MagicAttack = 0;
                player.MagicDefence = 0;
                player.evolutionExp = 0;
                player.evolutionGrade = 0;
                player.Gold = 0;
                player.GP = 1;
                player.Grade = 1;
                player.ID = 0;
                player.Luck = 0;
                player.Money = 0;
                player.NickName = "";
                player.Sex = sex;
                player.State = 0;
                player.Style = ",,,,,,";
                player.Hide = 0x423a35c7;
                SqlParameter[] sqlParameters = new SqlParameter[0x19];
                sqlParameters[0] = new SqlParameter("@UserID", SqlDbType.Int);
                sqlParameters[0].Direction = ParameterDirection.Output;
                sqlParameters[1] = new SqlParameter("@Attack", player.Attack);
                sqlParameters[2] = new SqlParameter("@Colors", (player.Colors == null) ? "" : player.Colors);
                sqlParameters[3] = new SqlParameter("@ConsortiaID", player.ConsortiaID);
                sqlParameters[4] = new SqlParameter("@Defence", player.Defence);
                sqlParameters[5] = new SqlParameter("@Gold", player.Gold);
                sqlParameters[6] = new SqlParameter("@GP", player.GP);
                sqlParameters[7] = new SqlParameter("@Grade", player.Grade);
                sqlParameters[8] = new SqlParameter("@Luck", player.Luck);
                sqlParameters[9] = new SqlParameter("@Money", player.Money);
                sqlParameters[10] = new SqlParameter("@Style", (player.Style == null) ? "" : player.Style);
                sqlParameters[11] = new SqlParameter("@Agility", player.Agility);
                sqlParameters[12] = new SqlParameter("@State", player.State);
                sqlParameters[13] = new SqlParameter("@UserName", userName);
                sqlParameters[14] = new SqlParameter("@PassWord", passWord);
                sqlParameters[15] = new SqlParameter("@Sex", sex);
                sqlParameters[0x10] = new SqlParameter("@Hide", player.Hide);
                sqlParameters[0x11] = new SqlParameter("@ActiveIP", IP);
                sqlParameters[0x12] = new SqlParameter("@Skin", (player.Skin == null) ? "" : player.Skin);
                sqlParameters[0x13] = new SqlParameter("@MagicAttack", player.MagicAttack);
                sqlParameters[20] = new SqlParameter("@MagicDefence", player.MagicDefence);
                sqlParameters[0x15] = new SqlParameter("@evolutionGrade", player.evolutionGrade);
                sqlParameters[0x16] = new SqlParameter("@evolutionExp", player.evolutionExp);
                sqlParameters[0x17] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[0x17].Direction = ParameterDirection.ReturnValue;
                sqlParameters[0x18] = new SqlParameter("@Site", site);
                flag = base.db.RunProcedure("SP_Users_Active", sqlParameters);
                player.ID = (int) sqlParameters[0].Value;
                flag = ((int) sqlParameters[0x15].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool AddActiveSystem(ActiveSystemInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[20];
                sqlParameters[0] = new SqlParameter("@ID", info.ID);
                sqlParameters[0].Direction = ParameterDirection.Output;
                sqlParameters[1] = new SqlParameter("@UserID", info.UserID);
                sqlParameters[2] = new SqlParameter("@useableScore", info.useableScore);
                sqlParameters[3] = new SqlParameter("@totalScore", info.totalScore);
                sqlParameters[4] = new SqlParameter("@AvailTime", info.AvailTime);
                sqlParameters[5] = new SqlParameter("@NickName", info.NickName);
                sqlParameters[6] = new SqlParameter("@CanGetGift", info.CanGetGift);
                sqlParameters[7] = new SqlParameter("@canOpenCounts", info.canOpenCounts);
                sqlParameters[8] = new SqlParameter("@canEagleEyeCounts", info.canEagleEyeCounts);
                sqlParameters[9] = new SqlParameter("@lastFlushTime", info.lastFlushTime.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[10] = new SqlParameter("@isShowAll", info.isShowAll);
                sqlParameters[11] = new SqlParameter("@AvtiveMoney", info.ActiveMoney);
                sqlParameters[12] = new SqlParameter("@activityTanabataNum", info.activityTanabataNum);
                sqlParameters[13] = new SqlParameter("@ChallengeNum", info.ChallengeNum);
                sqlParameters[14] = new SqlParameter("@BuyBuffNum", info.BuyBuffNum);
                sqlParameters[15] = new SqlParameter("@lastEnterYearMonter", info.lastEnterYearMonter);
                sqlParameters[0x10] = new SqlParameter("@DamageNum", info.DamageNum);
                sqlParameters[0x11] = new SqlParameter("@BoxState", info.BoxState);
                sqlParameters[0x12] = new SqlParameter("@LuckystarCoins", info.LuckystarCoins);
                sqlParameters[0x13] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[0x13].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_ActiveSystem_Add", sqlParameters);
                flag = ((int) sqlParameters[0x13].Value) == 0;
                info.ID = (int) sqlParameters[0].Value;
                info.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool AddAuction(AuctionInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[0x12];
                sqlParameters[0] = new SqlParameter("@AuctionID", info.AuctionID);
                sqlParameters[0].Direction = ParameterDirection.Output;
                sqlParameters[1] = new SqlParameter("@AuctioneerID", info.AuctioneerID);
                sqlParameters[2] = new SqlParameter("@AuctioneerName", (info.AuctioneerName == null) ? "" : info.AuctioneerName);
                sqlParameters[3] = new SqlParameter("@BeginDate", info.BeginDate.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[4] = new SqlParameter("@BuyerID", info.BuyerID);
                sqlParameters[5] = new SqlParameter("@BuyerName", (info.BuyerName == null) ? "" : info.BuyerName);
                sqlParameters[6] = new SqlParameter("@IsExist", info.IsExist);
                sqlParameters[7] = new SqlParameter("@ItemID", info.ItemID);
                sqlParameters[8] = new SqlParameter("@Mouthful", info.Mouthful);
                sqlParameters[9] = new SqlParameter("@PayType", info.PayType);
                sqlParameters[10] = new SqlParameter("@Price", info.Price);
                sqlParameters[11] = new SqlParameter("@Rise", info.Rise);
                sqlParameters[12] = new SqlParameter("@ValidDate", info.ValidDate);
                sqlParameters[13] = new SqlParameter("@TemplateID", info.TemplateID);
                sqlParameters[14] = new SqlParameter("Name", info.Name);
                sqlParameters[15] = new SqlParameter("Category", info.Category);
                sqlParameters[0x10] = new SqlParameter("Random", info.Random);
                sqlParameters[0x11] = new SqlParameter("goodsCount", info.goodsCount);
                flag = base.db.RunProcedure("SP_Auction_Add", sqlParameters);
                info.AuctionID = (int) sqlParameters[0].Value;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool AddCards(UsersCardInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[14];
                sqlParameters[0] = new SqlParameter("@CardID", item.CardID);
                sqlParameters[0].Direction = ParameterDirection.Output;
                sqlParameters[1] = new SqlParameter("@Count", item.Count);
                sqlParameters[2] = new SqlParameter("@UserID", item.UserID);
                sqlParameters[3] = new SqlParameter("@Place", item.Place);
                sqlParameters[4] = new SqlParameter("@TemplateID", item.TemplateID);
                sqlParameters[5] = new SqlParameter("@isFirstGet", false);
                sqlParameters[6] = new SqlParameter("@Attack", item.Attack);
                sqlParameters[7] = new SqlParameter("@Defence", item.Defence);
                sqlParameters[8] = new SqlParameter("@Luck", item.Luck);
                sqlParameters[9] = new SqlParameter("@Agility", item.Agility);
                sqlParameters[10] = new SqlParameter("@Damage", item.Damage);
                sqlParameters[11] = new SqlParameter("@Guard", item.Guard);
                sqlParameters[12] = new SqlParameter("@Level", item.Level);
                sqlParameters[13] = new SqlParameter("@CardGP", item.CardGP);
                flag = base.db.RunProcedure("SP_Users_Cards_Add", sqlParameters);
                item.CardID = (int) sqlParameters[0].Value;
                item.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool AddChargeMoney(string chargeID, string userName, int money, string payWay, decimal needMoney, ref int userID, ref int isResult, DateTime date, string IP, string nickName)
        {
            bool flag = false;
            userID = 0;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[10];
                sqlParameters[0] = new SqlParameter("@ChargeID", chargeID);
                sqlParameters[1] = new SqlParameter("@UserName", userName);
                sqlParameters[2] = new SqlParameter("@Money", money);
                sqlParameters[3] = new SqlParameter("@Date", date.ToString("yyyy-MM-dd HH:mm:ss"));
                sqlParameters[4] = new SqlParameter("@PayWay", payWay);
                sqlParameters[5] = new SqlParameter("@NeedMoney", needMoney);
                sqlParameters[6] = new SqlParameter("@UserID", (int) userID);
                sqlParameters[6].Direction = ParameterDirection.InputOutput;
                sqlParameters[7] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[7].Direction = ParameterDirection.ReturnValue;
                sqlParameters[8] = new SqlParameter("@IP", IP);
                sqlParameters[9] = new SqlParameter("@NickName", nickName);
                flag = base.db.RunProcedure("SP_Charge_Money_Add", sqlParameters);
                userID = (int) sqlParameters[6].Value;
                isResult = (int) sqlParameters[7].Value;
                flag = isResult == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool AddDiceData(DiceDataInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[10];
                sqlParameters[0] = new SqlParameter("@ID", info.ID);
                sqlParameters[0].Direction = ParameterDirection.Output;
                sqlParameters[1] = new SqlParameter("@UserID", info.UserID);
                sqlParameters[2] = new SqlParameter("@LuckIntegral", info.LuckIntegral);
                sqlParameters[3] = new SqlParameter("@LuckIntegralLevel", info.LuckIntegralLevel);
                sqlParameters[4] = new SqlParameter("@Level", info.Level);
                sqlParameters[5] = new SqlParameter("@FreeCount", info.FreeCount);
                sqlParameters[6] = new SqlParameter("@CurrentPosition", info.CurrentPosition);
                sqlParameters[7] = new SqlParameter("@UserFirstCell", info.UserFirstCell);
                sqlParameters[8] = new SqlParameter("@AwardArray", info.AwardArray);
                sqlParameters[9] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[9].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_DiceData_Add", sqlParameters);
                flag = ((int) sqlParameters[9].Value) == 0;
                info.ID = (int) sqlParameters[0].Value;
                info.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_DiceData_Add", exception);
                }
            }
            return flag;
        }

        public bool AddeqPet(PetEquipDataInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[9];
                sqlParameters[0] = new SqlParameter("@ID", info.ID);
                sqlParameters[0].Direction = ParameterDirection.Output;
                sqlParameters[1] = new SqlParameter("@UserID", info.UserID);
                sqlParameters[2] = new SqlParameter("@PetID", info.PetID);
                sqlParameters[3] = new SqlParameter("@eqType", info.eqType);
                sqlParameters[4] = new SqlParameter("@eqTemplateID", info.eqTemplateID);
                sqlParameters[5] = new SqlParameter("@startTime", info.startTime.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[6] = new SqlParameter("@ValidDate", info.ValidDate);
                sqlParameters[7] = new SqlParameter("@IsExit", info.IsExit);
                sqlParameters[8] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[8].Direction = ParameterDirection.ReturnValue;
                flag = base.db.RunProcedure("SP_User_Add_eqPet", sqlParameters);
                int num = (int) sqlParameters[8].Value;
                flag = num == 0;
                info.ID = (int) sqlParameters[0].Value;
                info.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool AddFarm(UserFarmInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[15];
                sqlParameters[0] = new SqlParameter("@FarmID", item.FarmID);
                sqlParameters[1] = new SqlParameter("@PayFieldMoney", item.PayFieldMoney);
                sqlParameters[2] = new SqlParameter("@PayAutoMoney", item.PayAutoMoney);
                sqlParameters[3] = new SqlParameter("@AutoPayTime", item.AutoPayTime.ToString());
                sqlParameters[4] = new SqlParameter("@AutoValidDate", item.AutoValidDate);
                sqlParameters[5] = new SqlParameter("@VipLimitLevel", item.VipLimitLevel);
                sqlParameters[6] = new SqlParameter("@FarmerName", item.FarmerName);
                sqlParameters[7] = new SqlParameter("@GainFieldId", item.GainFieldId);
                sqlParameters[8] = new SqlParameter("@MatureId", item.MatureId);
                sqlParameters[9] = new SqlParameter("@KillCropId", item.KillCropId);
                sqlParameters[10] = new SqlParameter("@isAutoId", item.isAutoId);
                sqlParameters[11] = new SqlParameter("@isFarmHelper", item.isFarmHelper);
                sqlParameters[12] = new SqlParameter("@ID", item.ID);
                sqlParameters[12].Direction = ParameterDirection.Output;
                sqlParameters[13] = new SqlParameter("@buyExpRemainNum", item.buyExpRemainNum);
                sqlParameters[14] = new SqlParameter("@isArrange", item.isArrange);
                flag = base.db.RunProcedure("SP_Users_Farm_Add", sqlParameters);
                item.ID = (int) sqlParameters[12].Value;
                item.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool AddFields(UserFieldInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { 
                    new SqlParameter("@FarmID", item.FarmID), new SqlParameter("@FieldID", item.FieldID), new SqlParameter("@SeedID", item.SeedID), new SqlParameter("@PlantTime", item.PlantTime.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@AccelerateTime", item.AccelerateTime), new SqlParameter("@FieldValidDate", item.FieldValidDate), new SqlParameter("@PayTime", item.PayTime.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@GainCount", item.GainCount), new SqlParameter("@AutoSeedID", item.AutoSeedID), new SqlParameter("@AutoFertilizerID", item.AutoFertilizerID), new SqlParameter("@AutoSeedIDCount", item.AutoSeedIDCount), new SqlParameter("@AutoFertilizerCount", item.AutoFertilizerCount), new SqlParameter("@isAutomatic", item.isAutomatic), new SqlParameter("@AutomaticTime", item.AutomaticTime.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@IsExit", item.IsExit), new SqlParameter("@payFieldTime", item.payFieldTime), 
                    new SqlParameter("@ID", item.ID)
                 };
                sqlParameters[0x10].Direction = ParameterDirection.Output;
                flag = base.db.RunProcedure("SP_Users_Fields_Add", sqlParameters);
                item.ID = (int) sqlParameters[0x10].Value;
                item.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool AddFriends(FriendInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", info.ID), new SqlParameter("@AddDate", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@FriendID", info.FriendID), new SqlParameter("@IsExist", true), new SqlParameter("@Remark", (info.Remark == null) ? "" : info.Remark), new SqlParameter("@UserID", info.UserID), new SqlParameter("@Relation", info.Relation) };
                flag = base.db.RunProcedure("SP_Users_Friends_Add", sqlParameters);
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool AddGoods(SqlDataProvider.Data.ItemInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[0x34];
                sqlParameters[0] = new SqlParameter("@ItemID", item.ItemID);
                sqlParameters[0].Direction = ParameterDirection.Output;
                sqlParameters[1] = new SqlParameter("@UserID", item.UserID);
                sqlParameters[2] = new SqlParameter("@TemplateID", item.Template.TemplateID);
                sqlParameters[3] = new SqlParameter("@Place", item.Place);
                sqlParameters[4] = new SqlParameter("@AgilityCompose", item.AgilityCompose);
                sqlParameters[5] = new SqlParameter("@AttackCompose", item.AttackCompose);
                sqlParameters[6] = new SqlParameter("@BeginDate", item.BeginDate.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[7] = new SqlParameter("@Color", (item.Color == null) ? "" : item.Color);
                sqlParameters[8] = new SqlParameter("@Count", item.Count);
                sqlParameters[9] = new SqlParameter("@DefendCompose", item.DefendCompose);
                sqlParameters[10] = new SqlParameter("@IsBinds", item.IsBinds);
                sqlParameters[11] = new SqlParameter("@IsExist", item.IsExist);
                sqlParameters[12] = new SqlParameter("@IsJudge", item.IsJudge);
                sqlParameters[13] = new SqlParameter("@LuckCompose", item.LuckCompose);
                sqlParameters[14] = new SqlParameter("@StrengthenLevel", item.StrengthenLevel);
                sqlParameters[15] = new SqlParameter("@ValidDate", item.ValidDate);
                sqlParameters[0x10] = new SqlParameter("@BagType", item.BagType);
                sqlParameters[0x11] = new SqlParameter("@Skin", (item.Skin == null) ? "" : item.Skin);
                sqlParameters[0x12] = new SqlParameter("@IsUsed", item.IsUsed);
                sqlParameters[0x13] = new SqlParameter("@RemoveType", item.RemoveType);
                sqlParameters[20] = new SqlParameter("@Hole1", item.Hole1);
                sqlParameters[0x15] = new SqlParameter("@Hole2", item.Hole2);
                sqlParameters[0x16] = new SqlParameter("@Hole3", item.Hole3);
                sqlParameters[0x17] = new SqlParameter("@Hole4", item.Hole4);
                sqlParameters[0x18] = new SqlParameter("@Hole5", item.Hole5);
                sqlParameters[0x19] = new SqlParameter("@Hole6", item.Hole6);
                sqlParameters[0x1a] = new SqlParameter("@StrengthenTimes", item.StrengthenTimes);
                sqlParameters[0x1b] = new SqlParameter("@Hole5Level", item.Hole5Level);
                sqlParameters[0x1c] = new SqlParameter("@Hole5Exp", item.Hole5Exp);
                sqlParameters[0x1d] = new SqlParameter("@Hole6Level", item.Hole6Level);
                sqlParameters[30] = new SqlParameter("@Hole6Exp", item.Hole6Exp);
                sqlParameters[0x1f] = new SqlParameter("@IsGold", item.IsGold);
                sqlParameters[0x20] = new SqlParameter("@goldValidDate", item.goldValidDate);
                sqlParameters[0x21] = new SqlParameter("@StrengthenExp", item.StrengthenExp);
                sqlParameters[0x22] = new SqlParameter("@beadExp", item.beadExp);
                sqlParameters[0x23] = new SqlParameter("@beadLevel", item.beadLevel);
                sqlParameters[0x24] = new SqlParameter("@beadIsLock", item.beadIsLock);
                sqlParameters[0x25] = new SqlParameter("@isShowBind", item.isShowBind);
                sqlParameters[0x26] = new SqlParameter("@Damage", item.Damage);
                sqlParameters[0x27] = new SqlParameter("@Guard", item.Guard);
                sqlParameters[40] = new SqlParameter("@Blood", item.Blood);
                sqlParameters[0x29] = new SqlParameter("@Bless", item.Bless);
                sqlParameters[0x2a] = new SqlParameter("@goldBeginTime", item.goldBeginTime.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[0x2b] = new SqlParameter("@latentEnergyEndTime", item.latentEnergyEndTime);
                sqlParameters[0x2c] = new SqlParameter("@latentEnergyCurStr", item.latentEnergyCurStr);
                sqlParameters[0x2d] = new SqlParameter("@latentEnergyNewStr", item.latentEnergyNewStr);
                sqlParameters[0x2e] = new SqlParameter("@MagicAttack", item.MagicAttack);
                sqlParameters[0x2f] = new SqlParameter("@MagicDefence", item.MagicDefence);
                sqlParameters[0x30] = new SqlParameter("@goodsLock", item.GoodsLock);
                sqlParameters[0x31] = new SqlParameter("@AdvanceDate", item.AdvanceDate.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[50] = new SqlParameter("@LianGrade", item.LianGrade);
                sqlParameters[0x33] = new SqlParameter("@LianExp", item.LianExp);
                flag = base.db.RunProcedure("SP_Users_Items_Add", sqlParameters);
                item.ItemID = (int) sqlParameters[0].Value;
                item.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool AddMarryInfo(MarryInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[5];
                sqlParameters[0] = new SqlParameter("@ID", info.ID);
                sqlParameters[0].Direction = ParameterDirection.Output;
                sqlParameters[1] = new SqlParameter("@UserID", info.UserID);
                sqlParameters[2] = new SqlParameter("@IsPublishEquip", info.IsPublishEquip);
                sqlParameters[3] = new SqlParameter("@Introduction", info.Introduction);
                sqlParameters[4] = new SqlParameter("@RegistTime", info.RegistTime.ToString("MM/dd/yyyy hh:mm:ss"));
                flag = base.db.RunProcedure("SP_MarryInfo_Add", sqlParameters);
                info.ID = (int) sqlParameters[0].Value;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("AddMarryInfo", exception);
                }
            }
            return flag;
        }

        public bool AddNewChickenBox(NewChickenBoxItemInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[15];
                sqlParameters[0] = new SqlParameter("@ID", info.ID);
                sqlParameters[0].Direction = ParameterDirection.Output;
                sqlParameters[1] = new SqlParameter("@UserID", info.UserID);
                sqlParameters[2] = new SqlParameter("@TemplateID", info.TemplateID);
                sqlParameters[3] = new SqlParameter("@Count", info.Count);
                sqlParameters[4] = new SqlParameter("@ValidDate", info.ValidDate);
                sqlParameters[5] = new SqlParameter("@StrengthenLevel", info.StrengthenLevel);
                sqlParameters[6] = new SqlParameter("@AttackCompose", info.AttackCompose);
                sqlParameters[7] = new SqlParameter("@DefendCompose", info.DefendCompose);
                sqlParameters[8] = new SqlParameter("@AgilityCompose", info.AgilityCompose);
                sqlParameters[9] = new SqlParameter("@LuckCompose", info.LuckCompose);
                sqlParameters[10] = new SqlParameter("@Position", info.Position);
                sqlParameters[11] = new SqlParameter("@IsSelected", info.IsSelected);
                sqlParameters[12] = new SqlParameter("@IsSeeded", info.IsSeeded);
                sqlParameters[13] = new SqlParameter("@IsBinds", info.IsBinds);
                sqlParameters[14] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[14].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_NewChickenBox_Add", sqlParameters);
                flag = ((int) sqlParameters[14].Value) == 0;
                info.ID = (int) sqlParameters[0].Value;
                info.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_NewChickenBox_Add", exception);
                }
            }
            return flag;
        }

        public bool AddPyramid(PyramidInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[12];
                sqlParameters[0] = new SqlParameter("@ID", info.ID);
                sqlParameters[0].Direction = ParameterDirection.Output;
                sqlParameters[1] = new SqlParameter("@UserID", info.UserID);
                sqlParameters[2] = new SqlParameter("@currentLayer", info.currentLayer);
                sqlParameters[3] = new SqlParameter("@maxLayer", info.maxLayer);
                sqlParameters[4] = new SqlParameter("@totalPoint", info.totalPoint);
                sqlParameters[5] = new SqlParameter("@turnPoint", info.turnPoint);
                sqlParameters[6] = new SqlParameter("@pointRatio", info.pointRatio);
                sqlParameters[7] = new SqlParameter("@currentFreeCount", info.currentFreeCount);
                sqlParameters[8] = new SqlParameter("@currentReviveCount", info.currentReviveCount);
                sqlParameters[9] = new SqlParameter("@isPyramidStart", info.isPyramidStart);
                sqlParameters[10] = new SqlParameter("@LayerItems", info.LayerItems);
                sqlParameters[11] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[11].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_Pyramid_Add", sqlParameters);
                flag = ((int) sqlParameters[11].Value) == 0;
                info.ID = (int) sqlParameters[0].Value;
                info.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_Pyramid_Add", exception);
                }
            }
            return flag;
        }

        public bool AddStore(SqlDataProvider.Data.ItemInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[14];
                sqlParameters[0] = new SqlParameter("@ItemID", item.ItemID);
                sqlParameters[0].Direction = ParameterDirection.Output;
                sqlParameters[1] = new SqlParameter("@UserID", item.UserID);
                sqlParameters[2] = new SqlParameter("@TemplateID", item.Template.TemplateID);
                sqlParameters[3] = new SqlParameter("@Place", item.Place);
                sqlParameters[4] = new SqlParameter("@AgilityCompose", item.AgilityCompose);
                sqlParameters[5] = new SqlParameter("@AttackCompose", item.AttackCompose);
                sqlParameters[6] = new SqlParameter("@BeginDate", item.BeginDate.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[7] = new SqlParameter("@Color", (item.Color == null) ? "" : item.Color);
                sqlParameters[8] = new SqlParameter("@Count", item.Count);
                sqlParameters[9] = new SqlParameter("@DefendCompose", item.DefendCompose);
                sqlParameters[10] = new SqlParameter("@IsBinds", item.IsBinds);
                sqlParameters[11] = new SqlParameter("@IsExist", item.IsExist);
                sqlParameters[12] = new SqlParameter("@IsJudge", item.IsJudge);
                sqlParameters[13] = new SqlParameter("@LuckCompose", item.LuckCompose);
                flag = base.db.RunProcedure("SP_Users_Items_Add", sqlParameters);
                item.ItemID = (int) sqlParameters[0].Value;
                item.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool AddTreasureData(TreasureDataInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[9];
                sqlParameters[0] = new SqlParameter("@ID", item.ID);
                sqlParameters[0].Direction = ParameterDirection.Output;
                sqlParameters[1] = new SqlParameter("@UserID", item.UserID);
                sqlParameters[2] = new SqlParameter("@TemplateID", item.TemplateID);
                sqlParameters[3] = new SqlParameter("@Count", item.Count);
                sqlParameters[4] = new SqlParameter("@Validate", item.ValidDate);
                sqlParameters[5] = new SqlParameter("@Pos", item.pos);
                sqlParameters[6] = new SqlParameter("@BeginDate", item.BeginDate.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[7] = new SqlParameter("@IsExit", item.IsExit);
                sqlParameters[8] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[8].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_TreasureData_Add", sqlParameters);
                flag = ((int) sqlParameters[8].Value) == 0;
                item.ID = (int) sqlParameters[0].Value;
                item.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool AddUserAdoptPet(UsersPetinfo info, bool isUse)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { 
                    new SqlParameter("@TemplateID", info.TemplateID), new SqlParameter("@Name", (info.Name == null) ? "Error!" : info.Name), new SqlParameter("@UserID", info.UserID), new SqlParameter("@Attack", info.Attack), new SqlParameter("@Defence", info.Defence), new SqlParameter("@Luck", info.Luck), new SqlParameter("@Agility", info.Agility), new SqlParameter("@Blood", info.Blood), new SqlParameter("@Damage", info.Damage), new SqlParameter("@Guard", info.Guard), new SqlParameter("@AttackGrow", info.AttackGrow), new SqlParameter("@DefenceGrow", info.DefenceGrow), new SqlParameter("@LuckGrow", info.LuckGrow), new SqlParameter("@AgilityGrow", info.AgilityGrow), new SqlParameter("@BloodGrow", info.BloodGrow), new SqlParameter("@DamageGrow", info.DamageGrow), 
                    new SqlParameter("@GuardGrow", info.GuardGrow), new SqlParameter("@Skill", info.Skill), new SqlParameter("@SkillEquip", info.SkillEquip), new SqlParameter("@Place", info.Place), new SqlParameter("@IsExit", info.IsExit), new SqlParameter("@IsUse", isUse), new SqlParameter("@currentStarExp", info.currentStarExp), new SqlParameter("@ID", info.ID)
                 };
                sqlParameters[0x17].Direction = ParameterDirection.Output;
                flag = base.db.RunProcedure("SP_User_AdoptPet", sqlParameters);
                info.ID = (int) sqlParameters[0x17].Value;
                info.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool AddUserAvatarCollect(UserAvatarCollectionInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[10];
                sqlParameters[0] = new SqlParameter("@ID", item.ID);
                sqlParameters[0].Direction = ParameterDirection.Output;
                sqlParameters[1] = new SqlParameter("@UserID", item.UserID);
                sqlParameters[2] = new SqlParameter("@AvatarID", item.AvatarID);
                sqlParameters[3] = new SqlParameter("@Sex", item.Sex);
                sqlParameters[4] = new SqlParameter("@IsActive", item.IsActive);
                sqlParameters[5] = new SqlParameter("@Data", item.Data);
                sqlParameters[6] = new SqlParameter("@TimeStart", item.TimeStart.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[7] = new SqlParameter("@TimeEnd", item.TimeEnd.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[8] = new SqlParameter("@IsExit", item.IsExit);
                sqlParameters[9] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[9].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_AvatarCollect_Add", sqlParameters);
                flag = ((int) sqlParameters[9].Value) == 0;
                item.ID = (int) sqlParameters[0].Value;
                item.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool AddUserChristmas(UserChristmasInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[13];
                sqlParameters[0] = new SqlParameter("@ID", info.ID);
                sqlParameters[0].Direction = ParameterDirection.Output;
                sqlParameters[1] = new SqlParameter("@UserID", info.UserID);
                sqlParameters[2] = new SqlParameter("@exp", info.exp);
                sqlParameters[3] = new SqlParameter("@awardState", info.awardState);
                sqlParameters[4] = new SqlParameter("@count", info.count);
                sqlParameters[5] = new SqlParameter("@packsNumber", info.packsNumber);
                sqlParameters[6] = new SqlParameter("@lastPacks", info.lastPacks);
                sqlParameters[7] = new SqlParameter("@gameBeginTime", info.gameBeginTime.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[8] = new SqlParameter("@gameEndTime", info.gameEndTime.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[9] = new SqlParameter("@isEnter", info.isEnter);
                sqlParameters[10] = new SqlParameter("@dayPacks", info.dayPacks);
                sqlParameters[11] = new SqlParameter("@AvailTime", info.AvailTime);
                sqlParameters[12] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[12].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UserChristmas_Add", sqlParameters);
                flag = ((int) sqlParameters[12].Value) == 0;
                info.ID = (int) sqlParameters[0].Value;
                info.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool AddUserDressModel(UserDressModelInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[7];
                sqlParameters[0] = new SqlParameter("@ID", item.ID);
                sqlParameters[0].Direction = ParameterDirection.Output;
                sqlParameters[1] = new SqlParameter("@UserID", item.UserID);
                sqlParameters[2] = new SqlParameter("@SlotID", item.SlotID);
                sqlParameters[3] = new SqlParameter("@TemplateID", item.TemplateID);
                sqlParameters[4] = new SqlParameter("@ItemID", item.ItemID);
                sqlParameters[5] = new SqlParameter("@CategoryID", item.CategoryID);
                sqlParameters[6] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[6].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_DressModel_Add", sqlParameters);
                flag = ((int) sqlParameters[6].Value) == 0;
                item.ID = (int) sqlParameters[0].Value;
                item.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool AddUserGemStone(UserGemStone item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@ID", item.ID);
                sqlParameters[0].Direction = ParameterDirection.Output;
                sqlParameters[1] = new SqlParameter("@UserID", item.UserID);
                sqlParameters[2] = new SqlParameter("@FigSpiritId", item.FigSpiritId);
                sqlParameters[3] = new SqlParameter("@FigSpiritIdValue", item.FigSpiritIdValue);
                sqlParameters[4] = new SqlParameter("@EquipPlace", item.EquipPlace);
                sqlParameters[5] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[5].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_Users_GemStones_Add", sqlParameters);
                int num = (int) sqlParameters[5].Value;
                flag = num == 0;
                item.ID = (int) sqlParameters[0].Value;
                item.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool AddUserLabyrinth(UserLabyrinthInfo laby)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { 
                    new SqlParameter("@UserID", laby.UserID), new SqlParameter("@myProgress", laby.myProgress), new SqlParameter("@myRanking", laby.myRanking), new SqlParameter("@completeChallenge", laby.completeChallenge), new SqlParameter("@isDoubleAward", laby.isDoubleAward), new SqlParameter("@currentFloor", laby.currentFloor), new SqlParameter("@accumulateExp", laby.accumulateExp), new SqlParameter("@remainTime", laby.remainTime), new SqlParameter("@currentRemainTime", laby.currentRemainTime), new SqlParameter("@cleanOutAllTime", laby.cleanOutAllTime), new SqlParameter("@cleanOutGold", laby.cleanOutGold), new SqlParameter("@tryAgainComplete", laby.tryAgainComplete), new SqlParameter("@isInGame", laby.isInGame), new SqlParameter("@isCleanOut", laby.isCleanOut), new SqlParameter("@serverMultiplyingPower", laby.serverMultiplyingPower), new SqlParameter("@LastDate", laby.LastDate.ToString("MM/dd/yyyy hh:mm:ss")), 
                    new SqlParameter("@ProcessAward", laby.ProcessAward), new SqlParameter("@Result", SqlDbType.Int)
                 };
                sqlParameters[0x11].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_Users_Labyrinth_Add", sqlParameters);
                int num = (int) sqlParameters[0x11].Value;
                flag = num == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool AddUserMatchInfo(UserMatchInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[14];
                sqlParameters[0] = new SqlParameter("@ID", info.ID);
                sqlParameters[0].Direction = ParameterDirection.Output;
                sqlParameters[1] = new SqlParameter("@UserID", info.UserID);
                sqlParameters[2] = new SqlParameter("@dailyScore", info.dailyScore);
                sqlParameters[3] = new SqlParameter("@dailyWinCount", info.dailyWinCount);
                sqlParameters[4] = new SqlParameter("@dailyGameCount", info.dailyGameCount);
                sqlParameters[5] = new SqlParameter("@DailyLeagueFirst", info.DailyLeagueFirst);
                sqlParameters[6] = new SqlParameter("@DailyLeagueLastScore", info.DailyLeagueLastScore);
                sqlParameters[7] = new SqlParameter("@weeklyScore", info.weeklyScore);
                sqlParameters[8] = new SqlParameter("@weeklyGameCount", info.weeklyGameCount);
                sqlParameters[9] = new SqlParameter("@weeklyRanking", info.weeklyRanking);
                sqlParameters[10] = new SqlParameter("@addDayPrestge", info.addDayPrestge);
                sqlParameters[11] = new SqlParameter("@totalPrestige", info.totalPrestige);
                sqlParameters[12] = new SqlParameter("@restCount", info.restCount);
                sqlParameters[13] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[13].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UserMatch_Add", sqlParameters);
                flag = ((int) sqlParameters[13].Value) == 0;
                info.ID = (int) sqlParameters[0].Value;
                info.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool AddUserPet(UsersPetinfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[0x1f];
                sqlParameters[0] = new SqlParameter("@TemplateID", info.TemplateID);
                sqlParameters[1] = new SqlParameter("@Name", (info.Name == null) ? "Error!" : info.Name);
                sqlParameters[2] = new SqlParameter("@UserID", info.UserID);
                sqlParameters[3] = new SqlParameter("@Attack", info.Attack);
                sqlParameters[4] = new SqlParameter("@Defence", info.Defence);
                sqlParameters[5] = new SqlParameter("@Luck", info.Luck);
                sqlParameters[6] = new SqlParameter("@Agility", info.Agility);
                sqlParameters[7] = new SqlParameter("@Blood", info.Blood);
                sqlParameters[8] = new SqlParameter("@Damage", info.Damage);
                sqlParameters[9] = new SqlParameter("@Guard", info.Guard);
                sqlParameters[10] = new SqlParameter("@AttackGrow", info.AttackGrow);
                sqlParameters[11] = new SqlParameter("@DefenceGrow", info.DefenceGrow);
                sqlParameters[12] = new SqlParameter("@LuckGrow", info.LuckGrow);
                sqlParameters[13] = new SqlParameter("@AgilityGrow", info.AgilityGrow);
                sqlParameters[14] = new SqlParameter("@BloodGrow", info.BloodGrow);
                sqlParameters[15] = new SqlParameter("@DamageGrow", info.DamageGrow);
                sqlParameters[0x10] = new SqlParameter("@GuardGrow", info.GuardGrow);
                sqlParameters[0x11] = new SqlParameter("@Level", info.Level);
                sqlParameters[0x12] = new SqlParameter("@GP", info.GP);
                sqlParameters[0x13] = new SqlParameter("@MaxGP", info.MaxGP);
                sqlParameters[20] = new SqlParameter("@Hunger", info.Hunger);
                sqlParameters[0x15] = new SqlParameter("@PetHappyStar", info.PetHappyStar);
                sqlParameters[0x16] = new SqlParameter("@MP", info.MP);
                sqlParameters[0x17] = new SqlParameter("@IsEquip", info.IsEquip);
                sqlParameters[0x18] = new SqlParameter("@Skill", info.Skill);
                sqlParameters[0x19] = new SqlParameter("@SkillEquip", info.SkillEquip);
                sqlParameters[0x1a] = new SqlParameter("@Place", info.Place);
                sqlParameters[0x1b] = new SqlParameter("@IsExit", info.IsExit);
                sqlParameters[0x1c] = new SqlParameter("@currentStarExp", info.currentStarExp);
                sqlParameters[0x1d] = new SqlParameter("@ID", info.ID);
                sqlParameters[0x1d].Direction = ParameterDirection.Output;
                sqlParameters[30] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[30].Direction = ParameterDirection.ReturnValue;
                flag = base.db.RunProcedure("SP_User_Add_Pet", sqlParameters);
                int num = (int) sqlParameters[30].Value;
                flag = num == 0;
                info.ID = (int) sqlParameters[0x1d].Value;
                info.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool AddUserRank(UserRankInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[14];
                sqlParameters[0] = new SqlParameter("@ID", item.ID);
                sqlParameters[0].Direction = ParameterDirection.Output;
                sqlParameters[1] = new SqlParameter("@UserID", item.UserID);
                sqlParameters[2] = new SqlParameter("@UserRank", item.UserRank);
                sqlParameters[3] = new SqlParameter("@Attack", item.Attack);
                sqlParameters[4] = new SqlParameter("@Defence", item.Defence);
                sqlParameters[5] = new SqlParameter("@Luck", item.Luck);
                sqlParameters[6] = new SqlParameter("@Agility", item.Agility);
                sqlParameters[7] = new SqlParameter("@HP", item.HP);
                sqlParameters[8] = new SqlParameter("@Damage", item.Damage);
                sqlParameters[9] = new SqlParameter("@Guard", item.Guard);
                sqlParameters[10] = new SqlParameter("@BeginDate", item.BeginDate.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[11] = new SqlParameter("@Validate", item.Validate);
                sqlParameters[12] = new SqlParameter("@IsExit", item.IsExit);
                sqlParameters[13] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[13].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UserRank_Add", sqlParameters);
                flag = ((int) sqlParameters[13].Value) == 0;
                item.ID = (int) sqlParameters[0].Value;
                item.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool AddUsersExtra(UsersExtraInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[0x11];
                sqlParameters[0] = new SqlParameter("@ID", info.ID);
                sqlParameters[0].Direction = ParameterDirection.Output;
                sqlParameters[1] = new SqlParameter("@UserID", info.UserID);
                sqlParameters[2] = new SqlParameter("@starlevel", info.starlevel);
                sqlParameters[3] = new SqlParameter("@nowPosition", info.nowPosition);
                sqlParameters[4] = new SqlParameter("@FreeCount", info.FreeCount);
                sqlParameters[5] = new SqlParameter("@SearchGoodItems", info.SearchGoodItems);
                sqlParameters[6] = new SqlParameter("@FreeAddAutionCount", info.FreeAddAutionCount);
                sqlParameters[7] = new SqlParameter("@FreeSendMailCount", info.FreeSendMailCount);
                sqlParameters[8] = new SqlParameter("@KingBlessInfo", info.KingBleesInfo);
                sqlParameters[9] = new SqlParameter("@MissionEnergy", info.MissionEnergy);
                sqlParameters[10] = new SqlParameter("@buyEnergyCount", info.buyEnergyCount);
                sqlParameters[11] = new SqlParameter("@KingBlessEnddate", info.KingBlessEnddate);
                sqlParameters[12] = new SqlParameter("@KingBlessIndex", info.KingBlessIndex);
                sqlParameters[13] = new SqlParameter("@LastTimeHotSpring", info.LastTimeHotSpring);
                sqlParameters[14] = new SqlParameter("@LastFreeTimeHotSpring", info.LastFreeTimeHotSpring);
                sqlParameters[15] = new SqlParameter("@MinHotSpring", info.MinHotSpring);
                sqlParameters[0x10] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[0x10].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UsersExtra_Add", sqlParameters);
                flag = ((int) sqlParameters[13].Value) == 0;
                info.ID = (int) sqlParameters[0].Value;
                info.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_UsersExtra_Add", exception);
                }
            }
            return flag;
        }

        public bool AddUserTreasureInfo(UserTreasureInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[11];
                sqlParameters[0] = new SqlParameter("@ID", item.ID);
                sqlParameters[0].Direction = ParameterDirection.Output;
                sqlParameters[1] = new SqlParameter("@UserID", item.UserID);
                sqlParameters[2] = new SqlParameter("@NickName", item.NickName);
                sqlParameters[3] = new SqlParameter("@logoinDays", item.logoinDays);
                sqlParameters[4] = new SqlParameter("@treasure", item.treasure);
                sqlParameters[5] = new SqlParameter("@treasureAdd", item.treasureAdd);
                sqlParameters[6] = new SqlParameter("@friendHelpTimes", item.friendHelpTimes);
                sqlParameters[7] = new SqlParameter("@isEndTreasure", item.isEndTreasure);
                sqlParameters[8] = new SqlParameter("@isBeginTreasure", item.isBeginTreasure);
                sqlParameters[9] = new SqlParameter("@LastLoginDay", item.LastLoginDay.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[10] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[10].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_Users_Treasure_Add", sqlParameters);
                flag = ((int) sqlParameters[10].Value) == 0;
                item.ID = (int) sqlParameters[0].Value;
                item.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool AddUserUserDrill(UserDrillInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", item.UserID), new SqlParameter("@BeadPlace", item.BeadPlace), new SqlParameter("@HoleExp", item.HoleExp), new SqlParameter("@HoleLv", item.HoleLv), new SqlParameter("@DrillPlace", item.DrillPlace), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[5].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_Users_UserDrill_Add", sqlParameters);
                int num = (int) sqlParameters[5].Value;
                flag = num == 0;
                item.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool CancelPaymentMail(int userid, int mailID, ref int senderID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[4];
                sqlParameters[0] = new SqlParameter("@userid", userid);
                sqlParameters[1] = new SqlParameter("@mailID", mailID);
                sqlParameters[2] = new SqlParameter("@senderID", SqlDbType.Int);
                sqlParameters[2].Value = (int) senderID;
                sqlParameters[2].Direction = ParameterDirection.InputOutput;
                sqlParameters[3] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[3].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_Mail_PaymentCancel", sqlParameters);
                int num = (int) sqlParameters[3].Value;
                flag = num == 0;
                if (flag)
                {
                    senderID = (int) sqlParameters[2].Value;
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool ClearAdoptPet(int ID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[1].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_Clear_AdoptPet", sqlParameters);
                int num = (int) sqlParameters[1].Value;
                flag = num == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool ClearDatabase()
        {
            bool flag = false;
            try
            {
                flag = base.db.RunProcedure("SP_Sys_Clear_All");
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool ChargeToUser(string userName, ref int money, string nickName)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("@UserName", userName);
                sqlParameters[1] = new SqlParameter("@money", SqlDbType.Int);
                sqlParameters[1].Direction = ParameterDirection.Output;
                sqlParameters[2] = new SqlParameter("@NickName", nickName);
                flag = base.db.RunProcedure("SP_Charge_To_User", sqlParameters);
                money = (int) sqlParameters[1].Value;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool CheckAccount(string username, string password)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@Username", username), new SqlParameter("@Password", password), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[2].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_CheckAccount", sqlParameters);
                int num = (int) sqlParameters[2].Value;
                flag = num == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool CheckEmailIsValid(string Email)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@Email", Email), new SqlParameter("@count", SqlDbType.BigInt) };
                sqlParameters[1].Direction = ParameterDirection.Output;
                base.db.RunProcedure("CheckEmailIsValid", sqlParameters);
                if (int.Parse(sqlParameters[1].Value.ToString()) == 0)
                {
                    flag = true;
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init CheckEmailIsValid", exception);
                }
            }
            return flag;
        }

        public bool DeleteAllFields(int ID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", ID), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[1].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_RemoveAllFields", sqlParameters);
                flag = ((int) sqlParameters[1].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_RemoveAllFields", exception);
                }
            }
            return flag;
        }

        public bool DeleteAuction(int auctionID, int userID, ref string msg)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@AuctionID", auctionID), new SqlParameter("@UserID", userID), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[2].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_Auction_Delete", sqlParameters);
                int num = (int) sqlParameters[2].Value;
                flag = num == 0;
                switch (num)
                {
                    case 0:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg1", new object[0]);
                        return flag;

                    case 1:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg2", new object[0]);
                        return flag;

                    case 2:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg3", new object[0]);
                        return flag;
                }
                msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg4", new object[0]);
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool DeleteFriends(int UserID, int FriendID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", FriendID), new SqlParameter("@UserID", UserID) };
                flag = base.db.RunProcedure("SP_Users_Friends_Delete", sqlParameters);
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool DeleteGoods(int itemID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", itemID) };
                flag = base.db.RunProcedure("SP_Users_Items_Delete", sqlParameters);
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool DeleteMail(int UserID, int mailID, out int senderID)
        {
            bool flag = false;
            senderID = 0;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[4];
                sqlParameters[0] = new SqlParameter("@ID", mailID);
                sqlParameters[1] = new SqlParameter("@UserID", UserID);
                sqlParameters[2] = new SqlParameter("@SenderID", SqlDbType.Int);
                sqlParameters[2].Value = (int) senderID;
                sqlParameters[2].Direction = ParameterDirection.InputOutput;
                sqlParameters[3] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[3].Direction = ParameterDirection.ReturnValue;
                flag = base.db.RunProcedure("SP_Mail_Delete", sqlParameters);
                if (((int) sqlParameters[3].Value) == 0)
                {
                    flag = true;
                    senderID = (int) sqlParameters[2].Value;
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool DeleteMail2(int UserID, int mailID, out int senderID)
        {
            bool flag = false;
            senderID = 0;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[4];
                sqlParameters[0] = new SqlParameter("@ID", mailID);
                sqlParameters[1] = new SqlParameter("@UserID", UserID);
                sqlParameters[2] = new SqlParameter("@SenderID", SqlDbType.Int);
                sqlParameters[2].Value = (int) senderID;
                sqlParameters[2].Direction = ParameterDirection.InputOutput;
                sqlParameters[3] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[3].Direction = ParameterDirection.ReturnValue;
                flag = base.db.RunProcedure("SP_Mail_Delete", sqlParameters);
                if (((int) sqlParameters[3].Value) == 0)
                {
                    flag = true;
                    senderID = (int) sqlParameters[2].Value;
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool DeleteMarryInfo(int ID, int userID, ref string msg)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@UserID", userID), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[2].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_MarryInfo_Delete", sqlParameters);
                int num = (int) sqlParameters[2].Value;
                flag = num == 0;
                if (num == 0)
                {
                    msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Succeed", new object[0]);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("DeleteAuction", exception);
                }
            }
            return flag;
        }

        public bool DeleteUserDressModel(UserDressModelInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", item.ID), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[1].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_DressModel_Delete", sqlParameters);
                flag = ((int) sqlParameters[1].Value) == 0;
                item.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_DressModel_Delete", exception);
                }
            }
            return flag;
        }

        public bool DisableUser(string userName, bool isExit)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserName", userName), new SqlParameter("@IsExist", isExit), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[2].Direction = ParameterDirection.ReturnValue;
                flag = base.db.RunProcedure("SP_Disable_User", sqlParameters);
                if (((int) sqlParameters[2].Value) == 0)
                {
                    flag = true;
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("DisableUser", exception);
                }
            }
            return flag;
        }

        public bool DisposeMarryRoomInfo(int ID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[1].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_Dispose_Marry_Room_Info", sqlParameters);
                flag = ((int) sqlParameters[1].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("DisposeMarryRoomInfo", exception);
                }
            }
            return flag;
        }

        public UsersPetinfo GetAdoptPetSingle(int PetID)
        {
            new UsersPetinfo();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = PetID;
                base.db.GetReader(ref resultDataReader, "SP_AdoptPet_By_Id", sqlParameters);
                if (resultDataReader.Read())
                {
                    return this.InitPet(resultDataReader);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public ActiveSystemInfo[] GetAllActiveSystemData()
        {
            List<ActiveSystemInfo> list = new List<ActiveSystemInfo>();
            SqlDataReader resultDataReader = null;
            int num = 1;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_ActiveSystem_All");
                while (resultDataReader.Read())
                {
                    ActiveSystemInfo item = new ActiveSystemInfo {
                        UserID = (int) resultDataReader["UserID"],
                        useableScore = (int) resultDataReader["useableScore"],
                        totalScore = (int) resultDataReader["totalScore"],
                        NickName = (string) resultDataReader["NickName"],
                        myRank = num,
                        CanGetGift = (bool) resultDataReader["CanGetGift"]
                    };
                    list.Add(item);
                    num++;
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllActiveSystem", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public ActivitySystemItemInfo[] GetAllActivitySystemItem()
        {
            List<ActivitySystemItemInfo> list = new List<ActivitySystemItemInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_ActivitySystemItem_All");
                while (resultDataReader.Read())
                {
                    ActivitySystemItemInfo item = new ActivitySystemItemInfo {
                        ID = (int) resultDataReader["ID"],
                        ActivityType = (int) resultDataReader["ActivityType"],
                        Quality = (int) resultDataReader["Quality"],
                        TemplateID = (int) resultDataReader["TemplateID"],
                        Count = (int) resultDataReader["Count"],
                        ValidDate = (int) resultDataReader["ValidDate"],
                        IsBinds = (bool) resultDataReader["IsBinds"],
                        StrengthenLevel = (int) resultDataReader["StrengthenLevel"],
                        AttackCompose = (int) resultDataReader["AttackCompose"],
                        DefendCompose = (int) resultDataReader["DefendCompose"],
                        AgilityCompose = (int) resultDataReader["AgilityCompose"],
                        LuckCompose = (int) resultDataReader["LuckCompose"],
                        Random = (int) resultDataReader["Random"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllActivitySystemItem", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public CardGrooveUpdateInfo[] GetAllCardGrooveUpdate()
        {
            List<CardGrooveUpdateInfo> list = new List<CardGrooveUpdateInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_CardGrooveUpdate_All");
                while (resultDataReader.Read())
                {
                    list.Add(this.InitCardGrooveUpdate(resultDataReader));
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllCardGrooveUpdate", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public CardUpdateConditionInfo[] GetAllCardUpdateCondition()
        {
            List<CardUpdateConditionInfo> list = new List<CardUpdateConditionInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Get_CardUpdateCondiction");
                while (resultDataReader.Read())
                {
                    CardUpdateConditionInfo item = new CardUpdateConditionInfo {
                        Level = (int) resultDataReader["Level"],
                        Exp = (int) resultDataReader["Exp"],
                        MinExp = (int) resultDataReader["MinExp"],
                        MaxExp = (int) resultDataReader["MaxExp"],
                        UpdateCardCount = (int) resultDataReader["UpdateCardCount"],
                        ResetCardCount = (int) resultDataReader["ResetCardCount"],
                        ResetMoney = (int) resultDataReader["ResetMoney"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public CardUpdateInfo[] GetAllCardUpdateInfo()
        {
            List<CardUpdateInfo> list = new List<CardUpdateInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Get_CardUpdateInfo");
                while (resultDataReader.Read())
                {
                    CardUpdateInfo item = new CardUpdateInfo {
                        Id = (int) resultDataReader["Id"],
                        Level = (int) resultDataReader["Level"],
                        Attack = (int) resultDataReader["Attack"],
                        Defend = (int) resultDataReader["Defend"],
                        Agility = (int) resultDataReader["Agility"],
                        Lucky = (int) resultDataReader["Lucky"],
                        Guard = (int) resultDataReader["Guard"],
                        Damage = (int) resultDataReader["Damage"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public ClothGroupTemplateInfo[] GetAllClothGroup()
        {
            List<ClothGroupTemplateInfo> list = new List<ClothGroupTemplateInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_ClothGroup_All");
                while (resultDataReader.Read())
                {
                    ClothGroupTemplateInfo item = new ClothGroupTemplateInfo {
                        ItemID = (int) resultDataReader["ItemID"],
                        ID = (int) resultDataReader["ID"],
                        TemplateID = (int) resultDataReader["TemplateID"],
                        Sex = (int) resultDataReader["Sex"],
                        Description = (int) resultDataReader["Description"],
                        Cost = (int) resultDataReader["Cost"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_ClothGroup_All", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public ClothPropertyTemplateInfo[] GetAllClothProperty()
        {
            List<ClothPropertyTemplateInfo> list = new List<ClothPropertyTemplateInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_ClothProperty_All");
                while (resultDataReader.Read())
                {
                    ClothPropertyTemplateInfo item = new ClothPropertyTemplateInfo {
                        ID = (int) resultDataReader["ID"],
                        Sex = (int) resultDataReader["Sex"],
                        Name = (string) resultDataReader["Name"],
                        Attack = (int) resultDataReader["Attack"],
                        Defend = (int) resultDataReader["Defend"],
                        Luck = (int) resultDataReader["Luck"],
                        Agility = (int) resultDataReader["Agility"],
                        Blood = (int) resultDataReader["Blood"],
                        Damage = (int) resultDataReader["Damage"],
                        Guard = (int) resultDataReader["Guard"],
                        Cost = (int) resultDataReader["Cost"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllClothProperty", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public CommunalActiveInfo[] GetAllCommunalActive()
        {
            List<CommunalActiveInfo> list = new List<CommunalActiveInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_CommunalActive_All");
                while (resultDataReader.Read())
                {
                    CommunalActiveInfo item = new CommunalActiveInfo {
                        ActiveID = (int) resultDataReader["ActiveID"],
                        BeginTime = (DateTime) resultDataReader["BeginTime"],
                        EndTime = (DateTime) resultDataReader["EndTime"],
                        LimitGrade = (int) resultDataReader["LimitGrade"],
                        DayMaxScore = (int) resultDataReader["DayMaxScore"],
                        MinScore = (int) resultDataReader["MinScore"],
                        AddPropertyByMoney = (string) resultDataReader["AddPropertyByMoney"],
                        AddPropertyByProp = (string) resultDataReader["AddPropertyByProp"],
                        IsReset = (bool) resultDataReader["IsReset"],
                        IsSendAward = (bool) resultDataReader["IsSendAward"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllCommunalActive", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public CommunalActiveAwardInfo[] GetAllCommunalActiveAward()
        {
            List<CommunalActiveAwardInfo> list = new List<CommunalActiveAwardInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_CommunalActiveAward_All");
                while (resultDataReader.Read())
                {
                    CommunalActiveAwardInfo item = new CommunalActiveAwardInfo {
                        ID = (int) resultDataReader["ID"],
                        ActiveID = (int) resultDataReader["ActiveID"],
                        IsArea = (int) resultDataReader["IsArea"],
                        RandID = (int) resultDataReader["RandID"],
                        TemplateID = (int) resultDataReader["TemplateID"],
                        StrengthenLevel = (int) resultDataReader["StrengthenLevel"],
                        AttackCompose = (int) resultDataReader["AttackCompose"],
                        DefendCompose = (int) resultDataReader["DefendCompose"],
                        AgilityCompose = (int) resultDataReader["AgilityCompose"],
                        LuckCompose = (int) resultDataReader["LuckCompose"],
                        Count = (int) resultDataReader["Count"],
                        IsBind = (bool) resultDataReader["IsBind"],
                        IsTime = (bool) resultDataReader["IsTime"],
                        ValidDate = (int) resultDataReader["ValidDate"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllCommunalActiveAward", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public CommunalActiveExpInfo[] GetAllCommunalActiveExp()
        {
            List<CommunalActiveExpInfo> list = new List<CommunalActiveExpInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_CommunalActiveExp_All");
                while (resultDataReader.Read())
                {
                    CommunalActiveExpInfo item = new CommunalActiveExpInfo {
                        ActiveID = (int) resultDataReader["ActiveID"],
                        Grade = (int) resultDataReader["Grade"],
                        Exp = (int) resultDataReader["Exp"],
                        AddExpPlus = (int) resultDataReader["AddExpPlus"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllCommunalActiveExp", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public List<UserDressModelInfo> GetAllDressModel(int userId)
        {
            SqlDataReader resultDataReader = null;
            List<UserDressModelInfo> list = new List<UserDressModelInfo>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = userId;
                base.db.GetReader(ref resultDataReader, "SP_Get_AllDressModel", sqlParameters);
                UserDressModelInfo info = new UserDressModelInfo();
                while (resultDataReader.Read())
                {
                    UserDressModelInfo item = new UserDressModelInfo {
                        ID = (int) resultDataReader["ID"],
                        SlotID = (int) resultDataReader["SlotID"],
                        ItemID = (int) resultDataReader["ItemID"],
                        TemplateID = (int) resultDataReader["TemplateID"],
                        CategoryID = (int) resultDataReader["CategoryID"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_Get_AllDressModel", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list;
        }

        public ExerciseInfo[] GetAllExercise()
        {
            List<ExerciseInfo> list = new List<ExerciseInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Exercise_All");
                while (resultDataReader.Read())
                {
                    ExerciseInfo item = new ExerciseInfo {
                        Grage = (int) resultDataReader["Grage"],
                        GP = (int) resultDataReader["GP"],
                        ExerciseA = (int) resultDataReader["ExerciseA"],
                        ExerciseAG = (int) resultDataReader["ExerciseAG"],
                        ExerciseD = (int) resultDataReader["ExerciseD"],
                        ExerciseH = (int) resultDataReader["ExerciseH"],
                        ExerciseL = (int) resultDataReader["ExerciseL"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllExercise", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public FairBattleRewardInfo[] GetAllFairBattleReward()
        {
            List<FairBattleRewardInfo> list = new List<FairBattleRewardInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_FairBattleReward_All");
                while (resultDataReader.Read())
                {
                    FairBattleRewardInfo item = new FairBattleRewardInfo {
                        Prestige = (int) resultDataReader["Prestige"],
                        Level = (int) resultDataReader["Level"],
                        Name = (string) resultDataReader["Name"],
                        PrestigeForWin = (int) resultDataReader["PrestigeForWin"],
                        PrestigeForLose = (int) resultDataReader["PrestigeForLose"],
                        Title = (string) resultDataReader["Title"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllFairBattleReward", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public FightSpiritTemplateInfo[] GetAllFightSpiritTemplate()
        {
            List<FightSpiritTemplateInfo> list = new List<FightSpiritTemplateInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_FightSpiritTemplate_All");
                while (resultDataReader.Read())
                {
                    FightSpiritTemplateInfo item = new FightSpiritTemplateInfo {
                        ID = (int) resultDataReader["ID"],
                        FightSpiritID = (int) resultDataReader["FightSpiritID"],
                        FightSpiritIcon = (string) resultDataReader["FightSpiritIcon"],
                        Level = (int) resultDataReader["Level"],
                        Exp = (int) resultDataReader["Exp"],
                        Attack = (int) resultDataReader["Attack"],
                        Defence = (int) resultDataReader["Defence"],
                        Agility = (int) resultDataReader["Agility"],
                        Lucky = (int) resultDataReader["Lucky"],
                        Blood = (int) resultDataReader["Blood"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetFightSpiritTemplateAll", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public LuckyStartToptenAwardInfo[] GetAllLanternriddlesTopTenAward()
        {
            List<LuckyStartToptenAwardInfo> list = new List<LuckyStartToptenAwardInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_LanternriddlesTopTenAward_All");
                while (resultDataReader.Read())
                {
                    LuckyStartToptenAwardInfo item = new LuckyStartToptenAwardInfo {
                        ID = (int) resultDataReader["ID"],
                        Type = (int) resultDataReader["Type"],
                        TemplateID = (int) resultDataReader["TemplateID"],
                        Validate = (int) resultDataReader["Validate"],
                        Count = (int) resultDataReader["Count"],
                        StrengthenLevel = (int) resultDataReader["StrengthenLevel"],
                        AttackCompose = (int) resultDataReader["AttackCompose"],
                        DefendCompose = (int) resultDataReader["DefendCompose"],
                        AgilityCompose = (int) resultDataReader["AgilityCompose"],
                        LuckCompose = (int) resultDataReader["LuckCompose"],
                        IsBinds = (bool) resultDataReader["IsBind"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetLuckyStart_Topten_Award_All", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public LevelInfo[] GetAllLevel()
        {
            List<LevelInfo> list = new List<LevelInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Level_All");
                while (resultDataReader.Read())
                {
                    LevelInfo item = new LevelInfo {
                        Grade = (int) resultDataReader["Grade"],
                        GP = (int) resultDataReader["GP"],
                        Blood = (int) resultDataReader["Blood"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllLevel", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public LightriddleQuestInfo[] GetAllLightriddleQuestInfo()
        {
            List<LightriddleQuestInfo> list = new List<LightriddleQuestInfo>();
            SqlDataReader resultDataReader = null;
            int num = 1;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Lightriddle_Quest_All");
                while (resultDataReader.Read())
                {
                    LightriddleQuestInfo item = new LightriddleQuestInfo {
                        QuestionID = (int) resultDataReader["QuestionID"],
                        QuestionContent = (string) resultDataReader["QuestionContent"],
                        Option1 = (string) resultDataReader["Option1"],
                        Option2 = (string) resultDataReader["Option2"],
                        Option3 = (string) resultDataReader["Option3"],
                        Option4 = (string) resultDataReader["Option4"],
                        OptionTrue = (int) resultDataReader["OptionTrue"]
                    };
                    list.Add(item);
                    num++;
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_Lightriddle_Quest_All", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public LuckyStartToptenAwardInfo[] GetAllLuckyStartToptenAward()
        {
            List<LuckyStartToptenAwardInfo> list = new List<LuckyStartToptenAwardInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_LuckyStart_Topten_Award_All");
                while (resultDataReader.Read())
                {
                    LuckyStartToptenAwardInfo item = new LuckyStartToptenAwardInfo {
                        ID = (int) resultDataReader["ID"],
                        Type = (int) resultDataReader["Type"],
                        TemplateID = (int) resultDataReader["TemplateID"],
                        Validate = (int) resultDataReader["Validate"],
                        Count = (int) resultDataReader["Count"],
                        StrengthenLevel = (int) resultDataReader["StrengthenLevel"],
                        AttackCompose = (int) resultDataReader["AttackCompose"],
                        DefendCompose = (int) resultDataReader["DefendCompose"],
                        AgilityCompose = (int) resultDataReader["AgilityCompose"],
                        LuckCompose = (int) resultDataReader["LuckCompose"],
                        IsBinds = (bool) resultDataReader["IsBind"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetLuckyStart_Topten_Award_All", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public MagicStoneTemplateInfo[] GetAllMagicStoneTemplate()
        {
            List<MagicStoneTemplateInfo> list = new List<MagicStoneTemplateInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_MagicStoneTemplate_All");
                while (resultDataReader.Read())
                {
                    MagicStoneTemplateInfo item = new MagicStoneTemplateInfo {
                        ID = (int) resultDataReader["ID"],
                        TemplateID = (int) resultDataReader["TemplateID"],
                        Level = (int) resultDataReader["Level"],
                        Exp = (int) resultDataReader["Exp"],
                        Attack = (int) resultDataReader["Attack"],
                        Defence = (int) resultDataReader["Defence"],
                        Agility = (int) resultDataReader["Agility"],
                        Luck = (int) resultDataReader["Luck"],
                        MagicAttack = (int) resultDataReader["MagicAttack"],
                        MagicDefence = (int) resultDataReader["MagicDefence"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllMagicStoneTemplate", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

		public ConsortiaUserInfo[] GetAllMemberByConsortia(int ConsortiaID)
				{
					List<ConsortiaUserInfo> list = new List<ConsortiaUserInfo>();
					SqlDataReader ResultDataReader = null;
					try
					{
						SqlParameter[] SqlParameters = new SqlParameter[]
						{
							new SqlParameter("@ConsortiaID", SqlDbType.Int, 4)
						};
						SqlParameters[0].Value = ConsortiaID;
						this.db.GetReader(ref ResultDataReader, "SP_Consortia_Users_All", SqlParameters);
						while (ResultDataReader.Read())
						{
							list.Add(this.InitConsortiaUserInfo(ResultDataReader));
						}
					}
					catch (Exception ex)
					{
						if (BaseBussiness.log.IsErrorEnabled)
						{
							BaseBussiness.log.Error("Init", ex);
						}
					}
					finally
					{
						if (ResultDataReader != null && !ResultDataReader.IsClosed)
						{
							ResultDataReader.Close();
						}
					}
					return list.ToArray();
				}

        public PetConfig[] GetAllPetConfig()
        {
            List<PetConfig> list = new List<PetConfig>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_PetConfig_All");
                while (resultDataReader.Read())
                {
                    PetConfig item = new PetConfig {
                        ID = (int) resultDataReader["ID"],
                        Name = resultDataReader["Name"].ToString(),
                        Value = resultDataReader["Value"].ToString()
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllPetConfig", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public PetExpItemPriceInfo[] GetAllPetExpItemPriceInfoInfo()
        {
            List<PetExpItemPriceInfo> list = new List<PetExpItemPriceInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_PetExpItemPriceInfo_All");
                while (resultDataReader.Read())
                {
                    PetExpItemPriceInfo item = new PetExpItemPriceInfo {
                        ID = (int) resultDataReader["ID"],
                        Count = (int) resultDataReader["Count"],
                        Money = (int) resultDataReader["Money"],
                        ItemCount = (int) resultDataReader["ItemCount"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllPetTemplateInfo", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public PetFightPropertyInfo[] GetAllPetFightProp()
        {
            List<PetFightPropertyInfo> list = new List<PetFightPropertyInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Get_AllPetFightProperty");
                while (resultDataReader.Read())
                {
                    PetFightPropertyInfo item = new PetFightPropertyInfo {
                        ID = (int) resultDataReader["ID"],
                        Agility = (int) resultDataReader["Agility"],
                        Attack = (int) resultDataReader["Attack"],
                        Blood = (int) resultDataReader["Blood"],
                        Defence = (int) resultDataReader["Defence"],
                        Exp = (int) resultDataReader["Exp"],
                        Lucky = (int) resultDataReader["Lucky"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("PetFightProperty", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public PetLevel[] GetAllPetLevel()
        {
            List<PetLevel> list = new List<PetLevel>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_PetLevel_All");
                while (resultDataReader.Read())
                {
                    PetLevel item = new PetLevel {
                        Level = (int) resultDataReader["Level"],
                        GP = (int) resultDataReader["GP"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllPetLevel", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public PetSkillElementInfo[] GetAllPetSkillElementInfo()
        {
            List<PetSkillElementInfo> list = new List<PetSkillElementInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_PetSkillElementInfo_All");
                while (resultDataReader.Read())
                {
                    PetSkillElementInfo item = new PetSkillElementInfo {
                        ID = (int) resultDataReader["ID"],
                        Name = resultDataReader["Name"].ToString(),
                        EffectPic = resultDataReader["EffectPic"].ToString(),
                        Description = resultDataReader["Description"].ToString(),
                        Pic = (int) resultDataReader["Pic"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllPetSkillElementInfo", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public PetSkillInfo[] GetAllPetSkillInfo()
        {
            List<PetSkillInfo> list = new List<PetSkillInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_PetSkillInfo_All");
                while (resultDataReader.Read())
                {
                    PetSkillInfo item = new PetSkillInfo {
                        ID = (int) resultDataReader["ID"],
                        Name = resultDataReader["Name"].ToString(),
                        ElementIDs = resultDataReader["ElementIDs"].ToString(),
                        Description = resultDataReader["Description"].ToString(),
                        BallType = (int) resultDataReader["BallType"],
                        NewBallID = (int) resultDataReader["NewBallID"],
                        CostMP = (int) resultDataReader["CostMP"],
                        Pic = (int) resultDataReader["Pic"],
                        Action = resultDataReader["Action"].ToString(),
                        EffectPic = resultDataReader["EffectPic"].ToString(),
                        Delay = (int) resultDataReader["Delay"],
                        ColdDown = (int) resultDataReader["ColdDown"],
                        GameType = (int) resultDataReader["GameType"],
                        Probability = (int) resultDataReader["Probability"],
                        Damage = (int) resultDataReader["Damage"],
                        DamageCrit = (int) resultDataReader["DamageCrit"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllPetSkillInfo", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public PetSkillTemplateInfo[] GetAllPetSkillTemplateInfo()
        {
            List<PetSkillTemplateInfo> list = new List<PetSkillTemplateInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_PetSkillTemplateInfo_All");
                while (resultDataReader.Read())
                {
                    PetSkillTemplateInfo item = new PetSkillTemplateInfo {
                        ID = (int) resultDataReader["ID"],
                        PetTemplateID = (int) resultDataReader["PetTemplateID"],
                        KindID = (int) resultDataReader["KindID"],
                        GetTypes = (int) resultDataReader["GetType"],
                        SkillID = (int) resultDataReader["SkillID"],
                        SkillBookID = (int) resultDataReader["SkillBookID"],
                        MinLevel = (int) resultDataReader["MinLevel"],
                        DeleteSkillIDs = resultDataReader["DeleteSkillIDs"].ToString()
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllPetSkillTemplateInfo", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public PetStarExpInfo[] GetAllPetStarExp()
        {
            List<PetStarExpInfo> list = new List<PetStarExpInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Get_AllPetStarExp");
                while (resultDataReader.Read())
                {
                    PetStarExpInfo item = new PetStarExpInfo {
                        Exp = (int) resultDataReader["Exp"],
                        NewID = (int) resultDataReader["NewID"],
                        OldID = (int) resultDataReader["OldID"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("PetStarExp", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public PetTemplateInfo[] GetAllPetTemplateInfo()
        {
            List<PetTemplateInfo> list = new List<PetTemplateInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_PetTemplateInfo_All");
                while (resultDataReader.Read())
                {
                    PetTemplateInfo item = new PetTemplateInfo {
                        ID = (int) resultDataReader["ID"],
                        TemplateID = (int) resultDataReader["TemplateID"],
                        Name = resultDataReader["Name"].ToString(),
                        KindID = (int) resultDataReader["KindID"],
                        Description = resultDataReader["Description"].ToString(),
                        Pic = resultDataReader["Pic"].ToString(),
                        RareLevel = (int) resultDataReader["RareLevel"],
                        MP = (int) resultDataReader["MP"],
                        StarLevel = (int) resultDataReader["StarLevel"],
                        GameAssetUrl = resultDataReader["GameAssetUrl"].ToString(),
                        EvolutionID = (int) resultDataReader["EvolutionID"],
                        HighAgility = (int) resultDataReader["HighAgility"],
                        HighAgilityGrow = (int) resultDataReader["HighAgilityGrow"],
                        HighAttack = (int) resultDataReader["HighAttack"],
                        HighAttackGrow = (int) resultDataReader["HighAttackGrow"],
                        HighBlood = (int) resultDataReader["HighBlood"],
                        HighBloodGrow = (int) resultDataReader["HighBloodGrow"],
                        HighDamage = (int) resultDataReader["HighDamage"],
                        HighDamageGrow = (int) resultDataReader["HighDamageGrow"],
                        HighDefence = (int) resultDataReader["HighDefence"],
                        HighDefenceGrow = (int) resultDataReader["HighDefenceGrow"],
                        HighGuard = (int) resultDataReader["HighGuard"],
                        HighGuardGrow = (int) resultDataReader["HighGuardGrow"],
                        HighLuck = (int) resultDataReader["HighLuck"],
                        HighLuckGrow = (int) resultDataReader["HighLuckGrow"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllPetTemplateInfo", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public SubActiveInfo[] GetAllSubActive()
        {
            List<SubActiveInfo> list = new List<SubActiveInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_SubActive_All");
                while (resultDataReader.Read())
                {
                    SubActiveInfo item = new SubActiveInfo {
                        ID = (int) resultDataReader["ID"],
                        ActiveID = (int) resultDataReader["ActiveID"],
                        SubID = (int) resultDataReader["SubID"],
                        IsOpen = (bool) resultDataReader["IsOpen"],
                        StartDate = (DateTime) resultDataReader["StartDate"],
                        StartTime = (DateTime) resultDataReader["StartTime"],
                        EndDate = (DateTime) resultDataReader["EndDate"],
                        EndTime = (DateTime) resultDataReader["EndTime"],
                        IsContinued = (bool) resultDataReader["IsContinued"],
                        ActiveInfo = (resultDataReader["ActiveInfo"] == null) ? "" : resultDataReader["ActiveInfo"].ToString()
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init AllSubActive", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public SubActiveConditionInfo[] GetAllSubActiveCondition(int ActiveID)
        {
            List<SubActiveConditionInfo> list = new List<SubActiveConditionInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ActiveID", ActiveID) };
                base.db.GetReader(ref resultDataReader, "SP_SubActiveCondition_All", sqlParameters);
                while (resultDataReader.Read())
                {
                    SubActiveConditionInfo item = new SubActiveConditionInfo {
                        ID = (int) resultDataReader["ID"],
                        ActiveID = (int) resultDataReader["ActiveID"],
                        SubID = (int) resultDataReader["SubID"],
                        ConditionID = (int) resultDataReader["ConditionID"],
                        Type = (int) resultDataReader["Type"],
                        Value = (resultDataReader["Value"] == null) ? "" : resultDataReader["Value"].ToString(),
                        AwardType = (int) resultDataReader["AwardType"],
                        AwardValue = (resultDataReader["AwardValue"] == null) ? "" : resultDataReader["AwardValue"].ToString(),
                        IsValid = (bool) resultDataReader["IsValid"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init AllSubActive", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public TotemInfo[] GetAllTotem()
        {
            List<TotemInfo> list = new List<TotemInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Totem_All");
                while (resultDataReader.Read())
                {
                    TotemInfo item = new TotemInfo {
                        ID = (int) resultDataReader["ID"],
                        ConsumeExp = (int) resultDataReader["ConsumeExp"],
                        ConsumeHonor = (int) resultDataReader["ConsumeHonor"],
                        AddAttack = (int) resultDataReader["AddAttack"],
                        AddDefence = (int) resultDataReader["AddDefence"],
                        AddAgility = (int) resultDataReader["AddAgility"],
                        AddLuck = (int) resultDataReader["AddLuck"],
                        AddBlood = (int) resultDataReader["AddBlood"],
                        AddDamage = (int) resultDataReader["AddDamage"],
                        AddGuard = (int) resultDataReader["AddGuard"],
                        Random = (int) resultDataReader["Random"],
                        Page = (int) resultDataReader["Page"],
                        Layers = (int) resultDataReader["Layers"],
                        Location = (int) resultDataReader["Location"],
                        Point = (int) resultDataReader["Point"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetTotemAll", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public TotemHonorTemplateInfo[] GetAllTotemHonorTemplate()
        {
            List<TotemHonorTemplateInfo> list = new List<TotemHonorTemplateInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_TotemHonorTemplate_All");
                while (resultDataReader.Read())
                {
                    TotemHonorTemplateInfo item = new TotemHonorTemplateInfo {
                        ID = (int) resultDataReader["ID"],
                        NeedMoney = (int) resultDataReader["NeedMoney"],
                        Type = (int) resultDataReader["Type"],
                        AddHonor = (int) resultDataReader["AddHonor"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetTotemHonorTemplateInfo", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public TreasureAwardInfo[] GetAllTreasureAward()
        {
            List<TreasureAwardInfo> list = new List<TreasureAwardInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Treasure_All");
                while (resultDataReader.Read())
                {
                    TreasureAwardInfo item = new TreasureAwardInfo {
                        ID = (int) resultDataReader["ID"],
                        TemplateID = (int) resultDataReader["TemplateID"],
                        Name = (string) resultDataReader["Name"],
                        Count = (int) resultDataReader["Count"],
                        Validate = (int) resultDataReader["Validate"],
                        Random = (int) resultDataReader["Random"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetTreasureAwardAll", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public UserMatchInfo[] GetAllUserMatchInfo()
        {
            List<UserMatchInfo> list = new List<UserMatchInfo>();
            SqlDataReader resultDataReader = null;
            int num = 1;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_UserMatch_All_DESC");
                while (resultDataReader.Read())
                {
                    UserMatchInfo item = new UserMatchInfo {
                        UserID = (int) resultDataReader["UserID"],
                        totalPrestige = (int) resultDataReader["totalPrestige"],
                        rank = num
                    };
                    list.Add(item);
                    num++;
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllUserMatchDESC", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public AuctionInfo[] GetAuctionPage(int page, string name, int type, int pay, ref int total, int userID, int buyID, int order, bool sort, int size, string AuctionIDs)
        {
            List<AuctionInfo> list = new List<AuctionInfo>();
            try
            {
                string str = " IsExist=1 ";
                if (!string.IsNullOrEmpty(name))
                {
                    str = str + " and Name like '%" + name + "%' ";
                }
                switch (type)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 0x10:
                    case 0x11:
                    {
                        object obj2 = str;
                        str = string.Concat(new object[] { obj2, " and Category =", type, " " });
                        break;
                    }
                    case 0x15:
                        str = str + " and Category in(1,2,5,8,9) ";
                        break;

                    case 0x16:
                        str = str + " and Category in(13,15,6,4,3) ";
                        break;

                    case 0x17:
                        str = str + " and Category in(16,11,10) ";
                        break;

                    case 0x18:
                        str = str + " and Category in(8,9) ";
                        break;

                    case 0x19:
                        str = str + " and Category in (7,17) ";
                        break;

                    case 0x1a:
                        str = str + " and TemplateId>=311000 and TemplateId<=313999";
                        break;

                    case 0x1b:
                        str = str + " and TemplateId>=311000 and TemplateId<=311999 ";
                        break;

                    case 0x1c:
                        str = str + " and TemplateId>=312000 and TemplateId<=312999 ";
                        break;

                    case 0x1d:
                        str = str + " and TemplateId>=313000 and TempLateId<=313999";
                        break;

                    case 0x44c:
                        str = str + " and TemplateID in (11019,11021,11022,11023) ";
                        break;

                    case 0x44d:
                        str = str + " and TemplateID='11019' ";
                        break;

                    case 0x44e:
                        str = str + " and TemplateID='11021' ";
                        break;

                    case 0x44f:
                        str = str + " and TemplateID='11022' ";
                        break;

                    case 0x450:
                        str = str + " and TemplateID='11023' ";
                        break;

                    case 0x451:
                        str = str + " and TemplateID in (11001,11002,11003,11004,11005,11006,11007,11008,11009,11010,11011,11012,11013,11014,11015,11016) ";
                        break;

                    case 0x452:
                        str = str + " and TemplateID in (11001,11002,11003,11004) ";
                        break;

                    case 0x453:
                        str = str + " and TemplateID in (11005,11006,11007,11008) ";
                        break;

                    case 0x454:
                        str = str + " and TemplateID in (11009,11010,11011,11012) ";
                        break;

                    case 0x455:
                        str = str + " and TemplateID in (11013,11014,11015,11016) ";
                        break;
                }
                if (pay != -1)
                {
                    object obj3 = str;
                    str = string.Concat(new object[] { obj3, " and PayType =", pay, " " });
                }
                if (userID != -1)
                {
                    object obj4 = str;
                    str = string.Concat(new object[] { obj4, " and AuctioneerID =", userID, " " });
                }
                if (buyID != -1)
                {
                    object obj5 = str;
                    str = string.Concat(new object[] { obj5, " and (BuyerID =", buyID, " or AuctionID in (", AuctionIDs, ")) " });
                }
                string str2 = "Category,Name,Price,dd,AuctioneerID";
                switch (order)
                {
                    case 0:
                        str2 = "Name";
                        break;

                    case 2:
                        str2 = "dd";
                        break;

                    case 3:
                        str2 = "AuctioneerName";
                        break;

                    case 4:
                        str2 = "Price";
                        break;

                    case 5:
                        str2 = "BuyerName";
                        break;
                }
                str2 = str2 + (sort ? " desc" : "") + ",AuctionID ";
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@QueryStr", "V_Auction_Scan"), new SqlParameter("@QueryWhere", str), new SqlParameter("@PageSize", size), new SqlParameter("@PageCurrent", page), new SqlParameter("@FdShow", "*"), new SqlParameter("@FdOrder", str2), new SqlParameter("@FdKey", "AuctionID"), new SqlParameter("@TotalRow", (int) total) };
                sqlParameters[7].Direction = ParameterDirection.Output;
                DataTable table = base.db.GetDataTable("Auction", "SP_CustomPage", sqlParameters);
                total = (int) sqlParameters[7].Value;
                foreach (DataRow row in table.Rows)
                {
                    AuctionInfo item = new AuctionInfo {
                        AuctioneerID = (int) row["AuctioneerID"],
                        AuctioneerName = row["AuctioneerName"].ToString(),
                        AuctionID = (int) row["AuctionID"],
                        BeginDate = (DateTime) row["BeginDate"],
                        BuyerID = (int) row["BuyerID"],
                        BuyerName = row["BuyerName"].ToString(),
                        Category = (int) row["Category"],
                        IsExist = (bool) row["IsExist"],
                        ItemID = (int) row["ItemID"],
                        Name = row["Name"].ToString(),
                        Mouthful = (int) row["Mouthful"],
                        PayType = (int) row["PayType"],
                        Price = (int) row["Price"],
                        Rise = (int) row["Rise"],
                        ValidDate = (int) row["ValidDate"],
                        goodsCount = (int) row["goodsCount"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return list.ToArray();
        }

        public AuctionInfo GetAuctionSingle(int auctionID)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@AuctionID", auctionID) };
                base.db.GetReader(ref resultDataReader, "SP_Auction_Single", sqlParameters);
                if (resultDataReader.Read())
                {
                    return this.InitAuctionInfo(resultDataReader);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public BestEquipInfo[] GetCelebByDayBestEquip()
        {
            List<BestEquipInfo> list = new List<BestEquipInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Users_BestEquip");
                while (resultDataReader.Read())
                {
                    BestEquipInfo item = new BestEquipInfo {
                        Date = (DateTime) resultDataReader["RemoveDate"],
                        GP = (int) resultDataReader["GP"],
                        Grade = (int) resultDataReader["Grade"],
                        ItemName = (resultDataReader["Name"] == null) ? "" : resultDataReader["Name"].ToString(),
                        NickName = (resultDataReader["NickName"] == null) ? "" : resultDataReader["NickName"].ToString(),
                        Sex = (bool) resultDataReader["Sex"],
                        Strengthenlevel = (int) resultDataReader["Strengthenlevel"],
                        UserName = (resultDataReader["UserName"] == null) ? "" : resultDataReader["UserName"].ToString()
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public ChargeRecordInfo[] GetChargeRecordInfo(DateTime date, int SaveRecordSecond)
        {
            List<ChargeRecordInfo> list = new List<ChargeRecordInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@Date", date.ToString("yyyy-MM-dd HH:mm:ss")), new SqlParameter("@Second", SaveRecordSecond) };
                base.db.GetReader(ref resultDataReader, "SP_Charge_Record", sqlParameters);
                while (resultDataReader.Read())
                {
                    ChargeRecordInfo item = new ChargeRecordInfo {
                        BoyTotalPay = (int) resultDataReader["BoyTotalPay"],
                        GirlTotalPay = (int) resultDataReader["GirlTotalPay"],
                        PayWay = (resultDataReader["PayWay"] == null) ? "" : resultDataReader["PayWay"].ToString(),
                        TotalBoy = (int) resultDataReader["TotalBoy"],
                        TotalGirl = (int) resultDataReader["TotalGirl"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public PetEquipDataInfo[] GetEqPetSingles(int UserID)
        {
            List<PetEquipDataInfo> list = new List<PetEquipDataInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_eqPet_Single", sqlParameters);
                while (resultDataReader.Read())
                {
                    PetEquipDataInfo item = new PetEquipDataInfo(ItemMgr.FindItemTemplate((int) resultDataReader["eqTemplateID"])) {
                        ID = (int) resultDataReader["ID"],
                        UserID = (int) resultDataReader["UserID"],
                        PetID = (int) resultDataReader["PetID"],
                        eqType = (int) resultDataReader["eqType"],
                        eqTemplateID = (int) resultDataReader["eqTemplateID"],
                        startTime = (DateTime) resultDataReader["startTime"],
                        ValidDate = (int) resultDataReader["ValidDate"],
                        IsExit = (bool) resultDataReader["IsExit"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public ExerciseInfo GetExerciseSingle(int Grade)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@Grage", Grade) };
                base.db.GetReader(ref resultDataReader, "SP_Get_Exercise_By_Grade", sqlParameters);
                if (resultDataReader.Read())
                {
                    return new ExerciseInfo { Grage = (int) resultDataReader["Grage"], GP = (int) resultDataReader["GP"], ExerciseA = (int) resultDataReader["ExerciseA"], ExerciseAG = (int) resultDataReader["ExerciseAG"], ExerciseD = (int) resultDataReader["ExerciseD"], ExerciseH = (int) resultDataReader["ExerciseH"], ExerciseL = (int) resultDataReader["ExerciseL"] };
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetExerciseInfoSingle", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public FriendInfo[] GetFriendsAll(int UserID)
        {
            List<FriendInfo> list = new List<FriendInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_Users_Friends", sqlParameters);
                while (resultDataReader.Read())
                {
                    FriendInfo item = new FriendInfo {
                        AddDate = (DateTime) resultDataReader["AddDate"],
                        Colors = (resultDataReader["Colors"] == null) ? "" : resultDataReader["Colors"].ToString(),
                        FriendID = (int) resultDataReader["FriendID"],
                        Grade = (int) resultDataReader["Grade"],
                        Hide = (int) resultDataReader["Hide"],
                        ID = (int) resultDataReader["ID"],
                        IsExist = (bool) resultDataReader["IsExist"],
                        NickName = (resultDataReader["NickName"] == null) ? "" : resultDataReader["NickName"].ToString(),
                        Remark = (resultDataReader["Remark"] == null) ? "" : resultDataReader["Remark"].ToString(),
                        Sex = ((bool) resultDataReader["Sex"]) ? 1 : 0,
                        State = (int) resultDataReader["State"],
                        Style = (resultDataReader["Style"] == null) ? "" : resultDataReader["Style"].ToString(),
                        UserID = (int) resultDataReader["UserID"],
                        ConsortiaName = (resultDataReader["ConsortiaName"] == null) ? "" : resultDataReader["ConsortiaName"].ToString(),
                        Offer = (int) resultDataReader["Offer"],
                        Win = (int) resultDataReader["Win"],
                        Total = (int) resultDataReader["Total"],
                        Escape = (int) resultDataReader["Escape"],
                        Relation = (int) resultDataReader["Relation"],
                        Repute = (int) resultDataReader["Repute"],
                        UserName = (resultDataReader["UserName"] == null) ? "" : resultDataReader["UserName"].ToString(),
                        DutyName = (resultDataReader["DutyName"] == null) ? "" : resultDataReader["DutyName"].ToString(),
                        Nimbus = (int) resultDataReader["Nimbus"],
                        typeVIP = Convert.ToByte(resultDataReader["typeVIP"]),
                        VIPLevel = (int) resultDataReader["VIPLevel"],
                        IsMarried = (bool) resultDataReader["IsMarried"],
                        LastDate = (DateTime) resultDataReader["AddDate"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public FriendInfo[] GetFriendsBbs(string condictArray)
        {
            List<FriendInfo> list = new List<FriendInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@SearchUserName", SqlDbType.NVarChar, 0xfa0) };
                sqlParameters[0].Value = condictArray;
                base.db.GetReader(ref resultDataReader, "SP_Users_FriendsBbs", sqlParameters);
                while (resultDataReader.Read())
                {
                    FriendInfo item = new FriendInfo {
                        NickName = (resultDataReader["NickName"] == null) ? "" : resultDataReader["NickName"].ToString(),
                        UserID = (int) resultDataReader["UserID"],
                        UserName = (resultDataReader["UserName"] == null) ? "" : resultDataReader["UserName"].ToString(),
                        IsExist = ((int) resultDataReader["UserID"]) > 0
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public ArrayList GetFriendsGood(string UserName)
        {
            ArrayList list = new ArrayList();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserName", SqlDbType.NVarChar) };
                sqlParameters[0].Value = UserName;
                base.db.GetReader(ref resultDataReader, "SP_Users_Friends_Good", sqlParameters);
                while (resultDataReader.Read())
                {
                    list.Add((resultDataReader["UserName"] == null) ? "" : resultDataReader["UserName"].ToString());
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list;
        }

        public Dictionary<int, int> GetFriendsIDAll(int UserID)
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_Users_Friends_All", sqlParameters);
                while (resultDataReader.Read())
                {
                    if (!dictionary.ContainsKey((int) resultDataReader["FriendID"]))
                    {
                        dictionary.Add((int) resultDataReader["FriendID"], (int) resultDataReader["Relation"]);
                    }
                    else
                    {
                        dictionary[(int) resultDataReader["FriendID"]] = (int) resultDataReader["Relation"];
                    }
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return dictionary;
        }

        public LuckStarRewardRecordInfo[] GetLuckStarTopTenRank(int MinUseNum)
        {
            List<LuckStarRewardRecordInfo> list = new List<LuckStarRewardRecordInfo>();
            SqlDataReader resultDataReader = null;
            int num = 1;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@MinUseNum", MinUseNum) };
                base.db.GetReader(ref resultDataReader, "SP_LuckStar_Reward_Record_All", sqlParameters);
                while (resultDataReader.Read())
                {
                    LuckStarRewardRecordInfo item = new LuckStarRewardRecordInfo {
                        PlayerID = (int) resultDataReader["UserID"],
                        useStarNum = (int) resultDataReader["useStarNum"],
                        isVip = (int) resultDataReader["isVip"],
                        nickName = (string) resultDataReader["nickName"],
                        rank = num
                    };
                    list.Add(item);
                    num++;
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init SP_LuckStar_Reward_Record_All", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public MailInfo[] GetMailBySenderID(int userID)
        {
            List<MailInfo> list = new List<MailInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = userID;
                base.db.GetReader(ref resultDataReader, "SP_Mail_BySenderID", sqlParameters);
                while (resultDataReader.Read())
                {
                    list.Add(this.InitMail(resultDataReader));
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public MailInfo[] GetMailByUserID(int userID)
        {
            List<MailInfo> list = new List<MailInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = userID;
                base.db.GetReader(ref resultDataReader, "SP_Mail_ByUserID", sqlParameters);
                while (resultDataReader.Read())
                {
                    list.Add(this.InitMail(resultDataReader));
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public MailInfo GetMailSingle(int UserID, int mailID)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", mailID), new SqlParameter("@UserID", UserID) };
                base.db.GetReader(ref resultDataReader, "SP_Mail_Single", sqlParameters);
                if (resultDataReader.Read())
                {
                    return this.InitMail(resultDataReader);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public MarryInfo[] GetMarryInfoPage(int page, string name, bool sex, int size, ref int total)
        {
            List<MarryInfo> list = new List<MarryInfo>();
            try
            {
                string str;
                if (sex)
                {
                    str = " IsExist=1 and Sex=1 and UserExist=1";
                }
                else
                {
                    str = " IsExist=1 and Sex=0 and UserExist=1";
                }
                if (!string.IsNullOrEmpty(name))
                {
                    str = str + " and NickName like '%" + name + "%' ";
                }
                string str2 = "State desc,IsMarried";
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@QueryStr", "V_Sys_Marry_Info"), new SqlParameter("@QueryWhere", str), new SqlParameter("@PageSize", size), new SqlParameter("@PageCurrent", page), new SqlParameter("@FdShow", "*"), new SqlParameter("@FdOrder", str2), new SqlParameter("@FdKey", "ID"), new SqlParameter("@TotalRow", (int) total) };
                sqlParameters[7].Direction = ParameterDirection.Output;
                DataTable table = base.db.GetDataTable("V_Sys_Marry_Info", "SP_CustomPage", sqlParameters);
                total = (int) sqlParameters[7].Value;
                foreach (DataRow row in table.Rows)
                {
                    MarryInfo item = new MarryInfo {
                        ID = (int) row["ID"],
                        UserID = (int) row["UserID"],
                        IsPublishEquip = (bool) row["IsPublishEquip"],
                        Introduction = row["Introduction"].ToString(),
                        NickName = row["NickName"].ToString(),
                        IsConsortia = (bool) row["IsConsortia"],
                        ConsortiaID = (int) row["ConsortiaID"],
                        Sex = (bool) row["Sex"],
                        Win = (int) row["Win"],
                        Total = (int) row["Total"],
                        Escape = (int) row["Escape"],
                        GP = (int) row["GP"],
                        Honor = row["Honor"].ToString(),
                        Style = row["Style"].ToString(),
                        Colors = row["Colors"].ToString(),
                        Hide = (int) row["Hide"],
                        Grade = (int) row["Grade"],
                        State = (int) row["State"],
                        Repute = (int) row["Repute"],
                        Skin = row["Skin"].ToString(),
                        Offer = (int) row["Offer"],
                        IsMarried = (bool) row["IsMarried"],
                        ConsortiaName = row["ConsortiaName"].ToString(),
                        DutyName = row["DutyName"].ToString(),
                        Nimbus = (int) row["Nimbus"],
                        FightPower = (int) row["FightPower"],
                        typeVIP = Convert.ToByte(row["typeVIP"]),
                        VIPLevel = (int) row["VIPLevel"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return list.ToArray();
        }

        public MarryInfo GetMarryInfoSingle(int ID)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", ID) };
                base.db.GetReader(ref resultDataReader, "SP_MarryInfo_Single", sqlParameters);
                if (resultDataReader.Read())
                {
                    return new MarryInfo { ID = (int) resultDataReader["ID"], UserID = (int) resultDataReader["UserID"], IsPublishEquip = (bool) resultDataReader["IsPublishEquip"], Introduction = resultDataReader["Introduction"].ToString(), RegistTime = (DateTime) resultDataReader["RegistTime"] };
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetMarryInfoSingle", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public MarryProp GetMarryProp(int id)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", id) };
                base.db.GetReader(ref resultDataReader, "SP_Select_Marry_Prop", sqlParameters);
                if (resultDataReader.Read())
                {
                    return new MarryProp { IsMarried = (bool) resultDataReader["IsMarried"], SpouseID = (int) resultDataReader["SpouseID"], SpouseName = resultDataReader["SpouseName"].ToString(), IsCreatedMarryRoom = (bool) resultDataReader["IsCreatedMarryRoom"], SelfMarryRoomID = (int) resultDataReader["SelfMarryRoomID"], IsGotRing = (bool) resultDataReader["IsGotRing"] };
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetMarryProp", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public MarryRoomInfo[] GetMarryRoomInfo()
        {
            SqlDataReader resultDataReader = null;
            List<MarryRoomInfo> list = new List<MarryRoomInfo>();
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Get_Marry_Room_Info");
                while (resultDataReader.Read())
                {
                    MarryRoomInfo item = new MarryRoomInfo {
                        ID = (int) resultDataReader["ID"],
                        Name = resultDataReader["Name"].ToString(),
                        PlayerID = (int) resultDataReader["PlayerID"],
                        PlayerName = resultDataReader["PlayerName"].ToString(),
                        GroomID = (int) resultDataReader["GroomID"],
                        GroomName = resultDataReader["GroomName"].ToString(),
                        BrideID = (int) resultDataReader["BrideID"],
                        BrideName = resultDataReader["BrideName"].ToString(),
                        Pwd = resultDataReader["Pwd"].ToString(),
                        AvailTime = (int) resultDataReader["AvailTime"],
                        MaxCount = (int) resultDataReader["MaxCount"],
                        GuestInvite = (bool) resultDataReader["GuestInvite"],
                        MapIndex = (int) resultDataReader["MapIndex"],
                        BeginTime = (DateTime) resultDataReader["BeginTime"],
                        BreakTime = (DateTime) resultDataReader["BreakTime"],
                        RoomIntroduction = resultDataReader["RoomIntroduction"].ToString(),
                        ServerID = (int) resultDataReader["ServerID"],
                        IsHymeneal = (bool) resultDataReader["IsHymeneal"],
                        IsGunsaluteUsed = (bool) resultDataReader["IsGunsaluteUsed"]
                    };
                    list.Add(item);
                }
                return list.ToArray();
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetMarryRoomInfo", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public MarryRoomInfo GetMarryRoomInfoSingle(int id)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", id) };
                base.db.GetReader(ref resultDataReader, "SP_Get_Marry_Room_Info_Single", sqlParameters);
                if (resultDataReader.Read())
                {
                    return new MarryRoomInfo { 
                        ID = (int) resultDataReader["ID"], Name = resultDataReader["Name"].ToString(), PlayerID = (int) resultDataReader["PlayerID"], PlayerName = resultDataReader["PlayerName"].ToString(), GroomID = (int) resultDataReader["GroomID"], GroomName = resultDataReader["GroomName"].ToString(), BrideID = (int) resultDataReader["BrideID"], BrideName = resultDataReader["BrideName"].ToString(), Pwd = resultDataReader["Pwd"].ToString(), AvailTime = (int) resultDataReader["AvailTime"], MaxCount = (int) resultDataReader["MaxCount"], GuestInvite = (bool) resultDataReader["GuestInvite"], MapIndex = (int) resultDataReader["MapIndex"], BeginTime = (DateTime) resultDataReader["BeginTime"], BreakTime = (DateTime) resultDataReader["BreakTime"], RoomIntroduction = resultDataReader["RoomIntroduction"].ToString(), 
                        ServerID = (int) resultDataReader["ServerID"], IsHymeneal = (bool) resultDataReader["IsHymeneal"], IsGunsaluteUsed = (bool) resultDataReader["IsGunsaluteUsed"]
                     };
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetMarryRoomInfo", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public void GetPasswordInfo(int userID, ref string PasswordQuestion1, ref string PasswordAnswer1, ref string PasswordQuestion2, ref string PasswordAnswer2, ref int Count)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", userID) };
                base.db.GetReader(ref resultDataReader, "SP_Users_PasswordInfo", sqlParameters);
                while (resultDataReader.Read())
                {
                    PasswordQuestion1 = (resultDataReader["PasswordQuestion1"] == null) ? "" : resultDataReader["PasswordQuestion1"].ToString();
                    PasswordAnswer1 = (resultDataReader["PasswordAnswer1"] == null) ? "" : resultDataReader["PasswordAnswer1"].ToString();
                    PasswordQuestion2 = (resultDataReader["PasswordQuestion2"] == null) ? "" : resultDataReader["PasswordQuestion2"].ToString();
                    PasswordAnswer2 = (resultDataReader["PasswordAnswer2"] == null) ? "" : resultDataReader["PasswordAnswer2"].ToString();
                    DateTime time = (DateTime) resultDataReader["LastFindDate"];
                    if (time == DateTime.Today)
                    {
                        Count = (int) resultDataReader["FailedPasswordAttemptCount"];
                    }
                    else
                    {
                        Count = 5;
                    }
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
        }

        public Dictionary<int, UserDrillInfo> GetPlayerDrillByID(int UserID)
        {
            Dictionary<int, UserDrillInfo> dictionary = new Dictionary<int, UserDrillInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_Users_Drill_All", sqlParameters);
                while (resultDataReader.Read())
                {
                    UserDrillInfo info = new UserDrillInfo {
                        UserID = (int) resultDataReader["UserID"],
                        BeadPlace = (int) resultDataReader["BeadPlace"],
                        HoleLv = (int) resultDataReader["HoleLv"],
                        HoleExp = (int) resultDataReader["HoleExp"],
                        DrillPlace = (int) resultDataReader["DrillPlace"]
                    };
                    if (!dictionary.ContainsKey(info.DrillPlace))
                    {
                        dictionary.Add(info.DrillPlace, info);
                    }
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return dictionary;
        }

        public MarryApplyInfo[] GetPlayerMarryApply(int UserID)
        {
            SqlDataReader resultDataReader = null;
            List<MarryApplyInfo> list = new List<MarryApplyInfo>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", UserID) };
                base.db.GetReader(ref resultDataReader, "SP_Get_Marry_Apply", sqlParameters);
                while (resultDataReader.Read())
                {
                    MarryApplyInfo item = new MarryApplyInfo {
                        UserID = (int) resultDataReader["UserID"],
                        ApplyUserID = (int) resultDataReader["ApplyUserID"],
                        ApplyUserName = resultDataReader["ApplyUserName"].ToString(),
                        ApplyType = (int) resultDataReader["ApplyType"],
                        ApplyResult = (bool) resultDataReader["ApplyResult"],
                        LoveProclamation = resultDataReader["LoveProclamation"].ToString(),
                        ID = (int) resultDataReader["Id"]
                    };
                    list.Add(item);
                }
                return list.ToArray();
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetPlayerMarryApply", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public PlayerInfo[] GetPlayerPage(int page, int size, ref int total, int order, int userID, ref bool resultValue)
        {
            List<PlayerInfo> list = new List<PlayerInfo>();
            try
            {
                string queryWhere = " IsExist=1 and IsFirst<> 0 ";
                if (userID != -1)
                {
                    queryWhere = string.Concat(new object[]
					{
						queryWhere,
						" and UserID =",
						userID,
						" "
					});
                }
                string str = "GP desc";
                switch (order)
                {
                    case 1:
                        str = "Offer desc";
                        break;
                    case 2:
                        str = "AddDayGP desc";
                        break;
                    case 3:
                        str = "AddWeekGP desc";
                        break;
                    case 4:
                        str = "AddDayOffer desc";
                        break;
                    case 5:
                        str = "AddWeekOffer desc";
                        break;
                    case 6:
                        str = "FightPower desc";
                        break;
                    case 7:
                        str = "AchievementPoint desc";
                        break;
                    case 8:
                        str = "AddDayAchievementPoint desc";
                        break;
                    case 9:
                        str = "AddWeekAchievementPoint desc";
                        break;
                    case 10:
                        str = "GiftGp desc";
                        break;
                    case 11:
                        str = "AddDayGiftGp desc";
                        break;
                    case 12:
                        str = "AddWeekGiftGp desc";
                        break;
                }
                string fdOreder = str + ",UserID";
                foreach (DataRow dataRow in base.GetPage("V_Sys_Users_Detail", queryWhere, page, size, "*", fdOreder, "UserID", ref total).Rows)
                {
                    PlayerInfo playerInfo = new PlayerInfo();
                    playerInfo.Agility = (int)dataRow["Agility"];
                    playerInfo.Attack = (int)dataRow["Attack"];
                    playerInfo.Colors = ((dataRow["Colors"] == null) ? "" : dataRow["Colors"].ToString());
                    playerInfo.ConsortiaID = (int)dataRow["ConsortiaID"];
                    playerInfo.Defence = (int)dataRow["Defence"];
                    playerInfo.Gold = (int)dataRow["Gold"];
                    playerInfo.GP = (int)dataRow["GP"];
                    playerInfo.Grade = (int)dataRow["Grade"];
                    playerInfo.ID = (int)dataRow["UserID"];
                    playerInfo.Luck = (int)dataRow["Luck"];
                    playerInfo.Money = (int)dataRow["Money"];
                    playerInfo.NickName = ((dataRow["NickName"] == null) ? "" : dataRow["NickName"].ToString());
                    playerInfo.Sex = (bool)dataRow["Sex"];
                    playerInfo.State = (int)dataRow["State"];
                    playerInfo.Style = ((dataRow["Style"] == null) ? "" : dataRow["Style"].ToString());
                    playerInfo.Hide = (int)dataRow["Hide"];
                    playerInfo.Repute = (int)dataRow["Repute"];
                    playerInfo.UserName = ((dataRow["UserName"] == null) ? "" : dataRow["UserName"].ToString());
                    playerInfo.ConsortiaName = ((dataRow["ConsortiaName"] == null) ? "" : dataRow["ConsortiaName"].ToString());
                    playerInfo.Offer = (int)dataRow["Offer"];
                    playerInfo.Skin = ((dataRow["Skin"] == null) ? "" : dataRow["Skin"].ToString());
                    playerInfo.IsBanChat = (bool)dataRow["IsBanChat"];
                    playerInfo.ReputeOffer = (int)dataRow["ReputeOffer"];
                    playerInfo.ConsortiaRepute = (int)dataRow["ConsortiaRepute"];
                    playerInfo.ConsortiaLevel = (int)dataRow["ConsortiaLevel"];
                    playerInfo.StoreLevel = (int)dataRow["StoreLevel"];
                    playerInfo.ShopLevel = (int)dataRow["ShopLevel"];
                    playerInfo.SmithLevel = (int)dataRow["SmithLevel"];
                    playerInfo.ConsortiaHonor = (int)dataRow["ConsortiaHonor"];
                    playerInfo.RichesOffer = (int)dataRow["RichesOffer"];
                    playerInfo.RichesRob = (int)dataRow["RichesRob"];
                    playerInfo.DutyLevel = (int)dataRow["DutyLevel"];
                    playerInfo.DutyName = ((dataRow["DutyName"] == null) ? "" : dataRow["DutyName"].ToString());
                    playerInfo.Right = (int)dataRow["Right"];
                    playerInfo.ChairmanName = ((dataRow["ChairmanName"] == null) ? "" : dataRow["ChairmanName"].ToString());
                    playerInfo.Win = (int)dataRow["Win"];
                    playerInfo.Total = (int)dataRow["Total"];
                    playerInfo.Escape = (int)dataRow["Escape"];
                    playerInfo.AddDayGP = (((int)dataRow["AddDayGP"] == 0) ? playerInfo.GP : ((int)dataRow["AddDayGP"]));
                    playerInfo.AddDayOffer = (((int)dataRow["AddDayOffer"] == 0) ? playerInfo.Offer : ((int)dataRow["AddDayOffer"]));
                    playerInfo.AddWeekGP = (((int)dataRow["AddWeekGP"] == 0) ? playerInfo.GP : ((int)dataRow["AddWeekyGP"]));
                    playerInfo.AddWeekOffer = (((int)dataRow["AddWeekOffer"] == 0) ? playerInfo.Offer : ((int)dataRow["AddWeekOffer"]));
                    playerInfo.ConsortiaRiches = (int)dataRow["ConsortiaRiches"];
                    playerInfo.CheckCount = (int)dataRow["CheckCount"];
                    playerInfo.Nimbus = (int)dataRow["Nimbus"];
                    playerInfo.GiftToken = (int)dataRow["GiftToken"];
                    playerInfo.QuestSite = ((dataRow["QuestSite"] == null) ? new byte[200] : ((byte[])dataRow["QuestSite"]));
                    playerInfo.PvePermission = ((dataRow["PvePermission"] == null) ? "" : dataRow["PvePermission"].ToString());
                    playerInfo.FightPower = (int)dataRow["FightPower"];
                    list.Add(playerInfo);
                }
                resultValue = true;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", ex);
                }
            }
            return list.ToArray();
        }

        public ActiveSystemInfo GetSingleActiveSystem(int UserID)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_GetSingleActiveSystem", sqlParameters);
                if (resultDataReader.Read())
                {
                    return new ActiveSystemInfo { 
                        ID = (int) resultDataReader["ID"], UserID = (int) resultDataReader["UserID"], useableScore = (int) resultDataReader["useableScore"], totalScore = (int) resultDataReader["totalScore"], AvailTime = (int) resultDataReader["AvailTime"], NickName = (string) resultDataReader["NickName"], CanGetGift = (bool) resultDataReader["CanGetGift"], canOpenCounts = (int) resultDataReader["canOpenCounts"], canEagleEyeCounts = (int) resultDataReader["canEagleEyeCounts"], lastFlushTime = (DateTime) resultDataReader["lastFlushTime"], isShowAll = (bool) resultDataReader["isShowAll"], ActiveMoney = (int) resultDataReader["AvtiveMoney"], activityTanabataNum = (int) resultDataReader["activityTanabataNum"], ChallengeNum = (int) resultDataReader["ChallengeNum"], BuyBuffNum = (int) resultDataReader["BuyBuffNum"], lastEnterYearMonter = (DateTime) resultDataReader["lastEnterYearMonter"], 
                        DamageNum = (int) resultDataReader["DamageNum"], BoxState = (string) resultDataReader["BoxState"], LuckystarCoins = (int) resultDataReader["LuckystarCoins"]
                     };
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_GetSingleActiveSystem", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public List<UserAvatarCollectionInfo> GetSingleAvatarCollect(int userId)
        {
            SqlDataReader resultDataReader = null;
            List<UserAvatarCollectionInfo> list = new List<UserAvatarCollectionInfo>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = userId;
                base.db.GetReader(ref resultDataReader, "SP_Get_AvatarCollect", sqlParameters);
                UserAvatarCollectionInfo info = new UserAvatarCollectionInfo();
                while (resultDataReader.Read())
                {
                    UserAvatarCollectionInfo item = new UserAvatarCollectionInfo {
                        ID = (int) resultDataReader["ID"],
                        AvatarID = (int) resultDataReader["AvatarID"],
                        UserID = (int) resultDataReader["UserID"],
                        Sex = (int) resultDataReader["Sex"],
                        IsActive = (bool) resultDataReader["IsActive"],
                        Data = (string) resultDataReader["Data"],
                        TimeStart = (DateTime) resultDataReader["TimeStart"],
                        TimeEnd = (DateTime) resultDataReader["TimeEnd"],
                        IsExit = (bool) resultDataReader["IsExit"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_Get_AllDressModel", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list;
        }

        public UserBoguAdventureInfo GetSingleBoguAdventure(int UserID)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_GetSingleBoguAdventure", sqlParameters);
                if (resultDataReader.Read())
                {
                    return new UserBoguAdventureInfo { UserID = (int) resultDataReader["UserID"], CurrentPostion = (int) resultDataReader["CurrentPostion"], OpenCount = (int) resultDataReader["OpenCount"], ResetCount = (int) resultDataReader["ResetCount"], Map = (string) resultDataReader["Map"], Award = (string) resultDataReader["Award"], HP = (int) resultDataReader["HP"] };
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_GetSingleBoguAdventure", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public DiceDataInfo GetSingleDiceData(int UserID)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_GetSingle_DiceData", sqlParameters);
                if (resultDataReader.Read())
                {
                    return new DiceDataInfo { ID = (int) resultDataReader["ID"], UserID = (int) resultDataReader["UserID"], LuckIntegral = (int) resultDataReader["LuckIntegral"], LuckIntegralLevel = (int) resultDataReader["LuckIntegralLevel"], Level = (int) resultDataReader["Level"], FreeCount = (int) resultDataReader["FreeCount"], CurrentPosition = (int) resultDataReader["CurrentPosition"], UserFirstCell = (bool) resultDataReader["UserFirstCell"], AwardArray = (resultDataReader["AwardArray"] == null) ? "" : resultDataReader["AwardArray"].ToString() };
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_GetSingleDiceData", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public UserFarmInfo GetSingleFarm(int Id)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = Id;
                base.db.GetReader(ref resultDataReader, "SP_Get_SingleFarm", sqlParameters);
                if (resultDataReader.Read())
                {
                    return new UserFarmInfo { ID = (int) resultDataReader["ID"], FarmID = (int) resultDataReader["FarmID"], PayFieldMoney = (string) resultDataReader["PayFieldMoney"], PayAutoMoney = (string) resultDataReader["PayAutoMoney"], AutoPayTime = (DateTime) resultDataReader["AutoPayTime"], AutoValidDate = (int) resultDataReader["AutoValidDate"], VipLimitLevel = (int) resultDataReader["VipLimitLevel"], FarmerName = (string) resultDataReader["FarmerName"], GainFieldId = (int) resultDataReader["GainFieldId"], MatureId = (int) resultDataReader["MatureId"], KillCropId = (int) resultDataReader["KillCropId"], isAutoId = (int) resultDataReader["isAutoId"], isFarmHelper = (bool) resultDataReader["isFarmHelper"], buyExpRemainNum = (int) resultDataReader["buyExpRemainNum"], isArrange = (bool) resultDataReader["isArrange"] };
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetSingleFarm", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public UserFieldInfo[] GetSingleFields(int ID)
        {
            List<UserFieldInfo> list = new List<UserFieldInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = ID;
                base.db.GetReader(ref resultDataReader, "SP_Get_SingleFields", sqlParameters);
                while (resultDataReader.Read())
                {
                    UserFieldInfo item = new UserFieldInfo {
                        ID = (int) resultDataReader["ID"],
                        FarmID = (int) resultDataReader["FarmID"],
                        FieldID = (int) resultDataReader["FieldID"],
                        SeedID = (int) resultDataReader["SeedID"],
                        PlantTime = (DateTime) resultDataReader["PlantTime"],
                        AccelerateTime = (int) resultDataReader["AccelerateTime"],
                        FieldValidDate = (int) resultDataReader["FieldValidDate"],
                        PayTime = (DateTime) resultDataReader["PayTime"],
                        GainCount = (int) resultDataReader["GainCount"],
                        AutoSeedID = (int) resultDataReader["AutoSeedID"],
                        AutoFertilizerID = (int) resultDataReader["AutoFertilizerID"],
                        AutoSeedIDCount = (int) resultDataReader["AutoSeedIDCount"],
                        AutoFertilizerCount = (int) resultDataReader["AutoFertilizerCount"],
                        isAutomatic = (bool) resultDataReader["isAutomatic"],
                        AutomaticTime = (DateTime) resultDataReader["AutomaticTime"],
                        IsExit = (bool) resultDataReader["IsExit"],
                        payFieldTime = (int) resultDataReader["payFieldTime"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_GetSingleFields", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public List<UserGemStone> GetSingleGemStones(int ID)
        {
            List<UserGemStone> list = new List<UserGemStone>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = ID;
                base.db.GetReader(ref resultDataReader, "SP_GetSingleGemStone", sqlParameters);
                while (resultDataReader.Read())
                {
                    UserGemStone item = new UserGemStone {
                        ID = (int) resultDataReader["ID"],
                        UserID = (int) resultDataReader["UserID"],
                        FigSpiritId = (int) resultDataReader["FigSpiritId"],
                        FigSpiritIdValue = (string) resultDataReader["FigSpiritIdValue"],
                        EquipPlace = (int) resultDataReader["EquipPlace"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_GetSingleUserGemStones", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list;
        }

        public UserLabyrinthInfo GetSingleLabyrinth(int ID)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = ID;
                base.db.GetReader(ref resultDataReader, "SP_GetSingleLabyrinth", sqlParameters);
                if (resultDataReader.Read())
                {
                    return new UserLabyrinthInfo { 
                        UserID = (int) resultDataReader["UserID"], myProgress = (int) resultDataReader["myProgress"], myRanking = (int) resultDataReader["myRanking"], completeChallenge = (bool) resultDataReader["completeChallenge"], isDoubleAward = (bool) resultDataReader["isDoubleAward"], currentFloor = (int) resultDataReader["currentFloor"], accumulateExp = (int) resultDataReader["accumulateExp"], remainTime = (int) resultDataReader["remainTime"], currentRemainTime = (int) resultDataReader["currentRemainTime"], cleanOutAllTime = (int) resultDataReader["cleanOutAllTime"], cleanOutGold = (int) resultDataReader["cleanOutGold"], tryAgainComplete = (bool) resultDataReader["tryAgainComplete"], isInGame = (bool) resultDataReader["isInGame"], isCleanOut = (bool) resultDataReader["isCleanOut"], serverMultiplyingPower = (bool) resultDataReader["serverMultiplyingPower"], LastDate = (DateTime) resultDataReader["LastDate"], 
                        ProcessAward = (string) resultDataReader["ProcessAward"]
                     };
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_GetSingleUserLabyrinth", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public NewChickenBoxItemInfo[] GetSingleNewChickenBox(int UserID)
        {
            List<NewChickenBoxItemInfo> list = new List<NewChickenBoxItemInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_GetSingleNewChickenBox", sqlParameters);
                while (resultDataReader.Read())
                {
                    NewChickenBoxItemInfo item = new NewChickenBoxItemInfo {
                        ID = (int) resultDataReader["ID"],
                        UserID = (int) resultDataReader["UserID"],
                        TemplateID = (int) resultDataReader["TemplateID"],
                        Count = (int) resultDataReader["Count"],
                        ValidDate = (int) resultDataReader["ValidDate"],
                        StrengthenLevel = (int) resultDataReader["StrengthenLevel"],
                        AttackCompose = (int) resultDataReader["AttackCompose"],
                        DefendCompose = (int) resultDataReader["DefendCompose"],
                        AgilityCompose = (int) resultDataReader["AgilityCompose"],
                        LuckCompose = (int) resultDataReader["LuckCompose"],
                        Position = (int) resultDataReader["Position"],
                        IsSelected = (bool) resultDataReader["IsSelected"],
                        IsSeeded = (bool) resultDataReader["IsSeeded"],
                        IsBinds = (bool) resultDataReader["IsBinds"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_GetSingleNewChickenBox", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public PyramidInfo GetSinglePyramid(int UserID)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_GetSinglePyramid", sqlParameters);
                if (resultDataReader.Read())
                {
                    return new PyramidInfo { ID = (int) resultDataReader["ID"], UserID = (int) resultDataReader["UserID"], currentLayer = (int) resultDataReader["currentLayer"], maxLayer = (int) resultDataReader["maxLayer"], totalPoint = (int) resultDataReader["totalPoint"], turnPoint = (int) resultDataReader["turnPoint"], pointRatio = (int) resultDataReader["pointRatio"], currentFreeCount = (int) resultDataReader["currentFreeCount"], currentReviveCount = (int) resultDataReader["currentReviveCount"], isPyramidStart = (bool) resultDataReader["isPyramidStart"], LayerItems = (string) resultDataReader["LayerItems"] };
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_GetSinglePyramid", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public string GetSingleRandomName(int sex)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                if (sex > 1)
                {
                    sex = 1;
                }
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@Sex", SqlDbType.Int, 4) };
                sqlParameters[0].Value = sex;
                base.db.GetReader(ref resultDataReader, "SP_GetSingle_RandomName", sqlParameters);
                if (resultDataReader.Read())
                {
                    return ((resultDataReader["Name"] == null) ? "unknown" : resultDataReader["Name"].ToString());
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetSingleRandomName", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public UserTreasureInfo GetSingleTreasure(int UserID)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_GetSingleTreasure", sqlParameters);
                if (resultDataReader.Read())
                {
                    return new UserTreasureInfo { ID = (int) resultDataReader["ID"], UserID = (int) resultDataReader["UserID"], NickName = (string) resultDataReader["NickName"], logoinDays = (int) resultDataReader["logoinDays"], treasure = (int) resultDataReader["treasure"], treasureAdd = (int) resultDataReader["treasureAdd"], friendHelpTimes = (int) resultDataReader["friendHelpTimes"], isEndTreasure = (bool) resultDataReader["isEndTreasure"], isBeginTreasure = (bool) resultDataReader["isBeginTreasure"], LastLoginDay = (DateTime) resultDataReader["LastLoginDay"] };
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_GetSingleTreasure", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public List<TreasureDataInfo> GetSingleTreasureData(int UserID)
        {
            SqlDataReader resultDataReader = null;
            List<TreasureDataInfo> list = new List<TreasureDataInfo>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_GetSingleTreasureData", sqlParameters);
                while (resultDataReader.Read())
                {
                    TreasureDataInfo item = new TreasureDataInfo {
                        ID = (int) resultDataReader["ID"],
                        UserID = (int) resultDataReader["UserID"],
                        TemplateID = (int) resultDataReader["TemplateID"],
                        Count = (int) resultDataReader["Count"],
                        ValidDate = (int) resultDataReader["Validate"],
                        pos = (int) resultDataReader["pos"],
                        BeginDate = (DateTime) resultDataReader["BeginDate"],
                        IsExit = (bool) resultDataReader["IsExit"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_GetSingleTreasureData", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list;
        }

        public UserChristmasInfo GetSingleUserChristmas(int UserID)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_GetSingleUserChristmas", sqlParameters);
                if (resultDataReader.Read())
                {
                    return new UserChristmasInfo { ID = (int) resultDataReader["ID"], UserID = (int) resultDataReader["UserID"], exp = (int) resultDataReader["exp"], awardState = (int) resultDataReader["awardState"], count = (int) resultDataReader["count"], packsNumber = (int) resultDataReader["packsNumber"], lastPacks = (int) resultDataReader["lastPacks"], gameBeginTime = (DateTime) resultDataReader["gameBeginTime"], gameEndTime = (DateTime) resultDataReader["gameEndTime"], isEnter = (bool) resultDataReader["isEnter"], dayPacks = (int) resultDataReader["dayPacks"], AvailTime = (int) resultDataReader["AvailTime"] };
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_GetSingleUserChristmas", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public UserMatchInfo GetSingleUserMatchInfo(int UserID)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_GetSingleUserMatchInfo", sqlParameters);
                if (resultDataReader.Read())
                {
                    return new UserMatchInfo { ID = (int) resultDataReader["ID"], UserID = (int) resultDataReader["UserID"], dailyScore = (int) resultDataReader["dailyScore"], dailyWinCount = (int) resultDataReader["dailyWinCount"], dailyGameCount = (int) resultDataReader["dailyGameCount"], DailyLeagueFirst = (bool) resultDataReader["DailyLeagueFirst"], DailyLeagueLastScore = (int) resultDataReader["DailyLeagueLastScore"], weeklyScore = (int) resultDataReader["weeklyScore"], weeklyGameCount = (int) resultDataReader["weeklyGameCount"], weeklyRanking = (int) resultDataReader["weeklyRanking"], addDayPrestge = (int) resultDataReader["addDayPrestge"], totalPrestige = (int) resultDataReader["totalPrestige"], restCount = (int) resultDataReader["restCount"] };
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_GetSingleUserMatchInfo", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public List<UserRankInfo> GetSingleUserRank(int UserID)
        {
            SqlDataReader resultDataReader = null;
            List<UserRankInfo> list = new List<UserRankInfo>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_GetSingleUserRank", sqlParameters);
                while (resultDataReader.Read())
                {
                    UserRankInfo item = new UserRankInfo {
                        ID = (int) resultDataReader["ID"],
                        UserID = (int) resultDataReader["UserID"],
                        UserRank = (string) resultDataReader["UserRank"],
                        Attack = (int) resultDataReader["Attack"],
                        Defence = (int) resultDataReader["Defence"],
                        Luck = (int) resultDataReader["Luck"],
                        Agility = (int) resultDataReader["Agility"],
                        HP = (int) resultDataReader["HP"],
                        Damage = (int) resultDataReader["Damage"],
                        Guard = (int) resultDataReader["Guard"],
                        BeginDate = (DateTime) resultDataReader["BeginDate"],
                        Validate = (int) resultDataReader["Validate"],
                        IsExit = (bool) resultDataReader["IsExit"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_GetSingleUserRankInfo", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list;
        }

        public UsersExtraInfo GetSingleUsersExtra(int UserID)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_GetSingleUsersExtra", sqlParameters);
                if (resultDataReader.Read())
                {
                    return new UsersExtraInfo { 
                        ID = (int) resultDataReader["ID"], UserID = (int) resultDataReader["UserID"], LastTimeHotSpring = (DateTime) resultDataReader["LastTimeHotSpring"], LastFreeTimeHotSpring = (DateTime) resultDataReader["LastFreeTimeHotSpring"], starlevel = (int) resultDataReader["starlevel"], nowPosition = (int) resultDataReader["nowPosition"], FreeCount = (int) resultDataReader["FreeCount"], SearchGoodItems = (string) resultDataReader["SearchGoodItems"], FreeAddAutionCount = (int) resultDataReader["FreeAddAutionCount"], FreeSendMailCount = (int) resultDataReader["FreeSendMailCount"], KingBleesInfo = (string) resultDataReader["KingBlessInfo"], MissionEnergy = (int) resultDataReader["MissionEnergy"], buyEnergyCount = (int) resultDataReader["buyEnergyCount"], KingBlessEnddate = (DateTime) resultDataReader["KingBlessEnddate"], KingBlessIndex = (int) resultDataReader["KingBlessIndex"], MinHotSpring = (int) resultDataReader["MinHotSpring"], 
                        coupleBossEnterNum = (int) resultDataReader["coupleBossEnterNum"], coupleBossHurt = (int) resultDataReader["coupleBossHurt"], coupleBossBoxNum = (int) resultDataReader["coupleBossBoxNum"]
                     };
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_GetSingleUsersExtra", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public AchievementData[] GetUserAchievement(int userID)
        {
            List<AchievementData> list = new List<AchievementData>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = userID;
                base.db.GetReader(ref resultDataReader, "SP_Get_User_AchievementData", sqlParameters);
                while (resultDataReader.Read())
                {
                    AchievementData item = new AchievementData {
                        UserID = (int) resultDataReader["UserID"],
                        AchievementID = (int) resultDataReader["AchievementID"],
                        IsComplete = (bool) resultDataReader["IsComplete"],
                        DateComplete = (DateTime) resultDataReader["DateComplete"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public UsersPetinfo[] GetUserAdoptPetSingles(int UserID)
        {
            List<UsersPetinfo> list = new List<UsersPetinfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_Get_User_AdoptPetList", sqlParameters);
                while (resultDataReader.Read())
                {
                    list.Add(this.InitPet(resultDataReader));
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public SqlDataProvider.Data.ItemInfo[] GetUserBagByType(int UserID, int bagType)
        {
            List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter("@UserID", SqlDbType.Int, 4);
                sqlParameters[0].Value = UserID;
                sqlParameters[1] = new SqlParameter("@BagType", bagType);
                base.db.GetReader(ref resultDataReader, "SP_Users_BagByType", sqlParameters);
                while (resultDataReader.Read())
                {
                    list.Add(this.InitItem(resultDataReader));
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public List<SqlDataProvider.Data.ItemInfo> GetUserBeadEuqip(int UserID)
        {
            List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_Users_Bead_Equip", sqlParameters);
                while (resultDataReader.Read())
                {
                    list.Add(this.InitItem(resultDataReader));
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list;
        }

        public BufferInfo[] GetUserBuffer(int userID)
        {
            List<BufferInfo> list = new List<BufferInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = userID;
                base.db.GetReader(ref resultDataReader, "SP_User_Buff_All", sqlParameters);
                while (resultDataReader.Read())
                {
                    BufferInfo item = new BufferInfo {
                        BeginDate = (DateTime) resultDataReader["BeginDate"],
                        Data = (resultDataReader["Data"] == null) ? "" : resultDataReader["Data"].ToString(),
                        Type = (int) resultDataReader["Type"],
                        UserID = (int) resultDataReader["UserID"],
                        ValidDate = (int) resultDataReader["ValidDate"],
                        Value = (int) resultDataReader["Value"],
                        IsExist = (bool) resultDataReader["IsExist"],
                        ValidCount = (int) resultDataReader["ValidCount"],
                        TemplateID = (int) resultDataReader["TemplateID"],
                        IsDirty = false
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public UsersCardInfo GetUserCardByPlace(int Place)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@Place", SqlDbType.Int, 4) };
                sqlParameters[0].Value = Place;
                base.db.GetReader(ref resultDataReader, "SP_Get_UserCard_By_Place", sqlParameters);
                if (resultDataReader.Read())
                {
                    return this.InitCard(resultDataReader);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public List<UsersCardInfo> GetUserCardEuqip(int UserID)
        {
            List<UsersCardInfo> list = new List<UsersCardInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_Users_Items_Card_Equip", sqlParameters);
                while (resultDataReader.Read())
                {
                    list.Add(this.InitCard(resultDataReader));
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list;
        }

        public UsersCardInfo[] GetUserCardSingles(int UserID)
        {
            List<UsersCardInfo> list = new List<UsersCardInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_Get_UserCard_By_ID", sqlParameters);
                while (resultDataReader.Read())
                {
                    list.Add(this.InitCard(resultDataReader));
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public ConsortiaBufferInfo[] GetUserConsortiaBuffer(int ConsortiaID)
		{
			List<ConsortiaBufferInfo> list = new List<ConsortiaBufferInfo>();
			SqlDataReader ResultDataReader = null;
			try
			{
				SqlParameter[] SqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", SqlDbType.Int, 4)
				};
				SqlParameters[0].Value = ConsortiaID;
				this.db.GetReader(ref ResultDataReader, "SP_User_Consortia_Buff_All", SqlParameters);
				while (ResultDataReader.Read())
				{
					list.Add(new ConsortiaBufferInfo
					{
						ConsortiaID = (int)ResultDataReader["ConsortiaID"],
						BufferID = (int)ResultDataReader["BufferID"],
						IsOpen = (bool)ResultDataReader["IsOpen"],
						BeginDate = (DateTime)ResultDataReader["BeginDate"],
						ValidDate = (int)ResultDataReader["ValidDate"],
						Type = (int)ResultDataReader["Type"],
						Value = (int)ResultDataReader["Value"]
					});
				}
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init SP_User_Consortia_Buff_All", ex);
				}
			}
			finally
			{
				if (ResultDataReader != null && !ResultDataReader.IsClosed)
				{
					ResultDataReader.Close();
				}
			}
			return list.ToArray();
		}

        public ConsortiaBufferInfo GetUserConsortiaBufferSingle(int ID)
		{
			SqlDataReader ResultDataReader = null;
			try
			{
				SqlParameter[] SqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ID", SqlDbType.Int, 4)
				};
				SqlParameters[0].Value = ID;
				this.db.GetReader(ref ResultDataReader, "SP_User_Consortia_Buff_Single", SqlParameters);
				if (ResultDataReader.Read())
				{
					return new ConsortiaBufferInfo
					{
						ConsortiaID = (int)ResultDataReader["ConsortiaID"],
						BufferID = (int)ResultDataReader["BufferID"],
						IsOpen = (bool)ResultDataReader["IsOpen"],
						BeginDate = (DateTime)ResultDataReader["BeginDate"],
						ValidDate = (int)ResultDataReader["ValidDate"],
						Type = (int)ResultDataReader["Type"],
						Value = (int)ResultDataReader["Value"]
					};
				}
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init SP_User_Consortia_Buff_Single", ex);
				}
			}
			finally
			{
				if (ResultDataReader != null && !ResultDataReader.IsClosed)
				{
					ResultDataReader.Close();
				}
			}
			return null;
		}

        public List<SqlDataProvider.Data.ItemInfo> GetUserEuqip(int UserID)
        {
            List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_Users_Items_Equip", sqlParameters);
                while (resultDataReader.Read())
                {
                    list.Add(this.InitItem(resultDataReader));
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list;
        }

        public List<SqlDataProvider.Data.ItemInfo> GetUserEuqipByNick(string Nick)
        {
            List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@NickName", SqlDbType.NVarChar, 200) };
                sqlParameters[0].Value = Nick;
                base.db.GetReader(ref resultDataReader, "SP_Users_Items_Equip_By_Nick", sqlParameters);
                while (resultDataReader.Read())
                {
                    list.Add(this.InitItem(resultDataReader));
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list;
        }

        public EventRewardProcessInfo[] GetUserEventProcess(int userID)
        {
            SqlDataReader resultDataReader = null;
            List<EventRewardProcessInfo> list = new List<EventRewardProcessInfo>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = userID;
                base.db.GetReader(ref resultDataReader, "SP_Get_User_EventProcess", sqlParameters);
                while (resultDataReader.Read())
                {
                    EventRewardProcessInfo item = new EventRewardProcessInfo {
                        UserID = (int) resultDataReader["UserID"],
                        ActiveType = (int) resultDataReader["ActiveType"],
                        Conditions = (int) resultDataReader["Conditions"],
                        AwardGot = (int) resultDataReader["AwardGot"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public UserInfo GetUserInfo(int UserId)
        {
            SqlDataReader resultDataReader = null;
            UserInfo info = new UserInfo {
                UserID = UserId
            };
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", UserId) };
                base.db.GetReader(ref resultDataReader, "SP_Get_User_Info", sqlParameters);
                while (resultDataReader.Read())
                {
                    info.UserID = int.Parse(resultDataReader["UserID"].ToString());
                    info.UserEmail = (resultDataReader["UserEmail"] == null) ? "" : resultDataReader["UserEmail"].ToString();
                    info.UserPhone = (resultDataReader["UserPhone"] == null) ? "" : resultDataReader["UserPhone"].ToString();
                    info.UserOther1 = (resultDataReader["UserOther1"] == null) ? "" : resultDataReader["UserOther1"].ToString();
                    info.UserOther2 = (resultDataReader["UserOther2"] == null) ? "" : resultDataReader["UserOther2"].ToString();
                    info.UserOther3 = (resultDataReader["UserOther3"] == null) ? "" : resultDataReader["UserOther3"].ToString();
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return info;
        }

        public SqlDataProvider.Data.ItemInfo[] GetUserItem(int UserID)
        {
            List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_Users_Items_All", sqlParameters);
                while (resultDataReader.Read())
                {
                    list.Add(this.InitItem(resultDataReader));
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public SqlDataProvider.Data.ItemInfo GetUserItemSingle(int itemID)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = itemID;
                base.db.GetReader(ref resultDataReader, "SP_Users_Items_Single", sqlParameters);
                if (resultDataReader.Read())
                {
                    return this.InitItem(resultDataReader);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public LevelInfo GetUserLevelSingle(int Grade)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@Grade", Grade) };
                base.db.GetReader(ref resultDataReader, "SP_Get_Level_By_Grade", sqlParameters);
                if (resultDataReader.Read())
                {
                    return new LevelInfo { Grade = (int) resultDataReader["Grade"], GP = (int) resultDataReader["GP"], Blood = (int) resultDataReader["Blood"] };
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetLevelInfoSingle", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public PlayerLimitInfo GetUserLimitByUserName(string userName)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserName", SqlDbType.NVarChar, 200) };
                sqlParameters[0].Value = userName;
                base.db.GetReader(ref resultDataReader, "SP_Users_LimitByUserName", sqlParameters);
                if (resultDataReader.Read())
                {
                    return new PlayerLimitInfo { ID = (int) resultDataReader["UserID"], NickName = (string) resultDataReader["NickName"] };
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public PlayerInfo[] GetUserLoginList(string userName)
        {
            List<PlayerInfo> list = new List<PlayerInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserName", SqlDbType.NVarChar, 200) };
                sqlParameters[0].Value = userName;
                base.db.GetReader(ref resultDataReader, "SP_Users_LoginList", sqlParameters);
                while (resultDataReader.Read())
                {
                    list.Add(this.InitPlayerInfo(resultDataReader));
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public List<SqlDataProvider.Data.ItemInfo> GetUserMagicStoneEuqip(int UserID)
        {
            List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_Users_MagicStone_Equip", sqlParameters);
                while (resultDataReader.Read())
                {
                    list.Add(this.InitItem(resultDataReader));
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list;
        }

        public List<UsersPetinfo> GetUserPetIsExitSingles(int UserID)
        {
            List<UsersPetinfo> list = new List<UsersPetinfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_Get_UserPet_By_IsExit", sqlParameters);
                while (resultDataReader.Read())
                {
                    list.Add(this.InitPet(resultDataReader));
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list;
        }

        public UsersPetinfo GetUserPetSingle(int ID)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = ID;
                base.db.GetReader(ref resultDataReader, "SP_Get_UserPet_By_ID", sqlParameters);
                if (resultDataReader.Read())
                {
                    return this.InitPet(resultDataReader);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetPetInfoSingle", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public UsersPetinfo[] GetUserPetSingles(int UserID)
        {
            List<UsersPetinfo> list = new List<UsersPetinfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_Get_UserPet_By_ID", sqlParameters);
                while (resultDataReader.Read())
                {
                    list.Add(this.InitPet(resultDataReader));
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public QuestDataInfo[] GetUserQuest(int userID)
        {
            List<QuestDataInfo> list = new List<QuestDataInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = userID;
                base.db.GetReader(ref resultDataReader, "SP_QuestData_All", sqlParameters);
                while (resultDataReader.Read())
                {
                    QuestDataInfo item = new QuestDataInfo {
                        CompletedDate = (DateTime) resultDataReader["CompletedDate"],
                        IsComplete = (bool) resultDataReader["IsComplete"],
                        Condition1 = (int) resultDataReader["Condition1"],
                        Condition2 = (int) resultDataReader["Condition2"],
                        Condition3 = (int) resultDataReader["Condition3"],
                        Condition4 = (int) resultDataReader["Condition4"],
                        QuestID = (int) resultDataReader["QuestID"],
                        UserID = (int) resultDataReader["UserId"],
                        IsExist = (bool) resultDataReader["IsExist"],
                        RandDobule = (int) resultDataReader["RandDobule"],
                        RepeatFinish = (int) resultDataReader["RepeatFinish"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return list.ToArray();
        }

        public QuestDataInfo GetUserQuestSiger(int userID, int QuestID)
        {
            QuestDataInfo info = new QuestDataInfo();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int), new SqlParameter("@QuestID", SqlDbType.Int) };
                sqlParameters[0].Value = userID;
                sqlParameters[1].Value = QuestID;
                base.db.GetReader(ref resultDataReader, "SP_QuestData_One", sqlParameters);
                while (resultDataReader.Read())
                {
                    return new QuestDataInfo { CompletedDate = (DateTime) resultDataReader["CompletedDate"], IsComplete = (bool) resultDataReader["IsComplete"], Condition1 = (int) resultDataReader["Condition1"], Condition2 = (int) resultDataReader["Condition2"], Condition3 = (int) resultDataReader["Condition3"], Condition4 = (int) resultDataReader["Condition4"], QuestID = (int) resultDataReader["QuestID"], UserID = (int) resultDataReader["UserId"], IsExist = (bool) resultDataReader["IsExist"], RandDobule = (int) resultDataReader["RandDobule"], RepeatFinish = (int) resultDataReader["RepeatFinish"] };
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public PlayerInfo GetUserSingleByNickName(string nickName)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@NickName", SqlDbType.NVarChar, 200) };
                sqlParameters[0].Value = nickName;
                base.db.GetReader(ref resultDataReader, "SP_Users_SingleByNickName", sqlParameters);
                if (resultDataReader.Read())
                {
                    return this.InitPlayerInfo(resultDataReader);
                }
            }
            catch
            {
                throw new Exception();
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public PlayerInfo GetUserSingleByUserID(int UserID)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = UserID;
                base.db.GetReader(ref resultDataReader, "SP_Users_SingleByUserID", sqlParameters);
                if (resultDataReader.Read())
                {
                    return this.InitPlayerInfo(resultDataReader);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public PlayerInfo GetUserSingleByUserName(string userName)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserName", SqlDbType.NVarChar, 200) };
                sqlParameters[0].Value = userName;
                base.db.GetReader(ref resultDataReader, "SP_Users_SingleByUserName", sqlParameters);
                if (resultDataReader.Read())
                {
                    return this.InitPlayerInfo(resultDataReader);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public TexpInfo GetUserTexpInfoSingle(int ID)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", ID) };
                base.db.GetReader(ref resultDataReader, "SP_Get_UserTexp_By_ID", sqlParameters);
                if (resultDataReader.Read())
                {
                    return new TexpInfo { UserID = (int) resultDataReader["UserID"], attTexpExp = (int) resultDataReader["attTexpExp"], defTexpExp = (int) resultDataReader["defTexpExp"], hpTexpExp = (int) resultDataReader["hpTexpExp"], lukTexpExp = (int) resultDataReader["lukTexpExp"], spdTexpExp = (int) resultDataReader["spdTexpExp"], texpCount = (int) resultDataReader["texpCount"], texpTaskCount = (int) resultDataReader["texpTaskCount"], texpTaskDate = (DateTime) resultDataReader["texpTaskDate"] };
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetTexpInfoSingle", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public UserVIPInfo GetUserVIP(int userID)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = userID;
                base.db.GetReader(ref resultDataReader, "SP_Get_User_VIP", sqlParameters);
                while (resultDataReader.Read())
                {
                    return new UserVIPInfo { UserID = (int) resultDataReader["UserID"], typeVIP = Convert.ToByte(resultDataReader["typeVIP"]), VIPLevel = (int) resultDataReader["VIPLevel"], VIPExp = (int) resultDataReader["VIPExp"], VIPOnlineDays = (int) resultDataReader["VIPOnlineDays"], VIPOfflineDays = (int) resultDataReader["VIPOfflineDays"], VIPExpireDay = (DateTime) resultDataReader["VIPExpireDay"], LastVIPPackTime = (DateTime) resultDataReader["LastVIPPackTime"], VIPLastdate = (DateTime) resultDataReader["VIPLastdate"], VIPNextLevelDaysNeeded = (int) resultDataReader["VIPNextLevelDaysNeeded"], CanTakeVipReward = (bool) resultDataReader["CanTakeVipReward"] };
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public int GetVip(string UserName)
        {
            int num = 0;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserName", UserName), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[1].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_GetVip", sqlParameters);
                num = (int) sqlParameters[1].Value;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return num;
        }

        public AuctionInfo InitAuctionInfo(SqlDataReader reader)
        {
            return new AuctionInfo { AuctioneerID = (int) reader["AuctioneerID"], AuctioneerName = (reader["AuctioneerName"] == null) ? "" : reader["AuctioneerName"].ToString(), AuctionID = (int) reader["AuctionID"], BeginDate = (DateTime) reader["BeginDate"], BuyerID = (int) reader["BuyerID"], BuyerName = (reader["BuyerName"] == null) ? "" : reader["BuyerName"].ToString(), IsExist = (bool) reader["IsExist"], ItemID = (int) reader["ItemID"], Mouthful = (int) reader["Mouthful"], PayType = (int) reader["PayType"], Price = (int) reader["Price"], Rise = (int) reader["Rise"], ValidDate = (int) reader["ValidDate"], Name = reader["Name"].ToString(), Category = (int) reader["Category"], goodsCount = (int) reader["goodsCount"] };
        }

        public UsersCardInfo InitCard(SqlDataReader reader)
        {
            return new UsersCardInfo { UserID = (int) reader["UserID"], TemplateID = (int) reader["TemplateID"], CardID = (int) reader["CardID"], Count = (int) reader["Count"], Attack = (int) reader["Attack"], Agility = (int) reader["Agility"], Defence = (int) reader["Defence"], Luck = (int) reader["Luck"], Damage = (int) reader["Damage"], Guard = (int) reader["Guard"], Level = (int) reader["Level"], Place = (int) reader["Place"], isFirstGet = (bool) reader["isFirstGet"], CardGP = (int) reader["CardGP"] };
        }

        public CardGrooveUpdateInfo InitCardGrooveUpdate(SqlDataReader reader)
        {
            return new CardGrooveUpdateInfo { ID = (int) reader["ID"], Attack = (int) reader["Attack"], Defend = (int) reader["Defend"], Agility = (int) reader["Agility"], Lucky = (int) reader["Lucky"], Damage = (int) reader["Damage"], Guard = (int) reader["Guard"], Level = (int) reader["Level"], Type = (int) reader["Type"], Exp = (int) reader["Exp"] };
        }

        public CardTemplateInfo InitCardTemplate(SqlDataReader reader)
        {
            return new CardTemplateInfo { ID = (int) reader["ID"], CardID = (int) reader["CardID"], Count = (int) reader["Count"], probability = (int) reader["probability"], AttackRate = (int) reader["Attack"], AddAttack = (int) reader["AddAttack"], DefendRate = (int) reader["DefendRate"], AddDefend = (int) reader["AddDefend"], AgilityRate = (int) reader["AgilityRate"], AddAgility = (int) reader["AddAgility"], LuckyRate = (int) reader["LuckyRate"], AddLucky = (int) reader["AddLucky"], DamageRate = (int) reader["DamageRate"], AddDamage = (int) reader["AddDamage"], GuardRate = (int) reader["GuardRate"], AddGuard = (int) reader["AddGuard"] };
        }

        public ConsortiaUserInfo InitConsortiaUserInfo(SqlDataReader dr)
		{
			ConsortiaUserInfo consortiaUserInfo = new ConsortiaUserInfo();
			consortiaUserInfo.ID = (int)dr["ID"];
			consortiaUserInfo.ConsortiaID = (int)dr["ConsortiaID"];
			consortiaUserInfo.DutyID = (int)dr["DutyID"];
			consortiaUserInfo.DutyName = dr["DutyName"].ToString();
			consortiaUserInfo.IsExist = (bool)dr["IsExist"];
			consortiaUserInfo.RatifierID = (int)dr["RatifierID"];
			consortiaUserInfo.RatifierName = dr["RatifierName"].ToString();
			consortiaUserInfo.Remark = dr["Remark"].ToString();
			consortiaUserInfo.UserID = (int)dr["UserID"];
			consortiaUserInfo.UserName = dr["UserName"].ToString();
			consortiaUserInfo.Grade = (int)dr["Grade"];
			consortiaUserInfo.GP = (int)dr["GP"];
			consortiaUserInfo.Repute = (int)dr["Repute"];
			consortiaUserInfo.State = (int)dr["State"];
			consortiaUserInfo.Right = (int)dr["Right"];
			consortiaUserInfo.Offer = (int)dr["Offer"];
			consortiaUserInfo.Colors = dr["Colors"].ToString();
			consortiaUserInfo.Style = dr["Style"].ToString();
			consortiaUserInfo.Hide = (int)dr["Hide"];
			consortiaUserInfo.Skin = ((dr["Skin"] == null) ? "" : consortiaUserInfo.Skin);
			consortiaUserInfo.Level = (int)dr["Level"];
			consortiaUserInfo.LastDate = (DateTime)dr["LastDate"];
			consortiaUserInfo.Sex = (bool)dr["Sex"];
			consortiaUserInfo.IsBanChat = (bool)dr["IsBanChat"];
			consortiaUserInfo.Win = (int)dr["Win"];
			consortiaUserInfo.Total = (int)dr["Total"];
			consortiaUserInfo.Escape = (int)dr["Escape"];
			consortiaUserInfo.RichesOffer = (int)dr["RichesOffer"];
			consortiaUserInfo.RichesRob = (int)dr["RichesRob"];
			consortiaUserInfo.LoginName = ((dr["LoginName"] == null) ? "" : dr["LoginName"].ToString());
			consortiaUserInfo.Nimbus = (int)dr["Nimbus"];
			consortiaUserInfo.FightPower = (int)dr["FightPower"];
			consortiaUserInfo.typeVIP = Convert.ToByte(dr["typeVIP"]);
			consortiaUserInfo.VIPLevel = (int)dr["VIPLevel"];
			return consortiaUserInfo;
		}

        public SqlDataProvider.Data.ItemInfo InitItem(SqlDataReader reader)
        {
            SqlDataProvider.Data.ItemInfo info = new SqlDataProvider.Data.ItemInfo(ItemMgr.FindItemTemplate((int) reader["TemplateID"])) {
                AgilityCompose = (int) reader["AgilityCompose"],
                AttackCompose = (int) reader["AttackCompose"],
                Color = reader["Color"].ToString(),
                Count = (int) reader["Count"],
                DefendCompose = (int) reader["DefendCompose"],
                ItemID = (int) reader["ItemID"],
                LuckCompose = (int) reader["LuckCompose"],
                Place = (int) reader["Place"],
                StrengthenLevel = (int) reader["StrengthenLevel"],
                TemplateID = (int) reader["TemplateID"],
                UserID = (int) reader["UserID"],
                ValidDate = (int) reader["ValidDate"],
                IsDirty = false,
                IsExist = (bool) reader["IsExist"],
                IsBinds = (bool) reader["IsBinds"],
                IsUsed = (bool) reader["IsUsed"],
                BeginDate = (DateTime) reader["BeginDate"],
                IsJudge = (bool) reader["IsJudge"],
                BagType = (int) reader["BagType"],
                Skin = reader["Skin"].ToString(),
                RemoveDate = (DateTime) reader["RemoveDate"],
                RemoveType = (int) reader["RemoveType"],
                Hole1 = (int) reader["Hole1"],
                Hole2 = (int) reader["Hole2"],
                Hole3 = (int) reader["Hole3"],
                Hole4 = (int) reader["Hole4"],
                Hole5 = (int) reader["Hole5"],
                Hole6 = (int) reader["Hole6"],
                StrengthenTimes = (int) reader["StrengthenTimes"],
                StrengthenExp = (int) reader["StrengthenExp"],
                Hole5Level = (int) reader["Hole5Level"],
                Hole5Exp = (int) reader["Hole5Exp"],
                Hole6Level = (int) reader["Hole6Level"],
                Hole6Exp = (int) reader["Hole6Exp"],
                goldBeginTime = (DateTime) reader["goldBeginTime"],
                goldValidDate = (int) reader["goldValidDate"],
                beadExp = (int) reader["beadExp"],
                beadLevel = (int) reader["beadLevel"],
                beadIsLock = (bool) reader["beadIsLock"],
                isShowBind = (bool) reader["isShowBind"],
                latentEnergyCurStr = (string) reader["latentEnergyCurStr"],
                latentEnergyNewStr = (string) reader["latentEnergyNewStr"],
                latentEnergyEndTime = (DateTime) reader["latentEnergyEndTime"],
                MagicAttack = (int) reader["MagicAttack"],
                MagicDefence = (int) reader["MagicDefence"],
                GoodsLock = (bool) reader["goodsLock"],
                Damage = (int) reader["Damage"],
                Guard = (int) reader["Guard"],
                Blood = (int) reader["Blood"],
                Bless = (int) reader["Bless"],
                AdvanceDate = (DateTime) reader["AdvanceDate"],
                LianGrade = (int) reader["LianGrade"],
                LianExp = (int) reader["LianExp"]
            };
            if (info.IsGold)
            {
                GoldEquipTemplateLoadInfo info2 = GoldEquipMgr.FindGoldEquipNewTemplate(info.TemplateID);
                if (info2 != null)
                {
                    info.GoldEquip = ItemMgr.FindItemTemplate(info2.NewTemplateId);
                }
            }
            return info;
        }

        public MailInfo InitMail(SqlDataReader reader)
        {
            return new MailInfo { 
                Annex1 = reader["Annex1"].ToString(), Annex2 = reader["Annex2"].ToString(), Content = reader["Content"].ToString(), Gold = (int) reader["Gold"], ID = (int) reader["ID"], IsExist = (bool) reader["IsExist"], Money = (int) reader["Money"], GiftToken = (int) reader["GiftToken"], Receiver = reader["Receiver"].ToString(), ReceiverID = (int) reader["ReceiverID"], Sender = reader["Sender"].ToString(), SenderID = (int) reader["SenderID"], Title = reader["Title"].ToString(), Type = (int) reader["Type"], ValidDate = (int) reader["ValidDate"], IsRead = (bool) reader["IsRead"], 
                SendTime = (DateTime) reader["SendTime"], Annex1Name = (reader["Annex1Name"] == null) ? "" : reader["Annex1Name"].ToString(), Annex2Name = (reader["Annex2Name"] == null) ? "" : reader["Annex2Name"].ToString(), Annex3 = reader["Annex3"].ToString(), Annex4 = reader["Annex4"].ToString(), Annex5 = reader["Annex5"].ToString(), Annex3Name = (reader["Annex3Name"] == null) ? "" : reader["Annex3Name"].ToString(), Annex4Name = (reader["Annex4Name"] == null) ? "" : reader["Annex4Name"].ToString(), Annex5Name = (reader["Annex5Name"] == null) ? "" : reader["Annex5Name"].ToString(), AnnexRemark = (reader["AnnexRemark"] == null) ? "" : reader["AnnexRemark"].ToString()
             };
        }

        public UsersPetinfo InitPet(SqlDataReader reader)
        {
            return new UsersPetinfo { 
                ID = (int) reader["ID"], TemplateID = (int) reader["TemplateID"], Name = reader["Name"].ToString(), UserID = (int) reader["UserID"], Attack = (int) reader["Attack"], AttackGrow = (int) reader["AttackGrow"], Agility = (int) reader["Agility"], AgilityGrow = (int) reader["AgilityGrow"], Defence = (int) reader["Defence"], DefenceGrow = (int) reader["DefenceGrow"], Luck = (int) reader["Luck"], LuckGrow = (int) reader["LuckGrow"], Blood = (int) reader["Blood"], BloodGrow = (int) reader["BloodGrow"], Damage = (int) reader["Damage"], DamageGrow = (int) reader["DamageGrow"], 
                Guard = (int) reader["Guard"], GuardGrow = (int) reader["GuardGrow"], Level = (int) reader["Level"], GP = (int) reader["GP"], MaxGP = (int) reader["MaxGP"], Hunger = (int) reader["Hunger"], MP = (int) reader["MP"], Place = (int) reader["Place"], IsEquip = (bool) reader["IsEquip"], IsExit = (bool) reader["IsExit"], Skill = reader["Skill"].ToString(), SkillEquip = reader["SkillEquip"].ToString(), currentStarExp = (int) reader["currentStarExp"]
             };
        }

        public PlayerInfo InitPlayerInfo(SqlDataReader reader)
        {
            PlayerInfo info = new PlayerInfo {
                Password = (string) reader["Password"],
                IsConsortia = (bool) reader["IsConsortia"],
                Agility = (int) reader["Agility"],
                Attack = (int) reader["Attack"],
                hp = (int) reader["hp"],
                Colors = (reader["Colors"] == null) ? "" : reader["Colors"].ToString(),
                ConsortiaID = (int) reader["ConsortiaID"],
                Defence = (int) reader["Defence"],
                MagicAttack = (int) reader["MagicAttack"],
                MagicDefence = (int) reader["MagicDefence"],
                evolutionExp = (int) reader["evolutionExp"],
                evolutionGrade = (int) reader["evolutionGrade"],
                Gold = (int) reader["Gold"],
                GP = (int) reader["GP"],
                Grade = (int) reader["Grade"],
                ID = (int) reader["UserID"],
                Luck = (int) reader["Luck"],
                Money = (int) reader["Money"],
                NickName = (((string) reader["NickName"]) == null) ? "" : ((string) reader["NickName"]),
                Sex = (bool) reader["Sex"],
                State = (int) reader["State"],
                Style = (reader["Style"] == null) ? "" : reader["Style"].ToString(),
                Hide = (int) reader["Hide"],
                Repute = (int) reader["Repute"],
                UserName = (reader["UserName"] == null) ? "" : reader["UserName"].ToString(),
                ConsortiaName = (reader["ConsortiaName"] == null) ? "" : reader["ConsortiaName"].ToString(),
                Offer = (int) reader["Offer"],
                Win = (int) reader["Win"],
                Total = (int) reader["Total"],
                Escape = (int) reader["Escape"],
                Skin = (reader["Skin"] == null) ? "" : reader["Skin"].ToString(),
                IsBanChat = (bool) reader["IsBanChat"],
                ReputeOffer = (int) reader["ReputeOffer"],
                ConsortiaRepute = (int) reader["ConsortiaRepute"],
                ConsortiaLevel = (int) reader["ConsortiaLevel"],
                StoreLevel = (int) reader["StoreLevel"],
                ShopLevel = (int) reader["ShopLevel"],
                SmithLevel = (int) reader["SmithLevel"],
                ConsortiaHonor = (int) reader["ConsortiaHonor"],
                RichesOffer = (int) reader["RichesOffer"],
                RichesRob = (int) reader["RichesRob"],
                AntiAddiction = (int) reader["AntiAddiction"],
                DutyLevel = (int) reader["DutyLevel"],
                DutyName = (reader["DutyName"] == null) ? "" : reader["DutyName"].ToString(),
                Right = (int) reader["Right"],
                ChairmanName = (reader["ChairmanName"] == null) ? "" : reader["ChairmanName"].ToString(),
                AddDayGP = (int) reader["AddDayGP"],
                AddDayOffer = (int) reader["AddDayOffer"],
                AddWeekGP = (int) reader["AddWeekGP"],
                AddWeekOffer = (int) reader["AddWeekOffer"],
                ConsortiaRiches = (int) reader["ConsortiaRiches"],
                CheckCount = (int) reader["CheckCount"],
                IsMarried = (bool) reader["IsMarried"],
                SpouseID = (int) reader["SpouseID"],
                SpouseName = (reader["SpouseName"] == null) ? "" : reader["SpouseName"].ToString(),
                MarryInfoID = (int) reader["MarryInfoID"],
                IsCreatedMarryRoom = (bool) reader["IsCreatedMarryRoom"],
                DayLoginCount = (int) reader["DayLoginCount"],
                PasswordTwo = (reader["PasswordTwo"] == null) ? "" : reader["PasswordTwo"].ToString(),
                SelfMarryRoomID = (int) reader["SelfMarryRoomID"],
                IsGotRing = (bool) reader["IsGotRing"],
                Rename = (bool) reader["Rename"],
                ConsortiaRename = (bool) reader["ConsortiaRename"],
                IsDirty = false,
                IsFirst = (int) reader["IsFirst"],
                Nimbus = (int) reader["Nimbus"],
                LastAward = (DateTime) reader["LastAward"],
                GiftToken = (int) reader["GiftToken"],
                QuestSite = (reader["QuestSite"] == null) ? new byte[200] : ((byte[]) reader["QuestSite"]),
                PvePermission = (reader["PvePermission"] == null) ? "" : reader["PvePermission"].ToString(),
                FightPower = (int) reader["FightPower"],
                PasswordQuest1 = (reader["PasswordQuestion1"] == null) ? "" : reader["PasswordQuestion1"].ToString(),
                PasswordQuest2 = (reader["PasswordQuestion2"] == null) ? "" : reader["PasswordQuestion2"].ToString()
            };
            PlayerInfo info2 = info;
            DateTime time = (DateTime) reader["LastFindDate"];
            info2.FailedPasswordAttemptCount = (int) reader["FailedPasswordAttemptCount"];
            info.AnswerSite = (int) reader["AnswerSite"];
            info.medal = (int) reader["Medal"];
            info.ChatCount = (int) reader["ChatCount"];
            info.SpaPubGoldRoomLimit = (int) reader["SpaPubGoldRoomLimit"];
            info.LastSpaDate = (DateTime) reader["LastSpaDate"];
            info.FightLabPermission = (string) reader["FightLabPermission"];
            info.SpaPubMoneyRoomLimit = (int) reader["SpaPubMoneyRoomLimit"];
            info.IsInSpaPubGoldToday = (bool) reader["IsInSpaPubGoldToday"];
            info.IsInSpaPubMoneyToday = (bool) reader["IsInSpaPubMoneyToday"];
            info.AchievementPoint = (int) reader["AchievementPoint"];
            info.LastWeekly = (DateTime) reader["LastWeekly"];
            info.LastWeeklyVersion = (int) reader["LastWeeklyVersion"];
            info.GiftGp = (int) reader["GiftGp"];
            info.GiftLevel = (int) reader["GiftLevel"];
            info.IsOpenGift = (bool) reader["IsOpenGift"];
            info.badgeID = (int) reader["badgeID"];
            info.typeVIP = Convert.ToByte(reader["typeVIP"]);
            info.VIPLevel = (int) reader["VIPLevel"];
            info.VIPExp = (int) reader["VIPExp"];
            info.VIPExpireDay = (DateTime) reader["VIPExpireDay"];
            info.LastVIPPackTime = (DateTime) reader["LastVIPPackTime"];
            info.CanTakeVipReward = (bool) reader["CanTakeVipReward"];
            info.WeaklessGuildProgressStr = (string) reader["WeaklessGuildProgressStr"];
            info.IsOldPlayer = (bool) reader["IsOldPlayer"];
            info.LastDate = (DateTime) reader["LastDate"];
            info.VIPLastDate = (DateTime) reader["VIPLastDate"];
            info.Score = (int) reader["Score"];
            info.OptionOnOff = (int) reader["OptionOnOff"];
            info.isOldPlayerHasValidEquitAtLogin = (bool) reader["isOldPlayerHasValidEquitAtLogin"];
            info.badLuckNumber = (int) reader["badLuckNumber"];
            info.luckyNum = (int) reader["luckyNum"];
            info.lastLuckyNumDate = (DateTime) reader["lastLuckyNumDate"];
            info.lastLuckNum = (int) reader["lastLuckNum"];
            info.CardSoul = (int) reader["CardSoul"];
            info.uesedFinishTime = (int) reader["uesedFinishTime"];
            info.totemId = (int) reader["totemId"];
            info.damageScores = (int) reader["damageScores"];
            info.petScore = (int) reader["petScore"];
            info.IsShowConsortia = (bool) reader["IsShowConsortia"];
            info.LastRefreshPet = (DateTime) reader["LastRefreshPet"];
            info.GetSoulCount = (int) reader["GetSoulCount"];
            info.isFirstDivorce = (int) reader["isFirstDivorce"];
            info.myScore = (int) reader["myScore"];
            info.LastGetEgg = (DateTime) reader["LastGetEgg"];
            info.TimeBox = (DateTime) reader["TimeBox"];
            info.IsFistGetPet = (bool) reader["IsFistGetPet"];
            info.myHonor = (int) reader["myHonor"];
            info.hardCurrency = (int) reader["hardCurrency"];
            info.MaxBuyHonor = (int) reader["MaxBuyHonor"];
            info.LeagueMoney = (int) reader["LeagueMoney"];
            info.Honor = (string) reader["Honor"];
            info.necklaceExp = (int) reader["necklaceExp"];
            info.necklaceExpAdd = (int) reader["necklaceExpAdd"];
            info.CurrentDressModel = (int) reader["CurrentDressModel"];
            info.MagicStonePoint = (int) reader["MagicStonePoint"];
            info.GoXu = (int) reader["GoXu"];
            info.DDPlayPoint = (int) reader["DDPlayPoint"];
            info.AchievementProcess = (reader["AchievementProcess"] == null) ? "" : reader["AchievementProcess"].ToString();
            return info;
        }

        public bool InsertMarryRoomInfo(MarryRoomInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[20];
                sqlParameters[0] = new SqlParameter("@ID", info.ID);
                sqlParameters[0].Direction = ParameterDirection.InputOutput;
                sqlParameters[1] = new SqlParameter("@Name", info.Name);
                sqlParameters[2] = new SqlParameter("@PlayerID", info.PlayerID);
                sqlParameters[3] = new SqlParameter("@PlayerName", info.PlayerName);
                sqlParameters[4] = new SqlParameter("@GroomID", info.GroomID);
                sqlParameters[5] = new SqlParameter("@GroomName", info.GroomName);
                sqlParameters[6] = new SqlParameter("@BrideID", info.BrideID);
                sqlParameters[7] = new SqlParameter("@BrideName", info.BrideName);
                sqlParameters[8] = new SqlParameter("@Pwd", info.Pwd);
                sqlParameters[9] = new SqlParameter("@AvailTime", info.AvailTime);
                sqlParameters[10] = new SqlParameter("@MaxCount", info.MaxCount);
                sqlParameters[11] = new SqlParameter("@GuestInvite", info.GuestInvite);
                sqlParameters[12] = new SqlParameter("@MapIndex", info.MapIndex);
                sqlParameters[13] = new SqlParameter("@BeginTime", info.BeginTime);
                sqlParameters[14] = new SqlParameter("@BreakTime", info.BreakTime);
                sqlParameters[15] = new SqlParameter("@RoomIntroduction", info.RoomIntroduction);
                sqlParameters[0x10] = new SqlParameter("@ServerID", info.ServerID);
                sqlParameters[0x11] = new SqlParameter("@IsHymeneal", info.IsHymeneal);
                sqlParameters[0x12] = new SqlParameter("@IsGunsaluteUsed", info.IsGunsaluteUsed);
                sqlParameters[0x13] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[0x13].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_Insert_Marry_Room_Info", sqlParameters);
                flag = ((int) sqlParameters[0x13].Value) == 0;
                if (flag)
                {
                    info.ID = (int) sqlParameters[0].Value;
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("InsertMarryRoomInfo", exception);
                }
            }
            return flag;
        }

        public bool InsertPlayerMarryApply(MarryApplyInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", info.UserID), new SqlParameter("@ApplyUserID", info.ApplyUserID), new SqlParameter("@ApplyUserName", info.ApplyUserName), new SqlParameter("@ApplyType", info.ApplyType), new SqlParameter("@ApplyResult", info.ApplyResult), new SqlParameter("@LoveProclamation", info.LoveProclamation), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[6].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_Insert_Marry_Apply", sqlParameters);
                flag = ((int) sqlParameters[6].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("InsertPlayerMarryApply", exception);
                }
            }
            return flag;
        }

        public bool InsertUserTexpInfo(TexpInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", info.UserID), new SqlParameter("@attTexpExp", info.attTexpExp), new SqlParameter("@defTexpExp", info.defTexpExp), new SqlParameter("@hpTexpExp", info.hpTexpExp), new SqlParameter("@lukTexpExp", info.lukTexpExp), new SqlParameter("@spdTexpExp", info.spdTexpExp), new SqlParameter("@texpCount", info.texpCount), new SqlParameter("@texpTaskCount", info.texpTaskCount), new SqlParameter("@texpTaskDate", info.texpTaskDate.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[9].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UserTexp_Add", sqlParameters);
                flag = ((int) sqlParameters[9].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("InsertTexpInfo", exception);
                }
            }
            return flag;
        }

        public PlayerInfo LoginGame(string username, string password)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserName", username), new SqlParameter("@Password", password) };
                base.db.GetReader(ref resultDataReader, "SP_Users_Login", sqlParameters);
                if (resultDataReader.Read())
                {
                    return this.InitPlayerInfo(resultDataReader);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public PlayerInfo LoginGame(string username, ref int isFirst, ref bool isExist, ref bool isError, bool firstValidate, ref DateTime forbidDate, string nickname)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserName", username), new SqlParameter("@Password", ""), new SqlParameter("@FirstValidate", firstValidate), new SqlParameter("@Nickname", nickname) };
                base.db.GetReader(ref resultDataReader, "SP_Users_LoginWeb", sqlParameters);
                if (resultDataReader.Read())
                {
                    isFirst = (int) resultDataReader["IsFirst"];
                    isExist = (bool) resultDataReader["IsExist"];
                    forbidDate = (DateTime) resultDataReader["ForbidDate"];
                    if (isFirst > 1)
                    {
                        isFirst--;
                    }
                    return this.InitPlayerInfo(resultDataReader);
                }
            }
            catch (Exception exception)
            {
                isError = true;
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public PlayerInfo LoginGame(string username, ref int isFirst, ref bool isExist, ref bool isError, bool firstValidate, ref DateTime forbidDate, ref string nickname, string ActiveIP)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserName", username), new SqlParameter("@Password", ""), new SqlParameter("@FirstValidate", firstValidate), new SqlParameter("@Nickname", nickname), new SqlParameter("@ActiveIP", ActiveIP) };
                base.db.GetReader(ref resultDataReader, "SP_Users_LoginWeb", sqlParameters);
                while (resultDataReader.Read())
                {
                    isFirst = (int) resultDataReader["IsFirst"];
                    isExist = (bool) resultDataReader["IsExist"];
                    forbidDate = (DateTime) resultDataReader["ForbidDate"];
                    nickname = (string) resultDataReader["NickName"];
                    if (isFirst > 1)
                    {
                        isFirst--;
                    }
                    return this.InitPlayerInfo(resultDataReader);
                }
            }
            catch (Exception exception)
            {
                isError = true;
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public bool RegisterPlayer(string userName, string passWord, string nickName, string bStyle, string gStyle, string armColor, string hairColor, string faceColor, string clothColor, string hatColor, int sex, ref string msg, int validDate)
        {
            bool flag = false;
            try
            {
                string[] strArray = bStyle.Split(new char[] { ',' });
                string[] strArray2 = gStyle.Split(new char[] { ',' });
                SqlParameter[] sqlParameters = new SqlParameter[] { 
                    new SqlParameter("@UserName", userName), new SqlParameter("@PassWord", passWord), new SqlParameter("@NickName", nickName), new SqlParameter("@BArmID", int.Parse(strArray[0])), new SqlParameter("@BHairID", int.Parse(strArray[1])), new SqlParameter("@BFaceID", int.Parse(strArray[2])), new SqlParameter("@BClothID", int.Parse(strArray[3])), new SqlParameter("@BHatID", int.Parse(strArray[4])), new SqlParameter("@GArmID", int.Parse(strArray2[0])), new SqlParameter("@GHairID", int.Parse(strArray2[1])), new SqlParameter("@GFaceID", int.Parse(strArray2[2])), new SqlParameter("@GClothID", int.Parse(strArray2[3])), new SqlParameter("@GHatID", int.Parse(strArray2[4])), new SqlParameter("@ArmColor", armColor), new SqlParameter("@HairColor", hairColor), new SqlParameter("@FaceColor", faceColor), 
                    new SqlParameter("@ClothColor", clothColor), new SqlParameter("@HatColor", clothColor), new SqlParameter("@Sex", sex), new SqlParameter("@StyleDate", validDate), new SqlParameter("@Result", SqlDbType.Int)
                 };
                sqlParameters[20].Direction = ParameterDirection.ReturnValue;
                flag = base.db.RunProcedure("SP_Users_RegisterNotValidate", sqlParameters);
                int num = (int) sqlParameters[20].Value;
                flag = num == 0;
                switch (num)
                {
                    case 2:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.RegisterPlayer.Msg2", new object[0]);
                        return flag;

                    case 3:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.RegisterPlayer.Msg3", new object[0]);
                        return flag;
                }
                return flag;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool RegisterUser(string UserName, string NickName, string Password, bool Sex, int Money, int GiftToken, int Gold)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserName", UserName), new SqlParameter("@Password", Password), new SqlParameter("@NickName", NickName), new SqlParameter("@Sex", Sex), new SqlParameter("@Money", Money), new SqlParameter("@GiftToken", GiftToken), new SqlParameter("@Gold", Gold), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[7].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_Account_Register", sqlParameters);
                if (((int) sqlParameters[7].Value) == 0)
                {
                    flag = true;
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init Register", exception);
                }
            }
            return flag;
        }

        public bool RegisterUserInfo(UserInfo userinfo)
        {
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", userinfo.UserID), new SqlParameter("@UserEmail", userinfo.UserEmail), new SqlParameter("@UserPhone", (userinfo.UserPhone == null) ? string.Empty : userinfo.UserPhone), new SqlParameter("@UserOther1", (userinfo.UserOther1 == null) ? string.Empty : userinfo.UserOther1), new SqlParameter("@UserOther2", (userinfo.UserOther2 == null) ? string.Empty : userinfo.UserOther2), new SqlParameter("@UserOther3", (userinfo.UserOther3 == null) ? string.Empty : userinfo.UserOther3) };
                return base.db.RunProcedure("SP_User_Info_Add", sqlParameters);
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return false;
        }

        public PlayerInfo ReLoadPlayer(int ID)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", ID) };
                base.db.GetReader(ref resultDataReader, "SP_Users_Reload", sqlParameters);
                if (resultDataReader.Read())
                {
                    return this.InitPlayerInfo(resultDataReader);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }

        public bool RemoveIsArrange(int ID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", ID), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[1].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_RemoveIsArrange", sqlParameters);
                flag = ((int) sqlParameters[1].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_RemoveIsArrange", exception);
                }
            }
            return flag;
        }

        public bool RemoveTreasureDataByUser(int ID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", ID), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[1].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_RemoveTreasureDataByUser", sqlParameters);
                flag = ((int) sqlParameters[1].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_RemoveTreasureDataByUser", exception);
                }
            }
            return flag;
        }

        public bool RemoveUserAdoptPet(int ID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[1].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_Remove_User_AdoptPet", sqlParameters);
                int num = (int) sqlParameters[1].Value;
                flag = num == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool RemoveUserPet(UsersPetinfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { 
                    new SqlParameter("@TemplateID", info.TemplateID), new SqlParameter("@Name", (info.Name == null) ? "Error!" : info.Name), new SqlParameter("@UserID", info.UserID), new SqlParameter("@Attack", info.Attack), new SqlParameter("@Defence", info.Defence), new SqlParameter("@Luck", info.Luck), new SqlParameter("@Agility", info.Agility), new SqlParameter("@Blood", info.Blood), new SqlParameter("@Damage", info.Damage), new SqlParameter("@Guard", info.Guard), new SqlParameter("@AttackGrow", info.AttackGrow), new SqlParameter("@DefenceGrow", info.DefenceGrow), new SqlParameter("@LuckGrow", info.LuckGrow), new SqlParameter("@AgilityGrow", info.AgilityGrow), new SqlParameter("@BloodGrow", info.BloodGrow), new SqlParameter("@DamageGrow", info.DamageGrow), 
                    new SqlParameter("@GuardGrow", info.GuardGrow), new SqlParameter("@Level", info.Level), new SqlParameter("@GP", info.GP), new SqlParameter("@MaxGP", info.MaxGP), new SqlParameter("@Hunger", info.Hunger), new SqlParameter("@PetHappyStar", info.PetHappyStar), new SqlParameter("@MP", info.MP), new SqlParameter("@IsEquip", info.IsEquip), new SqlParameter("@Place", info.Place), new SqlParameter("@IsExit", info.IsExit), new SqlParameter("@ID", info.ID), new SqlParameter("@currentStarExp", info.currentStarExp), new SqlParameter("@Result", SqlDbType.Int)
                 };
                sqlParameters[0x1c].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UserPet_Remove", sqlParameters);
                int num = (int) sqlParameters[0x1c].Value;
                flag = num == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool RenameNick(string userName, string nickName, string newNickName)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserName", userName), new SqlParameter("@NickName", nickName), new SqlParameter("@NewNickName", newNickName), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[3].Direction = ParameterDirection.ReturnValue;
                flag = base.db.RunProcedure("SP_Users_RenameNick2", sqlParameters);
                int num = (int) sqlParameters[3].Value;
                flag = num == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("RenameNick", exception);
                }
            }
            return flag;
        }

        public bool RenameNick(string userName, string nickName, string newNickName, ref string msg)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserName", userName), new SqlParameter("@NickName", nickName), new SqlParameter("@NewNickName", newNickName), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[3].Direction = ParameterDirection.ReturnValue;
                flag = base.db.RunProcedure("SP_Users_RenameNick", sqlParameters);
                int num = (int) sqlParameters[3].Value;
                flag = num == 0;
                switch (num)
                {
                    case 4:
                    case 5:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.RenameNick.Msg4", new object[0]);
                        return flag;
                }
                return flag;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("RenameNick", exception);
                }
            }
            return flag;
        }

        public bool ResetCommunalActive(int ActiveID, bool IsReset)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ActiveID", ActiveID), new SqlParameter("@IsReset", IsReset), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[2].Direction = ParameterDirection.ReturnValue;
                flag = base.db.RunProcedure("SP_Reset_CommunalActive", sqlParameters);
                flag = ((int) sqlParameters[2].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init CommunalActive", exception);
                }
            }
            return flag;
        }

        public bool ResetDragonBoat()
        {
            bool flag = false;
            try
            {
                flag = base.db.RunProcedure("SP_Reset_DragonBoat_Data");
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init ResetDragonBoat", exception);
                }
            }
            return flag;
        }

        public bool ResetLuckStarRank()
        {
            bool flag = false;
            try
            {
                flag = base.db.RunProcedure("SP_Reset_LuckStar_Rank_Info");
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init ResetLuckStar", exception);
                }
            }
            return flag;
        }

        public bool SaveBuffer(BufferInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", info.UserID), new SqlParameter("@Type", info.Type), new SqlParameter("@BeginDate", info.BeginDate.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@Data", (info.Data == null) ? "" : info.Data), new SqlParameter("@IsExist", info.IsExist), new SqlParameter("@ValidDate", info.ValidDate), new SqlParameter("@ValidCount", info.ValidCount), new SqlParameter("@Value", info.Value), new SqlParameter("@TemplateID", info.TemplateID) };
                flag = base.db.RunProcedure("SP_User_Buff_Add", sqlParameters);
                info.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool SaveConsortiaBuffer(ConsortiaBufferInfo info)
		{
			bool flag = false;
			try
			{
				flag = this.db.RunProcedure("SP_User_Consortia_Buff_Add", new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", info.ConsortiaID),
					new SqlParameter("@BufferID", info.BufferID),
					new SqlParameter("@IsOpen", info.IsOpen ? 1 : 0),
					new SqlParameter("@BeginDate", info.BeginDate),
					new SqlParameter("@ValidDate", info.ValidDate),
					new SqlParameter("@Type ", info.Type),
					new SqlParameter("@Value", info.Value)
				});
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", ex);
				}
			}
			return flag;
		}

        public bool SaveLuckStarRankInfo(LuckStarRewardRecordInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", info.PlayerID), new SqlParameter("@useStarNum", info.useStarNum), new SqlParameter("@nickName", info.nickName), new SqlParameter("@isVip", info.isVip) };
                flag = base.db.RunProcedure("SP_LuckStar_Rank_Info_Add", sqlParameters);
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool SavePlayerMarryNotice(MarryApplyInfo info, int answerId, ref int id)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[9];
                sqlParameters[0] = new SqlParameter("@UserID", info.UserID);
                sqlParameters[1] = new SqlParameter("@ApplyUserID", info.ApplyUserID);
                sqlParameters[2] = new SqlParameter("@ApplyUserName", info.ApplyUserName);
                sqlParameters[3] = new SqlParameter("@ApplyType", info.ApplyType);
                sqlParameters[4] = new SqlParameter("@ApplyResult", info.ApplyResult);
                sqlParameters[5] = new SqlParameter("@LoveProclamation", info.LoveProclamation);
                sqlParameters[6] = new SqlParameter("@AnswerId", answerId);
                sqlParameters[7] = new SqlParameter("@ouototal", SqlDbType.Int);
                sqlParameters[7].Direction = ParameterDirection.Output;
                sqlParameters[8] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[8].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_Insert_Marry_Notice", sqlParameters);
                id = (int) sqlParameters[7].Value;
                flag = ((int) sqlParameters[8].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SavePlayerMarryNotice", exception);
                }
            }
            return flag;
        }

        public bool ScanAuction(ref string noticeUserID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@NoticeUserID", SqlDbType.NVarChar, 0xfa0) };
                sqlParameters[0].Direction = ParameterDirection.Output;
                base.db.RunProcedure("SP_Auction_Scan", sqlParameters);
                noticeUserID = sqlParameters[0].Value.ToString();
                flag = true;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool ScanMail(ref string noticeUserID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@NoticeUserID", SqlDbType.NVarChar, 0xfa0) };
                sqlParameters[0].Direction = ParameterDirection.Output;
                base.db.RunProcedure("SP_Mail_Scan", sqlParameters);
                noticeUserID = sqlParameters[0].Value.ToString();
                flag = true;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool SendMail(MailInfo mail)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[0x1d];
                sqlParameters[0] = new SqlParameter("@ID", mail.ID);
                sqlParameters[0].Direction = ParameterDirection.Output;
                sqlParameters[1] = new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1);
                sqlParameters[2] = new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2);
                sqlParameters[3] = new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content);
                sqlParameters[4] = new SqlParameter("@Gold", mail.Gold);
                sqlParameters[5] = new SqlParameter("@IsExist", true);
                sqlParameters[6] = new SqlParameter("@Money", mail.Money);
                sqlParameters[7] = new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver);
                sqlParameters[8] = new SqlParameter("@ReceiverID", mail.ReceiverID);
                sqlParameters[9] = new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender);
                sqlParameters[10] = new SqlParameter("@SenderID", mail.SenderID);
                sqlParameters[11] = new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title);
                sqlParameters[12] = new SqlParameter("@IfDelS", false);
                sqlParameters[13] = new SqlParameter("@IsDelete", false);
                sqlParameters[14] = new SqlParameter("@IsDelR", false);
                sqlParameters[15] = new SqlParameter("@IsRead", false);
                sqlParameters[0x10] = new SqlParameter("@SendTime", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[0x11] = new SqlParameter("@Type", mail.Type);
                sqlParameters[0x12] = new SqlParameter("@Annex1Name", (mail.Annex1Name == null) ? "" : mail.Annex1Name);
                sqlParameters[0x13] = new SqlParameter("@Annex2Name", (mail.Annex2Name == null) ? "" : mail.Annex2Name);
                sqlParameters[20] = new SqlParameter("@Annex3", (mail.Annex3 == null) ? "" : mail.Annex3);
                sqlParameters[0x15] = new SqlParameter("@Annex4", (mail.Annex4 == null) ? "" : mail.Annex4);
                sqlParameters[0x16] = new SqlParameter("@Annex5", (mail.Annex5 == null) ? "" : mail.Annex5);
                sqlParameters[0x17] = new SqlParameter("@Annex3Name", (mail.Annex3Name == null) ? "" : mail.Annex3Name);
                sqlParameters[0x18] = new SqlParameter("@Annex4Name", (mail.Annex4Name == null) ? "" : mail.Annex4Name);
                sqlParameters[0x19] = new SqlParameter("@Annex5Name", (mail.Annex5Name == null) ? "" : mail.Annex5Name);
                sqlParameters[0x1a] = new SqlParameter("@ValidDate", mail.ValidDate);
                sqlParameters[0x1b] = new SqlParameter("@AnnexRemark", (mail.AnnexRemark == null) ? "" : mail.AnnexRemark);
                sqlParameters[0x1c] = new SqlParameter("GiftToken", mail.GiftToken);
                flag = base.db.RunProcedure("SP_Mail_Send", sqlParameters);
                mail.ID = (int) sqlParameters[0].Value;
                using (CenterServiceClient client = new CenterServiceClient())
                {
                    client.MailNotice(mail.ReceiverID);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool SendMailAndItem(MailInfo mail, SqlDataProvider.Data.ItemInfo item, ref int returnValue)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[0x22];
                sqlParameters[0] = new SqlParameter("@ItemID", item.ItemID);
                sqlParameters[1] = new SqlParameter("@UserID", item.UserID);
                sqlParameters[2] = new SqlParameter("@TemplateID", item.TemplateID);
                sqlParameters[3] = new SqlParameter("@Place", item.Place);
                sqlParameters[4] = new SqlParameter("@AgilityCompose", item.AgilityCompose);
                sqlParameters[5] = new SqlParameter("@AttackCompose", item.AttackCompose);
                sqlParameters[6] = new SqlParameter("@BeginDate", item.BeginDate);
                sqlParameters[7] = new SqlParameter("@Color", (item.Color == null) ? "" : item.Color);
                sqlParameters[8] = new SqlParameter("@Count", item.Count);
                sqlParameters[9] = new SqlParameter("@DefendCompose", item.DefendCompose);
                sqlParameters[10] = new SqlParameter("@IsBinds", item.IsBinds);
                sqlParameters[11] = new SqlParameter("@IsExist", item.IsExist);
                sqlParameters[12] = new SqlParameter("@IsJudge", item.IsJudge);
                sqlParameters[13] = new SqlParameter("@LuckCompose", item.LuckCompose);
                sqlParameters[14] = new SqlParameter("@StrengthenLevel", item.StrengthenLevel);
                sqlParameters[15] = new SqlParameter("@ValidDate", item.ValidDate);
                sqlParameters[0x10] = new SqlParameter("@BagType", item.BagType);
                sqlParameters[0x11] = new SqlParameter("@ID", mail.ID);
                sqlParameters[0x11].Direction = ParameterDirection.Output;
                sqlParameters[0x12] = new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1);
                sqlParameters[0x13] = new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2);
                sqlParameters[20] = new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content);
                sqlParameters[0x15] = new SqlParameter("@Gold", mail.Gold);
                sqlParameters[0x16] = new SqlParameter("@Money", mail.Money);
                sqlParameters[0x17] = new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver);
                sqlParameters[0x18] = new SqlParameter("@ReceiverID", mail.ReceiverID);
                sqlParameters[0x19] = new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender);
                sqlParameters[0x1a] = new SqlParameter("@SenderID", mail.SenderID);
                sqlParameters[0x1b] = new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title);
                sqlParameters[0x1c] = new SqlParameter("@IfDelS", false);
                sqlParameters[0x1d] = new SqlParameter("@IsDelete", false);
                sqlParameters[30] = new SqlParameter("@IsDelR", false);
                sqlParameters[0x1f] = new SqlParameter("@IsRead", false);
                sqlParameters[0x20] = new SqlParameter("@SendTime", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[0x21] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[0x21].Direction = ParameterDirection.ReturnValue;
                flag = base.db.RunProcedure("SP_Admin_SendUserItem", sqlParameters);
                returnValue = (int) sqlParameters[0x21].Value;
                flag = returnValue == 0;
                if (!flag)
                {
                    return flag;
                }
                using (CenterServiceClient client = new CenterServiceClient())
                {
                    client.MailNotice(mail.ReceiverID);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public int SendMailAndItem(string title, string content, int userID, int gold, int money, string param)
        {
            int num = 1;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@Title", title), new SqlParameter("@Content", content), new SqlParameter("@UserID", userID), new SqlParameter("@Gold", gold), new SqlParameter("@Money", money), new SqlParameter("@GiftToken", SqlDbType.BigInt), new SqlParameter("@Param", param), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[7].Direction = ParameterDirection.ReturnValue;
                bool flag = base.db.RunProcedure("SP_Admin_SendAllItem", sqlParameters);
                num = (int) sqlParameters[7].Value;
                if (num != 0)
                {
                    return num;
                }
                using (CenterServiceClient client = new CenterServiceClient())
                {
                    client.MailNotice(userID);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return num;
        }

        public int SendMailAndItem(string title, string content, int UserID, int templateID, int count, int validDate, int gold, int money, int StrengthenLevel, int AttackCompose, int DefendCompose, int AgilityCompose, int LuckCompose, bool isBinds)
        {
            MailInfo mail = new MailInfo {
                Annex1 = "",
                Content = title,
                Gold = gold,
                Money = money,
                Receiver = "",
                ReceiverID = UserID,
                Sender = "Administrators",
                SenderID = 0,
                Title = content
            };
            SqlDataProvider.Data.ItemInfo item = new SqlDataProvider.Data.ItemInfo(null) {
                AgilityCompose = AgilityCompose,
                AttackCompose = AttackCompose,
                BeginDate = DateTime.Now,
                Color = "",
                DefendCompose = DefendCompose,
                IsDirty = false,
                IsExist = true,
                IsJudge = true,
                LuckCompose = LuckCompose,
                StrengthenLevel = StrengthenLevel,
                TemplateID = templateID,
                ValidDate = validDate,
                Count = count,
                IsBinds = isBinds
            };
            int returnValue = 1;
            this.SendMailAndItem(mail, item, ref returnValue);
            return returnValue;
        }

        public int SendMailAndItemByNickName(string title, string content, string nickName, int gold, int money, string param)
        {
            PlayerInfo userSingleByNickName = this.GetUserSingleByNickName(nickName);
            if (userSingleByNickName != null)
            {
                return this.SendMailAndItem(title, content, userSingleByNickName.ID, gold, money, param);
            }
            return 2;
        }

        public int SendMailAndItemByNickName(string title, string content, string NickName, int templateID, int count, int validDate, int gold, int money, int StrengthenLevel, int AttackCompose, int DefendCompose, int AgilityCompose, int LuckCompose, bool isBinds)
        {
            PlayerInfo userSingleByNickName = this.GetUserSingleByNickName(NickName);
            if (userSingleByNickName != null)
            {
                return this.SendMailAndItem(title, content, userSingleByNickName.ID, templateID, count, validDate, gold, money, StrengthenLevel, AttackCompose, DefendCompose, AgilityCompose, LuckCompose, isBinds);
            }
            return 2;
        }

        public int SendMailAndItemByUserName(string title, string content, string userName, int gold, int money, string param)
        {
            PlayerInfo userSingleByUserName = this.GetUserSingleByUserName(userName);
            if (userSingleByUserName != null)
            {
                return this.SendMailAndItem(title, content, userSingleByUserName.ID, gold, money, param);
            }
            return 2;
        }

        public int SendMailAndItemByUserName(string title, string content, string userName, int templateID, int count, int validDate, int gold, int money, int StrengthenLevel, int AttackCompose, int DefendCompose, int AgilityCompose, int LuckCompose, bool isBinds)
        {
            PlayerInfo userSingleByUserName = this.GetUserSingleByUserName(userName);
            if (userSingleByUserName != null)
            {
                return this.SendMailAndItem(title, content, userSingleByUserName.ID, templateID, count, validDate, gold, money, StrengthenLevel, AttackCompose, DefendCompose, AgilityCompose, LuckCompose, isBinds);
            }
            return 2;
        }

        public bool SendMailAndMoney(MailInfo mail, ref int returnValue)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[0x12];
                sqlParameters[0] = new SqlParameter("@ID", mail.ID);
                sqlParameters[0].Direction = ParameterDirection.Output;
                sqlParameters[1] = new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1);
                sqlParameters[2] = new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2);
                sqlParameters[3] = new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content);
                sqlParameters[4] = new SqlParameter("@Gold", mail.Gold);
                sqlParameters[5] = new SqlParameter("@IsExist", true);
                sqlParameters[6] = new SqlParameter("@Money", mail.Money);
                sqlParameters[7] = new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver);
                sqlParameters[8] = new SqlParameter("@ReceiverID", mail.ReceiverID);
                sqlParameters[9] = new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender);
                sqlParameters[10] = new SqlParameter("@SenderID", mail.SenderID);
                sqlParameters[11] = new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title);
                sqlParameters[12] = new SqlParameter("@IfDelS", false);
                sqlParameters[13] = new SqlParameter("@IsDelete", false);
                sqlParameters[14] = new SqlParameter("@IsDelR", false);
                sqlParameters[15] = new SqlParameter("@IsRead", false);
                sqlParameters[0x10] = new SqlParameter("@SendTime", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[0x11] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[0x11].Direction = ParameterDirection.ReturnValue;
                flag = base.db.RunProcedure("SP_Admin_SendUserMoney", sqlParameters);
                returnValue = (int) sqlParameters[0x11].Value;
                flag = returnValue == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool TankAll()
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[0];
                flag = base.db.RunProcedure("SP_Tank_All", sqlParameters);
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool Test(string DutyName)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@DutyName", DutyName) };
                flag = base.db.RunProcedure("SP_Test1", sqlParameters);
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool UpateStore(SqlDataProvider.Data.ItemInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[14];
                sqlParameters[0] = new SqlParameter("@ItemID", item.ItemID);
                sqlParameters[0].Direction = ParameterDirection.Output;
                sqlParameters[1] = new SqlParameter("@UserID", item.UserID);
                sqlParameters[2] = new SqlParameter("@TemplateID", item.Template.TemplateID);
                sqlParameters[3] = new SqlParameter("@Place", item.Place);
                sqlParameters[4] = new SqlParameter("@AgilityCompose", item.AgilityCompose);
                sqlParameters[5] = new SqlParameter("@AttackCompose", item.AttackCompose);
                sqlParameters[6] = new SqlParameter("@BeginDate", item.BeginDate.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[7] = new SqlParameter("@Color", (item.Color == null) ? "" : item.Color);
                sqlParameters[8] = new SqlParameter("@Count", item.Count);
                sqlParameters[9] = new SqlParameter("@DefendCompose", item.DefendCompose);
                sqlParameters[10] = new SqlParameter("@IsBinds", item.IsBinds);
                sqlParameters[11] = new SqlParameter("@IsExist", item.IsExist);
                sqlParameters[12] = new SqlParameter("@IsJudge", item.IsJudge);
                sqlParameters[13] = new SqlParameter("@LuckCompose", item.LuckCompose);
                flag = base.db.RunProcedure("SP_Users_Items_Add", sqlParameters);
                item.ItemID = (int) sqlParameters[0].Value;
                item.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool UpdateActiveSystem(ActiveSystemInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { 
                    new SqlParameter("@ID", info.ID), new SqlParameter("@UserID", info.UserID), new SqlParameter("@useableScore", info.useableScore), new SqlParameter("@totalScore", info.totalScore), new SqlParameter("@AvailTime", info.AvailTime), new SqlParameter("@NickName", info.NickName), new SqlParameter("@CanGetGift", info.CanGetGift), new SqlParameter("@canOpenCounts", info.canOpenCounts), new SqlParameter("@canEagleEyeCounts", info.canEagleEyeCounts), new SqlParameter("@lastFlushTime", info.lastFlushTime.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@isShowAll", info.isShowAll), new SqlParameter("@AvtiveMoney", info.ActiveMoney), new SqlParameter("@activityTanabataNum", info.activityTanabataNum), new SqlParameter("@ChallengeNum", info.ChallengeNum), new SqlParameter("@BuyBuffNum", info.BuyBuffNum), new SqlParameter("@lastEnterYearMonter", info.lastEnterYearMonter.ToString("MM/dd/yyyy hh:mm:ss")), 
                    new SqlParameter("@DamageNum", info.DamageNum), new SqlParameter("@BoxState", info.BoxState), new SqlParameter("@LuckystarCoins", info.LuckystarCoins), new SqlParameter("@Result", SqlDbType.Int)
                 };
                sqlParameters[0x13].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UpdateActiveSystem", sqlParameters);
                flag = ((int) sqlParameters[0x13].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_UpdateActiveSystem", exception);
                }
            }
            return flag;
        }

        public bool UpdateAuction(AuctionInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@AuctionID", info.AuctionID), new SqlParameter("@AuctioneerID", info.AuctioneerID), new SqlParameter("@AuctioneerName", (info.AuctioneerName == null) ? "" : info.AuctioneerName), new SqlParameter("@BeginDate", info.BeginDate), new SqlParameter("@BuyerID", info.BuyerID), new SqlParameter("@BuyerName", (info.BuyerName == null) ? "" : info.BuyerName), new SqlParameter("@IsExist", info.IsExist), new SqlParameter("@ItemID", info.ItemID), new SqlParameter("@Mouthful", info.Mouthful), new SqlParameter("@PayType", info.PayType), new SqlParameter("@Price", info.Price), new SqlParameter("@Rise", info.Rise), new SqlParameter("@ValidDate", info.ValidDate), new SqlParameter("Name", info.Name), new SqlParameter("Category", info.Category), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[15].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_Auction_Update", sqlParameters);
                int num = (int) sqlParameters[15].Value;
                flag = num == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool UpdateBoguAdventure(UserBoguAdventureInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", info.UserID), new SqlParameter("@CurrentPostion", info.CurrentPostion), new SqlParameter("@Map", info.Map), new SqlParameter("@Award", info.Award), new SqlParameter("@OpenCount", info.OpenCount), new SqlParameter("@ResetCount", info.ResetCount), new SqlParameter("@HP", info.HP), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[7].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UpdateBoguAdventure", sqlParameters);
                flag = ((int) sqlParameters[7].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_UpdateBoguAdventure", exception);
                }
            }
            return flag;
        }

        public bool UpdateBreakTimeWhereServerStop()
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[0].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_Update_Marry_Room_Info_Sever_Stop", sqlParameters);
                flag = ((int) sqlParameters[0].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("UpdateBreakTimeWhereServerStop", exception);
                }
            }
            return flag;
        }

        public bool UpdateBuyStore(int storeId)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@StoreID", storeId) };
                flag = base.db.RunProcedure("SP_Update_Buy_Store", sqlParameters);
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_Update_Buy_Store", exception);
                }
            }
            return flag;
        }

        public bool UpdateCards(UsersCardInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@Count", info.Count), new SqlParameter("@UserID", info.UserID), new SqlParameter("@Place", info.Place), new SqlParameter("@TemplateID", info.TemplateID), new SqlParameter("@isFirstGet", info.isFirstGet), new SqlParameter("@Attack", info.Attack), new SqlParameter("@Defence", info.Defence), new SqlParameter("@Luck", info.Luck), new SqlParameter("@Agility", info.Agility), new SqlParameter("@Damage", info.Damage), new SqlParameter("@Guard", info.Guard), new SqlParameter("@Level", info.Level), new SqlParameter("@CardGP", info.CardGP), new SqlParameter("@CardID", info.CardID), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[14].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UpdateUserCard", sqlParameters);
                int num = (int) sqlParameters[14].Value;
                flag = num == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public int Updatecash(string UserName, int cash)
        {
            int num = 3;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserName", UserName), new SqlParameter("@Cash", cash), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[2].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_Update_Cash", sqlParameters);
                num = (int) sqlParameters[2].Value;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return num;
        }

        public bool UpdateDbAchievementDataInfo(AchievementData info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", info.UserID), new SqlParameter("@AchievementID", info.AchievementID), new SqlParameter("@IsComplete", info.IsComplete), new SqlParameter("@DateComplete", info.DateComplete) };
                flag = base.db.RunProcedure("SP_Update_AchievementData", sqlParameters);
                info.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool UpdateDbQuestDataInfo(QuestDataInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", info.UserID), new SqlParameter("@QuestID", info.QuestID), new SqlParameter("@CompletedDate", info.CompletedDate), new SqlParameter("@IsComplete", info.IsComplete), new SqlParameter("@Condition1", info.Condition1), new SqlParameter("@Condition2", info.Condition2), new SqlParameter("@Condition3", info.Condition3), new SqlParameter("@Condition4", info.Condition4), new SqlParameter("@IsExist", info.IsExist), new SqlParameter("@RepeatFinish", info.RepeatFinish), new SqlParameter("@RandDobule", info.RandDobule) };
                flag = base.db.RunProcedure("SP_QuestData_Add", sqlParameters);
                info.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool UpdateDiceData(DiceDataInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", info.ID), new SqlParameter("@UserID", info.UserID), new SqlParameter("@LuckIntegral", info.LuckIntegral), new SqlParameter("@LuckIntegralLevel", info.LuckIntegralLevel), new SqlParameter("@Level", info.Level), new SqlParameter("@FreeCount", info.FreeCount), new SqlParameter("@CurrentPosition", info.CurrentPosition), new SqlParameter("@UserFirstCell", info.UserFirstCell), new SqlParameter("@AwardArray", info.AwardArray), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[9].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_Update_DiceData", sqlParameters);
                flag = ((int) sqlParameters[9].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_Update_DiceData", exception);
                }
            }
            return flag;
        }

        public bool UpdateFarm(UserFarmInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", info.ID), new SqlParameter("@FarmID", info.FarmID), new SqlParameter("@PayFieldMoney", info.PayFieldMoney), new SqlParameter("@PayAutoMoney", info.PayAutoMoney), new SqlParameter("@AutoPayTime", info.AutoPayTime.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@AutoValidDate", info.AutoValidDate), new SqlParameter("@VipLimitLevel", info.VipLimitLevel), new SqlParameter("@FarmerName", info.FarmerName), new SqlParameter("@GainFieldId", info.GainFieldId), new SqlParameter("@MatureId", info.MatureId), new SqlParameter("@KillCropId", info.KillCropId), new SqlParameter("@isAutoId", info.isAutoId), new SqlParameter("@isFarmHelper", info.isFarmHelper), new SqlParameter("@buyExpRemainNum", info.buyExpRemainNum), new SqlParameter("@isArrange", info.isArrange) };
                flag = base.db.RunProcedure("SP_Users_Farm_Update", sqlParameters);
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool UpdateFields(UserFieldInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { 
                    new SqlParameter("@ID", info.ID), new SqlParameter("@FarmID", info.FarmID), new SqlParameter("@FieldID", info.FieldID), new SqlParameter("@SeedID", info.SeedID), new SqlParameter("@PlantTime", info.PlantTime.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@AccelerateTime", info.AccelerateTime), new SqlParameter("@FieldValidDate", info.FieldValidDate), new SqlParameter("@PayTime", info.PayTime.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@GainCount", info.GainCount), new SqlParameter("@AutoSeedID", info.AutoSeedID), new SqlParameter("@AutoFertilizerID", info.AutoFertilizerID), new SqlParameter("@AutoSeedIDCount", info.AutoSeedIDCount), new SqlParameter("@AutoFertilizerCount", info.AutoFertilizerCount), new SqlParameter("@isAutomatic", info.isAutomatic), new SqlParameter("@AutomaticTime", info.AutomaticTime.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@IsExit", info.IsExit), 
                    new SqlParameter("@payFieldTime", info.payFieldTime)
                 };
                flag = base.db.RunProcedure("SP_Users_Fields_Update", sqlParameters);
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool UpdateFriendHelpTimes(int ID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", ID), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[1].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UpdateFriendHelpTimes", sqlParameters);
                flag = ((int) sqlParameters[1].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_UpdateFriendHelpTimes", exception);
                }
            }
            return flag;
        }

        public bool UpdateGemStoneInfo(UserGemStone g)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", g.ID), new SqlParameter("@UserID", g.UserID), new SqlParameter("@FigSpiritId", g.FigSpiritId), new SqlParameter("@FigSpiritIdValue", g.FigSpiritIdValue), new SqlParameter("@EquipPlace", g.EquipPlace), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[5].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UpdateGemStoneInfo", sqlParameters);
                flag = true;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_UpdateGemStoneInfo", exception);
                }
            }
            return flag;
        }

        public bool UpdateGoods(SqlDataProvider.Data.ItemInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { 
                    new SqlParameter("@ItemID", item.ItemID), new SqlParameter("@UserID", item.UserID), new SqlParameter("@TemplateID", item.Template.TemplateID), new SqlParameter("@Place", item.Place), new SqlParameter("@AgilityCompose", item.AgilityCompose), new SqlParameter("@AttackCompose", item.AttackCompose), new SqlParameter("@BeginDate", item.BeginDate), new SqlParameter("@Color", (item.Color == null) ? "" : item.Color), new SqlParameter("@Count", item.Count), new SqlParameter("@DefendCompose", item.DefendCompose), new SqlParameter("@IsBinds", item.IsBinds), new SqlParameter("@IsExist", item.IsExist), new SqlParameter("@IsJudge", item.IsJudge), new SqlParameter("@LuckCompose", item.LuckCompose), new SqlParameter("@StrengthenLevel", item.StrengthenLevel), new SqlParameter("@ValidDate", item.ValidDate), 
                    new SqlParameter("@BagType", item.BagType), new SqlParameter("@Skin", item.Skin), new SqlParameter("@IsUsed", item.IsUsed), new SqlParameter("@RemoveDate", item.RemoveDate), new SqlParameter("@RemoveType", item.RemoveType), new SqlParameter("@Hole1", item.Hole1), new SqlParameter("@Hole2", item.Hole2), new SqlParameter("@Hole3", item.Hole3), new SqlParameter("@Hole4", item.Hole4), new SqlParameter("@Hole5", item.Hole5), new SqlParameter("@Hole6", item.Hole6), new SqlParameter("@StrengthenTimes", item.StrengthenTimes), new SqlParameter("@Hole5Level", item.Hole5Level), new SqlParameter("@Hole5Exp", item.Hole5Exp), new SqlParameter("@Hole6Level", item.Hole6Level), new SqlParameter("@Hole6Exp", item.Hole6Exp), 
                    new SqlParameter("@IsGold", item.IsGold), new SqlParameter("@goldBeginTime", item.goldBeginTime.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@goldValidDate", item.goldValidDate), new SqlParameter("@StrengthenExp", item.StrengthenExp), new SqlParameter("@beadExp", item.beadExp), new SqlParameter("@beadLevel", item.beadLevel), new SqlParameter("@beadIsLock", item.beadIsLock), new SqlParameter("@isShowBind", item.isShowBind), new SqlParameter("@latentEnergyCurStr", item.latentEnergyCurStr), new SqlParameter("@latentEnergyNewStr", item.latentEnergyNewStr), new SqlParameter("@latentEnergyEndTime", item.latentEnergyEndTime.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@MagicAttack", item.MagicAttack), new SqlParameter("@MagicDefence", item.MagicDefence), new SqlParameter("@goodsLock", item.GoodsLock), new SqlParameter("@Damage", item.Damage), new SqlParameter("@Guard", item.Guard), 
                    new SqlParameter("@Blood", item.Blood), new SqlParameter("@Bless", item.Bless), new SqlParameter("@AdvanceDate", item.AdvanceDate.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@LianGrade", item.LianGrade), new SqlParameter("@LianExp", item.LianExp)
                 };
                flag = base.db.RunProcedure("SP_Users_Items_Update", sqlParameters);
                item.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool UpdateLabyrinthInfo(UserLabyrinthInfo laby)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { 
                    new SqlParameter("@UserID", laby.UserID), new SqlParameter("@myProgress", laby.myProgress), new SqlParameter("@myRanking", laby.myRanking), new SqlParameter("@completeChallenge", laby.completeChallenge), new SqlParameter("@isDoubleAward", laby.isDoubleAward), new SqlParameter("@currentFloor", laby.currentFloor), new SqlParameter("@accumulateExp", laby.accumulateExp), new SqlParameter("@remainTime", laby.remainTime), new SqlParameter("@currentRemainTime", laby.currentRemainTime), new SqlParameter("@cleanOutAllTime", laby.cleanOutAllTime), new SqlParameter("@cleanOutGold", laby.cleanOutGold), new SqlParameter("@tryAgainComplete", laby.tryAgainComplete), new SqlParameter("@isInGame", laby.isInGame), new SqlParameter("@isCleanOut", laby.isCleanOut), new SqlParameter("@serverMultiplyingPower", laby.serverMultiplyingPower), new SqlParameter("@LastDate", laby.LastDate.ToString("MM/dd/yyyy hh:mm:ss")), 
                    new SqlParameter("@ProcessAward", laby.ProcessAward), new SqlParameter("@Result", SqlDbType.Int)
                 };
                sqlParameters[0x11].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UpdateLabyrinthInfo", sqlParameters);
                flag = true;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_UpdateLabyrinthInfo", exception);
                }
            }
            return flag;
        }

        public bool UpdateLastVIPPackTime(int ID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", ID), new SqlParameter("@LastVIPPackTime", DateTime.Now.Date), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[2].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UpdateUserLastVIPPackTime", sqlParameters);
                flag = true;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_UpdateUserLastVIPPackTime", exception);
                }
            }
            return flag;
        }

        public bool UpdateMail(MailInfo mail, int oldMoney)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[30];
                sqlParameters[0] = new SqlParameter("@ID", mail.ID);
                sqlParameters[1] = new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1);
                sqlParameters[2] = new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2);
                sqlParameters[3] = new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content);
                sqlParameters[4] = new SqlParameter("@Gold", mail.Gold);
                sqlParameters[5] = new SqlParameter("@IsExist", mail.IsExist);
                sqlParameters[6] = new SqlParameter("@Money", mail.Money);
                sqlParameters[7] = new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver);
                sqlParameters[8] = new SqlParameter("@ReceiverID", mail.ReceiverID);
                sqlParameters[9] = new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender);
                sqlParameters[10] = new SqlParameter("@SenderID", mail.SenderID);
                sqlParameters[11] = new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title);
                sqlParameters[12] = new SqlParameter("@IfDelS", false);
                sqlParameters[13] = new SqlParameter("@IsDelete", false);
                sqlParameters[14] = new SqlParameter("@IsDelR", false);
                sqlParameters[15] = new SqlParameter("@IsRead", mail.IsRead);
                sqlParameters[0x10] = new SqlParameter("@SendTime", mail.SendTime.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[0x11] = new SqlParameter("@Type", mail.Type);
                sqlParameters[0x12] = new SqlParameter("@OldMoney", oldMoney);
                sqlParameters[0x13] = new SqlParameter("@ValidDate", mail.ValidDate);
                sqlParameters[20] = new SqlParameter("@Annex1Name", mail.Annex1Name);
                sqlParameters[0x15] = new SqlParameter("@Annex2Name", mail.Annex2Name);
                sqlParameters[0x16] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[0x16].Direction = ParameterDirection.ReturnValue;
                sqlParameters[0x17] = new SqlParameter("@Annex3", (mail.Annex3 == null) ? "" : mail.Annex3);
                sqlParameters[0x18] = new SqlParameter("@Annex4", (mail.Annex4 == null) ? "" : mail.Annex4);
                sqlParameters[0x19] = new SqlParameter("@Annex5", (mail.Annex5 == null) ? "" : mail.Annex5);
                sqlParameters[0x1a] = new SqlParameter("@Annex3Name", (mail.Annex3Name == null) ? "" : mail.Annex3Name);
                sqlParameters[0x1b] = new SqlParameter("@Annex4Name", (mail.Annex4Name == null) ? "" : mail.Annex4Name);
                sqlParameters[0x1c] = new SqlParameter("@Annex5Name", (mail.Annex5Name == null) ? "" : mail.Annex5Name);
                sqlParameters[0x1d] = new SqlParameter("GiftToken", mail.GiftToken);
                base.db.RunProcedure("SP_Mail_Update", sqlParameters);
                int num = (int) sqlParameters[0x16].Value;
                flag = num == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool UpdateMarryInfo(MarryInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", info.ID), new SqlParameter("@UserID", info.UserID), new SqlParameter("@IsPublishEquip", info.IsPublishEquip), new SqlParameter("@Introduction", info.Introduction), new SqlParameter("@RegistTime", info.RegistTime.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[5].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_MarryInfo_Update", sqlParameters);
                int num = (int) sqlParameters[5].Value;
                flag = num == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool UpdateMarryRoomInfo(MarryRoomInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", info.ID), new SqlParameter("@AvailTime", info.AvailTime), new SqlParameter("@BreakTime", info.BreakTime), new SqlParameter("@roomIntroduction", info.RoomIntroduction), new SqlParameter("@isHymeneal", info.IsHymeneal), new SqlParameter("@Name", info.Name), new SqlParameter("@Pwd", info.Pwd), new SqlParameter("@IsGunsaluteUsed", info.IsGunsaluteUsed), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[8].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_Update_Marry_Room_Info", sqlParameters);
                flag = ((int) sqlParameters[8].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("UpdateMarryRoomInfo", exception);
                }
            }
            return flag;
        }

        public bool UpdateNewChickenBox(NewChickenBoxItemInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", info.ID), new SqlParameter("@UserID", info.UserID), new SqlParameter("@TemplateID", info.TemplateID), new SqlParameter("@Count", info.Count), new SqlParameter("@ValidDate", info.ValidDate), new SqlParameter("@StrengthenLevel", info.StrengthenLevel), new SqlParameter("@AttackCompose", info.AttackCompose), new SqlParameter("@DefendCompose", info.DefendCompose), new SqlParameter("@AgilityCompose", info.AgilityCompose), new SqlParameter("@LuckCompose", info.LuckCompose), new SqlParameter("@Position", info.Position), new SqlParameter("@IsSelected", info.IsSelected), new SqlParameter("@IsSeeded", info.IsSeeded), new SqlParameter("@IsBinds", info.IsBinds), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[14].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UpdateNewChickenBox", sqlParameters);
                flag = ((int) sqlParameters[14].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_UpdateNewChickenBox", exception);
                }
            }
            return flag;
        }

        public bool UpdatePassWord(int userID, string password)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", userID), new SqlParameter("@Password", password) };
                flag = base.db.RunProcedure("SP_Users_UpdatePassword", sqlParameters);
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool UpdatePasswordInfo(int userID, string PasswordQuestion1, string PasswordAnswer1, string PasswordQuestion2, string PasswordAnswer2, int Count)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", userID), new SqlParameter("@PasswordQuestion1", PasswordQuestion1), new SqlParameter("@PasswordAnswer1", PasswordAnswer1), new SqlParameter("@PasswordQuestion2", PasswordQuestion2), new SqlParameter("@PasswordAnswer2", PasswordAnswer2), new SqlParameter("@FailedPasswordAttemptCount", Count) };
                flag = base.db.RunProcedure("SP_Users_Password_Add", sqlParameters);
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool UpdatePasswordTwo(int userID, string passwordTwo)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", userID), new SqlParameter("@PasswordTwo", passwordTwo) };
                flag = base.db.RunProcedure("SP_Users_UpdatePasswordTwo", sqlParameters);
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool UpdatePlayer(PlayerInfo player)
        {
            bool flag = false;
            try
            {
                if (player.Grade < 1)
                {
                    return flag;
                }
                SqlParameter[] sqlParameters = new SqlParameter[90];
                sqlParameters[0] = new SqlParameter("@UserID", player.ID);
                sqlParameters[1] = new SqlParameter("@Attack", player.Attack);
                sqlParameters[2] = new SqlParameter("@Colors", (player.Colors == null) ? "" : player.Colors);
                sqlParameters[3] = new SqlParameter("@ConsortiaID", player.ConsortiaID);
                sqlParameters[4] = new SqlParameter("@Defence", player.Defence);
                sqlParameters[5] = new SqlParameter("@Gold", player.Gold);
                sqlParameters[6] = new SqlParameter("@GP", player.GP);
                sqlParameters[7] = new SqlParameter("@Grade", player.Grade);
                sqlParameters[8] = new SqlParameter("@Luck", player.Luck);
                sqlParameters[9] = new SqlParameter("@Money", player.Money);
                sqlParameters[10] = new SqlParameter("@Style", (player.Style == null) ? "" : player.Style);
                sqlParameters[11] = new SqlParameter("@Agility", player.Agility);
                sqlParameters[12] = new SqlParameter("@State", player.State);
                sqlParameters[13] = new SqlParameter("@Hide", player.Hide);
                sqlParameters[14] = new SqlParameter("@ExpendDate", !player.ExpendDate.HasValue ? "" : player.ExpendDate.ToString());
                sqlParameters[15] = new SqlParameter("@Win", player.Win);
                sqlParameters[0x10] = new SqlParameter("@Total", player.Total);
                sqlParameters[0x11] = new SqlParameter("@Escape", player.Escape);
                sqlParameters[0x12] = new SqlParameter("@Skin", (player.Skin == null) ? "" : player.Skin);
                sqlParameters[0x13] = new SqlParameter("@Offer", player.Offer);
                sqlParameters[20] = new SqlParameter("@AntiAddiction", player.AntiAddiction);
                sqlParameters[20].Direction = ParameterDirection.InputOutput;
                sqlParameters[0x15] = new SqlParameter("@Result", SqlDbType.Int);
                sqlParameters[0x15].Direction = ParameterDirection.ReturnValue;
                sqlParameters[0x16] = new SqlParameter("@RichesOffer", player.RichesOffer);
                sqlParameters[0x17] = new SqlParameter("@RichesRob", player.RichesRob);
                sqlParameters[0x18] = new SqlParameter("@CheckCount", player.CheckCount);
                sqlParameters[0x18].Direction = ParameterDirection.InputOutput;
                sqlParameters[0x19] = new SqlParameter("@MarryInfoID", player.MarryInfoID);
                sqlParameters[0x1a] = new SqlParameter("@DayLoginCount", player.DayLoginCount);
                sqlParameters[0x1b] = new SqlParameter("@Nimbus", player.Nimbus);
                sqlParameters[0x1c] = new SqlParameter("@LastAward", player.LastAward.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[0x1d] = new SqlParameter("@GiftToken", player.GiftToken);
                sqlParameters[30] = new SqlParameter("@QuestSite", player.QuestSite);
                sqlParameters[0x1f] = new SqlParameter("@PvePermission", player.PvePermission);
                sqlParameters[0x20] = new SqlParameter("@FightPower", player.FightPower);
                sqlParameters[0x21] = new SqlParameter("@AnswerSite", player.AnswerSite);
                sqlParameters[0x22] = new SqlParameter("@LastAuncherAward", player.LastAward.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[0x23] = new SqlParameter("@hp", player.hp);
                sqlParameters[0x24] = new SqlParameter("@ChatCount", player.ChatCount);
                sqlParameters[0x25] = new SqlParameter("@SpaPubGoldRoomLimit", player.SpaPubGoldRoomLimit);
                sqlParameters[0x26] = new SqlParameter("@LastSpaDate", player.LastSpaDate.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[0x27] = new SqlParameter("@FightLabPermission", player.FightLabPermission);
                sqlParameters[40] = new SqlParameter("@SpaPubMoneyRoomLimit", player.SpaPubMoneyRoomLimit);
                sqlParameters[0x29] = new SqlParameter("@IsInSpaPubGoldToday", player.IsInSpaPubGoldToday);
                sqlParameters[0x2a] = new SqlParameter("@IsInSpaPubMoneyToday", player.IsInSpaPubMoneyToday);
                sqlParameters[0x2b] = new SqlParameter("@AchievementPoint", player.AchievementPoint);
                sqlParameters[0x2c] = new SqlParameter("@LastWeekly", player.LastWeekly.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[0x2d] = new SqlParameter("@LastWeeklyVersion", player.LastWeeklyVersion);
                sqlParameters[0x2e] = new SqlParameter("@GiftGp", player.GiftGp);
                sqlParameters[0x2f] = new SqlParameter("@GiftLevel", player.GiftLevel);
                sqlParameters[0x30] = new SqlParameter("@IsOpenGift", player.IsOpenGift);
                sqlParameters[0x31] = new SqlParameter("@WeaklessGuildProgressStr", player.WeaklessGuildProgressStr);
                sqlParameters[50] = new SqlParameter("@IsOldPlayer", player.IsOldPlayer);
                sqlParameters[0x33] = new SqlParameter("@VIPLevel", player.VIPLevel);
                sqlParameters[0x34] = new SqlParameter("@VIPExp", player.VIPExp);
                sqlParameters[0x35] = new SqlParameter("@Score", player.Score);
                sqlParameters[0x36] = new SqlParameter("@OptionOnOff", player.OptionOnOff);
                sqlParameters[0x37] = new SqlParameter("@isOldPlayerHasValidEquitAtLogin", player.isOldPlayerHasValidEquitAtLogin);
                sqlParameters[0x38] = new SqlParameter("@badLuckNumber", player.badLuckNumber);
                sqlParameters[0x39] = new SqlParameter("@luckyNum", player.luckyNum);
                sqlParameters[0x3a] = new SqlParameter("@lastLuckyNumDate", player.lastLuckyNumDate.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[0x3b] = new SqlParameter("@lastLuckNum", player.lastLuckNum);
                sqlParameters[60] = new SqlParameter("@CardSoul", player.CardSoul);
                sqlParameters[0x3d] = new SqlParameter("@uesedFinishTime", player.uesedFinishTime);
                sqlParameters[0x3e] = new SqlParameter("@totemId", player.totemId);
                sqlParameters[0x3f] = new SqlParameter("@damageScores", player.damageScores);
                sqlParameters[0x40] = new SqlParameter("@petScore", player.petScore);
                sqlParameters[0x41] = new SqlParameter("@IsShowConsortia", player.IsShowConsortia);
                sqlParameters[0x42] = new SqlParameter("@LastRefreshPet", player.LastRefreshPet.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[0x43] = new SqlParameter("@GetSoulCount", player.GetSoulCount);
                sqlParameters[0x44] = new SqlParameter("@isFirstDivorce", player.isFirstDivorce);
                sqlParameters[0x45] = new SqlParameter("@needGetBoxTime", player.needGetBoxTime);
                sqlParameters[70] = new SqlParameter("@myScore", player.myScore);
                sqlParameters[0x47] = new SqlParameter("@TimeBox", player.TimeBox.ToString("MM/dd/yyyy hh:mm:ss"));
                sqlParameters[0x48] = new SqlParameter("@IsFistGetPet", player.IsFistGetPet);
                sqlParameters[0x49] = new SqlParameter("@MaxBuyHonor", player.MaxBuyHonor);
                sqlParameters[0x4a] = new SqlParameter("@Medal", player.medal);
                sqlParameters[0x4b] = new SqlParameter("@myHonor", player.myHonor);
                sqlParameters[0x4c] = new SqlParameter("@LeagueMoney", player.LeagueMoney);
                sqlParameters[0x4d] = new SqlParameter("@Honor", player.Honor);
                sqlParameters[0x4e] = new SqlParameter("@necklaceExp", player.necklaceExp);
                sqlParameters[0x4f] = new SqlParameter("@necklaceExpAdd", player.necklaceExpAdd);
                sqlParameters[80] = new SqlParameter("@hardCurrency", player.hardCurrency);
                sqlParameters[0x51] = new SqlParameter("@CurrentDressModel", player.CurrentDressModel);
                sqlParameters[0x52] = new SqlParameter("@MagicAttack", player.MagicAttack);
                sqlParameters[0x53] = new SqlParameter("@MagicDefence", player.MagicDefence);
                sqlParameters[0x54] = new SqlParameter("@evolutionExp", player.evolutionExp);
                sqlParameters[0x55] = new SqlParameter("@evolutionGrade", player.evolutionGrade);
                sqlParameters[0x56] = new SqlParameter("@MagicStonePoint", player.MagicStonePoint);
                sqlParameters[0x57] = new SqlParameter("@GoXu", player.GoXu);
                sqlParameters[0x58] = new SqlParameter("@DDPlayPoint", player.DDPlayPoint);
                sqlParameters[0x59] = new SqlParameter("@AchievementProcess", player.AchievementProcess);
                base.db.RunProcedure("SP_Users_Update", sqlParameters);
                flag = ((int) sqlParameters[0x15].Value) == 0;
                if (flag)
                {
                    player.AntiAddiction = (int) sqlParameters[20].Value;
                    player.CheckCount = (int) sqlParameters[0x18].Value;
                }
                player.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool UpdatePlayerGotRingProp(int groomID, int brideID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@GroomID", groomID), new SqlParameter("@BrideID", brideID), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[2].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_Update_GotRing_Prop", sqlParameters);
                flag = ((int) sqlParameters[2].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("UpdatePlayerGotRingProp", exception);
                }
            }
            return flag;
        }

        public bool UpdatePlayerLastAward(int id, int type)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", id), new SqlParameter("@Type", type) };
                flag = base.db.RunProcedure("SP_Users_LastAward", sqlParameters);
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("UpdatePlayerAward", exception);
                }
            }
            return flag;
        }

        public bool UpdatePlayerMarry(PlayerInfo player)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", player.ID), new SqlParameter("@IsMarried", player.IsMarried), new SqlParameter("@SpouseID", player.SpouseID), new SqlParameter("@SpouseName", player.SpouseName), new SqlParameter("@IsCreatedMarryRoom", player.IsCreatedMarryRoom), new SqlParameter("@SelfMarryRoomID", player.SelfMarryRoomID), new SqlParameter("@IsGotRing", player.IsGotRing) };
                flag = base.db.RunProcedure("SP_Users_Marry", sqlParameters);
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("UpdatePlayerMarry", exception);
                }
            }
            return flag;
        }

        public bool UpdatePlayerMarryApply(int UserID, string loveProclamation, bool isExist)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", UserID), new SqlParameter("@LoveProclamation", loveProclamation), new SqlParameter("@isExist", isExist), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[3].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_Update_Marry_Apply", sqlParameters);
                flag = ((int) sqlParameters[3].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("UpdatePlayerMarryApply", exception);
                }
            }
            return flag;
        }

        public bool UpdatePyramid(PyramidInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", info.ID), new SqlParameter("@UserID", info.UserID), new SqlParameter("@currentLayer", info.currentLayer), new SqlParameter("@maxLayer", info.maxLayer), new SqlParameter("@totalPoint", info.totalPoint), new SqlParameter("@turnPoint", info.turnPoint), new SqlParameter("@pointRatio", info.pointRatio), new SqlParameter("@currentFreeCount", info.currentFreeCount), new SqlParameter("@currentReviveCount", info.currentReviveCount), new SqlParameter("@isPyramidStart", info.isPyramidStart), new SqlParameter("@LayerItems", info.LayerItems), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[11].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UpdatePyramid", sqlParameters);
                flag = ((int) sqlParameters[11].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_UpdatePyramid", exception);
                }
            }
            return flag;
        }

        public bool UpdateqPet(PetEquipDataInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", info.ID), new SqlParameter("@UserID", info.UserID), new SqlParameter("@PetID", info.PetID), new SqlParameter("@eqType", info.eqType), new SqlParameter("@eqTemplateID", info.eqTemplateID), new SqlParameter("@startTime", info.startTime.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@ValidDate", info.ValidDate), new SqlParameter("@IsExit", info.IsExit), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[8].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_eqPet_Update", sqlParameters);
                int num = (int) sqlParameters[8].Value;
                flag = num == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool UpdateTreasureData(TreasureDataInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", item.ID), new SqlParameter("@UserID", item.UserID), new SqlParameter("@TemplateID", item.TemplateID), new SqlParameter("@Count", item.Count), new SqlParameter("@Validate", item.ValidDate), new SqlParameter("@Pos", item.pos), new SqlParameter("@BeginDate", item.BeginDate.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@IsExit", item.IsExit), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[8].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UpdateTreasureData", sqlParameters);
                flag = ((int) sqlParameters[8].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_UpdateTreasureData", exception);
                }
            }
            return flag;
        }

        public bool UpdateUserAdoptPet(int ID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", ID), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[1].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_Update_User_AdoptPet", sqlParameters);
                int num = (int) sqlParameters[1].Value;
                flag = num == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool UpdateUserAvatarCollect(UserAvatarCollectionInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", item.ID), new SqlParameter("@UserID", item.UserID), new SqlParameter("@AvatarID", item.AvatarID), new SqlParameter("@Sex", item.Sex), new SqlParameter("@IsActive", item.IsActive), new SqlParameter("@Data", item.Data), new SqlParameter("@TimeStart", item.TimeStart.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@TimeEnd", item.TimeEnd.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@IsExit", item.IsExit), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[9].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_AvatarCollect_Update", sqlParameters);
                flag = ((int) sqlParameters[9].Value) == 0;
                item.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_AvatarCollect_Update", exception);
                }
            }
            return flag;
        }

        public bool UpdateUserChristmas(UserChristmasInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", info.ID), new SqlParameter("@UserID", info.UserID), new SqlParameter("@exp", info.exp), new SqlParameter("@awardState", info.awardState), new SqlParameter("@count", info.count), new SqlParameter("@packsNumber", info.packsNumber), new SqlParameter("@lastPacks", info.lastPacks), new SqlParameter("@gameBeginTime", info.gameBeginTime.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@gameEndTime", info.gameEndTime.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@isEnter", info.isEnter), new SqlParameter("@dayPacks", info.dayPacks), new SqlParameter("@AvailTime", info.AvailTime), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[12].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UpdateUserChristmas", sqlParameters);
                flag = ((int) sqlParameters[12].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_UpdateUserChristmas", exception);
                }
            }
            return flag;
        }

        public bool UpdateUserDressModel(UserDressModelInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", item.ID), new SqlParameter("@UserID", item.UserID), new SqlParameter("@SlotID", item.SlotID), new SqlParameter("@TemplateID", item.TemplateID), new SqlParameter("@ItemID", item.ItemID), new SqlParameter("@CategoryID", item.CategoryID), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[6].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_DressModel_Update", sqlParameters);
                flag = ((int) sqlParameters[6].Value) == 0;
                item.IsDirty = false;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_DressModel_Update", exception);
                }
            }
            return flag;
        }

        public bool UpdateUserDrillInfo(UserDrillInfo g)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", g.UserID), new SqlParameter("@BeadPlace", g.BeadPlace), new SqlParameter("@HoleExp", g.HoleExp), new SqlParameter("@HoleLv", g.HoleLv), new SqlParameter("@DrillPlace", g.DrillPlace), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[5].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UpdateUserDrillInfo", sqlParameters);
                flag = true;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_UpdateUserDrillInfo", exception);
                }
            }
            return flag;
        }

        public bool UpdateUserMatchInfo(UserMatchInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", info.ID), new SqlParameter("@UserID", info.UserID), new SqlParameter("@dailyScore", info.dailyScore), new SqlParameter("@dailyWinCount", info.dailyWinCount), new SqlParameter("@dailyGameCount", info.dailyGameCount), new SqlParameter("@DailyLeagueFirst", info.DailyLeagueFirst), new SqlParameter("@DailyLeagueLastScore", info.DailyLeagueLastScore), new SqlParameter("@weeklyScore", info.weeklyScore), new SqlParameter("@weeklyGameCount", info.weeklyGameCount), new SqlParameter("@weeklyRanking", info.weeklyRanking), new SqlParameter("@addDayPrestge", info.addDayPrestge), new SqlParameter("@totalPrestige", info.totalPrestige), new SqlParameter("@restCount", info.restCount), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[13].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UpdateUserMatch", sqlParameters);
                flag = ((int) sqlParameters[13].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_UpdateUserMatch", exception);
                }
            }
            return flag;
        }

        public bool UpdateUserPet(UsersPetinfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { 
                    new SqlParameter("@TemplateID", info.TemplateID), new SqlParameter("@Name", (info.Name == null) ? "Error!" : info.Name), new SqlParameter("@UserID", info.UserID), new SqlParameter("@Attack", info.Attack), new SqlParameter("@Defence", info.Defence), new SqlParameter("@Luck", info.Luck), new SqlParameter("@Agility", info.Agility), new SqlParameter("@Blood", info.Blood), new SqlParameter("@Damage", info.Damage), new SqlParameter("@Guard", info.Guard), new SqlParameter("@AttackGrow", info.AttackGrow), new SqlParameter("@DefenceGrow", info.DefenceGrow), new SqlParameter("@LuckGrow", info.LuckGrow), new SqlParameter("@AgilityGrow", info.AgilityGrow), new SqlParameter("@BloodGrow", info.BloodGrow), new SqlParameter("@DamageGrow", info.DamageGrow), 
                    new SqlParameter("@GuardGrow", info.GuardGrow), new SqlParameter("@Level", info.Level), new SqlParameter("@GP", info.GP), new SqlParameter("@MaxGP", info.MaxGP), new SqlParameter("@Hunger", info.Hunger), new SqlParameter("@PetHappyStar", info.PetHappyStar), new SqlParameter("@MP", info.MP), new SqlParameter("@IsEquip", info.IsEquip), new SqlParameter("@Place", info.Place), new SqlParameter("@IsExit", info.IsExit), new SqlParameter("@ID", info.ID), new SqlParameter("@Skill", info.Skill), new SqlParameter("@SkillEquip", info.SkillEquip), new SqlParameter("@currentStarExp", info.currentStarExp), new SqlParameter("@Result", SqlDbType.Int)
                 };
                sqlParameters[30].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UserPet_Update", sqlParameters);
                int num = (int) sqlParameters[30].Value;
                flag = num == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool UpdateUserRank(UserRankInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", item.ID), new SqlParameter("@UserID", item.UserID), new SqlParameter("@UserRank", item.UserRank), new SqlParameter("@Attack", item.Attack), new SqlParameter("@Defence", item.Defence), new SqlParameter("@Luck", item.Luck), new SqlParameter("@Agility", item.Agility), new SqlParameter("@HP", item.HP), new SqlParameter("@Damage", item.Damage), new SqlParameter("@Guard", item.Guard), new SqlParameter("@BeginDate", item.BeginDate.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@Validate", item.Validate), new SqlParameter("@IsExit", item.IsExit), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[13].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UpdateUserRank", sqlParameters);
                flag = ((int) sqlParameters[13].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_UpdateUserRank", exception);
                }
            }
            return flag;
        }

        public bool UpdateUsersExtra(UsersExtraInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { 
                    new SqlParameter("@ID", info.ID), new SqlParameter("@UserID", info.UserID), new SqlParameter("@starlevel", info.starlevel), new SqlParameter("@nowPosition", info.nowPosition), new SqlParameter("@FreeCount", info.FreeCount), new SqlParameter("@SearchGoodItems", info.SearchGoodItems), new SqlParameter("@FreeAddAutionCount", info.FreeAddAutionCount), new SqlParameter("@FreeSendMailCount", info.FreeSendMailCount), new SqlParameter("@KingBlessInfo", info.KingBleesInfo), new SqlParameter("@MissionEnergy", info.MissionEnergy), new SqlParameter("@buyEnergyCount", info.buyEnergyCount), new SqlParameter("@KingBlessEnddate", info.KingBlessEnddate), new SqlParameter("@KingBlessIndex", info.KingBlessIndex), new SqlParameter("@LastTimeHotSpring", info.LastTimeHotSpring), new SqlParameter("@LastFreeTimeHotSpring", info.LastFreeTimeHotSpring), new SqlParameter("@MinHotSpring", info.MinHotSpring), 
                    new SqlParameter("@coupleBossEnterNum", info.coupleBossEnterNum), new SqlParameter("@coupleBossHurt", info.coupleBossHurt), new SqlParameter("@coupleBossBoxNum", info.coupleBossBoxNum), new SqlParameter("@Result", SqlDbType.Int)
                 };
                sqlParameters[0x13].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UpdateUsersExtra", sqlParameters);
                flag = ((int) sqlParameters[0x10].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_UpdateUsersExtra", exception);
                }
            }
            return flag;
        }

        public bool UpdateUserTexpInfo(TexpInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", info.UserID), new SqlParameter("@attTexpExp", info.attTexpExp), new SqlParameter("@defTexpExp", info.defTexpExp), new SqlParameter("@hpTexpExp", info.hpTexpExp), new SqlParameter("@lukTexpExp", info.lukTexpExp), new SqlParameter("@spdTexpExp", info.spdTexpExp), new SqlParameter("@texpCount", info.texpCount), new SqlParameter("@texpTaskCount", info.texpTaskCount), new SqlParameter("@texpTaskDate", info.texpTaskDate.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[9].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UserTexp_Update", sqlParameters);
                int num = (int) sqlParameters[9].Value;
                flag = num == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
            }
            return flag;
        }

        public bool UpdateUserTreasureInfo(UserTreasureInfo item)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", item.UserID), new SqlParameter("@NickName", item.NickName), new SqlParameter("@logoinDays", item.logoinDays), new SqlParameter("@treasure", item.treasure), new SqlParameter("@treasureAdd", item.treasureAdd), new SqlParameter("@friendHelpTimes", item.friendHelpTimes), new SqlParameter("@isEndTreasure", item.isEndTreasure), new SqlParameter("@isBeginTreasure", item.isBeginTreasure), new SqlParameter("@LastLoginDay", item.LastLoginDay.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[9].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UpdateUserTreasure", sqlParameters);
                flag = ((int) sqlParameters[9].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_UpdateUserTreasure", exception);
                }
            }
            return flag;
        }

        public bool UpdateVIPInfo(PlayerInfo p)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", p.ID), new SqlParameter("@VIPLevel", p.VIPLevel), new SqlParameter("@VIPExp", p.VIPExp), new SqlParameter("@VIPOnlineDays", SqlDbType.BigInt), new SqlParameter("@VIPOfflineDays", SqlDbType.BigInt), new SqlParameter("@VIPExpireDay", p.VIPExpireDay.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@VIPLastDate", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss")), new SqlParameter("@VIPNextLevelDaysNeeded", SqlDbType.BigInt), new SqlParameter("@CanTakeVipReward", p.CanTakeVipReward), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[9].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_UpdateVIPInfo", sqlParameters);
                flag = true;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_UpdateVIPInfo", exception);
                }
            }
            return flag;
        }

        public int VIPLastdate(int ID)
        {
            int num = 0;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", ID), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[1].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_VIPLastdate_Single", sqlParameters);
                num = (int) sqlParameters[1].Value;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_VIPLastdate_Single", exception);
                }
            }
            return num;
        }

        public int VIPRenewal(string nickName, int renewalDays, ref DateTime ExpireDayOut)
        {
            int num = 0;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@NickName", nickName), new SqlParameter("@RenewalDays", renewalDays), new SqlParameter("@ExpireDayOut", DateTime.Now), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[2].Direction = ParameterDirection.Output;
                sqlParameters[3].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_VIPRenewal_Single", sqlParameters);
                ExpireDayOut = (DateTime) sqlParameters[2].Value;
                num = (int) sqlParameters[3].Value;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_VIPRenewal_Single", exception);
                }
            }
            return num;
        }
    }
}

