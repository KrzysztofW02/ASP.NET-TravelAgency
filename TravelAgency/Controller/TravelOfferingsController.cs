using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelAgency.Models;
using TravelAgency.Services;
using TravelAgency.ViewModels;

namespace TravelAgency.Controllers
{
    [Authorize]
    public class TravelOfferingsController : Controller
    {
        private readonly ITravelAgencyService _travelAgencyService;
        private readonly IMapper _mapper;
        private readonly IValidator<TravelOffering> _validator;

        public TravelOfferingsController(ITravelAgencyService travelOfferingsService, IMapper mapper, IValidator<TravelOffering> validator)
        {
            _travelAgencyService = travelOfferingsService;
            _mapper = mapper;
            _validator = validator;
        }

        // GET: TravelOfferings
        public async Task<IActionResult> Index()
        {
            var travelOfferings = await _travelAgencyService.GetAllAsync();
            var travelOfferingViewModels = _mapper.Map<IEnumerable<TravelOfferingViewModel>>(travelOfferings);
            return View(travelOfferingViewModels);
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
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Destination,StartDate,EndDate,Price,Description")] TravelOfferingViewModel travelOfferingViewModel)
        {
            if (ModelState.IsValid)
            {
                TravelOffering travelOffering = _mapper.Map<TravelOffering>(travelOfferingViewModel);

                var validationResult = await _validator.ValidateAsync(travelOffering);

                if (validationResult.IsValid)
                {
                    await _travelAgencyService.AddAsync(travelOffering);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in validationResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.ErrorMessage);
                    }
                }
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
        [Authorize(Roles = "Manager")]
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

                var validationResult = await _validator.ValidateAsync(travelOffering);

                if (validationResult.IsValid)
                {
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
                else
                {
                    foreach (var error in validationResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.ErrorMessage);
                    }
                }
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
        [Authorize(Roles = "Administrator")]
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
