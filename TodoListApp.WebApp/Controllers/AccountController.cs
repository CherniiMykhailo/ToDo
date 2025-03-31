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
        if (signInManager.IsSignedIn(User))
        {
            ViewData["IsAuthenticated"] = true;
            ViewData["Username"] = User.Identity.Name;
        }
        else
        {
            ViewData["IsAuthenticated"] = false;
        }

        return View();
    }

    [Route("Login")]
    [AllowAnonymous]
    public ViewResult Login(string returnUrl = "/")
    {
        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [Route("Register")]
    [AllowAnonymous]
    public ViewResult Register(string returnUrl = "/")
    {
        return View(new RegisterViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [Route("Restore")]
    [AllowAnonymous]
    public ViewResult Restore(string returnUrl = "/")
    {
        return View(new RestoreViewModel
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
        if (ModelState.IsValid)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = registerViewModel.Name
            };

            var result = await userManager.CreateAsync(user, registerViewModel.Password);

            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(registerViewModel);
    }


    [HttpPost]
    [Route("Login")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        if (ModelState.IsValid)
        {
            IdentityUser user = await userManager.FindByNameAsync(loginViewModel.Name);

            if (user != null)
            {
                await signInManager.SignOutAsync();

                if ((await signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false)).Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid name or password.");
        }

        return View(loginViewModel);
    }

    [Route("Restore")]
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Restore(RestoreViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await userManager.FindByNameAsync(model.Name);
            if (user != null)
            {
                var result = await userManager.RemovePasswordAsync(user);
                if (result.Succeeded)
                {
                    result = await userManager.AddPasswordAsync(user, model.Password);
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Name not found");
                return View(model);
            }
        }
        else
        {
            ModelState.AddModelError("", "Somethink went wrong");
            return View(model);
        }
    }

    [Route("Logout")]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }
}
