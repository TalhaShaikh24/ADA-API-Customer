
using ADA.API.IRepositories;
using ADA.API.IServices;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADA.API.Utility
{
    public interface IDIUnit
    {

        IMemoryCache memoryCache { get; }
        IFlightService flightService { get; }
    }
}
