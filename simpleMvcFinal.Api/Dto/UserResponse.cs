using System.Collections.Generic;
using Newtonsoft.Json;

namespace simpleMvcFinal.Api.Dto
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

        public List<RoleResponse> Roles { get; set; }

        [JsonProperty("books")]
        public List<BookResponse> Books { get; set; }
    }
}