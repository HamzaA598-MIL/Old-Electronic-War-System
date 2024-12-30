using ElecWarSystem.Models.IModel;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{
    [Table("Errand", Schema = "FileShare")]
    public class Errand : Outdoor, IDateLogic, ICloneable
    {
        public long ErrandDetailID { get; set; }
        [ForeignKey("ErrandDetailID")]
        public ErrandDetail ErrandDetail { get; set; }

        public object Clone()
        {
            Errand errand = new Errand();
            errand.ErrandDetailID = this.ErrandDetailID;
            errand.TmamID = this.TmamID;
            return errand;
        }

        public bool IsDateLogic()
        {
            bool result = ErrandDetail.DateFrom <= Tmam.Date &&
                            ErrandDetail.DateTo >= Tmam.Date;
            return result;
        }
    }
}