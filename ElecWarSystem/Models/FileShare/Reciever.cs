using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{
    [Table("Reciever", Schema = "FileShare")]
    public class Reciever
    {
        [Key]
        public long ID { get; set; }
        public long EmailID { get; set; }
        [ForeignKey("EmailID")]
        public Email Email { get; set; }
        public int RecieverID { get; set; }
        [ForeignKey("RecieverID")]
        public Unit RecieverUser { get; set; }
        public bool Readed { get; set; } = false;
        public bool Starred { get; set; } = false;
        public DateTime? ViewTime { get; set; }
}
}