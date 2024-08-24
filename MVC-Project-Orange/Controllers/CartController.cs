using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Project_Orange.Data;
using MVC_Project_Orange.Models;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MVC_Project_Orange.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        public static bool Flag { get; set; } = false;

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
            return Json(new { success = true });  // Or return to a view that shows the cart
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
            if (string.IsNullOrWhiteSpace(couponCode))
            {
                return Json(new { success = false, message = "Coupon code cannot be empty." });
            }
            var coupon = await ValidateCoupon(couponCode);
            if (coupon == null)
            {
                return Json(new { success = false, message = "Invalid coupon code." });
            }

            var cart = GetCart();
            decimal subtotal = cart.Sum(item => item.Price * item.Quantity);
            decimal discount = subtotal * 0.20m;  
            decimal total = subtotal - discount;
            Flag = true;

            TempData["Total"] = total.ToString();




            return Json(new { success = true, message = "Coupon applied successfully. 20% discount granted." });
        }

        public async Task<IActionResult> CheackOut()
        {
            var cart = GetCart();
            decimal subtotal = cart.Sum(item => item.Price * item.Quantity);
            if (Flag)
            {
                decimal discount = subtotal * 0.20m;
                decimal total = subtotal - discount;
                ViewBag.Total = total;
            }
            else
            {
                ViewBag.Total = subtotal;
            }
            
            
            return View(cart);
        }

        public IActionResult PlaceOrder()
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                TempData["Error"] = "You must be logged in to checkout.";
                return RedirectToAction("Login", "Account");  // Redirect to login if not logged in
            }

            foreach (var item in cart)
            {
                var transaction = new Transaction
                {
                    UserID = userId,
                    ProductID = item.ProductId,
                    Quantity = item.Quantity,
                    TransactionDate = DateTime.Now
                };
                _context.Transactions.Add(transaction);
            }

             _context.SaveChanges();
            ClearCart();
            Flag = false;
            TempData["Success"] = "Checkout successful!";
            return RedirectToAction("Index");
        }
        private void ClearCart()
        {
            // Clear the cart by setting an empty list or null
            HttpContext.Session.Remove("Cart");
        }

    }
}
