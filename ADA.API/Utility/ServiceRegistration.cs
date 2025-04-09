
using ADA.API.DBManager;
using ADA.API.IRepositories;
using ADA.API.IServices;
using ADA.API.Repositories;
using ADA.API.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADA.API.Utility
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
          

            ///repository
        
            services.AddTransient<IDapper, Dapperr>();
 
          
            services.AddTransient<IFlightRepositery, FlightRepositery>();


            ////services
          
            services.AddTransient<IFlightService, FlightService>();

            ///Dependency Ijection
            services.AddTransient<IDIUnit, DUnit>();

            services.AddMemoryCache();
        }
    }
}
