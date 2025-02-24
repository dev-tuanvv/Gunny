namespace Bussiness
{
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    public class ProduceBussiness : BaseBussiness
    {
        public bool AddAASInfo(AASInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", info.UserID), new SqlParameter("@Name", info.Name), new SqlParameter("@IDNumber", info.IDNumber), new SqlParameter("@State", info.State), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[4].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_ASSInfo_Add", sqlParameters);
                flag = ((int) sqlParameters[4].Value) == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("UpdateAASInfo", exception);
                }
            }
            return flag;
        }

        public bool AddDailyLogList(DailyLogListInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[5];
                para[0] = new SqlParameter("@UserID", info.UserID);
                para[1] = new SqlParameter("@UserAwardLog", info.UserAwardLog);
                para[2] = new SqlParameter("@DayLog", info.DayLog);
                para[3] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[3].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_DailyLogList_Add", para);
                result = (int)para[3].Value == 0;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("UpdateAASInfo", exception);
                }
            }
            return result;
        }

        public void AddUserLogEvent(int UserID, string UserName, string NickName, string Type, string Content)
        {
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", UserID), new SqlParameter("@UserName", UserName), new SqlParameter("@NickName", NickName), new SqlParameter("@Type", Type), new SqlParameter("@Content", Content) };
                base.db.RunProcedure("SP_Insert_UsersLog", sqlParameters);
            }
            catch (Exception)
            {
            }
        }

        public AASInfo[] GetAllAASInfo()
        {
            List<AASInfo> list = new List<AASInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_AASInfo_All");
                while (resultDataReader.Read())
                {
                    AASInfo item = new AASInfo {
                        UserID = (int) resultDataReader["ID"],
                        Name = resultDataReader["Name"].ToString(),
                        IDNumber = resultDataReader["IDNumber"].ToString(),
                        State = (int) resultDataReader["State"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllAASInfo", exception);
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

        public ActiveAwardInfo[] GetAllActiveAwardInfo()
        {
            List<ActiveAwardInfo> list = new List<ActiveAwardInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Active_Award");
                while (resultDataReader.Read())
                {
                    ActiveAwardInfo item = new ActiveAwardInfo {
                        ID = (int) resultDataReader["ID"],
                        ActiveID = (int) resultDataReader["ActiveID"],
                        AgilityCompose = (int) resultDataReader["AgilityCompose"],
                        AttackCompose = (int) resultDataReader["AttackCompose"],
                        Count = (int) resultDataReader["Count"],
                        DefendCompose = (int) resultDataReader["DefendCompose"],
                        Gold = (int) resultDataReader["Gold"],
                        ItemID = (int) resultDataReader["ItemID"],
                        LuckCompose = (int) resultDataReader["LuckCompose"],
                        Mark = (int) resultDataReader["Mark"],
                        Money = (int) resultDataReader["Money"],
                        Sex = (int) resultDataReader["Sex"],
                        StrengthenLevel = (int) resultDataReader["StrengthenLevel"],
                        ValidDate = (int) resultDataReader["ValidDate"],
                        GiftToken = (int) resultDataReader["GiftToken"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllActiveAwardInfo", exception);
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

        public ActiveConditionInfo[] GetAllActiveConditionInfo()
        {
            List<ActiveConditionInfo> list = new List<ActiveConditionInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Active_Condition");
                while (resultDataReader.Read())
                {
                    ActiveConditionInfo item = new ActiveConditionInfo {
                        ID = (int) resultDataReader["ID"],
                        ActiveID = (int) resultDataReader["ActiveID"],
                        Conditiontype = (int) resultDataReader["Conditiontype"],
                        Condition = (int) resultDataReader["Condition"],
                        LimitGrade = (resultDataReader["LimitGrade"].ToString() == null) ? "" : resultDataReader["LimitGrade"].ToString(),
                        AwardId = (resultDataReader["AwardId"].ToString() == null) ? "" : resultDataReader["AwardId"].ToString(),
                        IsMult = (bool) resultDataReader["IsMult"],
                        StartTime = (DateTime) resultDataReader["StartTime"],
                        EndTime = (DateTime) resultDataReader["EndTime"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllActiveConditionInfo", exception);
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

        public AchievementInfo[] GetAllAchievement()
        {
            List<AchievementInfo> list = new List<AchievementInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Get_Achievement");
                while (resultDataReader.Read())
                {
                    AchievementInfo item = new AchievementInfo {
                        ID = (int) resultDataReader["ID"],
                        PlaceID = (int) resultDataReader["PlaceID"],
                        Title = (string) resultDataReader["Title"],
                        Detail = (string) resultDataReader["Detail"],
                        NeedMinLevel = (int) resultDataReader["NeedMinLevel"],
                        NeedMaxLevel = (int) resultDataReader["NeedMaxLevel"],
                        PreAchievementID = (string) resultDataReader["PreAchievementID"],
                        IsOther = (int) resultDataReader["IsOther"],
                        AchievementType = (int) resultDataReader["AchievementType"],
                        CanHide = (bool) resultDataReader["CanHide"],
                        StartDate = (DateTime) resultDataReader["StartDate"],
                        EndDate = (DateTime) resultDataReader["EndDate"],
                        AchievementPoint = (int) resultDataReader["AchievementPoint"],
                        IsActive = (int) resultDataReader["IsActive"],
                        PicID = (int) resultDataReader["PicID"],
                        IsShare = (bool) resultDataReader["IsShare"]
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

        public AchievementInfo[] GetALlAchievement()
        {
            List<AchievementInfo> list = new List<AchievementInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Achievement_All");
                while (resultDataReader.Read())
                {
                    list.Add(this.InitAchievement(resultDataReader));
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetALlAchievement:", exception);
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

        public AchievementCondictionInfo[] GetAllAchievementCondiction()
        {
            List<AchievementCondictionInfo> list = new List<AchievementCondictionInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Get_AchievementCondiction");
                while (resultDataReader.Read())
                {
                    AchievementCondictionInfo item = new AchievementCondictionInfo {
                        AchievementID = (int) resultDataReader["AchievementID"],
                        CondictionID = (int) resultDataReader["CondictionID"],
                        CondictionType = (int) resultDataReader["CondictionType"],
                        Condiction_Para1 = (int) resultDataReader["Condiction_Para1"],
                        Condiction_Para2 = (int) resultDataReader["Condiction_Para2"]
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

        public AchievementConditionInfo[] GetALlAchievementCondition()
        {
            List<AchievementConditionInfo> list = new List<AchievementConditionInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Achievement_Condition_All");
                while (resultDataReader.Read())
                {
                    list.Add(this.InitAchievementCondition(resultDataReader));
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetALlAchievementCondition:", exception);
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

        public AchievementDataInfo[] GetAllAchievementData(int userID)
        {
            List<AchievementDataInfo> list = new List<AchievementDataInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", userID) };
                base.db.GetReader(ref resultDataReader, "SP_Achievement_Data_All", sqlParameters);
                while (resultDataReader.Read())
                {
                    list.Add(this.InitAchievementData(resultDataReader));
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllAchievementData", exception);
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

        public AchievementGoodsInfo[] GetAllAchievementGoods()
        {
            List<AchievementGoodsInfo> list = new List<AchievementGoodsInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Get_AchievementGoods");
                while (resultDataReader.Read())
                {
                    AchievementGoodsInfo item = new AchievementGoodsInfo {
                        AchievementID = (int) resultDataReader["AchievementID"],
                        RewardType = (int) resultDataReader["RewardType"],
                        RewardPara = (string) resultDataReader["RewardPara"],
                        RewardValueId = (int) resultDataReader["RewardValueId"],
                        RewardCount = (int) resultDataReader["RewardCount"]
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

        public AchievementRewardInfo[] GetALlAchievementReward()
        {
            List<AchievementRewardInfo> list = new List<AchievementRewardInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Achievement_Reward_All");
                while (resultDataReader.Read())
                {
                    list.Add(this.InitAchievementReward(resultDataReader));
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetALlAchievementReward", exception);
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

        public List<BigBugleInfo> GetAllAreaBigBugleRecord()
        {
            SqlDataReader resultDataReader = null;
            List<BigBugleInfo> list = new List<BigBugleInfo>();
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Get_AreaBigBugle_Record");
                while (resultDataReader.Read())
                {
                    BigBugleInfo item = new BigBugleInfo {
                        ID = (int) resultDataReader["ID"],
                        UserID = (int) resultDataReader["UserID"],
                        AreaID = (int) resultDataReader["AreaID"],
                        NickName = (resultDataReader["NickName"] == null) ? "" : resultDataReader["NickName"].ToString(),
                        Message = (resultDataReader["Message"] == null) ? "" : resultDataReader["Message"].ToString(),
                        State = (bool) resultDataReader["State"],
                        IP = (resultDataReader["IP"] == null) ? "" : resultDataReader["IP"].ToString()
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllAreaBigBugleRecord", exception);
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

        public BallInfo[] GetAllBall()
        {
            List<BallInfo> list = new List<BallInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Ball_All");
                while (resultDataReader.Read())
                {
                    BallInfo item = new BallInfo {
                        Amount = (int) resultDataReader["Amount"],
                        ID = (int) resultDataReader["ID"],
                        Name = resultDataReader["Name"].ToString(),
                        Crater = (resultDataReader["Crater"] == null) ? "" : resultDataReader["Crater"].ToString(),
                        Power = (double) resultDataReader["Power"],
                        Radii = (int) resultDataReader["Radii"],
                        AttackResponse = (int) resultDataReader["AttackResponse"],
                        BombPartical = resultDataReader["BombPartical"].ToString(),
                        FlyingPartical = resultDataReader["FlyingPartical"].ToString(),
                        IsSpin = (bool) resultDataReader["IsSpin"],
                        Mass = (int) resultDataReader["Mass"],
                        SpinV = (int) resultDataReader["SpinV"],
                        SpinVA = (double) resultDataReader["SpinVA"],
                        Wind = (int) resultDataReader["Wind"],
                        DragIndex = (int) resultDataReader["DragIndex"],
                        Weight = (int) resultDataReader["Weight"],
                        Shake = (bool) resultDataReader["Shake"],
                        Delay = (int) resultDataReader["Delay"],
                        ShootSound = (resultDataReader["ShootSound"] == null) ? "" : resultDataReader["ShootSound"].ToString(),
                        BombSound = (resultDataReader["BombSound"] == null) ? "" : resultDataReader["BombSound"].ToString(),
                        ActionType = (int) resultDataReader["ActionType"],
                        HasTunnel = (bool) resultDataReader["HasTunnel"]
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

        public BallConfigInfo[] GetAllBallConfig()
        {
            List<BallConfigInfo> list = new List<BallConfigInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "[SP_Ball_Config_All]");
                while (resultDataReader.Read())
                {
                    BallConfigInfo item = new BallConfigInfo {
                        Common = (int) resultDataReader["Common"],
                        TemplateID = (int) resultDataReader["TemplateID"],
                        CommonAddWound = (int) resultDataReader["CommonAddWound"],
                        CommonMultiBall = (int) resultDataReader["CommonMultiBall"],
                        Special = (int) resultDataReader["Special"]
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

        public CategoryInfo[] GetAllCategory()
        {
            List<CategoryInfo> list = new List<CategoryInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Items_Category_All");
                while (resultDataReader.Read())
                {
                    CategoryInfo item = new CategoryInfo {
                        ID = (int) resultDataReader["ID"],
                        Name = (resultDataReader["Name"] == null) ? "" : resultDataReader["Name"].ToString(),
                        Place = (int) resultDataReader["Place"],
                        Remark = (resultDataReader["Remark"] == null) ? "" : resultDataReader["Remark"].ToString()
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

        public DailyAwardInfo[] GetAllDailyAward()
        {
            List<DailyAwardInfo> list = new List<DailyAwardInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Daily_Award_All");
                while (resultDataReader.Read())
                {
                    DailyAwardInfo item = new DailyAwardInfo {
                        Count = (int) resultDataReader["Count"],
                        ID = (int) resultDataReader["ID"],
                        IsBinds = (bool) resultDataReader["IsBinds"],
                        TemplateID = (int) resultDataReader["TemplateID"],
                        Type = (int) resultDataReader["Type"],
                        ValidDate = (int) resultDataReader["ValidDate"],
                        Sex = (int) resultDataReader["Sex"],
                        Remark = (resultDataReader["Remark"] == null) ? "" : resultDataReader["Remark"].ToString(),
                        CountRemark = (resultDataReader["CountRemark"] == null) ? "" : resultDataReader["CountRemark"].ToString(),
                        GetWay = (int) resultDataReader["GetWay"],
                        AwardDays = (int) resultDataReader["AwardDays"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllDaily", exception);
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

        public DropCondiction[] GetAllDropCondictions()
        {
            List<DropCondiction> list = new List<DropCondiction>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Drop_Condiction_All");
                while (resultDataReader.Read())
                {
                    list.Add(this.InitDropCondiction(resultDataReader));
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

        public DropItem[] GetAllDropItems()
        {
            List<DropItem> list = new List<DropItem>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Drop_Item_All");
                while (resultDataReader.Read())
                {
                    list.Add(this.InitDropItem(resultDataReader));
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

        public EdictumInfo[] GetAllEdictum()
        {
            List<EdictumInfo> list = new List<EdictumInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Edictum_All");
                while (resultDataReader.Read())
                {
                    list.Add(this.InitEdictum(resultDataReader));
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllEdictum", exception);
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

        public EventRewardGoodsInfo[] GetAllEventRewardGoods()
        {
            List<EventRewardGoodsInfo> list = new List<EventRewardGoodsInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Get_EventRewardGoods");
                while (resultDataReader.Read())
                {
                    EventRewardGoodsInfo item = new EventRewardGoodsInfo {
                        ActivityType = (int) resultDataReader["ActivityType"],
                        SubActivityType = (int) resultDataReader["SubActivityType"],
                        TemplateId = (int) resultDataReader["TemplateId"],
                        StrengthLevel = (int) resultDataReader["StrengthLevel"],
                        AttackCompose = (int) resultDataReader["AttackCompose"],
                        DefendCompose = (int) resultDataReader["DefendCompose"],
                        LuckCompose = (int) resultDataReader["LuckCompose"],
                        AgilityCompose = (int) resultDataReader["AgilityCompose"],
                        IsBind = (bool) resultDataReader["IsBind"],
                        ValidDate = (int) resultDataReader["ValidDate"],
                        Count = (int) resultDataReader["Count"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllEventRewardGoods", exception);
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

        public EventRewardInfo[] GetAllEventRewardInfo()
        {
            List<EventRewardInfo> list = new List<EventRewardInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Get_EventRewardInfo");
                while (resultDataReader.Read())
                {
                    EventRewardInfo item = new EventRewardInfo {
                        ActivityType = (int) resultDataReader["ActivityType"],
                        SubActivityType = (int) resultDataReader["SubActivityType"],
                        Condition = (int) resultDataReader["Condition"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllEventRewardInfo", exception);
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

        public FusionInfo[] GetAllFusion()
        {
            List<FusionInfo> list = new List<FusionInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Fusion_All");
                while (resultDataReader.Read())
                {
                    FusionInfo item = new FusionInfo {
                        FusionID = (int) resultDataReader["FusionID"],
                        Item1 = (int) resultDataReader["Item1"],
                        Item2 = (int) resultDataReader["Item2"],
                        Item3 = (int) resultDataReader["Item3"],
                        Item4 = (int) resultDataReader["Item4"],
                        Formula = (int) resultDataReader["Formula"],
                        Reward = (int) resultDataReader["Reward"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllFusion", exception);
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

        public FusionInfo[] GetAllFusionDesc()
        {
            List<FusionInfo> list = new List<FusionInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Fusion_All_Desc");
                while (resultDataReader.Read())
                {
                    FusionInfo item = new FusionInfo {
                        FusionID = (int) resultDataReader["FusionID"],
                        Item1 = (int) resultDataReader["Item1"],
                        Item2 = (int) resultDataReader["Item2"],
                        Item3 = (int) resultDataReader["Item3"],
                        Item4 = (int) resultDataReader["Item4"],
                        Formula = (int) resultDataReader["Formula"],
                        Reward = (int) resultDataReader["Reward"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllFusion", exception);
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

        public Items_Fusion_List_Info[] GetAllFusionList()
        {
            List<Items_Fusion_List_Info> list = new List<Items_Fusion_List_Info>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "GET_ItemFusion_All");
                while (resultDataReader.Read())
                {
                    Items_Fusion_List_Info item = new Items_Fusion_List_Info {
                        ID = (int) resultDataReader["ID"],
                        TemplateID = (int) resultDataReader["TemplateID"],
                        Show = (int) resultDataReader["Show"],
                        Real = (int) resultDataReader["Real"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GET_ItemFusion_All", exception);
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

        public GoldEquipTemplateLoadInfo[] GetAllGoldEquipTemplateLoad()
        {
            List<GoldEquipTemplateLoadInfo> list = new List<GoldEquipTemplateLoadInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_GoldEquipTemplateLoad_All");
                while (resultDataReader.Read())
                {
                    list.Add(this.InitGoldEquipTemplateLoad(resultDataReader));
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllGoldEquipTemplateLoad", exception);
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

        public ItemTemplateInfo[] GetAllGoods()
        {
            List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Items_All");
                while (resultDataReader.Read())
                {
                    list.Add(this.InitItemTemplateInfo(resultDataReader));
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

        public ItemTemplateInfo[] GetAllGoodsASC()
        {
            List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Items_All_ASC");
                while (resultDataReader.Read())
                {
                    list.Add(this.InitItemTemplateInfo(resultDataReader));
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

        public HotSpringRoomInfo[] GetAllHotSpringRooms()
        {
            List<HotSpringRoomInfo> list = new List<HotSpringRoomInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Get_HotSpring_Room");
                while (resultDataReader.Read())
                {
                    HotSpringRoomInfo item = new HotSpringRoomInfo {
                        roomID = (int) resultDataReader["roomID"],
                        roomNumber = (int) resultDataReader["roomNumber"],
                        roomName = resultDataReader["roomName"].ToString(),
                        roomPassword = (resultDataReader["roomPassword"] == DBNull.Value) ? null : ((string) resultDataReader["roomPassword"]),
                        effectiveTime = (int) resultDataReader["effectiveTime"],
                        curCount = (int) resultDataReader["curCount"],
                        playerID = (int) resultDataReader["playerID"],
                        playerName = (string) resultDataReader["playerName"],
                        startTime = (resultDataReader["startTime"] == DBNull.Value) ? DateTime.Now : ((DateTime) resultDataReader["startTime"]),
                        endTime = (resultDataReader["endTime"] == DBNull.Value) ? DateTime.Now.AddYears(1) : ((DateTime) resultDataReader["endTime"]),
                        roomIntroduction = (resultDataReader["roomIntroduction"] == DBNull.Value) ? "" : ((string) resultDataReader["roomIntroduction"]),
                        roomType = (int) resultDataReader["roomType"],
                        maxCount = (int) resultDataReader["maxCount"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllHotSpringRooms", exception);
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

        public ItemRecordTypeInfo[] GetAllItemRecordType()
        {
            List<ItemRecordTypeInfo> list = new List<ItemRecordTypeInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Item_Record_Type_All");
                while (resultDataReader.Read())
                {
                    list.Add(this.InitItemRecordType(resultDataReader));
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllItemRecordType:", exception);
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

        public ShopItemInfo[] GetALllShop()
        {
            List<ShopItemInfo> list = new List<ShopItemInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Shop_All");
                while (resultDataReader.Read())
                {
                    ShopItemInfo item = new ShopItemInfo {
                        ID = int.Parse(resultDataReader["ID"].ToString()),
                        ShopID = int.Parse(resultDataReader["ShopID"].ToString()),
                        GroupID = int.Parse(resultDataReader["GroupID"].ToString()),
                        TemplateID = int.Parse(resultDataReader["TemplateID"].ToString()),
                        BuyType = int.Parse(resultDataReader["BuyType"].ToString()),
                        Sort = 0,
                        IsVouch = int.Parse(resultDataReader["IsVouch"].ToString()),
                        Label = int.Parse(resultDataReader["Label"].ToString()),
                        Beat = decimal.Parse(resultDataReader["Beat"].ToString()),
                        AUnit = int.Parse(resultDataReader["AUnit"].ToString()),
                        APrice1 = int.Parse(resultDataReader["APrice1"].ToString()),
                        AValue1 = int.Parse(resultDataReader["AValue1"].ToString()),
                        APrice2 = int.Parse(resultDataReader["APrice2"].ToString()),
                        AValue2 = int.Parse(resultDataReader["AValue2"].ToString()),
                        APrice3 = int.Parse(resultDataReader["APrice3"].ToString()),
                        AValue3 = int.Parse(resultDataReader["AValue3"].ToString()),
                        BUnit = int.Parse(resultDataReader["BUnit"].ToString()),
                        BPrice1 = int.Parse(resultDataReader["BPrice1"].ToString()),
                        BValue1 = int.Parse(resultDataReader["BValue1"].ToString()),
                        BPrice2 = int.Parse(resultDataReader["BPrice2"].ToString()),
                        BValue2 = int.Parse(resultDataReader["BValue2"].ToString()),
                        BPrice3 = int.Parse(resultDataReader["BPrice3"].ToString()),
                        BValue3 = int.Parse(resultDataReader["BValue3"].ToString()),
                        CUnit = int.Parse(resultDataReader["CUnit"].ToString()),
                        CPrice1 = int.Parse(resultDataReader["CPrice1"].ToString()),
                        CValue1 = int.Parse(resultDataReader["CValue1"].ToString()),
                        CPrice2 = int.Parse(resultDataReader["CPrice2"].ToString()),
                        CValue2 = int.Parse(resultDataReader["CValue2"].ToString()),
                        CPrice3 = int.Parse(resultDataReader["CPrice3"].ToString()),
                        CValue3 = int.Parse(resultDataReader["CValue3"].ToString()),
                        IsContinue = bool.Parse(resultDataReader["IsContinue"].ToString()),
                        IsCheap = bool.Parse(resultDataReader["IsCheap"].ToString()),
                        LimitCount = int.Parse(resultDataReader["LimitCount"].ToString()),
                        StartDate = DateTime.Parse(resultDataReader["StartDate"].ToString()),
                        EndDate = DateTime.Parse(resultDataReader["EndDate"].ToString())
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

        public LuckstarActivityRankInfo[] GetAllLuckstarActivityRank()
        {
            List<LuckstarActivityRankInfo> list = new List<LuckstarActivityRankInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "[SP_Luckstar_Activity_Rank_All]");
                for (int i = 1; resultDataReader.Read(); i++)
                {
                    LuckstarActivityRankInfo item = new LuckstarActivityRankInfo {
                        rank = i,
                        UserID = (int) resultDataReader["UserID"],
                        useStarNum = (int) resultDataReader["useStarNum"],
                        isVip = (int) resultDataReader["isVip"],
                        nickName = (string) resultDataReader["nickName"]
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

        public MissionEnergyInfo[] GetAllMissionEnergyInfo()
        {
            List<MissionEnergyInfo> list = new List<MissionEnergyInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_MissionEnergyInfo_All");
                while (resultDataReader.Read())
                {
                    MissionEnergyInfo item = new MissionEnergyInfo {
                        Count = (int) resultDataReader["Count"],
                        Money = (int) resultDataReader["Money"],
                        Energy = (int) resultDataReader["Energy"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllMissionEnergyInfo", exception);
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

        public MissionInfo[] GetAllMissionInfo()
        {
            List<MissionInfo> list = new List<MissionInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Mission_Info_All");
                while (resultDataReader.Read())
                {
                    MissionInfo item = new MissionInfo {
                        Id = (int) resultDataReader["ID"],
                        Name = (resultDataReader["Name"] == null) ? "" : resultDataReader["Name"].ToString(),
                        TotalCount = (int) resultDataReader["TotalCount"],
                        TotalTurn = (int) resultDataReader["TotalTurn"],
                        Script = (resultDataReader["Script"] == null) ? "" : resultDataReader["Script"].ToString(),
                        Success = (resultDataReader["Success"] == null) ? "" : resultDataReader["Success"].ToString(),
                        Failure = (resultDataReader["Failure"] == null) ? "" : resultDataReader["Failure"].ToString(),
                        Description = (resultDataReader["Description"] == null) ? "" : resultDataReader["Description"].ToString(),
                        IncrementDelay = (int) resultDataReader["IncrementDelay"],
                        Delay = (int) resultDataReader["Delay"],
                        Title = (resultDataReader["Title"] == null) ? "" : resultDataReader["Title"].ToString(),
                        Param1 = (int) resultDataReader["Param1"],
                        Param2 = (int) resultDataReader["Param2"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllMissionInfo", exception);
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

        public NpcInfo[] GetAllNPCInfo()
        {
            List<NpcInfo> list = new List<NpcInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_NPC_Info_All");
                while (resultDataReader.Read())
                {
                    NpcInfo item = new NpcInfo {
                        ID = (int) resultDataReader["ID"],
                        Name = (resultDataReader["Name"] == null) ? "" : resultDataReader["Name"].ToString(),
                        Level = (int) resultDataReader["Level"],
                        Camp = (int) resultDataReader["Camp"],
                        Type = (int) resultDataReader["Type"],
                        Blood = (int) resultDataReader["Blood"],
                        X = (int) resultDataReader["X"],
                        Y = (int) resultDataReader["Y"],
                        Width = (int) resultDataReader["Width"],
                        Height = (int) resultDataReader["Height"],
                        MoveMin = (int) resultDataReader["MoveMin"],
                        MoveMax = (int) resultDataReader["MoveMax"],
                        BaseDamage = (int) resultDataReader["BaseDamage"],
                        BaseGuard = (int) resultDataReader["BaseGuard"],
                        Attack = (int) resultDataReader["Attack"],
                        Defence = (int) resultDataReader["Defence"],
                        Agility = (int) resultDataReader["Agility"],
                        Lucky = (int) resultDataReader["Lucky"],
                        ModelID = (resultDataReader["ModelID"] == null) ? "" : resultDataReader["ModelID"].ToString(),
                        ResourcesPath = (resultDataReader["ResourcesPath"] == null) ? "" : resultDataReader["ResourcesPath"].ToString(),
                        DropRate = (resultDataReader["DropRate"] == null) ? "" : resultDataReader["DropRate"].ToString(),
                        Experience = (int) resultDataReader["Experience"],
                        Delay = (int) resultDataReader["Delay"],
                        Immunity = (int) resultDataReader["Immunity"],
                        Alert = (int) resultDataReader["Alert"],
                        Range = (int) resultDataReader["Range"],
                        Preserve = (int) resultDataReader["Preserve"],
                        Script = (resultDataReader["Script"] == null) ? "" : resultDataReader["Script"].ToString(),
                        FireX = (int) resultDataReader["FireX"],
                        FireY = (int) resultDataReader["FireY"],
                        DropId = (int) resultDataReader["DropId"],
                        CurrentBallId = (int) resultDataReader["CurrentBallId"],
                        speed = (int) resultDataReader["speed"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllNPCInfo", exception);
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

        public PropInfo[] GetAllProp()
        {
            List<PropInfo> list = new List<PropInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Prop_All");
                while (resultDataReader.Read())
                {
                    PropInfo item = new PropInfo {
                        AffectArea = (int) resultDataReader["AffectArea"],
                        AffectTimes = (int) resultDataReader["AffectTimes"],
                        AttackTimes = (int) resultDataReader["AttackTimes"],
                        BoutTimes = (int) resultDataReader["BoutTimes"],
                        BuyGold = (int) resultDataReader["BuyGold"],
                        BuyMoney = (int) resultDataReader["BuyMoney"],
                        Category = (int) resultDataReader["Category"],
                        Delay = (int) resultDataReader["Delay"],
                        Description = resultDataReader["Description"].ToString(),
                        Icon = resultDataReader["Icon"].ToString(),
                        ID = (int) resultDataReader["ID"],
                        Name = resultDataReader["Name"].ToString(),
                        Parameter = (int) resultDataReader["Parameter"],
                        Pic = resultDataReader["Pic"].ToString(),
                        Property1 = (int) resultDataReader["Property1"],
                        Property2 = (int) resultDataReader["Property2"],
                        Property3 = (int) resultDataReader["Property3"],
                        Random = (int) resultDataReader["Random"],
                        Script = resultDataReader["Script"].ToString()
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

        public QQtipsMessagesInfo[] GetAllQQtipsMessagesLoad()
        {
            List<QQtipsMessagesInfo> list = new List<QQtipsMessagesInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_QQtipsMessages_All");
                while (resultDataReader.Read())
                {
                    list.Add(this.InitQQtipsMessagesLoad(resultDataReader));
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllQQtipsMessagesLoad", exception);
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

        public QuestInfo[] GetALlQuest()
        {
            List<QuestInfo> list = new List<QuestInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Quest_All");
                while (resultDataReader.Read())
                {
                    list.Add(this.InitQuest(resultDataReader));
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

        public QuestConditionInfo[] GetAllQuestCondiction()
        {
            List<QuestConditionInfo> list = new List<QuestConditionInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Quest_Condiction_All");
                while (resultDataReader.Read())
                {
                    list.Add(this.InitQuestCondiction(resultDataReader));
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

        public QuestAwardInfo[] GetAllQuestGoods()
        {
            List<QuestAwardInfo> list = new List<QuestAwardInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Quest_Goods_All");
                while (resultDataReader.Read())
                {
                    list.Add(this.InitQuestGoods(resultDataReader));
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

        public QuestRateInfo[] GetAllQuestRate()
        {
            List<QuestRateInfo> list = new List<QuestRateInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Quest_Rate_All");
                while (resultDataReader.Read())
                {
                    list.Add(this.InitQuestRate(resultDataReader));
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

        public List<RefineryInfo> GetAllRefineryInfo()
        {
            List<RefineryInfo> list = new List<RefineryInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Item_Refinery_All");
                while (resultDataReader.Read())
                {
                    RefineryInfo item = new RefineryInfo {
                        RefineryID = (int) resultDataReader["RefineryID"]
                    };
                    item.m_Equip.Add((int) resultDataReader["Equip1"]);
                    item.m_Equip.Add((int) resultDataReader["Equip2"]);
                    item.m_Equip.Add((int) resultDataReader["Equip3"]);
                    item.m_Equip.Add((int) resultDataReader["Equip4"]);
                    item.Item1 = (int) resultDataReader["Item1"];
                    item.Item2 = (int) resultDataReader["Item2"];
                    item.Item3 = (int) resultDataReader["Item3"];
                    item.Item1Count = (int) resultDataReader["Item1Count"];
                    item.Item2Count = (int) resultDataReader["Item2Count"];
                    item.Item3Count = (int) resultDataReader["Item3Count"];
                    item.m_Reward.Add((int) resultDataReader["Material1"]);
                    item.m_Reward.Add((int) resultDataReader["Operate1"]);
                    item.m_Reward.Add((int) resultDataReader["Reward1"]);
                    item.m_Reward.Add((int) resultDataReader["Material2"]);
                    item.m_Reward.Add((int) resultDataReader["Operate2"]);
                    item.m_Reward.Add((int) resultDataReader["Reward2"]);
                    item.m_Reward.Add((int) resultDataReader["Material3"]);
                    item.m_Reward.Add((int) resultDataReader["Operate3"]);
                    item.m_Reward.Add((int) resultDataReader["Reward3"]);
                    item.m_Reward.Add((int) resultDataReader["Material4"]);
                    item.m_Reward.Add((int) resultDataReader["Operate4"]);
                    item.m_Reward.Add((int) resultDataReader["Reward4"]);
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllRefineryInfo", exception);
                }
            }
            finally
            {
                if ((resultDataReader != null) && resultDataReader.IsClosed)
                {
                    resultDataReader.Close();
                }
            }
            return list;
        }

        public StrengthenInfo[] GetAllRefineryStrengthen()
        {
            List<StrengthenInfo> list = new List<StrengthenInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Item_Refinery_Strengthen_All");
                while (resultDataReader.Read())
                {
                    StrengthenInfo item = new StrengthenInfo {
                        StrengthenLevel = (int) resultDataReader["StrengthenLevel"],
                        Rock = (int) resultDataReader["Rock"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllRefineryStrengthen", exception);
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

        public RuneTemplateInfo[] GetAllRuneTemplate()
        {
            List<RuneTemplateInfo> list = new List<RuneTemplateInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_RuneTemplate_All");
                while (resultDataReader.Read())
                {
                    RuneTemplateInfo item = new RuneTemplateInfo {
                        TemplateID = (int) resultDataReader["TemplateID"],
                        NextTemplateID = (int) resultDataReader["NextTemplateID"],
                        Name = (string) resultDataReader["Name"],
                        BaseLevel = (int) resultDataReader["BaseLevel"],
                        MaxLevel = (int) resultDataReader["MaxLevel"],
                        Type1 = (int) resultDataReader["Type1"],
                        Attribute1 = (string) resultDataReader["Attribute1"],
                        Turn1 = (int) resultDataReader["Turn1"],
                        Rate1 = (int) resultDataReader["Rate1"],
                        Type2 = (int) resultDataReader["Type2"],
                        Attribute2 = (string) resultDataReader["Attribute2"],
                        Turn2 = (int) resultDataReader["Turn2"],
                        Rate2 = (int) resultDataReader["Rate2"],
                        Type3 = (int) resultDataReader["Type3"],
                        Attribute3 = (string) resultDataReader["Attribute3"],
                        Turn3 = (int) resultDataReader["Turn3"],
                        Rate3 = (int) resultDataReader["Rate3"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetRuneTemplateInfo", exception);
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

        public SearchGoodsTempInfo[] GetAllSearchGoodsTemp()
        {
            List<SearchGoodsTempInfo> list = new List<SearchGoodsTempInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_SearchGoodsTemp_All");
                while (resultDataReader.Read())
                {
                    SearchGoodsTempInfo item = new SearchGoodsTempInfo {
                        StarID = (int) resultDataReader["StarID"],
                        NeedMoney = (int) resultDataReader["NeedMoney"],
                        DestinationReward = (int) resultDataReader["DestinationReward"],
                        VIPLevel = (int) resultDataReader["VIPLevel"],
                        ExtractNumber = (resultDataReader["ExtractNumber"] == null) ? "" : resultDataReader["ExtractNumber"].ToString()
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllDaily", exception);
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

        public ShopGoodsShowListInfo[] GetAllShopGoodsShowList()
        {
            List<ShopGoodsShowListInfo> list = new List<ShopGoodsShowListInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_ShopGoodsShowList_All");
                while (resultDataReader.Read())
                {
                    list.Add(this.InitShopGoodsShowListInfo(resultDataReader));
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

        public ShopGoXuInfo[] GetAllShopGoXu()
        {
            List<ShopGoXuInfo> list = new List<ShopGoXuInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_ShopGoXu_All");
                while (resultDataReader.Read())
                {
                    list.Add(this.InitShopGoXuInfo(resultDataReader));
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

        public StrengthenInfo[] GetAllStrengthen()
        {
            List<StrengthenInfo> list = new List<StrengthenInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Item_Strengthen_All");
                while (resultDataReader.Read())
                {
                    StrengthenInfo item = new StrengthenInfo {
                        StrengthenLevel = (int) resultDataReader["StrengthenLevel"],
                        Random = (int) resultDataReader["Random"],
                        Rock = (int) resultDataReader["Rock"],
                        Rock1 = (int) resultDataReader["Rock1"],
                        Rock2 = (int) resultDataReader["Rock2"],
                        Rock3 = (int) resultDataReader["Rock3"],
                        StoneLevelMin = (int) resultDataReader["StoneLevelMin"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllStrengthen", exception);
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

        public StrengThenExpInfo[] GetAllStrengThenExp()
        {
            List<StrengThenExpInfo> list = new List<StrengThenExpInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_StrengThenExp_All");
                while (resultDataReader.Read())
                {
                    StrengThenExpInfo item = new StrengThenExpInfo {
                        ID = (int) resultDataReader["ID"],
                        Level = (int) resultDataReader["Level"],
                        Exp = (int) resultDataReader["Exp"],
                        NecklaceStrengthExp = (int) resultDataReader["NecklaceStrengthExp"],
                        NecklaceStrengthPlus = (int) resultDataReader["NecklaceStrengthPlus"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetStrengThenExpInfo", exception);
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

        public StrengthenGoodsInfo[] GetAllStrengthenGoodsInfo()
        {
            List<StrengthenGoodsInfo> list = new List<StrengthenGoodsInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Item_StrengthenGoodsInfo_All");
                while (resultDataReader.Read())
                {
                    StrengthenGoodsInfo item = new StrengthenGoodsInfo {
                        ID = (int) resultDataReader["ID"],
                        Level = (int) resultDataReader["Level"],
                        CurrentEquip = (int) resultDataReader["CurrentEquip"],
                        GainEquip = (int) resultDataReader["GainEquip"],
                        OrginEquip = (int) resultDataReader["OrginEquip"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllStrengthenGoodsInfo", exception);
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

        public LoadUserBoxInfo[] GetAllTimeBoxAward()
        {
            List<LoadUserBoxInfo> list = new List<LoadUserBoxInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_TimeBox_Award_All");
                while (resultDataReader.Read())
                {
                    LoadUserBoxInfo item = new LoadUserBoxInfo {
                        ID = (int) resultDataReader["ID"],
                        Type = (int) resultDataReader["Type"],
                        Level = (int) resultDataReader["Level"],
                        Condition = (int) resultDataReader["Condition"],
                        TemplateID = (int) resultDataReader["TemplateID"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllDaily", exception);
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

        public UserBoxInfo[] GetAllUserBox()
        {
            List<UserBoxInfo> list = new List<UserBoxInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_TimeBox_Award_All");
                while (resultDataReader.Read())
                {
                    UserBoxInfo item = new UserBoxInfo {
                        ID = (int) resultDataReader["ID"],
                        Type = (int) resultDataReader["Type"],
                        Level = (int) resultDataReader["Level"],
                        Condition = (int) resultDataReader["Condition"],
                        TemplateID = (int) resultDataReader["TemplateID"]
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllUserBox", exception);
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

        public string GetASSInfoSingle(int UserID)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", UserID) };
                base.db.GetReader(ref resultDataReader, "SP_ASSInfo_Single", sqlParameters);
                if (resultDataReader.Read())
                {
                    return resultDataReader["IDNumber"].ToString();
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetASSInfoSingle", exception);
                }
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return "";
        }

        public DailyLogListInfo GetDailyLogListSingle(int UserID)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", UserID) };
                base.db.GetReader(ref resultDataReader, "SP_DailyLogList_Single", sqlParameters);
                if (resultDataReader.Read())
                {
                    return new DailyLogListInfo { ID = (int) resultDataReader["ID"], UserID = (int) resultDataReader["UserID"], UserAwardLog = (int) resultDataReader["UserAwardLog"], DayLog = (string) resultDataReader["DayLog"], LastDate = (DateTime) resultDataReader["LastDate"] };
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("DailyLogList", exception);
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

        public DiceLevelAwardInfo[] GetDiceLevelAwardInfos()
        {
            List<DiceLevelAwardInfo> list = new List<DiceLevelAwardInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_DiceLevelAward_All");
                while (resultDataReader.Read())
                {
                    DiceLevelAwardInfo item = new DiceLevelAwardInfo {
                        ID = (int) resultDataReader["ID"],
                        DiceLevel = (int) resultDataReader["DiceLevel"],
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
                    BaseBussiness.log.Error("GetAllDiceLevelAward", exception);
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

        public EventAwardInfo[] GetEventAwardInfos()
        {
            List<EventAwardInfo> list = new List<EventAwardInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_EventAwardItem_All");
                while (resultDataReader.Read())
                {
                    EventAwardInfo item = new EventAwardInfo {
                        ID = (int) resultDataReader["ID"],
                        ActivityType = (int) resultDataReader["ActivityType"],
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
                    BaseBussiness.log.Error("GetAllEventAward", exception);
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

        public ItemTemplateInfo[] GetFusionType()
        {
            List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Items_FusionType");
                while (resultDataReader.Read())
                {
                    list.Add(this.InitItemTemplateInfo(resultDataReader));
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

        public ItemBoxInfo[] GetItemBoxInfos()
        {
            List<ItemBoxInfo> list = new List<ItemBoxInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_ItemsBox_All");
                while (resultDataReader.Read())
                {
                    list.Add(this.InitItemBoxInfo(resultDataReader));
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init@Shop_Goods_Box：" + exception);
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

        public ItemTemplateInfo[] GetSingleCategory(int CategoryID)
        {
            List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@CategoryID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = CategoryID;
                base.db.GetReader(ref resultDataReader, "SP_Items_Category_Single", sqlParameters);
                while (resultDataReader.Read())
                {
                    list.Add(this.InitItemTemplateInfo(resultDataReader));
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

        public ItemTemplateInfo GetSingleGoods(int goodsID)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = goodsID;
                base.db.GetReader(ref resultDataReader, "SP_Items_Single", sqlParameters);
                if (resultDataReader.Read())
                {
                    return this.InitItemTemplateInfo(resultDataReader);
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

        public ItemBoxInfo[] GetSingleItemsBox(int DataID)
        {
            List<ItemBoxInfo> list = new List<ItemBoxInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = DataID;
                base.db.GetReader(ref resultDataReader, "SP_ItemsBox_Single", sqlParameters);
                while (resultDataReader.Read())
                {
                    list.Add(this.InitItemBoxInfo(resultDataReader));
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

        public QuestInfo GetSingleQuest(int questID)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@QuestID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = questID;
                base.db.GetReader(ref resultDataReader, "SP_Quest_Single", sqlParameters);
                if (resultDataReader.Read())
                {
                    return this.InitQuest(resultDataReader);
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

        public AchievementInfo InitAchievement(SqlDataReader reader)
        {
            return new AchievementInfo { ID = (int) reader["ID"], PlaceID = (int) reader["PlaceID"], Title = (reader["Title"] == null) ? "" : reader["Title"].ToString(), Detail = (reader["Detail"] == null) ? "" : reader["Detail"].ToString(), NeedMinLevel = (int) reader["NeedMinLevel"], NeedMaxLevel = (int) reader["NeedMaxLevel"], PreAchievementID = (reader["PreAchievementID"] == null) ? "" : reader["PreAchievementID"].ToString(), IsOther = (int) reader["IsOther"], AchievementType = (int) reader["AchievementType"], CanHide = (bool) reader["CanHide"], StartDate = (DateTime) reader["StartDate"], EndDate = (DateTime) reader["EndDate"], AchievementPoint = (int) reader["AchievementPoint"], IsActive = (int) reader["IsActive"], PicID = (int) reader["PicID"], IsShare = (bool) reader["IsShare"] };
        }

        public AchievementConditionInfo InitAchievementCondition(SqlDataReader reader)
        {
            return new AchievementConditionInfo { AchievementID = (int) reader["AchievementID"], CondictionID = (int) reader["CondictionID"], CondictionType = (int) reader["CondictionType"], Condiction_Para1 = (reader["Condiction_Para1"] == null) ? "" : reader["Condiction_Para1"].ToString(), Condiction_Para2 = (int) reader["Condiction_Para2"] };
        }

        public AchievementDataInfo InitAchievementData(SqlDataReader reader)
        {
            return new AchievementDataInfo { UserID = (int) reader["UserID"], AchievementID = (int) reader["AchievementID"], IsComplete = (bool) reader["IsComplete"], CompletedDate = (DateTime) reader["CompletedDate"] };
        }

        public AchievementRewardInfo InitAchievementReward(SqlDataReader reader)
        {
            return new AchievementRewardInfo { AchievementID = (int) reader["AchievementID"], RewardType = (int) reader["RewardType"], RewardPara = (reader["RewardPara"] == null) ? "" : reader["RewardPara"].ToString(), RewardValueId = (int) reader["RewardValueId"], RewardCount = (int) reader["RewardCount"] };
        }

        public DropCondiction InitDropCondiction(SqlDataReader reader)
        {
            return new DropCondiction { DropId = (int) reader["DropID"], CondictionType = (int) reader["CondictionType"], Para1 = (string) reader["Para1"], Para2 = (string) reader["Para2"] };
        }

        public DropItem InitDropItem(SqlDataReader reader)
        {
            return new DropItem { Id = (int) reader["Id"], DropId = (int) reader["DropId"], ItemId = (int) reader["ItemId"], ValueDate = (int) reader["ValueDate"], IsBind = (bool) reader["IsBind"], Random = (int) reader["Random"], BeginData = (int) reader["BeginData"], EndData = (int) reader["EndData"], IsLogs = (bool) reader["IsLogs"], IsTips = (bool) reader["IsTips"] };
        }

        public EdictumInfo InitEdictum(SqlDataReader reader)
        {
            return new EdictumInfo { ID = (int) reader["ID"], Title = (reader["Title"] == null) ? "" : reader["Title"].ToString(), BeginDate = (DateTime) reader["BeginDate"], BeginTime = (DateTime) reader["BeginTime"], EndDate = (DateTime) reader["EndDate"], EndTime = (DateTime) reader["EndTime"], Text = (reader["Text"] == null) ? "" : reader["Text"].ToString(), IsExist = (bool) reader["IsExist"] };
        }

        public GoldEquipTemplateLoadInfo InitGoldEquipTemplateLoad(SqlDataReader reader)
        {
            return new GoldEquipTemplateLoadInfo { ID = (int) reader["ID"], OldTemplateId = (int) reader["OldTemplateId"], NewTemplateId = (int) reader["NewTemplateId"], CategoryID = (int) reader["CategoryID"], Strengthen = (int) reader["Strengthen"], Attack = (int) reader["Attack"], Defence = (int) reader["Defence"], Agility = (int) reader["Agility"], Luck = (int) reader["Luck"], Damage = (int) reader["Damage"], Guard = (int) reader["Guard"], Boold = (int) reader["Boold"], BlessID = (int) reader["BlessID"], Pic = (reader["pic"] == null) ? "" : reader["pic"].ToString() };
        }

        public ItemBoxInfo InitItemBoxInfo(SqlDataReader reader)
        {
            return new ItemBoxInfo { 
                Id = (int) reader["id"], DataId = (int) reader["DataId"], TemplateId = (int) reader["TemplateId"], IsSelect = (bool) reader["IsSelect"], IsBind = (bool) reader["IsBind"], ItemValid = (int) reader["ItemValid"], ItemCount = (int) reader["ItemCount"], StrengthenLevel = (int) reader["StrengthenLevel"], AttackCompose = (int) reader["AttackCompose"], DefendCompose = (int) reader["DefendCompose"], AgilityCompose = (int) reader["AgilityCompose"], LuckCompose = (int) reader["LuckCompose"], MagicAttack = (int) reader["MagicAttack"], MagicDefence = (int) reader["MagicDefence"], Random = (int) reader["Random"], IsTips = (int) reader["IsTips"], 
                IsLogs = (bool) reader["IsLogs"]
             };
        }

        public ItemRecordTypeInfo InitItemRecordType(SqlDataReader reader)
        {
            return new ItemRecordTypeInfo { RecordID = (int) reader["RecordID"], Name = (reader["Name"] == null) ? "" : reader["Name"].ToString(), Description = (reader["Description"] == null) ? "" : reader["Description"].ToString() };
        }

        public ItemTemplateInfo InitItemTemplateInfo(SqlDataReader reader)
        {
            return new ItemTemplateInfo { 
                AddTime = reader["AddTime"].ToString(), Agility = (int) reader["Agility"], Attack = (int) reader["Attack"], CanDelete = (bool) reader["CanDelete"], CanDrop = (bool) reader["CanDrop"], CanEquip = (bool) reader["CanEquip"], CanUse = (bool) reader["CanUse"], CategoryID = (int) reader["CategoryID"], Colors = reader["Colors"].ToString(), Defence = (int) reader["Defence"], Description = reader["Description"].ToString(), Level = (int) reader["Level"], Luck = (int) reader["Luck"], MaxCount = (int) reader["MaxCount"], Name = reader["Name"].ToString(), NeedSex = (int) reader["NeedSex"], 
                Pic = reader["Pic"].ToString(), Data = (reader["Data"] == null) ? "" : reader["Data"].ToString(), Property1 = (int) reader["Property1"], Property2 = (int) reader["Property2"], Property3 = (int) reader["Property3"], Property4 = (int) reader["Property4"], Property5 = (int) reader["Property5"], Property6 = (int) reader["Property6"], Property7 = (int) reader["Property7"], Property8 = (int) reader["Property8"], Quality = (int) reader["Quality"], Script = reader["Script"].ToString(), TemplateID = (int) reader["TemplateID"], CanCompose = (bool) reader["CanCompose"], CanStrengthen = (bool) reader["CanStrengthen"], NeedLevel = (int) reader["NeedLevel"], 
                BindType = (int) reader["BindType"], FusionType = (int) reader["FusionType"], FusionRate = (int) reader["FusionRate"], FusionNeedRate = (int) reader["FusionNeedRate"], Hole = (reader["Hole"] == null) ? "" : reader["Hole"].ToString(), RefineryLevel = (int) reader["RefineryLevel"], ReclaimValue = (int) reader["ReclaimValue"], ReclaimType = (int) reader["ReclaimType"], CanRecycle = (int) reader["CanRecycle"], IsDirty = false
             };
        }

        public QQtipsMessagesInfo InitQQtipsMessagesLoad(SqlDataReader reader)
        {
            return new QQtipsMessagesInfo { ID = (int) reader["ID"], title = (reader["title"] == null) ? "QQTips" : reader["title"].ToString(), content = (reader["content"] == null) ? "Th\x00f4ng b\x00e1o, gợi \x00fd hệ thống" : reader["content"].ToString(), maxLevel = (int) reader["maxLevel"], minLevel = (int) reader["minLevel"], outInType = (int) reader["outInType"], moduleType = (int) reader["moduleType"], inItemID = (int) reader["inItemID"], url = (reader["url"] == null) ? "http://gunny.zing.vn" : reader["url"].ToString() };
        }

        public QuestInfo InitQuest(SqlDataReader reader)
        {
            return new QuestInfo { 
                ID = (int) reader["ID"], QuestID = (int) reader["QuestID"], Title = (reader["Title"] == null) ? "" : reader["Title"].ToString(), Detail = (reader["Detail"] == null) ? "" : reader["Detail"].ToString(), Objective = (reader["Objective"] == null) ? "" : reader["Objective"].ToString(), NeedMinLevel = (int) reader["NeedMinLevel"], NeedMaxLevel = (int) reader["NeedMaxLevel"], PreQuestID = (reader["PreQuestID"] == null) ? "" : reader["PreQuestID"].ToString(), NextQuestID = (reader["NextQuestID"] == null) ? "" : reader["NextQuestID"].ToString(), IsOther = (int) reader["IsOther"], CanRepeat = (bool) reader["CanRepeat"], RepeatInterval = (int) reader["RepeatInterval"], RepeatMax = (int) reader["RepeatMax"], RewardGP = (int) reader["RewardGP"], RewardGold = (int) reader["RewardGold"], RewardGiftToken = (int) reader["RewardGiftToken"], 
                RewardOffer = (int) reader["RewardOffer"], RewardRiches = (int) reader["RewardRiches"], RewardBuffID = (int) reader["RewardBuffID"], RewardBuffDate = (int) reader["RewardBuffDate"], RewardMoney = (int) reader["RewardMoney"], Rands = (decimal) reader["Rands"], RandDouble = (int) reader["RandDouble"], TimeMode = (bool) reader["TimeMode"], StartDate = (DateTime) reader["StartDate"], EndDate = (DateTime) reader["EndDate"], MapID = (int) reader["MapID"]
             };
        }

        public QuestConditionInfo InitQuestCondiction(SqlDataReader reader)
        {
            return new QuestConditionInfo { QuestID = (int) reader["QuestID"], CondictionID = (int) reader["CondictionID"], CondictionTitle = (reader["CondictionTitle"] == null) ? "" : reader["CondictionTitle"].ToString(), CondictionType = (int) reader["CondictionType"], Para1 = (int) reader["Para1"], Para2 = (int) reader["Para2"], isOpitional = (bool) reader["isOpitional"] };
        }

        public QuestAwardInfo InitQuestGoods(SqlDataReader reader)
        {
            return new QuestAwardInfo { QuestID = (int) reader["QuestID"], RewardItemID = (int) reader["RewardItemID"], IsSelect = (bool) reader["IsSelect"], RewardItemValid = (int) reader["RewardItemValid"], RewardItemCount = (int) reader["RewardItemCount"], StrengthenLevel = (int) reader["StrengthenLevel"], AttackCompose = (int) reader["AttackCompose"], DefendCompose = (int) reader["DefendCompose"], AgilityCompose = (int) reader["AgilityCompose"], LuckCompose = (int) reader["LuckCompose"], IsCount = (bool) reader["IsCount"] };
        }

        public QuestRateInfo InitQuestRate(SqlDataReader reader)
        {
            return new QuestRateInfo { BindMoneyRate = (reader["BindMoneyRate"] == null) ? "" : reader["BindMoneyRate"].ToString(), ExpRate = (reader["ExpRate"] == null) ? "" : reader["ExpRate"].ToString(), GoldRate = (reader["GoldRate"] == null) ? "" : reader["GoldRate"].ToString(), ExploitRate = (reader["ExploitRate"] == null) ? "" : reader["ExploitRate"].ToString(), CanOneKeyFinishTime = (int) reader["CanOneKeyFinishTime"] };
        }

        public ShopGoodsShowListInfo InitShopGoodsShowListInfo(SqlDataReader reader)
        {
            return new ShopGoodsShowListInfo { Type = (int) reader["Type"], ShopId = (int) reader["ShopId"] };
        }

        public ShopGoXuInfo InitShopGoXuInfo(SqlDataReader reader)
        {
            return new ShopGoXuInfo { TemplateID = (int) reader["TemplateID"], Price1 = (int) reader["Price1"], Price2 = (int) reader["Price2"], Price3 = (int) reader["Price3"] };
        }

        public bool UpdateDailyLogList(DailyLogListInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", info.UserID), new SqlParameter("@UserAwardLog", info.UserAwardLog), new SqlParameter("@DayLog", info.DayLog), new SqlParameter("@LastDate", info.LastDate.ToString()), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[4].Direction = ParameterDirection.ReturnValue;
                flag = base.db.RunProcedure("SP_DailyLogList_Update", sqlParameters);
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("User_Update_BoxProgression", exception);
                }
            }
            return flag;
        }

        public bool UpdatePlayerInfoHistory(PlayerInfoHistory info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@UserID", info.UserID), new SqlParameter("@LastQuestsTime", info.LastQuestsTime), new SqlParameter("@LastTreasureTime", info.LastTreasureTime), new SqlParameter("@OutPut", SqlDbType.Int) };
                sqlParameters[3].Direction = ParameterDirection.Output;
                base.db.RunProcedure("SP_User_Update_History", sqlParameters);
                flag = ((int) sqlParameters[6].Value) == 1;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("User_Update_BoxProgression", exception);
                }
            }
            return flag;
        }
    }
}

