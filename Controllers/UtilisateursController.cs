using Microsoft.AspNetCore.Mvc;
using LearnHubBackOffice.Models;
using LearnHubFO.Services;
using System;

namespace LearnHubFO.Controllers
{
    public class UtilisateursController : Controller
    {
        private readonly UtilisateursService _utilisateursService;

        public UtilisateursController(UtilisateursService utilisateursService)
        {
            _utilisateursService = utilisateursService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

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
    }
}
