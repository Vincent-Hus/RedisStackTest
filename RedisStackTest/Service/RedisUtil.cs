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
    public class RedisUtil: BackgroundService
    {
        private readonly RedisConnectionProvider _provider;
        private readonly CatchToDB _catchToDB;
        public RedisUtil(RedisConnectionProvider provider,CatchToDB catchToDB)
        {
            _provider = provider;
            _catchToDB = catchToDB;
        }

        //public override async Task StartAsync(CancellationToken cancellationToken)
        //{
        //    await _provider.Connection.DropIndexAsync(typeof(RedisArt));
        //
        //}
        //
        //public override Task StopAsync(CancellationToken cancellationToken)
        //{
        //    return Task.CompletedTask;
        //}

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
