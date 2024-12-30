using ElecWarSystem.Models.IModel;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{
    [Table("Absences", Schema = "FileShare")]
    public class Absence : Outdoor , ICloneable, IDateLogic
    {
        public long AbsenceDetailID { get; set; }
        [ForeignKey("AbsenceDetailID")]
        public AbsenceDetail AbsenceDetail { get; set; }

        public object Clone()
        {
            Absence absence = new Absence();
            absence.AbsenceDetailID = this.AbsenceDetailID;
            absence.TmamID = this.TmamID;
            return absence;
        }

        public bool IsDateLogic()
        {
            bool result = AbsenceDetail.DateFrom <= Tmam.Date && AbsenceDetail.DateTo >=Tmam.Date;
            return result;
        }
    }
}