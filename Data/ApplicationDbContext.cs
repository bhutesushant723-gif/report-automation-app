using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using ReportAutomationApp.Models; // change namespace

namespace ReportAutomationApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<GraphTemplate> GraphTemplates { get; set; }
        public DbSet<ExcelColumn> ExcelColumns { get; set; }
        public DbSet<GraphTemplateColumn> GraphTemplateColumns { get; set; }
        public DbSet<UserClientAccess> UserClientAccess { get; set; }
    }
}