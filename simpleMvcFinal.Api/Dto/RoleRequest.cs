using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace simpleMvcFinal.Api.Dto
{
    public class RoleRequest
    {
        [JsonProperty("role_name")]
        public string RoleName { get; set; }

        [JsonProperty("action")] 
        public string Action { get; set; }

        [JsonProperty("access_token")]
        public string Token { get; set; }
    }
}