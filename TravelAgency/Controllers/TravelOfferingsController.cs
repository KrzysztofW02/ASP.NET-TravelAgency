using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TravelAgency.Models;

namespace TravelAgency.Controllers
{
    public class TravelOfferingsController : Controller
    {
        private readonly TravelAgencyDbContext _context;

        public TravelOfferingsController(TravelAgencyDbContext context)
        {
            _context = context;
        }

        // GET: TravelOfferings
        public async Task<IActionResult> Index()
        {
              return _context.TravelOfferings != null ? 
                          View(await _context.TravelOfferings.ToListAsync()) :
                          Problem("Entity set 'TravelAgencyDbContext.TravelOfferings'  is null.");
        }

        // GET: TravelOfferings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TravelOfferings == null)
            {
                return NotFound();
            }

            var travelOffering = await _context.TravelOfferings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (travelOffering == null)
            {
                return NotFound();
            }

            return View(travelOffering);
        }

        // GET: TravelOfferings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TravelOfferings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Destination,StartDate,EndDate,Price,Description")] TravelOffering travelOffering)
        {
            if (ModelState.IsValid)
            {
                _context.Add(travelOffering);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(travelOffering);
        }

        // GET: TravelOfferings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TravelOfferings == null)
            {
                return NotFound();
            }

            var travelOffering = await _context.TravelOfferings.FindAsync(id);
            if (travelOffering == null)
            {
                return NotFound();
            }
            return View(travelOffering);
        }

        // POST: TravelOfferings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Destination,StartDate,EndDate,Price,Description")] TravelOffering travelOffering)
        {
            if (id != travelOffering.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(travelOffering);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TravelOfferingExists(travelOffering.Id))
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
            return View(travelOffering);
        }

        // GET: TravelOfferings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TravelOfferings == null)
            {
                return NotFound();
            }

            var travelOffering = await _context.TravelOfferings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (travelOffering == null)
            {
                return NotFound();
            }

            return View(travelOffering);
        }

        // POST: TravelOfferings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TravelOfferings == null)
            {
                return Problem("Entity set 'TravelAgencyDbContext.TravelOfferings'  is null.");
            }
            var travelOffering = await _context.TravelOfferings.FindAsync(id);
            if (travelOffering != null)
            {
                _context.TravelOfferings.Remove(travelOffering);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TravelOfferingExists(int id)
        {
          return (_context.TravelOfferings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
