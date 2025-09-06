using Librelia.DTO;

namespace Librelia.Models;

public class DataTableViewModel
{
    public string Searching { get; set; } = null;
    public List<UserDTO> Users { get; set; } = null;
    public List<Book> Books { get; set; } = null;
    public List<Reservation> Reservations { get; set; } = null;

    public int TotalItems { get; set; }
    public PaginationModel Pagination { get; set; } = null;
}