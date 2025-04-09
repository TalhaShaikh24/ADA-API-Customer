using ADAClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADA.API.IServices
{
    public interface ICheckInService : IService<CheckIn>
    {
        CheckIn Add(CheckIn obj);

        CheckIn Update(CheckIn obj);

        CheckInDTO GetAllReserVationByFlightId(int id);
        CheckInDTO GetFlightNoByUserId(int id);
        List<CheckinCardDetails> GetCheckinCardDetails(int id);

        object GetuserDatabyRsvnId(int Id);
        object GetCheckInsDropDown();
    }
}
