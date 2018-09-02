using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaizen.MircoORM
{
    public interface IDataService
    {

        IEnumerable<T> FindAll<T>(string sql,object parameters, bool isStoredProcedure = true ) where T : class, new();

        T FirstOrDefault<T>(string sql,object parameters, bool isStoredProcedure = true) where T : class, new();

        object GetSingle(string sql, object parameters, bool isStoredProcedure = true);

        int ExecuteNonQuery(string sql, object parameters, bool isStoredProcedure = true);
        int ExecuteNonQuery(string sql, SqlParameter[] parameters, SqlParameter returnParameter, bool isStoredProcedure = true);

        object ExecuteScalar(string sql, object parameters, bool isStoredProcedure = true);

        DataTable GetDataTable(string sql, object parameters, bool isStoredProcedure = true);

        void BulkInsert(DataTable table);

    }
}
