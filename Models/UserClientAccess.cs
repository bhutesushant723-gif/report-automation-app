namespace ReportAutomationApp.Models
{
    public class UserClientAccess
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ClientId { get; set; }

        public User User { get; set; }

        public Client Client { get; set; }
    }
}