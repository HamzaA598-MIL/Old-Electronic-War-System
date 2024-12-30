using ElecWarSystem.Models;
using ElecWarSystem.Models.OutDoor;
using ElecWarSystem.Models.OutDoorDetails;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace ElecWarSystem.Data
{
    public class AppDBContext : DbContext
    {

        
        public AppDBContext() : base("name = ElectronicWarDB") { }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<Reciever> Recievers { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<SmallUnit> SmallUnits { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Rank> Ranks { get; set; }
        public DbSet<Tmam> Tmams { get; set; }
        public DbSet<TmamDetail> TmamDetails { get; set; }
        public DbSet<SickLeavesDetails> SickLeavesDetails { get; set; }
        public DbSet<SickLeave> SickLeaves { get; set; }
        public DbSet<ErrandDetail> ErrandDetails { get; set; }
        public DbSet<Errand> Errands { get; set; }
        public DbSet<CommandItem> CommandItems { get; set; }
        public DbSet<Prison> Prisons { get; set; }
        public DbSet<PrisonDetail> PrisonDetails { get; set; }
        public DbSet<Vacation> Vacations { get; set; }
        public DbSet<VacationDetail> VacationDetails { get; set; }
        public DbSet<Absence> Absences { get; set; }
        public DbSet<AbsenceDetail> AbsenceDetails { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<HospitalDetails> HospitalDetails { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<OutOfCountry> OutOfCountries { get; set; }
        public DbSet<OutOfCountryDetail> OutOfCountryDetails { get; set; }
        public DbSet<Camp> Camps { get; set; }
        public DbSet<CampDetail> CampDetails { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseDetails> CourseDetails { get; set; }
        public DbSet<PersonStatus> PersonStatus { get; set; }


    }
}

/*
 <add name="ElectronicWarDBLocal" connectionString="Data Source=DESKTOP-L9CA1CG; Initial Catalog=ElecWarDB;Integrated Security=True"
			providerName="System.Data.SqlClient"/>
 */