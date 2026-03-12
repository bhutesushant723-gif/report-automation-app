using ReportAutomationApp.Models;

    namespace ReportAutomationApp.Models
    {
        public class ExcelColumn
        {
            public int ExcelColumnId { get; set; }

            public string ColumnName { get; set; } = string.Empty;

            public int ClientId { get; set; }

            public Client? Client { get; set; }
        }
    }
