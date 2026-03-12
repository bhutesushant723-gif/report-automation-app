namespace ReportAutomationApp.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }
        // Admin / User / Client

        public bool IsActive { get; set; } = true;

        // Optional: Only for Client users
        public int? ClientId { get; set; }

        public Client Client { get; set; }
    }
}