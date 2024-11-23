
using System.ComponentModel.DataAnnotations;

namespace MongodbAspStory.Models.DataModels
{
    public class Login
    {
        [Required(ErrorMessage = "Email không thể để trống! Vui lòng nhập đầy đủ thông tin")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password không thể để trống! Vui lòng nhập đầy đủ thông tin")]
        public string? Password { get; set; }
    }
}
