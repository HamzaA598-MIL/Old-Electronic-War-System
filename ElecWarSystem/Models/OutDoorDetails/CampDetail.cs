using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElecWarSystem.Models
{
    public class CampDetail : OutdoorDetail
    {
        public String CurrentExistance { get; set; } = String.Empty;
        public String Reason { get; set; } = String.Empty;
    }
}