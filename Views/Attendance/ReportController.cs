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
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserID") == null)
                return RedirectToAction("Login", "Account");

            var studentBLL = new StudentBLL();
            ViewBag.Departments = studentBLL.GetDepartments();
            ViewBag.Courses = studentBLL.GetCoursesByDept(0);
            return View();
        }

        [HttpGet]
        public IActionResult GetReport(int courseId)
        {
            var list = new List<AttendanceReport>();
            using (SqlConnection con = DbHelper.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("sp_GetAttendanceReport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CourseID", courseId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new AttendanceReport
                    {
                        RegNo = row["RegNo"].ToString(),
                        FullName = row["FullName"].ToString(),
                        TotalPresent = (int)row["TotalPresent"],
                        TotalAbsent = (int)row["TotalAbsent"],
                        TotalClasses = (int)row["TotalClasses"],
                        Percentage = (decimal)row["Percentage"]
                    });
                }
            }
            return Json(list);
        }
    }
}