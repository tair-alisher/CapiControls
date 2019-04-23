using CapiControls.BLL.DTO.Account;
using CapiControls.BLL.Interfaces;
using CapiControls.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace CapiControls.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [Authorize(Policy = "IsAdministrator")]
        public IActionResult Index(int page = 1)
        {
            int pageSize = 10;
            int count = _userService.CountUsers();
            var users = _userService.GetUsers(page, pageSize);

            var pageViewModel = new PageViewModel(count, page, pageSize);
            var listModel = new ListViewModel<UserDTO>
            {
                PageViewModel = pageViewModel,
                Items = users
            };

            return View(listModel);
        }

        [HttpGet]
        [Authorize(Policy = "IsAdministrator")]
        public IActionResult Register()
        {
            ViewBag.Roles = _userService.GetRoles();
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "IsAdministrator")]
        public IActionResult Register(RegisterViewModel user, Guid[] roles)
        {
            if (ModelState.IsValid)
            {
                if (!_userService.UserExists(user.Login))
                {
                    var userData = new RegisterDTO
                    {
                        Login = user.Login,
                        UserName = user.UserName,
                        Password = user.Password,
                        Secret = _configuration.GetValue<string>("Secrets:Salt")
                    };
                    _userService.AddUser(userData, roles);

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Login", "Пользователь с таким логином уже существует");
                }
            }

            ViewBag.Roles = _userService.GetRoles();

            return View();
        }

        [Authorize(Policy = "IsAdministrator")]
        public IActionResult Details(Guid id)
        {
            var user = _userService.GetUserById(id);

            return View(user);
        }

        [HttpGet]
        [Authorize(Policy = "IsAdministrator")]
        public IActionResult Edit(Guid id)
        {
            ViewBag.Roles = _userService.GetRoles();
            var user = _userService.GetUserById(id);

            return View(user);
        }

        [HttpPost]
        [Authorize(Policy = "IsAdministrator")]
        public IActionResult Edit(UserDTO user, Guid[] roles)
        {
            if (ModelState.IsValid)
            {
                _userService.UpdateUser(user, roles);
                return RedirectToAction("Index");
            }

            ViewBag.Roles = _userService.GetRoles();

            return View(user);
        }

        [Authorize(Policy = "IsAdministrator")]
        public IActionResult Delete(Guid id)
        {
            _userService.DeleteUser(id);

            return RedirectToAction("Index");
        }
    }
}