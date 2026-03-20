using LibraryWebServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo( "TestProject1" )]
namespace LibraryWebServer.Controllers
{
    public class HomeController : Controller
    {

        // WARNING:
        // This very simple web server is designed to be as tiny and simple as possible
        // This is NOT the way to save user data.
        // This will only allow one user of the web server at a time (aside from major security concerns).
        private static string user = "";
        private static int card = -1;

        private readonly ILogger<HomeController> _logger;

        // private variable for accessing the DB
        private readonly Team3LibraryContext _DBcontext;


        /// <summary>
        /// Given a Patron name and CardNum, verify that they exist and match in the database.
        /// If the login is successful, sets the global variables "user" and "card"
        /// </summary>
        /// <param name="name">The Patron's name</param>
        /// <param name="cardnum">The Patron's card number</param>
        /// <returns>A JSON object with a single field: "success" with a boolean value:
        /// true if the login is accepted, false otherwise.
        /// </returns>
        [HttpPost]
        public IActionResult CheckLogin( string name, int cardnum )
        {
            bool loginSuccessful = _DBcontext.Patrons
                .Any(p => p.Name == name && p.CardNum == cardnum);

            if ( !loginSuccessful )
            {
                return Json( new { success = false } );
            }
            else
            {
                user = name;
                card = cardnum;
                return Json( new { success = true } );
            }
        }


        /// <summary>
        /// Logs a user out. This is implemented for you.
        /// </summary>
        /// <returns>Success</returns>
        [HttpPost]
        public ActionResult LogOut()
        {
            user = "";
            card = -1;
            return Json( new { success = true } );
        }

        /// <summary>
        /// Returns a JSON array representing all known books.
        /// Each book should contain the following fields:
        /// {"isbn" (string), "title" (string), "author" (string), "serial" (uint?), "name" (string)}
        /// Every object in the list should have isbn, title, and author.
        /// Books that are not in the Library's inventory (such as Dune) should have a null serial.
        /// The "name" field is the name of the Patron who currently has the book checked out (if any)
        /// Books that are not checked out should have an empty string "" for name.
        /// </summary>
        /// <returns>The JSON representation of the books</returns>
        [HttpPost]
        public ActionResult AllTitles()
        {

            // TODO: Implement
            // want the format ISBN, Title, Author, Serial, Name:
            var results = _DBcontext.Inventory
                .GroupJoin(_DBcontext.Titles,
                    i => i.Isbn,
                    t => t.Isbn,
                    (i, t) => new { i, t })
                .SelectMany(it => it.t.DefaultIfEmpty(),
                    (it, t) => new { it.i, t })
                .GroupJoin(_DBcontext.CheckedOut,
                    it => it.i.Serial,
                    c => c.Serial,
                    (it, c) => new { it, c })
                .SelectMany(itc => itc.c.DefaultIfEmpty(),
                    (itc, c) => new { itc.it.i, itc.it.t, c })
                .GroupJoin(_DBcontext.Patrons,
                    itc => itc.c.CardNum,
                    p => p.CardNum,
                    (itc, p) => new { itc, p })
                .SelectMany(itcp => itcp.p.DefaultIfEmpty(),
                // Jason is expecting a Null name to be a blank string.
                    (itcp, p) => new { itcp.itc.i.Isbn, itcp.itc.t.Title, itcp.itc.t.Author, 
                     itcp.itc.i.Serial, Name = p == null ? "" : p.Name })
                .ToList();

            return Json(results);

        }

        /// <summary>
        /// Returns a JSON array representing all books checked out by the logged in user 
        /// The logged in user is tracked by the global variable "card".
        /// Every object in the array should contain the following fields:
        /// {"title" (string), "author" (string), "serial" (uint) (note this is not a nullable uint) }
        /// Every object in the list should have a valid (non-null) value for each field.
        /// </summary>
        /// <returns>The JSON representation of the books</returns>
        [HttpPost]
        public ActionResult ListMyBooks()
        {
            // Need the Order "title": ..., "author": ..., "serial":...
            // Joind Patrons => CheckedOut => Inventory => Titles
            var results = _DBcontext.Patrons
                .Join(_DBcontext.CheckedOut,
                    p => p.CardNum,
                    c => c.CardNum,
                    (p, c) => new { p, c })
                .Join(_DBcontext.Inventory,
                    pc => pc.c.Serial,
                    i => i.Serial,
                    (pc, i) => new { pc, i })
                .Join(_DBcontext.Titles,
                    pci => pci.i.Isbn,
                    t => t.Isbn,
                    // create a final tables (note with cardnum so we can match the card num)
                    (pci, t) => new { t.Title, t.Author, pci.i.Serial, pci.pc.p.CardNum })
                .Where(x => x.CardNum == card)
                .Select(x => new { x.Title, x.Author, x.Serial }) // only select the things we need to display
                .ToList();

            return Json(results);
        }


        /// <summary>
        /// Updates the database to represent that
        /// the given book is checked out by the logged in user (global variable "card").
        /// In other words, insert a row into the CheckedOut table.
        /// You can assume that the book is not currently checked out by anyone.
        /// </summary>
        /// <param name="serial">The serial number of the book to check out</param>
        /// <returns>success</returns>
        [HttpPost]
        public ActionResult CheckOutBook( int serial )
        {
            // You may have to cast serial to a (uint)

            return Json( new { success = true } );
        }

        /// <summary>
        /// Returns a book currently checked out by the logged in user (global variable "card").
        /// In other words, removes a row from the CheckedOut table.
        /// You can assume the book is checked out by the user.
        /// </summary>
        /// <param name="serial">The serial number of the book to return</param>
        /// <returns>Success</returns>
        [HttpPost]
        public ActionResult ReturnBook( int serial )
        {
            // You may have to cast serial to a (uint)

            return Json( new { success = true } );
        }


        /*******************************************/
        /****** Do not modify below this line ******/
        /*******************************************/


        public IActionResult Index()
        {
            if ( user == "" && card == -1 )
                return View( "Login" );

            return View();
        }


        /// <summary>
        /// Return the Login page.
        /// </summary>
        /// <returns></returns>
        public IActionResult Login()
        {
            user = "";
            card = -1;

            ViewData["Message"] = "Please login.";

            return View();
        }

        /// <summary>
        /// Return the MyBooks page.
        /// </summary>
        /// <returns></returns>
        public IActionResult MyBooks()
        {
            if ( user == "" && card == -1 )
                return View( "Login" );

            return View();
        }

        // Added Team3LibraryContext context parameter so ASP.NET can inject the database
        // and assigned it to _DBcontext so the controller can query the database
        public HomeController(ILogger<HomeController> logger, Team3LibraryContext context)
        {
            _logger = logger;
            _DBcontext = context;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache( Duration = 0, Location = ResponseCacheLocation.None, NoStore = true )]
        public IActionResult Error()
        {
            return View( new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier } );
        }
    }
}