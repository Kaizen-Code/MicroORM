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


        int ExecuteNonQuery(string sql, SqlParameter[] parameters, SqlParameter returnParameter, CommandType cmdType);
        int ExecuteNonQuery(string sql, object parameters, bool isStoredProcedure = true);
        

        object ExecuteScalar(string sql, SqlParameter[] sqlParameters, CommandType cmdType);
        object ExecuteScalar(string sql, object parameters, bool isStoredProcedure = true);


        IEnumerable<T> FindAll<T>(string sql, SqlParameter[] sqlParameters, CommandType cmdType) where T : new();
        IEnumerable<T> FindAll<T>(string sql, object parameters, bool isStoredProcedure = true) where T : new();


        IEnumerable<T> QueringData<T>(string sql, SqlParameter[] sqlParameters, CommandType cmdType) where T : new();
        IEnumerable<T> QueringData<T>(string sql, object parameters, bool isStoredProcedure = true) where T : new();


        T FirstOrDefault<T>(string sql, SqlParameter[] sqlParameters, CommandType cmdType) where T : class, new();
        T FirstOrDefault<T>(string sql, object parameters, bool isStoredProcedure = true) where T : class, new();


        DataTable GetDataTable(string sql, SqlParameter[] sqlParameters, CommandType cmdType);
        DataTable GetDataTable(string sql, object parameters, bool isStoredProcedure = true);


        void BulkInsert(DataTable table);

    }
}
