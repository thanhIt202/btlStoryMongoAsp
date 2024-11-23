
using System.ComponentModel.DataAnnotations;

namespace MongodbAspStory.Models.DataModels
{
    public class Story
    {
        public int _id { get; set; }

        [Required(ErrorMessage = "Story Name không thể để trống! Vui lòng nhập đầy đủ thông tin")]
        public string? title { get; set; }

        [Required(ErrorMessage = "Story author không thể để trống! Vui lòng nhập đầy đủ thông tin")]
        public string? author { get; set; }
		public string? interpreter { get; set; }
        public string? image { get; set; }
		public string? stoInformation { get; set; }
		public string? stoStatus { get; set; }

        [Required(ErrorMessage = "Number Chap không thể để trống! Vui lòng nhập đầy đủ thông tin")]
        public int numberChap { get; set; }
        public int catId { get; set; }
	}
}
