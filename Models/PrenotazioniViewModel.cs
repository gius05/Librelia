namespace Librelia.Models
{

    public class PrenotazioniViewModel
    {
        public List<Reservation>? Reservations { get; set; }
        public List<Book>? Books { get; set; }
        public ReservationFilters? Filters { get; set; }
        public PaginationModel? Pagination { get; set; }
        public string? Error { get; set; }
    }
}