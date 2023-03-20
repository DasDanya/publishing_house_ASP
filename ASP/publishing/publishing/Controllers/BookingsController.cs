using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using publishing.Models;
using publishing.Models.ViewModels;

namespace publishing.Controllers
{
    public class BookingsController : Controller
    {
        private readonly PublishingDBContext _context;

        public BookingsController(PublishingDBContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            //BookingIndexViewModel model = new BookingIndexViewModel();
            //model.Bookings = _context.Bookings.Include(b => b.PrintingHouse).Include(b => b.Employees).ToList();
            //var products = (from bp in _context.BookingProducts.Include(bp => bp.Product).Include(bp => bp.Booking) select bp.Product).ToList();
            //model.Products = _context.Products.Include(p => p.Customer).Where(p => products.Contains(p)).ToList();
            //return View(model);
            var publishingDBContext = _context.Bookings.Include(b => b.PrintingHouse).Include(b => b.Employees);
            return View(await publishingDBContext.ToListAsync());
        }

        // GET: Bookings/Detail
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Bookings == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.PrintingHouse).Include(b=>b.Employees).Include(b=>b.BookingProducts)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            var products = (from bp in _context.BookingProducts.Include(bp => bp.Product).Include(bp => bp.Booking) where bp.BookingId == id select bp.Product).ToList();
            if (products == null)
                return NotFound();

            var modelProducts = _context.Products.Include(p => p.Customer).Where(p => products.Contains(p)).ToList();
            if (modelProducts == null)
                return NotFound();

            //var bookingProducts = (from bp in _context.BookingProducts.Include(bp => bp.Product).Include(bp => bp.Booking) where bp.BookingId == id select bp).ToList();
            //_context.Products.Include(p => p.Customer);
            BookingDetailsViewModel model = new BookingDetailsViewModel();
            model.Booking = booking;
            model.BookingProducts = (from bp in _context.BookingProducts.Include(bp => bp.Product).Include(bp => bp.Booking) where bp.BookingId == id select bp).ToList();
            //model.Products = modelProducts;

            return View(model);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["PrintingHouseId"] = new SelectList(_context.PrintingHouses, "Id", "Name");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Start,End,Status,Cost,PrintingHouseId")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PrintingHouseId"] = new SelectList(_context.PrintingHouses, "Id", "Name", booking.PrintingHouseId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Bookings == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["PrintingHouseId"] = new SelectList(_context.PrintingHouses, "Id", "Name", booking.PrintingHouseId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Start,End,Status,Cost,PrintingHouseId")] Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
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
            ViewData["PrintingHouseId"] = new SelectList(_context.PrintingHouses, "Id", "Name", booking.PrintingHouseId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Bookings == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.PrintingHouse)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Bookings == null)
            {
                return Problem("Entity set 'PublishingDBContext.Bookings'  is null.");
            }
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
          return (_context.Bookings?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> UnpinProduct(int? bookingId, int? productId)
        {
            if (bookingId == null || productId == null)
                return NotFound();

            //BookingProduct bookingProduct = _context.BookingProducts.Single(bp => bp.ProductId == productId && bp.BookingId == bookingId);
            Booking booking = _context.Bookings.Find(bookingId);
            Product product = _context.Products.Find(productId);
            if (booking == null || product == null)
                return NotFound();

            if (_context.BookingProducts.Count(bp => bp.BookingId == bookingId) > 1)
            {
                _context.BookingProducts.Remove(_context.BookingProducts.Single(bp => bp.ProductId == productId && bp.BookingId == bookingId));
                _context.SaveChanges();

                CostController costController = new CostController(_context);
                costController.SetCostBooking(booking);
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> UnpinEmployee(int? bookingId, int? employeeId)
        {
            if (bookingId == null || employeeId == null)
                return NotFound();

            Booking booking = _context.Bookings.Include(b => b.Employees).Where(b => b.Id == bookingId).First();
            if (booking == null)
                return NotFound();

            if (booking.Employees.Count > 1)
            {
                booking.Employees.Remove(_context.Employees.Find(employeeId));
                _context.SaveChanges();
            }

            return Redirect(Request.Headers["Referer"].ToString());
            //return View("Details", _context.Employees.Include(e => e.Bookings).Where(e => e.Id == employeeId).First());
        }


        public IActionResult LinkEmployeeWithBooking(int? id)
        {
            if (id == null)
                return NotFound();

            Booking booking = _context.Bookings.Include(b => b.Employees).Single(b => b.Id == id);
            if (booking == null)
                return NotFound();

            ViewBag.bookingId = id;
            return View(_context.Employees.Where(e=> !booking.Employees.Contains(e)));
        }

        [HttpPost]
        public async Task<IActionResult> LinkEmployeeWithBooking(int? bookingId, int[]? selectedEmployees) 
        { 
            if(bookingId == null || selectedEmployees == null)
                return NotFound();

            Booking booking = _context.Bookings.Find(bookingId);
            if (booking == null)
                return NotFound();

            foreach (var employeeId in selectedEmployees)
            {
                Employee employee = _context.Employees.Find(employeeId);

                if (employee == null)
                    return NotFound();

                booking.Employees.Add(employee);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = bookingId });
        }
    }
}
