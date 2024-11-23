using System.ComponentModel.DataAnnotations;

namespace MongodbAspStory.Models.DataModels
{
    public class Account
    {
        public int _id { get; set; }

        [Required(ErrorMessage = "Account Name không thể để trống! Vui lòng nhập đầy đủ thông tin")]
        public string? fullName { get; set; }

        [Required(ErrorMessage = "Email không thể để trống! Vui lòng nhập đầy đủ thông tin")]
        public string? email { get; set; }

        [Required(ErrorMessage = "Password không thể để trống! Vui lòng nhập đầy đủ thông tin")]
        public string? password { get; set; }

        [Required(ErrorMessage = "Phone Number không thể để trống! Vui lòng nhập đầy đủ thông tin")]
        [MinLength(10, ErrorMessage = "Phone Number phải có ít nhất 10 chữ số")]
        [MaxLength(10, ErrorMessage = "Phone Number tối đa 10 chữ số")]
        public string? accPhone { get; set; }
        public string? accAddress { get; set; }
        public string? avatar { get; set; }
        public string? accStatus { get; set; }
        public string? accType { get; set; }
    }
}
