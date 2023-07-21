namespace simpleMvc4.Dto
{
    public class UserRequest
    {
        public int userId { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string passcode { get; set; }
        public string refreshToken { get; set; }
    }
}