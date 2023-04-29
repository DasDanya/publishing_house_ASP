using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using publishing.Areas.Identity.Data;
using publishing.Models;
using publishing.Models.ViewModels;
using publishing.Infrastructure;

namespace publishing.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly PublishingDBContext _context;
        private readonly UserManager<publishingUser> _userManager;

        public CustomersController(PublishingDBContext context, UserManager<publishingUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Customers
        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> Index()
        {
            return _context.Customers != null ?
                        View(await _context.Customers.ToListAsync()) :
                        Problem("Entity set 'PublishingDBContext.Customers'  is null.");
        }

        // GET: Customers/Details/5
        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.Include(c => c.Products)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            CustomerDetailsViewModel model = new CustomerDetailsViewModel();
            model.Customer = customer;

            //var products = _context.Products.Include(p => p.TypeProduct).Where(p => customer.Products.Contains(p)).ToList();
            //if(products == null)
            //    return NotFound();

            model.Bookings = GetBookings(customer);
            model.Products = GetProducts(customer,true);
            //model.Bookings.AddRange(from bp in _context.BookingProducts.Include(bp => bp.Booking).Include(bp => bp.Product) where products.Contains(bp.Product) && !model.Bookings.Contains(bp.Booking) && bp.Booking != null select bp.Booking);
            //model.Products.AddRange((from bp in _context.BookingProducts.Include(bp => bp.Product) where products.Contains(bp.Product) && bp.Booking != null select bp.Product).Distinct());

            //foreach (var product in products)
            //{
            //    model.Bookings.AddRange(from bp in _context.BookingProducts.Include(bp => bp.Booking) where bp.ProductId == product.Id && !model.Bookings.Contains(bp.Booking) && bp.Booking != null select bp.Booking);
            //    model.Products.AddRange((from bp in _context.BookingProducts.Include(bp => bp.Product) where bp.ProductId == product.Id && bp.Booking != null select bp.Product).Distinct());
            //}

            return View(model);
        }

        // GET: Customers/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Phone,Email")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Phone,Email")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'PublishingDBContext.Customers'  is null.");
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return (_context.Customers?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private List<Booking> GetBookings(Customer customer)
        {
            List<Booking> bookings = new List<Booking>();
            var customerProducts = _context.Products.Include(p => p.TypeProduct).Where(p => customer.Products.Contains(p)).ToList();
            if (customerProducts != null)
            {
                bookings.AddRange((from bp in _context.BookingProducts.Include(bp => bp.Booking).Include(bp => bp.Product) where customerProducts.Contains(bp.Product) && bp.Booking != null select bp.Booking).Distinct());
            }

            return bookings;
        }

        private List<Product> GetProducts(Customer customer,bool InBooking)
        {
            List<Product> products = new List<Product>();
            var customerProducts = _context.Products.Include(p => p.TypeProduct).Include(p=>p.BookingProducts).ThenInclude(bp=>bp.Booking).Where(p => customer.Products.Contains(p)).ToList();
            if (customerProducts != null)
            {
                if (InBooking)
                    products.AddRange((from bp in _context.BookingProducts.Include(bp => bp.Product) where customerProducts.Contains(bp.Product) && bp.Booking != null select bp.Product).Distinct());

                else
                    products = customerProducts;
            }
            return products;
        }

        [Authorize(Roles = "customer")]
        public async Task<IActionResult> CustomerBookings(string emailCustomer)
        {
            var customer = _context.Customers.Include(c => c.Products).FirstOrDefault(c => c.Email == emailCustomer);
            if (customer == null)
                return NotFound();

            if (!IsCustomerEmail(emailCustomer))
                return new StatusCodeResult(403);

            //return NotFound($"{customer.Name}, {customer.Email}, {customer.Phone}, {customer.Products}");
            return View(GetBookings(customer));
        }

        [Authorize(Roles = "customer")]
        public async Task<IActionResult> CustomerProducts(string emailCustomer)
        {
            var customer = _context.Customers.Include(c => c.Products).FirstOrDefault(c => c.Email == emailCustomer);
            if (customer == null)
                return NotFound();

            if (!IsCustomerEmail(emailCustomer))
                return new StatusCodeResult(403);

            return View(GetProducts(customer, false));
        }

        private bool IsCustomerEmail(string email)
        {
            //var user =  _userManager.GetUserAsync(HttpContext.User);

            var user = _userManager.GetUserAsync(HttpContext.User);
            //var user = await _userManager.GetUserAsync(HttpContext.User);
            if (_userManager.IsInRoleAsync(user.Result, "customer").Result)
            {
                if (email == user.Result.Email)
                    return true;
            }
            return false;
        }


        public IActionResult TransitionToSelectProducts(int bookingId)
        {
            HttpContext.Session.SetJson($"BookingBy_{_userManager.GetUserAsync(HttpContext.User).Result.Email}", bookingId);
            return Redirect("SelectProducts");
        }


        [Authorize(Roles = "customer")]
        public IActionResult SelectProducts(string typeProduct = "", int p = 1)
        {
            //if (bookingId == null)
            //    return NotFound();
            int bookingId = HttpContext.Session.GetJson<int>($"BookingBy_{_userManager.GetUserAsync(HttpContext.User).Result.Email}");
            if (bookingId == 0)
                return NotFound();

            //if (bookingId != -1)
            //{
            //    BookingsController bookingsController = new BookingsController();
            //    if (!bookingsController.IsUserBooking(bookingId.Value))
            //        return new StatusCodeResult(403);
            //}

            int pageSize = 3;
            ViewBag.pageNumber = p;
            ViewBag.pageRange = pageSize;
            ViewBag.typeProduct = typeProduct;
            //ViewBag.bookingId = bookingId;

            var user = _userManager.GetUserAsync(HttpContext.User);
            var customer = _context.Customers.Include(c => c.Products).FirstOrDefault(c => c.Email == user.Result.Email);
            if (customer == null)
                return NotFound();

            var customerProducts = _context.Products.Include(p => p.TypeProduct).Where(p => customer.Products.Contains(p));
            List<string> typesProducts = new List<string>();
            foreach (var product in customerProducts)
            {
                if (!typesProducts.Contains(product.TypeProduct.Type))
                    typesProducts.Add(product.TypeProduct.Type);
            }
            ViewBag.typeProducts = typesProducts;

            // Корзина
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>($"{user.Result.Email}_Product");
            SmallCartViewModel smallCartModel = null;

            if (cart != null && cart.Count > 0)
            {
                //var customerCart = _context.Customers.Include(c => c.Products).FirstOrDefault(c => c.Products.Contains(cart.First().Product));
                //if (customerCart == null)
                //    return NotFound();

                //if (customerCart.Email != user.Result.Email) 
                //{
                //    //Очистка Json
                //    HttpContext.Session.Remove("Cart");
                //}
                //else
                //{
                smallCartModel = new()
                {
                    NumberOfItems = cart.Sum(x => x.Quantity),
                    TotalAmount = cart.Sum(x => x.Quantity * x.Product.Cost)

                };
            }
            //smallCartModel = null;            

            ViewBag.smallCartModel = smallCartModel;
            //Конец корзины

            if (typeProduct == "")
            {
                ViewBag.totalPages = (int)Math.Ceiling((decimal)customer.Products.Count / pageSize);
                ViewBag.productsPhoto = GetVisualProducts(customer.Products.ToList());
                return View(customer.Products.OrderBy(p => p.Name).Skip((p - 1) * pageSize).Take(pageSize).ToList());
            }

            //TypeProduct type = _context.TypeProducts.FirstOrDefault(tp => tp.Type == typeProduct);
            //if(type == null)
            //    return Redirect(Request.Headers["Referer"].ToString());

            var productsByType = _context.Products.Include(p => p.TypeProduct).Where(p => p.TypeProduct.Type == typeProduct && customer.Products.Contains(p)).ToList();
            if (productsByType.Count == 0)
                return Redirect(Request.Headers["Referer"].ToString());

                ViewBag.productsPhoto = GetVisualProducts(productsByType);

            ViewBag.totalPages = (int)Math.Ceiling((decimal)productsByType.Count / pageSize);

            return View(productsByType.OrderBy(p => p.Name).Skip((p - 1) * pageSize).Take(pageSize).ToList());
            
        }

        private List<VisualProduct> GetVisualProducts(List<Product> products) 
        {
            List<VisualProduct> productsPhoto = new List<VisualProduct>();

            if (products != null && products.Count > 0)
            {
                foreach (var product in products)
                {
                    productsPhoto.AddRange((from vp in _context.VisualProducts where vp.ProductId == product.Id select vp));
                }
            }

            return productsPhoto;
        }
    }
}
