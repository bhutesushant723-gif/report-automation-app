namespace ReportAutomationApp.Models
{
    public class GraphTemplateColumn
    {
        public int Id { get; set; }

        public int GraphTemplateId { get; set; }

        public string ColumnName { get; set; }

        public GraphTemplate GraphTemplate { get; set; }
    }
}