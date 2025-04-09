using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAClassLibrary
{
   public  class Role
    {

        public int? Id { get; set; }
        public string RoleName { get; set; }
        public bool Active { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? CreatedBy { get; set; }
    }
}
