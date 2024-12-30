using ElecWarSystem.Models.IModel;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{
    [Table("SickLeaves", Schema = "FileShare")]
    public class SickLeave : Outdoor, IDateLogic, ICleanNav,ICloneable
    {
        public long SickLeaveDetailID { get; set; }
        [ForeignKey("SickLeaveDetailID")]
        public SickLeavesDetails SickLeaveDetail { get; set; }

        public void CleanNav()
        {
            Tmam = null;
        }

        public object Clone()
        {
            SickLeave sickLeave = new SickLeave();
            sickLeave.SickLeaveDetailID = this.SickLeaveDetailID;
            sickLeave.TmamID= this.TmamID;
            return sickLeave;
        }

        public bool IsDateLogic()
        {
            bool result = SickLeaveDetail.DateFrom <= Tmam.Date &&
                            SickLeaveDetail.DateTo > Tmam.Date &&
                            SickLeaveDetail.HospitalDate <= Tmam.Date;
            return result;
        }
    }
}