using System.Collections.Generic;

namespace simpleMvc.Api4.Dto
{
    public class UserRespone
    {
        public int userId { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string passcode { get; set; }
        public string refreshToken { get; set; }
        public ICollection<BookRespone> Books { get; set; }
        public ICollection<RoleRespone> Roles { get; set; }
    }
}