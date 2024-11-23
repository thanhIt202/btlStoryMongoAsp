using MongodbAspStory.Models.DataModels;
using MongoDB.Driver;

namespace MongodbAspStory.Models.BusinessModels
{
    public class RepositoryCategory : IRepositoryCategory
    {
        MongodbAspStoryDbContext _db;
        public RepositoryCategory(MongodbAspStoryDbContext db)
        {
            _db = db;
        }
        public bool Delete(int key)
        {
            try
            {
                _db.Categories.DeleteOne(c => c._id == key);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public List<Category> GetAll()
        {
            return _db.Categories.Find(FilterDefinition<Category>.Empty).ToList();
        }

        public Category GetById(int key)
        {
            return _db.Categories.Find(x => x._id == key).FirstOrDefault();
        }


        public bool Insert(Category entity)
        {
            try
            {
                entity.catInformation = entity.catInformation ?? "";

                // Get the next available id from the counters collection
                var counter = _db.Counters.FindOneAndUpdate(
                    Builders<Counter>.Filter.Eq(c => c._id, "categories"),
                    Builders<Counter>.Update.Inc(c => c.seq, 1),
                    new FindOneAndUpdateOptions<Counter> { IsUpsert = true, ReturnDocument = ReturnDocument.After }
                );

                // Set the entity's _id to the incremented value
                entity._id = counter.seq;

                // Insert the category
                _db.Categories.InsertOne(entity);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message); // Add proper logging
                return false;
            }
        }



        public List<Category> Paging(int page, int pageSize, out long totalPage)
        {
            int skip = pageSize * (page - 1);
            long rows = _db.Categories.CountDocuments(FilterDefinition<Category>.Empty);
            totalPage = rows % pageSize == 0 ? rows / pageSize : (rows / pageSize) + 1;
            return _db.Categories.Find(FilterDefinition<Category>.Empty).Skip(skip).Limit(pageSize).ToList();
        }

        public List<Category> SearchPaging(string name, int page, int pageSize, out long totalPage)
        {
            int skip = pageSize * (page - 1);
            long rows = _db.Categories.CountDocuments(c => c.catName.ToLower().Contains(name.ToLower()));
            totalPage = rows % pageSize == 0 ? rows / pageSize : (rows / pageSize) + 1;
            return _db.Categories.Find(c => c.catName.ToLower().Contains(name.ToLower())).Skip(skip).Limit(pageSize).ToList();
        }

        public bool Update(Category entity)
        {
            var c = Builders<Category>.Update.Set("catName", entity.catName)
                .Set("catInformation", entity.catInformation)
                .Set("catStatus", entity.catStatus);
			_db.Categories.UpdateOne(x => x._id == entity._id, c);
            return true;
        }
    }
}
