using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartRecruitment_Project.Data;
using SmartRecruitment_Project.Interfaces.Repositories;
using SmartRecruitment_Project.Interfaces.Services;
using SmartRecruitment_Project.Middleware;
using SmartRecruitment_Project.Options;
using SmartRecruitment_Project.Repositories;
using SmartRecruitment_Project.Services;
using System.Text;

namespace SmartRecruitment_Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ==========================================
            // Database Connection
            // ==========================================
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString(
                        "DefaultConnection")));

            // ==========================================
            // Controllers
            // ==========================================
            builder.Services.AddControllers();

            // ==========================================
            // JWT Options
            // ==========================================
            builder.Services.Configure<JwtOptions>(
                builder.Configuration.GetSection("Jwt"));

            var jwtSettings =
                builder.Configuration.GetSection("Jwt");

            var jwtKey = jwtSettings["Key"];

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException(
                    "JWT Key is missing from configuration.");
            }

            // ==========================================
            // JWT Authentication
            // ==========================================
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwtKey))
                    };
            });

            // ==========================================
            // Authorization
            // ==========================================
            builder.Services.AddAuthorization();

            // ==========================================
            // Dependency Injection
            // ==========================================
            builder.Services.AddScoped<
                IAuthRepository,
                AuthRepository>();

            builder.Services.AddScoped<
                IAuthService,
                AuthService>();

            builder.Services.AddScoped<
                IJwtTokenService,
                JwtTokenService>();

            builder.Services.AddScoped<
    IEmployerRepository,
    EmployerRepository>();

            builder.Services.AddScoped<
                IEmployerService,
                EmployerService>();

            builder.Services.AddScoped<
                IJobRepository,
                JobRepository>();

            builder.Services.AddScoped<
                IJobService,
                JobService>();
            builder.Services.AddScoped<
    INotificationRepository,
    NotificationRepository>();

            builder.Services.AddScoped<
                INotificationService,
                NotificationService>();

            builder.Services.AddScoped<
                IAdminRepository,
                AdminRepository>();

            builder.Services.AddScoped<
                IAdminService,
                AdminService>();
            // ==========================================
            // Swagger / OpenAPI
            // ==========================================
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Enter JWT token"
                    });

                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type =
                                        ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
            });

            // ==========================================
            // Build Application
            // ==========================================
            var app = builder.Build();

            // ==========================================
            // Global Exception Handling
            // ==========================================
            app.UseMiddleware<GlobalExceptionMiddleware>();

            // ==========================================
            // Swagger
            // ==========================================
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // ==========================================
            // HTTP Request Pipeline
            // ==========================================
            app.UseHttpsRedirection();

            // Authentication must come before Authorization
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}