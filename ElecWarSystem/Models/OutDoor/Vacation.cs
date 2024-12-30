using ElecWarSystem.Models.IModel;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{
    [Table("Vacations", Schema = "FileShare")]
    public class Vacation : Outdoor , ICleanNav, IDateLogic, ICloneable
    {
        public long VacationDetailID { get; set; }
        [ForeignKey("VacationDetailID")]
        public VacationDetail VacationDetail { get; set; }

        public void CleanNav()
        {
            Tmam = null;
        }

        public object Clone()
        {
            Vacation vacation = new Vacation();
            vacation.VacationDetailID = this.VacationDetailID;
            vacation.TmamID = this.TmamID;
            return vacation;
        }

        public bool IsDateLogic()
        {
            bool result = VacationDetail.DateFrom <= Tmam.Date &&
                            VacationDetail.DateTo >= Tmam.Date;
            return result;
        }

    }
}