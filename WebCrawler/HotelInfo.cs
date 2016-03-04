using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler
{ 
    [Serializable]
    public class Comment
    {
        public decimal? Rate { get; set; }
        public DateTime? Timestamp { get; set; }
        public int? UsefulCount { get; set; } = -1;
    }

    [Serializable]
    public class HotelBaseData
    {
        public string PrimaryKey { get; set; } 
        public string ForeignKey { get; set; }
        public string Name { get; set; }
        public int? Star { get; set; } 
        public decimal? Rate { get; set; }
        public string Locality { get; set; }
        public string Location { get; set; }
        public int? NumberOfRooms { get; set; }
        public int? BuiltYear { get; set; } 
        public CommentData PrimaryCommentData { get; set; }
        public CommentData ForeignCommentData { get; set; }
    }

    [Serializable]
    public class CommentData
    {
        public int? CommentCount { get; set; }
        public int? SatisfiedCommentCount { get; set; }
        public int? NeedToImproveCommentCount { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }

    public class HotelInfo
    {
        public string Name { get; set; }
        public string NumberOfRate { get; set; }
        public string Price { get;  set; }
        public string Rate { get; set; }
        public string RecommendPercentage { get; set; }
        public string Stars { get; set; }
    }
}
