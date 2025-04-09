using ADAClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADA.API.IRepositories
{
   public interface IFlightRepositery
    {
        Flight Add(Flight obj);

        Flight Update(Flight obj);

        List<Flight> GetAll();
        Flight GetByID(int id);

        List<Register> GetAllMembersDetails(int FltId, List<int> MembersIds, string RegisterType);
       
        object GetFlightsDropDown();
        List<Flight> GetSearchFlight(SearchFlight obj);
        object GetAircraftType(string type);
        object GetFlightDesitnationAndNationality(DestinationAndNationality obj);
        List<FlightStatus> GetFlightStatusByFlightId(string FNo);
    }
}
