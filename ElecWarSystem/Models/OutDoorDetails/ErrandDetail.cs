using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{
    [Table("ErrandDetails", Schema = "FileShare")]
    public class ErrandDetail : OutdoorDetail
    {
        public String ErrandPlace { get; set; } = String.Empty;
        public String ErrandCommandor { get; set; } = String.Empty;
    }
}