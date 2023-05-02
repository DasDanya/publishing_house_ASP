using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using publishing.Models;

namespace publishing.Controllers
{
    [Authorize(Roles ="manager,admin")]
    public class EmployeesController : Controller
    {
        private readonly PublishingDBContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public EmployeesController(PublishingDBContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
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

            if (!employee.Visual.IsNullOrEmpty())
            {
                byte[] photodata = System.IO.File.ReadAllBytes(_appEnvironment.WebRootPath + employee.Visual);
                ViewBag.Photodata = photodata;
            }
            else
            {
                ViewBag.Photodata = null;
            }

            return View(employee);
        }

        // GET: Employees/Create
        [Authorize(Roles ="admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Surname,Middlename,Type,Phone,Email,Birthday,Description")] Employee employee, IFormFile photo)
        {
            if (ModelState.IsValid)
            {
                if (!IsNewEmployee(employee,"create"))
                    return RedirectToAction("Create");

                if (photo != null)
                {
                    string extension = Path.GetExtension(photo.FileName);
                    string[] extensions = { ".jpg", ".jpeg", ".png", ".bmp"};
                    if (!extensions.Contains(extension))
                        return RedirectToAction("Index", "Error", new { errorMessage = "Файл для изображения должен иметь одно из следующих расширений: .jpg, .jpeg, .png, .tif, .bmp"});

                    Random random = new Random();
                    string path = "/Files/" + $"{employee.Surname}_{employee.Name}_{random.Next(1, int.MaxValue - 1)}{extension}";
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await photo.CopyToAsync(fileStream);
                    }
                    employee.Visual = path;
                }
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else 
            {
                string erorrs = "";
                foreach (var item in ModelState)
                {
                    foreach (var error in item.Value.Errors)
                    {
                        erorrs += error.ErrorMessage + "\t";
                    }
                }
                return NotFound(erorrs);
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        [Authorize(Roles ="admin")]
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

            if (!employee.Visual.IsNullOrEmpty())
            {
                byte[] photodata = System.IO.File.ReadAllBytes(_appEnvironment.WebRootPath + employee.Visual);
                ViewBag.Photodata = photodata;
            }
            else
            {
                ViewBag.Photodata = null;
            }

            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Surname,Middlename,Type,Phone,Email,Birthday,Visual,Description")] Employee employee, IFormFile? photo)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (!IsNewEmployee(employee,"edit"))
                    return RedirectToAction("Edit",new {id});

                if (photo != null)
                {
                    string extension = Path.GetExtension(photo.FileName);
                    string[] extensions = { ".jpg", ".jpeg", ".png", ".bmp" };
                    if (!extensions.Contains(extension))
                        return RedirectToAction("Index", "Error", new { errorMessage = "Файл для изображения должен иметь одно из следующих расширений: .jpg, .jpeg, .png, .tif, .bmp" });

                    Random random = new Random();
                    string path = "/Files/" + $"{employee.Surname}_{employee.Name}_{random.Next(1, int.MaxValue - 1)}{extension}";
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await photo.CopyToAsync(fileStream);
                    }

                    if (!employee.Visual.IsNullOrEmpty())
                    {
                        System.IO.File.Delete(_appEnvironment.WebRootPath + employee.Visual);
                    }

                    employee.Visual = path;
                }
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
        [Authorize(Roles ="admin")]
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

            if (!employee.Visual.IsNullOrEmpty())
            {
                byte[] photodata = System.IO.File.ReadAllBytes(_appEnvironment.WebRootPath + employee.Visual);
                ViewBag.Photodata = photodata;
            }
            else
            {
                ViewBag.Photodata = null;
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

            var employee = _context.Employees.Include(e => e.Bookings).FirstOrDefault(e => e.Id == id);
            if (employee != null)
            {
                if (employee.Bookings.Count != 0)
                    return RedirectToAction("Delete", new { id });
                
                if (!employee.Visual.IsNullOrEmpty())
                {
                    System.IO.File.Delete(_appEnvironment.WebRootPath + employee.Visual);
                }

                _context.Employees.Remove(employee);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
          return (_context.Employees?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public IActionResult LinkEmployeeWithBooking(int? id)
        {
            if(id == null)
                return NotFound();

            Employee employee = _context.Employees.Include(e => e.Bookings).Single(e => e.Id == id);
            if(employee == null)
                return NotFound();

            ViewBag.employeeId = id;
            return View(_context.Bookings.Where(b => b.Status == "Выполняется" && !employee.Bookings.Contains(b)));
        }

        [HttpPost]
        public async Task<IActionResult> LinkEmployeeWithBooking(int? employeeId, int[]? selectedBookings)
        {
            if (employeeId == null || selectedBookings == null)
                return NotFound();

            Employee employee = _context.Employees.Find(employeeId);
            if (employee == null)
                return NotFound();

            foreach (var bookingId in selectedBookings)
            {
                Booking booking = _context.Bookings.Find(bookingId);
                
                if (booking == null)
                    return NotFound();

                booking.Employees.Add(employee);
                //employee.Bookings.Add(booking);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = employeeId });
        }

        private bool IsNewEmployee(Employee employee,string typeAction) 
        {
            Employee existEmployee = null;

            if(typeAction == "create")
                existEmployee = _context.Employees.FirstOrDefault(e => e.Email == employee.Email | e.Phone == employee.Phone);
            else if(typeAction == "edit")
                existEmployee = _context.Employees.FirstOrDefault(e => (e.Email == employee.Email | e.Phone == employee.Phone) & e.Id != employee.Id);

            if (existEmployee == null)
                return true;

            return false;
        }
    }
}
