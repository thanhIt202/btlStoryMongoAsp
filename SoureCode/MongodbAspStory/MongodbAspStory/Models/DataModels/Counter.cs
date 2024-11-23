namespace MongodbAspStory.Models.DataModels
{
    public class Counter
    {
        public string _id { get; set; } // Use "categories" as the collection name
        public int seq { get; set; } // The auto-incrementing sequence
    }
}
