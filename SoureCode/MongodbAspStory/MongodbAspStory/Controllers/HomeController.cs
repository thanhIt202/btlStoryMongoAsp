using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MongoDB.Bson;
using MongoDB.Driver;
using MongodbAspStory.Models;
using MongodbAspStory.Models.BusinessModels;
using MongodbAspStory.Models.DataModels;
using MongodbAspStory.Models.ViewModels;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace MongodbAspStory.Controllers
{
	public class HomeController : Controller
	{
        IRepositoryStory _repositoryStory;
        IRepositoryCategory _repositoryCategory;
        IRepositoryChapter _repositoryChapter;
        MongodbAspStoryDbContext _db;

		public HomeController(IRepositoryStory repositoryStory, MongodbAspStoryDbContext db, IRepositoryCategory repositoryCategory, IRepositoryChapter repositoryChapter)
        {
            _repositoryStory = repositoryStory;
            _db = db;
            _repositoryCategory = repositoryCategory;
            _repositoryChapter = repositoryChapter;
        }

        public IActionResult Index()
		{
            var data = _repositoryStory.GetStoryFull();
            if (data == null)
            {
                return NotFound();
            }
            ViewBag.stoStatus = _db.Stories.Find(Builders<Story>.Filter.Eq(s => s.stoStatus, "Hoàn thành")).ToList();
            ViewBag.categories = _repositoryCategory.GetAll();
            return View(data);
        }

        public IActionResult Story(int id, string name, int page = 1, int pageSize = 7)
        {
            if (id == null)
            {
                return NotFound();
            }
            var s = _repositoryStory.GetStoryFullById(id);
            if (s == null)
            {
                return NotFound();
            }
            page = page < 1 ? 1 : page;
            if (string.IsNullOrEmpty(name))
            {
                int skip = pageSize * (page - 1);
                long rows = _db.Chapters.CountDocuments(Builders<Chapter>.Filter.Eq(x => x.stoId, id));
                long totalPage = rows % pageSize == 0 ? rows / pageSize : (rows / pageSize) + 1;
                ViewBag.chapters = _db.Chapters.Find(Builders<Chapter>.Filter.Eq(x => x.stoId, id)).Skip(skip).Limit(pageSize).ToList();
                ViewBag.totalPage = totalPage;
                ViewBag.page = page;
            }
            else
            {
                int skip = pageSize * (page - 1);
                long rows = _db.Chapters.CountDocuments(Builders<Chapter>.Filter.Regex(c => c.chapName, new MongoDB.Bson.BsonRegularExpression(name, "i")) & Builders<Chapter>.Filter.Eq(x => x.stoId, id));
                long totalPage = rows % pageSize == 0 ? rows / pageSize : (rows / pageSize) + 1;
                ViewBag.chapters = _db.Chapters.Find(Builders<Chapter>.Filter.Regex(c => c.chapName, new MongoDB.Bson.BsonRegularExpression(name, "i")) & Builders<Chapter>.Filter.Eq(x => x.stoId, id)).Skip(skip).Limit(pageSize).ToList();
                ViewBag.totalPage = totalPage;
                ViewBag.page = page;
                ViewBag.name = name;
            }
            ViewBag.categories = _repositoryCategory.GetAll();
            BsonDocument[] lookup1 = new BsonDocument[1]
            {
                new BsonDocument
                {
                    {
                        "$lookup", new BsonDocument
                        {
                            {"from", "categories"},
                            {"localField", "catId"},
                            {"foreignField", "_id"},
                            {"as", "categories" }
                        }
                    }
                }
            };
            var stories = _db.Stories.Aggregate<BsonDocument>(lookup1).ToList();
            var dbo = new List<StoryViewModel>();
            foreach (var e in stories)
            {
                var x = new StoryViewModel();
                x._id = e["_id"].ToInt32();
                x.title = e["title"].ToString();
                x.author = e["author"].ToString();
                x.interpreter = e["interpreter"].ToString();
                x.image = e["image"].ToString();
                x.stoInformation = e["stoInformation"].ToString();
                x.stoStatus = e["stoStatus"].ToString();
                x.numberChap = e["numberChap"].ToInt32();
                x.catId = e["catId"].ToInt32();

                x.catName = e["categories"].AsBsonArray[0]["catName"].ToString();
                x.catStatus = e["categories"].AsBsonArray[0]["catStatus"].ToInt32();
                dbo.Add(x);
            }
            ViewBag.stoCat = dbo.FindAll(x => x.catId == s.catId);
            ViewBag.stoAuthor = dbo.FindAll(x => x.author == s.author);
            ViewBag.stoStatus = dbo.FindAll(x => x.stoStatus == "Hoàn thành");
            BsonDocument[] lookup = new BsonDocument[2]
            {
                new BsonDocument
                {
                    {
                        "$lookup", new BsonDocument
                        {
                            {"from", "stories"},
                            {"localField", "stoId"},
                            {"foreignField", "_id"},
                            {"as", "stories" }
                        }
                    }
                },
                new BsonDocument
                {
                    {
                        "$lookup", new BsonDocument
                        {
                            {"from", "accounts"},
                            {"localField", "accId"},
                            {"foreignField", "_id"},
                            {"as", "accounts" }
                        }
                    }
                }
            };
            var comment = _db.Comments.Aggregate<BsonDocument>(lookup).ToList();
            var data = new List<CommentViewModel>();
            foreach (var e in comment)
            {
                var c = new CommentViewModel();
                c._id = ((ObjectId)e["_id"]);
                c.commContent = e["commContent"].ToString();
                c.stoId = e["stoId"].ToInt32();
                c.accId = e["accId"].ToInt32();
                c.commDate = ((DateTime)e["commDate"]);

                c.title = e["stories"].AsBsonArray[0]["title"].ToString();
                c.author = e["stories"].AsBsonArray[0]["author"].ToString();
                c.interpreter = e["stories"].AsBsonArray[0]["interpreter"].ToString();
                c.image = e["stories"].AsBsonArray[0]["image"].ToString();
                c.stoInformation = e["stories"].AsBsonArray[0]["stoInformation"].ToString();
                c.stoStatus = e["stories"].AsBsonArray[0]["stoStatus"].ToString();
                c.numberChap = e["stories"].AsBsonArray[0]["numberChap"].ToInt32();
                c.catId = e["stories"].AsBsonArray[0]["catId"].ToInt32();

                c.fullName = e["accounts"].AsBsonArray[0]["fullName"].ToString();
                c.email = e["accounts"].AsBsonArray[0]["email"].ToString();
                c.password = e["accounts"].AsBsonArray[0]["password"].ToString();
                c.accPhone = e["accounts"].AsBsonArray[0]["accPhone"].ToString();
                c.accAddress = e["accounts"].AsBsonArray[0]["accAddress"].ToString();
                c.avatar = e["accounts"].AsBsonArray[0]["avatar"].ToString();
                c.accStatus = e["accounts"].AsBsonArray[0]["accStatus"].ToString();
                c.accType = e["accounts"].AsBsonArray[0]["accType"].ToString();
                data.Add(c);
            }
            ViewBag.comments = data.FindAll(x => x.stoId == id);
            return View(s);
        }
        public IActionResult Chapter(int id)
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
        public IActionResult StoCat(int id)
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
            ViewBag.stoCats = _db.Stories.Find(Builders<Story>.Filter.Eq(x => x.catId, id)).ToList();
            return View(c);
        }
        // Create a new comment and redirect back to the story page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CommentCreate(Comment comment)
        {
            if (ModelState.IsValid)
            {
                // Insert the new comment into the database
                _db.Comments.InsertOne(comment);
            }
            // Redirect to the story page, passing the storyId as a parameter
            return RedirectToAction("Story", "Home", new { id = comment.stoId });
        }

        // Delete an existing comment and redirect back to the story page
        [HttpGet]
        public IActionResult CommentDelete(ObjectId id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Comment comment = _db.Comments.Find(c => c._id == id).FirstOrDefault();
            if (comment == null)
            {
                return NotFound();
            }

            try
            {
                // Delete the comment with the specified id
                _db.Comments.DeleteOne(c => c._id == id);
            }
            catch (Exception)
            {
                return NotFound();
            }

            // Redirect to the story page, passing the storyId as a parameter (stoId of the deleted comment)
            return RedirectToAction("Story", "Home", new { id = comment.stoId });
        }



        public IActionResult StoSearch(string name, int page = 1)
        {
            HttpContext.Session.SetString("SearchName", name);
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
