using Microsoft.Extensions.Configuration;
using SqlSugar;
using System;
using System.IO;
using System.Xml;

namespace Infrastructure
{
    /// <summary>
    /// 数据库工厂
    /// </summary>
    public class DbFactory
    {
        /// <summary>
        /// SqlSugarClient属性
        /// </summary>
        /// <returns></returns>
        public static SqlSugarClient GetSqlSugarClient()
        {
            string rootdir = AppContext.BaseDirectory;
            var _connectString = String.Empty;

            var builder = new ConfigurationBuilder().SetBasePath(rootdir).AddJsonFile("infrastructuresetting.json");
            var config = builder.Build();
            _connectString = config["ConnetString"];

            var db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = _connectString,  //必填
                DbType = DbType.SqlServer,          //必填
                IsAutoCloseConnection = true,       //默认false
                InitKeyType = InitKeyType.SystemTable
            }); 
            return db;
        }
    }
}
