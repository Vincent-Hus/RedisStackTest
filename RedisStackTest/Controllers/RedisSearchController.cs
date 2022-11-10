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
        private readonly RedisUtil _redisUtil;
        public RedisSearchController(NorthwindContext context, RedisConnectionProvider provider , DapperUtil dapperUtil ,RedisUtil redisUtil)
        {
            _redisUtil = redisUtil;
            _dapperUtil = dapperUtil;
            _DbContext = context;
            _provider = provider;
            _redis = (RedisCollection<RedisArt>)provider.RedisCollection<RedisArt>(); 
        }
        // GET: api/<ValuesController>
        [HttpGet]
        public IList<RedisArt> Get()
        {
            //using (var conn = _provider.Connection)
            //{
            //    conn.DropIndex(typeof(RedisArt));
            //    conn.CreateIndex(typeof(RedisArt));
            //}
            IList<RedisArt> Db = _redis.ToList();
            return Db;
        }
        [HttpGet("ArtID/{ArtID}")]
        public RedisArt GetFromId([FromRoute] int ArtID)
        {
            if (_redis.Any(a=>a.ArtId == ArtID))
            {
                return _redis.Where(a => a.ArtId == ArtID).FirstOrDefault();
            }
            else
            {
                var ArtDb = GetArtFromDb(ArtID);
                _redis.Insert(ArtDb, TimeSpan.FromMinutes(10));
                return ArtDb;
            }

        }
       
        [HttpGet("Title/{Title}")]
        public async Task<IList<RedisArt>> Get([FromRoute] string Title)
        {
            using (var conn = _provider.Connection)
            {
                conn.CreateIndex(typeof(RedisArt));
            }

            if (_redis.Any(a => a.Title.Contains(Title)))
            {

                var redisresult = _redis.Where(a => a.Title.Contains(Title))
                                        .OrderBy(a => a.ArtId)
                                        
                                        .ToList();
                var count = redisresult.Count;
                return redisresult;
            }
            else
            {

                var dbresult = GetArtFromDbUseTitle(Title);
                await _redisUtil.BulkInsert(dbresult);
                return dbresult;
            }

            //if (_redis.Any(a => a.Title.Contains(Title)))
            //{

            //    var redisresult = _redis.Where(a => a.Title == Title)
            //                            .OrderBy(a => a.ArtId)
            //                            .Select(a => new Art
            //                            {
            //                                UserId = a.UserId
            //                                ,
            //                                Title = a.Title
            //                                ,
            //                                ArtContent = a.ArtContent
            //                                ,
            //                                CreateTime = a.CreateTime
            //                            })
            //                            .ToList();
            //    return redisresult;
            //}
            //else
            //{
            //    RedisArt redisart = new RedisArt { Title = Title };

            //    List<Art> art = new List<Art>();

            //    string strsql = " select [ArtID],";
            //    strsql += " [ArtContent],";
            //    strsql += " [CreateTime],";
            //    strsql += " [UpdateTime],";
            //    strsql += " [Title],";
            //    strsql += " [UserId],";
            //    strsql += " [VisibleStatus],";
            //    strsql += " [ClicksNumber],";
            //    strsql += " (select count(*) from [ArtLike] L where L.[ArtID] = A.[ArtID]) as LikeClicks";
            //    strsql += "  from [Art] A ";
            //    strsql += " Where Title like '%'+@Title+'%'";
            //    var dbresult = _dapperUtil.DapperQuery(strsql, redisart).ToList();

            //    art = dbresult.Select(a => new Art
            //    {
            //        ArtContent = a.ArtContent
            //       ,
            //        ArtId = a.ArtId
            //       ,
            //        ClicksNumber = a.ClicksNumber
            //       ,
            //        CreateTime = a.CreateTime
            //       ,
            //        VisibleStatus = a.VisibleStatus
            //       ,
            //        Title = a.Title
            //       ,
            //        UpdateTime = a.UpdateTime
            //       ,
            //        UserId = a.UserId
            //    }).ToList();

            //    await _redisUtil.BulkInsert(dbresult);
            //    return art;
            //}



        }
        [HttpGet("FullText/{FullText}")]
        public IList<Art> GetFull([FromRoute] string FullText)
        {
            var result = _DbContext.Arts.Where(a => a.Title.Contains(FullText) ||
                                                  a.ArtContent.Contains(FullText) ||
                                                  a.UserId.Contains(FullText))
                                        .OrderBy(a=>a.ArtId)
                                        .Select(a=>new Art
                                        {
                                            ArtContent = a.ArtContent
                                           ,Title=a.Title
                                           ,CreateTime=a.CreateTime
                                           ,UserId=a.UserId
                                        })
                                        .ToList();
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
        private RedisArt GetArtFromDb(int ArtId)
        {
            RedisArt redisart = new RedisArt { ArtId = ArtId };

            string strsql = " select [ArtID],";
            strsql += " [ArtContent],";
            strsql += " [CreateTime],";
            strsql += " [UpdateTime],";
            strsql += " [Title],";
            strsql += " [UserId],";
            strsql += " [VisibleStatus],";
            strsql += " [ClicksNumber],";
            strsql += " (select count(*) from [ArtLike] L where L.[ArtID] = A.[ArtID]) as LikeClicks";
            strsql += "  from [Art] A ";
            strsql += " Where ArtID = @ArtId";
            var dbresult = _dapperUtil.DapperQuery(strsql, redisart).FirstOrDefault();

            return dbresult;
        }

        private IList<RedisArt> GetArtFromDbUseTitle(string Title)
        {
            RedisArt redisart = new RedisArt { Title = Title };

            string strsql = " select [ArtID],";
            strsql += " [ArtContent],";
            strsql += " [CreateTime],";
            strsql += " [UpdateTime],";
            strsql += " [Title],";
            strsql += " [UserId],";
            strsql += " [VisibleStatus],";
            strsql += " [ClicksNumber],";
            strsql += " (select count(*) from [ArtLike] L where L.[ArtID] = A.[ArtID]) as LikeClicks";
            strsql += "  from [Art] A ";
            strsql += " Where Title like '%'+@Title+'%'";
            var dbresult = _dapperUtil.DapperQuery(strsql, redisart).ToList();

            return dbresult;
        }
    }
}
