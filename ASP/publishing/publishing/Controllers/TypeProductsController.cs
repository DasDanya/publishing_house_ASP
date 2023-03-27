using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using publishing.Models;
using publishing.Models.ViewModels;

namespace publishing.Controllers
{
    [Authorize(Roles ="admin,manager")]
    public class TypeProductsController : Controller
    {
        private readonly PublishingDBContext _context;

        public TypeProductsController(PublishingDBContext context)
        {
            _context = context;
        }

        // GET: TypeProducts
        public async Task<IActionResult> Index()
        {
              return _context.TypeProducts != null ? 
                          View(await _context.TypeProducts.ToListAsync()) :
                          Problem("Entity set 'PublishingDBContext.TypeProducts'  is null.");
        }

        // GET: TypeProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TypeProducts == null)
            {
                return NotFound();
            }

            var typeProduct = await _context.TypeProducts.Include(tp=> tp.Products)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (typeProduct == null)
            {
                return NotFound();
            }

            var products = ((from bp in _context.BookingProducts.Include(b => b.Product) where bp.BookingId != null select bp.Product).Distinct()).ToList();
            if (products == null)
                return NotFound();

            TypeProductDetailsViewModel model = new TypeProductDetailsViewModel();
            model.TypeProduct = typeProduct;
            model.Products = _context.Products.Include(p => p.Customer).Where(p => typeProduct.Products.Contains(p) && products.Contains(p)).ToList();
            //_context.Products.Include(p=> p.Customer).Where(p => typeProduct.Products.Contains(p)).ToList();
            return View(model);
        }

        // GET: TypeProducts/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: TypeProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,Margin")] TypeProduct typeProduct)
        {
            if (ModelState.IsValid)
            {
                _context.Add(typeProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(typeProduct);
        }

        // GET: TypeProducts/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TypeProducts == null)
            {
                return NotFound();
            }

            var typeProduct = await _context.TypeProducts.FindAsync(id);
            if (typeProduct == null)
            {
                return NotFound();
            }
            return View(typeProduct);
        }

        // POST: TypeProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,Margin")] TypeProduct typeProduct)
        {
            if (id != typeProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(typeProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TypeProductExists(typeProduct.Id))
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
            return View(typeProduct);
        }

        // GET: TypeProducts/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TypeProducts == null)
            {
                return NotFound();
            }

            var typeProduct = await _context.TypeProducts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (typeProduct == null)
            {
                return NotFound();
            }

            return View(typeProduct);
        }

        // POST: TypeProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TypeProducts == null)
            {
                return Problem("Entity set 'PublishingDBContext.TypeProducts'  is null.");
            }
            var typeProduct = await _context.TypeProducts.FindAsync(id);
            if (typeProduct != null)
            {
                _context.TypeProducts.Remove(typeProduct);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TypeProductExists(int id)
        {
          return (_context.TypeProducts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
