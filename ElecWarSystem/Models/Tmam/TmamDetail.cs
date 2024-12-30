using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{
    [Table("TmamDetails", Schema = "FileShare")]
    public class TmamDetail : ICloneable
    {
        [Key]
        public long ID { get; set; }
        public long TmamID { get; set; }
        [ForeignKey("TmamID")]
        public Tmam Tmam { get; set; }
        public bool IsOfficers { get; set; }
        public int totalPower { get; set; } = 0;
        public int errand { get; set; } = 0;
        public int vacation { get; set; } = 0;
        public int sickLeave { get; set; } = 0;
        public int prison { get; set; } = 0;
        public int absence { get; set; } = 0;
        public int hospital { get; set; } = 0;
        public int outOfCountry { get; set; } = 0;
        public int outdoorCamp { get; set; } = 0;
        public int escape { get; set; } = 0;
        public int course { get; set; } = 0;
        //return total number of people not existting in the militry unit
        public int GetOutting()
        {
            return errand + vacation + sickLeave + prison + absence + hospital + outOfCountry + outdoorCamp + escape + course;
        }
        public int GetExisting()
        {
            return totalPower - GetOutting();
        }
        public int GetOuttingPrecetage()
        {
            int outs = this.GetOutting();
            float prec;
            if (this.totalPower == 0)
            {
                prec = 0;
            }
            else
            {
                prec = ((float)outs / (float)this.totalPower) * 100;

            }
            return (int)prec;
        }

        public object Clone()
        {
            TmamDetail tmamDetail = new TmamDetail();
            tmamDetail.IsOfficers = this.IsOfficers;
            tmamDetail.hospital = this.hospital;
            tmamDetail.totalPower = this.totalPower;
            tmamDetail.errand = this.errand;
            tmamDetail.absence = this.absence;
            tmamDetail.sickLeave = this.sickLeave;
            tmamDetail.vacation = this.vacation;
            tmamDetail.prison = this.prison;
            tmamDetail.outOfCountry = this.outOfCountry;
            tmamDetail.outdoorCamp = this.outdoorCamp;
            tmamDetail.escape = this.escape;
            tmamDetail.course = this.course;
            return tmamDetail;
        }
    }
}