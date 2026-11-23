
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using ShelterAPI.Models;

namespace ShelterAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var hibak = context.ModelState.Values
                        .SelectMany(value => value.Errors)
                        .Select(error => error.ErrorMessage)
                        .ToList();
                    return new BadRequestObjectResult(new
                    {
                        message = "Hibás vagy hiányos kérésadatok.",
                        result = hibak
                    });
                };
            });
            builder.Services.AddOpenApi();
            builder.Services.AddDbContext<AnimalshelterContext>(options =>
                options.UseMySQL(builder.Configuration.GetConnectionString("Mysql")!));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();
            app.MapControllers();
            app.Run();
        }
    }
}
