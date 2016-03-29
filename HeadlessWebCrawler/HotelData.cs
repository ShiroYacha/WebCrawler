using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadlessWebCrawler
{
    [Serializable]
    public class HotelData
    {
        public string PrimaryKey { get; set; }
        public string ForeignKey { get; set; }
        public string Name { get; set; }
        public string Star { get; set; }
        public string Ranking { get; set; }
        public string Type { get; set; }
        public string Locality { get; set; }
        public string Location { get; set; }
        public string NumberOfRooms { get; set; }
        public string BuiltYear { get; set; }
        public PrimaryCommentData PrimaryCommentData { get; set; }
        public ForeignCommentData ForeignCommentData { get; set; }
    }

    [Serializable]
    public class PrimaryCommentData
    {
        public string CommentCount { get; set; }
        public string ExcellentCount{ get; set; }
        public string GoodCount{ get; set; }
        public string NormalCount { get; set; }
        public string BadCount { get; set; }
        public string VeryBadCount { get; set; }
        public string FamilyCat { get; set; }
        public string CoupleCat { get; set; }
        public string SoloCat { get; set; }
        public string BusinessCat { get; set; }
    }

    [Serializable]
    public class ForeignCommentData
    {
        public string ForeignId { get; set; }
        public string ForeignKey { get; set; }


        public string Star { get; set; }

        public string ForeignNameId { get; set; }
        public string CommentCount { get; set; }
        public string ForeignName { get; set; }
        public string RecommendCommentCount{ get; set; }
        public string NeedToImproveCommentCount { get; set; }
        public string Score { get; set; }
        public string Price { get; set; }
        public string OpenYear { get; set; }
        public string RoomCount { get; set; }
    }
}
