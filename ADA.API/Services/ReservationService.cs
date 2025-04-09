using ADA.API.IRepositories;
using ADA.API.IServices;
using ADAClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADA.API.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepositery _ReservationRepository;

        public ReservationService(IReservationRepositery ReservationRepository)
        {
            _ReservationRepository = ReservationRepository;
        }
        public List<Reservation> Add(Reservation obj)
        {

            return _ReservationRepository.Add(obj);
        }

        public List<Reservation> GetAll()
        {
            return _ReservationRepository.GetAll();
        }

        public object GetReservationsDropDown()
        {
            return _ReservationRepository.GetReservationsDropDown();
        }


        public Reservation GetReservationBtID(int id)
        {
            return _ReservationRepository.GetByID(id);
        }


        public Reservation Update(Reservation obj)
        {

            return _ReservationRepository.Update(obj);
        }

        public List<MyBookingDetails> GetMyBookingDetailsByID(int id)
        {
            return _ReservationRepository.GetMyBookingDetailsByID(id);
        }
        public List<MyBookingDetails> GetCancelBookingDetailsByUserId(int id)
        {
            return _ReservationRepository.GetCancelBookingDetailsByUserId(id);
        }
       

        public List<MyBookingDetails> CancelBookingByRsvnId(Reservation obj)
        {
            return _ReservationRepository.CancelBookingByRsvnId(obj);
        }
        public List<MyBookingDetails> ReBookingByRsvnId(Reservation obj)
        {
            return _ReservationRepository.ReBookingByRsvnId(obj);
        }
        public List<BordingPassDetails> GetBordingPassDetails(string PNR)
        {
            return _ReservationRepository.GetBordingPassDetails(PNR);
        }
    }
}
