using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using simpleMvc.Api5.Websocket.Dto;
using simpleMvc.Api5.Websocket.Models;

namespace simpleMvc.Api5.Websocket.Repository
{
    public class BookRepository
    {
        private readonly DatabaseSimpleMvcEntities _context = new DatabaseSimpleMvcEntities();
        public List<BookResponse> GetAllBook()
        {
            List<BookResponse> books = new List<BookResponse>();
            foreach (var book in _context.Books)
            {
                books.Add(new BookResponse
                {
                    BookTitle = book.bookTitle,
                    Author = book.author
                });
            }
            return books;
        }

        public BookResponse GetBook(int id)
        {
            var isExistBook = _context.Books.FirstOrDefault(x => x.bookId == id);
            if (isExistBook == null) return null;

            return new BookResponse
            {
                Author = isExistBook.author,
                BookTitle = isExistBook.bookTitle,
            };
        }

        public BookResponse AddBook(BookRequest book, string username)
        {
            var isExistUser = _context.Users.FirstOrDefault(x => x.username == username);
            if (isExistUser == null) return null;

            Book bookModel = new Book();
            if (_context.Books.Any())
            {
                int lastId = _context.Books.OrderByDescending(x => x.bookId).FirstOrDefault().bookId;
                bookModel.bookId = lastId + 1;
            }
            else bookModel.bookId = 1;

            bookModel.bookTitle = book.BookTitle;
            bookModel.author = book.Author;
            bookModel.userId = isExistUser.userId;
            _context.Books.Add(bookModel);
            _context.SaveChanges();
            return new BookResponse
            {
                Author = bookModel.author,
                BookTitle = bookModel.bookTitle,
            };
        }

        public BookResponse UpdateBook(BookRequest book, string username, int id)
        {
            if (!IsOwnerBookWithUsername(username, id)) return null;
            var bookModel = _context.Books.FirstOrDefault(x => x.bookId == id);
            if (bookModel != null)
            {
                bookModel.author = book.Author;
                bookModel.bookTitle = book.BookTitle;
            }

            return new BookResponse
            {
                BookTitle = bookModel.bookTitle,
                Author = bookModel.author,
            };
        }

        public BookResponse DeleteBook(string username, int id)
        {
            if (!IsOwnerBookWithUsername(username, id)) return null;
            var bookModel = _context.Books.FirstOrDefault(x => x.bookId == id);
            if (bookModel != null)
            {
                _context.Books.Remove(bookModel);
                _context.SaveChanges();
            }
            return new BookResponse
            {
                BookTitle = bookModel.bookTitle,
                Author = bookModel.author,
            };

        }

        public List<BookResponse> GetBookWithUsername(string username)
        {
            var isExistUser = _context.Users.FirstOrDefault(x => x.username == username);
            if (isExistUser == null) return null;

            List<BookResponse> books = new List<BookResponse>();

            if (!_context.Books.Any(x => x.userId == isExistUser.userId)) return null;
            foreach (var book in _context.Books)
            {
                if (book.userId == isExistUser.userId)
                {
                    books.Add(new BookResponse
                    {
                        Author = book.author,
                        BookTitle = book.bookTitle,
                    });
                }
                
            }
            
            return books;
        }

        public bool IsOwnerBookWithUsername(string username, int id)
        {
            if (username == null) return false;
            var isExistUser = _context.Users.FirstOrDefault(x => x.username == username);
            if (isExistUser == null) return false;
            var isExistBook = _context.Books.FirstOrDefault(x => x.bookId == id);
            if (isExistBook == null) return false;
            if (isExistBook.userId != isExistUser.userId) return false;
            return true;
        }
    }
}