using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{
    [Table("AbsenceDetails", Schema = "FileShare")]
    public class AbsenceDetail : OutdoorDetail
    {
        public int AbsenceTimes { get; set; }
        public CommandItem commandItem { get; set; }

    }
}