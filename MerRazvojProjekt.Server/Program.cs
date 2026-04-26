using MerRazvojProjekt.Server.Data;
using MerRazvojProjekt.Server.Models;
using MerRazvojProjekt.Server.Service.Implementations;
using MerRazvojProjekt.Server.Service.Interfaces;
using MerRazvojProjekt.Server.Exceptions;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using MerRazvojProjekt.Server.Validators;
using MerRazvojProjekt.Server.Middlewares;


namespace MerRazvojProjekt.Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AngularDev", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            builder.Services.AddProblemDetails(configure =>
            {
                configure.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
                };
            });
            builder.Services.AddExceptionHandler<ExceptionMiddlware>();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IRequestLogService, RequestLogService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<ICarService, CarService>();
            builder.Services.AddValidatorsFromAssemblyContaining<CustomerDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<CarDtoValidator>();


            builder.Services.RegisterMapsterConfiguration();

            var app = builder.Build();

            app.UseExceptionHandler();
            app.UseMiddleware<LoggingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AngularDev");
            app.UseAuthorization();
            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.Database.MigrateAsync();
                await ApplicationDbSeeder.SeedCustomersAsync(dbContext);
                await ApplicationDbSeeder.SeedCarsAsync(dbContext);
            }

            app.Run();
        }
    }
}