using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS.Controllers
{
    public class AdministratorController : Controller
    {
        private readonly LMSContext db;

        public AdministratorController(LMSContext _db)
        {
            db = _db; // db context
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Department(string subject)
        {
            ViewData["subject"] = subject;
            return View();
        }

        public IActionResult Course(string subject, string num)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }

        /*******Begin code to modify********/

        /// <summary>
        /// Create a department which is uniquely identified by it's subject code
        /// </summary>
        /// <param name="subject">the subject code</param>
        /// <param name="name">the full name of the department</param>
        /// <returns>A JSON object containing {success = true/false}.
        /// false if the department already exists, true otherwise.</returns>
        public IActionResult CreateDepartment(string subject, string name)
        {
            try
            {
                // step 1: create a new dapartment item so we can add a row:
                Department dept = new Department()
                {
                    Subject = subject,
                    Name = name,
                };
                db.Departments.Add(dept);
                db.SaveChanges();
                return Json(new { success = false });
            }
            catch (Exception)
            {
                // failed: already exits or somthing
                return Json(new { success = false });
            }
        }

        /// <summary>
        /// Returns a JSON array of all the courses in the given department.
        /// Each object in the array should have the following fields:
        /// "number" - The course number (as in 5530)
        /// "name" - The course name (as in "Database Systems")
        /// </summary>
        /// <param name="subjCode">The department subject abbreviation (as in "CS")</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetCourses(string subject)
        {

            var courses = db.Courses.Where( c => c.Subject == subject )
                .Select( c => new { number = c.Num, name = c.Name } )
                .ToList();

            return Json(courses);
        }

        /// <summary>
        /// Returns a JSON array of all the professors working in a given department.
        /// Each object in the array should have the following fields:
        /// "lname" - The professor's last name
        /// "fname" - The professor's first name
        /// "uid" - The professor's uid
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetProfessors(string subject)
        {
            var professors = db.Professors.Where( p => p.Subject == subject )
                .Select( p => new { lname = p.LastName, fname = p.FirstName, uid = p.UId } )
                .ToList();
            return Json(professors);
           
        }



        /// <summary>
        /// Creates a course.
        /// A course is uniquely identified by its number + the subject to which it belongs
        /// </summary>
        /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
        /// <param name="number">The course number</param>
        /// <param name="name">The course name</param>
        /// <returns>A JSON object containing {success = true/false}.
        /// false if the course already exists, true otherwise.</returns>
        public IActionResult CreateCourse(string subject, int number, string name)
        {           
            try
            {
                // step 1: create a new Course item so we can add a row:
                Course course = new Course()
                {
                    CourseId = 0,
                    Name = name,
                    Num = (uint)number,
                    Subject = subject,
                    Classes = new HashSet<Class>(),
                    // this can be null if the department doesn't exist, but we can still add the course and fix it later:
                    SubjectNavigation = db.Departments.Where( d => d.Subject == subject ).FirstOrDefault()
                };

                db.Courses.Add(course);
                db.SaveChanges();
                return Json(new { success = false });
            }
            catch (Exception)
            {
                // failed: already exits or somthing
                return Json(new { success = false });
            }
        }



        /// <summary>
        /// Creates a class offering of a given course.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="number">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="start">The start time</param>
        /// <param name="end">The end time</param>
        /// <param name="location">The location</param>
        /// <param name="instructor">The uid of the professor</param>
        /// <returns>A JSON object containing {success = true/false}. 
        /// false if another class occupies the same location during any time 
        /// within the start-end range in the same semester, or if there is already
        /// a Class offering of the same Course in the same Semester,
        /// true otherwise.</returns>
        public IActionResult CreateClass(string subject, int number, string season, int year, DateTime start, DateTime end, string location, string instructor)
        {
            try
            {
                // step 1: create a new class item so we can add a row:
                Class newclass = new Class()
                {
                    ClassId = 0,
                    SemesterSeason = season,
                    SemesterYear = (uint)year,
                    Start = TimeOnly.FromDateTime(start),
                    End = TimeOnly.FromDateTime(end), 
                    Location = location,
                    Professor = instructor,
                    AssignmentCategories = new HashSet<AssignmentCategory>(),
                    Enrolleds = new HashSet<Enrolled>(),

                    // these can be null if the course or professor doesn't exist, but we can still add the class and fix it later:
                    CourseId = db.Courses.Where( c => c.Subject == subject && c.Num == number ).FirstOrDefault().CourseId,
                    Course = db.Courses.Where(c => c.Subject == subject && c.Num == number).FirstOrDefault(), 
                    ProfessorNavigation = db.Professors.Where( p => p.UId == instructor ).FirstOrDefault(),

                };

                db.Classes.Add(newclass);
                db.SaveChanges();
                return Json(new { success = false });
            }
            catch (Exception)
            {
                // failed: already exits or somthing
                return Json(new { success = false });
            }
        }


        /*******End code to modify********/

    }
}

