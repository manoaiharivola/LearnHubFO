using Microsoft.AspNetCore.Mvc;
using LearnHubBackOffice.Models;
using LearnHubFO.Services;
using System;
using LearnHubFO.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace LearnHubFO.Controllers
{

    [Authorize]
    public class UtilisateursController : Controller
    {
        private readonly UtilisateursService _utilisateursService;

        public UtilisateursController(UtilisateursService utilisateursService)
        {
            _utilisateursService = utilisateursService;
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register(Utilisateur user)
        {
            if (ModelState.IsValid)
            {
                // Vérifiez si l'email existe déjà
                if (_utilisateursService.EmailExists(user.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    return View(user);
                }

                user.SetPassword(user.MotDePasseHash);
                try
                {
                    _utilisateursService.Register(user);
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while registering the user");
                }
            }

            foreach (var value in ModelState.Values)
            {
                foreach (var error in value.Errors)
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }
            }

            return View(user);
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var user = _utilisateursService.GetUserByEmail(loginModel.Email);
                if (user != null && user.VerifyPassword(loginModel.Password))
                {
                    // Créer des claims de sécurité
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.NomUtilisateur),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.IdUtilisateur.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Home");
                }

                TempData["ErrorMessage"] = "Les informations de connexion sont incorrectes.";
            }

            return View(loginModel);
        }


        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
