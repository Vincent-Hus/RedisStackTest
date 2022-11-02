using Redis.OM;
using Redis.OM.Searching;
using RedisStackTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisStackTest.Service
{
    public class CatchToDB
    {
        private readonly DapperUtil _dapperUtil;
        private readonly RedisCollection<RedisArt> _redis;
        public CatchToDB(RedisConnectionProvider provider, DapperUtil dapperUtil)
        {
            _dapperUtil = dapperUtil;
            _redis = (RedisCollection<RedisArt>)provider.RedisCollection<RedisArt>();
        }
        public async Task ArtCatchToDB()
        {
            var Insertitem = new List<RedisArt>();
            
            if (!_redis.Any(a => a.InsertData == 1))
            {
                return;
            }
            Insertitem = _redis.Where(a => a.InsertData == 1).ToList();
            string strinsert = @" Insert into [Art] ([Title],[ArtContent],[CreateTime],[UserID]) values (@Title , @ArtContent, @CreateTime,@UserID ) ";
            await _dapperUtil.DapperExecuteAsync(strinsert, Insertitem);
            foreach (var item in Insertitem)
            {
                await _redis.DeleteAsync(item);
            }
            
        }
    }
}
