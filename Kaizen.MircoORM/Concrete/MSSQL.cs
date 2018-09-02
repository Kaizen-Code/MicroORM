using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kaizen.MircoORM.Concrete
{
    public class MSSQL : IDataService
    {
        static string _connenctionString;
        
        MSSQL()
        {
            _connenctionString = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
        }

        public void BulkInsert(DataTable table)
        {
            throw new NotImplementedException();
        }
        private SqlParameter[] createParameters(object parameters)
        {
            if (parameters == null)
                return null;
            var propInfos = parameters.GetType().GetProperties();
            var paramArray = new SqlParameter[propInfos.Length];
            var i = 0;
            foreach (var item in propInfos)
            {
                paramArray[i++] = new SqlParameter($"@{item.Name}", item.GetValue(parameters, null));
            }
            return paramArray;
        }
        private int executeNonQuery(string sql,SqlParameter[] parameters,CommandType type)
        {
            using (SqlConnection con = new SqlConnection(_connenctionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                if(parameters != null)
                    cmd.Parameters.AddRange(parameters);
                cmd.CommandType = type;
                con.Open();
                var ans = cmd.ExecuteNonQuery();
                con.Close();
                return ans;
            }
        }
        private object executeScalar(string sql, SqlParameter[] parameters, CommandType type)
        {
            using (SqlConnection con = new SqlConnection(_connenctionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                cmd.CommandType = type;
                con.Open();
                var ans = cmd.ExecuteScalar();
                con.Close();
                return ans;
            }
        }


        public int ExecuteNonQuery(string sql, object parameters, bool isStoredProcedure = true)
        {
            var sqlParams = this.createParameters(parameters);
            var cmdType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            return this.executeNonQuery(sql, sqlParams, cmdType);
        }

        public int ExecuteNonQuery(string sql, SqlParameter[] parameters,SqlParameter returnParameter, bool isStoredProcedure = true)
        {
            throw new NotImplementedException();
        }

        public object ExecuteScalar(string sql, object parameters, bool isStoredProcedure = true)
        {
            var sqlParams = this.createParameters(parameters);
            var cmdType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            return this.executeScalar(sql, sqlParams, cmdType);
        }
        private IEnumerable<SqlDataReader> reading(string sql,SqlParameter[] sqlParameters,CommandType cmdType)
        {
            using (SqlConnection con = new SqlConnection(_connenctionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.CommandType = CommandType.Text;
                if (sqlParameters != null)
                    cmd.Parameters.AddRange(sqlParameters);
                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                        while (reader.Read())
                        {
                            yield return reader;
                        }
                    reader.Close();
                }
                con.Close();
            }
        }
        private IEnumerable<T> findAll<T>(string sql,SqlParameter[] sqlParameters,CommandType cmdType) where T: new()
        {
            var props = typeof(T).GetProperties();
            var data = this.reading(sql, sqlParameters, cmdType);
            foreach (var item in data)
            {
                var obj = new T();
                foreach (var p in props)
                {
                    p.SetValue(obj, item[p.Name],null);
                }
                yield return obj;
            }
        }
        private IEnumerable<T> QueringData<T>(string sql, SqlParameter[] sqlParameters, CommandType cmdType) where T : new()
        {
            
            PropertyInfo[] propInfos = null;
            var data = this.reading(sql, sqlParameters, cmdType);
            foreach (var item in data)
            {
                if(propInfos == null)
                {
                    propInfos = new PropertyInfo[item.FieldCount];
                    var type = typeof(T);
                    for (int i = 0; i < item.FieldCount; i++)
                    {
                        propInfos[i] = type.GetProperty(item.GetName(i));
                    }
                }
                var obj = new T();
                for (int i = 0; i < propInfos.Length; i++)
                {
                    propInfos[i].SetValue(obj, item[i], null);
                }
                yield return obj;
            }
        }
        public IEnumerable<T> FindAll<T>(string sql, object parameters, bool isStoredProcedure = true) where T : class, new()
        {
            var sqlParams = this.createParameters(parameters);
            var cmdType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            return this.findAll<T>(sql, sqlParams, cmdType);
        }

        public T FirstOrDefault<T>(string sql, object parameters, bool isStoredProcedure = true) where T : class, new()
        {
            T ans = null;
            var sqlParams = this.createParameters(parameters);
            var cmdType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            foreach (var item in this.findAll<T>(sql, sqlParams, cmdType))
            {
                ans = item;
                break;
            }
            return ans;
        }

        public DataTable GetDataTable(string sql, object parameters, bool isStoredProcedure = true)
        {
            throw new NotImplementedException();
        }

        public object GetSingle(string sql, object parameters, bool isStoredProcedure = true)
        {
            throw new NotImplementedException();
        }
    }
}
