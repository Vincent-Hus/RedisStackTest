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
    public class RedisScript: BackgroundService
    {
        private readonly RedisConnectionProvider _provider;
        private readonly CatchToDB _catchToDB;
        private readonly RedisCollection<RedisArt> _redis;
        public RedisScript(RedisConnectionProvider provider, CatchToDB catchToDB)
        {
            _provider = provider;
            _catchToDB = catchToDB;
            _redis = (RedisCollection<RedisArt>)provider.RedisCollection<RedisArt>();
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _catchToDB.ArtCatchToDB();
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
