using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongodbAspStory.Controllers;
using MongodbAspStory.Models.BusinessModels;
using MongodbAspStory.Models.DataModels;

namespace BtlManga.Controllers
{
    [Area("Admin")]
    public class StoriesAdminController : BaseAController
    {
        IRepositoryCategory _repositoryCategory;
        IRepositoryStory _repositoryStory;
        MongodbAspStoryDbContext _db;
        public StoriesAdminController(IRepositoryCategory repositoryCategory, IRepositoryStory repositoryStory, MongodbAspStoryDbContext db)
        {
            _repositoryCategory = repositoryCategory;
            _repositoryStory = repositoryStory;
            _db = db;
        }

        // Hiển thị phân sang và tìm kiếm
        [HttpGet]
        public IActionResult Index(string name, int page=1)
        {
            page = page < 1 ? 1 : page;
            if (string.IsNullOrEmpty(name))
            {
                long totalPage;
                var data = _repositoryStory.Paging(page, 3, out totalPage);
                ViewBag.totalPage = totalPage;
                ViewBag.page = page;
                return View(data);
            }
            else
            {
                long totalPage;
                var data = _repositoryStory.SearchPaging(name, page, 3, out totalPage);
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
            var b = _repositoryStory.GetStoryFullById(id);
            if (b == null)
            {
                return NotFound();
            }
            return View(b);
        }

        // GET: Create
        [HttpGet]
        public IActionResult Create()
        {
            // Populate categories for the dropdown list
            ViewBag.catId = new SelectList(_repositoryCategory.GetAll(), "_id", "catName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Story story)
        {
            // Set TempData message to empty initially
            TempData["Message"] = "";

            // Check if the story name already exists in the database
            var existingStory = _db.Stories
                .Find(s => s.title.ToLower() == story.title.ToLower() && s._id != story._id)
                .FirstOrDefault();

            // If the story name exists, show an error message and re-render the form with existing data
            if (existingStory != null)
            {
                ViewBag.errorName = "The story name already exists";
                ViewBag.catId = new SelectList(_repositoryCategory.GetAll(), "_id", "catName", story.catId);
                return View(story);  // Pass the current story object back to the view
            }

            // Validate if the image is selected
            var files = HttpContext.Request.Form.Files;
            if (files.Count == 0 || files[0].Length == 0)
            {
                ViewBag.errorImage = "Please upload an image for the story.";
                ViewBag.catId = new SelectList(_repositoryCategory.GetAll(), "_id", "catName", story.catId);
                return View(story);  // Return the same story object to the view to correct the error
            }

            // If model is valid, proceed with saving the new story
            if (ModelState.IsValid)
            {
                // Process the image (if exists)
                var file = files[0];
                var fileName = file.FileName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                    story.image = fileName;
                }

                // Insert the new story into the repository
                _repositoryStory.Insert(story);
                TempData["Message"] = "Story added successfully!";
                return RedirectToAction(nameof(Index));  // Redirect to the index page after successful creation
            }

            // If validation fails, re-render the form with the existing data
            ViewBag.catId = new SelectList(_repositoryCategory.GetAll(), "_id", "catName", story.catId);
            return View(story);  // Return the same story object to the view so the user can correct any issues
        }

        // GET: Edit
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            // Retrieve the story from the repository
            var story = _repositoryStory.GetById(id);
            if (story == null)
            {
                return NotFound();
            }

            // Populate categories for the dropdown list
            ViewBag.catId = new SelectList(_repositoryCategory.GetAll(), "_id", "catName", story.catId);
            return View(story);
        }
        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Story story, string img)
        {
            TempData["Message"] = "";
            TempData["MessageErr"] = "";

            // Check if the story name already exists in the database
            var existingStory = _db.Stories
                .Find(s => s.title.ToLower() == story.title.ToLower() && s._id != story._id)
                .FirstOrDefault();

            if (existingStory != null)
            {
                // Set error message if the story name already exists
                ViewBag.errorName = "The story name already exists";
                // Repopulate the category dropdown with the selected category
                ViewBag.catId = new SelectList(_repositoryCategory.GetAll(), "_id", "catName", story.catId);
                // Ensure the existing image is maintained in the model
                story.image = img;
                return View(story);
            }

            // If the model is valid, proceed with updating the story
            if (ModelState.IsValid)
            {
                try
                {
                    // Handle image upload only if a new image is provided
                    var files = HttpContext.Request.Form.Files;
                    if (files.Count() > 0 && files[0].Length > 0)  // New image uploaded
                    {
                        var file = files[0];
                        var fileName = file.FileName;
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", fileName);

                        // Delete the old image if a new one is uploaded
                        if (!string.IsNullOrEmpty(story.image))
                        {
                            string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", story.image);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Save the new image
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                            story.image = fileName; // Update the story's image field
                        }
                    }
                    else
                    {
                        // If no new image is uploaded, retain the old image (from the model or passed 'img' value)
                        story.image = string.IsNullOrEmpty(story.image) ? img : story.image;
                    }

                    // Update the story in the repository
                    _repositoryStory.Update(story);
                    TempData["Message"] = "Story updated successfully!";
                    return RedirectToAction(nameof(Index)); // Redirect to the Index page after a successful update
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Handle concurrency exception and display an appropriate message
                    if (!StoryExists(story._id))
                    {
                        TempData["MessageErr"] = "The story does not exist or has been deleted.";
                        return NotFound();
                    }
                    else
                    {
                        TempData["MessageErr"] = "An error occurred while updating the story.";
                        throw;
                    }
                }
            }

            // If model state is invalid, re-populate the category dropdown and return the view with the current story object
            ViewBag.catId = new SelectList(_repositoryCategory.GetAll(), "_id", "catName", story.catId);
            return View(story);
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
                var chapters = _db.Chapters.Find(Builders<Chapter>.Filter.Eq(c => c.stoId, id)).Any();
                if (chapters)
                {
                    TempData["MessageErr"] = "Không được xoá thể loại này vì đang chứa Chapters";
                    return RedirectToAction(nameof(Index));
                }
                
                var b = _repositoryStory.GetById(id);
                if (b == null)
                {
                    return NotFound();
                }
                if (b.image != null)
                {
                    string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", b.image);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _repositoryStory.Delete(id);
                TempData["Message"] = "Xóa thành công";
            }
            catch (Exception)
            {
                TempData["MessageError"] = "Lỗi to! Xóa không thành công";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool StoryExists(int id)
        {
            return (_db.Stories.AsQueryable().Any(c => c._id == id));
        }
    }
}
