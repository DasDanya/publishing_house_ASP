using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using publishing.Areas.Identity.Data;
using publishing.Models;
using publishing.Models.ViewModels;
using publishing.Infrastructure;

namespace publishing.Controllers
{
    public class CartController : Controller
    {
        private readonly PublishingDBContext _context;
        private readonly UserManager<publishingUser> _userManager;

        public CartController(PublishingDBContext context, UserManager<publishingUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            CartViewModel smallCartModel = new()
            {
                CartItems = cart,
                GrandTotal = cart.Sum(x => x.Quantity * x.Product.Cost)
            };
            return View(smallCartModel);
        }

        public async Task<IActionResult> Add(int id) 
        {
            Product product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            CartItem cartItem = cart.Where(c => c.Product.Id == id).FirstOrDefault();
            if (cartItem == null)
                cart.Add(new CartItem(product));
            else
                cartItem.Quantity += 1;

            HttpContext.Session.SetJson("Cart", cart);

            return Redirect(Request.Headers["Referer"].ToString());
           
        }
    }
}
