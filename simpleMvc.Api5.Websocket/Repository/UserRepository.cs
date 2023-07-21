using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using simpleMvc.Api5.Websocket.Dto;
using simpleMvc.Api5.Websocket.Models;

namespace simpleMvc.Api5.Websocket.Repository
{
    public class UserRepository
    {
        private readonly DatabaseSimpleMvcEntities _context = new DatabaseSimpleMvcEntities();

        public UserResponse FindUserByUsernameAndPassword(string username, string password)
        {
            if (username == null || password == null) return null;
            User user =  _context.Users.FirstOrDefault(x => x.username == username && x.passcode == password);
            if (user == null) return null;

            return new UserResponse
            {
                Email = user.email,
                Username = user.username,
                Passcode = user.passcode,
                RefreshToken = user.refreshToken,
                RoleResponse = GetUserRoleWithId(user.userId),
                BookResponse = GetBookWithId(user.userId)
            };
        }

        public UserResponse RegisterNewUser(RegisterRequest req, string refreshToken)
        {
            if (req == null) return null;
            User user = new User();
            int lastId = 1;
            if (_context.Users.Any())
            {
                lastId = _context.Users.OrderByDescending(x => x.userId).FirstOrDefault().userId + 1;

            }

            user.userId = lastId;
            user.email = req.Email;
            user.username = req.Username;
            user.passcode = req.Passcode;
            user.refreshToken = refreshToken;

            _context.Users.Add(user);

            if (req.RoleResponse != null)
            {
                foreach (var role in req.RoleResponse)
                {
                    int roleId = _context.Roles.FirstOrDefault(x => x.roleName == role.RoleName).roleId;
                    _context.UserRoles.Add(new UserRole
                    {
                        userId = lastId,
                        roleId = roleId
                    });
                }
            }

            _context.SaveChanges();

            return new UserResponse
            {
                Email = req.Email,
                Username = req.Username,
                Passcode = req.Passcode,
                RefreshToken = refreshToken,
                RoleResponse = req.RoleResponse,
            };
        }

        public UserResponse GetUser(string username)
        {
            if (username == null) return null;
            User user = _context.Users.FirstOrDefault(x => x.username == username);
            if (user == null) return null;

            return new UserResponse
            {
                Email = user.email,
                Username = user.username,
                Passcode = user.passcode,
                RefreshToken = user.refreshToken,
                RoleResponse = GetUserRoleWithId(user.userId),
                BookResponse = GetBookWithId(user.userId)
            };
        }

        public UserResponse GetUserWithRefreshToken(string refreshToken)
        {
            
            User user = _context.Users.FirstOrDefault(x => x.refreshToken.ToString() == refreshToken);
            if (user == null) return null;
            return new UserResponse
            {
                RefreshToken = refreshToken,
                Email = user.email,
                Username = user.username,
                Passcode = user.passcode,
                RoleResponse = GetUserRoleWithId(user.userId),
                BookResponse = GetBookWithId(user.userId)
            };
        }

        public bool StoreRefreshToken(string refreshToken, string username)
        {
            var isExistUser = _context.Users.FirstOrDefault(x => x.username == username);
            if (isExistUser != null)
            {
                isExistUser.refreshToken = refreshToken;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public List<RoleResponse> GetUserRoleWithId(int id)
        {
            List<RoleResponse> roles = new List<RoleResponse>();

            if (!_context.UserRoles.Any(x => x.userId == id)) return null;
            
            foreach (var userRoles in _context.UserRoles)
                if (userRoles.userId == id)
                    roles.Add(new RoleResponse
                    {
                        RoleName = _context.Roles.FirstOrDefault(x => x.roleId == userRoles.roleId)?.roleName,
                    });
            return roles;
        }

        public List<BookResponse> GetBookWithId(int id)
        {
            List<BookResponse> books = new List<BookResponse>();

            if (!_context.Books.Any(x => x.userId == id)) return null;

            foreach (var book in _context.Books)
                if (book.userId == id)
                    books.Add(new BookResponse { Author = book.author, BookTitle = book.bookTitle });

            return books;
        }
    }
}