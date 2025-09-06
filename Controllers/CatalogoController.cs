using DocumentFormat.OpenXml.Wordprocessing;
using Librelia.Database;
using Librelia.DTO;
using Librelia.Models;
using Librelia.Repositories;
using Librelia.Services;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using System;

namespace Librelia.Controllers
{
    public class CatalogoController : Controller
    {
        private readonly BookRepository _bookRepo;
        private readonly ReservationRepository _reservationRepo;
        private readonly AuthService _authService;
        private readonly EmailService _emailService;
        public CatalogoController(BookRepository repositoryBooks, ReservationRepository reservationRepo, AuthService authService, EmailService emailService)
        {
            _bookRepo = repositoryBooks;
            _reservationRepo = reservationRepo;
            _authService = authService;
            _emailService = emailService;
        }

        public async Task<IActionResult> Catalogo(string searching, string category, string availability, string sort, int page)
        {
            var filters = new BookFilters()
            {
                Searching = searching,
                Category = category,
                Availability = availability,
                SortBy = sort?.Split('-')[0] ?? "title",
                SortDirection = sort?.Split('-')[1] ?? "asc"
            };
            var books = await _bookRepo.SearchBooks(filters);

            if(books != null){
                var filteredBooks = books
                    .GroupBy(b => b.Isbn)
                    .Select(g =>
                        g.FirstOrDefault(book => book.Reserved == false) ?? g.First()
                    )
                    .ToList();  
            
                page = page < 1 ? 1 : page;
                var pagination =  new PaginationModel(page, (int)Math.Ceiling((double)filteredBooks.Count / StaticValues.itemsForPage))
                                     {
                                         Searching = searching,
                                         Category = category,
                                         Availability = availability,
                                         Sort = sort
                                     };
          

                var catalogViewModel = new CatalogViewModel()
                {
                    Books = filteredBooks.Any() ? filteredBooks.Skip((page - 1) * StaticValues.itemsForPage).Take(StaticValues.itemsForPage).ToList() : null,
                    Filters = filters != null ? filters : null,
                    Pagination = pagination,
                    Error = filteredBooks.Any() ? null : "Libri non trovati",
                };
                return View(catalogViewModel);
            }
            else
            {
                var pagination =  new PaginationModel(page, (int)Math.Ceiling((double)0 / StaticValues.itemsForPage))
                {
                    Searching = searching,
                    Category = category,
                    Availability = availability,
                    Sort = sort
                };
                var catalogViewModel = new CatalogViewModel()
                {
                    Books =  null,
                    Filters = filters != null ? filters : null,
                    Pagination = pagination,
                    Error = "Libri non trovati",
                };
                return View(catalogViewModel);
            }
            
        }


        public async Task<IActionResult> Libro(string id)
        {
            var book = await _bookRepo.GetById(id);

            if (book == null) return NotFound();
            
            return View(book);
        }

        public async Task<IActionResult> Prenota(string id, string returnUrl)
        {

            if (!_authService.IsAuthenticated())
            {
                TempData["Notification"] = new Dictionary<string, string>
                {
                    { "Title", "Account necessario!" },
                    { "Message", $"Non é possibile prenotare un libro se non sei registrato"},
                    { "Type", "error" }
                };
                return Redirect(returnUrl);
            }

            var user = _authService.GetCurrentUser();
            var totalReservation =
                await _reservationRepo.GetByEmailOrBookId(user.Email, null, "prenotato");
            totalReservation.AddRange(await _reservationRepo.GetByEmailOrBookId(user.Email, null, "scaduto"));

            if (totalReservation.Count == StaticValues.bookForUser)
            {
                TempData["Notification"] = new Dictionary<string, string>
                {
                    { "Title", "Prenotazione Fallita!" },
                    { "Message", $"Non é possibile prenotare piú di {StaticValues.bookForUser} per utente"},
                    { "Type", "error" }
                };
                return Redirect(returnUrl);
            }
            var book = await _bookRepo.GetBookForPrenotation(id);
            if (book == null)
            {
                TempData["Notification"] = new Dictionary<string, string>
                {
                    { "Title", "Prenotazione Fallita!" },
                    { "Message", $"Non é stato possibile procedere poiché questo libro non é disponibile"},
                    { "Type", "error" }
                };
                return Redirect(returnUrl);
            }
            if (book.Reserved)
            {
                TempData["Notification"] = new Dictionary<string, string>
                {
                    { "Title", "Prenotazione Fallita!" },
                    { "Message", $"Non é stato possibile procedere poiché qualcuno ha appena prenotato il libro"},
                    { "Type", "error" }
                };
                return Redirect(returnUrl);
            }
            
            var reservation = new Reservation()
            {
                Email = user.Email,
                Register_Date = DateTime.Now,
                Expire_Date = DateTime.Now.AddDays(30),
                BookId = book.Id,
                Status = "prenotato"
            };

            await _reservationRepo.AddReservation(reservation);
            
            book.Reserved = true;

            await _bookRepo.UpdateBook(book);
            
            var urlPrenotation = Url.Action(
                            action: "Prenotazioni",
                            controller: "Profilo",
                            values: null,
                            protocol: Request.Scheme);


            TempData["Notification"] = new Dictionary<string, string>
            {
                { "Title", "Prenotazione effetuata!" },
                { "Message", $"Puoi andare a ritirare il tuo libro"},
                { "Type", "success" }
            };
            var bodyEmail = $@"  <!DOCTYPE html>
                    <html lang=""it"">
                    <head>
                        <meta charset=""UTF-8"">
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        <title>Prenotazione effettuata con successo!</title>
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
                            <h1>Conferma Prenotazione</h1>
                            <p>Ciao {user.Name + " " + user.Surname},</p>
                            <p>Siamo lieti di informarti che la tua prenotazione per il libro: <strong>{book.Title}</strong> (ISBN: {book.Isbn})</p>
                            <p>di: {string.Join(",",  book.Authors)}</p>
                            <p>Edito da: {book.House},</p>
                            <p>è stata effettuata con successo.</p>
                            <p>Ti ricordiamo che puoi ritirarlo presso la nostra biblioteca entro la data stabilita <a href=""{urlPrenotation}"">qui</a>.</p>
                            <div class=""footer"">
                              <p>Grazie per aver scelto Librelia! Buona lettura.</p>
                            </div>
                        </div>
                    </body>
                    </html>
                    ";
            await _emailService.SendMailAsync(user.Email, "Prenotazione Effettuata", bodyEmail);
            return Redirect(returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> GetBookData(string id)
        {
            var book = await _bookRepo.GetById(id);

            if (book == null) return NotFound();

            return Json(book);
        }

        [HttpGet]
        public async Task<IActionResult> SearchBooks(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Json(new List<object>());

            var filter = new BookFilters()
            {
                Searching = searchTerm
            };
            var books = await _bookRepo.SearchBooks(filter);

            if (books == null) return NotFound();

            return Json(books);
        }
    }
}
