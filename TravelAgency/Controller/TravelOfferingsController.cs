using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TravelAgency.Models;
using TravelAgency.Services;
using TravelAgency.ViewModels;

namespace TravelAgency.Controllers
{
    public class TravelOfferingsController : Controller
    {
        private readonly ITravelAgencyService _travelAgencyService;
        private readonly IMapper _mapper;

        public TravelOfferingsController(ITravelAgencyService travelOfferingsService, IMapper mapper)
        {
            _travelAgencyService = travelOfferingsService;
            _mapper = mapper;
        }

        // GET: TravelOfferings
        public async Task<IActionResult> Index()
        {
            var travelOfferings = await _travelAgencyService.GetAllAsync();
            var travelOfferingViewModel = _mapper.Map<IEnumerable<TravelOfferingViewModel>>(travelOfferings);
            return View(travelOfferingViewModel);
        }

        // GET: TravelOfferings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var travelOffering = await _travelAgencyService.GetByIdAsync(id.Value);
            if (travelOffering == null)
            {
                return NotFound();
            }
            var travelOfferingViewModel = _mapper.Map<TravelOfferingViewModel>(travelOffering);

            return View(travelOfferingViewModel);
        }

        // GET: TravelOfferings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TravelOfferings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Destination,StartDate,EndDate,Price,Description")] TravelOfferingViewModel travelOfferingViewModel)
        {
            if (ModelState.IsValid)
            {
                TravelOffering travelOffering = _mapper.Map<TravelOffering>(travelOfferingViewModel);

                await _travelAgencyService.AddAsync(travelOffering);
                return RedirectToAction(nameof(Index));
            }
            return View(travelOfferingViewModel);
        }

        // GET: TravelOfferings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var travelOffering = await _travelAgencyService.GetByIdAsync(id.Value);
            if (travelOffering == null)
            {
                return NotFound();
            }
            var travelOfferingViewModel = _mapper.Map<TravelOfferingViewModel>(travelOffering);
            return View(travelOfferingViewModel);
        }

        // POST: TravelOfferings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Destination,StartDate,EndDate,Price,Description")] TravelOfferingViewModel travelOfferingViewModel)
        {
            if (id != travelOfferingViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                TravelOffering travelOffering = _mapper.Map<TravelOffering>(travelOfferingViewModel);

                try
                {
                    await _travelAgencyService.UpdateAsync(travelOffering);
                }
                catch
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(travelOfferingViewModel);
        }

        // GET: TravelOfferings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var travelOffering = await _travelAgencyService.GetByIdAsync(id.Value);
            if (travelOffering == null)
            {
                return NotFound();
            }

            var travelOfferingViewModel = _mapper.Map<TravelOfferingViewModel>(travelOffering);

            return View(travelOfferingViewModel);
        }

        // POST: TravelOfferings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _travelAgencyService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: TravelOfferings/Search
        public async Task<IActionResult> Search(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return RedirectToAction(nameof(Index));
            }

            var searchResults = await _travelAgencyService.SearchAsync(searchString);
            var searchResultViewModels = _mapper.Map<IEnumerable<TravelOfferingViewModel>>(searchResults);
            return View("Index", searchResultViewModels);
        }

        public async Task<IActionResult> Sort(string sortOrder)
        {
            var sortedResults = await _travelAgencyService.SortAsync(sortOrder);
            var sortedResultViewModels = _mapper.Map<IEnumerable<TravelOfferingViewModel>>(sortedResults);
            return View("Index", sortedResultViewModels);
        }

    }
}
