using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAClassLibrary
{
    public class Register
    {
        public int Id { get; set; }
        public bool IsHead { get; set; }
        public int UserId { get; set; }
        public string Honorifics { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Nationality { get; set; }
        public string NationalityArabic { get; set; }
        public string NationalityCode { get; set; }
        public DateTime Birthdate { get; set; }
        public string Documents { get; set; }
        public List<string> DocumentType { get; set; }
        public string Mobile { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool Active { get; set; }
        public bool IsDelmaIsland { get; set; }
        public bool IsUAEId { get; set; }
        public List<string> FileName { get; set; }
        public bool GovEntity { get; set; }
        public bool IsApproved { get; set; }
        public string DocumentTypetxt { get; set; }
        public string FilePathNametxt { get; set; }
        public string FlightDestinationName { get; set; }
        public string FlightDestinationNameArabic { get; set; }
        public int DestinationId { get; set; }
        public string MemberType { get; set; }
        public List<RegisterGroup> Groups { get; set; }

        public List<RegisterCorporate> registerCorporate { get; set; }

   
    }

    public class RegisterGroup
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool IsHead { get; set; }
        public string Honorifics { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Nationality { get; set; }
        public DateTime Birthdate { get; set; }
        public string Documents { get; set; }
        public List<string> DocumentType { get; set; }
        public string Mobile { get; set; }
        public bool IsDelmaIsland { get; set; }
        public bool IsUAEId { get; set; }
        public bool IsApproved { get; set; }
        public List<string> FileName { get; set; }
      
    }


    public class RegisterCorporate
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool IsHead { get; set; }
        public string Honorifics { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Nationality { get; set; }
        public DateTime Birthdate { get; set; }
        public string Documents { get; set; }
        public List<string> DocumentType { get; set; }
        public List<string> FileName { get; set; }
        public string Mobile { get; set; }
        public bool IsDelmaIsland { get; set; }
        public bool IsUAEId { get; set; }
        public bool GovEntity { get; set; }
        public bool IsApproved { get; set; }
      

    }

    public class UserIdWithType{
        public int Id { get; set; }
        public string MemberType { get; set; }


    }

    public class CodeVerification
    {
        public int Id { get; set; }

        public string Email { get; set; }
        public string Mobile { get; set; }
        public string VerifyCode { get; set; }
        public string Password { get; set; }
        public string Action { get; set; }


    }
    public class TempClassErrors
    {
        public string Message { get; set; }

        public bool isExists { get; set; } = false;


    }



    public class GetFltIdsForGroupHead
    {

        public int FltID { get; set; }
        public int PaxDestination { get; set; }
        public string RsvnPNR { get; set; }
        public DateTime PaxBDay { get; set; }

    }


}
