using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ElecWarSystem.Models
{
    [Table("OutOfCountryDetails", Schema = "FileShare")]
    public class OutOfCountryDetail : OutdoorDetail
    {
        public String Country { get; set; } = String.Empty;
        public String Puspose { get; set; } = String.Empty;
    }
}