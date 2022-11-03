using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Redis.OM;
using Redis.OM.Searching;
using Redis.OM.Searching.Query;
using RedisStackTest.Models;
using RedisStackTest.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RedisStackTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisSearchController : ControllerBase
    {
        private readonly NorthwindContext _DbContext;
        private readonly DapperUtil _dapperUtil;
        private readonly RedisConnectionProvider _provider;
        private readonly RedisCollection<RedisArt> _redis;
        public RedisSearchController(NorthwindContext context, RedisConnectionProvider provider , DapperUtil dapperUtil)
        {
            _dapperUtil = dapperUtil;
            _DbContext = context;
            _provider = provider;
            _redis = (RedisCollection<RedisArt>)provider.RedisCollection<RedisArt>(); 
        }
        // GET: api/<ValuesController>
        [HttpGet]
        public IList<RedisArt> Get()
        {
            _provider.Connection.CreateIndex(typeof(RedisArt));
            IList<RedisArt> Db = _redis.ToList();

            return Db;
        }

       
        [HttpGet("Title/{Title}")]
        public IList<RedisArt> Get([FromRoute] string Title)
        {
            _provider.Connection.CreateIndex(typeof(RedisArt));

            List<RedisArt> tresult = new List<RedisArt>();
            foreach (var item in _redis)
            {
                if (item.Title.ToLower().Contains(Title.ToLower()))
                {
                    tresult.Add(item);
                }
            }

            if (tresult.Count != 0)
            {

                return tresult;
            }
            else
            {
                RedisArt redisArt = new RedisArt();
                redisArt.Title = Title;
                string strsql = " select [ArtID],[ArtContent],[CreateTime],[UpdateTime],[Title],[UserId],[VisibleStatus],[ClicksNumber],(select count(*) from [ArtLike] L where L.[ArtID] = A.[ArtID]) as LikeClicks from [Art] A Where Title like '%'+@Title+'%'";
                var result = _dapperUtil.DapperQuery(strsql, redisArt).ToList();
                foreach (var item in result)
                {
                    _redis.Insert(item, TimeSpan.FromMinutes(10));
                }
                return result;
            }

            //var result = _redis.Where(a => a.Title.Contains(Title)).ToList();
            //if (_redis.Any(a => a.Title.Contains(Title)))
            //{

            //    var redisresult = _redis.Where(a => a.Title.Contains(Title)).OrderBy(a => a.Title).ToList();
            //    return redisresult;
            //}
            //else
            //{
            //    RedisArt redisArt = new RedisArt();
            //    redisArt.Title = Title;
            //    string strsql = " select [ArtID],[ArtContent],[CreateTime],[UpdateTime],[Title],[UserId],[VisibleStatus],[ClicksNumber],(select count(*) from [ArtLike] L where L.[ArtID] = A.[ArtID]) as LikeClicks from [Art] A Where Title like '%'+@Title+'%'";
            //    var dbresult = _dapperUtil.DapperQuery(strsql, redisArt).ToList();
            //    foreach (var item in dbresult)
            //    {
            //        _redis.Insert(item, TimeSpan.FromMinutes(10));
            //    }
            //    return dbresult;
            //}



        }
        [HttpGet("FullText/{FullText}")]
        public IList<Art> GetFull([FromRoute] string FullText)
        {
            var result = _DbContext.Arts.Where(a => a.Title.Contains(FullText) ||
                                                  a.ArtContent.Contains(FullText) ||
                                                  a.UserId.Contains(FullText)).ToList();
            return result;

        }
        // POST api/<ValuesController>
        [HttpPost]
        public async Task<RedisArt> Post([FromBody] RedisArt art)
        {
            // TimeSpan time = DateTime.Now.ToLocalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            //  art.CreateTimeStamp = time.TotalSeconds;
            art.CreateTime = DateTime.Now;
            art.VisibleStatus = 1;
            art.DataMethod = 1;
            await _redis.InsertAsync(art, TimeSpan.FromMinutes(10));
            return art;
        }

        // PUT api/<ValuesController>/5
        [HttpPut]
        public async Task Put([FromBody] RedisArt art)
        {

            art.CreateTime = DateTime.Now;
            art.VisibleStatus = 1;
            art.DataMethod = 2;

            await _redis.InsertAsync(art, TimeSpan.FromMinutes(10));
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete]
        public async Task Delete([FromBody] RedisArt art)
        {
            art.CreateTime = DateTime.Now;
            art.DataMethod = 3;

            await _redis.InsertAsync(art, TimeSpan.FromMinutes(10));
        }
    }
}
