﻿using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using TravelAgency.Models;
using TravelAgency.ViewModels;

namespace TravelAgency.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IEnumerable<TravelOfferingsViewModel> _offers = new List<TravelOfferingsViewModel>
        {
            new TravelOfferingsViewModel
            {
                Id = 1,
                Name = "France Adventure",
                Destination = "Paris, France",
                StartDate = new DateTime(2024, 07, 01),
                EndDate = new DateTime(2024, 07, 08),
                Price = 2500.00m,
                Description = "Enjoy a relaxing vacation in the beautiful Paris."
            },
            new TravelOfferingsViewModel
            {
                Id = 2,
                Name = "Italy Adventure",
                Destination = "Rome, Italy",
                StartDate = new DateTime(2024, 08, 01),
                EndDate = new DateTime(2024, 08, 15),
                Price = 3500.00m,
                Description = "Enjoy a relaxing vacation in the beautiful Rome."
            },
            new TravelOfferingsViewModel
            {
                Id = 3,
                Name = "Australia Adventure",
                Destination = "Sydney, Australia",
                StartDate = new DateTime(2024, 09, 01),
                EndDate = new DateTime(2024, 09, 12),
                Price = 5000.00m,
                Description = "Enjoy a relaxing vacation in the beautiful Sydney"
            }
        };

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(_offers);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Detail(int id)
        {
            var bike = _offers
                .FirstOrDefault(x => x.Id == id);
            return View(bike);
        }
        public IActionResult Print()
        {
            var printViewModel = new PrintViewModel();
            printViewModel.IsAccessBlocked = true;

            return View(printViewModel);
        }

        [HttpPost]
        public IActionResult CheckLicense(string LicenseKey)
        {
            var decryptedLicense = Decrypt(LicenseKey, "haslo");
            if(decryptedLicense == "TRAVEL")
            {
                var printViewModel = new PrintViewModel();
                printViewModel.IsAccessBlocked = false;
                return View("Print", printViewModel);
            }
            else
            {
                var printView = new PrintViewModel();
                return View("Print", printView);
            }
        }


        public static string Decrypt(string cipherText, string key)
        {
            StringBuilder decryptedText = new StringBuilder();
            int keyIndex = 0;
            key = key.ToUpper();

            foreach (char c in cipherText)
            {
                if (char.IsLetter(c))
                {
                    bool isUpperCase = char.IsUpper(c);
                    char offset = isUpperCase ? 'A' : 'a';

                    int keyShift = key[keyIndex] - 'A';
                    char decryptedChar = (char)((((c - offset) - keyShift + 26) % 26) + offset);

                    decryptedText.Append(decryptedChar);

                    keyIndex = (keyIndex + 1) % key.Length;
                }
                else
                {
                    decryptedText.Append(c);
                }
            }

            return decryptedText.ToString();
        }

    }
}