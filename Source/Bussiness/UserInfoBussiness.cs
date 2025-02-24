namespace Bussiness
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class UserInfoBussiness : BaseBussiness
    {
        public bool AddUserInfo(string uid, string userName, string portrait)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@Uid", uid), new SqlParameter("@UserName", userName), new SqlParameter("@Portrait", portrait), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[3].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_User_Info_Insert", sqlParameters);
                int num = (int) sqlParameters[3].Value;
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

        public bool GetFromDbByUid(string uid, ref string userName, ref string portrait)
        {
            SqlDataReader resultDataReader = null;
            bool flag;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@Uid", uid) };
                base.db.GetReader(ref resultDataReader, "SP_User_Info_QueryByUid", sqlParameters);
                while (resultDataReader.Read())
                {
                    userName = (resultDataReader["UserName"] == null) ? "" : resultDataReader["UserName"].ToString();
                    portrait = (resultDataReader["Portrait"] == null) ? "" : resultDataReader["Portrait"].ToString();
                }
                return !(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(portrait));
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", exception);
                }
                flag = false;
            }
            finally
            {
                if (!((resultDataReader == null) || resultDataReader.IsClosed))
                {
                    resultDataReader.Close();
                }
            }
            return flag;
        }
    }
}

