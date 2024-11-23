using Microsoft.AspNetCore.Mvc;
using MongodbAspStory.Controllers;

namespace MongodbAspStory.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    public class HomeAdminController : BaseAController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
