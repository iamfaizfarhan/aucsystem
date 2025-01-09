using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Aucsystem.Models;
using Aucsystem.Data;

namespace Aucsystem.Controllers;

public class HomeController(ILogger<HomeController> logger, ApplicationDbContext context) : Controller
{
    private readonly ILogger<HomeController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Auctions()
    {
        return View();
    }

    public async Task<IActionResult> Properties()
    {
        var properties = await _context.Properties.Include(p => p.Seller).ToListAsync();

        if (properties == null)
        {
            properties = new List<Property>();
        } else{
            
        }

        return View(properties);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
