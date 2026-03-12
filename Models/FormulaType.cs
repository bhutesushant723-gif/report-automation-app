namespace ReportAutomationApp.Models
{
    public class FormulaType
    {
        public int FormulaTypeId { get; set; }

        public string FormulaCode { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}