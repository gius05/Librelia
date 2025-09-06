namespace Librelia.Models
{
    public class CatalogViewModel
    {
        public List<Book>? Books { get; set; }
        public BookFilters? Filters { get; set; }
        public PaginationModel? Pagination { get; set; }
        public string? Error { get; set; }

    }
}
