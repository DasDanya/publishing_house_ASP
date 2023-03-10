using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using publishing.Models;


namespace publishing.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly PublishingDBContext _context;

        public EmployeesController(PublishingDBContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
              return _context.Employees != null ? 
                          View(await _context.Employees.ToListAsync()) :
                          Problem("Entity set 'PublishingDBContext.Employees'  is null.");
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.Include(e=> e.Bookings).Where(e=> e.Id == id).FirstOrDefaultAsync();

            if (employee == null) 
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Surname,Middlename,Type,Phone,Email,Birthday,Visual")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Surname,Middlename,Type,Phone,Email,Birthday,Visual")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'PublishingDBContext.Employees'  is null.");
            }
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
          return (_context.Employees?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> UnpinOrder(int? bookingId, int? employeeId)
        {
            if (bookingId == null || employeeId == null)
                return NotFound();

            Booking booking = _context.Bookings.Include(b => b.Employees).Where(b => b.Id == bookingId).First();
            if (booking == null)
                return NotFound();

            booking.Employees.Remove(_context.Employees.Find(employeeId));
            _context.SaveChanges();
            
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public IActionResult LinkEmployeeWithBooking(int? id)
        {
            if(id == null)
                return NotFound();

            Employee employee = _context.Employees.Include(e=> e.Bookings).Where(e => e.Id == id).First();
            if(employee == null)
                return NotFound();

            ViewBag.employeeId = id;
            return View(_context.Bookings.Where(b => b.Status == "Выполняется" && !employee.Bookings.Contains(b)));
        }

        [HttpPost]
        public async Task<IActionResult> LinkEmployeeWithBooking(int? employeeId, int? bookingId)
        {
            if (employeeId == null || bookingId == null)
                return NotFound();

            Booking booking = _context.Bookings.Find(bookingId);
            Employee employee = _context.Employees.Find(employeeId);
            if (employee == null || booking == null)
                return NotFound();

            booking.Employees.Add(employee);
            employee.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = employeeId });
        }
    }
}
