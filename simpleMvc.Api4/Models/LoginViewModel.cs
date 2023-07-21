using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace simpleMvc.Api4.Models
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Passcode { get; set; }
    }
}