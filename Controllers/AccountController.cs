using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using ReportAutomationApp.Data;
using ReportAutomationApp.Models;
using System.Linq;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _db;

    public AccountController(ApplicationDbContext db)
    {
        _db = db;
    }

    // Login Page
    public IActionResult Login()
    {
        return View();
    }

    // Login POST
    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        var user = _db.Users
            .FirstOrDefault(x => x.Email == email
                              && x.Password == password
                              && x.IsActive);

        if (user != null)
        {
            // Save session
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Role", user.Role);

            if (user.ClientId.HasValue)
                HttpContext.Session.SetInt32("ClientId", user.ClientId.Value);

            // Role-based redirect
            if (user.Role == "Admin")
                return RedirectToAction("Dashboard", "Admin");

            if (user.Role == "User")
                return RedirectToAction("Dashboard", "User");

            if (user.Role == "Client")
                return RedirectToAction("Dashboard", "Client");
        }

        ViewBag.Error = "Invalid credentials";
        return View();
    }

    // Logout
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}