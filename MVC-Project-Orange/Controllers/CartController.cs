using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC_Project_Orange.Data;
using MVC_Project_Orange.Models;
using Newtonsoft.Json;

namespace MVC_Project_Orange.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var cart = GetCart();
            decimal subtotal = cart.Sum(item => item.Price * item.Quantity);
           
            ViewBag.Total = subtotal;
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

    }
}
