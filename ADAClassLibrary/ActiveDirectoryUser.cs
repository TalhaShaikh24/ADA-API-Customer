using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAClassLibrary
{
   public class ActiveDirectoryUser
    {
            public int? StaffID { get; set; }
            public string EmpNum { get; set; }
            public string StaffSurname { get; set; }
            public string StaffName { get; set; }
            public string StaffPwd { get; set; }
            public string StaffGrp { get; set; }
            public string StaffType { get; set; }
            public bool StaffRights { get; set; }
            public bool StaffActive { get; set; }
            public bool SuperAdminRights { get; set; }

            public string UserName { get; set; }
            public string Email { get; set; }
            public bool IsADUser { get; set; }



    }

}
