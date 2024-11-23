using System.ComponentModel.DataAnnotations;

namespace MongodbAspStory.Models.DataModels
{
    public class Category
    {
        [Required(ErrorMessage = "Category Name không thể để trống! Vui lòng nhập đầy đủ thông tin")]
        public int _id { get; set; }

        [Required(ErrorMessage = "Category Name không thể để trống! Vui lòng nhập đầy đủ thông tin")]
        public string? catName { get; set; }
        public string? catInformation { get; set; }
        public int catStatus { get; set; }
    }
}
