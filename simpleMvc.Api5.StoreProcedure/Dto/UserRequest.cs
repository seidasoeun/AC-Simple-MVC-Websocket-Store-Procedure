using Newtonsoft.Json;

namespace simpleMvc.Api5.StoreProcedure.Dto
{
    public class UserRequest
    {
        [JsonProperty("access_token")]
        public string Token { get; set; }

    }
}