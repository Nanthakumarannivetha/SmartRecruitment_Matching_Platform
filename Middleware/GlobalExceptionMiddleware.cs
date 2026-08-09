using System.Net;
using System.Text.Json;
using SmartRecruitment_Project.Exceptions;

namespace SmartRecruitment_Project.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            var statusCode = exception switch
            {
                BadRequestException =>
                    HttpStatusCode.BadRequest,

                UnauthorizedException =>
                    HttpStatusCode.Unauthorized,

                ForbiddenException =>
                    HttpStatusCode.Forbidden,

                NotFoundException =>
                    HttpStatusCode.NotFound,

                ConflictException =>
                    HttpStatusCode.Conflict,

                ArgumentException =>
                    HttpStatusCode.BadRequest,

                KeyNotFoundException =>
                    HttpStatusCode.NotFound,

                FileNotFoundException =>
                    HttpStatusCode.NotFound,

                _ =>
                    HttpStatusCode.InternalServerError
            };

            var response = new
            {
                statusCode = (int)statusCode,
                message = statusCode ==
                          HttpStatusCode.InternalServerError
                    ? "An unexpected error occurred."
                    : exception.Message
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var jsonResponse =
                JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}