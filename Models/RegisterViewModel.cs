using System.ComponentModel.DataAnnotations;

namespace Librelia.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Inserisci il tuo nome")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Inserisci il tuo cognome")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Scegli un ruolo")]
        public string Role { get; set; }

        [Required(ErrorMessage = "Devi scegliere la tua posizione")]
        public string IsExternal { get; set; }

        [Required, EmailAddress(ErrorMessage = "Inserisci un indirizzo email valido")]
        public string Email { get; set; }

        [Required, MinLength(8, ErrorMessage = "La password deve contenere almeno 8 caratteri")]
        public string Password { get; set; }

        [Required, Compare("Password", ErrorMessage = "Le password non corrispondono")]
        public string ConfirmPassword { get; set; }
    }
}
