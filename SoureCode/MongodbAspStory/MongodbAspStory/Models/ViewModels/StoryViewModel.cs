namespace MongodbAspStory.Models.ViewModels
{
    public class StoryViewModel
    {
		public int _id { get; set; }
        public string? title { get; set; }
        public string? author { get; set; }
        public string? interpreter { get; set; }
        public string? image { get; set; }
        public string? stoInformation { get; set; }
        public string? stoStatus { get; set; }
        public int numberChap { get; set; }
        public int catId { get; set; }

        public string? catName { get; set; }
        public string? catInformation { get; set; }
        public int catStatus { get; set; }
    }
}
