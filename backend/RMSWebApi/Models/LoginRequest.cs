namespace RMSWebApi.Models
{
    public class LoginRequest
    {

        // holds what the user typed into the login form on the React frontend it's not in the db 
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
