using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace simpleMvcFinal.Api.Models
{
    public class ProcedureModel
    {
        [JsonProperty("book_id")]
        public int BookId { get; set; }

        [JsonProperty("role_id")] 
        public int RoleId { get; set; }

        [JsonProperty("book_title")]
        public string BookTitle { get; set; }

        [JsonProperty("book_author")]
        public string Author { get; set; }

        [JsonProperty("role_name")]
        public string RoleName { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }
    }
}