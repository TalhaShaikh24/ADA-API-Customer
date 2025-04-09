using System;
using System.Collections.Generic;
using System.Text;

namespace ADAClassLibrary
{
    public class Pagination
    {
        public string draw { get; set; }
        public int recordsFiltered { get; set; }
        public int recordsTotal { get; set; }
        public object Data { get; set; }
        public int Status { get; set; }
        public string ResponseMsg { get; set; }
        public string Token { get; set; }
    }
}
