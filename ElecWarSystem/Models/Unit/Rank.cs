using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{
    [Table("Rank", Schema = "FileShare")]
    public class Rank
    {
        [Key]
        public int ID { get; set; }
        public String RankName { get; set; }
        public int RankType { get; set; }
    }

}