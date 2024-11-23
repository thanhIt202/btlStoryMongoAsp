namespace MongodbAspStory.Models.ViewModels
{
    public class ChapterViewModel
    {
		public int _id { get; set; }
        public string? chapName { get; set; }
        public string? chapContent { get; set; }
        public int stoId { get; set; }
        public DateTime chapDate { get; set; }

        public string? title { get; set; }
        public string? author { get; set; }
        public string? interpreter { get; set; }
        public string? image { get; set; }
        public string? stoInformation { get; set; }
        public string? stoStatus { get; set; }
        public int numberChap { get; set; }
        public int catId { get; set; }
    }
}
