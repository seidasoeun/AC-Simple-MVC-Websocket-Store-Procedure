using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace simpleMvc.Api4.Dto
{
    public class BookRequest
    {
        public string bookTitle { get; set; }
        public string author { get; set; }
        public string token { get; set; }
    }
}