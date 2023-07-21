using simpleMvc.Api4.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simpleMvc.Api4.Service
{
    internal interface UserService
    {
        List<RoleRespone> GetRoleFromUserId(int userId);
        List<BookRespone> GetBookFromUserId(int userId);
    }
}
