using Librelia.Database;
using Librelia.Models;
using Librelia.Repositories;
using Librelia.Services;
using Microsoft.AspNetCore.Mvc;

namespace Librelia.Controllers;

public class ProfiloController : Controller
{
    private readonly ReservationRepository _reservationRepo;
    private readonly BookRepository _bookRepository;
    private readonly AuthService _authService;

    public ProfiloController(IHttpContextAccessor httpContextAccessor, ReservationRepository reservationRepo, BookRepository bookRepository)
    {
        _authService = new AuthService(httpContextAccessor);
        _reservationRepo = reservationRepo;
        _bookRepository = bookRepository;   
    }
    
    public async Task<IActionResult> Prenotazioni(string searching, string status, string sort, int page)
    {
        if (!_authService.IsAuthenticated()) return RedirectToAction("Index", "Home");

        var filters = new ReservationFilters()
        {
            Searching = searching,
            Status = status,
            SortBy = sort?.Split('-')[0] ?? "date",
            SortDirection = sort?.Split('-')[1] ?? "asc"
        };
        var reservations = await _reservationRepo.SearchReservations( _authService.GetCurrentUser().Email, filters);
        var books = await _bookRepository.GetAll();
        page = page < 1 ? 1 : page;
        var pagination =  new PaginationModel(page, (int)Math.Ceiling((double)reservations.Count / StaticValues.itemsForPage))
        {
            Searching = searching,
            Status = status,
            Category = null,
            Availability = null,
            Sort = sort
        };

        var catalogViewModel = new PrenotazioniViewModel()
        {
            Reservations = reservations.Any() ? reservations.Skip((page - 1) * StaticValues.itemsForPage).Take(StaticValues.itemsForPage).ToList() : null,
            Books = books.Any() ? books : null,
            Filters = filters != null ? filters : null,
            Pagination = pagination,
            Error = reservations.Any() ? null : "Non ci sono prenotazioni",
        };
            
        return View(catalogViewModel);
    }

}