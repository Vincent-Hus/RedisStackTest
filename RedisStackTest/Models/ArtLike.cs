using Redis.OM.Modeling;
using System;
using System.Collections.Generic;

#nullable disable

namespace RedisStackTest.Models
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "ArtLike" })]
    public partial class ArtLike
    {
        public int ArtLikeId { get; set; }
        public int ArtId { get; set; }
        public string UserId { get; set; }
        public byte DisLike { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
