using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private LMSContext db;
        public StudentController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
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


        public IActionResult ClassListings(string subject, string num)
        {
            System.Diagnostics.Debug.WriteLine(subject + num);
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }


        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            // need to Hoin three tables:
            // Enrolled.ClassID --> Classes.classID --> Classes.courseID --> Courses.courseID
            /*Example SQL query:
                 SELECT 
                    c.Subject,
                    cl.classID,
                    c.Name AS course_name,
                    cl.semester_season AS Season,
                    cl.semester_year AS Year,
                    e.Grade
                FROM Enrolled e
                JOIN Classes cl 
                    ON e.ClassID = cl.classID
                JOIN Courses c 
                    ON cl.courseID = c.courseID
                WHERE e.uID = 'uID';
            */
            var student_classes = db.Enrolleds.
                    Join(db.Classes,
                        e => e.ClassId, 
                        cl => cl.ClassId, 
                        (e, cl) => new { e, cl })
                    .Join(db.Courses, 
                        ec => ec.cl.CourseId, 
                        c => c.CourseId, 
                        (ec, c) => new { ec.e, ec.cl, c })
                    .Where(ecc => ecc.e.UId == uid)
                .Select(ecc => new
                {
                    subject = ecc.c.Subject,
                    number = ecc.c.Num,
                    name = ecc.c.Name,
                    season = ecc.cl.SemesterSeason,
                    year = ecc.cl.SemesterYear,
                    grade = ecc.e.Grade
                }).ToList();

            return Json(student_classes);
        }

        /// <summary>
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
        {            
            return Json(null);
        }



        /// <summary>
        /// Adds a submission to the given assignment for the given student
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// The score of the submission should start as 0 until a Professor grades it
        /// If a Student submits to an assignment again, it should replace the submission contents
        /// and the submission time (the score should remain the same).
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// <returns>A JSON object containing {success = true/false}</returns>
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
     string category, string asgname, string uid, string contents)
        {
            try // Safty to protect againt errors: 
            {
                // 1. Find course
                var course = db.Courses
                    .FirstOrDefault(c => c.Subject == subject && c.Num == num);

                if (course == null) 
                    return Json(new { success = false }); // course not found

                // 2. Find class
                var cls = db.Classes
                    .FirstOrDefault(cl =>
                        cl.CourseId == course.CourseId &&
                        cl.SemesterSeason == season &&
                        cl.SemesterYear == year);

                if (cls == null)
                    return Json(new { success = false }); // class does not exist

                // 3. Find category
                var cat = db.AssignmentCategories
                    .FirstOrDefault(ac =>
                        ac.ClassId == cls.ClassId &&
                        ac.Category == category);

                if (cat == null)
                    return Json(new { success = false }); // catagory does not exist

                // 4. Find assignment
                var asg = db.Assignments
                    .FirstOrDefault(a =>
                        a.CategoryId == cat.CategoryId &&
                        a.Name == asgname);

                if (asg == null)
                    return Json(new { success = false }); // assignment does not exist

                // 5. Check for existing submission
                var sub = db.Submissions
                    .FirstOrDefault(s =>
                        s.AssignmentId == asg.AssignmentId &&
                        s.UId == uid);

                if (sub != null)
                {
                    // Update existing submission (keep score)
                    sub.Content = contents;
                    sub.Submitted = DateTime.Now;
                }
                else
                {
                    // Create new submission
                    db.Submissions.Add(new Submission
                    {
                        AssignmentId = asg.AssignmentId,
                        UId = uid,
                        Content = contents,
                        Submitted = DateTime.Now,
                        Score = 0 // start at 0 until graded
                    });
                }

                db.SaveChanges(); // must do last incase or and error.

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }


        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false}. 
        /// false if the student is already enrolled in the class, true otherwise.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {
            try // protect against errors such as invalid course or class information, or database errors
            {
                // Step 1: find the course
                var course = db.Courses
                .FirstOrDefault(c => c.Subject == subject && c.Num == num);

                if (course == null)
                    return Json(new { success = false });

                // Step 2: find the class
                var cls = db.Classes
                    .FirstOrDefault(cl =>
                        cl.CourseId == course.CourseId &&
                        cl.SemesterSeason == season &&
                        cl.SemesterYear == year);

                if (cls == null)
                    return Json(new { success = false });

                // Step 3: check if the student is already enrolled
                bool alreadyEnrolled = db.Enrolleds.Any(e =>
                    e.UId == uid && e.ClassId == cls.ClassId);

                if (alreadyEnrolled) // already enrolled
                    return Json(new { success = false });

                // Step 4: Enroll the student
                var enrollment = new Enrolled
                {
                    UId = uid,
                    ClassId = cls.ClassId,
                    // Note the Defaut for Grade is null
                };

                db.Enrolleds.Add(enrollment);
                db.SaveChanges();

                return Json(new { success = true });
            } catch
            {
                return Json(new { success = false });
            }
        }



        /// <summary>
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// Assume all classes are 4 credit hours.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// If a student is not enrolled in any classes, they have a GPA of 0.0.
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {

            // find all the grade for a given uID:
            var grades = db.Enrolleds
                .Where(e => e.UId == uid)
                .Select(e => e.Grade)
                .ToList();

            double total = 0;
            int count = 0;

            // go through the list of grades and convert each letter to a corrisponging value:
            foreach (var g in grades)
            {
                double pts = GradeToPoints(g);
                if (pts >= 0)
                {
                    total += pts;
                    count++;
                }
            }

            double gpa = count > 0 ? total / count : 0;
            return Json(new
            {
                success = count > 0,
                gpa = count > 0 ? Math.Round(gpa) : 0 // round for consistant decimal places
            });
        }

        /*******End code to modify********/

        /// <summary>
        /// Helper method for calculating the grade point value of a given letter grade
        /// </summary>
        /// <param name="grade"> letter grade </param>
        /// <returns> point value out of 4.0 scale: -1 if not grade given</returns>
        double GradeToPoints(string grade)
        {
            return grade switch
            {
                "A" => 4.0,
                "A-" => 3.7,
                "B+" => 3.3,
                "B" => 3.0,
                "B-" => 2.7,
                "C+" => 2.3,
                "C" => 2.0,
                "C-" => 1.7,
                "D+" => 1.3,
                "D" => 1.0,
                "D-" => 0.7,
                "F" => 0.0,
                _ => -1 // invalid / not graded
            };
        }
    }

    }

