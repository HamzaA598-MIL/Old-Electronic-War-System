using ElecWarSystem.Data;
using ElecWarSystem.Models;
using ElecWarSystem.Models.OutDoor;
using ElecWarSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;

namespace ElecWarSystem.Serivces
{
    public class TmamGatheringService
    {
        private readonly AppDBContext appDBContext;
        private readonly TmamService tmamService;
        private readonly UserService userService;
        private readonly ZoneService zoneService;
        private readonly UnitService unitService;
        private DateTime dateTime;
        public TmamGatheringService(DateTime? date = null)
        {
            appDBContext = new AppDBContext();
            userService = new UserService();
            zoneService = new ZoneService();
            unitService = new UnitService();
            if (date != null)
            {
                dateTime = (DateTime)date;
            }
            else
            {
                dateTime = DateTime.Today.AddDays(1);
            }
            tmamService = new TmamService(dateTime);

        }
        private Dictionary<String, LeaderTmamView> GetLeaderTmamPerUnit(int unitID)
        {
            Dictionary<String, LeaderTmamView> leaderTmamDic = new Dictionary<string, LeaderTmamView>();
            Tmam tmam = tmamService.GetTmam(unitID);
            if (tmam != null && tmam.Submitted && tmam.Recieved)
            {
                Unit unit = userService.GetUnit(unitID);
                if (unit.UCID != null && unit.UOCHID != null)
                {
                    leaderTmamDic["UC-0"] = new LeaderTmamView(tmam.ID, (long)unit.UCID);
                    leaderTmamDic["UOC-0"] = new LeaderTmamView(tmam.ID, (long)unit.UOCHID);
                    int i = 1;
                    foreach (SmallUnit smallUnit in unit.SmallUnits)
                    {
                        leaderTmamDic[$"UC-{i}"] = new LeaderTmamView(tmam.ID, (long)smallUnit.UCID);
                        leaderTmamDic[$"UOC-{i}"] = new LeaderTmamView(tmam.ID, (long)smallUnit?.UOCHID);
                        i++;
                    }
                }
            }
            return leaderTmamDic;
        }
        public Dictionary<String, Dictionary<String, Dictionary<String, LeaderTmamView>>> GetAllLeaderTmam()
        {
            List<Zone> zones = zoneService.GetZones();
            var zoneUnitsTmam = new Dictionary<string, Dictionary<string, Dictionary<string, LeaderTmamView>>>();

            foreach (Zone zone in zones)
            {
                
                List<Unit> unitsPerZone = unitService.GetByZone(zone.ID)
                    .OrderBy(u => u.Order) 
                    .ToList();

                var leaderTmams = new Dictionary<string, Dictionary<string, LeaderTmamView>>();
                foreach (Unit unit in unitsPerZone)
                {
                    leaderTmams[unit.UnitName] = GetLeaderTmamPerUnit(unit.ID);
                }
                zoneUnitsTmam[zone.ZoneName] = leaderTmams;
            }

            return zoneUnitsTmam;
        }

        public Dictionary<String, Dictionary<String, Person>> GetAllAltCommandor()
        {
            List<Zone> zones = zoneService.GetZones();
            var zoneUnitsTmam = new Dictionary<string, Dictionary<string, Person>>();
            foreach (Zone zone in zones)
            {
                List<Unit> unitsPerZone = unitService.GetByZone(zone.ID).OrderBy(u => u.Order)
                    .ToList();
                    
                var altComTmams = new Dictionary<string, Person>();
                foreach (Unit unit in unitsPerZone)
                {
                    Tmam tmam = tmamService.GetTmamSubmitted(unit.ID);
                    Person altCommander = tmam?.AltCommander;
                    if (altCommander != null)
                    {
                        altComTmams[unit.UnitName] = altCommander;
                    }
                }
                zoneUnitsTmam[zone.ZoneName] = altComTmams;
            }
            return zoneUnitsTmam;
        }

        public List<Tmam> GetTmamsSubmitted()
        {
            List<Tmam> ListOfSubmittedTmams = appDBContext.Tmams
                .Include("Unit")
                .Where(row => row.Submitted && row.Date == dateTime)
                .OrderBy(row => row.Unit.Order)
                .ToList();
            return ListOfSubmittedTmams;
        }

        public Dictionary<String, List<TmamDetail>> GetOfficersTmam(bool IsOfficers)
        {
            List<Zone> zones = zoneService.GetZones();
            Dictionary<String, List<TmamDetail>> zoneUnitsTmam = new Dictionary<string, List<TmamDetail>>();
            foreach (Zone zone in zones)
            {
                List<Unit> unitsPerZone = unitService.GetByZone(zone.ID).OrderBy(u => u.Order)
                    .ToList();
                List<TmamDetail> tmamDetails1 = new List<TmamDetail>();
                foreach (Unit unit in unitsPerZone)
                {
                    TmamDetail tmamDetail = tmamService.GetTmamDetailOrDefault(unit.ID, IsOfficers);
                    tmamDetails1.Add(tmamDetail);
                }
                zoneUnitsTmam[zone.ZoneName] = tmamDetails1;
            }
            return zoneUnitsTmam;
        }

        public Dictionary<String, Dictionary<String, List<SickLeave>>> GetSickLeavesTmam()
        {
            List<Zone> zones = zoneService.GetZones();
            var zoneUnitsTmam = new Dictionary<String, Dictionary<String, List<SickLeave>>>();

            foreach (Zone zone in zones)
            {
                var sickLeavesDic = new Dictionary<String, List<SickLeave>>();
                foreach (Unit unit in unitService.GetByZone(zone.ID))
                {
                    long tmamId = tmamService.GetTmamIDToday(unit.ID);
                    if (tmamId > 0)
                    {
                        var sickLeavePerUnit = appDBContext.SickLeaves
                            .Include("SickLeaveDetail.Person.Rank")
                            .Include("Tmam")
                            .Where(row => row.TmamID == tmamId &&
                                        row.Tmam.Submitted &&
                                        row.Tmam.Recieved)
                            .OrderBy(row => row.SickLeaveDetail.Person.RankID)
                            .ToList<SickLeave>();
                        if (sickLeavePerUnit.Count > 0)
                        {
                            sickLeavesDic[unit.UnitName] = sickLeavePerUnit;
                        }
                    }
                }
                if (sickLeavesDic.Count > 0)
                {
                    zoneUnitsTmam[zone.ZoneName] = sickLeavesDic;
                }
            }
            return zoneUnitsTmam;
        }
        public Dictionary<String, Dictionary<String, List<OutOfCountry>>> GetOutOfCountriesTmam()
        {
            List<Zone> zones = zoneService.GetZones();
            var zoneUnitsTmam = new Dictionary<String, Dictionary<String, List<OutOfCountry>>>();

            foreach (Zone zone in zones)
            {
                var outOfCountryDic = new Dictionary<String, List<OutOfCountry>>();
                foreach (Unit unit in unitService.GetByZone(zone.ID))
                {
                    long tmamId = tmamService.GetTmamIDToday(unit.ID);
                    if (tmamId > 0)
                    {
                        var OutOfCountryPerUnit = appDBContext.OutOfCountries
                            .Include("OutOfCountryDetail.Person.Rank")
                            .Include("Tmam")
                            .Where(row => row.TmamID == tmamId &&
                                        row.Tmam.Submitted &&
                                        row.Tmam.Recieved)
                            .OrderBy(row => row.OutOfCountryDetail.Person.RankID)
                            .ToList<OutOfCountry>();
                        if (OutOfCountryPerUnit.Count > 0)
                        {
                            outOfCountryDic[unit.UnitName] = OutOfCountryPerUnit;
                        }
                    }
                }
                if (outOfCountryDic.Count > 0)
                {
                    zoneUnitsTmam[zone.ZoneName] = outOfCountryDic;
                }
            }
            return zoneUnitsTmam;
        }
        public Dictionary<String, Dictionary<String, List<Errand>>> GetErrandsTmam()
        {
            List<Zone> zones = zoneService.GetZones();
            var zoneUnitsTmam = new Dictionary<String, Dictionary<String, List<Errand>>>();

            foreach (Zone zone in zones)
            {
                var ErrandsDic = new Dictionary<String, List<Errand>>();
                foreach (Unit unit in unitService.GetByZone(zone.ID))
                {
                    long tmamId = tmamService.GetTmamIDToday(unit.ID);
                    var ErrandPerUnit = appDBContext.Errands
                                .Include("ErrandDetail.Person.Rank")
                                .Include("Tmam")
                                .Where(row => row.TmamID == tmamId &&
                                        row.Tmam.Submitted &&
                                        row.Tmam.Recieved)
                                .OrderBy(row => row.ErrandDetail.Person.RankID)
                                .ToList<Errand>();
                    if (ErrandPerUnit.Count > 0)
                    {
                        ErrandsDic[unit.UnitName] = ErrandPerUnit;
                    }
                }
                if (ErrandsDic.Count > 0)
                {
                    zoneUnitsTmam[zone.ZoneName] = ErrandsDic;
                }
            }
            return zoneUnitsTmam;
        }
        public Dictionary<String, Dictionary<String, List<Prison>>> GetPrisonsTmam()
        {
            List<Zone> zones = zoneService.GetZones();
            var zoneUnitsTmam = new Dictionary<String, Dictionary<String, List<Prison>>>();

            foreach (Zone zone in zones)
            {
                var PrisonsDic = new Dictionary<String, List<Prison>>();
                foreach (Unit unit in unitService.GetByZone(zone.ID))
                {
                    long tmamId = tmamService.GetTmamIDToday(unit.ID);
                    var PrisonPerUnit = appDBContext.Prisons
                                .Include("PrisonDetails.Person.Rank")
                                .Include("PrisonDetails.CommandItem")
                                .Include("Tmam")
                                .Where(row => row.TmamID == tmamId &&
                                            row.Tmam.Submitted &&
                                            row.Tmam.Recieved)
                                .OrderBy(row => row.PrisonDetails.Person.RankID)
                                .ToList<Prison>();
                    if (PrisonPerUnit.Count > 0)
                    {
                        PrisonsDic[unit.UnitName] = PrisonPerUnit;
                    }
                }
                if (PrisonsDic.Count > 0)
                {
                    zoneUnitsTmam[zone.ZoneName] = PrisonsDic;
                }
            }
            return zoneUnitsTmam;
        }
        public Dictionary<String, Dictionary<String, List<Absence>>> GetAbsencesTmam()
        {
            List<Zone> zones = zoneService.GetZones();
            var zoneUnitsTmam = new Dictionary<String, Dictionary<String, List<Absence>>>();

            foreach (Zone zone in zones)
            {
                var AbsencesDic = new Dictionary<String, List<Absence>>();
                foreach (Unit unit in unitService.GetByZone(zone.ID))
                {
                    long tmamId = tmamService.GetTmamIDToday(unit.ID);
                    var AbsencePerUnit = appDBContext.Absences
                                .Include("AbsenceDetail.Person.Rank")
                                .Include("AbsenceDetail.CommandItem")
                                .Include("Tmam")
                                .Where(row => row.TmamID == tmamId &&
                                            row.Tmam.Submitted &&
                                            row.Tmam.Recieved)
                                .OrderBy(row => row.AbsenceDetail.Person.RankID)
                                .ToList<Absence>();
                    if (AbsencePerUnit.Count > 0)
                    {
                        AbsencesDic[unit.UnitName] = AbsencePerUnit;
                    }
                }
                if (AbsencesDic.Count > 0)
                {
                    zoneUnitsTmam[zone.ZoneName] = AbsencesDic;
                }
            }
            return zoneUnitsTmam;
        }
        public Dictionary<String, Dictionary<String, List<Hospital>>> GetHospitalsTmam()
        {
            List<Zone> zones = zoneService.GetZones();
            var zoneUnitsTmam = new Dictionary<String, Dictionary<String, List<Hospital>>>();

            foreach (Zone zone in zones)
            {
                var HospitalsDic = new Dictionary<String, List<Hospital>>();
                foreach (Unit unit in unitService.GetByZone(zone.ID))
                {
                    long tmamId = tmamService.GetTmamIDToday(unit.ID);
                    var HospitalPerUnit = appDBContext.Hospitals
                                .Include("HospitalDetails.Person.Rank")
                                .Include("Tmam")
                                .Where(row => row.TmamID == tmamId &&
                                            row.Tmam.Submitted &&
                                            row.Tmam.Recieved)
                                .OrderBy(row => row.HospitalDetails.Person.RankID)
                                .ToList<Hospital>();
                    if (HospitalPerUnit.Count > 0)
                    {
                        HospitalsDic[unit.UnitName] = HospitalPerUnit;
                    }
                }
                if (HospitalsDic.Count > 0)
                {
                    zoneUnitsTmam[zone.ZoneName] = HospitalsDic;
                }
            }
            return zoneUnitsTmam;
        }
        public Dictionary<String, Dictionary<String, List<Camp>>> GetCampsTmam()
        {
            List<Zone> zones = zoneService.GetZones();
            var zoneUnitsTmam = new Dictionary<String, Dictionary<String, List<Camp>>>();

            foreach (Zone zone in zones)
            {
                var CampsDic = new Dictionary<String, List<Camp>>();
                foreach (Unit unit in unitService.GetByZone(zone.ID))
                {
                    long tmamId = tmamService.GetTmamIDToday(unit.ID);
                    var CampPerUnit = appDBContext.Camps
                                .Include("CampDetail.Person.Rank")
                                .Include("Tmam")
                                .Where(row => row.TmamID == tmamId &&
                                            row.Tmam.Submitted &&
                                            row.Tmam.Recieved)
                                .OrderBy(row => row.CampDetail.Person.RankID)
                                .ToList<Camp>();
                    if (CampPerUnit.Count > 0)
                    {
                        CampsDic[unit.UnitName] = CampPerUnit;
                    }
                }
                if (CampsDic.Count > 0)
                {
                    zoneUnitsTmam[zone.ZoneName] = CampsDic;
                }
            }
            return zoneUnitsTmam;
        }
        public Dictionary<String, Dictionary<String, List<Course>>> GetCoursesTmam()
        {
            List<Zone> zones = zoneService.GetZones();
            var zoneUnitsTmam = new Dictionary<String, Dictionary<String, List<Course>>>();

            foreach (Zone zone in zones)
            {
                var CoursesDic = new Dictionary<String, List<Course>>();
                foreach (Unit unit in unitService.GetByZone(zone.ID))
                {
                    long tmamId = tmamService.GetTmamIDToday(unit.ID);
                    var CoursePerUnit = appDBContext.Courses
                                .Include("CourseDetails.Person.Rank")
                                .Include("Tmam")
                                .Include("CourseDetails.CommandItem")
                                .Where(row => row.TmamID == tmamId &&
                                            row.Tmam.Submitted &&
                                            row.Tmam.Recieved)
                                .OrderBy(row => row.CourseDetails.Person.RankID)
                                .ToList<Course>();
                    if (CoursePerUnit.Count > 0)
                    {
                        CoursesDic[unit.UnitName] = CoursePerUnit;
                    }
                }
                if (CoursesDic.Count > 0)
                {
                    zoneUnitsTmam[zone.ZoneName] = CoursesDic;
                }
            }
            return zoneUnitsTmam;
        }
    }
}