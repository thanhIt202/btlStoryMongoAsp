
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongodbAspStory.Models.BusinessModels;
using MongodbAspStory.Models.DataModels;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongodbAspStory.Controllers;

namespace MongodbAspStory.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ChaptersAdminController : BaseAController
    {
        IRepositoryChapter _repositoryChapter;
        IRepositoryStory _repositoryStory;
        MongodbAspStoryDbContext _db;
		public ChaptersAdminController(IRepositoryChapter repositoryChapter, MongodbAspStoryDbContext db, IRepositoryStory repositoryStory)
        {
            _repositoryChapter = repositoryChapter;
            _db = db;
            _repositoryStory = repositoryStory;
        }

        // Hiển thị phân sang và tìm kiếm
        [HttpGet]
        public IActionResult Index(string name, int page = 1)
        {
            page = page < 1 ? 1 : page;
            if (string.IsNullOrEmpty(name))
            {
                long totalPage;
                var data = _repositoryChapter.Paging(page, 7, out totalPage);
                ViewBag.totalPage = totalPage;
                ViewBag.page = page;
                return View(data);
            }
            else
            {
                long totalPage;
                var data = _repositoryChapter.SearchPaging(name, page, 7, out totalPage);
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
            var c = _repositoryChapter.GetChapterFullById(id);
            if (c == null)
            {
                return NotFound();
            }
            return View(c);
        }
        //
        [HttpGet]
        public IActionResult GetChapterFull()
        {
            var data = _repositoryChapter.GetChapterFull();
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }

        // GET: Create
        [HttpGet]
        public IActionResult Create()
        {
            // Populate the story dropdown list
            ViewBag.stoId = new SelectList(_repositoryStory.GetAll(), "_id", "title");
            return View();
        }
        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Chapter chapter)
        {
            TempData["Message"] = "";

            // Check if chapter name already exists in the database
            var existingChapter = _db.Chapters
                .Find(c => c.chapName.ToLower() == chapter.chapName.ToLower() && c._id != chapter._id)
                .FirstOrDefault();

            // If the chapter name exists, show an error message and re-render the form
            if (existingChapter != null)
            {
                ViewBag.errorName = "The chapter name already exists";
                // Re-populate the dropdown list for story selection
                ViewBag.stoId = new SelectList(_repositoryStory.GetAll(), "_id", "title", chapter.stoId);
                return View(chapter); // Pass the current chapter object back to the view
            }

            // If model is valid, save the new chapter
            if (ModelState.IsValid)
            {
                // Process and insert the new chapter
                _repositoryChapter.Insert(chapter);
                TempData["Message"] = "Chapter added successfully!";
                return RedirectToAction(nameof(Index)); // Redirect to the index page after successful creation
            }

            // If validation fails, return to the view with the current data (including error messages)
            ViewBag.stoId = new SelectList(_repositoryStory.GetAll(), "_id", "title", chapter.stoId);
            return View(chapter); // Return the same chapter object to the view so the user can correct any issues
        }

        // GET: Edit
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // If the id is invalid or not found, return NotFound()
            if (id == 0)
            {
                return NotFound();
            }
            // Retrieve the chapter by id
            var chapter = _repositoryChapter.GetById(id);
            if (chapter == null)
            {
                return NotFound();
            }
            // Populate the story dropdown list and set the selected story based on the chapter's stoId
            ViewBag.stoId = new SelectList(_repositoryStory.GetAll(), "_id", "title", chapter.stoId);

            return View(chapter);
        }
        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Chapter chapter)
        {
            TempData["Message"] = "";
            TempData["MessageErr"] = "";

            // Check if the chapter name already exists in the database
            var existingChapter = _db.Chapters
                .Find(c => c.chapName.ToLower() == chapter.chapName.ToLower() && c._id != chapter._id)
                .FirstOrDefault();
            // If a chapter with the same name already exists, return the error message and preserve the selected story
            if (existingChapter != null)
            {
                ViewBag.errorName = "The chapter name already exists";
                // Re-populate the dropdown for story selection and maintain the selected story ID
                ViewBag.stoId = new SelectList(_repositoryStory.GetAll(), "_id", "title", chapter.stoId);
                return View(chapter); // Return to the Edit view with the error message
            }

            // If the model is valid, update the chapter
            if (ModelState.IsValid)
            {
                try
                {
                    // Update the chapter in the repository
                    _repositoryChapter.Update(chapter);
                    TempData["Message"] = "Chapter updated successfully!";
                    return RedirectToAction(nameof(Index)); // Redirect to the Index page after a successful update
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Handle concurrency exception (when the chapter was deleted or updated by another user)
                    if (!ChapterExists(chapter._id))
                    {
                        TempData["MessageErr"] = "The chapter no longer exists.";
                        return NotFound();
                    }
                    else
                    {
                        TempData["MessageErr"] = "An error occurred while updating the chapter.";
                        throw;
                    }
                }
            }

            // If model validation fails, re-populate the story dropdown and return the view with the current chapter object
            ViewBag.stoId = new SelectList(_repositoryStory.GetAll(), "_id", "title", chapter.stoId);
            return View(chapter); // Return the same chapter object to the view
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
			try
            {
                _repositoryChapter.Delete(id);
                TempData["Message"] = "Xóa thành công";
            }
            catch (Exception)
            {
                TempData["MessageErr"] = "Lỗi to! Xóa không thành công";
            }
            return RedirectToAction(nameof(Index));
        }
		private bool ChapterExists(int id)
		{
			return (_db.Chapters.AsQueryable().Any(c => c._id == id));
		}
	}
}
