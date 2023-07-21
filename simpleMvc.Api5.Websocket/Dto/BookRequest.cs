using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace simpleMvc.Api5.Websocket.Dto
{
    public class BookRequest
    {
        [JsonProperty("title")]
        public string BookTitle { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }
    }
}