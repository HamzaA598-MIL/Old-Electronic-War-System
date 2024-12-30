using ElecWarSystem.Data;
using ElecWarSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElecWarSystem.Serivces
{
    public class AbsenceService : IOutdoorService<Absence, AbsenceDetail>
    {
        private readonly AppDBContext dBContext;
        private readonly PersonService personService;
        private readonly TmamService tmamService;
        private readonly PersonStatusService personStatusService;

        public AbsenceService()
        {
            dBContext = new AppDBContext();
            personService = new PersonService();
            tmamService = new TmamService();
            personStatusService = new PersonStatusService();
        }
        public Absence Get(long id)
        {
            Absence Absence = dBContext.Absences
                .FirstOrDefault(row=> row.ID == id);
            return Absence;
        }
        public AbsenceDetail GetDetail(long id)
        {
            AbsenceDetail AbsenceDetail = dBContext.AbsenceDetails.Find(id);
            return AbsenceDetail;
        }
        public List<Absence> GetAll(int unitID)
        {
            Tmam tmam = new Tmam() { UnitID = unitID, Date = DateTime.Today.AddDays(1) };
            long tmamID = tmamService.AddTmam(tmam);
            List<Absence> Absences = dBContext.Absences
                .Include("AbsenceDetail")
                .Include("AbsenceDetail.Person.Rank")
                .Include("AbsenceDetail.commandItem")
                .OrderByDescending(row => row.AbsenceDetail.Person.RankID)
                .Where(row => row.TmamID == tmamID).ToList();
            return Absences;
        }
        public long GetID(AbsenceDetail AbsenceDetail)
        {
            AbsenceDetail AbsenceDetailTemp =
                dBContext.AbsenceDetails
                .FirstOrDefault(row => row.DateFrom == AbsenceDetail.DateFrom &&
                    row.PersonID == AbsenceDetail.PersonID);
            return AbsenceDetailTemp?.ID ?? 0;
        }

        public long AddDetail(AbsenceDetail AbsenceDetail)
        {
            long id = GetID(AbsenceDetail);
            if (id == 0)
            {
                dBContext.AbsenceDetails.Add(AbsenceDetail);
                dBContext.SaveChanges();
                id = AbsenceDetail.ID;
            }
            return id;
        }
        public bool IsDatesLogic(Absence Absence)
        {
            Tmam tmam = tmamService.GetTmam(Absence.TmamID);
            bool result = Absence.AbsenceDetail.DateFrom <= tmam.Date;
            return result;
        }
        public long Add(Absence Absence)
        {
            long personId = 0;
            if (IsDatesLogic(Absence))
            {
                Absence.AbsenceDetailID = AddDetail(Absence.AbsenceDetail);
                personId = Absence.AbsenceDetail.PersonID;
                Absence.AbsenceDetail = null;
                dBContext.Absences.Add(Absence);
                dBContext.SaveChanges();
                if(personService.PersonIsLeader(personId) != 0){
                    personStatusService.setPersonStatus(new PersonStatus
                    {
                        PersonID = Absence.AbsenceDetail.PersonID,
                        TmamID = Absence.TmamID,
                        Status = TmamEnum.Absence
                    });
                }
                return Absence.ID;
            }
            else
            {
                return -1;
            }
        }
        public int GetCount(long AbsenceDetailID)
        {
            int AbsenceCount = dBContext.Absences
                .Where(row => row.AbsenceDetailID == AbsenceDetailID)
                .Count();
            return AbsenceCount;
        }
        public void Delete(long id)
        {
            Absence Absence = Get(id);
            long absenceID = Absence.AbsenceDetailID;
            AbsenceDetail absenceDetail = GetDetail(absenceID);
            long personID = absenceDetail.PersonID;
            personStatusService.DeletePersonStatus(Absence.TmamID, personID);

            if (GetCount(absenceID) == 1)
            {
                dBContext.AbsenceDetails.Remove(absenceDetail);
            }
            else
            {
                dBContext.Absences.Remove(Absence);
            }
            dBContext.SaveChanges();
        }
        public int getTotal(int unitID)
        {
            Tmam tmam = new Tmam { UnitID = unitID, Date = DateTime.Today.AddDays(1) };
            long tmamID = tmamService.GetTmamID(tmam);
            int? totalAbsences = 0;
            if (tmamID > 0)
            {
                totalAbsences = dBContext.TmamDetails.Where(row => row.TmamID == tmamID)?.ToList().Sum(row => row.absence) ?? 0; ;
            }
            return (int)totalAbsences;
        }
        public int getEntered(int unitID)
        {
            Tmam tmam = new Tmam { UnitID = unitID, Date = DateTime.Today.AddDays(1) };
            long tmamID = tmamService.GetTmamID(tmam);
            int enteredAbsences = 0;
            if (tmamID > 0)
            {
                try
                {
                    enteredAbsences = dBContext.Absences.Where(row => row.TmamID == tmamID).Count();
                }
                catch (Exception ex)
                {
                    enteredAbsences = 0;
                }
            }
            return enteredAbsences;
        }
    }
}