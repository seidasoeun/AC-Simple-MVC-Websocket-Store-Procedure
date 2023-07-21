using simpleMvc4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace simpleMvc4.Dto
{
    public class UserRespone
    {
        public string username { get; set; }
        public string email { get; set; }
        public string passcode { get; set; }
        public virtual ICollection<RoleRespone> Roles { get; set; }
        public virtual ICollection<BookRespone> Books { get; set; }
    }
}