using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongodbAspStory.Models.BusinessModels;
using MongodbAspStory.Models.DataModels;
using MongoDB.Driver;
using MongodbAspStory.Controllers;

namespace MongodbAspStory.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountsAdminController : BaseAController
    {
        IRepositoryAccount _repositoryAccount;
		MongodbAspStoryDbContext _db;
		public AccountsAdminController(IRepositoryAccount repositoryAccount, MongodbAspStoryDbContext db)
        {
            _repositoryAccount = repositoryAccount;
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
                var data = _repositoryAccount.Paging(page, 3, out totalPage);
                ViewBag.totalPage = totalPage;
                ViewBag.page = page;
                return View(data);
            }
            else
            {
                long totalPage;
                var data = _repositoryAccount.SearchPaging(name, page, 3, out totalPage);
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
            var a = _repositoryAccount.GetById(id);
            if (a == null)
            {
                return NotFound();
            }
            return View(a);
        }
        // Sửa
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var a = _repositoryAccount.GetById(id);
            if (a == null)
            {
                return NotFound();
            }
            return View(a);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Account account, string avt)
        {
            TempData["Message"] = "";
            TempData["MessageErr"] = "";
            if (ModelState.IsValid)
            {
                try
                {
                    // xử lý ảnh
                    var files = HttpContext.Request.Form.Files;
                    if (files.Count() > 0 && files[0].Length > 0)
                    {
                        var file = files[0];
                        var FileName = file.FileName;
                        if (account.avatar != null)
                        {
                            string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", account.avatar);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", FileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                            account.avatar = FileName;
                        }
                    }
                    else
                    {
                        account.avatar = avt;
                    }
                    // xử lý sửa
                    _repositoryAccount.Update(account);
                    TempData["Message"] = "Cập nhật thành công";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account._id))
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
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }
		private bool AccountExists(int id)
		{
			return (_db.Accounts.AsQueryable().Any(c => c._id == id));
		}
	}
}
