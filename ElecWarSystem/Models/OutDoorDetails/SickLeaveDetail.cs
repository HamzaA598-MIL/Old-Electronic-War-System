using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{
    [Table("SickLeavesDetails", Schema = "FileShare")]
    public class SickLeavesDetails : OutdoorDetail
    {
        public String Hospital { get; set; } = String.Empty;
        [Column(TypeName = "datetime")]
        [DisplayFormat(DataFormatString = "{0:D}")]
        public DateTime HospitalDate { get; set; }
        public String Diagnosis { get; set; } = String.Empty;
    }
}