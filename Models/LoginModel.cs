using System.ComponentModel.DataAnnotations;

namespace LearnHubFO.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "L'adresse email est requise")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}