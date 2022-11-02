using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisStackTest.Service
{
    public class DapperUtil
    {
        private readonly IConfiguration _Configuration;

        public DapperUtil(IConfiguration configuration )
        {
            _Configuration = configuration;

        }
        private SqlConnection GetSqlConnection()
        {
            return new SqlConnection(_Configuration.GetConnectionString("conn"));
        }
        public void DapperExecute<T>(string strSql, T data)
        {
            using (var conn = GetSqlConnection()) {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    conn.Execute(strSql, data,trans);
                    trans.Commit();
                }
            }
        }
        public async Task DapperExecuteAsync<T>(string strSql, T data)
        {
            using (var conn = GetSqlConnection())
            {
                await conn.OpenAsync();
                using (var trans = await conn.BeginTransactionAsync())
                {
                    await conn.ExecuteAsync(strSql, data, trans);
                    await trans.CommitAsync();
                }
            }
        }

        public IEnumerable<T> DapperQuery<T>(string strSql, T data)
        {
            using (var conn = GetSqlConnection())
            {
                IEnumerable<T> result = conn.Query<T>(strSql, data);
                return result;
            }
        }

        public IEnumerable<T> DapperTableLinkList<T, T2>(string strSql, T2 data)
        {
            using (var conn = GetSqlConnection())
            {
                IEnumerable<T> result = conn.Query<T>(strSql, data);
                return result;
            }
        }

        public T DapperTableLinkSingle<T, T2>(string strSql, T2 data)
        {
            using (var conn = GetSqlConnection())
            {
                T result = conn.QuerySingle<T>(strSql, data);
                return result;
            }
        }

        public T DapperQuerySingleOrDefault<T>(string strSql, T data)
        {
            using (var conn = GetSqlConnection())
            {
                T result = conn.QuerySingleOrDefault<T>(strSql, data);
                return result;
            }
        }
    }
}
