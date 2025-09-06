using Librelia.DTO;
using Librelia.Models;
using System.Security.Claims;

namespace Librelia.Services
{
    public class AuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }

        public UserDTO? GetCurrentUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated)
                return null;

            return new UserDTO
            {
                Id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Name = user.FindFirst(ClaimTypes.GivenName)?.Value,
                Surname = user.FindFirst(ClaimTypes.Surname)?.Value,
                Email = user.FindFirst(ClaimTypes.Email)?.Value,
                Role = user.FindFirst(ClaimTypes.Role)?.Value,
                External = bool.TryParse(user.FindFirst("isExternal")?.Value, out var isExternal) && isExternal,
                Status = user.FindFirst("status")?.Value
            };
        }
    }
}
