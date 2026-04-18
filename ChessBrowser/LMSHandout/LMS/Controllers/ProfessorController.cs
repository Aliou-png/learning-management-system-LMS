using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS_CustomIdentity.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : Controller
    {

        private readonly LMSContext db;

        public ProfessorController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            var students = db.Enrolleds
                .Where(e =>
                    .Where(e =>
                        e.ClassNavigation.CourseNavigation.Subject == subject &&
                        e.ClassNavigation.CourseNavigation.Num == num &&
                        e.ClassNavigation.SemesterSeason == season &&
                        e.ClassNavigation.SemesterYear == year)
                .Select(e => new
                {
                    fname = e.UIdNavigation.FirstName,
                    lname = e.UIdNavigation.LastName,
                    uid = e.UId,
                    dob = e.UIdNavigation.Dob,
                    grade = e.Grade
                })
                .ToList();

            return Json(students);
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            var query = db.Assignments
                .Where(a => a.CategoryNavigation.ClassNavigation.Course.Subject == subject &&
                            a.CategoryNavigation.ClassNavigation.Course.Num == num &&
                            a.CategoryNavigation.ClassNavigation.SemesterSeason == season &&
                            a.CategoryNavigation.ClassNavigation.SemesterYear == year);

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(a => a.CategoryNavigation.Category == category);
            }

            var assignments = query
                .Select(a => new
                {
                    aname = a.Name,
                    cname = a.CategoryNavigation.Category,
                    due = a.Due,
                    submissions = a.Submissions.Count()
                })
                .ToList();

            return Json(assignments);
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            var cats = db.AssignmentCategories
                .Where(c =>
                    c.ClassNavigation.Course.Subject == subject &&
                    c.ClassNavigation.Course.Num == num &&
                    c.ClassNavigation.SemesterSeason == season &&
                    c.ClassNavigation.SemesterYear == year)
                .Select(c => new
                {
                    name = c.Category, 
                    weight = c.Weight
                })
                .ToList();

            return Json(cats);
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// If a category of the given class with the given name already exists, return success = false.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false} </returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            var cls = db.Classes
                 .FirstOrDefault(c =>
                     c.CourseNavigation.Subject == subject &&
                     c.CourseNavigation.Num == num &&
                     c.SemesterSeason == season &&
                     c.SemesterYear == year);

            if (cls == null)
                return Json(new { success = false });

            if (db.AssignmentCategories.Any(c => c.ClassId == cls.ClassId && c.Category == category))
                return Json(new { success = false });

            db.AssignmentCategories.Add(new AssignmentCategory
            {
                Category = category,        // not Name
                Weight = (uint)catweight,
                ClassId = cls.ClassId       // not InClass
            });

            db.SaveChanges();
            return Json(new { success = true });
        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            var cat = db.AssignmentCategories
           .FirstOrDefault(c =>
               c.Category == category &&
               c.ClassNavigation.Course.Subject == subject &&
               c.ClassNavigation.Course.Num == num &&
               c.ClassNavigation.SemesterSeason == season &&
               c.ClassNavigation.SemesterYear == year);

            if (cat == null)
                return Json(new { success = false });

            if (db.Assignments.Any(a => a.Name == asgname && a.CategoryId == cat.CategoryId))
                return Json(new { success = false });

            db.Assignments.Add(new Assignment
            {
                Name = asgname,
                MaxPoints = (uint)asgpoints,
                Due = asgdue,
                Contents = asgcontents,
                CategoryId = cat.CategoryId
            });

            db.SaveChanges();

            var students = db.Enrolleds
                .Where(e => e.ClassId == cat.ClassId)
                .Select(e => e.UId)
                .ToList();

            foreach (var s in students)
                UpdateStudentGrade(cat.ClassId, s);

            db.SaveChanges();
            return Json(new { success = true });
        }


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            var subs = db.Submissions
                .Where(s =>
                    s.AssignmentNavigation.Name == asgname &&
                    s.AssignmentNavigation.CategoryNavigation.Category == category &&  // not .Name
                    s.AssignmentNavigation.CategoryNavigation.ClassNavigation.Course.Subject == subject &&
                    s.AssignmentNavigation.CategoryNavigation.ClassNavigation.Course.Num == num &&
                    s.AssignmentNavigation.CategoryNavigation.ClassNavigation.SemesterSeason == season &&
                    s.AssignmentNavigation.CategoryNavigation.ClassNavigation.SemesterYear == year)
                .Select(s => new
                {
                    fname = s.StudentNavigation.FirstName,
                    lname = s.StudentNavigation.LastName,
                    uid = s.Student,      // not s.UId
                    time = s.Time,        // not s.Submitted
                    score = s.Score
                })
                .ToList();

            return Json(subs);
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            var sub = db.Submissions.FirstOrDefault(s =>
                s.Student == uid &&
                s.AssignmentNavigation.Name == asgname &&
                s.AssignmentNavigation.CategoryNavigation.Category == category &&
                s.AssignmentNavigation.CategoryNavigation.ClassNavigation.Course.Subject == subject &&
                s.AssignmentNavigation.CategoryNavigation.ClassNavigation.Course.Num == num &&
                s.AssignmentNavigation.CategoryNavigation.ClassNavigation.SemesterSeason == season &&
                s.AssignmentNavigation.CategoryNavigation.ClassNavigation.SemesterYear == year);

            if (sub == null)
                return Json(new { success = false });

            sub.Score = (uint)score;  // cast needed
            db.SaveChanges();

            uint classId = sub.AssignmentNavigation.CategoryNavigation.ClassId;  // uint not int
            UpdateStudentGrade(classId, uid);

            db.SaveChanges();

            return Json(new { success = true });
        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var classes = db.Classes
                .Where(c => c.Professor == uid)
                .Select(c => new
                {
                    subject = c.CourseNavigation.Subject,
                    number = c.CourseNavigation.Num,
                    name = c.CourseNavigation.Name,
                    season = c.SemesterSeason,
                    year = c.SemesterYear
                })
                .ToList();

            return Json(classes);
        }

        private void UpdateStudentGrade(uint classId, string uid)
        {
            var categories = db.AssignmentCategories
                .Where(c => c.ClassId == classId)
                .ToList();

            double totalWeighted = 0;
            double totalWeights = 0;

            foreach (var cat in categories)
            {
                var assignments = db.Assignments
                    .Where(a => a.CategoryId == cat.CategoryId)
                    .ToList();

                if (!assignments.Any())
                    continue;

                double earned = 0;
                double possible = 0;

                foreach (var asg in assignments)
                {
                    possible += asg.MaxPoints;

                    var sub = db.Submissions
                        .FirstOrDefault(s => s.Assignment == asg.AssignmentId && s.Student == uid);

                    if (sub != null)
                        earned += sub.Score;
                }

                if (possible == 0) continue;

                double percent = earned / possible;
                totalWeighted += percent * cat.Weight;
                totalWeights += cat.Weight;
            }

            string letter = "--";

            if (totalWeights > 0)
            {
                double final = totalWeighted * (100.0 / totalWeights);
                letter = PercentToLetter(final);
            }

            var enroll = db.Enrolleds
                .FirstOrDefault(e => e.ClassId == classId && e.UId == uid);
            if (enroll != null)
                enroll.Grade = letter;
        }

        private string PercentToLetter(double pct)
        {
            return pct switch
            {
                >= 93 => "A",
                >= 90 => "A-",
                >= 87 => "B+",
                >= 83 => "B",
                >= 80 => "B-",
                >= 77 => "C+",
                >= 73 => "C",
                >= 70 => "C-",
                >= 67 => "D+",
                >= 63 => "D",
                >= 60 => "D-",  
                _ => "F"
            };
        }
        /*******End code to modify********/
    }
}

