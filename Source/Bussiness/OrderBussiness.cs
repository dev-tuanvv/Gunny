namespace Bussiness
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class OrderBussiness : BaseBussiness
    {
        public bool AddOrder(string order, double amount, string username, string payWay, string serverId)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@Order", order), new SqlParameter("@Amount", amount), new SqlParameter("@Username", username), new SqlParameter("@PayWay", payWay), new SqlParameter("@ServerId", serverId), new SqlParameter("@Result", SqlDbType.Int) };
                sqlParameters[5].Direction = ParameterDirection.ReturnValue;
                base.db.RunProcedure("SP_Charge_Order", sqlParameters);
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

        public string GetOrderToName(string order, ref string serverId)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@Order", order) };
                base.db.GetReader(ref resultDataReader, "SP_Charge_Order_Single", sqlParameters);
                if (resultDataReader.Read())
                {
                    serverId = (resultDataReader["ServerId"] == null) ? "" : resultDataReader["ServerId"].ToString();
                    return ((resultDataReader["UserName"] == null) ? "" : resultDataReader["UserName"].ToString());
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
            return "";
        }
    }
}

