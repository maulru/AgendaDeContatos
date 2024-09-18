using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Middleware
{
    public class JwtCookieMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtCookieMiddleware> _logger;

        public JwtCookieMiddleware(RequestDelegate next, ILogger<JwtCookieMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Cookies["JWTToken"];

            if (!string.IsNullOrEmpty(token))
            {
                _logger.LogInformation("JWT Token encontrado no cookie. Adicionando ao cabeçalho Authorization.");
                context.Request.Headers.Add("Authorization", $"Bearer {token}");
            }
            else
            {
                _logger.LogWarning("JWT Token não encontrado no cookie.");
            }

            await _next(context);
        }
    }

}
