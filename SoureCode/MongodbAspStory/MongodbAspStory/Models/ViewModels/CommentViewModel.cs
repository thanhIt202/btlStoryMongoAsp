using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MongodbAspStory.Models.ViewModels
{
    public class CommentViewModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; }
        public string? commContent { get; set; }
        public int stoId { get; set; }
        public int accId { get; set; }
        public DateTime commDate { get; set; }

        public string? title { get; set; }
        public string? author { get; set; }
        public string? interpreter { get; set; }
        public string? image { get; set; }
        public string? stoInformation { get; set; }
        public string? stoStatus { get; set; }
        public int numberChap { get; set; }
        public int catId { get; set; }

        public string? fullName { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
        public string? accPhone { get; set; }
        public string? accAddress { get; set; }
        public string? avatar { get; set; }
        public string? accStatus { get; set; }
        public string? accType { get; set; }
    }
}
