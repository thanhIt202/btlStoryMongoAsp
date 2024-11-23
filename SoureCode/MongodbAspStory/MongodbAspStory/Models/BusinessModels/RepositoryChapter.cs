using MongodbAspStory.Models.DataModels;
using MongoDB.Driver;
using MongoDB.Bson;
using MongodbAspStory.Models.ViewModels;

namespace MongodbAspStory.Models.BusinessModels
{
	public class RepositoryChapter : IRepositoryChapter
	{
		MongodbAspStoryDbContext _db;
		public RepositoryChapter(MongodbAspStoryDbContext db)
		{
			_db = db;
		}

		public bool Delete(int key)
		{
			try
			{
				_db.Chapters.DeleteOne(c => c._id == key);
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		public List<Chapter> GetAll()
		{
			return _db.Chapters.Find(FilterDefinition<Chapter>.Empty).ToList();
		}

		public List<ChapterViewModel> GetChapterFull()
		{
			BsonDocument[] lookup = new BsonDocument[1]
			{
				new BsonDocument
				{
					{
						"$lookup", new BsonDocument
						{
							{"from", "stories"},
							{"localField", "stoId"},
							{"foreignField", "_id"},
							{"as", "stories" }
						}
					}
				}
			};
			var chapters = _db.Chapters.Aggregate<BsonDocument>(lookup).ToList();
			var data = new List<ChapterViewModel>();
			foreach (var e in chapters)
			{
				var c = new ChapterViewModel();
				c._id = e["_id"].ToInt32();
				c.chapName = e["chapName"].ToString();
				c.chapContent = e["chapContent"].ToString();
				c.stoId = e["stoId"].ToInt32();
				c.chapDate = ((DateTime)e["chapDate"]);

                c.title = e["stories"].AsBsonArray[0]["title"].ToString();
				c.author = e["stories"].AsBsonArray[0]["author"].ToString();
				c.interpreter = e["stories"].AsBsonArray[0]["interpreter"].ToString();
				c.image = e["stories"].AsBsonArray[0]["image"].ToString();
				c.stoInformation = e["stories"].AsBsonArray[0]["stoInformation"].ToString();
				c.stoStatus = e["stories"].AsBsonArray[0]["stoStatus"].ToString();
                c.numberChap = e["stories"].AsBsonArray[0]["numberChap"].ToInt32();
                c.catId = e["stories"].AsBsonArray[0]["catId"].ToInt32();
				data.Add(c);
			}
			return data;
		}

		public ChapterViewModel GetChapterFullById(int key)
		{
			BsonDocument[] lookup = new BsonDocument[1]
			{
				new BsonDocument
				{
					{
						"$lookup", new BsonDocument
						{
							{"from", "stories"},
							{"localField", "stoId"},
							{"foreignField", "_id"},
							{"as", "stories" }
						}
					}
				}
			};
			var chapters = _db.Chapters.Aggregate<BsonDocument>(lookup).ToList();
			var data = new List<ChapterViewModel>();
            foreach (var e in chapters)
            {
                var c = new ChapterViewModel();
                c._id = e["_id"].ToInt32();
                c.chapName = e["chapName"].ToString();
                c.chapContent = e["chapContent"].ToString();
                c.stoId = e["stoId"].ToInt32();
                c.chapDate = ((DateTime)e["chapDate"]);

                c.title = e["stories"].AsBsonArray[0]["title"].ToString();
                c.author = e["stories"].AsBsonArray[0]["author"].ToString();
                c.interpreter = e["stories"].AsBsonArray[0]["interpreter"].ToString();
                c.image = e["stories"].AsBsonArray[0]["image"].ToString();
                c.stoInformation = e["stories"].AsBsonArray[0]["stoInformation"].ToString();
                c.stoStatus = e["stories"].AsBsonArray[0]["stoStatus"].ToString();
                c.numberChap = e["stories"].AsBsonArray[0]["numberChap"].ToInt32();
                c.catId = e["stories"].AsBsonArray[0]["catId"].ToInt32();
                data.Add(c);
            }
            return data.Find(x => x._id == key);
		}

		public Chapter GetById(int key)
		{
			return _db.Chapters.Find(x => x._id == key).FirstOrDefault();
		}

		public bool Insert(Chapter entity)
		{
			entity.chapContent = entity.chapContent ?? "";

            // Get the next available id from the counters collection
            var counter = _db.Counters.FindOneAndUpdate(
                Builders<Counter>.Filter.Eq(c => c._id, "chapters"),
                Builders<Counter>.Update.Inc(c => c.seq, 1),
                new FindOneAndUpdateOptions<Counter> { IsUpsert = true, ReturnDocument = ReturnDocument.After }
            );

            // Set the entity's _id to the incremented value
            entity._id = counter.seq;

            // Insert the chapter
            _db.Chapters.InsertOne(entity);
			return true;
		}

		public List<Chapter> Paging(int page, int pageSize, out long totalPage)
		{
			int skip = pageSize * (page - 1);
			long rows = _db.Chapters.CountDocuments(FilterDefinition<Chapter>.Empty);
			totalPage = rows % pageSize == 0 ? rows / pageSize : (rows / pageSize) + 1;
			return _db.Chapters.Find(FilterDefinition<Chapter>.Empty).Skip(skip).Limit(pageSize).ToList();
		}

		public List<Chapter> SearchPaging(string name, int page, int pageSize, out long totalPage)
		{
			int skip = pageSize * (page - 1);
			long rows = _db.Chapters.CountDocuments(c => c.chapName.ToLower().Contains(name.ToLower()));
			totalPage = rows % pageSize == 0 ? rows / pageSize : (rows / pageSize) + 1;
			return _db.Chapters.Find(c => c.chapName.ToLower().Contains(name.ToLower())).Skip(skip).Limit(pageSize).ToList();
		}

		public bool Update(Chapter entity)
		{
			var c = Builders<Chapter>.Update.Set("chapName", entity.chapName)
				.Set("chapContent", entity.chapContent)
				.Set("stoId", entity.stoId)
                .Set("chapDate", entity.chapDate);
            _db.Chapters.UpdateOne(x => x._id == entity._id, c);
			return true;
		}
	}
}
