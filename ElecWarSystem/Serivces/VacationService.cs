using ElecWarSystem.Data;
using ElecWarSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ElecWarSystem.Serivces
{
    public class VacationService
    {
        private readonly AppDBContext dBContext;
        private readonly TmamService tmamService;
        private readonly PersonStatusService personStatusService;
        private readonly PersonService personService;
        public VacationService()
        {
            dBContext = new AppDBContext();
            tmamService = new TmamService();
            personStatusService = new PersonStatusService();
            personService = new PersonService();
        }
        public Vacation Get(long id)
        {
            Vacation Vacation = dBContext.Vacations.FirstOrDefault(row => row.ID == id);
            return Vacation;
        }
        public VacationDetail GetDetail(long id)
        {
            VacationDetail VacationDetail = dBContext.VacationDetails.Find(id);
            return VacationDetail;
        }
        public List<Vacation> GetAll(int unitID)
        {
            Tmam tmam = new Tmam() { UnitID = unitID, Date = DateTime.Today.AddDays(1) };
            long tmamID = tmamService.AddTmam(tmam);
            List<Vacation> Vacations = dBContext.Vacations
                .Include("VacationDetail")
                .OrderByDescending(row => row.VacationDetail.Person.RankID)
                .Include("VacationDetail.Person.Rank")
                .Where(row => row.TmamID == tmamID).ToList();
            return Vacations;
        }
        public long GetID(VacationDetail VacationDetail)
        {
            VacationDetail VacationDetailTemp =
                dBContext.VacationDetails
                .FirstOrDefault(row =>
                    row.DateFrom.Equals(VacationDetail.DateFrom.Date) &&
                    row.DateTo.Equals(VacationDetail.DateTo.Date) &&
                    row.PersonID == VacationDetail.PersonID);


            long VacationID = 0;

            if (VacationDetailTemp != null)
            {
                VacationID = VacationDetailTemp.ID;
            }

            return VacationID;
        }

        public long AddDetail(VacationDetail VacationDetail)
        {
            long id = GetID(VacationDetail);

            if (id == 0)
            {
                dBContext.VacationDetails.Add(VacationDetail);
                dBContext.SaveChanges();
                id = VacationDetail.ID;
            }
            return id;
        }
        public long Add(Vacation Vacation)
        {
            Vacation.Tmam = tmamService.GetTmam(Vacation.TmamID);
            if (Vacation.IsDateLogic())
            {
                if (personService.PersonIsLeader(Vacation.VacationDetail.PersonID) != 0)
                {
                    personStatusService.setPersonStatus(new PersonStatus
                    {
                        PersonID = Vacation.VacationDetail.PersonID,
                        TmamID = Vacation.TmamID,
                        Status = TmamEnum.Vacation
                    });
                }
                
                Vacation.CleanNav();
                dBContext.Vacations.Add(Vacation);
                dBContext.SaveChanges();
                return Vacation.ID;
            }
            else
            {
                return -1;
            }
        }
        public int GetCount(long VacationDetailID)
        {
            int VacationCount = dBContext.Vacations
                .Where(row => row.VacationDetailID == VacationDetailID)
                .Count();
            return VacationCount;
        }
        public void Delete(long id)
        {
            Vacation Vacation = Get(id);
            long VacationID = Vacation.VacationDetailID;
            VacationDetail VacationDetail = GetDetail(VacationID);
            personStatusService.DeletePersonStatus(Vacation.TmamID, Vacation.VacationDetail.PersonID);
            if (GetCount(VacationID) == 1)
            {
                dBContext.VacationDetails.Remove(VacationDetail);
            }
            else
            {
                dBContext.Vacations.Remove(Vacation);
            }
            dBContext.SaveChanges();
            
        }

        public int getTotal(int unitID)
        {
            Tmam tmam = new Tmam { UnitID = unitID, Date = DateTime.Today.AddDays(1) };
            long tmamID = tmamService.GetTmamID(tmam);
            int totalVacations = 0;
            if (tmamID > 0)
            {
                totalVacations = dBContext.TmamDetails.Where(row => row.TmamID == tmamID)?.ToList().Sum(row => row.vacation) ?? 0;
            }
            return totalVacations;
        }
        public int getEntered(int unitID)
        {
            Tmam tmam = new Tmam { UnitID = unitID, Date = DateTime.Today.AddDays(1) };
            long tmamID = tmamService.GetTmamID(tmam);
            int enteredVacations = 0;
            if (tmamID > 0)
            {
                try
                {
                    enteredVacations = dBContext.Vacations.Where(row => row.TmamID == tmamID).Count();
                }
                catch (Exception ex)
                {
                    enteredVacations = 0;
                }
            }
            return enteredVacations;
        }
    }
}