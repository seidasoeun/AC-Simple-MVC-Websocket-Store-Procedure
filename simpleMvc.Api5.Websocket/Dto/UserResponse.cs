using System.Collections.Generic;
using Newtonsoft.Json;

namespace simpleMvc.Api5.Websocket.Dto
{
    public class UserResponse
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Passcode { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("roles")]

        public List<RoleResponse> RoleResponse { get; set; }

        [JsonProperty("books")]
        public List<BookResponse> BookResponse { get; set; }
    }
}