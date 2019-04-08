using CapiControls.Exceptions;
using CapiControls.Services.Interfaces;
using CapiControls.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CapiControls.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService UserService;

        public AccountController(IUserService userService)
        {
            UserService = userService;
        }

        [HttpGet("account/login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserService.GetUser(model);
                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim("Login", user.Login)
                    };

                    foreach (var role in user.Roles)
                    {
                        claims.Add(new Claim(role.Name, "true"));
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity)
                    );

                    return RedirectToAction("Index", "Control");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Проверье введеные данные.");
                }
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                string login = HttpContext.User.FindFirst("Login").Value;
                try
                {
                    UserService.UpdatePassword(login, model);
                    return RedirectToAction("Index", "Control");
                }
                catch (WrongOldPasswordException)
                {
                    ModelState.AddModelError(string.Empty, "Старый пароль неверен.");
                }
            }

            return View(model);
        }
    }
}