namespace consensoria_autos.Models
{
    public class AuthResponse
    {
        public string Message { get; set; } = "";
        public string Token { get; set; } = "";
        public AuthUser User { get; set; } = new();
    }

    public class AuthUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Role { get; set; } = "";
    }

}
