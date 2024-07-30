using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Project_Orange.Data;
using MVC_Project_Orange.Models;

namespace MVC_Project_Orange.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            int categoryCount = _context.Categories.Count();
            ViewBag.CategoryCount = categoryCount;

            int Usercount = _context.Users.Count();
            ViewBag.UserCount = Usercount;

            int ProductCount = _context.Products.Count();
            ViewBag.ProductCount = ProductCount;

            var totalTransactions = _context.Transactions
                              .Include(t => t.Product)
                              .Sum(t => t.Quantity * t.Product.Price);
            ViewBag.TotalTransactions = totalTransactions;
            return View();
        }
        //Products
        public async Task<IActionResult> ManageProducts()
        {
            var applicationDbContext = _context.Products.Include(p => p.Category);
            return View(await applicationDbContext.ToListAsync());
        }
    }
}
