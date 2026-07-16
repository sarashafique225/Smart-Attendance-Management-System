using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SAMS.BLL;
using SAMS.DAL;
using SAMS.Models.Entities;
using System.Collections.Generic;
using System.Data;

namespace SmartAttendanceSystem.Controllers
{
    public class AttendanceController : Controller
    {
        private StudentBLL _studentBLL = new StudentBLL();
        private AttendanceBLL _bll = new AttendanceBLL();

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserID") == null)
                return RedirectToAction("Login", "Account");

            ViewBag.Departments = _studentBLL.GetDepartments();
            ViewBag.Courses = _studentBLL.GetCoursesByDept(0);
            return View();
        }

        public IActionResult Mark(int courseId = 1, int deptID = 0, int semester = 0)
        {
            if (HttpContext.Session.GetString("UserID") == null)
                return RedirectToAction("Login", "Account");

            var students = _studentBLL.GetFilteredStudents(deptID, semester);
            ViewBag.Departments = _studentBLL.GetDepartments();

            ViewBag.Courses = _studentBLL.GetCoursesByDept(deptID) ?? new List<SAMS.Models.Entities.Course>();

            ViewBag.CourseId = courseId;
            ViewBag.SelectedDept = deptID;
            ViewBag.SelectedSemester = semester;
            ViewBag.Date = System.DateTime.Today.ToString("yyyy-MM-dd");
            return View(students);
        }
        
        [HttpPost]
        public IActionResult SaveAttendance([FromBody] AttendanceSaveModel model)
        {
            if (HttpContext.Session.GetString("UserID") == null)
                return Json(new { success = false, message = "Not authorized" });

            int teacherID = int.Parse(HttpContext.Session.GetString("UserID"));
            var result = _bll.SaveAttendance(model, teacherID);
            return Json(result);
        }
    }
}