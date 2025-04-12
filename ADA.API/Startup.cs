using ADA.API.DBManager;
using ADA.API.Helpers;
using ADA.API.IRepositories;
using ADA.API.IServices;
using ADA.API.Repositories;
using ADA.API.Services;
using ADA.API.Utility;
using ADA.IServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ADA.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCors();
            services.AddMemoryCache();

            // JWT Configuration
            var jwtSettings = Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
            {
                throw new ArgumentException("JWT SecretKey must be at least 32 characters long");
            }
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
             .AddCookie(options =>
              {
                  options.Cookie.HttpOnly = true;
                  options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
              });

            //services.AddControllers(options =>
            //{
            //    options.Filters.Add(new Utility.AuthorizeAttribute());
            //});

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero,
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var userId = context.Principal.FindFirst("id")?.Value;
                        var token = context.SecurityToken as JwtSecurityToken;
                        var tokenFromRequest = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(tokenFromRequest))
                        {
                            context.Fail("Unauthorized: Missing user ID or token.");
                            return;
                        }

                        var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationService>();
                        var user = authService.GetUserByIdAsync(int.Parse(userId));

                        if (user == null)
                        {
                            context.Fail("Unauthorized");
                            return;
                        }

                        var currentToken = await authService.GetSavedTokenAsync(int.Parse(userId));

                        if (currentToken != tokenFromRequest)
                        {
                            context.Fail("Token no longer valid");
                        }

                        bool isValid = await authService.IsTokenValidAsync(int.Parse(userId), tokenFromRequest);

                        if (!isValid)
                        {
                            context.Fail("Unauthorized: Token has been logged out or invalidated.");
                        }
                    },
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Cookies["AuthToken"];

                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            // Dependency Injection
            services.AddScoped<IDIUnit, DUnit>();
            services.AddScoped<IDapper, Dapperr>();
            services.AddScoped<TokenManager>();

            // Repositories
            services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            services.AddScoped<IReservationRepositery, ReservationRepositery>();
            services.AddScoped<IRegisterRepository, RegisterRepository>();
            services.AddScoped<ICheckInRepositery, CheckInRepositery>();
            services.AddScoped<ICargoRepositery, CargoRepositery>();
            services.AddScoped<IFlightRepositery, FlightRepositery>();

            // Services
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IReservationService, ReservationService>();
            services.AddTransient<IRegisterService, RegisterService>();
            services.AddTransient<ICheckInService, CheckInService>();
            services.AddTransient<ICargoService, CargoService>();
            services.AddTransient<IFlightService, FlightService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ADA API",
                    Version = "v1",
                    Description = "JWT Authentication Required for Protected Endpoints"
                });
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter JWT Bearer token",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };


                c.AddSecurityDefinition("Bearer", securityScheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        securityScheme,
                        new string[] {}
                    }
                });
                c.OperationFilter<AuthResponsesOperationFilter>();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ADA API v1");
                    c.OAuthClientId("swagger-ui");
                    c.OAuthAppName("Swagger UI");
                    c.OAuthUsePkce();
                    c.InjectJavascript("/swagger/swagger-config.js");
                });
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors(options => options
    .AllowAnyHeader()
    .AllowAnyMethod()
    .SetIsOriginAllowed(_ => true) // <- allows any origin
    .AllowCredentials());



            app.UseAuthentication();
            app.UseAuthorization();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
   
}