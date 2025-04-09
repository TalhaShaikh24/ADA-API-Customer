using ADA.API.IRepositories;
using ADA.API.IServices;
using ADAClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADA.API.Services
{
    public class CheckInService : ICheckInService
    {
        private readonly ICheckInRepositery _CheckInRepository;

        public CheckInService(ICheckInRepositery CheckInRepository)
        {
            _CheckInRepository = CheckInRepository;
        }
        public CheckIn Add(CheckIn obj)
        {

            return _CheckInRepository.Add(obj);
        }

        public List<CheckIn> GetAll()
        {
            return _CheckInRepository.GetAll();
        }

        public CheckInDTO GetAllReserVationByFlightId(int id)
        {
            return _CheckInRepository.GetAllReserVationByFlightId(id);
        }

        public List<CheckinCardDetails> GetCheckinCardDetails(int id)
        {
            return _CheckInRepository.GetCheckinCardDetails(id);


        }

        public object GetCheckInsDropDown()
        {
            return _CheckInRepository.GetCheckInsDropDown();
        }

        public CheckInDTO GetFlightNoByUserId(int id)
        {
            return _CheckInRepository.GetFlightNoByUserId(id);
        }

        public object GetuserDatabyRsvnId(int Id)
        {
            return _CheckInRepository.GetuserDatabyRsvnId(Id);
        }

        public CheckIn Update(CheckIn obj)
        {

            return _CheckInRepository.Update(obj);
        }
    }
}
