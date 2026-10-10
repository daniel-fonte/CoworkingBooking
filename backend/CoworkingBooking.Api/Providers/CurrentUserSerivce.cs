using System.Security.Claims;
using CoworkingBooking.Application.Providers;

namespace CoworkingBooking.Api.Providers
{
    public class CurrentUserSerivce : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserSerivce(
            IHttpContextAccessor  httpContextAccessor
        )
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}