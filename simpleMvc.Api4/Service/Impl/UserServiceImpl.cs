using simpleMvc.Api4.Dto;
using simpleMvc.Api4.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace simpleMvc.Api4.Service.Impl
{
    public class UserServiceImpl : UserService
    {
        private UserRepository _userRepository;
        public UserServiceImpl() {
            this._userRepository = new UserRepository();
        }
        public List<BookRespone> GetBookFromUserId(int userId)
        {
            return
               (from x in _userRepository._context.Books
                where x.userId == userId
                select new BookRespone
                {
                    bookTitle = x.bookTitle,
                    author = x.author,
                }
               )
               .ToList();
        }

        public List<RoleRespone> GetRoleFromUserId(int userId)
        {
            List<RoleRespone> roles = new List<RoleRespone>();
            foreach (var item in _userRepository._context.UserRoles)
            {
                if (item.userId == userId)
                {
                    roles.Add(new RoleRespone
                    {
                        roleName = _userRepository._context.Roles.FirstOrDefault(x => x.roleId == item.roleId).roleName
                    });
                }
            }
            return roles;
        }
    }
}