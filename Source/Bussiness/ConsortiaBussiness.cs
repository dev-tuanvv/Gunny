using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace Bussiness
{
	public class ConsortiaBussiness : BaseBussiness
	{
		public bool AddAndUpdateConsortiaEuqipControl(ConsortiaEquipControlInfo info, int userID, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", info.ConsortiaID),
					new SqlParameter("@Level", info.Level),
					new SqlParameter("@Type", info.Type),
					new SqlParameter("@Riches", info.Riches),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[5].Direction = ParameterDirection.ReturnValue;
				flag = this.db.RunProcedure("SP_Consortia_Equip_Control_Add", sqlParameters);
				int num = (int)sqlParameters[2].Value;
				flag = (num == 0);
				int num2 = num;
				if (num2 == 2)
				{
					msg = "ConsortiaBussiness.AddAndUpdateConsortiaEuqipControl.Msg2";
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
		public bool ConsortiaRichRemove(int consortiID, ref int riches)
		{
			bool flag = false;
			try
			{
				SqlParameter[] array = new SqlParameter[3];
				array[0] = new SqlParameter("@ConsortiaID", consortiID);
				array[1] = new SqlParameter("@Riches", SqlDbType.Int);
				SqlParameter[] SqlParameters = array;
				SqlParameters[1].Direction = ParameterDirection.InputOutput;
				SqlParameters[1].Value = riches;
				SqlParameters[2] = new SqlParameter("@Result", SqlDbType.Int);
				SqlParameters[2].Direction = ParameterDirection.ReturnValue;
				flag = this.db.RunProcedure("SP_Consortia_Riches_Remove", SqlParameters);
				riches = (int)SqlParameters[1].Value;
				flag = ((int)SqlParameters[2].Value == 0);
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Consortia_Riches_Remove", ex);
				}
			}
			return flag;
		}
		public ConsortiaBossConfigInfo[] GetConsortiaBossConfigAll()
		{
			List<ConsortiaBossConfigInfo> list = new List<ConsortiaBossConfigInfo>();
			SqlDataReader ResultDataReader = null;
			try
			{
				this.db.GetReader(ref ResultDataReader, "SP_Consortia_Boss_Config_All");
				while (ResultDataReader.Read())
				{
					list.Add(new ConsortiaBossConfigInfo
					{
						Level = (int)ResultDataReader["Level"],
						NpcID = (int)ResultDataReader["NpcID"],
						MissionID = (int)ResultDataReader["MissionID"],
						AwardID = (int)ResultDataReader["AwardID"],
						CostRich = (int)ResultDataReader["CostRich"],
						ProlongRich = (int)ResultDataReader["ProlongRich"],
						BossLevel = (int)ResultDataReader["BossLevel"]
					});
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
		public ConsortiaBuffTempInfo[] GetAllConsortiaBuffTemp()
		{
			List<ConsortiaBuffTempInfo> list = new List<ConsortiaBuffTempInfo>();
			SqlDataReader ResultDataReader = null;
			try
			{
				this.db.GetReader(ref ResultDataReader, "SP_Consortia_Buff_Temp_All");
				while (ResultDataReader.Read())
				{
					list.Add(new ConsortiaBuffTempInfo
					{
						id = (int)ResultDataReader["id"],
						name = (string)ResultDataReader["name"],
						descript = (string)ResultDataReader["descript"],
						type = (int)ResultDataReader["type"],
						level = (int)ResultDataReader["level"],
						value = (int)ResultDataReader["value"],
						riches = (int)ResultDataReader["riches"],
						metal = (int)ResultDataReader["metal"],
						pic = (int)ResultDataReader["pic"],
						group = (int)ResultDataReader["group"]
					});
				}
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllConsortiaBuffTemp", ex);
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
		public bool AddConsortia(ConsortiaInfo info, ref string msg, ref ConsortiaDutyInfo dutyInfo)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[23];
				sqlParameters[0] = new SqlParameter("@ConsortiaID", info.ConsortiaID);
				sqlParameters[0].Direction = ParameterDirection.InputOutput;
				sqlParameters[1] = new SqlParameter("@BuildDate", info.BuildDate);
				sqlParameters[2] = new SqlParameter("@CelebCount", info.CelebCount);
				sqlParameters[3] = new SqlParameter("@ChairmanID", info.ChairmanID);
				sqlParameters[4] = new SqlParameter("@ChairmanName", (info.ChairmanName == null) ? "" : info.ChairmanName);
				sqlParameters[5] = new SqlParameter("@ConsortiaName", (info.ConsortiaName == null) ? "" : info.ConsortiaName);
				sqlParameters[6] = new SqlParameter("@CreatorID", info.CreatorID);
				sqlParameters[7] = new SqlParameter("@CreatorName", (info.CreatorName == null) ? "" : info.CreatorName);
				sqlParameters[8] = new SqlParameter("@Description", info.Description);
				sqlParameters[9] = new SqlParameter("@Honor", info.Honor);
				sqlParameters[10] = new SqlParameter("@IP", info.IP);
				sqlParameters[11] = new SqlParameter("@IsExist", info.IsExist);
				sqlParameters[12] = new SqlParameter("@Level", info.Level);
				sqlParameters[13] = new SqlParameter("@MaxCount", info.MaxCount);
				sqlParameters[14] = new SqlParameter("@Placard", info.Placard);
				sqlParameters[15] = new SqlParameter("@Port", info.Port);
				sqlParameters[16] = new SqlParameter("@Repute", info.Repute);
				sqlParameters[17] = new SqlParameter("@Count", info.Count);
				sqlParameters[18] = new SqlParameter("@Riches", info.Riches);
				sqlParameters[19] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[19].Direction = ParameterDirection.ReturnValue;
				sqlParameters[20] = new SqlParameter("@tempDutyLevel", SqlDbType.Int);
				sqlParameters[20].Direction = ParameterDirection.InputOutput;
				sqlParameters[20].Value = dutyInfo.Level;
				sqlParameters[21] = new SqlParameter("@tempDutyName", SqlDbType.NVarChar, 100);
				sqlParameters[21].Direction = ParameterDirection.InputOutput;
				sqlParameters[21].Value = "";
				sqlParameters[22] = new SqlParameter("@tempRight", SqlDbType.Int);
				sqlParameters[22].Direction = ParameterDirection.InputOutput;
				sqlParameters[22].Value = dutyInfo.Right;
				flag = this.db.RunProcedure("SP_Consortia_Add", sqlParameters);
				int num = (int)sqlParameters[19].Value;
				flag = (num == 0);
				if (flag)
				{
					info.ConsortiaID = (int)sqlParameters[0].Value;
					dutyInfo.Level = (int)sqlParameters[20].Value;
					dutyInfo.DutyName = sqlParameters[21].Value.ToString();
					dutyInfo.Right = (int)sqlParameters[22].Value;
				}
				int num2 = num;
				if (num2 == 2)
				{
					msg = "ConsortiaBussiness.AddConsortia.Msg2";
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
		public bool AddConsortiaAlly(ConsortiaAllyInfo info, int userID, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[9];
				sqlParameters[0] = new SqlParameter("@ID", info.ID);
				sqlParameters[0].Direction = ParameterDirection.InputOutput;
				sqlParameters[1] = new SqlParameter("@Consortia1ID", info.Consortia1ID);
				sqlParameters[2] = new SqlParameter("@Consortia2ID", info.Consortia2ID);
				sqlParameters[3] = new SqlParameter("@State", info.State);
				sqlParameters[4] = new SqlParameter("@Date", info.Date);
				sqlParameters[5] = new SqlParameter("@ValidDate", info.ValidDate);
				sqlParameters[6] = new SqlParameter("@IsExist", info.IsExist);
				sqlParameters[7] = new SqlParameter("@UserID", userID);
				sqlParameters[8] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[8].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_ConsortiaAlly_Add", sqlParameters);
				int num = (int)sqlParameters[8].Value;
				flag = (num == 0);
				switch (num)
				{
				case 2:
				{
					msg = "ConsortiaBussiness.AddConsortiaAlly.Msg2";
					bool result = flag;
					return result;
				}
				case 3:
				{
					msg = "ConsortiaBussiness.AddConsortiaAlly.Msg3";
					bool result = flag;
					return result;
				}
				default:
				{
					bool result = flag;
					return result;
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
			return flag;
		}
		public bool AddConsortiaApplyAlly(ConsortiaApplyAllyInfo info, int userID, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[9];
				sqlParameters[0] = new SqlParameter("@ID", info.ID);
				sqlParameters[0].Direction = ParameterDirection.InputOutput;
				sqlParameters[1] = new SqlParameter("@Consortia1ID", info.Consortia1ID);
				sqlParameters[2] = new SqlParameter("@Consortia2ID", info.Consortia2ID);
				sqlParameters[3] = new SqlParameter("@Date", info.Date);
				sqlParameters[4] = new SqlParameter("@Remark", info.Remark);
				sqlParameters[5] = new SqlParameter("@IsExist", info.IsExist);
				sqlParameters[6] = new SqlParameter("@UserID", userID);
				sqlParameters[7] = new SqlParameter("@State", info.State);
				sqlParameters[8] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[8].Direction = ParameterDirection.ReturnValue;
				flag = this.db.RunProcedure("SP_ConsortiaApplyAlly_Add", sqlParameters);
				info.ID = (int)sqlParameters[0].Value;
				int num = (int)sqlParameters[8].Value;
				flag = (num == 0);
				switch (num)
				{
				case 2:
				{
					msg = "ConsortiaBussiness.AddConsortiaApplyAlly.Msg2";
					bool result = flag;
					return result;
				}
				case 3:
				{
					msg = "ConsortiaBussiness.AddConsortiaApplyAlly.Msg3";
					bool result = flag;
					return result;
				}
				case 4:
				{
					msg = "ConsortiaBussiness.AddConsortiaApplyAlly.Msg4";
					bool result = flag;
					return result;
				}
				default:
				{
					bool result = flag;
					return result;
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
			return flag;
		}
		public bool AddConsortiaApplyUsers(ConsortiaApplyUserInfo info, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[9];
				sqlParameters[0] = new SqlParameter("@ID", info.ID);
				sqlParameters[0].Direction = ParameterDirection.InputOutput;
				sqlParameters[1] = new SqlParameter("@ApplyDate", info.ApplyDate);
				sqlParameters[2] = new SqlParameter("@ConsortiaID", info.ConsortiaID);
				sqlParameters[3] = new SqlParameter("@ConsortiaName", (info.ConsortiaName == null) ? "" : info.ConsortiaName);
				sqlParameters[4] = new SqlParameter("@IsExist", info.IsExist);
				sqlParameters[5] = new SqlParameter("@Remark", (info.Remark == null) ? "" : info.Remark);
				sqlParameters[6] = new SqlParameter("@UserID", info.UserID);
				sqlParameters[7] = new SqlParameter("@UserName", (info.UserName == null) ? "" : info.UserName);
				sqlParameters[8] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[8].Direction = ParameterDirection.ReturnValue;
				flag = this.db.RunProcedure("SP_ConsortiaApplyUser_Add", sqlParameters);
				info.ID = (int)sqlParameters[0].Value;
				int num = (int)sqlParameters[8].Value;
				flag = (num == 0);
				int num2 = num;
				if (num2 != 2)
				{
					switch (num2)
					{
					case 6:
					{
						msg = "ConsortiaBussiness.AddConsortiaApplyUsers.Msg6";
						bool result = flag;
						return result;
					}
					case 7:
					{
						msg = "ConsortiaBussiness.AddConsortiaApplyUsers.Msg7";
						bool result = flag;
						return result;
					}
					default:
					{
						bool result = flag;
						return result;
					}
					}
				}
				else
				{
					msg = "ConsortiaBussiness.AddConsortiaApplyUsers.Msg2";
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
		public bool AddConsortiaDuty(ConsortiaDutyInfo info, int userID, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[7];
				sqlParameters[0] = new SqlParameter("@DutyID", info.DutyID);
				sqlParameters[0].Direction = ParameterDirection.InputOutput;
				sqlParameters[1] = new SqlParameter("@ConsortiaID", info.ConsortiaID);
				sqlParameters[2] = new SqlParameter("@DutyName", info.DutyName);
				sqlParameters[3] = new SqlParameter("@Level", info.Level);
				sqlParameters[4] = new SqlParameter("@UserID", userID);
				sqlParameters[5] = new SqlParameter("@Right", info.Right);
				sqlParameters[6] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[6].Direction = ParameterDirection.ReturnValue;
				flag = this.db.RunProcedure("SP_ConsortiaDuty_Add", sqlParameters);
				info.DutyID = (int)sqlParameters[0].Value;
				int num = (int)sqlParameters[6].Value;
				flag = (num == 0);
				switch (num)
				{
				case 2:
				{
					msg = "ConsortiaBussiness.AddConsortiaDuty.Msg2";
					bool result = flag;
					return result;
				}
				case 3:
				{
					msg = "ConsortiaBussiness.AddConsortiaDuty.Msg3";
					bool result = flag;
					return result;
				}
				default:
				{
					bool result = flag;
					return result;
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
			return flag;
		}
		public bool AddConsortiaInviteUsers(ConsortiaInviteUserInfo info, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[11];
				sqlParameters[0] = new SqlParameter("@ID", info.ID);
				sqlParameters[0].Direction = ParameterDirection.InputOutput;
				sqlParameters[1] = new SqlParameter("@ConsortiaID", info.ConsortiaID);
				sqlParameters[2] = new SqlParameter("@ConsortiaName", (info.ConsortiaName == null) ? "" : info.ConsortiaName);
				sqlParameters[3] = new SqlParameter("@InviteDate", info.InviteDate);
				sqlParameters[4] = new SqlParameter("@InviteID", info.InviteID);
				sqlParameters[5] = new SqlParameter("@InviteName", (info.InviteName == null) ? "" : info.InviteName);
				sqlParameters[6] = new SqlParameter("@IsExist", info.IsExist);
				sqlParameters[7] = new SqlParameter("@Remark", (info.Remark == null) ? "" : info.Remark);
				sqlParameters[8] = new SqlParameter("@UserID", info.UserID);
				sqlParameters[8].Direction = ParameterDirection.InputOutput;
				sqlParameters[9] = new SqlParameter("@UserName", (info.UserName == null) ? "" : info.UserName);
				sqlParameters[10] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[10].Direction = ParameterDirection.ReturnValue;
				flag = this.db.RunProcedure("SP_ConsortiaInviteUser_Add", sqlParameters);
				info.ID = (int)sqlParameters[0].Value;
				info.UserID = (int)sqlParameters[8].Value;
				int num = (int)sqlParameters[10].Value;
				flag = (num == 0);
				switch (num)
				{
				case 2:
				{
					msg = "ConsortiaBussiness.AddConsortiaInviteUsers.Msg2";
					bool result = flag;
					return result;
				}
				case 3:
				{
					bool result = flag;
					return result;
				}
				case 4:
				{
					msg = "ConsortiaBussiness.AddConsortiaInviteUsers.Msg4";
					bool result = flag;
					return result;
				}
				case 5:
				{
					msg = "ConsortiaBussiness.AddConsortiaInviteUsers.Msg5";
					bool result = flag;
					return result;
				}
				case 6:
				{
					msg = "ConsortiaBussiness.AddConsortiaInviteUsers.Msg6";
					bool result = flag;
					return result;
				}
				default:
				{
					bool result = flag;
					return result;
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
			return flag;
		}
		public bool BuyBadge(int consortiaID, int userID, ConsortiaInfo info, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@BadgeID", info.BadgeID),
					new SqlParameter("@ValidDate", info.ValidDate),
					new SqlParameter("@BadgeBuyTime", info.BadgeBuyTime),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[5].Direction = ParameterDirection.ReturnValue;
				flag = this.db.RunProcedure("SP_ConsortiaBadge_Update", sqlParameters);
				int num = (int)sqlParameters[5].Value;
				flag = (num == 0);
				int num2 = num;
				if (num2 == 2)
				{
					msg = "ConsortiaBussiness.BuyBadge.Msg2";
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
		public bool ConsortiaFight(int consortiWin, int consortiaLose, int playerCount, out int riches, int state, int totalKillHealth, float richesRate)
		{
			bool flag = false;
			riches = 0;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[8];
				sqlParameters[0] = new SqlParameter("@ConsortiaWin", consortiWin);
				sqlParameters[1] = new SqlParameter("@ConsortiaLose", consortiaLose);
				sqlParameters[2] = new SqlParameter("@PlayerCount", playerCount);
				sqlParameters[3] = new SqlParameter("@Riches", SqlDbType.Int);
				sqlParameters[3].Direction = ParameterDirection.InputOutput;
				sqlParameters[3].Value = riches;
				sqlParameters[4] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[4].Direction = ParameterDirection.ReturnValue;
				sqlParameters[5] = new SqlParameter("@State", state);
				sqlParameters[6] = new SqlParameter("@TotalKillHealth", totalKillHealth);
				sqlParameters[7] = new SqlParameter("@RichesRate", richesRate);
				flag = this.db.RunProcedure("SP_Consortia_Fight", sqlParameters);
				riches = (int)sqlParameters[3].Value;
				int num = (int)sqlParameters[4].Value;
				flag = (num == 0);
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("ConsortiaFight", exception);
				}
			}
			return flag;
		}
		public bool ConsortiaRichAdd(int consortiID, ref int riches)
		{
			return this.ConsortiaRichAdd(consortiID, ref riches, 0, "");
		}
		public bool ConsortiaRichAdd(int consortiID, ref int riches, int type, string username)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[5];
				sqlParameters[0] = new SqlParameter("@ConsortiaID", consortiID);
				sqlParameters[1] = new SqlParameter("@Riches", SqlDbType.Int);
				sqlParameters[1].Direction = ParameterDirection.InputOutput;
				sqlParameters[1].Value = riches;
				sqlParameters[2] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[2].Direction = ParameterDirection.ReturnValue;
				sqlParameters[3] = new SqlParameter("@Type", type);
				sqlParameters[4] = new SqlParameter("@UserName", username);
				flag = this.db.RunProcedure("SP_Consortia_Riches_Add", sqlParameters);
				riches = (int)sqlParameters[1].Value;
				int num = (int)sqlParameters[2].Value;
				flag = (num == 0);
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("ConsortiaRichAdd", exception);
				}
			}
			return flag;
		}
		public bool DeleteConsortia(int consortiaID, int userID, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[2].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_Consortia_Delete", sqlParameters);
				int num = (int)sqlParameters[2].Value;
				flag = (num == 0);
				switch (num)
				{
				case 2:
				{
					msg = "ConsortiaBussiness.DeleteConsortia.Msg2";
					bool result = flag;
					return result;
				}
				case 3:
				{
					msg = "ConsortiaBussiness.DeleteConsortia.Msg3";
					bool result = flag;
					return result;
				}
				default:
				{
					bool result = flag;
					return result;
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
			return flag;
		}
		public bool DeleteConsortiaApplyAlly(int applyID, int userID, int consortiaID, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ID", applyID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[3].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_ConsortiaApplyAlly_Delete", sqlParameters);
				int num = (int)sqlParameters[3].Value;
				flag = (num == 0);
				int num2 = num;
				if (num2 == 2)
				{
					msg = "ConsortiaBussiness.DeleteConsortiaApplyAlly.Msg2";
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
		public bool DeleteConsortiaApplyUsers(int applyID, int userID, int consortiaID, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ID", applyID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[3].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_ConsortiaApplyUser_Delete", sqlParameters);
				int num = (int)sqlParameters[3].Value;
				flag = (num == 0 || num == 3);
				int num2 = num;
				if (num2 == 2)
				{
					msg = "ConsortiaBussiness.DeleteConsortiaApplyUsers.Msg2";
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
		public bool DeleteConsortiaDuty(int dutyID, int userID, int consortiaID, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@UserID", userID),
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@DutyID", dutyID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[3].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_ConsortiaDuty_Delete", sqlParameters);
				int num = (int)sqlParameters[3].Value;
				flag = (num == 0);
				switch (num)
				{
				case 2:
				{
					msg = "ConsortiaBussiness.DeleteConsortiaDuty.Msg2";
					bool result = flag;
					return result;
				}
				case 3:
				{
					msg = "ConsortiaBussiness.DeleteConsortiaDuty.Msg3";
					bool result = flag;
					return result;
				}
				default:
				{
					bool result = flag;
					return result;
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
			return flag;
		}
		public bool DeleteConsortiaInviteUsers(int intiveID, int userID)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ID", intiveID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[2].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_ConsortiaInviteUser_Delete", sqlParameters);
				int num = (int)sqlParameters[2].Value;
				flag = (num == 0);
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
		public bool DeleteConsortiaUser(int userID, int kickUserID, int consortiaID, ref string msg, ref string nickName)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[5];
				sqlParameters[0] = new SqlParameter("@UserID", userID);
				sqlParameters[1] = new SqlParameter("@KickUserID", kickUserID);
				sqlParameters[2] = new SqlParameter("@ConsortiaID", consortiaID);
				sqlParameters[3] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[3].Direction = ParameterDirection.ReturnValue;
				sqlParameters[4] = new SqlParameter("@NickName", SqlDbType.NVarChar, 200);
				sqlParameters[4].Direction = ParameterDirection.InputOutput;
				sqlParameters[4].Value = nickName;
				this.db.RunProcedure("SP_ConsortiaUser_Delete", sqlParameters);
				int num = (int)sqlParameters[3].Value;
				if (num == 0)
				{
					flag = true;
					nickName = sqlParameters[4].Value.ToString();
				}
				switch (num)
				{
				case 2:
				{
					msg = "ConsortiaBussiness.DeleteConsortiaUser.Msg2";
					bool result = flag;
					return result;
				}
				case 3:
				{
					msg = "ConsortiaBussiness.DeleteConsortiaUser.Msg3";
					bool result = flag;
					return result;
				}
				case 4:
				{
					msg = "ConsortiaBussiness.DeleteConsortiaUser.Msg4";
					bool result = flag;
					return result;
				}
				case 5:
				{
					msg = "ConsortiaBussiness.DeleteConsortiaUser.Msg5";
					bool result = flag;
					return result;
				}
				default:
				{
					bool result = flag;
					return result;
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
			return flag;
		}
		public ConsortiaLevelInfo[] GetAllConsortiaLevel()
		{
			List<ConsortiaLevelInfo> list = new List<ConsortiaLevelInfo>();
			SqlDataReader resultDataReader = null;
			try
			{
				this.db.GetReader(ref resultDataReader, "SP_Consortia_Level_All");
				while (resultDataReader.Read())
				{
					ConsortiaLevelInfo item = new ConsortiaLevelInfo
					{
						Count = (int)resultDataReader["Count"],
						Deduct = (int)resultDataReader["Deduct"],
						Level = (int)resultDataReader["Level"],
						NeedGold = (int)resultDataReader["NeedGold"],
						NeedItem = (int)resultDataReader["NeedItem"],
						Reward = (int)resultDataReader["Reward"],
						Riches = (int)resultDataReader["Riches"],
						ShopRiches = (int)resultDataReader["ShopRiches"],
						SmithRiches = (int)resultDataReader["SmithRiches"],
						StoreRiches = (int)resultDataReader["StoreRiches"],
						BufferRiches = (int)resultDataReader["BufferRiches"]
					};
					list.Add(item);
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllConsortiaLevel", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return list.ToArray();
		}
		public ConsortiaInfo[] GetConsortiaAll()
		{
			List<ConsortiaInfo> list = new List<ConsortiaInfo>();
			SqlDataReader resultDataReader = null;
			try
			{
				this.db.GetReader(ref resultDataReader, "SP_Consortia_All");
				while (resultDataReader.Read())
				{
					ConsortiaInfo item = new ConsortiaInfo
					{
						ConsortiaID = (int)resultDataReader["ConsortiaID"],
						Honor = (int)resultDataReader["Honor"],
						Level = (int)resultDataReader["Level"],
						Riches = (int)resultDataReader["Riches"],
						MaxCount = (int)resultDataReader["MaxCount"],
						BuildDate = (DateTime)resultDataReader["BuildDate"],
						IsExist = (bool)resultDataReader["IsExist"],
						DeductDate = (DateTime)resultDataReader["DeductDate"],
						StoreLevel = (int)resultDataReader["StoreLevel"],
						SmithLevel = (int)resultDataReader["SmithLevel"],
						ShopLevel = (int)resultDataReader["ShopLevel"],
						SkillLevel = (int)resultDataReader["SkillLevel"],
						ConsortiaName = (resultDataReader["ConsortiaName"] == null) ? "" : resultDataReader["ConsortiaName"].ToString(),
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
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return list.ToArray();
		}
		public ConsortiaAllyInfo[] GetConsortiaAllyAll()
		{
			List<ConsortiaAllyInfo> list = new List<ConsortiaAllyInfo>();
			SqlDataReader resultDataReader = null;
			try
			{
				this.db.GetReader(ref resultDataReader, "SP_ConsortiaAlly_All");
				while (resultDataReader.Read())
				{
					ConsortiaAllyInfo item = new ConsortiaAllyInfo
					{
						Consortia1ID = (int)resultDataReader["Consortia1ID"],
						Consortia2ID = (int)resultDataReader["Consortia2ID"],
						Date = (DateTime)resultDataReader["Date"],
						ID = (int)resultDataReader["ID"],
						State = (int)resultDataReader["State"],
						ValidDate = (int)resultDataReader["ValidDate"],
						IsExist = (bool)resultDataReader["IsExist"]
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
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return list.ToArray();
		}
		public ConsortiaAllyInfo[] GetConsortiaAllyPage(int page, int size, ref int total, int order, int consortiaID, int state, string name)
		{
			List<ConsortiaAllyInfo> list = new List<ConsortiaAllyInfo>();
			string queryWhere = " IsExist=1 and ConsortiaID<>" + consortiaID;
			Dictionary<int, int> consortiaByAlly = this.GetConsortiaByAlly(consortiaID);
			try
			{
				if (state != -1)
				{
					string str2 = string.Empty;
					foreach (int num in consortiaByAlly.Keys)
					{
						str2 = str2 + num + ",";
					}
					str2 += 0;
					if (state == 0)
					{
						queryWhere = queryWhere + " and ConsortiaID not in (" + str2 + ") ";
					}
					else
					{
						queryWhere = queryWhere + " and ConsortiaID in (" + str2 + ") ";
					}
				}
				if (!string.IsNullOrEmpty(name))
				{
					queryWhere = queryWhere + " and ConsortiaName like '%" + name + "%' ";
				}
				DataTable table = base.GetPage("Consortia", queryWhere, page, size, "*", "ConsortiaID", "ConsortiaID", ref total);
				foreach (DataRow row in table.Rows)
				{
					ConsortiaAllyInfo item = new ConsortiaAllyInfo
					{
						Consortia1ID = (int)row["ConsortiaID"],
						ConsortiaName1 = (row["ConsortiaName"] == null) ? "" : row["ConsortiaName"].ToString(),
						ConsortiaName2 = "",
						Count1 = (int)row["Count"],
						Repute1 = (int)row["Repute"],
						ChairmanName1 = (row["ChairmanName"] == null) ? "" : row["ChairmanName"].ToString(),
						ChairmanName2 = "",
						Level1 = (int)row["Level"],
						Honor1 = (int)row["Honor"],
						Description1 = (row["Description"] == null) ? "" : row["Description"].ToString(),
						Description2 = "",
						Riches1 = (int)row["Riches"],
						Date = DateTime.Now,
						IsExist = true
					};
					if (consortiaByAlly.ContainsKey(item.Consortia1ID))
					{
						item.State = consortiaByAlly[item.Consortia1ID];
					}
					item.ValidDate = 0;
					list.Add(item);
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetConsortiaAllyPage", exception);
				}
			}
			return list.ToArray();
		}
		public ConsortiaApplyAllyInfo[] GetConsortiaApplyAllyPage(int page, int size, ref int total, int order, int consortiaID, int applyID, int state)
		{
			List<ConsortiaApplyAllyInfo> list = new List<ConsortiaApplyAllyInfo>();
			try
			{
				string queryWhere = " IsExist=1 ";
				if (consortiaID != -1)
				{
					object obj2 = queryWhere;
					queryWhere = string.Concat(new object[]
					{
						obj2,
						" and Consortia2ID =",
						consortiaID,
						" "
					});
				}
				if (applyID != -1)
				{
					object obj3 = queryWhere;
					queryWhere = string.Concat(new object[]
					{
						obj3,
						" and ID =",
						applyID,
						" "
					});
				}
				if (state != -1)
				{
					object obj4 = queryWhere;
					queryWhere = string.Concat(new object[]
					{
						obj4,
						" and State =",
						state,
						" "
					});
				}
				string fdOreder = "ConsortiaName";
				switch (order)
				{
				case 1:
					fdOreder = "Repute";
					break;
				case 2:
					fdOreder = "ChairmanName";
					break;
				case 3:
					fdOreder = "Count";
					break;
				case 4:
					fdOreder = "Level";
					break;
				case 5:
					fdOreder = "Honor";
					break;
				}
				fdOreder += ",ID ";
				DataTable table = base.GetPage("V_Consortia_Apply_Ally", queryWhere, page, size, "*", fdOreder, "ID", ref total);
				foreach (DataRow row in table.Rows)
				{
					ConsortiaApplyAllyInfo item = new ConsortiaApplyAllyInfo
					{
						ID = (int)row["ID"],
						CelebCount = (int)row["CelebCount"],
						ChairmanName = row["ChairmanName"].ToString(),
						Consortia1ID = (int)row["Consortia1ID"],
						Consortia2ID = (int)row["Consortia2ID"],
						ConsortiaName = row["ConsortiaName"].ToString(),
						Count = (int)row["Count"],
						Date = (DateTime)row["Date"],
						Honor = (int)row["Honor"],
						IsExist = (bool)row["IsExist"],
						Remark = row["Remark"].ToString(),
						Repute = (int)row["Repute"],
						State = (int)row["State"],
						Level = (int)row["Level"],
						Description = (row["Description"] == null) ? "" : row["Description"].ToString()
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
		public ConsortiaApplyUserInfo[] GetConsortiaApplyUserPage(int page, int size, ref int total, int order, int consortiaID, int applyID, int userID)
		{
			List<ConsortiaApplyUserInfo> list = new List<ConsortiaApplyUserInfo>();
			try
			{
				string queryWhere = " IsExist=1 ";
				if (consortiaID != -1)
				{
					object obj2 = queryWhere;
					queryWhere = string.Concat(new object[]
					{
						obj2,
						" and ConsortiaID =",
						consortiaID,
						" "
					});
				}
				if (applyID != -1)
				{
					object obj3 = queryWhere;
					queryWhere = string.Concat(new object[]
					{
						obj3,
						" and ID =",
						applyID,
						" "
					});
				}
				if (userID != -1)
				{
					object obj4 = queryWhere;
					queryWhere = string.Concat(new object[]
					{
						obj4,
						" and UserID ='",
						userID,
						"' "
					});
				}
				string fdOreder = "ID";
				switch (order)
				{
				case 1:
					fdOreder = "UserName,ID";
					break;
				case 2:
					fdOreder = "ApplyDate,ID";
					break;
				}
				DataTable table = base.GetPage("V_Consortia_Apply_Users", queryWhere, page, size, "*", fdOreder, "ID", ref total);
				foreach (DataRow row in table.Rows)
				{
					ConsortiaApplyUserInfo item = new ConsortiaApplyUserInfo
					{
						ID = (int)row["ID"],
						ApplyDate = (DateTime)row["ApplyDate"],
						ConsortiaID = (int)row["ConsortiaID"],
						ConsortiaName = row["ConsortiaName"].ToString(),
						ChairmanID = (int)row["ChairmanID"],
						ChairmanName = row["ChairmanName"].ToString(),
						IsExist = (bool)row["IsExist"],
						Remark = row["Remark"].ToString(),
						UserID = (int)row["UserID"],
						UserName = row["UserName"].ToString(),
						UserLevel = (int)row["Grade"],
						typeVIP = (int)row["typeVIP"],
						Win = (int)row["Win"],
						Total = (int)row["Total"],
						Repute = (int)row["Repute"],
						FightPower = (int)row["FightPower"],
						IsOld = (bool)row["IsOldPlayer"],
						Offer = (int)row["Offer"]
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
		public Dictionary<int, int> GetConsortiaByAlly(int consortiaID)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID)
				};
				this.db.GetReader(ref resultDataReader, "SP_Consortia_Ally_Neutral", sqlParameters);
				while (resultDataReader.Read())
				{
					if ((int)resultDataReader["Consortia1ID"] != consortiaID)
					{
						dictionary.Add((int)resultDataReader["Consortia1ID"], (int)resultDataReader["State"]);
					}
					else
					{
						dictionary.Add((int)resultDataReader["Consortia2ID"], (int)resultDataReader["State"]);
					}
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetConsortiaByAlly", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return dictionary;
		}
		public int[] GetConsortiaByAllyByState(int consortiaID, int state)
		{
			List<int> list = new List<int>();
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@State", state)
				};
				this.db.GetReader(ref resultDataReader, "SP_Consortia_AllyByState", sqlParameters);
				while (resultDataReader.Read())
				{
					list.Add((int)resultDataReader["Consortia2ID"]);
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
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return list.ToArray();
		}
		public ConsortiaDutyInfo[] GetConsortiaDutyPage(int page, int size, ref int total, int order, int consortiaID, int dutyID)
		{
			List<ConsortiaDutyInfo> list = new List<ConsortiaDutyInfo>();
			try
			{
				string queryWhere = " IsExist=1 ";
				if (consortiaID != -1)
				{
					object obj2 = queryWhere;
					queryWhere = string.Concat(new object[]
					{
						obj2,
						" and ConsortiaID =",
						consortiaID,
						" "
					});
				}
				if (dutyID != -1)
				{
					object obj3 = queryWhere;
					queryWhere = string.Concat(new object[]
					{
						obj3,
						" and DutyID =",
						dutyID,
						" "
					});
				}
				string fdOreder = "Level";
				if (order == 1)
				{
					fdOreder = "DutyName";
				}
				fdOreder += ",DutyID ";
				DataTable table = base.GetPage("Consortia_Duty", queryWhere, page, size, "*", fdOreder, "DutyID", ref total);
				foreach (DataRow row in table.Rows)
				{
					ConsortiaDutyInfo item = new ConsortiaDutyInfo
					{
						DutyID = (int)row["DutyID"],
						ConsortiaID = (int)row["ConsortiaID"],
						DutyName = row["DutyName"].ToString(),
						IsExist = (bool)row["IsExist"],
						Right = (int)row["Right"],
						Level = (int)row["Level"]
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
		public ConsortiaEquipControlInfo[] GetConsortiaEquipControlPage(int page, int size, ref int total, int order, int consortiaID, int level, int type)
		{
			List<ConsortiaEquipControlInfo> list = new List<ConsortiaEquipControlInfo>();
			try
			{
				string queryWhere = " IsExist=1 ";
				if (consortiaID != -1)
				{
					object obj2 = queryWhere;
					queryWhere = string.Concat(new object[]
					{
						obj2,
						" and ConsortiaID =",
						consortiaID,
						" "
					});
				}
				if (level != -1)
				{
					object obj3 = queryWhere;
					queryWhere = string.Concat(new object[]
					{
						obj3,
						" and Level =",
						level,
						" "
					});
				}
				if (type != -1)
				{
					object obj4 = queryWhere;
					queryWhere = string.Concat(new object[]
					{
						obj4,
						" and Type =",
						type,
						" "
					});
				}
				string fdOreder = "ConsortiaID ";
				DataTable table = base.GetPage("Consortia_Equip_Control", queryWhere, page, size, "*", fdOreder, "ConsortiaID", ref total);
				foreach (DataRow row in table.Rows)
				{
					ConsortiaEquipControlInfo item = new ConsortiaEquipControlInfo
					{
						ConsortiaID = (int)row["ConsortiaID"],
						Level = (int)row["Level"],
						Riches = (int)row["Riches"],
						Type = (int)row["Type"]
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
		public ConsortiaEquipControlInfo GetConsortiaEuqipRiches(int consortiaID, int Level, int type)
		{
			ConsortiaEquipControlInfo info = null;
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@Level", Level),
					new SqlParameter("@Type", type)
				};
				this.db.GetReader(ref resultDataReader, "SP_Consortia_Equip_Control_Single", sqlParameters);
				if (resultDataReader.Read())
				{
					ConsortiaEquipControlInfo result = new ConsortiaEquipControlInfo
					{
						ConsortiaID = (int)resultDataReader["ConsortiaID"],
						Level = (int)resultDataReader["Level"],
						Riches = (int)resultDataReader["Riches"],
						Type = (int)resultDataReader["Type"]
					};
					return result;
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllConsortiaLevel", exception);
				}
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			if (info == null)
			{
				return new ConsortiaEquipControlInfo
				{
					ConsortiaID = consortiaID,
					Level = Level,
					Riches = 100,
					Type = type
				};
			}
			return info;
		}
		public ConsortiaEventInfo[] GetConsortiaEventPage(int page, int size, ref int total, int order, int consortiaID)
		{
			List<ConsortiaEventInfo> list = new List<ConsortiaEventInfo>();
			try
			{
				string queryWhere = " IsExist=1 ";
				if (consortiaID != -1)
				{
					object obj2 = queryWhere;
					queryWhere = string.Concat(new object[]
					{
						obj2,
						" and ConsortiaID =",
						consortiaID,
						" "
					});
				}
				string fdOreder = "Date desc,ID ";
				DataTable table = base.GetPage("Consortia_Event", queryWhere, page, size, "*", fdOreder, "ID", ref total);
				foreach (DataRow row in table.Rows)
				{
					ConsortiaEventInfo item = new ConsortiaEventInfo
					{
						ID = (int)row["ID"],
						ConsortiaID = (int)row["ConsortiaID"],
						Date = (DateTime)row["Date"],
						IsExist = (bool)row["IsExist"],
						Remark = row["Remark"].ToString(),
						Type = (int)row["Type"]
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
		public ConsortiaInviteUserInfo[] GetConsortiaInviteUserPage(int page, int size, ref int total, int order, int userID, int inviteID)
		{
			List<ConsortiaInviteUserInfo> list = new List<ConsortiaInviteUserInfo>();
			try
			{
				string queryWhere = " IsExist=1 ";
				if (userID != -1)
				{
					object obj2 = queryWhere;
					queryWhere = string.Concat(new object[]
					{
						obj2,
						" and UserID =",
						userID,
						" "
					});
				}
				if (inviteID != -1)
				{
					object obj3 = queryWhere;
					queryWhere = string.Concat(new object[]
					{
						obj3,
						" and UserID =",
						inviteID,
						" "
					});
				}
				string fdOreder = "ConsortiaName";
				switch (order)
				{
				case 1:
					fdOreder = "Repute";
					break;
				case 2:
					fdOreder = "ChairmanName";
					break;
				case 3:
					fdOreder = "Count";
					break;
				case 4:
					fdOreder = "CelebCount";
					break;
				case 5:
					fdOreder = "Honor";
					break;
				}
				fdOreder += ",ID ";
				DataTable table = base.GetPage("V_Consortia_Invite", queryWhere, page, size, "*", fdOreder, "ID", ref total);
				foreach (DataRow row in table.Rows)
				{
					ConsortiaInviteUserInfo item = new ConsortiaInviteUserInfo
					{
						ID = (int)row["ID"],
						CelebCount = (int)row["CelebCount"],
						ChairmanName = row["ChairmanName"].ToString(),
						ConsortiaID = (int)row["ConsortiaID"],
						ConsortiaName = row["ConsortiaName"].ToString(),
						Count = (int)row["Count"],
						Honor = (int)row["Honor"],
						InviteDate = (DateTime)row["InviteDate"],
						InviteID = (int)row["InviteID"],
						InviteName = row["InviteName"].ToString(),
						IsExist = (bool)row["IsExist"],
						Remark = row["Remark"].ToString(),
						Repute = (int)row["Repute"],
						UserID = (int)row["UserID"],
						UserName = row["UserName"].ToString()
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
		public ConsortiaInfo[] GetConsortiaPage(int page, int size, ref int total, int order, string name, int consortiaID, int level, int openApply)
		{
			List<ConsortiaInfo> list = new List<ConsortiaInfo>();
			try
			{
				string queryWhere = " IsExist=1 ";
				if (!string.IsNullOrEmpty(name))
				{
					queryWhere = queryWhere + " and ConsortiaName like '%" + name + "%' ";
				}
				if (consortiaID != -1)
				{
					object obj2 = queryWhere;
					queryWhere = string.Concat(new object[]
					{
						obj2,
						" and ConsortiaID =",
						consortiaID,
						" "
					});
				}
				if (level != -1)
				{
					object obj3 = queryWhere;
					queryWhere = string.Concat(new object[]
					{
						obj3,
						" and Level =",
						level,
						" "
					});
				}
				if (openApply != -1)
				{
					object obj4 = queryWhere;
					queryWhere = string.Concat(new object[]
					{
						obj4,
						" and OpenApply =",
						openApply,
						" "
					});
				}
				string fdOreder = "ConsortiaName";
				switch (order)
				{
				case 1:
					fdOreder = "ReputeSort";
					break;
				case 2:
					fdOreder = "ChairmanName";
					break;
				case 3:
					fdOreder = "Count desc";
					break;
				case 4:
					fdOreder = "Level desc";
					break;
				case 5:
					fdOreder = "Honor desc";
					break;
				case 10:
					fdOreder = "LastDayRiches desc";
					break;
				case 11:
					fdOreder = "AddDayRiches desc";
					break;
				case 12:
					fdOreder = "AddWeekRiches desc";
					break;
				case 13:
					fdOreder = "LastDayHonor desc";
					break;
				case 14:
					fdOreder = "AddDayHonor desc";
					break;
				case 15:
					fdOreder = "AddWeekHonor desc";
					break;
				case 16:
					fdOreder = "level desc,LastDayRiches desc";
					break;
				}
				fdOreder += ",ConsortiaID ";
				DataTable table = base.GetPage("V_Consortia", queryWhere, page, size, "*", fdOreder, "ConsortiaID", ref total);
				foreach (DataRow row in table.Rows)
				{
					ConsortiaInfo item = new ConsortiaInfo
					{
						ConsortiaID = (int)row["ConsortiaID"],
						BuildDate = (DateTime)row["BuildDate"],
						CelebCount = (int)row["CelebCount"],
						ChairmanID = (int)row["ChairmanID"],
						ChairmanName = row["ChairmanName"].ToString(),
						ChairmanTypeVIP = Convert.ToByte(row["typeVIP"]),
						ChairmanVIPLevel = (int)row["VIPLevel"],
						ConsortiaName = row["ConsortiaName"].ToString(),
						CreatorID = (int)row["CreatorID"],
						CreatorName = row["CreatorName"].ToString(),
						Description = row["Description"].ToString(),
						Honor = (int)row["Honor"],
						IsExist = (bool)row["IsExist"],
						Level = (int)row["Level"],
						MaxCount = (int)row["MaxCount"],
						Placard = row["Placard"].ToString(),
						IP = row["IP"].ToString(),
						Port = (int)row["Port"],
						Repute = (int)row["Repute"],
						Count = (int)row["Count"],
						Riches = (int)row["Riches"],
						DeductDate = (DateTime)row["DeductDate"],
						AddDayHonor = (int)row["AddDayHonor"],
						AddDayRiches = (int)row["AddDayRiches"],
						AddWeekHonor = (int)row["AddWeekHonor"],
						AddWeekRiches = (int)row["AddWeekRiches"],
						LastDayRiches = (int)row["LastDayRiches"],
						OpenApply = (bool)row["OpenApply"],
						StoreLevel = (int)row["StoreLevel"],
						SmithLevel = (int)row["SmithLevel"],
						ShopLevel = (int)row["ShopLevel"],
						SkillLevel = (int)row["SkillLevel"]
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
		public ConsortiaInfo GetConsortiaSingle(int id)
		{
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ID", id)
				};
				this.db.GetReader(ref resultDataReader, "SP_Consortia_Single", sqlParameters);
				if (resultDataReader.Read())
				{
					return new ConsortiaInfo
					{
						ConsortiaID = (int)resultDataReader["ConsortiaID"],
						BuildDate = (DateTime)resultDataReader["BuildDate"],
						CelebCount = (int)resultDataReader["CelebCount"],
						ChairmanID = (int)resultDataReader["ChairmanID"],
						ChairmanName = resultDataReader["ChairmanName"].ToString(),
						ChairmanTypeVIP = Convert.ToByte(resultDataReader["typeVIP"]),
						ChairmanVIPLevel = (int)resultDataReader["VIPLevel"],
						ConsortiaName = resultDataReader["ConsortiaName"].ToString(),
						CreatorID = (int)resultDataReader["CreatorID"],
						CreatorName = resultDataReader["CreatorName"].ToString(),
						Description = resultDataReader["Description"].ToString(),
						Honor = (int)resultDataReader["Honor"],
						IsExist = (bool)resultDataReader["IsExist"],
						Level = (int)resultDataReader["Level"],
						MaxCount = (int)resultDataReader["MaxCount"],
						Placard = resultDataReader["Placard"].ToString(),
						IP = resultDataReader["IP"].ToString(),
						Port = (int)resultDataReader["Port"],
						Repute = (int)resultDataReader["Repute"],
						Count = (int)resultDataReader["Count"],
						Riches = (int)resultDataReader["Riches"],
						DeductDate = (DateTime)resultDataReader["DeductDate"],
						StoreLevel = (int)resultDataReader["StoreLevel"],
						SmithLevel = (int)resultDataReader["SmithLevel"],
						ShopLevel = (int)resultDataReader["ShopLevel"],
						SkillLevel = (int)resultDataReader["SkillLevel"]
					};
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
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return null;
		}
		public ConsortiaInfo GetConsortiaSingleByName(string ConsortiaName)
		{
			SqlDataReader resultDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaName", SqlDbType.NVarChar, 200)
				};
				sqlParameters[0].Value = ConsortiaName;
				this.db.GetReader(ref resultDataReader, "SP_Consortia_CheckByName", sqlParameters);
				if (resultDataReader.Read())
				{
					return new ConsortiaInfo
					{
						ConsortiaID = (int)resultDataReader["ConsortiaID"],
						BuildDate = (DateTime)resultDataReader["BuildDate"],
						CelebCount = (int)resultDataReader["CelebCount"],
						ChairmanID = (int)resultDataReader["ChairmanID"],
						ChairmanName = resultDataReader["ChairmanName"].ToString(),
						ConsortiaName = resultDataReader["ConsortiaName"].ToString(),
						CreatorID = (int)resultDataReader["CreatorID"],
						CreatorName = resultDataReader["CreatorName"].ToString(),
						Description = resultDataReader["Description"].ToString(),
						Honor = (int)resultDataReader["Honor"],
						IsExist = (bool)resultDataReader["IsExist"],
						Level = (int)resultDataReader["Level"],
						MaxCount = (int)resultDataReader["MaxCount"],
						Placard = resultDataReader["Placard"].ToString(),
						IP = resultDataReader["IP"].ToString(),
						Port = (int)resultDataReader["Port"],
						Repute = (int)resultDataReader["Repute"],
						Count = (int)resultDataReader["Count"],
						Riches = (int)resultDataReader["Riches"],
						DeductDate = (DateTime)resultDataReader["DeductDate"],
						StoreLevel = (int)resultDataReader["StoreLevel"],
						SmithLevel = (int)resultDataReader["SmithLevel"],
						ShopLevel = (int)resultDataReader["ShopLevel"],
						SkillLevel = (int)resultDataReader["SkillLevel"]
					};
				}
			}
			catch
			{
				throw new Exception();
			}
			finally
			{
				if (resultDataReader != null && !resultDataReader.IsClosed)
				{
					resultDataReader.Close();
				}
			}
			return null;
		}
		public ConsortiaUserInfo GetConsortiaUsersByUserID(int userID)
		{
			int total = 0;
			ConsortiaUserInfo[] infoArray = this.GetConsortiaUsersPage(1, 1, ref total, -1, -1, userID, -1);
			if (infoArray.Length == 1)
			{
				return infoArray[0];
			}
			return null;
		}
		public ConsortiaUserInfo[] GetConsortiaUsersPage(int page, int size, ref int total, int order, int consortiaID, int userID, int state)
		{
			List<ConsortiaUserInfo> list = new List<ConsortiaUserInfo>();
			try
			{
				string queryWhere = " IsExist=1 ";
				if (consortiaID != -1)
				{
					queryWhere = string.Concat(new object[]
					{
						queryWhere,
						" and ConsortiaID =",
						consortiaID,
						" "
					});
				}
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
				if (state != -1)
				{
					queryWhere = string.Concat(new object[]
					{
						queryWhere,
						" and state =",
						state,
						" "
					});
				}
				string str = "UserName";
				switch (order)
				{
				case 1:
					str = "DutyID";
					break;
				case 2:
					str = "Grade";
					break;
				case 3:
					str = "Repute";
					break;
				case 4:
					str = "GP";
					break;
				case 5:
					str = "State";
					break;
				case 6:
					str = "Offer";
					break;
				}
				string fdOreder = str + ",ID ";
				foreach (DataRow dataRow in base.GetPage("V_Consortia_Users", queryWhere, page, size, "*", fdOreder, "ID", ref total).Rows)
				{
					ConsortiaUserInfo consortiaUserInfo = new ConsortiaUserInfo();
					consortiaUserInfo.ID = (int)dataRow["ID"];
					consortiaUserInfo.ConsortiaID = (int)dataRow["ConsortiaID"];
					consortiaUserInfo.DutyID = (int)dataRow["DutyID"];
					consortiaUserInfo.DutyName = dataRow["DutyName"].ToString();
					consortiaUserInfo.IsExist = (bool)dataRow["IsExist"];
					consortiaUserInfo.RatifierID = (int)dataRow["RatifierID"];
					consortiaUserInfo.RatifierName = dataRow["RatifierName"].ToString();
					consortiaUserInfo.Remark = dataRow["Remark"].ToString();
					consortiaUserInfo.UserID = (int)dataRow["UserID"];
					consortiaUserInfo.UserName = dataRow["UserName"].ToString();
					consortiaUserInfo.Grade = (int)dataRow["Grade"];
					consortiaUserInfo.GP = (int)dataRow["GP"];
					consortiaUserInfo.Repute = (int)dataRow["Repute"];
					consortiaUserInfo.State = (int)dataRow["State"];
					consortiaUserInfo.Right = (int)dataRow["Right"];
					consortiaUserInfo.Offer = (int)dataRow["Offer"];
					consortiaUserInfo.Colors = dataRow["Colors"].ToString();
					consortiaUserInfo.Style = dataRow["Style"].ToString();
					consortiaUserInfo.Hide = (int)dataRow["Hide"];
					consortiaUserInfo.Skin = ((dataRow["Skin"] == null) ? "" : consortiaUserInfo.Skin);
					consortiaUserInfo.Level = (int)dataRow["Level"];
					consortiaUserInfo.LastDate = (DateTime)dataRow["LastDate"];
					consortiaUserInfo.Sex = (bool)dataRow["Sex"];
					consortiaUserInfo.IsBanChat = (bool)dataRow["IsBanChat"];
					consortiaUserInfo.Win = (int)dataRow["Win"];
					consortiaUserInfo.Total = (int)dataRow["Total"];
					consortiaUserInfo.Escape = (int)dataRow["Escape"];
					consortiaUserInfo.RichesOffer = (int)dataRow["RichesOffer"];
					consortiaUserInfo.RichesRob = (int)dataRow["RichesRob"];
					consortiaUserInfo.LoginName = ((dataRow["LoginName"] == null) ? "" : dataRow["LoginName"].ToString());
					consortiaUserInfo.Nimbus = (int)dataRow["Nimbus"];
					consortiaUserInfo.FightPower = (int)dataRow["FightPower"];
					consortiaUserInfo.typeVIP = Convert.ToByte(dataRow["typeVIP"]);
					consortiaUserInfo.VIPLevel = (int)dataRow["VIPLevel"];
					list.Add(consortiaUserInfo);
				}
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
		public bool PassConsortiaApplyAlly(int applyID, int userID, int consortiaID, ref int tempID, ref int state, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[6];
				sqlParameters[0] = new SqlParameter("@ID", applyID);
				sqlParameters[1] = new SqlParameter("@UserID", userID);
				sqlParameters[2] = new SqlParameter("@ConsortiaID", consortiaID);
				sqlParameters[3] = new SqlParameter("@tempID", tempID);
				sqlParameters[3].Direction = ParameterDirection.InputOutput;
				sqlParameters[4] = new SqlParameter("@State", state);
				sqlParameters[4].Direction = ParameterDirection.InputOutput;
				sqlParameters[5] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[5].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_ConsortiaApplyAlly_Pass", sqlParameters);
				int num = (int)sqlParameters[5].Value;
				if (num == 0)
				{
					flag = true;
					tempID = (int)sqlParameters[3].Value;
					state = (int)sqlParameters[4].Value;
				}
				switch (num)
				{
				case 2:
				{
					msg = "ConsortiaBussiness.PassConsortiaApplyAlly.Msg2";
					bool result = flag;
					return result;
				}
				case 3:
				{
					msg = "ConsortiaBussiness.PassConsortiaApplyAlly.Msg3";
					bool result = flag;
					return result;
				}
				default:
				{
					bool result = flag;
					return result;
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
			return flag;
		}
		public bool PassConsortiaApplyUsers(int applyID, int userID, string userName, int consortiaID, ref string msg, ConsortiaUserInfo info, ref int consortiaRepute)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[24];
				sqlParameters[0] = new SqlParameter("@ID", applyID);
				sqlParameters[1] = new SqlParameter("@UserID", userID);
				sqlParameters[2] = new SqlParameter("@UserName", userName);
				sqlParameters[3] = new SqlParameter("@ConsortiaID", consortiaID);
				sqlParameters[4] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[4].Direction = ParameterDirection.ReturnValue;
				sqlParameters[5] = new SqlParameter("@tempID", SqlDbType.Int);
				sqlParameters[5].Direction = ParameterDirection.InputOutput;
				sqlParameters[5].Value = info.UserID;
				sqlParameters[6] = new SqlParameter("@tempName", SqlDbType.NVarChar, 100);
				sqlParameters[6].Direction = ParameterDirection.InputOutput;
				sqlParameters[6].Value = "";
				sqlParameters[7] = new SqlParameter("@tempDutyID", SqlDbType.Int);
				sqlParameters[7].Direction = ParameterDirection.InputOutput;
				sqlParameters[7].Value = info.DutyID;
				sqlParameters[8] = new SqlParameter("@tempDutyName", SqlDbType.NVarChar, 100);
				sqlParameters[8].Direction = ParameterDirection.InputOutput;
				sqlParameters[8].Value = "";
				sqlParameters[9] = new SqlParameter("@tempOffer", SqlDbType.Int);
				sqlParameters[9].Direction = ParameterDirection.InputOutput;
				sqlParameters[9].Value = info.Offer;
				sqlParameters[10] = new SqlParameter("@tempRichesOffer", SqlDbType.Int);
				sqlParameters[10].Direction = ParameterDirection.InputOutput;
				sqlParameters[10].Value = info.RichesOffer;
				sqlParameters[11] = new SqlParameter("@tempRichesRob", SqlDbType.Int);
				sqlParameters[11].Direction = ParameterDirection.InputOutput;
				sqlParameters[11].Value = info.RichesRob;
				sqlParameters[12] = new SqlParameter("@tempLastDate", SqlDbType.DateTime);
				sqlParameters[12].Direction = ParameterDirection.InputOutput;
				sqlParameters[12].Value = DateTime.Now;
				sqlParameters[13] = new SqlParameter("@tempWin", SqlDbType.Int);
				sqlParameters[13].Direction = ParameterDirection.InputOutput;
				sqlParameters[13].Value = info.Win;
				sqlParameters[14] = new SqlParameter("@tempTotal", SqlDbType.Int);
				sqlParameters[14].Direction = ParameterDirection.InputOutput;
				sqlParameters[14].Value = info.Total;
				sqlParameters[15] = new SqlParameter("@tempEscape", SqlDbType.Int);
				sqlParameters[15].Direction = ParameterDirection.InputOutput;
				sqlParameters[15].Value = info.Escape;
				sqlParameters[16] = new SqlParameter("@tempGrade", SqlDbType.Int);
				sqlParameters[16].Direction = ParameterDirection.InputOutput;
				sqlParameters[16].Value = info.Grade;
				sqlParameters[17] = new SqlParameter("@tempLevel", SqlDbType.Int);
				sqlParameters[17].Direction = ParameterDirection.InputOutput;
				sqlParameters[17].Value = info.Level;
				sqlParameters[18] = new SqlParameter("@tempCUID", SqlDbType.Int);
				sqlParameters[18].Direction = ParameterDirection.InputOutput;
				sqlParameters[18].Value = info.ID;
				sqlParameters[19] = new SqlParameter("@tempState", SqlDbType.Int);
				sqlParameters[19].Direction = ParameterDirection.InputOutput;
				sqlParameters[19].Value = info.State;
				sqlParameters[20] = new SqlParameter("@tempSex", SqlDbType.Bit);
				sqlParameters[20].Direction = ParameterDirection.InputOutput;
				sqlParameters[20].Value = info.Sex;
				sqlParameters[21] = new SqlParameter("@tempDutyRight", SqlDbType.Int);
				sqlParameters[21].Direction = ParameterDirection.InputOutput;
				sqlParameters[21].Value = info.Right;
				sqlParameters[22] = new SqlParameter("@tempConsortiaRepute", SqlDbType.Int);
				sqlParameters[22].Direction = ParameterDirection.InputOutput;
				sqlParameters[22].Value = consortiaRepute;
				sqlParameters[23] = new SqlParameter("@tempLoginName", SqlDbType.NVarChar, 200);
				sqlParameters[23].Direction = ParameterDirection.InputOutput;
				sqlParameters[23].Value = consortiaRepute;
				this.db.RunProcedure("SP_ConsortiaApplyUser_Pass", sqlParameters);
				int num = (int)sqlParameters[4].Value;
				flag = (num == 0);
				if (flag)
				{
					info.UserID = (int)sqlParameters[5].Value;
					info.UserName = sqlParameters[6].Value.ToString();
					info.DutyID = (int)sqlParameters[7].Value;
					info.DutyName = sqlParameters[8].Value.ToString();
					info.Offer = (int)sqlParameters[9].Value;
					info.RichesOffer = (int)sqlParameters[10].Value;
					info.RichesRob = (int)sqlParameters[11].Value;
					info.LastDate = (DateTime)sqlParameters[12].Value;
					info.Win = (int)sqlParameters[13].Value;
					info.Total = (int)sqlParameters[14].Value;
					info.Escape = (int)sqlParameters[15].Value;
					info.Grade = (int)sqlParameters[16].Value;
					info.Level = (int)sqlParameters[17].Value;
					info.ID = (int)sqlParameters[18].Value;
					info.State = (int)sqlParameters[19].Value;
					info.Sex = (bool)sqlParameters[20].Value;
					info.Right = (int)sqlParameters[21].Value;
					consortiaRepute = (int)sqlParameters[22].Value;
					info.LoginName = sqlParameters[23].Value.ToString();
				}
				switch (num)
				{
				case 2:
				{
					msg = "ConsortiaBussiness.PassConsortiaApplyUsers.Msg2";
					bool result = flag;
					return result;
				}
				case 3:
				{
					msg = "ConsortiaBussiness.PassConsortiaApplyUsers.Msg3";
					bool result = flag;
					return result;
				}
				case 4:
				case 5:
				{
					bool result = flag;
					return result;
				}
				case 6:
				{
					msg = "ConsortiaBussiness.PassConsortiaApplyUsers.Msg6";
					bool result = flag;
					return result;
				}
				default:
				{
					bool result = flag;
					return result;
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
			return flag;
		}
		public bool PassConsortiaInviteUsers(int inviteID, int userID, string userName, ref int consortiaID, ref string consortiaName, ref string msg, ConsortiaUserInfo info, ref int tempID, ref string tempName, ref int consortiaRepute)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[24];
				sqlParameters[0] = new SqlParameter("@ID", inviteID);
				sqlParameters[1] = new SqlParameter("@UserID", userID);
				sqlParameters[2] = new SqlParameter("@UserName", userName);
				sqlParameters[3] = new SqlParameter("@ConsortiaID", consortiaID);
				sqlParameters[3].Direction = ParameterDirection.InputOutput;
				sqlParameters[4] = new SqlParameter("@ConsortiaName", SqlDbType.NVarChar, 100);
				sqlParameters[4].Value = consortiaName;
				sqlParameters[4].Direction = ParameterDirection.InputOutput;
				sqlParameters[5] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[5].Direction = ParameterDirection.ReturnValue;
				sqlParameters[6] = new SqlParameter("@tempName", SqlDbType.NVarChar, 100);
				sqlParameters[6].Direction = ParameterDirection.InputOutput;
				sqlParameters[6].Value = tempName;
				sqlParameters[7] = new SqlParameter("@tempDutyID", SqlDbType.Int);
				sqlParameters[7].Direction = ParameterDirection.InputOutput;
				sqlParameters[7].Value = info.DutyID;
				sqlParameters[8] = new SqlParameter("@tempDutyName", SqlDbType.NVarChar, 100);
				sqlParameters[8].Direction = ParameterDirection.InputOutput;
				sqlParameters[8].Value = "";
				sqlParameters[9] = new SqlParameter("@tempOffer", SqlDbType.Int);
				sqlParameters[9].Direction = ParameterDirection.InputOutput;
				sqlParameters[9].Value = info.Offer;
				sqlParameters[10] = new SqlParameter("@tempRichesOffer", SqlDbType.Int);
				sqlParameters[10].Direction = ParameterDirection.InputOutput;
				sqlParameters[10].Value = info.RichesOffer;
				sqlParameters[11] = new SqlParameter("@tempRichesRob", SqlDbType.Int);
				sqlParameters[11].Direction = ParameterDirection.InputOutput;
				sqlParameters[11].Value = info.RichesRob;
				sqlParameters[12] = new SqlParameter("@tempLastDate", SqlDbType.DateTime);
				sqlParameters[12].Direction = ParameterDirection.InputOutput;
				sqlParameters[12].Value = DateTime.Now;
				sqlParameters[13] = new SqlParameter("@tempWin", SqlDbType.Int);
				sqlParameters[13].Direction = ParameterDirection.InputOutput;
				sqlParameters[13].Value = info.Win;
				sqlParameters[14] = new SqlParameter("@tempTotal", SqlDbType.Int);
				sqlParameters[14].Direction = ParameterDirection.InputOutput;
				sqlParameters[14].Value = info.Total;
				sqlParameters[15] = new SqlParameter("@tempEscape", SqlDbType.Int);
				sqlParameters[15].Direction = ParameterDirection.InputOutput;
				sqlParameters[15].Value = info.Escape;
				sqlParameters[16] = new SqlParameter("@tempID", SqlDbType.Int);
				sqlParameters[16].Direction = ParameterDirection.InputOutput;
				sqlParameters[16].Value = tempID;
				sqlParameters[17] = new SqlParameter("@tempGrade", SqlDbType.Int);
				sqlParameters[17].Direction = ParameterDirection.InputOutput;
				sqlParameters[17].Value = info.Level;
				sqlParameters[18] = new SqlParameter("@tempLevel", SqlDbType.Int);
				sqlParameters[18].Direction = ParameterDirection.InputOutput;
				sqlParameters[18].Value = info.Level;
				sqlParameters[19] = new SqlParameter("@tempCUID", SqlDbType.Int);
				sqlParameters[19].Direction = ParameterDirection.InputOutput;
				sqlParameters[19].Value = info.ID;
				sqlParameters[20] = new SqlParameter("@tempState", SqlDbType.Int);
				sqlParameters[20].Direction = ParameterDirection.InputOutput;
				sqlParameters[20].Value = info.State;
				sqlParameters[21] = new SqlParameter("@tempSex", SqlDbType.Bit);
				sqlParameters[21].Direction = ParameterDirection.InputOutput;
				sqlParameters[21].Value = info.Sex;
				sqlParameters[22] = new SqlParameter("@tempRight", SqlDbType.Int);
				sqlParameters[22].Direction = ParameterDirection.InputOutput;
				sqlParameters[22].Value = info.Right;
				sqlParameters[23] = new SqlParameter("@tempConsortiaRepute", SqlDbType.Int);
				sqlParameters[23].Direction = ParameterDirection.InputOutput;
				sqlParameters[23].Value = consortiaRepute;
				this.db.RunProcedure("SP_ConsortiaInviteUser_Pass", sqlParameters);
				int num = (int)sqlParameters[5].Value;
				flag = (num == 0);
				if (flag)
				{
					consortiaID = (int)sqlParameters[3].Value;
					consortiaName = sqlParameters[4].Value.ToString();
					tempName = sqlParameters[6].Value.ToString();
					info.DutyID = (int)sqlParameters[7].Value;
					info.DutyName = sqlParameters[8].Value.ToString();
					info.Offer = (int)sqlParameters[9].Value;
					info.RichesOffer = (int)sqlParameters[10].Value;
					info.RichesRob = (int)sqlParameters[11].Value;
					info.LastDate = (DateTime)sqlParameters[12].Value;
					info.Win = (int)sqlParameters[13].Value;
					info.Total = (int)sqlParameters[14].Value;
					info.Escape = (int)sqlParameters[15].Value;
					tempID = (int)sqlParameters[16].Value;
					info.Grade = (int)sqlParameters[17].Value;
					info.Level = (int)sqlParameters[18].Value;
					info.ID = (int)sqlParameters[19].Value;
					info.State = (int)sqlParameters[20].Value;
					info.Sex = (bool)sqlParameters[21].Value;
					info.Right = (int)sqlParameters[22].Value;
					consortiaRepute = (int)sqlParameters[23].Value;
				}
				int num2 = num;
				if (num2 != 3)
				{
					if (num2 == 6)
					{
						msg = "ConsortiaBussiness.PassConsortiaInviteUsers.Msg6";
					}
					return flag;
				}
				msg = "ConsortiaBussiness.PassConsortiaInviteUsers.Msg3";
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
		public bool RenameConsortia(int ConsortiaID, string nickName, string newNickName)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", ConsortiaID),
					new SqlParameter("@NickName", nickName),
					new SqlParameter("@NewNickName", newNickName),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[3].Direction = ParameterDirection.ReturnValue;
				flag = this.db.RunProcedure("SP_Users_RenameConsortia", sqlParameters);
				int num = (int)sqlParameters[3].Value;
				flag = (num == 0);
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
		public bool RenameConsortiaName(string userName, string nickName, string consortiaName, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@UserName", userName),
					new SqlParameter("@NickName", nickName),
					new SqlParameter("@ConsortiaName", consortiaName),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[3].Direction = ParameterDirection.ReturnValue;
				flag = this.db.RunProcedure("SP_Users_RenameConsortiaName", sqlParameters);
				int num = (int)sqlParameters[3].Value;
				flag = (num == 0);
				switch (num)
				{
				case 4:
				case 5:
				{
					msg = LanguageMgr.GetTranslation("PlayerBussiness.SP_Users_RenameConsortiaName.Msg4", new object[0]);
					bool result = flag;
					return result;
				}
				default:
				{
					bool result = flag;
					return result;
				}
				}
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
		public bool ScanConsortia(ref string noticeID)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[2];
				sqlParameters[0] = new SqlParameter("@NoticeID", SqlDbType.NVarChar, 4000);
				sqlParameters[0].Direction = ParameterDirection.Output;
				sqlParameters[1] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[1].Direction = ParameterDirection.ReturnValue;
				flag = this.db.RunProcedure("SP_Consortia_Scan", sqlParameters);
				int num = (int)sqlParameters[1].Value;
				flag = (num == 0);
				if (flag)
				{
					noticeID = sqlParameters[0].Value.ToString();
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
		public bool UpdateConsortiaChairman(string nickName, int consortiaID, int userID, ref string msg, ref ConsortiaDutyInfo info, ref int tempUserID, ref string tempUserName)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[9];
				sqlParameters[0] = new SqlParameter("@NickName", nickName);
				sqlParameters[1] = new SqlParameter("@ConsortiaID", consortiaID);
				sqlParameters[2] = new SqlParameter("@UserID", userID);
				sqlParameters[3] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[3].Direction = ParameterDirection.ReturnValue;
				sqlParameters[4] = new SqlParameter("@tempUserID", SqlDbType.Int);
				sqlParameters[4].Direction = ParameterDirection.InputOutput;
				sqlParameters[4].Value = tempUserID;
				sqlParameters[5] = new SqlParameter("@tempUserName", SqlDbType.NVarChar, 100);
				sqlParameters[5].Direction = ParameterDirection.InputOutput;
				sqlParameters[5].Value = tempUserName;
				sqlParameters[6] = new SqlParameter("@tempDutyLevel", SqlDbType.Int);
				sqlParameters[6].Direction = ParameterDirection.InputOutput;
				sqlParameters[6].Value = info.Level;
				sqlParameters[7] = new SqlParameter("@tempDutyName", SqlDbType.NVarChar, 100);
				sqlParameters[7].Direction = ParameterDirection.InputOutput;
				sqlParameters[7].Value = "";
				sqlParameters[8] = new SqlParameter("@tempRight", SqlDbType.Int);
				sqlParameters[8].Direction = ParameterDirection.InputOutput;
				sqlParameters[8].Value = info.Right;
				flag = this.db.RunProcedure("SP_ConsortiaChangeChairman", sqlParameters);
				int num = (int)sqlParameters[3].Value;
				flag = (num == 0);
				if (flag)
				{
					tempUserID = (int)sqlParameters[4].Value;
					tempUserName = sqlParameters[5].Value.ToString();
					info.Level = (int)sqlParameters[6].Value;
					info.DutyName = sqlParameters[7].Value.ToString();
					info.Right = (int)sqlParameters[8].Value;
				}
				switch (num)
				{
				case 1:
				{
					msg = "ConsortiaBussiness.UpdateConsortiaChairman.Msg3";
					bool result = flag;
					return result;
				}
				case 2:
				{
					msg = "ConsortiaBussiness.UpdateConsortiaChairman.Msg2";
					bool result = flag;
					return result;
				}
				default:
				{
					bool result = flag;
					return result;
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
			return flag;
		}
		public bool UpdateConsortiaDescription(int consortiaID, int userID, string description, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Description", description),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[3].Direction = ParameterDirection.ReturnValue;
				flag = this.db.RunProcedure("SP_ConsortiaDescription_Update", sqlParameters);
				int num = (int)sqlParameters[3].Value;
				flag = (num == 0);
				int num2 = num;
				if (num2 == 2)
				{
					msg = "ConsortiaBussiness.UpdateConsortiaDescription.Msg2";
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
		public bool UpdateConsortiaDuty(ConsortiaDutyInfo info, int userID, int updateType, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[8];
				sqlParameters[0] = new SqlParameter("@DutyID", info.DutyID);
				sqlParameters[0].Direction = ParameterDirection.InputOutput;
				sqlParameters[1] = new SqlParameter("@ConsortiaID", info.ConsortiaID);
				sqlParameters[2] = new SqlParameter("@DutyName", SqlDbType.NVarChar, 100);
				sqlParameters[2].Direction = ParameterDirection.InputOutput;
				sqlParameters[2].Value = info.DutyName;
				sqlParameters[3] = new SqlParameter("@Right", SqlDbType.Int);
				sqlParameters[3].Direction = ParameterDirection.InputOutput;
				sqlParameters[3].Value = info.Right;
				sqlParameters[4] = new SqlParameter("@Level", SqlDbType.Int);
				sqlParameters[4].Direction = ParameterDirection.InputOutput;
				sqlParameters[4].Value = info.Level;
				sqlParameters[5] = new SqlParameter("@UserID", userID);
				sqlParameters[6] = new SqlParameter("@UpdateType", updateType);
				sqlParameters[7] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[7].Direction = ParameterDirection.ReturnValue;
				flag = this.db.RunProcedure("SP_ConsortiaDuty_Update", sqlParameters);
				int num = (int)sqlParameters[7].Value;
				flag = (num == 0);
				if (flag)
				{
					info.DutyID = (int)sqlParameters[0].Value;
					info.DutyName = ((sqlParameters[2].Value == null) ? "" : sqlParameters[2].Value.ToString());
					info.Right = (int)sqlParameters[3].Value;
					info.Level = (int)sqlParameters[4].Value;
				}
				switch (num)
				{
				case 2:
				{
					msg = "ConsortiaBussiness.UpdateConsortiaDuty.Msg2";
					bool result = flag;
					return result;
				}
				case 3:
				case 4:
				{
					msg = "ConsortiaBussiness.UpdateConsortiaDuty.Msg3";
					bool result = flag;
					return result;
				}
				case 5:
				{
					msg = "ConsortiaBussiness.DeleteConsortiaDuty.Msg5";
					bool result = flag;
					return result;
				}
				default:
				{
					bool result = flag;
					return result;
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
			return flag;
		}
		public bool UpdateConsortiaIsBanChat(int banUserID, int consortiaID, int userID, bool isBanChat, ref int tempID, ref string tempName, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[7];
				sqlParameters[0] = new SqlParameter("@ID", banUserID);
				sqlParameters[1] = new SqlParameter("@ConsortiaID", consortiaID);
				sqlParameters[2] = new SqlParameter("@UserID", userID);
				sqlParameters[3] = new SqlParameter("@IsBanChat", isBanChat);
				sqlParameters[4] = new SqlParameter("@TempID", tempID);
				sqlParameters[4].Direction = ParameterDirection.InputOutput;
				sqlParameters[5] = new SqlParameter("@TempName", SqlDbType.NVarChar, 100);
				sqlParameters[5].Value = tempName;
				sqlParameters[5].Direction = ParameterDirection.InputOutput;
				sqlParameters[6] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[6].Direction = ParameterDirection.ReturnValue;
				flag = this.db.RunProcedure("SP_ConsortiaIsBanChat_Update", sqlParameters);
				int num = (int)sqlParameters[6].Value;
				tempID = (int)sqlParameters[4].Value;
				tempName = sqlParameters[5].Value.ToString();
				flag = (num == 0);
				switch (num)
				{
				case 2:
				{
					msg = "ConsortiaBussiness.UpdateConsortiaIsBanChat.Msg2";
					bool result = flag;
					return result;
				}
				case 3:
				{
					msg = "ConsortiaBussiness.UpdateConsortiaIsBanChat.Msg3";
					bool result = flag;
					return result;
				}
				default:
				{
					bool result = flag;
					return result;
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
			return flag;
		}
		public bool UpdateConsortiaPlacard(int consortiaID, int userID, string placard, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Placard", placard),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[3].Direction = ParameterDirection.ReturnValue;
				flag = this.db.RunProcedure("SP_ConsortiaPlacard_Update", sqlParameters);
				int num = (int)sqlParameters[3].Value;
				flag = (num == 0);
				int num2 = num;
				if (num2 == 2)
				{
					msg = "ConsortiaBussiness.UpdateConsortiaPlacard.Msg2";
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
		public bool UpdateConsortiaRiches(int consortiaID, int userID, int Riches, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Riches", Riches),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[3].Direction = ParameterDirection.ReturnValue;
				flag = this.db.RunProcedure("SP_ConsortiaRiches_Update", sqlParameters);
				int num = (int)sqlParameters[3].Value;
				flag = (num == 0);
				int num2 = num;
				if (num2 == 2)
				{
					msg = "ConsortiaBussiness.UpdateConsortiaRiches.Msg2";
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
		public bool UpdateConsortiaUserGrade(int id, int consortiaID, int userID, bool upGrade, ref string msg, ref ConsortiaDutyInfo info, ref string tempUserName)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[9];
				sqlParameters[0] = new SqlParameter("@ID", id);
				sqlParameters[1] = new SqlParameter("@ConsortiaID", consortiaID);
				sqlParameters[2] = new SqlParameter("@UserID", userID);
				sqlParameters[3] = new SqlParameter("@UpGrade", upGrade);
				sqlParameters[4] = new SqlParameter("@Result", SqlDbType.Int);
				sqlParameters[4].Direction = ParameterDirection.ReturnValue;
				sqlParameters[5] = new SqlParameter("@tempUserName", SqlDbType.NVarChar, 100);
				sqlParameters[5].Direction = ParameterDirection.InputOutput;
				sqlParameters[5].Value = tempUserName;
				sqlParameters[6] = new SqlParameter("@tempDutyLevel", SqlDbType.Int);
				sqlParameters[6].Direction = ParameterDirection.InputOutput;
				sqlParameters[6].Value = info.Level;
				sqlParameters[7] = new SqlParameter("@tempDutyName", SqlDbType.NVarChar, 100);
				sqlParameters[7].Direction = ParameterDirection.InputOutput;
				sqlParameters[7].Value = "";
				sqlParameters[8] = new SqlParameter("@tempRight", SqlDbType.Int);
				sqlParameters[8].Direction = ParameterDirection.InputOutput;
				sqlParameters[8].Value = info.Right;
				flag = this.db.RunProcedure("SP_ConsortiaUserGrade_Update", sqlParameters);
				int num = (int)sqlParameters[4].Value;
				flag = (num == 0);
				if (flag)
				{
					tempUserName = sqlParameters[5].Value.ToString();
					info.Level = (int)sqlParameters[6].Value;
					info.DutyName = sqlParameters[7].Value.ToString();
					info.Right = (int)sqlParameters[8].Value;
				}
				switch (num)
				{
				case 2:
				{
					msg = "ConsortiaBussiness.UpdateConsortiaUserGrade.Msg2";
					bool result = flag;
					return result;
				}
				case 3:
				{
					msg = (upGrade ? "ConsortiaBussiness.UpdateConsortiaUserGrade.Msg3" : "ConsortiaBussiness.UpdateConsortiaUserGrade.Msg10");
					bool result = flag;
					return result;
				}
				case 4:
				{
					msg = "ConsortiaBussiness.UpdateConsortiaUserGrade.Msg4";
					bool result = flag;
					return result;
				}
				case 5:
				{
					msg = "ConsortiaBussiness.UpdateConsortiaUserGrade.Msg5";
					bool result = flag;
					return result;
				}
				default:
				{
					bool result = flag;
					return result;
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
			return flag;
		}
		public bool UpdateConsortiaUserRemark(int id, int consortiaID, int userID, string remark, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ID", id),
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Remark", remark),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[4].Direction = ParameterDirection.ReturnValue;
				flag = this.db.RunProcedure("SP_ConsortiaUserRemark_Update", sqlParameters);
				int num = (int)sqlParameters[4].Value;
				flag = (num == 0);
				int num2 = num;
				if (num2 == 2)
				{
					msg = "ConsortiaBussiness.UpdateConsortiaUserRemark.Msg2";
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
		public bool UpdateConsotiaApplyState(int consortiaID, int userID, bool state, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@State", state),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[3].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_Consortia_Apply_State", sqlParameters);
				int num = (int)sqlParameters[3].Value;
				flag = (num == 0);
				int num2 = num;
				if (num2 == 2)
				{
					msg = "ConsortiaBussiness.UpdateConsotiaApplyState.Msg2";
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
		public bool UpGradeConsortia(int consortiaID, int userID, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[2].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_Consortia_UpGrade", sqlParameters);
				int num = (int)sqlParameters[2].Value;
				flag = (num == 0);
				switch (num)
				{
				case 2:
				{
					msg = "ConsortiaBussiness.UpGradeConsortia.Msg2";
					bool result = flag;
					return result;
				}
				case 3:
				{
					msg = "ConsortiaBussiness.UpGradeConsortia.Msg3";
					bool result = flag;
					return result;
				}
				case 4:
				{
					msg = "ConsortiaBussiness.UpGradeConsortia.Msg4";
					bool result = flag;
					return result;
				}
				default:
				{
					bool result = flag;
					return result;
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
			return flag;
		}
		public bool UpGradeShopConsortia(int consortiaID, int userID, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[2].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_Consortia_Shop_UpGrade", sqlParameters);
				int num = (int)sqlParameters[2].Value;
				flag = (num == 0);
				switch (num)
				{
				case 2:
				{
					msg = "ConsortiaBussiness.UpGradeShopConsortia.Msg2";
					bool result = flag;
					return result;
				}
				case 3:
				{
					msg = "ConsortiaBussiness.UpGradeShopConsortia.Msg3";
					bool result = flag;
					return result;
				}
				case 4:
				{
					msg = "ConsortiaBussiness.UpGradeShopConsortia.Msg4";
					bool result = flag;
					return result;
				}
				default:
				{
					bool result = flag;
					return result;
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
			return flag;
		}
		public bool UpGradeSkillConsortia(int consortiaID, int userID, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[2].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_Consortia_Skill_UpGrade", sqlParameters);
				int num = (int)sqlParameters[2].Value;
				flag = (num == 0);
				switch (num)
				{
				case 2:
				{
					msg = "ConsortiaBussiness.UpGradeSkillConsortia.Msg2";
					bool result = flag;
					return result;
				}
				case 3:
				{
					msg = "ConsortiaBussiness.UpGradeSkillConsortia.Msg3";
					bool result = flag;
					return result;
				}
				case 4:
				{
					msg = "ConsortiaBussiness.UpGradeSkillConsortia.Msg4";
					bool result = flag;
					return result;
				}
				default:
				{
					bool result = flag;
					return result;
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
			return flag;
		}
		public bool UpGradeSmithConsortia(int consortiaID, int userID, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[2].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_Consortia_Smith_UpGrade", sqlParameters);
				int num = (int)sqlParameters[2].Value;
				flag = (num == 0);
				switch (num)
				{
				case 2:
				{
					msg = "ConsortiaBussiness.UpGradeSmithConsortia.Msg2";
					bool result = flag;
					return result;
				}
				case 3:
				{
					msg = "ConsortiaBussiness.UpGradeSmithConsortia.Msg3";
					bool result = flag;
					return result;
				}
				case 4:
				{
					msg = "ConsortiaBussiness.UpGradeSmithConsortia.Msg4";
					bool result = flag;
					return result;
				}
				default:
				{
					bool result = flag;
					return result;
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
			return flag;
		}
		public bool UpGradeStoreConsortia(int consortiaID, int userID, ref string msg)
		{
			bool flag = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				sqlParameters[2].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_Consortia_Store_UpGrade", sqlParameters);
				int num = (int)sqlParameters[2].Value;
				flag = (num == 0);
				switch (num)
				{
				case 2:
				{
					msg = "ConsortiaBussiness.UpGradeStoreConsortia.Msg2";
					bool result = flag;
					return result;
				}
				case 3:
				{
					msg = "ConsortiaBussiness.UpGradeStoreConsortia.Msg3";
					bool result = flag;
					return result;
				}
				case 4:
				{
					msg = "ConsortiaBussiness.UpGradeStoreConsortia.Msg4";
					bool result = flag;
					return result;
				}
				default:
				{
					bool result = flag;
					return result;
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
			return flag;
		}
	}
}
