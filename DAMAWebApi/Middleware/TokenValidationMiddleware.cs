using DAMA.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DAMAWebApi.Middleware
{
    public class TokenValidationMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            // Get token from the Authorization header.
            var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader))
            {
                // Expect header format: "Bearer <token>"
                var token = authHeader.StartsWith("Bearer ")
                    ? authHeader.Substring("Bearer ".Length).Trim()
                    : authHeader;

                if (!string.IsNullOrEmpty(token))
                {
                    // Create a scope to resolve scoped services like IAuthService.
                    using var scope = context.RequestServices.CreateScope();
                    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                    bool isRevoked = await authService.IsTokenRevoked(token);
                    if (isRevoked)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Token has been revoked. Please log in again.");
                        return;
                    }
                }
            }
            await _next(context);
        }
    }
}
