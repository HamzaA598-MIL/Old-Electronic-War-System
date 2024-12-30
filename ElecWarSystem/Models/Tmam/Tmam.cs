using ElecWarSystem.Models.OutDoor;
using ElecWarSystem.Serivces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{
    [Table("Tmam", Schema = "FileShare")]
    public class Tmam : ICloneable
    {
        internal readonly DateTimeOffset? TmamDate;

        [Key]
        public long ID { get; set; }
        [Column(TypeName = "datetime")]
        [DisplayFormat(DataFormatString = "{0:D}")]
        public DateTime Date { get; set; }
        public int UnitID { get; set; }
        [ForeignKey("UnitID")]
        public Unit Unit { get; set; }
        //AltCommander property ==> ka2ed Manoop
        public long? AltCommanderID { get; set; }
        [ForeignKey("AltCommanderID")]
        public Person AltCommander { get; set; }
        public bool Submitted { get; set; } = false;
        public bool Recieved { get; set; } = false;
        public String TimeSended { get; set; } = "-";
        public List<TmamDetail> TmamDetails { get; set; } = new List<TmamDetail>();
        public List<SickLeave> SickLeaves { get; set; } = new List<SickLeave>();
        public List<Errand> Errands { get; set; } = new List<Errand>();
        public List<Vacation> Vacations { get; set; } = new List<Vacation>();
        public List<Prison> prisons { get; set; } = new List<Prison>();
        public List<Absence> Absences { get; set; } = new List<Absence>();
        public List<Hospital> Hospitals { get; set; } = new List<Hospital>();
        public List<OutOfCountry> OutOfCountries { get; set; } = new List<OutOfCountry>();
        public List<Camp> Camps { get; set; } = new List<Camp>();
        public List<Course> Courses { get; set; } = new List<Course>();


        public object Clone()
        {
            Tmam tmam = new Tmam() { UnitID = this.UnitID, Date = DateTime.Today.AddDays(1) };
            this.Date = tmam.Date;
            PersonStatusService personStatusService = new PersonStatusService();
            foreach (TmamDetail tmamDetail in this.TmamDetails)
            {
                
                tmam.TmamDetails.Add((TmamDetail)tmamDetail.Clone());
            }
            //each tmam-absence-vacation-errand-sickleave ==> clone function making a copy of yesterday tmam
            // and each unit can update its tmam 
            foreach (SickLeave sickLeave in this.SickLeaves)
            {
                if (sickLeave.IsDateLogic())
                {
                    tmam.SickLeaves.Add((SickLeave)sickLeave.Clone());
                }
            }

            foreach (Absence absence in this.Absences)
            {
                if (absence.IsDateLogic())
                {
                    tmam.Absences.Add((Absence)absence.Clone());
                }
            }

            foreach (Errand errand in this.Errands)
            {
                //if (errand.IsDateLogic())
                //{
                //    tmam.Errands.Add((Errand)errand.Clone());
                //}
                tmam.Errands.Add((Errand)errand.Clone());
            }

            foreach (Vacation vacation in this.Vacations)
            {
                if (vacation.IsDateLogic())
                {
                    tmam.Vacations.Add((Vacation)vacation.Clone());
                }
            }

            foreach (Prison prison in this.prisons)
            {
                //if (prison.IsDateLogic())
                //{
                //}
                tmam.prisons.Add((Prison)prison.Clone());

            }
            foreach (Hospital hospital in this.Hospitals)
            {
                if (hospital.IsDateLogic())
                {
                    tmam.Hospitals.Add((Hospital)hospital.Clone());
                }
            }
            foreach (OutOfCountry outOfCountry in this.OutOfCountries)
            {
                if (outOfCountry.IsDateLogic())
                {
                    tmam.OutOfCountries.Add((OutOfCountry)outOfCountry.Clone());
                }
            }
            foreach (Camp camp in this.Camps)
            {
                if (camp.IsDateLogic())
                {
                    tmam.Camps.Add((Camp)camp.Clone());
                }
            }
            foreach (Course course in this.Courses)
            {
                if (course.IsDateLogic())
                {
                    tmam.Courses.Add((Course)course.Clone());
                }
            }
            return tmam;
        }
    }
}