using Microsoft.AspNetCore.Mvc;
using SAMS.BLL;
using SAMS.DAL;
using SAMS.Models.Entities;

namespace SmartAttendanceSystem.Controllers
{
    public class AccountController : Controller
    {
        private UserBLL _userBLL = new UserBLL();
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserID") != null)
                return RedirectToAction("Index", "Dashboard");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            var user = _userBLL.Login(email, password);
            if (user != null)
            {
                HttpContext.Session.SetString("UserID", user.UserID.ToString());
                HttpContext.Session.SetString("UserName", user.FullName);
                HttpContext.Session.SetString("UserRole", user.Role);
                return RedirectToAction("Index", "Dashboard");
            }
            ViewBag.Error = "Invalid email or password. Please try again.";
            return View();
        }

        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User u)
        {
            if (ModelState.IsValid)
            {
                var (success, message) = _userBLL.Register(u);
                if (success)
                {
                    TempData["Success"] = message;
                    return RedirectToAction("Login");
                }
                ViewBag.Error = message;
            }
            return View(u);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
