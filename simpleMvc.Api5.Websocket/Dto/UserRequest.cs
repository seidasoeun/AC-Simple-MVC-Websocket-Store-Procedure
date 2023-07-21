using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace simpleMvc.Api5.Websocket.Dto
{
    public class UserRequest
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }
    }
}