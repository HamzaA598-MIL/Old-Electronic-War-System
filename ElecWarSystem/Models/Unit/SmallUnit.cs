using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{
    [Table("SmallUnit", Schema = "FileShare")]
    public class SmallUnit
    {
        [Key]
        public int ID { get; set; }
        public int ParentUnitID { get; set; }
        [ForeignKey("ParentUnitID")]
        public Unit ParentUnit { get; set; }
        public String UnitName { get; set; }
        public long? UCID { get; set; }
        [ForeignKey("UCID")]
        public Person UnitCommandor { get; set; }
        public long? UOCHID { get; set; }
        [ForeignKey("UOCHID")]
        public Person UnitOperationsChief { get; set; }
    }
}