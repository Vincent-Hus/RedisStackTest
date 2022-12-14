using Microsoft.Extensions.Hosting;
using Redis.OM;
using Redis.OM.Searching;
using RedisStackTest.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RedisStackTest.Service
{
    public class RedisUtil
    {
        private readonly RedisConnectionProvider _provider;
        private readonly CatchToDB _catchToDB;
        private readonly RedisCollection<RedisArt> _redis;
        public RedisUtil(RedisConnectionProvider provider,CatchToDB catchToDB)
        {
            _provider = provider;
            _catchToDB = catchToDB;
            _redis = (RedisCollection<RedisArt>)provider.RedisCollection<RedisArt>();
        }

        public async Task BulkInsert<T>(IList<T> items)
        {
            var tasks = new List<Task>();
            foreach (var item in items)
            {
                tasks.Add(_provider.Connection.SetAsync(item, TimeSpan.FromMinutes(10)));
            }

            await Task.WhenAll(tasks);
        }

    }

}
