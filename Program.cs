using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Mio_Rest_Api.Data;
using Mio_Rest_Api.Services;
using System.Text.Json.Serialization;



namespace Mio_Rest_Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string? connect = builder.Configuration.GetConnectionString("MioConnect");

            // Add services to the container.
            builder.Services.AddDbContext<ContextApplication>(opt => opt.UseSqlServer(connect));
            builder.Services.AddScoped<IServiceReservation, ServiceReservations>();
            builder.Services.AddScoped<IServiceOccupation, ServiceOccupation>();
            builder.Services.AddScoped<IServiceMenuDuJour, ServiceMenuDuJour>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            

            // Configure JWT authentication
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings.GetValue<string>("SecretKey"));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
                    ValidAudience = jwtSettings.GetValue<string>("Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            builder.Services.AddControllers().AddJsonOptions(opt =>
            opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", // Nom de la politique CORS
                    builder =>
                    {
                        builder.AllowAnyOrigin()    // Autoriser toutes les origines
                               .AllowAnyHeader()    // Autoriser tous les en-têtes
                               .AllowAnyMethod();   // Autoriser toutes les méthodes
                    });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication(); // cette ligne pour utiliser l'authentification
            app.UseAuthorization();

            app.MapControllers();
            app.UseCors("AllowAllOrigins");

            app.Run();
        }
    }
}
