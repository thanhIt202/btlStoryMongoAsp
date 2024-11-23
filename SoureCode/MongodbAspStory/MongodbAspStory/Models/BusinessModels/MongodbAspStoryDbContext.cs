using MongoDB.Driver;
using MongodbAspStory.Models.DataModels;

namespace MongodbAspStory.Models.BusinessModels
{
    public class MongodbAspStoryDbContext
    {
        IConfiguration _configuration;
        public MongodbAspStoryDbContext(IConfiguration Configuration)
        {
            this._configuration = Configuration;
        }
        // ket noi database
        public IMongoDatabase Connection
        {
            get
            {
                var client = new MongoClient(_configuration.GetConnectionString("MongoConnection"));
                var database = client.GetDatabase(_configuration.GetConnectionString("database"));
                return database;
            }
        }
        // thuoc tinh collection
        public IMongoCollection<Counter> Counters => Connection.GetCollection<Counter>("counters");
        public IMongoCollection<Category> Categories => Connection.GetCollection<Category>("categories");
		public IMongoCollection<Account> Accounts => Connection.GetCollection<Account>("accounts");
		public IMongoCollection<Story> Stories => Connection.GetCollection<Story>("stories");
        public IMongoCollection<Chapter> Chapters => Connection.GetCollection<Chapter>("chapters");
        public IMongoCollection<Comment> Comments => Connection.GetCollection<Comment>("comments");
    }
}
