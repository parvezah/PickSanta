using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PickSanta.DB;
using PickSanta.MVC.Models;

namespace PickSanta.MVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDataAccess _dataAccess;

        public HomeController(ILogger<HomeController> logger, IDataAccess dataAccess)
        {
            _logger = logger;
            _dataAccess = dataAccess;
        }

        public IActionResult Index()
        {
            var model = new HomeViewModel();
            if (this.User.Identity.IsAuthenticated)
            {
                var email = this.User.Identity.Name;

                var user = _dataAccess.GetUser(email);
                var santaMap = _dataAccess.GetGifteeForSanta(email);
                model.User = user;
                model.Map = santaMap;
            }
            ModelState.Clear();
            return View(model);
        }

        public IActionResult Rules()
        {
            return View();
        }

        public IActionResult Roll()
        {
            string result = "Failed";
            if (this.User.Identity.IsAuthenticated)
            {
                var email = this.User.Identity.Name;

                var user = _dataAccess.GetUser(email);
                var santaMap = _dataAccess.GetGifteeForSanta(email);

                if (santaMap == null)
                {
                    _dataAccess.CreateSantaMap("parvezah@microsoft.com", user.Email);
                    santaMap = _dataAccess.GetGifteeForSanta(email);
                    
                }
                result = santaMap.Giftee;
            }
            ModelState.Clear();
            return View("Roll", result);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
