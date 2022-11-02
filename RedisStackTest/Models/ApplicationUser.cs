using System;
using System.Collections.Generic;

#nullable disable

namespace RedisStackTest.Models
{
    public partial class ApplicationUser
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public DateTime CreateTime { get; set; }
        public string Salt { get; set; }
        public string Email { get; set; }
        public int AccountId { get; set; }
    }
}
