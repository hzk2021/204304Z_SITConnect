using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _204304Z_SITConnect.Models
{
    public class GObject
    {

        public string success { get; set; }
        public decimal score { get; set; }
        public string action { get; set; }
        public DateTime challenge_ts { get; set; }
        public string hostname { get; set; }
        public List<string> errorCodes { get; set; }
    }
}