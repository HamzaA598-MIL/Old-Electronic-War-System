using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{
    [Table("Email", Schema = "FileShare")]
    public class Email
    {
        public long ID { get; set; }
        public int SenderUserID { get; set; }
        [ForeignKey("SenderUserID")]
        public Unit Sender { get; set; }
        public String Subject { get; set; }
        public String EmailText { get; set; }
        [Column(TypeName = "datetime")]
        [DisplayFormat(DataFormatString = "{0:D}")]
        public DateTime SendDateTime { get; set; }
        public Email()
        {
            SendDateTime = DateTime.Now; // التوقيت المحلي للسيرفر
        }
        public List<Reciever> Recievers { get; set; } = new List<Reciever>();
        public List<Document> Documents { get; set; } = new List<Document>();
    }
}