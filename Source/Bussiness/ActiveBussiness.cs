namespace Bussiness
{
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    public class ActiveBussiness : BaseBussiness
    {
        public bool AddActiveNumber(string AwardID, int ActiveID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@AwardID", AwardID), new SqlParameter("@ActiveID", ActiveID), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[2].Direction = ParameterDirection.ReturnValue;
                flag = base.db.RunProcedure("SP_Active_Number_Add", sqlParameters);
                flag = ((int) sqlParameters[2].Value) == 0;
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

        public ActiveInfo[] GetAllActives()
        {
            List<ActiveInfo> list = new List<ActiveInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                base.db.GetReader(ref resultDataReader, "SP_Active_All");
                while (resultDataReader.Read())
                {
                    list.Add(this.InitActiveInfo(resultDataReader));
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

        public ActiveConvertItemInfo[] GetSingleActiveConvertItems(int activeID)
        {
            List<ActiveConvertItemInfo> list = new List<ActiveConvertItemInfo>();
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = activeID;
                base.db.GetReader(ref resultDataReader, "SP_Active_Convert_Item_Info_Single", sqlParameters);
                while (resultDataReader.Read())
                {
                    list.Add(this.InitActiveConvertItemInfo(resultDataReader));
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

        public ActiveInfo GetSingleActives(int activeID)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ID", SqlDbType.Int, 4) };
                sqlParameters[0].Value = activeID;
                base.db.GetReader(ref resultDataReader, "SP_Active_Single", sqlParameters);
                if (resultDataReader.Read())
                {
                    return this.InitActiveInfo(resultDataReader);
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

        public ActiveConvertItemInfo InitActiveConvertItemInfo(SqlDataReader reader)
        {
            return new ActiveConvertItemInfo { ID = (int) reader["ID"], ActiveID = (int) reader["ActiveID"], TemplateID = (int) reader["TemplateID"], ItemType = (int) reader["ItemType"], ItemCount = (int) reader["ItemCount"], LimitValue = (int) reader["LimitValue"], IsBind = (bool) reader["IsBind"], ValidDate = (int) reader["ValidDate"] };
        }

        public ActiveInfo InitActiveInfo(SqlDataReader reader)
        {
            ActiveInfo info = new ActiveInfo {
                ActiveID = (int) reader["ActiveID"],
                Description = (reader["Description"] == null) ? "" : reader["Description"].ToString(),
                Content = (reader["Content"] == null) ? "" : reader["Content"].ToString(),
                AwardContent = (reader["AwardContent"] == null) ? "" : reader["AwardContent"].ToString(),
                HasKey = (int) reader["HasKey"]
            };
            if (!string.IsNullOrEmpty(reader["EndDate"].ToString()))
            {
                info.EndDate = new DateTime?((DateTime) reader["EndDate"]);
            }
            info.IsOnly = (int) reader["IsOnly"];
            info.StartDate = (DateTime) reader["StartDate"];
            info.Title = reader["Title"].ToString();
            info.Type = (int) reader["Type"];
            info.ActiveType = (int) reader["ActiveType"];
            info.ActionTimeContent = (reader["ActionTimeContent"] == null) ? "" : reader["ActionTimeContent"].ToString();
            info.IsAdvance = (bool) reader["IsAdvance"];
            info.GoodsExchangeTypes = (reader["GoodsExchangeTypes"] == null) ? "" : reader["GoodsExchangeTypes"].ToString();
            info.GoodsExchangeNum = (reader["GoodsExchangeNum"] == null) ? "" : reader["GoodsExchangeNum"].ToString();
            info.limitType = (reader["limitType"] == null) ? "" : reader["limitType"].ToString();
            info.limitValue = (reader["limitValue"] == null) ? "" : reader["limitValue"].ToString();
            info.IsShow = (bool) reader["IsShow"];
            info.IconID = (int) reader["IconID"];
            return info;
        }

        public int PullDown(int activeID, string awardID, int userID, ref string msg)
        {
            int num = 1;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@ActiveID", activeID), new SqlParameter("@AwardID", awardID), new SqlParameter("@UserID", userID), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[3].Direction = ParameterDirection.ReturnValue;
                if (!base.db.RunProcedure("SP_Active_PullDown", sqlParameters))
                {
                    return num;
                }
                num = (int) sqlParameters[3].Value;
                switch (num)
                {
                    case 0:
                        msg = "ActiveBussiness.Msg0";
                        return num;

                    case 1:
                        msg = "ActiveBussiness.Msg1";
                        return num;

                    case 2:
                        msg = "ActiveBussiness.Msg2";
                        return num;

                    case 3:
                        msg = "ActiveBussiness.Msg3";
                        return num;

                    case 4:
                        msg = "ActiveBussiness.Msg4";
                        return num;

                    case 5:
                        msg = "ActiveBussiness.Msg5";
                        return num;

                    case 6:
                        msg = "ActiveBussiness.Msg6";
                        return num;

                    case 7:
                        msg = "ActiveBussiness.Msg7";
                        return num;

                    case 8:
                        msg = "ActiveBussiness.Msg8";
                        return num;
                }
                msg = "ActiveBussiness.Msg9";
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
    }
}

