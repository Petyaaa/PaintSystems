using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PaintSystem.Data;
using PaintSystem.Models;

namespace PaintSystem.Controllers
{
    public class PaintsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaintsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Paints
        public async Task<IActionResult> Index(string paintType, string paintColor, string paintBrand, string weight)
        {
            // Get unique values for the dropdowns
            IQueryable<string> typeQuery = from p in _context.Paints
                                           orderby p.Type
                                           select p.Type;

            IQueryable<string> colorQuery = from p in _context.Paints
                                            orderby p.ColorName
                                            select p.ColorName;

            IQueryable<string> brandQuery = from p in _context.Paints
                                            orderby p.Brand
                                            select p.Brand;

            var paints = from p in _context.Paints
                         select p;

            // Filter by Type
            if (!string.IsNullOrEmpty(paintType))
            {
                paints = paints.Where(x => x.Type == paintType);
            }

            // Filter by ColorName
            if (!string.IsNullOrEmpty(paintColor))
            {
                paints = paints.Where(x => x.ColorName == paintColor);
            }

            // Filter by Brand
            if (!string.IsNullOrEmpty(paintBrand))
            {
                paints = paints.Where(x => x.Brand == paintBrand);
            }

            // Filter by Weight (if Weight is a number like double/int/decimal, change .ToString() to <= weightVal)
            if (!string.IsNullOrEmpty(weight))
            {
                paints = paints.Where(x => x.Weight.ToString().Contains(weight));
            }

            ViewBag.PaintTypes = new SelectList(await typeQuery.Distinct().ToListAsync());
            ViewBag.PaintColors = new SelectList(await colorQuery.Distinct().ToListAsync());
            ViewBag.PaintBrands = new SelectList(await brandQuery.Distinct().ToListAsync());

            return View(await paints.ToListAsync());
        }

        // GET: Paints/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paints = await _context.Paints
                .FirstOrDefaultAsync(m => m.id == id);
            if (paints == null)
            {
                return NotFound();
            }

            return View(paints);
        }

        // GET: Paints/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Paints/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,ColorName,Brand,Type,Weight,ColourCode,Description")] Paints paints)
        {
            if (ModelState.IsValid)
            {
                _context.Add(paints);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(paints);
        }

        // GET: Paints/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paints = await _context.Paints.FindAsync(id);
            if (paints == null)
            {
                return NotFound();
            }
            return View(paints);
        }

        // POST: Paints/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,ColorName,Brand,Type,Weight,ColourCode,Description")] Paints paints)
        {
            if (id != paints.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paints);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaintsExists(paints.id))
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
            return View(paints);
        }

        // GET: Paints/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paints = await _context.Paints
                .FirstOrDefaultAsync(m => m.id == id);
            if (paints == null)
            {
                return NotFound();
            }

            return View(paints);
        }

        // POST: Paints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paints = await _context.Paints.FindAsync(id);
            if (paints != null)
            {
                _context.Paints.Remove(paints);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaintsExists(int id)
        {
            return _context.Paints.Any(e => e.id == id);
        }
    }
}
