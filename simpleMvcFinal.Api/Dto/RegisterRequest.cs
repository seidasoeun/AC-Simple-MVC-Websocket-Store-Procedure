using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace simpleMvcFinal.Api.Dto
{
    public class RegisterRequest
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Passcode { get; set; }

        [JsonProperty("roles")]
        public List<RoleResponse> RoleResponse { get; set; }
    }
}