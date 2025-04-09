using ADA.API.IRepositories;
using ADA.API.IServices;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADA.API.Utility
{
    public class DUnit : IDIUnit
    {
        public DUnit( IMemoryCache MemoryCache , IFlightService FlightService)
        {
         
            memoryCache = MemoryCache;
            flightService = FlightService;
        }
    

        public IMemoryCache memoryCache { get; }
        public IFlightService flightService { get; }


    }
}
