using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAClassLibrary
{

   public class Reservation
    {
        public int RsvnID { get; set; }
        public DateTime? RsvnTimeStamp { get; set; }
        public string RsvnPNR { get; set; }
        public string AdultPNR { get; set; }
        public string PaxName { get; set; }
        public string PaxIDNum { get; set; }
        public List<string> PaxIDType { get; set; }
        public DateTime? PaxIDExpiry { get; set; }
        public DateTime? PaxBDay { get; set; }
        public string PaxNationality { get; set; }
        public string PaxGender { get; set; }
        public string PaxCompany { get; set; }
        public string CardNum { get; set; }
        public string SeatNum { get; set; }
        public int PaxDestination { get; set; }
        public int PaxWT { get; set; }
        public int BagWt { get; set; }
        public int BagPcs { get; set; }
        public string RsvnStatus { get; set; }
        public string PaxMobNum { get; set; }
        public string RsvEMail { get; set; }
        public DateTime? SMSTimeStamp { get; set; }
        public string RsvRmks { get; set; }
        public int RsvnAgent { get; set; }
        public int RsvnChkAgent { get; set; }
        public DateTime? RsvnChkTimeStamp { get; set; }
        public int PaxBoarded { get; set; }
        public string ManifestColor { get; set; }
        public int PaxTransitDest { get; set; }
        public int FltID { get; set; }
        public string FltNumber { get; set; }
        public string FltDateTime { get; set; }
        public int UserId { get; set; }
        public int GlobalFKId { get; set; }
        public List<Reservation> Groupreservation { get; set; }

        public string Lang { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public string UpdatedActionSource { get; set; }
        public string ActionSource { get; set; }
    }


    public class MyBookingDetails
    {
        public string RsvnID { get; set; }
        public DateTime? BookingDate { get; set; }
        public string Name { get; set; }
        public string BookingPNR { get; set; }
        public string Base { get; set; }
        public string BaseArabic { get; set; }
        public string fltStaus { get; set; }
        public string RsvnStatus { get; set; }
        public string PaxMobNum { get; set; }
        public string PaxName { get; set; }
        public string PaxGender { get; set; }
        public string BookingDestination { get; set; }
        public string BookingDestinationAR { get; set; }
        public string PaxIDType { get; set; }
        public DateTime? FltDateTime { get; set; }
        public string FltNumber { get; set; }
        public string ACReg { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public string UpdatedActionSource { get; set; }
        public string ActionSource { get; set; }

    }

    public class BordingPassDetails
    {

        public int RsvnID { get; set; }
        public string Paxname { get; set; }
        public string RsvnPNR { get; set; }
        public string RsvnStatus { get; set; }
        public string SeatNum { get; set; }
        public int UserId { get; set; }
        public string FltNumber { get; set; }
        public DateTime FltDateTime { get; set; }
        public string Base { get; set; }
        public string BaseShort { get; set; }
        public string DestName { get; set; }
        public string DestShort { get; set; }
        public string BaseArabic { get; set; }
        public string DestNameArabic { get; set; }
        public int GateNum { get; set; }

    }

}
