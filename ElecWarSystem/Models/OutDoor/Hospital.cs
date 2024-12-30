using ElecWarSystem.Models.IModel;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{
    [Table("Hospital", Schema = "FileShare")]
    public class Hospital : Outdoor, IDateLogic, ICleanNav, ICloneable
    {
        public long HospitalDetailID { get; set; }
        [ForeignKey("HospitalDetailID")]
        public HospitalDetails HospitalDetails { get; set; }

        public void CleanNav()
        {
            Tmam = null;
        }

        public object Clone()
        {
            Hospital hospital = new Hospital();
            hospital.HospitalDetailID = this.HospitalDetailID;
            hospital.TmamID = this.TmamID;
            return hospital;
        }

        public bool IsDateLogic()
        {
            bool result = HospitalDetails?.DateFrom <= Tmam?.Date;
            return result;
        }
    }
}