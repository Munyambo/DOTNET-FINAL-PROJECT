namespace Restaurant_Management_System.Models
{
    public class UsersR
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "";
    }
}

