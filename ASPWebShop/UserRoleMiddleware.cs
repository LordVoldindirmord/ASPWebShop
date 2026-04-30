using System.Security.Claims;

namespace ASPWebShop
{
    public class UserRoleMiddleware
    {
        private readonly RequestDelegate _next;

        public UserRoleMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var role = context.User.FindFirstValue(ClaimTypes.Role) ?? "Customer";
                context.Items["UserRole"] = role;
            }

            await _next(context);
        }
    }
}
