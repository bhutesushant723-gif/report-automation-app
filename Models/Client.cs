namespace ReportAutomationApp.Models
{
    public class Client
    {
        public int ClientId { get; set; }

        public string ClientName { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public ICollection<User>? Users { get; set; }
        public ICollection<GraphTemplate>? GraphTemplates { get; set; }
    }
}