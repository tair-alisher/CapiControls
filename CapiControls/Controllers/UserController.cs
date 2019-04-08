using CapiControls.Models.Local.Account;
using CapiControls.Services.Interfaces;
using CapiControls.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CapiControls.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService UserService;

        public UserController(IUserService userService)
        {
            UserService = userService;
        }

        [Authorize(Policy = "IsAdministrator")]
        public IActionResult Index(int page = 1)
        {
            int pageSize = 10;
            int count = UserService.CountUsers();
            var users = UserService.GetUsers(pageSize, page);

            var pageViewModel = new PageViewModel(count, page, pageSize);
            var viewModel = new IndexViewModel<User>
            {
                PageViewModel = pageViewModel,
                Items = users
            };

            return View(viewModel);
        }

        [HttpGet]
        [Authorize(Policy = "IsAdministrator")]
        public IActionResult Register()
        {
            ViewBag.Roles = UserService.GetRoles();
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "IsAdministrator")]
        public IActionResult Register(RegisterViewModel user, Guid[] roles)
        {
            if (ModelState.IsValid)
            {
                if (!UserService.UserExists(user.Login))
                {
                    UserService.AddUser(user, roles);
                    return RedirectToAction("Index");
                } else
                {
                    ModelState.AddModelError("Login", "Пользователь с таким логином уже существует");
                }
            }

            ViewBag.Roles = UserService.GetRoles();

            return View();
        }
    }
}