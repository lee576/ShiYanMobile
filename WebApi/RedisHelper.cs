using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ServiceStack.Redis;

namespace WebApi
{
    public class RedisHelper
    {
        /// <summary>
        /// 创建Redis客户端
        /// </summary>
        /// <returns></returns>
        public static RedisClient CreateClient()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var config = builder.Build();
            var redisConnection = config.GetConnectionString("RedisConnection");
            var redisIp = redisConnection.Split(':')[0];
            var redisPort = int.Parse(redisConnection.Split(':')[1]);

            RedisClient redisClient = new RedisClient(redisIp, redisPort);
            return redisClient;
        }
    }
}
