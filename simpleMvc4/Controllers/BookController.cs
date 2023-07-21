using Microsoft.IdentityModel.Tokens;
using simpleMvc4.Models;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace simpleMvc4.Controllers
{
    public class BookController : Controller
    {
        DatabaseSimpleMvcEntities _context = new DatabaseSimpleMvcEntities();
        [HttpGet]
        public ActionResult Index()
        {
            return View(_context.Books.ToList());
        }

        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            if (Request.Cookies["refresh-token"].Value == null)
                return RedirectToAction("Login", "User");
            
                
            return View();
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var data = _context.Books.FirstOrDefault(x => x.bookId == id);
            return View(data);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Edit(int id)
        {
            if (Request.Cookies["refresh-token"].Value == null)
                return RedirectToAction("Login", "User");
            var b = _context.Books.FirstOrDefault(x => x.bookId == id);
            if (b == null)
                return RedirectToAction("Index", "Book");
            if (getUserFromToken().userId != b.userId)
                return RedirectToAction("Index", "Book", new { error = "Unauthorized!" });
            return View(_context.Books.FirstOrDefault(x => x.bookId == id));
        }

        [HttpGet]
        [Authorize]
        public ActionResult Delete(int id)
        {
            if (Request.Cookies["refresh-token"].Value == null)
                return RedirectToAction("Login", "User");
            var book = _context.Books.FirstOrDefault(x => x.bookId == id);
            if(book == null)
                return RedirectToAction("Index", "Book", new {error = "Not found!"});
            if (book.userId != getUserFromToken().userId)
                return RedirectToAction("Index", "Book", new { error = "Unauthorized!" });

            _context.Books.Remove(book);
            _context.SaveChanges();
            return RedirectToAction("Index", "Book");

        }

        [HttpPost]
        public ActionResult Create(Book book)
        {
            if (Request.Cookies["refresh-token"].Value == null)
                return RedirectToAction("Login", "User");
            book.userId = getUserFromToken().userId;
            if (_context.Books.Any())
            {
                var lastId = _context.Books.OrderByDescending(x => x.bookId).FirstOrDefault().bookId;
                book.bookId = lastId + 1;
            }
            _context.Books.Add(book);
            _context.SaveChanges();
            return RedirectToAction("Index", "Book");
        }

        [HttpPost]
        public ActionResult Edit(int id, Book book)
        {
            if (Request.Cookies["refresh-token"].Value == null)
                return RedirectToAction("Login", "User");
            var b = _context.Books.FirstOrDefault(x => x.bookId == id);

            if (b != null && getUserFromToken().userId == b.userId)
            {
                b.bookTitle = book.bookTitle;
                b.author = book.author;
                _context.SaveChanges();
                return RedirectToAction("Index", "Book");
            }
            return RedirectToAction("Index", "Book", new {error = "Unauthorized!" });
        }
        private User getUserFromToken()
        {
            var token = getAccessToken();
            var secret = ConfigurationManager.AppSettings["JwtSecret"];
            var issuer = ConfigurationManager.AppSettings["JwtIssuer"];
            var audience = ConfigurationManager.AppSettings["JwtAudience"];
            
            var handler = new JwtSecurityTokenHandler();
            var validations = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            try
            {
                var claims = handler.ValidateToken(token, validations, out var tokenSecure);
                var user = _context.Users.Where(x => x.username == claims.Identity.Name).FirstOrDefault();
                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private string getAccessToken()
        {
            return Request.Cookies["access-token"].Value;
        }

    }
}