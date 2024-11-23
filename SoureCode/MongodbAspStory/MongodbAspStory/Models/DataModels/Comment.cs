
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongodbAspStory.Models.DataModels
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; }
        public string? commContent { get; set; }
		public int stoId { get; set; }
		public int accId { get; set; }
		public DateTime commDate { get; set; }

    }
}
