using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace secure_webapp_api;

public class ExceptionsMiddleware
{
  private readonly RequestDelegate _next;
  public ExceptionsMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext httpContext)
  {
    try
    {
      await _next(httpContext);
    }
    catch (Exception ex)
    {
      await HandleExceptionAsync(httpContext, ex);
    }
  }
  private async Task HandleExceptionAsync(HttpContext context, Exception exception)
  {
    context.Response.ContentType = "application/json";
    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    await context.Response.WriteAsync(new
    {
      StatusCode = context.Response.StatusCode,
      Message = "Internal Server Error."
    }.ToString()!);
  }
}
