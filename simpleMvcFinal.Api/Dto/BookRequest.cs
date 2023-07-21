using Newtonsoft.Json;

namespace simpleMvcFinal.Api.Dto
{
    public class BookRequest
    {
        [JsonProperty("title")]
        public string BookTitle { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("access_token")]
        public string Token { get; set; }
    }
}