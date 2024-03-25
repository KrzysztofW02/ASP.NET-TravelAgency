using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TravelAgency.Models;
using TravelAgency.Repository;

namespace TravelAgency.Controllers
{
    public class TravelOfferingsController : Controller
    {
        private readonly ITravelOfferingsRepository _travelOfferingsRepository;

        public TravelOfferingsController(ITravelOfferingsRepository travelOfferingsRepository)
        {
            _travelOfferingsRepository = travelOfferingsRepository;
        }

        // GET: TravelOfferings
        public async Task<IActionResult> Index()
        {
            var travelOfferings = await _travelOfferingsRepository.GetAllAsync();
            return View(travelOfferings);
        }

        // GET: TravelOfferings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var travelOffering = await _travelOfferingsRepository.GetByIdAsync(id.Value);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Destination,StartDate,EndDate,Price,Description")] TravelOffering travelOffering)
        {
            if (ModelState.IsValid)
            {
                await _travelOfferingsRepository.AddAsync(travelOffering);
                return RedirectToAction(nameof(Index));
            }
            return View(travelOffering);
        }

        // GET: TravelOfferings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var travelOffering = await _travelOfferingsRepository.GetByIdAsync(id.Value);
            if (travelOffering == null)
            {
                return NotFound();
            }
            return View(travelOffering);
        }

        // POST: TravelOfferings/Edit/5
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
                    await _travelOfferingsRepository.UpdateAsync(travelOffering);
                }
                catch
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(travelOffering);
        }

        // GET: TravelOfferings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var travelOffering = await _travelOfferingsRepository.GetByIdAsync(id.Value);
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
            await _travelOfferingsRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
