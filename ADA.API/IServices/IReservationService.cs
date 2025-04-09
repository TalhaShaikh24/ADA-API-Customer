using ADAClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADA.API.IServices
{
    public interface IReservationService : IService<Reservation>
    {
        List<Reservation> Add(Reservation obj);
        Reservation Update(Reservation obj);
        Reservation GetReservationBtID(int id);
        List<MyBookingDetails> GetMyBookingDetailsByID(int id);
        List<MyBookingDetails> GetCancelBookingDetailsByUserId(int id);
       
        List<MyBookingDetails> CancelBookingByRsvnId(Reservation obj);
        List<MyBookingDetails> ReBookingByRsvnId(Reservation obj);
        List<BordingPassDetails> GetBordingPassDetails(string PNR);
        object GetReservationsDropDown();

    }
}
