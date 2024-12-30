using ElecWarSystem.Models.IModel;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{
    [Table("Prisons", Schema = "FileShare")]
    public class Prison : Outdoor, IDateLogic, ICloneable
    {
        public long PrisonDetailID { get; set; }
        [ForeignKey("PrisonDetailID")]
        public PrisonDetail PrisonDetails { get; set; }

        public object Clone()
        {
            Prison prison = new Prison();
            prison.PrisonDetailID = this.PrisonDetailID;
            prison.TmamID = this.TmamID;
            return prison;
        }

        public bool IsDateLogic()
        {
            bool result = PrisonDetails.DateFrom <= Tmam.Date &&
                            PrisonDetails.DateTo >= Tmam.Date;
            return result;
        }
    }
}