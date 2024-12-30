using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{

    [Table("CommandItems", Schema = "FileShare")]
    public class CommandItem
    {
        [Key]
        public long ID { get; set; }
        public int Number { get; set; } = 0;
        [Column(TypeName = "datetime")]
        [DisplayFormat(DataFormatString = "{0:D}")]
        public DateTime Date { get; set; }
    }
}