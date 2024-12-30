using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ElecWarSystem.Models
{
    [Table("PersonStatus", Schema = "FileShare")]
    public class PersonStatus
    {
        [Key]
        public long ID { get; set; }
        public long TmamID { get; set; }
        [ForeignKey("TmamID")]
        public Tmam Tmam { get; set; }
        public long PersonID { get; set; }
        [ForeignKey("PersonID")]
        public Person Person { get; set; }
        public TmamEnum Status { get; set; }
    }
}