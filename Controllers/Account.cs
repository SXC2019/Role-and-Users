using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using System.Threading.Tasks;

namespace MVC.Controllers
{
    public class Account : Controller
    {
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;

        public Account(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task< IActionResult> Register(RegisterModel registerModel)
        {
            if(ModelState.IsValid)
            {
                var user = new IdentityUser()
                {
                    UserName = registerModel.Email,
                    Email = registerModel.Email
                };
                var result = await userManager.CreateAsync(user,registerModel.Password);
                if(result.Succeeded)
                {
                    ViewBag.Message = "User Created Successfully";
                    return View("Information");
                }
                else
                {
                    foreach(var err in result.Errors)
                    {
                        ModelState.AddModelError("",err.Description);
                    }
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if(ModelState.IsValid)
            {
                
                var result = await signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, false, false);

                if(result.Succeeded)
                {
                    return RedirectToAction("Admin", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid User Name Or Password");
                }
            }
            return View();
        }

        [HttpGet]

        [Authorize(Roles ="Super")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Admin()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
