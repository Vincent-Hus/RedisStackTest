using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Redis.OM;
using Redis.OM.Searching;
using RedisStackTest.Models;
using RedisStackTest.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RedisStackTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisStackTestController : ControllerBase
    {
        private readonly RedisUtil _redisUtil;
        private readonly RedisConnectionProvider _provider;
        private readonly RedisCollection<RedisArt> _redis;
        public RedisStackTestController(RedisConnectionProvider provider,RedisUtil redisUtil)
        {
            _provider = provider;
            _redis = (RedisCollection<RedisArt>)provider.RedisCollection<RedisArt>();
            _redisUtil = redisUtil;
        }
        // GET: api/<RedisStackTestController>
        [HttpGet]
        public IEnumerable<RedisArt> Get()
        {
            return _redis.Skip(9000);
        }

        // GET api/<RedisStackTestController>/5
        [HttpGet("{id}")]
        public RedisArt Get(int id)
        {

            return _redis.Where(a=>a.ArtId==id).FirstOrDefault();
        }

        // POST api/<RedisStackTestController>
        [HttpPost]
        public async Task Post()
        {
            List<RedisArt> art = new List<RedisArt>();
            for (int i = 1; i < 10000; i++)
            {
                art.Add(new RedisArt
                {
                    ArtContent = RandomString(),
                    Title = RandomString(),
                    ArtId = i,
                    UserId = RandomString(),
                    CreateTime = DateTime.Now,
                    ClicksNumber = i,
                    VisibleStatus = 1,
                    DataMethod = 0,
                    LikeClicks = 0,
                    UpdateTime = null
                });
            }
            await _redisUtil.BulkInsert(art);
        }
       
        // PUT api/<RedisStackTestController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<RedisStackTestController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        private string RandomString()
        {

            var builder = new StringBuilder();
            Random random = new Random();
            var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            for (int i = 0; i < 10; i++)
            {
                builder.Append(characters[random.Next(1, characters.Length)]);
            }
            return builder.ToString();

        }
    }
}
