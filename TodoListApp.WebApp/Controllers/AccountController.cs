using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TodoListApp.WebApp.Controllers;
[Authorize]
[Route("Account")]
public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly SignInManager<IdentityUser> signInManager;

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    public async Task<IActionResult> Index()
    {
        if (this.signInManager.IsSignedIn(this.User))
        {
            this.ViewData["IsAuthenticated"] = true;
            this.ViewData["Username"] = User.Identity.Name;
        }
        else
        {
            this.ViewData["IsAuthenticated"] = false;
        }

        return this.View();
    }

    [Route("Login")]
    [AllowAnonymous]
    public ViewResult Login(string returnUrl = "/")
    {
        return this.View(new LoginViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [Route("Register")]
    [AllowAnonymous]
    public ViewResult Register(string returnUrl = "/")
    {
        return this.View(new RegisterViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [Route("Restore")]
    [AllowAnonymous]
    public ViewResult Restore(string returnUrl = "/")
    {
        return this.View(new RestoreViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [HttpPost]
    [Route("Register")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {
        if (this.ModelState.IsValid)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = registerViewModel.Name
            };

            var result = await this.userManager.CreateAsync(user, registerViewModel.Password);

            if (result.Succeeded)
            {
                await this.signInManager.SignInAsync(user, isPersistent: false);

                return this.RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return this.View(registerViewModel);
    }

    [HttpPost]
    [Route("Login")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        if (this.ModelState.IsValid)
        {
            IdentityUser user = await this.userManager.FindByNameAsync(loginViewModel.Name);

            if (user != null)
            {
                await this.signInManager.SignOutAsync();

                if ((await this.signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false)).Succeeded)
                {
                    return this.RedirectToAction("Index", "Home");
                }
            }

            this.ModelState.AddModelError(string.Empty, "Invalid name or password.");
        }

        return this.View(loginViewModel);
    }

    [Route("Restore")]
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Restore(RestoreViewModel model)
    {
        if (this.ModelState.IsValid)
        {
            var user = await this.userManager.FindByNameAsync(model.Name);
            if (user != null)
            {
                var result = await this.userManager.RemovePasswordAsync(user);
                if (result.Succeeded)
                {
                    _ = await this.userManager.AddPasswordAsync(user, model.Password);
                    return this.RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach(var error in result.Errors)
                    {
                        this.ModelState.AddModelError("", error.Description);
                    }

                    return this.View(model);
                }
            }
            else
            {
                this.ModelState.AddModelError("", "Name not found");
                return this.View(model);
            }
        }
        else
        {
            this.ModelState.AddModelError("", "Somethink went wrong");
            return this.View(model);
        }
    }

    [Route("Logout")]
    public async Task<IActionResult> Logout()
    {
        await this.signInManager.SignOutAsync();
        return this.RedirectToAction("Login", "Account");
    }
}
