namespace consensoria_autos.Models
{
    public class User
    {
        public int id { get; set; }
        public string username { get; set; } = "";
        public string full_name { get; set; } = "";
        public string email { get; set; } = "";
        public string? password { get; set; } = "";
        public bool? active { get; set; } = false;
        public DateTime creacte_at { get; set; }
        public UserRole? user_roles { get; set; }

    }
}
