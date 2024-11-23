using MongodbAspStory.Models.DataModels;
using MongoDB.Driver;

namespace MongodbAspStory.Models.BusinessModels
{
    public class RepositoryAccount : IRepositoryAccount
	{
        MongodbAspStoryDbContext _db;
        public RepositoryAccount(MongodbAspStoryDbContext db)
        {
            _db = db;
        }

		public bool Delete(int key)
		{
			throw new NotImplementedException();
		}

		public List<Account> GetAll()
		{
			return _db.Accounts.Find(FilterDefinition<Account>.Empty).ToList();
		}

		public Account GetById(int key)
		{
			return _db.Accounts.Find(x => x._id == key).FirstOrDefault();
		}

		public bool Insert(Account entity)
		{
			throw new NotImplementedException();
		}

		public List<Account> Paging(int page, int pageSize, out long totalPage)
		{
			int skip = pageSize * (page - 1);
			long rows = _db.Accounts.CountDocuments(FilterDefinition<Account>.Empty);
			totalPage = rows % pageSize == 0 ? rows / pageSize : (rows / pageSize) + 1;
			return _db.Accounts.Find(FilterDefinition<Account>.Empty).Skip(skip).Limit(pageSize).ToList();
		}

		public List<Account> SearchPaging(string name, int page, int pageSize, out long totalPage)
		{
			int skip = pageSize * (page - 1);
			long rows = _db.Accounts.CountDocuments(a => a.fullName.ToLower().Contains(name.ToLower()));
			totalPage = rows % pageSize == 0 ? rows / pageSize : (rows / pageSize) + 1;
			return _db.Accounts.Find(a => a.fullName.ToLower().Contains(name.ToLower())).Skip(skip).Limit(pageSize).ToList();
		}

		public bool Update(Account entity)
		{
			var a = Builders<Account>.Update.Set("fullName", entity.fullName)
				.Set("email", entity.email)
				.Set("password", entity.password)
				.Set("accPhone", entity.accPhone)
				.Set("accAddress", entity.accAddress)
				.Set("avatar", entity.avatar)
				.Set("accStatus", entity.accStatus)
				.Set("accType", entity.accType);
			_db.Accounts.UpdateOne(x => x._id == entity._id, a);
			return true;
		}
	}
}
