using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{
    public class Outdoor
    {
        [Key]
        public long ID { get; set; }
        public long TmamID { get; set; }
        [ForeignKey("TmamID")]
        public Tmam Tmam { get; set; }
    }
}