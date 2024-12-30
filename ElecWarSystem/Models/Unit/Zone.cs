using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{
    [Table("Zone", Schema = "FileShare")]
    public class Zone
    {
        [Key]
        [Display(Name = "ID")]
        public int ID { get; set; }

        public String ZoneName { get; set; } = String.Empty;
        public String ZoneAlias { get; set; } = String.Empty;
        public int Order { get; set; }
        public List<Unit> Units { get; set; } = new List<Unit>();
    }
}