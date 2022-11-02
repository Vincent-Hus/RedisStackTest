using Redis.OM.Modeling;
using System;
using System.Collections.Generic;

#nullable disable

namespace RedisStackTest.Models
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "Comment"})]
    public partial class Comment
    {
        [RedisIdField] [Indexed]public int CommentId { get; set; }
        [Searchable] public string CommentContent { get; set; }
        [Indexed] public DateTime CreateTime { get; set; }
        [Indexed] public DateTime? UpdateTime { get; set; }
        [Indexed] public int ArtId { get; set; }
        [Indexed] public string UserId { get; set; }
        public byte VisibleStatus { get; set; }
    }
}
