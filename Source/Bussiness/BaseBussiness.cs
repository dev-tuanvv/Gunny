namespace Bussiness
{
    using log4net;
    using SqlDataProvider.BaseClass;
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Reflection;

    public class BaseBussiness : IDisposable
    {
        protected Sql_DbObject db = new Sql_DbObject("AppConfig", "conString");
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Dispose()
        {
            this.db.Dispose();
            GC.SuppressFinalize(this);
        }

        public DataTable GetPage(string queryStr, string queryWhere, int pageCurrent, int pageSize, string fdShow, string fdOreder, string fdKey, ref int total)
        {
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[] { new SqlParameter("@QueryStr", queryStr), new SqlParameter("@QueryWhere", queryWhere), new SqlParameter("@PageSize", pageSize), new SqlParameter("@PageCurrent", pageCurrent), new SqlParameter("@FdShow", fdShow), new SqlParameter("@FdOrder", fdOreder), new SqlParameter("@FdKey", fdKey), new SqlParameter("@TotalRow", (int) total) };
                sqlParameters[7].Direction = ParameterDirection.Output;
                DataTable table = this.db.GetDataTable(queryStr, "SP_CustomPage", sqlParameters, 120);
                total = (int) sqlParameters[7].Value;
                return table;
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Init", exception);
                }
            }
            return new DataTable(queryStr);
        }
    }
}

