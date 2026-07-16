using Microsoft.AspNetCore.Mvc;
using SAMS.BLL;
using SAMS.DAL;
using SAMS.Models.Entities;

namespace SmartAttendanceSystem.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserID") == null)
                return RedirectToAction("Login", "Account");

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            return View();
        }
    }
}
