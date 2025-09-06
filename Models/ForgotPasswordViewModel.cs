using System.ComponentModel.DataAnnotations;

namespace Librelia.Models;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Email richiesta")]
    [EmailAddress(ErrorMessage = "Email non valida")]
    public string Email { get; set; }
}