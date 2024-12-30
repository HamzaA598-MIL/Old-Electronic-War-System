using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{
    [Table("Person", Schema = "FileShare")]
    public class Person
    {
        [Key]
        public long ID { get; set; }
        
        public string MilID { get; set; }
        public int UnitID { get; set; }
        [ForeignKey("UnitID")]
        public Unit Unit { get; set; }
        public int? RankID { get; set; }
        [ForeignKey("RankID")]
        public Rank Rank { get; set; }
        public String FullName { get; set; }
        public bool onDuty { get; set; } = true;
        public bool Status { get; set; } = false;
    }
}