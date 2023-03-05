using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using publishing.Models;
using System.Linq;

namespace publishing.Controllers
{
    public class ManagerBookingController : Controller
    {
        private readonly PublishingDBContext _context;

        public ManagerBookingController(PublishingDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View( await _context.Bookings.ToListAsync());
        }

        //[HttpPost]
        //public async Task<IActionResult> Index(int bookingId, string bookingStatus)
        //{

        //    Booking booking = (from b in _context.Bookings where b.Id == bookingId select b).Single();
        //    if (booking == null)
        //        return NotFound();

        //    if (bookingStatus == "Выполняется")
        //    {
        //        booking.Status = "Выполнен";
        //        _context.SaveChanges();

        //        return RedirectToAction();
        //    }
        //    else if (bookingStatus == "Ожидание")
        //    {
        //        ViewBag.bookingId = bookingId;
        //        return View("ChooseEmployees", await _context.Employees.ToListAsync());
        //        //TempData["bookingId"] = bookingId;
        //        //return RedirectToAction("ChooseEmployees");
        //    }

        //    return View(await _context.Bookings.ToListAsync());
        //}

        public async Task<IActionResult> ChooseEmployees(int? bookingId, string? bookingStatus)
        {
            if (bookingId == null || bookingStatus == null)
                return NotFound();

            Booking booking = (from b in _context.Bookings where b.Id == bookingId select b).Single();
            if (booking == null)
                return NotFound();

            if (bookingStatus == "Выполняется")
            {

                booking.Status = "Выполнен";
                _context.SaveChanges();

                //return RedirectToAction();

                return Redirect(Request.Headers["Referer"].ToString());
                //TempData["bookingId"] = bookingId;
                //TempData["printHouseId"] = 2;
                //return Redirect("~/ManagerBooking/Confirmation");
            }

            //ViewData["bookingId"] = bookingId;

            ViewBag.bookingId = bookingId;
            return View(await _context.Employees.ToListAsync());
        }

        public async Task<IActionResult> ChoosePrintHouse(int? bookingId, int[]? selectedEmployees)
        {
            if (bookingId == null || selectedEmployees == null || selectedEmployees.Length == 0)
                return NotFound();

            Booking booking = (from b in _context.Bookings where b.Id == bookingId select b).Single();
            Employee[] employees = (from emp in _context.Employees where selectedEmployees.Contains(emp.Id) select emp).ToArray();

            if (booking == null || selectedEmployees.Length != employees.Length)
                return NotFound();


            ViewBag.bookingId = bookingId;
            ViewBag.selectedEmployees = selectedEmployees;
            ViewBag.empLength = selectedEmployees.Length;
            //ViewData["bookingId"] = bookingId;
            //ViewData["selectedEmployees"] = selectedEmployees;

            return View(await _context.PrintingHouses.ToListAsync());
        }

        public async Task<IActionResult> ChooseDateOfComplete(int? bookingId, int? printHouseId, int[]? employees)
        {
            if (bookingId == null || printHouseId == null || employees == null || employees.Length == 0)
                return NotFound();

            PrintingHouse printHouse = (from pr in _context.PrintingHouses where pr.Id == printHouseId select pr).Single();
            if (printHouse == null)
                return NotFound();

            ViewBag.bookingId = bookingId;
            //ViewData["bookingId"] = bookingId;
            ViewBag.printHouseId = printHouseId;
            //ViewData["printHouse"] = printHouse;
            //ViewData["selectedEmployees"] = selectedEmployees;
            ViewBag.employees = employees;
            ViewBag.empLength = employees.Length;

            return View();
        }

        public async Task<IActionResult> Confirmation(int? bookingId, int? printHouseId,DateTime finishDate, int[]? employees) 
        {
            if (bookingId == null || printHouseId == null || employees == null || employees.Length == 0)
                return NotFound();

            PrintingHouse printHouse = (from pr in _context.PrintingHouses where pr.Id == printHouseId select pr).Single();
            Employee[] selectedEmployees = (from emp in _context.Employees where employees.Contains(emp.Id) select emp).ToArray();

            if (printHouse == null || selectedEmployees.Length != employees.Length)
                return NotFound();

            ViewBag.printHouseId = printHouse.Id;
            ViewBag.printHouseName = printHouse.Name;

            ViewBag.employees = selectedEmployees;
            ViewBag.empLength = selectedEmployees.Length;

            ViewBag.bookingId = bookingId;
            ViewBag.finishDate = finishDate.ToShortDateString();
            return View();
        }


        public async Task<IActionResult> ConfirmRegistration(int? bookingId, int? printHouseId, int[]? employees, string finishDate) 
        {
            if (bookingId == null || printHouseId == null || employees == null || employees.Length == 0 )
                return NotFound();

            DateTime dateFinish = DateTime.ParseExact(finishDate, "d", null);
            Booking booking = (from b in _context.Bookings where b.Id == bookingId select b).Single();

            booking.Status = "Выполняется";
            booking.End = dateFinish;
            booking.PrintingHouseId = (int)printHouseId;

            var bookingsEmployees = _context.Set<BookingEmployee>();

            for (int i = 0; i < employees.Length; i++)
            {
                bookingsEmployees.Add(new BookingEmployee { BookingId = (int)bookingId, EmployeeId = employees[i]});
            }

            _context.SaveChanges();
            return Redirect("~/ManagerBooking/Index");

        }


        //public async Task<IActionResult> ChooseEmployees(int? bookingId) 
        //{
        //    if (bookingId == null)
        //        return NotFound();

        //    //var booking = await _context.Bookings.FindAsync(bookingId);
        //    ViewData["bookingId"] = bookingId;

        //    return View(_context.Employees.ToListAsync());
        //}

        //[HttpPost]
        //public async Task<IActionResult> ChooseEmployees(int bookingId, int[] selectedEmployees)
        //{
        //    if (bookingId == 0 || selectedEmployees.Length == 0)
        //        return NotFound();
        //    Booking booking = (from b in _context.Bookings where b.Id == bookingId select b).Single();
        //    Employee[] employees = (from emp in _context.Employees where selectedEmployees.Contains(emp.Id) select emp).ToArray();
        //    if (booking == null || selectedEmployees.Length != employees.Length)
        //        return NotFound();


        //    ViewBag.bookingId = bookingId;
        //    ViewBag.employees = employees;
        //    return View("ChoosePrintHouse", await _context.PrintingHouses.ToListAsync());

        //    //TempData["bookingId"] = bookingId;
        //    //return RedirectToAction("/FinalChoose");
        //}

        //[HttpPost]
        //public async Task<IActionResult> ChoosePrintHouse(int bookingId, int printHouseId,Employee[] selectedEmployees)
        //{
        //    if (bookingId == 0 || selectedEmployees.Length == 0 || printHouseId == 0)
        //        return NotFound();
        //    PrintingHouse printingHouse = (from pr in _context.PrintingHouses where pr.Id == printHouseId select pr).Single();
        //    if (printingHouse == null)
        //        return NotFound();

        //    ViewBag.bookingId = bookingId;
        //    ViewBag.printHouse = printingHouse;
        //    ViewBag.employees = selectedEmployees;

        //    return View("ChooseDateOfComplete");
        //}

    }
}
