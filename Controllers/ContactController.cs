using Microsoft.AspNetCore.Mvc;

namespace Librelia.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Contacts()
        {
            return Redirect("./Pages/Contatti.cshtml");
        }
    }
}
