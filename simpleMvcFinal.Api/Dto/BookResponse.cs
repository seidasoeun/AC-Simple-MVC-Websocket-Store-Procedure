using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace simpleMvcFinal.Api.Dto
{
    public class BookResponse
    {
        [JsonProperty("book_title")]
        public string BookTitle { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }
    }
}