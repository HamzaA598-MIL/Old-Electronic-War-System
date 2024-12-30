using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{
    public class OutdoorDetail
    {
        [Key]
        public long ID { get; set; }
        public long PersonID { get; set; }
        [ForeignKey("PersonID")]
        public Person Person { get; set; }
        [Column(TypeName = "datetime")]
        [DisplayFormat(DataFormatString = "{0:D}")]
        public DateTime DateFrom { get; set; }
        [Column(TypeName = "datetime")]
        [DisplayFormat(DataFormatString = "{0:D}")]
        public DateTime DateTo { get; set; } = DateTime.Today.AddDays(1);
    }
}