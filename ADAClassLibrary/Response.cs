using System;
using System.Collections.Generic;
using System.Text;

namespace ADAClassLibrary
{
    public class Response
    {
        public int Status { get; set; }
        public string ResponseMsg { get; set; }
        public object Data { get; set; }
        public string Token { get; set; }

    }
}
