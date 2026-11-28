namespace RMSWebApi.Models
{
    public class StaffRequest
    {
        public string FullName { get; set; } = "";
        public string? Phone { get; set; }
        public string? Position { get; set; }
        public bool IsManager { get; set; }
        public int BranchId { get; set; }
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
