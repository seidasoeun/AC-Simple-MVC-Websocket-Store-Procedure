using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace simpleMvc.Api5.StoreProcedure.Dto
{
    public class BookRequest
    {
        public string Token { get; set; }
        public string BookTitle { get; set; }
        public string Author { get; set; }
    }
}