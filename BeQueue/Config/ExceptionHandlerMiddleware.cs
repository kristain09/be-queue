using System.Text.Json;
using BeQueue.Exception;
using Microsoft.AspNetCore.Diagnostics;

namespace BeQueue.Config;

public static class ExceptionMiddlewareConfiguration
{
  public static void ConfigureExceptionHandler(this WebApplication app)
  {
    app.UseExceptionHandler(appError =>
    {
      appError.Run(async context =>
      {
        context.Response.ContentType = "application/json";

        IExceptionHandlerFeature? contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature != null)
        {
          context.Response.StatusCode = contextFeature.Error switch
          {
            ResourceNotFoundException => StatusCodes.Status404NotFound,
            AzureNotAvailable => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status500InternalServerError
          };
          
          var errorMessage = contextFeature.Error switch
          {
            _ => contextFeature.Error.Message
          };

          await context.Response.WriteAsync(new ErrorDetail
          {
            StatusCode = context.Response.StatusCode, Message = errorMessage
          }.ToString());
        }
      });
    });
  }
}

public class ErrorDetail
{
  public int StatusCode { get; set; }
  public string? Message { get; set; }

  public override string ToString()
  {
    return JsonSerializer.Serialize(this, new JsonSerializerOptions(JsonSerializerDefaults.Web));
  }
}