using ADAClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADA.API.IServices
{
   public interface IFlightService : IService<Flight>
    {
        Flight Add(Flight obj);
        Flight Update(Flight obj);

        List<Register> GetAllMembersDetails(int FltId,List<int> MembersIds, string RegisterType);


        object GetDropdownValues();

        List<Flight> GetSearchFlight(SearchFlight obj);

        object GetFlightDesitnationAndNationality(DestinationAndNationality obj);
        List<FlightStatus> GetFlightStatusByFlightId(string FNo);
        object GetAircraftType(string type);
      
    }
}
