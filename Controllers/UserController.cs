using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using ReportAutomationApp.Data;
using ReportAutomationApp.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

public class UserController : Controller
{
    private readonly ApplicationDbContext _db;

    public UserController(ApplicationDbContext db)
    {
        _db = db;
    }

    // This prevents 404 when user opens /User or /User/Index
    public IActionResult Index()
    {
        return RedirectToAction("Dashboard");
    }

    // User Dashboard
    public async Task<IActionResult> Dashboard()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
            return RedirectToAction("Login", "Account");

        var clients = await _db.UserClientAccess
            .Where(x => x.UserId == userId)
            .Include(x => x.Client)
            .Select(x => x.Client)
            .ToListAsync();

        return View(clients);
    }

    // Generate Report
    public async Task<IActionResult> GenerateReport(int clientId)
    {
        var templates = await _db.GraphTemplates
                                 .Where(x => x.ClientId == clientId)
                                 .OrderBy(x => x.SlideOrder)
                                 .ToListAsync();

        // Check if Excel was uploaded
        var columnsExist = await _db.ExcelColumns.AnyAsync(x => x.ClientId == clientId);
        
        if (!columnsExist)
            return Content("No Excel uploaded. Please upload Excel first.");

        foreach (var template in templates)
        {
            // Future: PowerPoint generation logic will run here
        }

        return Content("Report Generated Successfully");
    }
}