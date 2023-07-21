using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace simpleMvc.Api5.StoreProcedure.Dto
{
    public class UserResponse
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Passcode { get; set; }
        public string RefreshToken { get; set; }

        public List<RoleResponse> Roles { get; set; }   
        public List<BookResponse> Books { get; set; }   
    }
}