
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongodbAspStory.Models.BusinessModels;
using MongodbAspStory.Models.DataModels;
using MongoDB.Driver;
using MongodbAspStory.Controllers;

namespace MongodbAspStory.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesAdminController : BaseAController
    {
        IRepositoryCategory _repositoryCategory;
		MongodbAspStoryDbContext _db;
		public CategoriesAdminController(IRepositoryCategory repositoryCategory, MongodbAspStoryDbContext db)
        {
            _repositoryCategory = repositoryCategory;
            _db = db;
        }

        // Hiển thị phân sang và tìm kiếm
        [HttpGet]
        public IActionResult Index(string name, int page = 1)
        {
            page = page < 1 ? 1 : page;
            if (string.IsNullOrEmpty(name))
            {
                long totalPage;
                var data = _repositoryCategory.Paging(page, 7, out totalPage);
                ViewBag.totalPage = totalPage;
                ViewBag.page = page;
                return View(data);
            }
            else
            {
                long totalPage;
                var data = _repositoryCategory.SearchPaging(name, page, 7, out totalPage);
                ViewBag.totalPage = totalPage;
                ViewBag.page = page;
                ViewBag.name = name;
                return View(data);
            }
        }
        // Chi tiết
        [HttpGet]
        public IActionResult Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var c = _repositoryCategory.GetById(id);
            if (c == null)
            {
                return NotFound();
            }
            return View(c);
        }
        // Thêm mới
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            TempData["Message"] = "";

            // Check if category name already exists in the database
            var existingCategory = _db.Categories
            .Find(c => c.catName.ToLower() == category.catName.ToLower() && c._id != category._id)
            .FirstOrDefault();
            // In the Create POST action method:
            if (existingCategory != null)
            {
                ViewBag.errorName = "The category name already exists";
                return View(category); // Return to the Create view with the error message
            }

            if (ModelState.IsValid)
            {
                // Process the insertion
                _repositoryCategory.Insert(category);
                TempData["Message"] = "Thêm mới thành công";
                return RedirectToAction(nameof(Index)); // Redirect to the Index page after successful insertion
            }

            return View(category); // Return the view if model state is invalid
        }

        // Sửa
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var c = _repositoryCategory.GetById(id);
            if (c == null)
            {
                return NotFound();
            }
            return View(c);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            TempData["Message"] = "";
            TempData["MessageErr"] = "";

            // Check if there is another category with the same name (excluding the current category being edited)
            var existingCategory = _db.Categories
    .Find(c => c.catName.ToLower() == category.catName.ToLower() && c._id != category._id)
    .FirstOrDefault();


            // In the Edit PUT action method:
            if (existingCategory != null)
            {
                ViewBag.errorName = "The category name already exists";
                return View(category); // Return to the Create view with the error message
            }


            if (ModelState.IsValid)
            {
                try
                {
                    // Process the update
                    _repositoryCategory.Update(category);
                    TempData["Message"] = "Cập nhật thành công";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category._id))
                    {
                        TempData["MessageErr"] = "Lỗi to! Cập nhật không thành công";
                        return NotFound();
                    }
                    else
                    {
                        TempData["MessageErr"] = "Lỗi to! Cập nhật không thành công";
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)); // Redirect to Index page after successful update
            }

            return View(category); // Return to the Edit view if model state is invalid
        }
        // Xóa
        [HttpGet]
        public IActionResult Delete(int id)
        {
            TempData["Message"] = "";
            TempData["MessageErr"] = "";
            if (id == null)
            {
                return NotFound();
            }
            var stories = _db.Stories.Find(Builders<Story>.Filter.Eq(s => s.catId, id)).Any();
            if (stories)
            {
                TempData["MessageErr"] = "Không được xoá thể loại này vì đang chứa Stories";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                _repositoryCategory.Delete(id);
                TempData["Message"] = "Xóa thành công";
            }
            catch (Exception)
            {
                TempData["MessageErr"] = "Lỗi to! Xóa không thành công";
            }
            return RedirectToAction(nameof(Index));
        }
		private bool CategoryExists(int id)
		{
			return (_db.Categories.AsQueryable().Any(c => c._id == id));
		}
	}
}
