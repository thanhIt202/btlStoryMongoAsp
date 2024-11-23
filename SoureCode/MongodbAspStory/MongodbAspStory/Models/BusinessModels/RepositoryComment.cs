using MongodbAspStory.Models.DataModels;
using MongoDB.Driver;
using MongoDB.Bson;
using MongodbAspStory.Models.ViewModels;

namespace MongodbAspStory.Models.BusinessModels
{
	public class RepositoryComment : IRepositoryComment
    {
		MongodbAspStoryDbContext _db;
		public RepositoryComment(MongodbAspStoryDbContext db)
		{
			_db = db;
		}

		public bool Delete(ObjectId key)
		{
			try
			{
				_db.Comments.DeleteOne(c => c._id == key);
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		public List<Comment> GetAll()
		{
			return _db.Comments.Find(FilterDefinition<Comment>.Empty).ToList();
		}

		public List<CommentViewModel> GetCommentFull()
		{
			BsonDocument[] lookup = new BsonDocument[2]
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
				},
                new BsonDocument
                {
                    {
                        "$lookup", new BsonDocument
                        {
                            {"from", "accounts"},
                            {"localField", "accId"},
                            {"foreignField", "_id"},
                            {"as", "accounts" }
                        }
                    }
                }
            };
			var comments = _db.Comments.Aggregate<BsonDocument>(lookup).ToList();
			var data = new List<CommentViewModel>();
			foreach (var e in comments)
			{
				var c = new CommentViewModel();
				c._id = ((ObjectId)e["_id"]);
				c.commContent = e["commContent"].ToString();
				c.stoId = e["stoId"].ToInt32();
				c.accId = e["accId"].ToInt32();
                c.commDate = ((DateTime)e["commDate"]);

                c.title = e["stories"].AsBsonArray[0]["title"].ToString();
				c.author = e["stories"].AsBsonArray[0]["author"].ToString();
				c.interpreter = e["stories"].AsBsonArray[0]["interpreter"].ToString();
				c.image = e["stories"].AsBsonArray[0]["image"].ToString();
				c.stoInformation = e["stories"].AsBsonArray[0]["stoInformation"].ToString();
				c.stoStatus = e["stories"].AsBsonArray[0]["stoStatus"].ToString();
                c.numberChap = e["stories"].AsBsonArray[0]["numberChap"].ToInt32();
                c.catId = e["stories"].AsBsonArray[0]["catId"].ToInt32();

                c.fullName = e["accounts"].AsBsonArray[0]["fullName"].ToString();
                c.email = e["accounts"].AsBsonArray[0]["email"].ToString();
                c.password = e["accounts"].AsBsonArray[0]["password"].ToString();
                c.accPhone = e["accounts"].AsBsonArray[0]["accPhone"].ToString();
                c.accAddress = e["accounts"].AsBsonArray[0]["accAddress"].ToString();
                c.avatar = e["accounts"].AsBsonArray[0]["avatar"].ToString();
                c.accStatus = e["accounts"].AsBsonArray[0]["accStatus"].ToString();
                c.accType = e["accounts"].AsBsonArray[0]["accType"].ToString();
                data.Add(c);
			}
			return data;
		}

		public CommentViewModel GetCommentFullById(ObjectId key)
		{
            BsonDocument[] lookup = new BsonDocument[2]
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
                },
                new BsonDocument
                {
                    {
                        "$lookup", new BsonDocument
                        {
                            {"from", "accounts"},
                            {"localField", "accId"},
                            {"foreignField", "_id"},
                            {"as", "accounts" }
                        }
                    }
                }
            };
            var comments = _db.Comments.Aggregate<BsonDocument>(lookup).ToList();
            var data = new List<CommentViewModel>();
            foreach (var e in comments)
            {
                var c = new CommentViewModel();
                c._id = ((ObjectId)e["_id"]);
                c.commContent = e["commContent"].ToString();
                c.stoId = e["stoId"].ToInt32();
                c.accId = e["accId"].ToInt32();
                c.commDate = ((DateTime)e["commDate"]);

                c.title = e["stories"].AsBsonArray[0]["title"].ToString();
                c.author = e["stories"].AsBsonArray[0]["author"].ToString();
                c.interpreter = e["stories"].AsBsonArray[0]["interpreter"].ToString();
                c.image = e["stories"].AsBsonArray[0]["image"].ToString();
                c.stoInformation = e["stories"].AsBsonArray[0]["stoInformation"].ToString();
                c.stoStatus = e["stories"].AsBsonArray[0]["stoStatus"].ToString();
                c.numberChap = e["stories"].AsBsonArray[0]["numberChap"].ToInt32();
                c.catId = e["stories"].AsBsonArray[0]["catId"].ToInt32();

                c.fullName = e["accounts"].AsBsonArray[0]["fullName"].ToString();
                c.email = e["accounts"].AsBsonArray[0]["email"].ToString();
                c.password = e["accounts"].AsBsonArray[0]["password"].ToString();
                c.accPhone = e["accounts"].AsBsonArray[0]["accPhone"].ToString();
                c.accAddress = e["accounts"].AsBsonArray[0]["accAddress"].ToString();
                c.avatar = e["accounts"].AsBsonArray[0]["avatar"].ToString();
                c.accStatus = e["accounts"].AsBsonArray[0]["accStatus"].ToString();
                c.accType = e["accounts"].AsBsonArray[0]["accType"].ToString();
                data.Add(c);
            }
            return data.Find(x => x._id == key);
		}

		public Comment GetById(ObjectId key)
		{
			return _db.Comments.Find(x => x._id == key).FirstOrDefault();
		}

		public bool Insert(Comment entity)
		{
			entity.commContent = entity.commContent ?? "";
			_db.Comments.InsertOne(entity);
			return true;
		}

		public bool Update(Comment entity)
		{
			var c = Builders<Comment>.Update.Set("stoId", entity.commContent)
				.Set("stoId", entity.stoId)
				.Set("accId", entity.accId);
            _db.Comments.UpdateOne(x => x._id == entity._id, c);
			return true;
		}

        public List<Comment> Paging(int page, int pageSize, out long totalPage)
        {
            throw new NotImplementedException();
        }

        public List<Comment> SearchPaging(string name, int page, int pageSize, out long totalPage)
        {
            throw new NotImplementedException();
        }
    }
}
