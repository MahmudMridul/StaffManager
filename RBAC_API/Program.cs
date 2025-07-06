using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RBAC_API.Database;
using RBAC_API.ExceptionHandler;
using RBAC_API.Models;
using RBAC_API.Services;
using RBAC_API.Servies;
using System.Text;

namespace RBAC_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            #region Custom Services
            builder.Services.AddScoped<SignupValidationService>();
            builder.Services.AddScoped<JwtService>();
            #endregion

            #region Database connection configuration
            builder.Services.AddDbContext<RbacContext>(ops => 
                ops.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            );
            #endregion

            #region Global exception handler configuration
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails(); // additional support for RFC 7807 problem details format
            #endregion

            #region Identity Configuration
            builder.Services.AddIdentity<User, Role>(ops =>
            {
                ops.User.RequireUniqueEmail = true;

                ops.Password.RequiredLength = 8;
                ops.Password.RequireDigit = true;
                ops.Password.RequireUppercase = true;
                ops.Password.RequireNonAlphanumeric = true;
                ops.Password.RequiredUniqueChars = 8;

                ops.Lockout.AllowedForNewUsers = true;
                ops.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                ops.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<RbacContext>()
            .AddDefaultTokenProviders();
            #endregion

            #region JWT Configuration
            builder.Services.AddAuthentication(ops =>
            {
                ops.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                ops.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(ops =>
            {
                ops.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });
            #endregion

            #region CORS
            builder.Services.AddCors(op =>
                op.AddPolicy(
                    "AllowAll",
                    policy => policy
                    .WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                )
            );
            #endregion

            var app = builder.Build();

            // Use global exception handler
            app.UseExceptionHandler();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            // CORS
            app.UseCors("AllowAll");

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
