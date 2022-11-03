using Redis.OM.Modeling;
using System;
using System.Collections.Generic;

#nullable disable

namespace RedisStackTest.Models
{
    public partial class Art
    {
        public int ArtId { get; set; }
        public string ArtContent { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string Title { get; set; }
        public string UserId { get; set; }
        public byte VisibleStatus { get; set; }
        public int ClicksNumber { get; set; }
    }


    [Document(StorageType = StorageType.Json, Prefixes = new[] { "RedisArt" },IndexName ="RedisArtIdx") ]
    public class RedisArt
    {
       
        [RedisIdField] public int ArtId { get; set; }

        [Searchable(Sortable = true, Weight = 3)] public string ArtContent { get; set; }
        [Indexed(Sortable = true)] public DateTime CreateTime { get; set; }

        [Indexed] public DateTime? UpdateTime { get; set; }
        [Searchable(Sortable =true,Weight =2)] public string Title { get; set; }
        [Indexed] public string UserId { get; set; }
        [Indexed] public byte VisibleStatus { get; set; }
         public int ClicksNumber { get; set; }
        [Indexed] public int LikeClicks { get; set; }
        [Indexed] public int DataMethod { get; set; }
        //[Indexed(Sortable =true)] public double CreateTimeStamp {
        //    get
        //    {
        //        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
        //        TimeSpan span = (CreateTime.ToLocalTime() - epoch);
        //        return span.TotalSeconds;
        //    }
        //    set
        //    {
        //        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
        //        CreateTime = epoch.AddSeconds(value);
        //    }
        //}
    }
}
