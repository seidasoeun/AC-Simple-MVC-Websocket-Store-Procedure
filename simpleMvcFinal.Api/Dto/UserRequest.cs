using Newtonsoft.Json;

namespace simpleMvcFinal.Api.Dto
{
    public class UserRequest
    {
        [JsonProperty("access_token")]
        public string Token { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }
    }
}