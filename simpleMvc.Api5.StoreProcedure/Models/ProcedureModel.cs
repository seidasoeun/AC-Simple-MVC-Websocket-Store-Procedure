using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreProcedure.Api1.Models
{
    public class ProcedureModel
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string Author { get; set; }
        public string RoleName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
        public string Action { get; set; }
    }
}