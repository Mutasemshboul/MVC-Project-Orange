using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Project_Orange.Models;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVC_Project_Orange.Data;

namespace MVC_Project_Orange.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var applicationDbContext = _context.Products.Include(p => p.Category);
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> ShopAsync()
        {
            var applicationDbContext = _context.Products.Include(p => p.Category);
            return View(await applicationDbContext.ToListAsync());
        }
        [Authorize(Roles =SD.Role_Admin)]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult About_Us()
        {
            int categoryCount = _context.Categories.Count();
            ViewBag.CategoryCount = categoryCount;

            int Usercount = _context.Users.Count();
            ViewBag.UserCout = Usercount;

            int ProductCount =_context.Products.Count();
            ViewBag.ProductCount = ProductCount;

            return View();
        }
        public IActionResult Shop_Detailes()
        {
            return View();
        }
        public IActionResult Shoping_Cart()
        {
            return View();
        }
        public IActionResult Check_Out()
        {
            return View();
        }
        public IActionResult Contacts()
        {
            return View();
        }
        public IActionResult Testimonial()
        {
            return View();
        }

    }
}
