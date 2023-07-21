using Microsoft.IdentityModel.Tokens;
using simpleMvc.Api4.Dto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace simpleMvc.Api4.Controllers
{
    public class BookController : ApiController
    {
        DatabaseSimpleMvcEntities _context = new DatabaseSimpleMvcEntities();

        [HttpGet]
        public IEnumerable<BookRespone> Index()
        {
            List<BookRespone> books = new List<BookRespone>();
            foreach (var book in _context.Books)
            {
                books.Add(new BookRespone
                {
                    author = book.author,
                    bookTitle = book.bookTitle
                });
            }
            return books;
        }

        [HttpPost]
        public HttpResponseMessage Store(BookRequest req)
        {
            Book book = new Book();
            if (req == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Book Object can not be null!");

            string username = GetUsernameFromToken(req.token);
            if (username == null)
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Unauthorized!");
            book.userId = _context.Users.FirstOrDefault(x => x.username == username).userId;
            if (_context.Books.Any())
            {
                int lastId = _context.Books.OrderByDescending(x => x.bookId).FirstOrDefault().bookId;
                book.bookId = lastId + 1;
            }else
                book.bookId = 1;
            
            book.author = req.author;
            book.bookTitle = req.bookTitle;
            
            _context.Books.Add(book);
            _context.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Created, new BookRespone { 
                author = req.author, bookTitle = req.bookTitle});
        }

        [HttpPut]
        public HttpResponseMessage Update(int id, [FromBody] BookRequest req)
        {
            if (req == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Book Object can not be null!");

            var books = _context.Books.FirstOrDefault(x => x.bookId == id);
            if (books == null)
                return Request.CreateResponse(HttpStatusCode.NotFound, "Book with id = " + id + " Not found!");

            string username = GetUsernameFromToken(req.token);
            int userId = _context.Users.FirstOrDefault(x => x.username == username).userId;
            if (books.userId != userId)
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Unauthorized!");
            books.author = req.author;
            books.bookTitle = req.bookTitle;
            _context.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, new BookRespone { author = req.author, bookTitle = req.bookTitle});
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id, UserRequest req)
        {
            string username = GetUsernameFromToken(req.accessToken);
            if (username == null)
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Unauthorized!");

            var user = _context.Users.FirstOrDefault(x => x.username == username);
            if (user == null)
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Unauthorized!");

            var book = _context.Books.FirstOrDefault(x => x.bookId == id);
            if (book == null)
                return Request.CreateResponse(HttpStatusCode.NotFound, "Book with id = " + id + " Not found!");
            if(book.userId != user.userId)
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Unauthorized!");
            _context.Books.Remove(book);
            _context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, "Book Deleted!");
        }

        private string GetUsernameFromToken(string token)
        {
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

            };
            try{
                var claims = handler.ValidateToken(token, validations, out var tokenSecure);
                return claims.Identity.Name;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}