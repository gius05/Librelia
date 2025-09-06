using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Librelia.Database;
using Librelia.DTO;
using Librelia.Models;
using Librelia.Repositories;
using Librelia.Services;
using Librelia.Utility;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Librelia.Controllers;

public class AdminController : Controller
{
    private readonly UserRepository _userRepository;
    private readonly BookRepository _bookRepository;
    private readonly ReservationRepository _reservationRepository;
    private readonly AuthService _authService;
    private readonly EmailService _emailService;

    public AdminController(UserRepository userRepository, BookRepository bookRepository,
        ReservationRepository reservationRepository, AuthService authService, EmailService emailService)
    {
        _userRepository = userRepository;
        _bookRepository = bookRepository;
        _reservationRepository = reservationRepository;
        _authService = authService;
        _emailService = emailService;
    }

    public async Task<IActionResult> Index()
    {
        if (!_authService.IsAuthenticated() || _authService.GetCurrentUser().Role != "admin") return RedirectToAction("Index", "Home");

        ViewBag.UsersCount = await _userRepository.GetCount();
        ViewBag.BooksCount = await _bookRepository.GetCount();
        ViewBag.ResCount = await _reservationRepository.GetCount();
        
        return View("Dashboard/Index");
    }

    
    public async Task<IActionResult> TabellaUtenti(string searching, int page)
    {
        if (!_authService.IsAuthenticated() || _authService.GetCurrentUser().Role != "admin") return RedirectToAction("Index", "Home");

        
        var userFilter = new UserFilter()
        {
            Searching = searching,
            Status = null
        };
        var users = await _userRepository.SearchUsers(userFilter);
        
        
        page = page < 1 ? 1 : page;
        var pagination =  new PaginationModel(page, (int)Math.Ceiling((double)users.Count / StaticValues.itemsForPage))
        {
            Searching = searching,
        };

        
        
        var dataTableModel = new DataTableViewModel()
        {
            Users = users.Any() ? users.Adapt<List<UserDTO>>().Skip((page - 1) * StaticValues.itemsForPage).Take(StaticValues.itemsForPage).ToList() : null,
            Pagination = pagination,
            TotalItems = users.Count
            
        };
        return View("Dashboard/Utenti/Tabella", dataTableModel);
    }

    public async Task<IActionResult> TabellaNonVerificati(string searching, int page)
    {
        if (!_authService.IsAuthenticated() || _authService.GetCurrentUser().Role != "admin") return RedirectToAction("Index", "Home");

        var userFilter = new UserFilter()
        {
            Searching = searching,
            Status = "not-verified"
        };
        var users = await _userRepository.SearchUsers(userFilter);


        page = page < 1 ? 1 : page;
        var pagination = new PaginationModel(page, (int)Math.Ceiling((double)users.Count / StaticValues.itemsForPage))
        {
            Searching = searching,
        };



        var dataTableModel = new DataTableViewModel()
        {
            Users = users.Any() ? users.Adapt<List<UserDTO>>().Skip((page - 1) * StaticValues.itemsForPage).Take(StaticValues.itemsForPage).ToList() : null,
            Pagination = pagination,
            TotalItems = users.Count

        };
        return View("Dashboard/NonVerificati/Tabella", dataTableModel);
    }

    [HttpGet]
    public async Task<IActionResult> ModificaUtente(string id)
    {
        if (!_authService.IsAuthenticated() || _authService.GetCurrentUser().Role != "admin") return RedirectToAction("Index", "Home");

        if (id == null) return NotFound();
        
        var user = await _userRepository.GetById(id);

        if (user == null) return NotFound();
        
        return View("Dashboard/Utenti/Modifica", user.Adapt<UserDTO>());
    }
    
    [HttpPost]
    public async Task<IActionResult> ModificaUtente(UserDTO userModify)
    {
        if (!_authService.IsAuthenticated() || _authService.GetCurrentUser().Role != "admin") return RedirectToAction("Index", "Home");

        if (!ModelState.IsValid)
        {
            TempData["Notification"] = new Dictionary<string, string>
            {
                { "Title", "Errore!" },
                { "Message", $"Tutti i campi devono essere riempiti!"},
                { "Type", "error" }
            };

            return View("Dashboard/Utenti/Modifica", userModify);
        }

        var checkUser = await _userRepository.GetByEmail(userModify.Email);
        if (checkUser != null && checkUser.Id != userModify.Id)
        {
            TempData["Notification"] = new Dictionary<string, string>
            {
                { "Title", "Email giá presente!" },
                { "Message", $"Questa email é giá associata ad un utente diverso!"},
                { "Type", "error" }
            };

            return View("Dashboard/Utenti/Modifica", userModify);
        }
        
        
        var user = await _userRepository.GetById(userModify.Id);
        
        user.Status = user.Status != userModify.Status ? userModify.Status : user.Status;

        user.Name = !string.IsNullOrEmpty(userModify.Name) && !string.IsNullOrWhiteSpace(userModify.Name)
            ? userModify.Name
            : user.Name;
        
        user.Surname = !string.IsNullOrEmpty(userModify.Surname) && !string.IsNullOrWhiteSpace(userModify.Surname)
            ? userModify.Surname
            : user.Surname;
        
        user.Email = !string.IsNullOrEmpty(userModify.Email) && !string.IsNullOrWhiteSpace(userModify.Email)
            ? userModify.Email
            : user.Email;
        
        user.Role = !string.IsNullOrEmpty(userModify.Role) && !string.IsNullOrWhiteSpace(userModify.Role)
            ? userModify.Role
            : user.Role;
        
        user.External = user.External != userModify.External ? userModify.External : user.External;

        await _userRepository.UpdateUser(user);
        
        
        TempData["Notification"] = new Dictionary<string, string>
        {
            { "Title", "Modifica Effettuata!" },
            { "Message", $"Le modifiche sono state salvate con successo!"},
            { "Type", "success" }
        };
        
        return RedirectToAction("TabellaUtenti");
    }
    
    [HttpGet]
    public async Task<IActionResult> EliminaUtente(string id, bool verified = false)
    {
        if (!_authService.IsAuthenticated() || _authService.GetCurrentUser().Role != "admin") return RedirectToAction("Index", "Home");

        if (id == null)
        {
            TempData["Notification"] = new Dictionary<string, string>
            {
                { "Title", "Errore!" },
                { "Message", $"Utente con questo id non trovato!"},
                { "Type", "error" }
            };
            if(verified) return RedirectToAction("TabellaUtenti");

            RedirectToAction("TabellaNonVerificati");
        }
       
        await _userRepository.RemoveUser(id);
        
        
        TempData["Notification"] = new Dictionary<string, string>
        {
            { "Title", "Eliminazione Effettuata!" },
            { "Message", $"Utente eliminato con successo!"},
            { "Type", "success" }
        };

        if (verified) return RedirectToAction("TabellaUtenti");

        return RedirectToAction("TabellaNonVerificati");

    }

    [HttpGet]
    public async Task<IActionResult> VerificaUtente(string id)
    {
        if (!_authService.IsAuthenticated() || _authService.GetCurrentUser().Role != "admin") return RedirectToAction("Index", "Home");

        if (id == null)
        {
            TempData["Notification"] = new Dictionary<string, string>
            {
                { "Title", "Errore!" },
                { "Message", $"Utente con questo id non trovato!"},
                { "Type", "error" }
            };
            return RedirectToAction("TabellaNonVerificati");
        }

        await _userRepository.VerifyUser(id);
        var user = await _userRepository.GetById(id);

        TempData["Notification"] = new Dictionary<string, string>
        {
            { "Title", "Verifica Effettuata!" },
            { "Message", $"Utente verificato con successo!"},
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
                            <h1>Verifica Effettuata</h1>
                            <p>Ciao {user.Name + " " + user.Surname},</p>
                            <p>Siamo lieti di informarti che la tua verifica é andata a buon fine,</p>
                            <p>ora potrai usufruire dei servizi della libreria.</p>
                            <div class=""footer"">
                              <p>Grazie per aver scelto Librelia! Buona lettura.</p>
                            </div>
                        </div>
                    </body>
                    </html>
                    ";
        await _emailService.SendMailAsync(user.Email, "Verifica Effettuata", bodyEmail);

        return RedirectToAction("TabellaNonVerificati");

    }


    /* Prenotazioni */

    public async Task<IActionResult> TabellaPrenotazioni(string searching, string status, string startDate, string endDate, int page)
    {
        if (!_authService.IsAuthenticated() || _authService.GetCurrentUser().Role != "admin") return RedirectToAction("Index", "Home");

        var reservationFilters = new ReservationFilters()
        {
            Searching = searching,
            Status = status,
            StartDate = startDate,
            EndDate = endDate,
        };
        var reservations = await _reservationRepository.SearchReservations(null, reservationFilters);


        page = page < 1 ? 1 : page;
        var pagination = new PaginationModel(page, (int)Math.Ceiling((double)reservations.Count / StaticValues.itemsForPage))
        {
            Searching = searching,
            Status = status,
            StartDate = startDate,
            EndDate = endDate,
        };



        var dataTableModel = new DataTableViewModel()
        {
            Reservations = reservations.Any() ? reservations.Skip((page - 1) * StaticValues.itemsForPage).Take(StaticValues.itemsForPage).ToList() : null,
            Books =  await _bookRepository.GetAll(),
            Pagination = pagination,
            TotalItems = reservations.Count

        };
        return View("Dashboard/Prenotazioni/Tabella", dataTableModel);
    }

    [HttpGet]
    public async Task<IActionResult> ModificaPrenotazione(string id)
    {
        if (!_authService.IsAuthenticated() || _authService.GetCurrentUser().Role != "admin") return RedirectToAction("Index", "Home");

        if (id == null) return NotFound();

        var reservation = await _reservationRepository.GetById(id);
        var book = await _bookRepository.GetById(reservation.BookId);
        if (reservation == null) return NotFound();

        var reservationModel = new ReservationModifyModel()
        {
            Id = id,
            Title = book.Title,
            ISBN = book.Isbn,
            Status = reservation.Status,
            Email = reservation.Email,
            RegisterDate = reservation.Register_Date,
            ExpireDate = reservation.Expire_Date
        };

        return View("Dashboard/Prenotazioni/Modifica", reservationModel);
    }

    [HttpPost]
    public async Task<IActionResult> ModificaPrenotazione(ReservationModifyModel reservationModify)
    {
        if (!_authService.IsAuthenticated() || _authService.GetCurrentUser().Role != "admin") return RedirectToAction("Index", "Home");

        if (!ModelState.IsValid)
        {
            TempData["Notification"] = new Dictionary<string, string>
            {
                { "Title", "Errore!" },
                { "Message", $"Tutti i campi devono essere riempiti!"},
                { "Type", "error" }
            };

            return View("Dashboard/Prenotazioni/Modifica", reservationModify);
        }

        var reservation = await _reservationRepository.GetById(reservationModify.Id);

        reservation.Status = reservation.Status != reservationModify.Status ? reservationModify.Status : reservation.Status; 
        reservation.Expire_Date = reservationModify.ExpireDate;


        if(reservationModify.Status == "consegnato")
        {
            await _bookRepository.BookReturned(reservation.BookId);

            TempData["Notification"] = new Dictionary<string, string>
            {
                { "Title", "Consegna Effettuata!" },
                { "Message", $"Il libro consegnato é tornato disponibile!"},
                { "Type", "success" }
            };

            var user = await _userRepository.GetByEmail(reservation.Email);
            var book = await _bookRepository.GetById(reservation.BookId);
            if(user != null){
                    var bodyEmail = $@"  <!DOCTYPE html>
                    <html lang=""it"">
                    <head>
                        <meta charset=""UTF-8"">
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        <title>Consegna Effetuata!</title>
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
                            <h1>Conferma Consegna</h1>
                            <p>Ciao {user.Name + " " + user.Surname},</p>
                            <p>hai riconsegnato il libro: <strong>{book.Title}</strong> (ISBN: {book.Isbn})</p>
                            <p>di: {string.Join(", ", book.Authors)}</p>
                            <p>Edito da: {book.House},</p>
                            <p>in data {DateTime.Now.ToString("dd/MM/yyyy")},</p>
                            <p>conserva questa mail come conferma della riconsegna in caso di accertamenti</p>
                            <div class=""footer"">
                              <p>Grazie per aver scelto Librelia! Buona lettura.</p>
                            </div>
                        </div>
                    </body>
                    </html>
                    ";
                await _emailService.SendMailAsync(user.Email, "Consegna Effettuata", bodyEmail);
            }
        }
        else
        {
            TempData["Notification"] = new Dictionary<string, string>
            {
                { "Title", "Modifica Effettuata!" },
                { "Message", $"Le modifiche sono state salvate con successo!"},
                { "Type", "success" }
            };
        }

        await _reservationRepository.UpdateReservation(reservation);

        return RedirectToAction("TabellaPrenotazioni");
    }

    [HttpGet]
    public async Task<IActionResult> EliminaPrenotazione(string id)
    {
        if (!_authService.IsAuthenticated() || _authService.GetCurrentUser().Role != "admin") return RedirectToAction("Index", "Home");

        if (id == null)
        {
            TempData["Notification"] = new Dictionary<string, string>
            {
                { "Title", "Errore!" },
                { "Message", $"Prenotazione con questo id non trovato!"},
                { "Type", "error" }
            };
            
            RedirectToAction("TabellaPrenotazioni");
        }

        await _reservationRepository.RemoveReservation(id);


        TempData["Notification"] = new Dictionary<string, string>
        {
            { "Title", "Eliminazione Effettuata!" },
            { "Message", $"Prenotazione eliminata con successo!"},
            { "Type", "success" }
        };

    

        return RedirectToAction("TabellaPrenotazioni");

    }
    
    
    
    /* LIBRI */ 

    public async Task<IActionResult> TabellaLibri(string searching, int page)
    {
        if (!_authService.IsAuthenticated() || _authService.GetCurrentUser().Role != "admin") return RedirectToAction("Index", "Home");

        var bookFilters = new BookFilters()
        {
            Searching = searching,
        };
        var books = await _bookRepository.SearchBooks(bookFilters);


        page = page < 1 ? 1 : page;
        var pagination = new PaginationModel(page, (int)Math.Ceiling((double)books.Count / StaticValues.itemsForPage))
        {
            Searching = searching,
        };



        var dataTableModel = new DataTableViewModel()
        {
            Books = books.Any() ? books.Skip((page - 1) * StaticValues.itemsForPage).Take(StaticValues.itemsForPage).ToList() : null,
            Pagination = pagination,
            TotalItems = books.Count

        };
        return View("Dashboard/Libri/Tabella", dataTableModel);
    }
     [HttpGet]
    public async Task<IActionResult> ModificaLibro(string id)
    {
        if (!_authService.IsAuthenticated() || _authService.GetCurrentUser().Role != "admin") return RedirectToAction("Index", "Home");

        if (id == null) return NotFound();
        
        var book = await _bookRepository.GetById(id);

        if (book == null) return NotFound();
        
        return View("Dashboard/Libri/Modifica", book);
    }
    
    [HttpPost]
    public async Task<IActionResult> ModificaLibro(Book bookModify)
    {
        if (!_authService.IsAuthenticated() || _authService.GetCurrentUser().Role != "admin") return RedirectToAction("Index", "Home");

        if (!Tools.CheckIfEmptyNullOrWhiteSpace(bookModify.Title) &&
            !Tools.CheckIfEmptyNullOrWhiteSpace(bookModify.Isbn) 
            && !Tools.CheckIfEmptyNullOrWhiteSpace(bookModify.ImagePath) && !Tools.CheckIfEmptyNullOrWhiteSpace(bookModify.House) 
            && !Tools.CheckIfEmptyNullOrWhiteSpace(bookModify.Language) && !Tools.CheckIfEmptyNullOrWhiteSpace(bookModify.Collocation) && !bookModify.Categories.Any() && !bookModify.Authors.Any())
        {
            TempData["Notification"] = new Dictionary<string, string>
            {
                { "Title", "Errore!" },
                { "Message", $"Tutti i campi devono essere riempiti!"},
                { "Type", "error" }
            };
            return View("Dashboard/Libri/Modifica", bookModify);
        }
        
        bookModify.Title = bookModify.Title.Trim();
        bookModify.Subtitle = !Tools.CheckIfEmptyNullOrWhiteSpace(bookModify.Subtitle) ? "" : bookModify.Subtitle.Trim();
        bookModify.Isbn = bookModify.Isbn.Trim();
        bookModify.Description = !Tools.CheckIfEmptyNullOrWhiteSpace(bookModify.Description) ? "" : bookModify.Description.Trim();
        bookModify.Collocation = bookModify.Collocation.Trim();
        bookModify.House = bookModify.House.Trim();
        bookModify.Language = !Tools.CheckIfEmptyNullOrWhiteSpace(bookModify.Language) ? bookModify.Language.Trim() : "ita";
        bookModify.Reserved = false;

        await _bookRepository.UpdateBook(bookModify);
        TempData["Notification"] = new Dictionary<string, string>
        {
            { "Title", "Modificato!" },
            { "Message", $"Il libro {bookModify.Title} é stato modificato con successo!"},
            { "Type", "success" }
        };        
        
        return RedirectToAction("TabellaLibri");
    }
    
    [HttpGet]
    public async Task<IActionResult> EliminaLibro(string id)
    {
        if (!_authService.IsAuthenticated() || _authService.GetCurrentUser().Role != "admin") return RedirectToAction("Index", "Home");

        if (id == null)
        {
            TempData["Notification"] = new Dictionary<string, string>
            {
                { "Title", "Errore!" },
                { "Message", $"Libro con questo id non trovato!"},
                { "Type", "error" }
            };
            RedirectToAction("TabellaLibri");
        }
       
        await _bookRepository.RemoveBook(id);
        
        
        TempData["Notification"] = new Dictionary<string, string>
        {
            { "Title", "Eliminazione Effettuata!" },
            { "Message", $"Libro eliminato con successo!"},
            { "Type", "success" }
        };
        

        return RedirectToAction("TabellaLibri");

    }

    
    public async Task<IActionResult> AggiungiLibro()
    {
        if (!_authService.IsAuthenticated() || _authService.GetCurrentUser().Role != "admin") return RedirectToAction("Index", "Home");

        var books = await _bookRepository.GetAll();

        var filteredBooks = books
                .GroupBy(b => b.Isbn)
                .Select(g =>
                    g.FirstOrDefault(book => book.Reserved == false) ?? g.First()
                )
                .ToList();
        var modelNewLibro = new AdminBookViewModel() {
            Book = new Book(),
            Books = filteredBooks
        };
        return View("Dashboard/Libri/Aggiungi", modelNewLibro);
    }
    
    [HttpPost]
    public async Task<IActionResult> AggiungiLibro(AdminBookViewModel model)
    {
        if (!_authService.IsAuthenticated() || _authService.GetCurrentUser().Role != "admin") return RedirectToAction("Index", "Home");

        if (!Tools.CheckIfEmptyNullOrWhiteSpace(model.Book.Title) && !Tools.CheckIfEmptyNullOrWhiteSpace(model.Book.Isbn) 
            && !Tools.CheckIfEmptyNullOrWhiteSpace(model.Book.ImagePath) && !Tools.CheckIfEmptyNullOrWhiteSpace(model.Book.House) 
            && !Tools.CheckIfEmptyNullOrWhiteSpace(model.Book.Language) && !Tools.CheckIfEmptyNullOrWhiteSpace(model.Book.Collocation) && !model.Book.Categories.Any() && !model.Book.Authors.Any())
        {
            TempData["Notification"] = new Dictionary<string, string>
            {
                { "Title", "Errore!" },
                { "Message", $"Tutti i campi devono essere riempiti!"},
                { "Type", "error" }
            };
            return View("Dashboard/Libri/Aggiungi", model);
        }

        var book = model.Book;
        book.Title = book.Title.Trim();
        book.Subtitle = !Tools.CheckIfEmptyNullOrWhiteSpace(book.Subtitle) ? "" : book.Subtitle.Trim();
        book.Isbn = book.Isbn.Trim();
        book.Description = !Tools.CheckIfEmptyNullOrWhiteSpace(book.Description) ? "" : book.Description.Trim();
        book.Collocation = book.Collocation.Trim();
        book.House = book.House.Trim();
        book.Language = !Tools.CheckIfEmptyNullOrWhiteSpace(book.Language) ? book.Language.Trim() : "ita";
        book.Reserved = false;

        await _bookRepository.AddBook(book);
        TempData["Notification"] = new Dictionary<string, string>
        {
            { "Title", "Aggiunto!" },
            { "Message", $"Il libro {book.Title} é stato aggiunto con successo!"},
            { "Type", "success" }
        };        
        
        return RedirectToAction("TabellaLibri");
    }
}