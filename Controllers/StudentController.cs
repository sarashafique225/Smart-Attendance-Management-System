using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAMS.BLL;
using SAMS.Models.Entities;
using System.Collections.Generic;

namespace SmartAttendanceSystem.Controllers
{
    public class StudentController : Controller
    {
        private StudentBLL _bll = new StudentBLL();

        private bool IsLoggedIn() =>
            HttpContext.Session.GetString("UserID") != null;

        private void LoadDropdowns(int deptID = 0)
        {
            ViewBag.Departments = _bll.GetDepartments();
            ViewBag.Courses = _bll.GetCoursesByDept(deptID);
        }

        public IActionResult Index(int deptID = 0, int semester = 0, int courseID = 0)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            ViewBag.Departments = _bll.GetDepartments();
            ViewBag.Courses = _bll.GetCoursesByDept(deptID);
            ViewBag.SelectedDept = deptID;
            ViewBag.SelectedSemester = semester;
            ViewBag.SelectedCourse = courseID;

            var students = _bll.GetFilteredStudents(deptID, semester);
            return View(students);
        }

        public IActionResult Create()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            LoadDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Student s)
        {
            ModelState.Remove("DeptName");
            if (ModelState.IsValid)
            {
                try
                {
                    var (success, msg) = _bll.AddStudent(s);
                    if (success)
                    {
                        TempData["Success"] = msg;
                        return RedirectToAction("Index");
                    }
                    ViewBag.Error = msg;
                }
                catch (System.Exception ex)
                {
                    ViewBag.Error = "Error: " + ex.Message;
                }
            }
            LoadDropdowns();
            return View(s);
        }

        public IActionResult Edit(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            var students = _bll.GetAllStudents();
            var s = students.Find(x => x.StudentID == id);
            if (s == null) return NotFound();
            LoadDropdowns(s.DeptID);
            return View(s);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Student s)
        {
            ModelState.Remove("DeptName");
            if (ModelState.IsValid)
            {
                var (success, msg) = _bll.UpdateStudent(s);
                TempData[success ? "Success" : "Error"] = msg;
                if (success) return RedirectToAction("Index");
            }
            LoadDropdowns(s.DeptID);
            return View(s);
        }

        [HttpPost]
        public IActionResult DeleteAjax(int id)
        {
            if (!IsLoggedIn())
                return Json(new { success = false, message = "Not authorized" });
            var (success, msg) = _bll.DeleteStudent(id);
            return Json(new { success, message = msg });
        }

        [HttpGet]
        public IActionResult GetCourses(int deptID)
        {
            var courses = _bll.GetCoursesByDept(deptID);
            return Json(courses);
        }
    }
}