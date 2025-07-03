using Microsoft.EntityFrameworkCore;
using RBAC_API.Database;
using RBAC_API.ExceptionHandler;

namespace RBAC_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            // Database connection configuration
            builder.Services.AddDbContext<RbacContext>(ops => 
                ops.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            // Global exception handler configuration
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails(); // additional support for RFC 7807 problem details format

            var app = builder.Build();

            // Use global exception handler
            app.UseExceptionHandler();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
