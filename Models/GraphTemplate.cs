namespace ReportAutomationApp.Models
{
    public class GraphTemplate
    {
        public int GraphTemplateId { get; set; }

        public string GraphName { get; set; }

        public string XColumn { get; set; }

        public string ChartType { get; set; }

        public int SlideOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public int ClientId { get; set; }

        public Client Client { get; set; }

        // ADD THIS
        public ICollection<GraphTemplateColumn> Columns { get; set; }
    }
}