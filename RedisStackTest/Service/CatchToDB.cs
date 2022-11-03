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
            if (_redis.Any(a => a.DataMethod == 1 ))
            {

                var Insertitem = new List<RedisArt>();
                Insertitem = _redis.Where(a => a.DataMethod == 1).ToList();
                string strinsert = @" Insert into [Art] ([Title],[ArtContent],[CreateTime],[UserID]) values (@Title , @ArtContent, @CreateTime,@UserID ) ";
                await _dapperUtil.DapperExecuteAsync(strinsert, Insertitem);
                await MultiDelete(_redis, Insertitem);
            }
            if (_redis.Any(a =>  a.DataMethod == 2 ))
            {
                var updateitem = new List<RedisArt>();
                updateitem = _redis.Where(a => a.DataMethod == 2).ToList();
                string strupdate = @" Update [Art] set [Title] = @Title ,[ArtContent] = @ArtContent ,[UpdateTime] = @UpdateTime Where ArtID = @ArtID";
                await _dapperUtil.DapperExecuteAsync(strupdate, updateitem);
                await MultiDelete(_redis, updateitem);
            }
            if (_redis.Any(a => a.DataMethod == 3))
            {
                var deleteitem = new List<RedisArt>();
                deleteitem = _redis.Where(a => a.DataMethod == 3).ToList();
                string strdelete = @" delete [Art] Where ArtID = @ArtID";
                await _dapperUtil.DapperExecuteAsync(strdelete, deleteitem);
                await MultiDelete(_redis, deleteitem);
            }

        }

        private async Task MultiDelete<T>(RedisCollection<T> param ,IList<T> data)
        {
            foreach (var item in data)
            {
                await param.DeleteAsync(item);
            }
        }
    }
}
