using ADA.API.IRepositories;
using ADA.API.IServices;
using ADAClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADA.API.Services
{
    public class FlightService : IFlightService
    {
        private readonly IFlightRepositery _flightRepository;

        public FlightService(IFlightRepositery flightRepository)
        {
            _flightRepository = flightRepository;
        }
        public Flight Add(Flight obj)
        {
     
            return _flightRepository.Add(obj);
        }

        public List<Flight> GetAll()
        {
            return _flightRepository.GetAll();
        }

        public object GetDropdownValues()
        {
            return _flightRepository.GetFlightsDropDown();
        }
        public object GetAircraftType(string type)
        {
            return _flightRepository.GetAircraftType(type);
        }

   

        public Flight Update(Flight obj)
        {
            
            return _flightRepository.Update(obj);
        }

        public List<Flight> GetSearchFlight(SearchFlight obj)
        {
            return _flightRepository.GetSearchFlight(obj);
        }

        public object GetFlightDesitnationAndNationality(DestinationAndNationality obj)
        {
            return _flightRepository.GetFlightDesitnationAndNationality(obj);
        }

        public List<Register> GetAllMembersDetails(int FltId, List<int> MembersIds, string RegisterType)
        {
            return _flightRepository.GetAllMembersDetails(FltId,MembersIds, RegisterType);
        }
        public List<FlightStatus> GetFlightStatusByFlightId(string FNo)
        {
            return _flightRepository.GetFlightStatusByFlightId(FNo);
        }

    }
}
