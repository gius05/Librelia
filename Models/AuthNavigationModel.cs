namespace Librelia.Models
{
    public class AuthNavigationModel
    {
        public bool IsAuthenticated { get; set; }
        public User? CurrentUser { get; set; }
        public bool IsAdmin { get; set; }
    }
}
