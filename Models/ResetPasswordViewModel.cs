using System.ComponentModel.DataAnnotations;

namespace Librelia.Models;

public class ResetPasswordViewModel
{
    [Required]
    public string Email { get; set; }

    [Required(ErrorMessage = "La nuova password è richiesta")]
    [DataType(DataType.Password)]
    [StringLength(100, ErrorMessage = "La {0} deve contenere almeno {2} caratteri.", MinimumLength = 8)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Conferma password")]
    [Compare("Password", ErrorMessage = "Le password non corrispondono.")]
    public string ConfirmPassword { get; set; }

    public string Token { get; set; } // Token per il reset

}