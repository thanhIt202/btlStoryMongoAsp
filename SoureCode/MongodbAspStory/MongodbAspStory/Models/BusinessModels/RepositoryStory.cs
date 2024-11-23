using MongodbAspStory.Models.DataModels;
using MongoDB.Driver;
using MongoDB.Bson;
using MongodbAspStory.Models.ViewModels;

namespace MongodbAspStory.Models.BusinessModels
{
    public class RepositoryStory : IRepositoryStory
	{
        MongodbAspStoryDbContext _db;
        public RepositoryStory(MongodbAspStoryDbContext db)
        {
            _db = db;
        }

        public bool Delete(int key)
        {
            try
            {
				_db.Stories.DeleteOne(b => b._id == key);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public List<Story> GetAll()
        {
            return _db.Stories.Find(FilterDefinition<Story>.Empty).ToList();
        }

		public List<StoryViewModel> GetStoryFull()
        {
            BsonDocument[] lookup = new BsonDocument[1]
            {
                new BsonDocument
                {
                    {
                        "$lookup", new BsonDocument
                        {
                            {"from", "categories"},
                            {"localField", "catId"},
                            {"foreignField", "_id"},
                            {"as", "categories" }
                        }
                    }
                }
            };
            var stories = _db.Stories.Aggregate<BsonDocument>(lookup).ToList();
            var data = new List<StoryViewModel>();
            foreach (var e in stories)
            {
                var s = new StoryViewModel();
                s._id = e["_id"].ToInt32();
                s.title = e["title"].ToString();
                s.author = e["author"].ToString();
				s.interpreter = e["interpreter"].ToString();
                s.image = e["image"].ToString();
				s.stoInformation = e["stoInformation"].ToString();
				s.stoStatus = e["stoStatus"].ToString();
                s.numberChap = e["numberChap"].ToInt32();
                s.catId = e["catId"].ToInt32();

				s.catName = e["categories"].AsBsonArray[0]["catName"].ToString();
				s.catStatus = e["categories"].AsBsonArray[0]["catStatus"].ToInt32();
				data.Add(s);
            }
            return data;
        }

		public StoryViewModel GetStoryFullById(int key)
		{
			BsonDocument[] lookup = new BsonDocument[1]
			{
				new BsonDocument
				{
					{
						"$lookup", new BsonDocument
						{
							{"from", "categories"},
							{"localField", "catId"},
							{"foreignField", "_id"},
							{"as", "categories" }
						}
					}
				}
			};
			var stories = _db.Stories.Aggregate<BsonDocument>(lookup).ToList();
			var data = new List<StoryViewModel>();
            foreach (var e in stories)
            {
                var s = new StoryViewModel();
                s._id = e["_id"].ToInt32();
                s.title = e["title"].ToString();
                s.author = e["author"].ToString();
                s.interpreter = e["interpreter"].ToString();
                s.image = e["image"].ToString();
                s.stoInformation = e["stoInformation"].ToString();
                s.stoStatus = e["stoStatus"].ToString();
                s.numberChap = e["numberChap"].ToInt32();
                s.catId = e["catId"].ToInt32();

                s.catName = e["categories"].AsBsonArray[0]["catName"].ToString();
                s.catStatus = e["categories"].AsBsonArray[0]["catStatus"].ToInt32();
                data.Add(s);
            }
            return data.Find(x => x._id == key);
		}

		public Story GetById(int key)
        {
            return _db.Stories.Find(x => x._id == key).FirstOrDefault();
        }

        public bool Insert(Story entity)
        {
            entity.interpreter = entity.interpreter ?? "";
            entity.image = entity.image ?? "";
            entity.stoInformation = entity.stoInformation ?? "";

            // Get the next available id from the counters collection
            var counter = _db.Counters.FindOneAndUpdate(
                Builders<Counter>.Filter.Eq(c => c._id, "stories"),
                Builders<Counter>.Update.Inc(c => c.seq, 1),
                new FindOneAndUpdateOptions<Counter> { IsUpsert = true, ReturnDocument = ReturnDocument.After }
            );

            // Set the entity's _id to the incremented value
            entity._id = counter.seq;

            // Insert the story
            _db.Stories.InsertOne(entity);
            return true;
        }

        public List<Story> Paging(int page, int pageSize, out long totalPage)
        {
            int skip = pageSize * (page - 1);
            long rows = _db.Stories.CountDocuments(FilterDefinition<Story>.Empty);
            totalPage = rows % pageSize == 0 ? rows / pageSize : (rows / pageSize) + 1;
            return _db.Stories.Find(FilterDefinition<Story>.Empty).Skip(skip).Limit(pageSize).ToList();
        }

        public List<Story> SearchPaging(string name, int page, int pageSize, out long totalPage)
        {
            int skip = pageSize * (page - 1);
            long rows = _db.Stories.CountDocuments(s => s.title.ToLower().Contains(name.ToLower()));
            totalPage = rows % pageSize == 0 ? rows / pageSize : (rows / pageSize) + 1;
            return _db.Stories.Find(s => s.title.ToLower().Contains(name.ToLower())).Skip(skip).Limit(pageSize).ToList();
        }

        public bool Update(Story entity)
        {
            var s = Builders<Story>.Update.Set("title", entity.title)
                .Set("author", entity.author)
                .Set("interpreter", entity.interpreter)
                .Set("image", entity.image)
                .Set("stoInformation", entity.stoInformation)
                .Set("stoStatus", entity.stoStatus)
                .Set("numberChap", entity.numberChap)
                .Set("catId", entity.catId);
            _db.Stories.UpdateOne(x => x._id == entity._id, s);
            return true;
        }
    }
}
