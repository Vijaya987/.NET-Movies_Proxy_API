using Microsoft.AspNetCore.Http;
using Movies_Proxy_API.Repository;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Movies_Proxy_API
{
    
    public class ExceptionHandlingMiddleware
    {
        private RequestDelegate _next;
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex)
            {
                await context.Response.WriteAsync(ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }
        public Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            string Message = "Internal server error";

            string result = new ErrorResponse
            {
                ErrorMessage = Message,
                StatusCode = context.Response.StatusCode,
            }.ToString();
            return context.Response.WriteAsync(result);
        }
    }
}
