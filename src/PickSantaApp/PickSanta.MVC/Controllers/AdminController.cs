using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PickSanta.DB;
using PickSanta.DB.Entities;
using PickSanta.MVC.Models;

namespace PickSanta.MVC.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDataAccess _dataAccess;

        public AdminController(ILogger<HomeController> logger, IDataAccess dataAccess)
        {
            _logger = logger;
            _dataAccess = dataAccess;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddData()
        {
            var users = new List<User>();

            users.Add(new User("Nithin", "nibalach@microsoft.com", false));
            users.Add(new User("Parvez", "parvezah@microsoft.com", false));
            users.Add(new User("Ganesh", "ganeshk@microsoft.com", false));

            foreach (var user in users)
            {
                _dataAccess.CreateUser(user.Name, user.Email);
            }

            return View(true);
        }

        public IActionResult GetAllData()
        {
            var users = _dataAccess.GetAllUsers();
            var maps = _dataAccess.GetAllSantaMaps();

            var model = new GetAllDataModel()
            {
                Maps = maps,
                Users = users
            };

            ModelState.Clear();

            return View(model);
        }
    }
}