using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace simpleMvc.Api5.Websocket.Dto
{
    public class RoleResponse
    {
        [JsonProperty("role_name")]
        public string RoleName { get; set; }
    }
}