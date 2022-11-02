using Microsoft.AspNetCore.Mvc;
using Redis.OM;
using Redis.OM.Searching;
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
    public class RedisGraphController : ControllerBase
    {
        private readonly NorthwindContext _DbContext;
        private readonly RedisConnectionProvider _provider;
        private readonly RedisCollection<Art> _redis; //不能做為建構式參數
        public RedisGraphController(NorthwindContext context, RedisConnectionProvider provider,RedisTool redis2)
        {
            _DbContext = context;
            _provider = provider;
            _redis = (RedisCollection<Art>)provider.RedisCollection<Art>();
        }
        // GET: api/<RedisGraphController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<RedisGraphController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<RedisGraphController>
        [HttpPost]
        public async void Post()
        {
            //_provider.Connection.Execute("GRAPH.QUERY ArtComment CREATE(:Art { title: '" + art.Title + "', content:'" + art.ArtContent + "' , id: " + art.ArtId.ToString() + " })");
            
            RedisReply tt = await _provider.Connection.ExecuteAsync("Bf.ADD","bftest","123");
            
        }


    }
}
