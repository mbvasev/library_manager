using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Movies.Service.Middleware.Translators;
using System;
using System.Threading.Tasks;

namespace Movies.Service.Middleware
{
    public sealed class ExceptionHanldingMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHanldingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception e)
            {
                await ExceptionToHttpTranslator.Translate(httpContext, e);
            }
        }
    }

    public static class AppBuilderExtensions
    {
        public static void UseCustomExceptionHandling(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHanldingMiddleware>();
        }
    }
}
