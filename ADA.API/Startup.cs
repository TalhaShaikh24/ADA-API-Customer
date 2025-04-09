using ADA.API.Helpers;
using ADA.API.IRepositories;
using ADA.API.IServices;
using ADA.API.Repositories;
using ADA.API.Services;
using ADA.API.Utility;
using ADA.IServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        public object BCryptNet { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddCors();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ADA.API", Version = "v1" });
            });
            services.AddTransient<IAuthenticationRepository, AuthenticationRepository>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IReservationRepositery, ReservationRepositery>();
            services.AddTransient<IReservationService, ReservationService>();
            services.AddTransient<IRegisterRepository, RegisterRepository>();
            services.AddTransient<IRegisterService, RegisterService>();
            services.AddTransient<ICheckInRepositery, CheckInRepositery>();
            services.AddTransient<ICheckInService, CheckInService>();
            services.AddTransient<ICargoRepositery, CargoRepositery>();
            services.AddTransient<ICargoService, CargoService>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddServices();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ADA.API v1"));
            }
            app.UseCors(options => options.AllowAnyHeader()
            .AllowAnyMethod().AllowAnyOrigin());
               
                app.UseDefaultFiles();
                app.UseStaticFiles();
                app.UseHttpsRedirection();

            app.UseRouting();

            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

          

        }
    }
}
