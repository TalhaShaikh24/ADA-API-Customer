using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAClassLibrary
{
    public class CheckIn
    {
        public int RsvnID { get; set; }
        public DateTime? RsvnTimeStamp { get; set; }
        public string RsvnPNR { get; set; }
        public string AdultPNR { get; set; }
        public string PaxName { get; set; }
        public string PaxIDNum { get; set; }
        public string PaxIDType { get; set; }
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
        public string GroupName { get; set; }
        public int PaxTransitDest { get; set; }
        public int FltID { get; set; }
        public int UserId { get; set; }
        public string Lang { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public string UpdatedActionSource { get; set; }
        public string ActionSource { get; set; }

    }



    public class CheckInDTO
    {
        public List<CheckIn> CheckIn { get; set; }
        public List<Aircraft> Aircraft { get; set; }
        public List<UserChekinDetails> UserChekinDetails { get; set; }
    }




    public class UserChekinDetails
    {
        public int RsvnID { get; set; }
        public int FltID { get; set; }
        public DateTime? PaxBDay { get; set; }
        public string PaxNationality { get; set; }
        public string ageMonths { get; set; }
        public string RsvnPNR { get; set; }
        public int PaxDestination { get; set; }
        public string PaxName { get; set; }
        public string RsvnStatus { get; set; }
        public string SeatNum { get; set; }
    }



    public class CheckinCardDetails
    {

        public int RsvnID { get; set; }
        public DateTime BookingTime { get; set; }
        public string BookingPNR { get; set; }
        public string Name { get; set; }
        public string FromDestination { get; set; }
        public string FromDestinationArabic { get; set; }
        public string ToDestination { get; set; }
        public string ToDestinationArabic { get; set; }
        public string FltNumber { get; set; }
        public DateTime FltDateTime { get; set; }
        public string PaxName { get; set; }
        public string CountryName { get; set; }
        public string CountryNameArabic { get; set; }
        public string PaxGender { get; set; }
        public string PaxMobNum { get; set; }
        public string RsvEMail { get; set; }
        public string RsvnStatus { get; set; }

    }


}
