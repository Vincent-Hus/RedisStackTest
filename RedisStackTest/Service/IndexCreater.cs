using Microsoft.Extensions.Hosting;
using Redis.OM;
using Redis.OM.Searching;
using RedisStackTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RedisStackTest.Service
{
    public class IndexCreater: IHostedService
    {
        private readonly RedisConnectionProvider _provider;
        private readonly CatchToDB _catchToDB;
        private readonly RedisCollection<RedisArt> _redis;
        public IndexCreater(RedisConnectionProvider provider, CatchToDB catchToDB)
        {
            _provider = provider;
            _catchToDB = catchToDB;
            _redis = (RedisCollection<RedisArt>)provider.RedisCollection<RedisArt>();
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var conn = _provider.Connection)
            {
                await conn.CreateIndexAsync(typeof(RedisArt));
            }


        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
