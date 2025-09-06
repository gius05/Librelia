using Microsoft.AspNetCore.Mvc.Rendering;

namespace Librelia.Utility
{
    public static class Tools
    {

        public static async Task<List<SelectListItem>> ToSelectListItems(List<Book> books)
        {
            if (books == null || !books.Any())
                return new List<SelectListItem>();

            return books
                .OrderBy(book => book.Title)
                .Select(book => new SelectListItem
                {
                    Value = book.Id,
                    Text = $"{book.Title} {(string.IsNullOrWhiteSpace(book.Isbn) ? "" : $"(ISBN: {book.Isbn})")}"
                }).ToList();
        }
        public static async Task<List<SelectListItem>> ToSelectListItems(List<string> list)
        {
            if (list == null || !list.Any())
                return new List<SelectListItem>();

            return list
                .OrderBy(x => x )
                .Select(x => new SelectListItem
                {
                    Value = x,
                    Text = $"{x}"
                }).ToList();
        }
        public static bool CheckIfEmptyNullOrWhiteSpace(string element) =>
            !string.IsNullOrWhiteSpace(element) && !string.IsNullOrEmpty(element);
    }
}
