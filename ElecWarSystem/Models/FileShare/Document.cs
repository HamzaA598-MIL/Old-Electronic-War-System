using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{
    [Table("Document", Schema = "FileShare")]
    public class Document
    {
        [Key]
        public long ID { get; set; }
        public long EmailID { get; set; }
        [ForeignKey("EmailID")]
        public Email Email { get; set; }
        public String FileName { get; set; } = String.Empty;
        public String FileExtension { get; set; } = String.Empty;
        public String FilePath { get; set; } = String.Empty;
    }
}