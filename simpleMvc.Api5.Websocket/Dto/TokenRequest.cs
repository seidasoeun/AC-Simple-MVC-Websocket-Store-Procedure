using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace simpleMvc.Api5.Websocket.Dto
{
    public class TokenRequest
    {
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}