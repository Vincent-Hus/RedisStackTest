using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisStackTest.Service
{
    public class RedisTool
    {
        public class RedisConnectorHelper
        {
            static RedisConnectorHelper()
            {
                RedisConnectorHelper._connection = new Lazy<ConnectionMultiplexer>(() =>
                {
                    return ConnectionMultiplexer.Connect("localhost");
                });
            }

            private static Lazy<ConnectionMultiplexer> _connection;

            public static ConnectionMultiplexer Connection
            {
                get
                {
                    return _connection.Value;
                }
            }

        }
    }
}
