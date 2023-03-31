using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using publishing.Areas.Identity.Data;
using publishing.Models;
using publishing.Models.ViewModels;
using publishing.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace publishing.Controllers
{
    public class CartController : Controller
    {
        private readonly PublishingDBContext _context;
        private readonly UserManager<publishingUser> _userManager;
        private string NameCart { get { return $"{_userManager.GetUserAsync(HttpContext.User).Result.Email}_Cart"; } }

        public CartController(PublishingDBContext context, UserManager<publishingUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index(int? bookingId)
        {
            if (bookingId == null)
                return NotFound();

            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>(NameCart) ?? new List<CartItem>();
            CartViewModel smallCartModel = new()
            {
                CartItems = cart,
                GrandTotal = cart.Sum(x => x.Quantity * x.Product.Cost)
            };
            return View(smallCartModel);
        }

        public async Task<IActionResult> Add(int id)
        {
            Product product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>(NameCart) ?? new List<CartItem>();
            CartItem cartItem = cart.Where(c => c.Product.Id == id).FirstOrDefault();
            if (cartItem == null)
                cart.Add(new CartItem(product));
            else
                cartItem.Quantity += 1;

            HttpContext.Session.SetJson(NameCart, cart);

            return Redirect(Request.Headers["Referer"].ToString());

        }

        public async Task<IActionResult> Decrease(int id)
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>(NameCart);
            CartItem cartItem = cart.Where(c => c.Product.Id == id).FirstOrDefault();

            if (cartItem.Quantity > 1)
                --cartItem.Quantity;
            else
                cart.RemoveAll(p => p.Product.Id == id);

            if (cart.Count == 0)
                HttpContext.Session.Remove(NameCart);
            else
                HttpContext.Session.SetJson(NameCart, cart);


            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> Remove(int id)
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>(NameCart);
            cart.RemoveAll(p => p.Product.Id == id);

            if (cart.Count == 0)
                HttpContext.Session.Remove(NameCart);
            else
                HttpContext.Session.SetJson(NameCart, cart);


            return Redirect(Request.Headers["Referer"].ToString());
        }

        public IActionResult Clear()
        {
            HttpContext.Session.Remove(NameCart);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> SetBooking(int? bookingId, double? grandTotal)
        {
            if (bookingId == null || grandTotal == null)
                return NotFound();

            List<CartItem> cartItems = HttpContext.Session.GetJson<List<CartItem>>(NameCart);

            if (bookingId == -1)
            {
                Booking booking = new Booking();
                booking.Start = DateTime.Now.Date;
                booking.Status = "Ожидание";
                booking.Cost = grandTotal.Value;
                
                _context.Bookings.Add(booking);
                _context.SaveChanges();

                Booking lastBooking = _context.Bookings.Include(b=> b.BookingProducts).OrderByDescending(b => b.Id).First();
                foreach (var cartItem in cartItems)
                {
                    BookingProduct bookingProduct = new BookingProduct();
                    bookingProduct.Product = _context.Products.First(p => p.Id == cartItem.Product.Id);
                    bookingProduct.ProductId = cartItem.Product.Id;
                    bookingProduct.Booking = lastBooking;
                    bookingProduct.BookingId = lastBooking.Id;
                    bookingProduct.Edition = cartItem.Quantity;

                    _context.BookingProducts.Add(bookingProduct);
                }
            }
            else 
            {
                Booking existBooking = _context.Bookings.First(b => b.Id == bookingId);
                if (existBooking == null)
                    return NotFound();

                existBooking.Cost += grandTotal.Value;

                foreach (var cartItem in cartItems)
                {
                    BookingProduct existBookingProduct = _context.BookingProducts.FirstOrDefault(bp => bp.ProductId == cartItem.Product.Id && bp.BookingId == existBooking.Id);
                    if (existBookingProduct != null)
                    {
                        existBookingProduct.Edition += cartItem.Quantity;
                    }
                    else 
                    {
                        existBookingProduct = new BookingProduct();
                        existBookingProduct.Product = _context.Products.First(p => p.Id == cartItem.Product.Id);
                        existBookingProduct.ProductId = cartItem.Product.Id;
                        existBookingProduct.Booking = existBooking;
                        existBookingProduct.BookingId = existBooking.Id;
                        existBookingProduct.Edition = cartItem.Quantity;

                        _context.BookingProducts.Add(existBookingProduct);
                    }
                }
            }

            HttpContext.Session.Remove(NameCart);
            _context.SaveChanges();
            return RedirectToAction("CustomerBookings", "Customers", new {emailCustomer = _userManager.GetUserAsync(HttpContext.User).Result.Email});
        }
    }
}
