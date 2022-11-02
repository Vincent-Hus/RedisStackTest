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
            if (_redis.Any(a =>  a.Title.Contains(Title) ))
            {
                var result = _redis.Where(a => a.Title == Title).ToList();
                return result;
            }
            else
            {
                RedisArt redisArt = new RedisArt();
                redisArt.Title = Title;
                string strsql = " select [ArtID],[ArtContent],[CreateTime],[UpdateTime],[Title],[UserId],[VisibleStatus],[ClicksNumber],(select count(*) from [ArtLike] L where L.[ArtID] = A.[ArtID]) as LikeClicks from [Art] A Where Title = @Title";
                var result = _dapperUtil.DapperQuery(strsql, redisArt).ToList();
                foreach (var item in result)
                {
                    _redis.Insert(item, TimeSpan.FromMinutes(10));
                }
                return result;
            }
            


        }
        [HttpGet("FullText/{FullText}")]
        public IList<RedisArt> GetFull([FromRoute] string FullText)
        {
            _provider.Connection.CreateIndex(typeof(RedisArt));
            if (_redis.Any(a => a.ArtContent.Contains(FullText) || a.Title.Contains(FullText) || a.UserId.Contains(FullText)))
            {

                var result = _redis.Where(a => a.ArtContent.Contains(FullText)
                                            || a.Title.Contains(FullText)
                                            || a.UserId.Contains(FullText) ).ToList();
                return result;
            }
            else
            {
                var resultdb = (from a in _DbContext.Arts
                                where a.Title.Contains(FullText)
                                    || a.ArtContent.Contains(FullText)
                                    || a.UserId.Contains(FullText)
                               select a).ToList();
                
                var resultredis = new List<RedisArt>();
                foreach (var dbitem in resultdb)
                {

                    foreach (var redisitem in resultredis)
                    {
                        
                        redisitem.ArtContent = dbitem.ArtContent;
                        redisitem.Title = dbitem.Title;
                        redisitem.UserId = dbitem.UserId;
                        redisitem.ArtId = dbitem.ArtId;
                        redisitem.CreateTime = dbitem.CreateTime;
                        redisitem.ClicksNumber = dbitem.ClicksNumber;
                        resultredis.Add(redisitem);
                        _redis.Insert(redisitem, TimeSpan.FromMinutes(10));
                    }
                }
                return resultredis;
            }

        }
        // POST api/<ValuesController>
        [HttpPost]
        public async Task<RedisArt> Post([FromBody] RedisArt art)
        {
            // TimeSpan time = DateTime.Now.ToLocalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            //  art.CreateTimeStamp = time.TotalSeconds;
            art.CreateTime = DateTime.Now;
            art.VisibleStatus = 1;
            art.InsertData = 1;
            await _redis.InsertAsync(art, TimeSpan.FromMinutes(10));
            return art;
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            
        }
    }
}
