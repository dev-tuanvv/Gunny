namespace Bussiness
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class CookieInfoBussiness : BaseBussiness
    {
        public bool AddCookieInfo(string bdSigUser, string bdSigPortrait, string bdSigSessionKey)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@BdSigUser", bdSigUser), new SqlParameter("@BdSigPortrait", bdSigPortrait), new SqlParameter("@BdSigSessionKey", bdSigSessionKey), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[3].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_Cookie_Info_Insert", sqlParameters);
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

        public bool GetFromDbByUser(string bdSigUser, ref string bdSigPortrait, ref string bdSigSessionKey)
        {
            SqlDataReader resultDataReader = null;
            bool flag;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@BdSigUser", bdSigUser) };
                base.db.GetReader(ref resultDataReader, "SP_Cookie_Info_QueryByUser", sqlParameters);
                while (resultDataReader.Read())
                {
                    bdSigPortrait = (resultDataReader["BdSigPortrait"] == null) ? "" : resultDataReader["BdSigPortrait"].ToString();
                    bdSigSessionKey = (resultDataReader["BdSigSessionKey"] == null) ? "" : resultDataReader["BdSigSessionKey"].ToString();
                }
                return !(string.IsNullOrEmpty(bdSigPortrait) || string.IsNullOrEmpty(bdSigSessionKey));
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

