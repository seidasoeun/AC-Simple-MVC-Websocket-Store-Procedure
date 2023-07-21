using System.Collections.Generic;

namespace simpleMvc.Api4.Dto
{
    public class RegisterRequest
    {
        public string username { get; set; }
        public string email { get; set; }
        public string passcode { get; set; }

        public List<RoleRespone> Roles{ get; set; }
    }
}