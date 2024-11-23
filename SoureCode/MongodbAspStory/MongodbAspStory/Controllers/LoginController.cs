using MongodbAspStory.Models.BusinessModels;
using MongodbAspStory.Models.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SharpCompress.Common;


namespace MongodbAspStory.Controllers
{
    public class LoginController : Controller
    {
        MongodbAspStoryDbContext _db;
        public LoginController(MongodbAspStoryDbContext db)
        {
            _db = db;
        }

        // dang nhap
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(Login model)
        {
            TempData["MessageErr"] = "";

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                // Filter for regular user accounts (user type, active status)
                var filterUser = Builders<Account>.Filter.Eq(a => a.email, model.Email)
                                 & Builders<Account>.Filter.Eq(a => a.password, model.Password)
                                 & Builders<Account>.Filter.Eq(a => a.accType, "user")
                                 & Builders<Account>.Filter.Eq(a => a.accStatus, "Đang hoạt động"); // Active user

                // Filter for admin accounts (admin type)
                var filterAdmin = Builders<Account>.Filter.Eq(a => a.email, model.Email)
                                  & Builders<Account>.Filter.Eq(a => a.password, model.Password)
                                  & Builders<Account>.Filter.Eq(a => a.accType, "admin");

                // Check for user account (active user)
                var checkUser = _db.Accounts.Find(filterUser).FirstOrDefault();

                // Check for admin account
                var checkAdmin = _db.Accounts.Find(filterAdmin).FirstOrDefault();

                // Check if account exists and is active
                var checkAccount = _db.Accounts.Find(Builders<Account>.Filter.Eq(a => a.email, model.Email)
                                                    & Builders<Account>.Filter.Eq(a => a.password, model.Password)).FirstOrDefault();

                if (checkUser != null)  // Regular user login
                {
                    // Set session variables for user
                    HttpContext.Session.Clear();
                    HttpContext.Session.SetInt32("LoginId", checkUser._id);
                    HttpContext.Session.SetString("LoginName", checkUser.fullName);
                    HttpContext.Session.SetString("LoginEmail", checkUser.email);
                    HttpContext.Session.SetString("LoginPhone", checkUser.accPhone);
                    HttpContext.Session.SetString("LoginAddress", checkUser.accAddress);
                    HttpContext.Session.SetString("LoginAvatar", checkUser.avatar);
                    HttpContext.Session.SetString("LoginType", checkUser.accType);

                    // Redirect to Home for regular user
                    return RedirectToAction("Index", "Home");
                }
                else if (checkAdmin != null)  // Admin login
                {
                    // Set session variables for admin
                    HttpContext.Session.Clear();
                    HttpContext.Session.SetInt32("LoginId", checkAdmin._id);
                    HttpContext.Session.SetString("LoginName", checkAdmin.fullName);
                    HttpContext.Session.SetString("LoginAvatar", checkAdmin.avatar);

                    // Redirect to the HomeAdmin controller in the Admin area
                    return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });
                }
                else if (checkAccount != null && checkAccount.accStatus != "Đang hoạt động")  // Account not active
                {
                    HttpContext.Session.SetString("LoginStatus", checkAccount.accStatus);
                    TempData["MessageErr"] = "Tài khoản của bạn không hoạt động. Vui lòng kiểm tra lại trạng thái tài khoản.";
                }
                else  // Invalid login credentials
                {
                    HttpContext.Session.Clear();
                    TempData["MessageErr"] = "Thông tin đăng nhập không chính xác hoặc không tồn tại.";
                }
            }

            return View();
        }

        // dang xuat
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("Login");
            return RedirectToAction("Index", "Home");
        }
        // them moi
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Account account)
        {
            TempData["Message"] = "";

            // Check if account name already exists in the database
            var existingName = _db.Accounts
    .Find(a => a.fullName.ToLower() == account.fullName.ToLower() && a._id != account._id)
    .FirstOrDefault();
            var existingEmail = _db.Accounts
    .Find(a => a.email.ToLower() == account.email.ToLower() && a._id != account._id)
    .FirstOrDefault();

            // In the Create POST action method:
            if (existingName != null)
            {
                ViewBag.errorName = "The name already exists";
                return View(account); // Return to the Create view with the error message
            }
            if (existingEmail != null)
            {
                ViewBag.errorEmail = "The email already exists";
                return View(account); // Return to the Create view with the error message
            }

            if (ModelState.IsValid)
            {
                // xử lý ảnh
                var files = HttpContext.Request.Form.Files;
                if (files.Count() > 0 && files[0].Length > 0)
                {
                    var file = files[0];
                    var FileName = file.FileName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        account.avatar = FileName;
                    }
                }
                // xử lý thêm mới
                account.accAddress = account.accAddress ?? "";
                account.avatar = account.avatar ?? "";

                // Get the next available id from the counters collection
                var counter = _db.Counters.FindOneAndUpdate(
                    Builders<Counter>.Filter.Eq(c => c._id, "accounts"),
                    Builders<Counter>.Update.Inc(c => c.seq, 1),
                    new FindOneAndUpdateOptions<Counter> { IsUpsert = true, ReturnDocument = ReturnDocument.After }
                );

                // Set the account _id to the incremented value
                account._id = counter.seq;

                _db.Accounts.InsertOne(account);
                TempData["Message"] = "Thêm mới thành công";
            }
            return RedirectToAction("Index", "Login");
        }
        // thong tin tai khoan
        [HttpGet]
        public IActionResult AccountInformation(int? id)
        {
            if (id == null || _db.Accounts == null)
            {
                return NotFound();
            }
            var a = _db.Accounts.Find(x => x._id == id).FirstOrDefault();
            if (a == null)
            {
                return NotFound();
            }
            return View(a);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AccountInformation(Account account, string avt)
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
                    var a = Builders<Account>.Update.Set("fullName", account.fullName)
                    .Set("email", account.email)
                    .Set("password", account.password)
                    .Set("accPhone", account.accPhone)
                    .Set("accAddress", account.accAddress)
                    .Set("avatar", account.avatar)
                    .Set("accStatus", account.accStatus)
                    .Set("accType", account.accType);
                    _db.Accounts.UpdateOne(x => x._id == account._id, a);
                    TempData["Message"] = "Cập nhật thành công";
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["MessageErr"] = "Lỗi to! Cập nhật không thành công";
                }
                return RedirectToAction("AccountInformation", "Login");
            }
            return View(account);
        }
        private bool AccountExists(int id)
        {
            return (_db.Accounts.AsQueryable().Any(a => a._id == id));
        }
    }
}