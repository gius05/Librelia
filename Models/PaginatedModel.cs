using System.Web;

namespace Librelia.Models
{
    public class PaginationModel
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public List<int> PageNumbers { get; set; }
        public bool ShowFirstPage => PageNumbers.Any() && PageNumbers.First() > 1;
        public bool ShowLastPage => PageNumbers.Any() && PageNumbers.Last() < TotalPages;
        public bool IsPrevDisabled => CurrentPage <= 1;
        public bool IsNextDisabled => CurrentPage >= TotalPages;

        // Proprietà per i filtri
        public string Searching { get; set; }
        public string Category { get; set; }
        
        public string Status { get; set; }
        public string Availability { get; set; }
        public string Sort { get; set; }

        public string StartDate { get; set; } = null;
        public string EndDate { get; set; } = null;
        public PaginationModel(int currentPage, int totalPages, int range = 2)
        {
            CurrentPage = currentPage;
            TotalPages = totalPages;

            int startPage = Math.Max(1, CurrentPage - range);
            int endPage = Math.Min(TotalPages, CurrentPage + range);

            PageNumbers = Enumerable.Range(startPage, endPage - startPage + 1).ToList();
        }

        public string GetPageUrl(int page)
        {
            // Costruisci la query string includendo la pagina e i filtri se presenti
            var queryParams = new List<string>
            {
                $"page={page}"
            };

            if (!string.IsNullOrEmpty(Searching))
            {
                queryParams.Add($"searching={HttpUtility.UrlEncode(Searching)}");
            }
            if (!string.IsNullOrEmpty(Category))
            {
                queryParams.Add($"category={HttpUtility.UrlEncode(Category)}");
            }
            if (!string.IsNullOrEmpty(Availability))
            {
                queryParams.Add($"availability={HttpUtility.UrlEncode(Availability)}");
            }
            if (!string.IsNullOrEmpty(Sort))
            {
                queryParams.Add($"sort={HttpUtility.UrlEncode(Sort)}");
            }
            if (!string.IsNullOrEmpty(Status))
            {
                queryParams.Add($"status={HttpUtility.UrlEncode(Status)}");
            }
            if (!DateTime.TryParse(StartDate, out var startDate))
            {
                queryParams.Add($"startDate={HttpUtility.UrlEncode(StartDate)}");
            }
            if (DateTime.TryParse(EndDate, out var endDate))
            {
                queryParams.Add($"endDate={HttpUtility.UrlEncode(EndDate)}");
            }
            return "?" + string.Join("&", queryParams);
        }
    }
}
