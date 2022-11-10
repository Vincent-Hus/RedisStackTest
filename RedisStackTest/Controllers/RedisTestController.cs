using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Redis.OM;
using Redis.OM.Searching;
using RedisStackTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RedisStackTest.Service.RedisTool;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RedisStackTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisTestController : ControllerBase
    {
       
        // GET: api/<RedisTestController>
        [HttpGet]
        public IEnumerable<RedisArt> Get()
        {
            var redis = RedisConnectorHelper.Connection.GetDatabase();
            var result = JsonConvert.DeserializeObject<IList<RedisArt>>(redis.StringGet("testArt"));
            return result.Skip(9000);
        }

        // GET api/<RedisTestController>/5
        [HttpGet("{id}")]
        public RedisArt Get(int id)
        {
            var redis = RedisConnectorHelper.Connection.GetDatabase();
            var result =  JsonConvert.DeserializeObject<IList<RedisArt>>(redis.StringGet("testArt"));
            
            return result.Where(a=>a.ArtId ==id).FirstOrDefault();
        }

        // POST api/<RedisTestController>
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
              await RedisConnectorHelper.Connection.GetDatabase().StringSetAsync("testArt", JsonConvert.SerializeObject(art), TimeSpan.FromMinutes(60));
            
        }

        // PUT api/<RedisTestController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<RedisTestController>/5
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
