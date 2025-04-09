using ADAClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADA.API.IRepositories
{
    public interface ICheckInRepositery
    {
        CheckIn Add(CheckIn obj);

        CheckIn Update(CheckIn obj);

        List<CheckIn> GetAll();

        CheckInDTO GetAllReserVationByFlightId(int id);
        CheckInDTO GetFlightNoByUserId(int id);
        List<CheckinCardDetails> GetCheckinCardDetails(int id);

        object GetuserDatabyRsvnId(int Id);
        object GetCheckInsDropDown();

    }
}
