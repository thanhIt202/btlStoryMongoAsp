using System.ComponentModel.DataAnnotations;

namespace MongodbAspStory.Models.DataModels
{
    public class Chapter
    {
        public int _id { get; set; }

        [Required(ErrorMessage = "Chapter Name không thể để trống! Vui lòng nhập đầy đủ thông tin")]
        public string? chapName { get; set; }
        public string? chapContent { get; set; }
        public int stoId { get; set; }
        public DateTime chapDate { get; set; }
    }
}
