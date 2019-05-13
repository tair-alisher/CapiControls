using CapiControls.BLL.DTO.Account;
using CapiControls.BLL.Exceptions;
using CapiControls.BLL.Interfaces;
using CapiControls.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CapiControls.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AccountController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
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
                var loginData = new LoginDTO
                {
                    Login = model.Login,
                    Password = model.Password,
                    Secret = _configuration.GetValue<string>("Secrets:Salt")
                };

                var user = _userService.GetUser(loginData);
                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim("Login", user.Login)
                    };

                    foreach (var role in user.Roles)
                        claims.Add(new Claim(role.Name, "true"));

                    var claimsIdentity = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme
                    );

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity)
                    );

                    return RedirectToAction("Index", "Control");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Проверьте введеные данные.");
                }
            }

            return View();
        }

        [Authorize]
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
                    var changePasswordData = new ChangePasswordDTO
                    {
                        OldPassword = model.OldPassword,
                        NewPassword = model.NewPassword,
                        Secret = _configuration.GetValue<string>("Secrets:Salt")
                    };
                    _userService.UpdatePassword(login, changePasswordData);

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