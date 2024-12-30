using ElecWarSystem.Data;
using ElecWarSystem.Models;
using ElecWarSystem.Models.OutDoor;
using ElecWarSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using Camp = ElecWarSystem.Models.Camp;
using OutOfCountry = ElecWarSystem.Models.OutOfCountry;
using PersonStatus = ElecWarSystem.Models.PersonStatus;

namespace ElecWarSystem.Serivces
{
    public class TmamService
    {
        private readonly AppDBContext dBContext;
        private readonly PersonService personService;
        private readonly UnitService unitService;
        private readonly UserService userService;
        private readonly PersonStatusService personStatusService;
        private DateTime dateTime;
        public TmamService(DateTime? date = null)
        {
            dBContext = new AppDBContext();
            personService = new PersonService();
            unitService = new UnitService();
            userService = new UserService();
            personStatusService = new PersonStatusService();
            if (date != null)
            {
                dateTime = (DateTime)date;
            }
            else
            {
                dateTime = DateTime.Today.AddDays(1);
            }
        }
        public Tmam GetTmam(long id)
        {
            return dBContext.Tmams.Find(id);
        }
        public long GetTmamIDToday(int unitID)
        {
            Tmam tmam = GetTmam(unitID);
            return tmam?.ID ?? 0;
        }
        public Person GetAltCommandorToday(int unitID)
        {
            Tmam tmam = GetTmam(unitID);
            Person altCommandor = tmam?.AltCommander ?? new Person() { ID = 0, RankID = 0 };
            return altCommandor;
        }
        public long AddTmam(Tmam tmam)
        {
            tmam.ID = GetTmamID(tmam);
            if (tmam.ID == 0)
            {
                dBContext.Tmams.Add(tmam);
                dBContext.SaveChanges();
                personService.ResetPersonsStatus(tmam.UnitID);
            }
            if (tmam.AltCommanderID != null)
            {
                Tmam tmam1 = dBContext.Tmams.Find(tmam.ID);
                tmam1.AltCommanderID = tmam.AltCommanderID;
                dBContext.SaveChanges();
            }
            return tmam.ID;
        }

        public void DeleteTmam(Tmam tmam)
        {
            try
            {
                dBContext.Tmams.Remove(tmam);
                dBContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
        public long GetTmamID(Tmam tmam)
        {
            Tmam tmamTemp = dBContext.Tmams.
                FirstOrDefault(row => row.UnitID == tmam.UnitID && row.Date == tmam.Date);

            return tmamTemp?.ID ?? 0;
        }
        public Tmam GetTmam(int unitID)
        {
            Tmam tmam = dBContext.Tmams.
                Include("AltCommander.Rank").
                FirstOrDefault(row => row.UnitID == unitID && row.Date == dateTime);
                
            return tmam;
        }
        public Tmam GetTmamSubmitted(int unitID)
        {
            Tmam tmam = dBContext.Tmams.
                Include("AltCommander.Rank").
                FirstOrDefault(row => row.UnitID == unitID && row.Date == dateTime && row.Recieved && row.Submitted);

            return tmam;
        }
        public long AddTmamDetail(TmamDetail tmamDetail)
        {
            tmamDetail.TmamID = AddTmam(tmamDetail.Tmam);
            tmamDetail.Tmam = null;
            tmamDetail.ID = GetTmamDetailsID(tmamDetail);
            dBContext.TmamDetails.AddOrUpdate(tmamDetail);
            dBContext.SaveChanges();

            return tmamDetail.ID;
        }
        public void DeleteTmamDetail(TmamDetail tmamDetail)
        {
            dBContext.TmamDetails.Remove(tmamDetail);
            dBContext.SaveChanges();
        }
        public long GetTmamDetailsID(TmamDetail tmamDetail)
        {
            TmamDetail tmamDetailTemp = dBContext.TmamDetails
                .FirstOrDefault(row => row.TmamID == tmamDetail.TmamID && row.IsOfficers == tmamDetail.IsOfficers);
            return tmamDetailTemp?.ID ?? 0;
        }
        public TmamDetail GetTmamDetailOrDefault(int unitID, bool isOfficers)
        {
            TmamDetail tmamDetail = dBContext.TmamDetails.
                Include("Tmam.Unit").
                FirstOrDefault(row => row.Tmam.UnitID == unitID &&
                                        row.Tmam.Date == dateTime &&
                                        row.IsOfficers == isOfficers &&
                                        row.Tmam.Submitted &&
                                        row.Tmam.Recieved);

            if (tmamDetail == null)
            {
                tmamDetail = new TmamDetail
                {
                    Tmam = new Tmam
                    {
                        Unit = unitService.GetUnit(unitID)
                    }
                };
            }
            return tmamDetail;
        }
        public TmamDetail GetTmamDetails(int unitID, bool isOfficers)
        {
            TmamDetail tmamDetail = new TmamDetail();
            Tmam expectedTmam = new Tmam() { UnitID = unitID, Date = dateTime };
            long tmamID = GetTmamID(expectedTmam);
            TmamDetail tmamDetailTemp = new TmamDetail() { TmamID = tmamID, IsOfficers = isOfficers };
            long tmamDetailsID = GetTmamDetailsID(tmamDetailTemp);
            if (tmamDetailsID == 0)
            {
                int totalPower = (isOfficers) ? personService.GetPersonsCount(unitID, 1) : (personService.GetPersonsCount(unitID, 2) + personService.GetPersonsCount(unitID, 3));
                tmamDetail = new TmamDetail()
                {
                    totalPower = totalPower

                    
                };
            }
            else
            {
                tmamDetail = dBContext.TmamDetails.FirstOrDefault(row => row.ID == tmamDetailsID);

                int totalPower = (isOfficers) ? personService.GetPersonsCount(unitID, 1) : (personService.GetPersonsCount(unitID, 2) + personService.GetPersonsCount(unitID, 3));
                tmamDetail.totalPower = totalPower;
                dBContext.SaveChanges();
            }

            return tmamDetail;
        }
        public List<TmamDetail> GetTmamDetailsByDate(int userId, DateTime date)
        {
            // هنا قم بإحضار التمام من قاعدة البيانات بناءً على التاريخ
            return dBContext.TmamDetails.Where(t => t.Tmam.UnitID == userId && t.Tmam.Date == date).ToList();
        }

        public Tmam GetTmamWithAllDetails(int unitID)
        {
            /*
             * Copy the Previous Tmam
             */
            PrepareTmam(unitID);
            Tmam tmam = dBContext.Tmams
                .Include("TmamDetails")
                .Include("SickLeaves.SickLeaveDetail.Person.Rank")
                .Include("Errands.ErrandDetail.Person.Rank")
                .Include("Vacations.VacationDetail.Person.Rank")
                .Include("prisons.PrisonDetails.Person.Rank")
                .Include("prisons.PrisonDetails.CommandItem")
                .Include("Absences.AbsenceDetail.Person.Rank")
                .Include("Absences.AbsenceDetail.CommandItem")
                .Include("Hospitals.HospitalDetails.Person.Rank")
                .Include("OutOfCountries.OutOfCountryDetail.Person.Rank")
                .Include("Camps.CampDetail.Person.Rank")
                .Include("AltCommander.Rank")
                .Include("Unit")
                .Include("Courses.CourseDetails.Person.Rank")
                .Include("Courses.CourseDetails.CommandItem")
                .FirstOrDefault(row => row.UnitID == unitID && row.Date == dateTime);
                
                
            return tmam;
        }
        public Tmam GetTmamWithNumbers(int unitID)
        {
            Tmam tmam = dBContext.Tmams
                .Include("TmamDetails")
                .Include("SickLeaves")
                .Include("Errands")
                .Include("Vacations")
                .Include("prisons")
                .Include("Absences")
                .Include("Hospitals")
                .Include("OutOfCountries")
                .Include("Camps")
                .Include("Unit")
                .Include("Courses")
                .FirstOrDefault(row => row.UnitID == unitID && row.Date == dateTime);
            return tmam;
        }
        public Tmam GetTmamWithAllDetails(int unitID, DateTime date)
        {
            Tmam tmam = dBContext.Tmams
                .Include("TmamDetails")
                .Include("SickLeaves.SickLeaveDetail.Person.Rank")
                .Include("Errands.ErrandDetail.Person.Rank")
                .Include("Vacations.VacationDetail.Person.Rank")
                .Include("prisons.PrisonDetails.Person.Rank")
                .Include("prisons.PrisonDetails.CommandItem")
                .Include("Absences.AbsenceDetail.Person.Rank")
                .Include("Absences.AbsenceDetail.CommandItem")
                .Include("Hospitals.HospitalDetails.Person.Rank")
                .Include("OutOfCountries.OutOfCountryDetail.Person.Rank")
                .Include("Camps.CampDetail.Person.Rank")
                .Include("AltCommander.Rank")
                .Include("Courses.CourseDetails.Person.Rank")
                .Include("Courses.CourseDetails.CommandItem")
                .FirstOrDefault(row => row.UnitID == unitID && row.Date == date);
            return tmam;
        }
        public void PrepareTmam(int unitID)
        {
            try
            {
                DateTime lastDate = dBContext.Tmams.Where(row => row.UnitID == unitID).Max(row => row.Date);
                if (lastDate < dateTime)
                {
                    personService.ResetPersonsStatus(unitID); //reset all person status in this unit 
                    Tmam oldTmam = GetTmamWithAllDetails(unitID, lastDate);
                    Tmam newTmam = (Tmam)oldTmam.Clone();
                    oldTmam.Date = lastDate;
                    long tmamID = AddTmam(newTmam);
                    Tmam temp = GetTmamWithAllDetails(unitID, dateTime);
                    foreach (SickLeave sickLeave in temp.SickLeaves)
                    {
                        if (personService.PersonIsLeader(sickLeave.SickLeaveDetail.PersonID) != 0)
                        {
                            personStatusService.setPersonStatus(new PersonStatus
                            {
                                TmamID = temp.ID,
                                PersonID = sickLeave.SickLeaveDetail.PersonID,
                                Status = TmamEnum.SickLeave
                            });
                        }
                    }
                    foreach (Absence absence in temp.Absences)
                    {
                        if (personService.PersonIsLeader(absence.AbsenceDetail.PersonID) != 0)
                        {
                            personStatusService.setPersonStatus(new PersonStatus
                            {
                                TmamID = temp.ID,
                                PersonID = absence.AbsenceDetail.PersonID,
                                Status = TmamEnum.Absence
                            });
                        }
                    }
                    foreach (Errand errand in temp.Errands)
                    {
                        if (personService.PersonIsLeader(errand.ErrandDetail.PersonID) != 0)
                        {
                            personStatusService.setPersonStatus(new PersonStatus
                            {
                                TmamID = temp.ID,
                                PersonID = errand.ErrandDetail.PersonID,
                                Status = TmamEnum.Errand
                            });
                        }
                    }
                   foreach (Vacation vacation in temp.Vacations)
                    {
                        if (personService.PersonIsLeader(vacation.VacationDetail.PersonID) != 0)
                        {
                            personStatusService.setPersonStatus(new PersonStatus
                            {
                                TmamID = temp.ID,
                                PersonID = vacation.VacationDetail.PersonID,
                                Status = TmamEnum.Vacation
                            });
                        }
                    }
                    foreach (Prison prison in temp.prisons)
                    {
                        if (personService.PersonIsLeader(prison.PrisonDetails.PersonID) != 0)
                        {
                            personStatusService.setPersonStatus(new PersonStatus
                            {
                                TmamID = temp.ID,
                                PersonID = prison.PrisonDetails.PersonID,
                                Status = TmamEnum.Prison
                            });
                        }
                    }
                    foreach (Hospital hospital in temp.Hospitals)
                    {
                        if (personService.PersonIsLeader(hospital.HospitalDetails.PersonID) != 0)
                        {
                            personStatusService.setPersonStatus(new PersonStatus
                            {
                                TmamID = temp.ID,
                                PersonID = hospital.HospitalDetails.PersonID,
                                Status = TmamEnum.Hospital
                            });
                        }
                    }
                    foreach (OutOfCountry outOfCountry in temp.OutOfCountries)
                    {
                        if (personService.PersonIsLeader(outOfCountry.OutOfCountryDetail.PersonID) != 0)
                        {
                            personStatusService.setPersonStatus(new PersonStatus
                            {
                                TmamID = temp.ID,
                                PersonID = outOfCountry.OutOfCountryDetail.PersonID,
                                Status = TmamEnum.OutOfCountry
                            });
                        }
                    }
                    foreach (Camp camp in temp.Camps)
                    {
                        if (personService.PersonIsLeader(camp.CampDetail.PersonID) != 0)
                        {
                            personStatusService.setPersonStatus(new PersonStatus
                            {
                                TmamID = temp.ID,
                                PersonID = camp.CampDetail.PersonID,
                                Status = TmamEnum.Camp
                            });
                        }
                    }
                    foreach (Course course in temp.Courses)
                    {
                        if (personService.PersonIsLeader(course.CourseDetails.PersonID) != 0)
                        {
                            personStatusService.setPersonStatus(new PersonStatus
                            {
                                TmamID = temp.ID,
                                PersonID = course.CourseDetails.PersonID,
                                Status = TmamEnum.Course
                            });
                        }
                    }
                }
            }
            catch
            {

            }
        }
        public String SubmitTmam(int unitID)
        {
            StringBuilder result = new StringBuilder();
            Tmam tmam = GetTmamWithNumbers(unitID);
            
            if (tmam.AltCommanderID == null && tmam.Unit.AltComExist)
                result.Append("\n-برجاء إدخال منوب العمليات عن اليوم .");
            
            if (tmam.TmamDetails.FirstOrDefault(row => row.IsOfficers) == null)
                result.Append("\n-برجاء إدخال تمام الضباط .");
            
            if (tmam.TmamDetails.FirstOrDefault(row => !row.IsOfficers) == null)
                result.Append("\n-برجاء إدخال تمام الدرجات الأخرى .");
            
            if (tmam.TmamDetails.Sum(row => row.errand) != tmam.Errands.Count)
                result.Append($"\n-عدد المأموريات {tmam.TmamDetails.Sum(row => row.errand)} مدخل {tmam.Errands.Count}");

            if (tmam.TmamDetails.Sum(row => row.sickLeave) != tmam.SickLeaves.Count)
                result.Append($"\n-عدد الأجازت المرضية {tmam.TmamDetails.Sum(row => row.sickLeave)} مدخل {tmam.SickLeaves.Count}");

            if (tmam.TmamDetails.Sum(row => row.prison) != tmam.prisons.Count)
                result.Append($"\n-عدد السجن {tmam.TmamDetails.Sum(row => row.prison)} مدخل {tmam.prisons.Count}");

            if (tmam.TmamDetails.Sum(row => row.absence) != tmam.Absences.Count)
                result.Append($"\n-عدد الغياب {tmam.TmamDetails.Sum(row => row.absence)} مدخل {tmam.Absences.Count}");

            if (tmam.TmamDetails.Sum(row => row.hospital) != tmam.Hospitals.Count)
                result.Append($"\n-عدد المستشفيات {tmam.TmamDetails.Sum(row => row.hospital)} مدخل {tmam.Hospitals.Count}");

            if (tmam.TmamDetails.Sum(row => row.course) != tmam.Courses.Count)
                result.Append($"\n-عدد الفرق {tmam.TmamDetails.Sum(row => row.course)} مدخل {tmam.Courses.Count}");

            if (result.Length == 0)
            {
                tmam.Submitted = true;
                tmam.TimeSended = ConvertTimeToMilatryFormat(DateTime.Now);
                dBContext.SaveChanges();
            }

            return result.ToString();
        }
        public String ConvertTimeToMilatryFormat(DateTime time)
        {
            StringBuilder stringBuilder = new StringBuilder();
            int timeNum = (time.Hour * 100) + time.Minute;
            stringBuilder.Append(timeNum.ToString());
            int len = stringBuilder.Length;
            for (int i = 0; i < 4 - len; i++)
            {
                stringBuilder.Insert(0, "0");
            }
            return stringBuilder.ToString();
        }
        //change the recieve property to true in tamam
        public void ReciveTmam(int unitID)
        {
            Tmam tmam = dBContext.Tmams.FirstOrDefault(row => row.UnitID == unitID && row.Date == dateTime);
            tmam.Recieved = true;
            dBContext.SaveChanges();
        }
        public Dictionary<String, bool> GetTmamStatus(int unitID)
        {
            Tmam tmam = GetTmam(unitID);
            Dictionary<String, bool> tmamStatus = new Dictionary<String, bool>();
            tmamStatus["Submitted"] = tmam?.Submitted ?? false;
            tmamStatus["Recieved"] = tmam?.Recieved ?? false;
            return tmamStatus;
        }

        //cancel tmam cuz there is an error 
        public void ReturnTmam(int unitID)
        {
            Tmam tmam = dBContext.Tmams.FirstOrDefault(row => row.UnitID == unitID && row.Date == dateTime);
            tmam.Recieved = false;
            tmam.Submitted = false;
            dBContext.SaveChanges();
        }
        public Dictionary<String, LeaderTmamView> GetLeaderTmamPerUnit(int unitID)
        {
            Dictionary<String, LeaderTmamView> leaderTmamDic = new Dictionary<string, LeaderTmamView>();
            Tmam tmam = GetTmam(unitID);
            if (tmam != null)
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
    }
}