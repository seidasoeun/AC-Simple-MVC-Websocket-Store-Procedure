using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace simpleMvc.Api4.Dto
{
    public class TokenRequest
    {
        public string Email { get; set; }
        public string Passcode { get; set; }
    }
}