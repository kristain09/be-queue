using BeQueue.Config;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
  options.SwaggerDoc("v1", new OpenApiInfo
  {
    Version = "v1",
    Title = "Interview Queues",
    Description = "Interview technical test",
    Contact = new OpenApiContact
    {
      Name = "Kristain Putra",
      Email = "kputrapar@gmail.com",
    }
  });
  options.EnableAnnotations();
});

builder.Services.InjectService();

var corsAllowAnyOrigin = true;

builder.Services.AddCors(options =>
{
  options.AddDefaultPolicy(
    policy  =>
    {
      if (corsAllowAnyOrigin)
      {
        policy.AllowAnyOrigin();
      }
      else
      {
        policy.WithOrigins();
      }

      policy.AllowAnyHeader();
      policy.AllowAnyMethod();
    });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.ConfigureExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors();
app.MapControllers();

app.Run();