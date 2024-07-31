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
using System.Security.Claims;

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
            var applicationDbContext = _context.Products.Include(p => p.Category).Where(p=>!p.IsDeleted);
            ViewBag.testimonials = _context.Testimonials
                                   .Include(t => t.User)  // Eager load the ApplicationUser associated with each testimonial
                                   .Where(t => t.Status == "Accept" && !t.IsDeleted)
                                   .ToList();
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> ShopAsync()
        {
            var applicationDbContext = _context.Products.Include(p => p.Category).Where(p => !p.IsDeleted);
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
        public async Task<IActionResult> Shop_Detailes(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
            // return View();
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
        [Authorize(Roles = SD.Role_Customer)]
        public IActionResult AddTestimonial(string message)
        {
            Testimonial testimonial = new Testimonial();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            testimonial.UserID = userId;
            testimonial.Message = message;
            testimonial.Status = "pending";
            _context.Add(testimonial);
            _context.SaveChanges();
            return View("Testimonial");

        }



    }
}
