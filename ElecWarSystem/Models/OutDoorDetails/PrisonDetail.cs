using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{
    [Table("PrisonDetails", Schema = "FileShare")]
    public class PrisonDetail : OutdoorDetail
    {
        public String Crime { get; set; } = String.Empty;
        public String Punishment { get; set; } = String.Empty;
        public String Punisher { get; set; } = String.Empty;
        public String PrisonPlace { get; set; } = String.Empty;
        public long CommandItemID { get; set; }
        [ForeignKey("CommandItemID")]
        public CommandItem CommandItem { get; set; }
    }
}