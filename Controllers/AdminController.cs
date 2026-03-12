using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReportAutomationApp.Data;
using ReportAutomationApp.Models;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _db;

    public AdminController(ApplicationDbContext db)
    {
        _db = db;
    }

    // Dashboard
    public IActionResult Dashboard()
    {
        var clients = _db.Clients.ToList();
        return View(clients);
    }

    public IActionResult Users()
    {
        var users = _db.Users.ToList();
        return View(users);
    }

    // View graphs of a client
    public IActionResult ClientGraphs(int id)
    {
        var graphs = _db.GraphTemplates
                        .Where(x => x.ClientId == id)
                        .ToList();

        ViewBag.ClientId = id;
        return View(graphs);
    }

    // Create Graph
    public IActionResult CreateGraph(int clientId)
    {
        ViewBag.ClientId = clientId;

        var columns = _db.ExcelColumns
                         .Where(x => x.ClientId == clientId)
                         .Select(x => x.ColumnName)
                         .ToList();

        ViewBag.Columns = columns;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateGraph(GraphTemplate model, List<string> YColumns)
    {
        _db.GraphTemplates.Add(model);
        await _db.SaveChangesAsync();

        if (YColumns != null)
        {
            foreach (var col in YColumns)
            {
                _db.GraphTemplateColumns.Add(new GraphTemplateColumn
                {
                    GraphTemplateId = model.GraphTemplateId,
                    ColumnName = col
                });
            }

            await _db.SaveChangesAsync();
        }

        return RedirectToAction("ClientGraphs",
            new { id = model.ClientId });
    }

    // Edit Graph
    public IActionResult EditGraph(int id)
    {
        var graph = _db.GraphTemplates.Find(id);

        if (graph == null)
        {
            return NotFound();
        }

        var columns = _db.ExcelColumns
            .Where(x => x.ClientId == graph.ClientId)
            .Select(x => x.ColumnName)
            .ToList();

        ViewBag.Columns = columns;

        return View(graph);
    }

    [HttpPost]
    public async Task<IActionResult> EditGraph(GraphTemplate model)
    {
        _db.GraphTemplates.Update(model);
        await _db.SaveChangesAsync();

        return RedirectToAction("ClientGraphs",
            new { id = model.ClientId });
    }

    // Delete Graph
    public async Task<IActionResult> DeleteGraph(int id)
    {
        var graph = await _db.GraphTemplates.FindAsync(id);

        if (graph == null)
            return NotFound();

        _db.GraphTemplates.Remove(graph);
        await _db.SaveChangesAsync();

        return RedirectToAction("ClientGraphs",
            new { id = graph.ClientId });
    }


    public IActionResult AddUser()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddUser(User model)
    {
        if (ModelState.IsValid)
        {
            _db.Users.Add(model);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction("Dashboard");
    }


    public IActionResult AssignClient(int userId)
    {
        ViewBag.UserId = userId;

        ViewBag.Clients = _db.Clients.ToList();

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AssignClient(int userId, List<int> ClientIds)
    {
        // Remove old assignments
        var oldAccess = _db.UserClientAccess
            .Where(x => x.UserId == userId);

        _db.UserClientAccess.RemoveRange(oldAccess);

        // If admin selected clients
        if (ClientIds != null && ClientIds.Count > 0)
        {
            foreach (var clientId in ClientIds)
            {
                _db.UserClientAccess.Add(new UserClientAccess
                {
                    UserId = userId,
                    ClientId = clientId
                });
            }
        }

        await _db.SaveChangesAsync();

        return RedirectToAction("Users");
    }

    // GET
    public IActionResult UploadExcel(int clientId)
    {
        var model = new ExcelUploadViewModel
        {
            ClientId = clientId
        };

        return View(model);
    }

    public IActionResult AddClient()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddClient(Client model)
    {
        if (ModelState.IsValid)
        {
            _db.Clients.Add(model);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction("Dashboard");
    }


    // POST
    [HttpPost]
    public async Task<IActionResult> UploadExcel(ExcelUploadViewModel model)
    {
        if (model.File != null && model.File.Length > 0)
        {
            using (var stream = new MemoryStream())
            {
                await model.File.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];

                    var columns = new List<string>();

                    // Read first row (headers)
                    for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                    {
                        var header = worksheet.Cells[1, col].Text;
                        columns.Add(header);
                    }

                    // Remove old columns
                    var oldColumns = _db.ExcelColumns
                        .Where(x => x.ClientId == model.ClientId);

                    _db.ExcelColumns.RemoveRange(oldColumns);

                    // Add new columns
                    foreach (var col in columns)
                    {
                        _db.ExcelColumns.Add(new ReportAutomationApp.Models.ExcelColumn
                        {
                            ColumnName = col,
                            ClientId = model.ClientId
                        });
                    }

                    await _db.SaveChangesAsync();

                    HttpContext.Session.SetString("ExcelUploaded", "true");
                }
            }
        }

        return RedirectToAction("ClientGraphs", new { id = model.ClientId });
    }
}