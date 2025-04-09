using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAClassLibrary
{
    public class Cargo
    {
        public int FrtID { get; set; }
        public DateTime? DateTimeRcvd { get; set; }
        public DateTime? FlightDateTime { get; set; }
        public string FrtDesc { get; set; }
        public int FrtWt { get; set; }
        public string FrtDest { get; set; }
        public string FltNumber { get; set; }
        public string ShipperName { get; set; }
        public string ShipperRefNum { get; set; }
        public string FrtRemarks { get; set; }
        public int FrtSerial { get; set; }
        public int ReceiverID { get; set; }
        public int FltID { get; set; }
        public string ManifestColor { get; set; }
        public int ForwarderID { get; set; }
        public DateTime? ForwarderTimeStamp { get; set; }
        public string FrtStatus { get; set; }
        public string BarCode { get; set; }
        public int FrtPcs { get; set; }
        public string FrtDirection { get; set; }
        public int ArrID { get; set; }
        public int CreatedBy { get; set; }


    }

    public class CargoDestination
    {
        public int DestID { get; set; }
        public string DestICAO { get; set; }
        public string DestIATA { get; set; }
        public string DestName { get; set; }
        public int HomeBase { get; set; }
        public int OilField { get; set; }
        public int FieldDestID { get; set; }
        public int DestActive { get; set; }
        public string CustDestCode { get; set; }
        public string CustDestName { get; set; }
        public string FIDSName { get; set; }
        public bool FltDest { get; set; }



    }


    public class CargoDTOFillter
    {
        public DateTime? DateTimeRcvd { get; set; }

        public string FrtDirection { get; set; }

    }


    public class CargoGetFlightByDate
    {

        public int FltID { get; set; }
        public string Name { get; set; }
        public DateTime FlightDateTime { get; set; }

    }


    public class CargoFlightDetails
    {

        public int FltID { get; set; }
        public string Aircraft { get; set; }
        public int Payload { get; set; }
        public string FltColor { get; set; }
        public string SubManifestColor { get; set; }
        public DateTime FltDateTime { get; set; }

    }

    public class CargoMultiUpdate
    {

        public int FltID { get; set; }

        public int CreateBy { get; set; }

        public List<int> CargoIds { get; set; }

        public string MinfestColor { get; set; }
    }



    public class CargoStaff
    {
        public int StaffID { get; set; }
        public string EmpNum { get; set; }
        public string StaffSurname { get; set; }
        public string StaffName { get; set; }
        public string StaffPwd { get; set; }
        public bool StaffRight { get; set; }
        public bool StaffActive { get; set; }
        public bool SuperAdminRights { get; set; }
        public bool IsADUser { get; set; }
        public string StaffGrp { get; set; }
        public string StaffType { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }



    }


    public class CargoExportDataFilter
    {

        public int FrtID { get; set; }
        public string CargoStatus { get; set; }
        public string CargoDirection { get; set; }
        public DateTime? CargoFrom { get; set; }
        public DateTime? CargoTo { get; set; }
        public string FlightNumber { get; set; }
        public string AircraftRegistration { get; set; }
        public DateTime? DateTimeOfFlight { get; set; }
        public string Receiver { get; set; }
        public string Forwarder { get; set; }


    }

}
