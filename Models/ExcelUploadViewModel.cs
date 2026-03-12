using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace ReportAutomationApp.Models
{
    public class ExcelUploadViewModel
    {
        public IFormFile? File { get; set; }

        public List<string> Columns { get; set; } = new List<string>();

        public string SelectedXColumn { get; set; }
        public List<string> SelectedYColumns { get; set; } = new List<string>();

        public string ChartType { get; set; }

        public List<string> GroupNames { get; set; } = new List<string>();
        public List<string> SelectedGroupNames { get; set; } = new List<string>();

        public int ClientId { get; set; }
    }
}