using System.Security.Claims;
using Librelia.DTO;
using Librelia.Models;
using Librelia.Repositories;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Librelia.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Librelia.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserRepository _userRepo;
        private readonly ResetTokensRepository _resetTokenRepo;
        private readonly AuthService _authService;
        private readonly EmailService _emailService;

        public AuthController(IHttpContextAccessor httpContextAccessor,UserRepository userRepo, ResetTokensRepository resetTokensRepository, EmailService emailService)
        {
            _authService = new AuthService(httpContextAccessor);
            _userRepo = userRepo;
            _resetTokenRepo = resetTokensRepository;
            _emailService = emailService;
        }

        public IActionResult Login()
        {
            if (_authService.IsAuthenticated()) return RedirectToAction("Index", "Home");
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AuthUserDTO model)
        {
            if (!ModelState.IsValid) return View(model);

            var user =  await _userRepo.GetByEmail(model.Email ?? "");
          
            if (user == null)
            {
                ViewData["ErrorEmail"] = "Utente con questa email non trovato";
                return View(model);
            }

            if (user.Status == "not-verified")
            {
                TempData["Notification"] = new Dictionary<string, string>
                {
                    { "Title", "Account Non Verificato" },
                    { "Message", $"Aspetta che un admin accetti la tua verifica, ricevarai una mail nella posta!"},
                    { "Type", "information" }
                };
                return View(model);
            }
            
            if (user.Status == "banned")
            {
                TempData["Notification"] = new Dictionary<string, string>
                {
                    { "Title", "Account Bannato" },
                    { "Message", $"Il tuo account é stato bannato, dirigiti in dagli amministatori se sapere i motivi o provare a recovare il ban..."},
                    { "Type", "error" }
                };
                return View(model);
            }
            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                ViewData["ErrorPassword"] = "La password non corrisponde!";
                return View(model);
            }
            
            
            
            
            // Creiamo i claims per l'utente
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.GivenName, user.Name),
                new Claim(ClaimTypes.Surname, user.Surname),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("isExternal", user.External.ToString().ToLower()),
                new Claim("status", user.Status.ToString().ToLower())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe // Ricorda l'utente se ha spuntato "Ricordami"
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            
            
            
            TempData["Notification"] = new Dictionary<string, string>
            {
                { "Title", "Login" },
                { "Message", $"Accesso come {user.Name + " " + user.Surname} effettuato con successo!"},
                { "Type", "success" }
            };
            return RedirectToAction("Index", "Home");

         //    return View(model);
        }


        [HttpGet]
        public ActionResult Registrazione()
        {
            if (_authService.IsAuthenticated()) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrazione(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userRegister = model.Adapt<User>();
            userRegister.External = model.IsExternal == "true" ? true : false;
            userRegister.Password = BCrypt.Net.BCrypt.HashPassword(userRegister.Password);  // Hasha la password utente
            
            var isAlreadyRegistered = await _userRepo.GetByEmail(userRegister.Email);
            if (isAlreadyRegistered != null)
            {
                ViewData["Error"] = "Tentativo di registrazione invalido email già registrata.";
                return View(model);
            }


            
            await _userRepo.AddUser(userRegister);
            
            TempData["Notification"] = new Dictionary<string, string>
            {
                { "Title", "Registrazione" },
                { "Message", "Registrazione effetutata con successo riceverai una mail di accettazione quando la tua richiesta verrà approvata da un admin." },
                { "Type", "success" }
            };
            
            
            var bodyEmail = $@"  <!DOCTYPE html>
                    <html lang=""it"">
                    <head>
                        <meta charset=""UTF-8"">
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        <title>Verifica effettuata con successo!</title>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                                background-color: #f4f4f4;
                                margin: 0;
                                padding: 20px;
                            }}
                            .container {{
                                max-width: 600px;
                                margin: auto;
                                background: #ffffff;
                                padding: 20px;
                                border-radius: 5px;
                                box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                            }}
                            h1 {{
                                color: #2d7b2d;
                            }}
                            p {{
                                color: #555;
                            }}
                            .footer {{
                                margin-top: 20px;
                                font-size: 12px;
                                color: #777;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class=""container"">
                            <h1>Registrazione Effettuata</h1>
                            <p>Ciao {userRegister.Name + " " + userRegister.Surname},</p>
                            <p>Siamo lieti di informarti che la tua registrazione é andata a buon fine,</p>
                            <p>un admin si occuperá di verificarti,</p>
                            <p>riceverai una mail di conferma nel caso di positivtá dell'esito</p>
                            <div class=""footer"">
                              <p>Grazie per aver scelto Librelia! Buona lettura.</p>
                            </div>
                        </div>
                    </body>
                    </html>
                    ";
        await _emailService.SendMailAsync(userRegister.Email, "Registrazione Effettuata", bodyEmail);
            return RedirectToAction("Index", "Home");
        }


        // This method can be used to handle logging out the user.
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home"); // Dopo il logout, reindirizza alla homepage
        }
        
        
        
        [HttpGet]
        public IActionResult PasswordDimenticata()
        {
            if (_authService.IsAuthenticated()) return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordDimenticata(ForgotPasswordViewModel model)
        {
            TempData["Notification"] = new Dictionary<string, string>
            {
                { "Title", "Richiesta Effettuata!" },
                { "Message", "Se l'email esiste riceverai una mail nella tua posta per resettare la password" },
                { "Type", "success" }
            };
            
            
            var user = await _userRepo.GetByEmail(model.Email);
            if (user == null)
                return Ok();

            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            var resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                Email = user.Email,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            await _resetTokenRepo.Add(resetToken);

            var resetLink = Url.Action("ResetPassword", "Auth", new
            {
                token = token,
                email = user.Email
            }, protocol: HttpContext.Request.Scheme);

             var bodyEmail = $@"  <!DOCTYPE html>
                    <html lang=""it"">
                    <head>
                        <meta charset=""UTF-8"">
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        <title>Resetta la password</title>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                                background-color: #f4f4f4;
                                margin: 0;
                                padding: 20px;
                            }}
                            .container {{
                                max-width: 600px;
                                margin: auto;
                                background: #ffffff;
                                padding: 20px;
                                border-radius: 5px;
                                box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                            }}
                            h1 {{
                                color: black;
                            }}
                            p {{
                                color: #555;
                            }}
                            .footer {{
                                margin-top: 20px;
                                font-size: 12px;
                                color: #777;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class=""container"">
                            <h1>Resetta la password</h1>
                            <p>Ciao {user.Name + " " + user.Surname},</p>
                            <p>ci é arrivata una richiesta di reset della password,</p>
                            <p>se non sei stato tu ignora l'email altrimenti clicca sul <a href=""{resetLink}"">link</a></p>
                            <div class=""footer"">
                              <p>Grazie per aver scelto Librelia! Buona lettura.</p>
                            </div>
                        </div>
                    </body>
                    </html>
                    ";
            await _emailService.SendMailAsync(user.Email, "Reset Password", bodyEmail);

            return View();
        }
        
        
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            if (_authService.IsAuthenticated()) return RedirectToAction("Index", "Home");

            var resetToken = await _resetTokenRepo.GetByEmailAndToken(email, token);

            if (resetToken == null)
            {
                
                TempData["Notification"] = new Dictionary<string, string>
                {
                    { "Title", "Errore!" },
                    { "Message", "Token Scaduto o non valido!" },
                    { "Type", "error" }
                };
                return View();
            }
                

            return View(new ResetPasswordViewModel { Email = email, Token = token });
        }
        
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var resetToken = await _resetTokenRepo.GetByEmailAndToken(model.Email, model.Token);

            if (resetToken == null)
            {
                ModelState.AddModelError("", "Token non valido o scaduto.");
                return View(model);
            }

            var user = await _userRepo.GetById(resetToken.UserId);
            if (user == null)
            {
                ModelState.AddModelError("", "Utente non trovato.");
                return View(model);
            }

            // Aggiorna la password
            user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
            await _userRepo.UpdateUser(user);

            // Marca il token come usato
            resetToken.Used = true;
            await _resetTokenRepo.Update(resetToken);

            
            TempData["Notification"] = new Dictionary<string, string>
            {
                { "Title", "Operazione Effettuata!" },
                { "Message", "Password cambiata con successo!" },
                { "Type", "success" }
            };
            return RedirectToAction("Login");
        }

    }
}

