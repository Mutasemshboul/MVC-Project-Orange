using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Project_Orange.Data;
using MVC_Project_Orange.Models;
using Newtonsoft.Json;

namespace MVC_Project_Orange.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        public static decimal Total { get; set; }

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var cart = GetCart();
            decimal subtotal = cart.Sum(item => item.Price * item.Quantity);


            if (TempData["Total"] != null)
            {
                ViewBag.Total = Convert.ToDecimal(TempData["Total"]);
            }
            else
            {
                ViewBag.Total = subtotal;
            }
            return View(cart);
        }
        [Authorize(Roles = SD.Role_Customer)]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            List<CartItem> cart = GetCart();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    ImgUrl=product.ImgURL
                   
                });
            }
            SaveCart(cart);
            return RedirectToAction("Index");  // Or return to a view that shows the cart
        }
        private List<CartItem> GetCart()
        {
            var sessionData = HttpContext.Session.GetString("Cart");
            return sessionData == null ? new List<CartItem>() : JsonConvert.DeserializeObject<List<CartItem>>(sessionData);
        }
        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
        }
        public IActionResult RemoveFromCart(int productId)
        {
            List<CartItem> cart = GetCart();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
                SaveCart(cart);
            }
            return RedirectToAction("Index");  
        }

        public async Task<Coupon> ValidateCoupon(string code)
        {
            var coupon = await _context.Coupons
                                       .Where(c => c.Code == code && c.Status == "Active" && c.ExpiryDate >= DateTime.UtcNow)
                                       .FirstOrDefaultAsync();
            return coupon;
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(string couponCode)
        {
            var coupon = await ValidateCoupon(couponCode);
            if (coupon == null)
            {
                TempData["ErrorMessage"] = "Invalid or expired coupon.";
                return RedirectToAction("Index");
            }

            var cart = GetCart();
            decimal subtotal = cart.Sum(item => item.Price * item.Quantity);
            decimal discount = subtotal * 0.20m;  
            decimal total = subtotal - discount;

            TempData["Total"] = total.ToString();




            TempData["SuccessMessage"] = $"Coupon applied! 20% discount has been applied.";
            return RedirectToAction("Index");
        }

    }
}
