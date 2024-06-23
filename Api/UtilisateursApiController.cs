using LearnHubBackOffice.Models;
using LearnHubFO.Services;
using Microsoft.AspNetCore.Mvc;
using LearnHubFO.Models;

namespace LearnHubFO.Api
{
    [Route("api/utilisateurs/")]
    [ApiController]
    public class UtilisateursApiController : ControllerBase
    {
        private readonly UtilisateursService _utilisateursService;

        public UtilisateursApiController(UtilisateursService utilisateursService)
        {
            _utilisateursService = utilisateursService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] Utilisateur user)
        {
            if (ModelState.IsValid)
            {
                // Vérifiez si l'email existe déjà
                if (_utilisateursService.EmailExists(user.Email))
                {
                    return Conflict(new { message = "Email already exists" });
                }

                user.SetPassword(user.MotDePasseHash);
                _utilisateursService.Register(user);
                return Ok(new { message = "User registered successfully" });
            }
            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var user = _utilisateursService.GetUserByEmail(loginModel.Email);
                if (user != null && user.VerifyPassword(loginModel.Password))
                {
                    return Ok(new { message = "Login successful" });
                }

                return Unauthorized(new { message = "Invalid email or password" });
            }
            return BadRequest(ModelState);
        }
    }
}