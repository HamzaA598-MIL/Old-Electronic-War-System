using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{
    [Table("VacationDetails", Schema = "FileShare")]
    public class VacationDetail : OutdoorDetail
    {
        public String VacationType { get; set; }
    }
}