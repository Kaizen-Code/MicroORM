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
    class MSSQL : IDataService
    {
        static string _connenctionString;
        
        public MSSQL()
        {
            _connenctionString = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
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
        private IEnumerable<SqlDataReader> reading(string sql, SqlParameter[] sqlParameters, CommandType cmdType)
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


        public int ExecuteNonQuery(string sql, SqlParameter[] parameters, SqlParameter returnParameter, CommandType cmdType)
        {
            throw new NotImplementedException();
        }
        public int ExecuteNonQuery(string sql, object parameters, bool isStoredProcedure = true)
        {
            var sqlParams = this.createParameters(parameters);
            var cmdType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            return this.executeNonQuery(sql, sqlParams, cmdType);
        }

        

        
        public object ExecuteScalar(string sql, SqlParameter[] sqlParameters, CommandType cmdType) => this.ExecuteScalar(sql, sqlParameters, cmdType);
        public object ExecuteScalar(string sql, object parameters, bool isStoredProcedure = true)
        {
            var sqlParams = this.createParameters(parameters);
            var cmdType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            return this.executeScalar(sql, sqlParams, cmdType);
        }

        public IEnumerable<T> FindAll<T>(string sql, SqlParameter[] sqlParameters, CommandType cmdType) where T : new()
        {
            var props = typeof(T).GetProperties();
            var data = this.reading(sql, sqlParameters, cmdType);
            foreach (var item in data)
            {
                var obj = new T();
                foreach (var p in props)
                {
                    p.SetValue(obj, item[p.Name], null);
                }
                yield return obj;
            }
        }
        public IEnumerable<T> FindAll<T>(string sql, object parameters, bool isStoredProcedure = true) where T : new()
        {
            var sqlParams = this.createParameters(parameters);
            var cmdType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            return this.FindAll<T>(sql, sqlParams, cmdType);
        }

        public IEnumerable<T> QueringData<T>(string sql, SqlParameter[] sqlParameters, CommandType cmdType) where T : new()
        {
            PropertyInfo[] propInfos = null;
            var data = this.reading(sql, sqlParameters, cmdType);
            foreach (var item in data)
            {
                if (propInfos == null)
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
        public IEnumerable<T> QueringData<T>(string sql, object parameters, bool isStoredProcedure = true) where T : new()
        {
            var sqlParams = this.createParameters(parameters);
            var cmdType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            return this.QueringData<T>(sql, sqlParams, cmdType);
        }
        public T FirstOrDefault<T>(string sql, SqlParameter[] sqlParameters, CommandType cmdType) where T : class, new()
        {
            T ans = null;
            foreach (var item in this.FindAll<T>(sql, sqlParameters, cmdType))
            {
                ans = item;
                break;
            }
            return ans;
        }
        public T FirstOrDefault<T>(string sql, object parameters, bool isStoredProcedure = true) where T : class, new()
        {
            T ans = null;
            foreach (var item in this.FindAll<T>(sql, parameters, isStoredProcedure))
            {
                ans = item;
                break;
            }
            return ans;
        }
        public DataTable GetDataTable(string sql, SqlParameter[] sqlParameters, CommandType cmdType)
        {
            throw new NotImplementedException();
        }
        public DataTable GetDataTable(string sql, object parameters, bool isStoredProcedure = true)
        {
            throw new NotImplementedException();
        }
        public void BulkInsert(DataTable table)
        {
            throw new NotImplementedException();
        }

    }
}
