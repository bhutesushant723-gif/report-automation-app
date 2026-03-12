using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReportAutomationApp.Data;
using ReportAutomationApp.Models;
using System.Linq;
using System.Threading.Tasks;

public class ClientController : Controller
{
    private readonly ApplicationDbContext _db;

    public ClientController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Dashboard()
    {
        var clientIdString = HttpContext.Session.GetString("ClientId");

        if (string.IsNullOrEmpty(clientIdString))
            return RedirectToAction("Login", "Account");

        int clientId = int.Parse(clientIdString);

        var client = await _db.Clients.FindAsync(clientId);

        return View(client);
    }

    public async Task<IActionResult> Generate()
    {
        var clientIdString = HttpContext.Session.GetString("ClientId");

        if (string.IsNullOrEmpty(clientIdString))
            return RedirectToAction("Login", "Account");

        int clientId = int.Parse(clientIdString);

        var templates = await _db.GraphTemplates
                                 .Where(x => x.ClientId == clientId)
                                 .OrderBy(x => x.SlideOrder)
                                 .ToListAsync();

        foreach (var template in templates)
        {
            // TODO: Generate PPT graphs using template logic
        }

        return Content("Client Report Generated");
    }
}