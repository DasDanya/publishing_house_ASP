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
    public class CustomerProductsController : Controller
    {
        private readonly PublishingDBContext _context;

        public CustomerProductsController(PublishingDBContext context)
        {
            _context = context;
        }

        // GET: CustomerProducts
        public async Task<IActionResult> Index()
        {
            var publishingDBContext = _context.CustomerProducts.Include(c => c.Customer).Include(c => c.Product);
            return View(await publishingDBContext.ToListAsync());
        }

        // GET: CustomerProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CustomerProducts == null)
            {
                return NotFound();
            }

            var customerProduct = await _context.CustomerProducts
                .Include(c => c.Customer)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customerProduct == null)
            {
                return NotFound();
            }

            return View(customerProduct);
        }

        // GET: CustomerProducts/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            return View();
        }

        // POST: CustomerProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CustomerId,ProductId")] CustomerProduct customerProduct)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customerProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", customerProduct.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", customerProduct.ProductId);
            return View(customerProduct);
        }

        // GET: CustomerProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CustomerProducts == null)
            {
                return NotFound();
            }

            var customerProduct = await _context.CustomerProducts.FindAsync(id);
            if (customerProduct == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", customerProduct.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", customerProduct.ProductId);
            return View(customerProduct);
        }

        // POST: CustomerProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerId,ProductId")] CustomerProduct customerProduct)
        {
            if (id != customerProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customerProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerProductExists(customerProduct.Id))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", customerProduct.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", customerProduct.ProductId);
            return View(customerProduct);
        }

        // GET: CustomerProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CustomerProducts == null)
            {
                return NotFound();
            }

            var customerProduct = await _context.CustomerProducts
                .Include(c => c.Customer)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customerProduct == null)
            {
                return NotFound();
            }

            return View(customerProduct);
        }

        // POST: CustomerProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CustomerProducts == null)
            {
                return Problem("Entity set 'PublishingDBContext.CustomerProducts'  is null.");
            }
            var customerProduct = await _context.CustomerProducts.FindAsync(id);
            if (customerProduct != null)
            {
                _context.CustomerProducts.Remove(customerProduct);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerProductExists(int id)
        {
          return (_context.CustomerProducts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
