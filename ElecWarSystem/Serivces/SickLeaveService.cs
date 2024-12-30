using ElecWarSystem.Data;
using ElecWarSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ElecWarSystem.Serivces
{
    public class SickLeaveService : IOutdoorService<SickLeave, SickLeavesDetails>
    {
        private readonly AppDBContext dBContext;
        private readonly PersonService personService;
        private readonly TmamService tmamService;
        private readonly PersonStatusService personStatusService;

        public SickLeaveService()
        {
            dBContext = new AppDBContext();
            personService = new PersonService();
            tmamService = new TmamService();
            personStatusService = new PersonStatusService();

        }
        public SickLeave Get(long id)
        {
            SickLeave sickLeave = dBContext.SickLeaves.Include(row => row.SickLeaveDetail). 
                FirstOrDefault(row => row.ID == id);
            return sickLeave;
        }
        public SickLeavesDetails GetDetail(long id)
        {
            SickLeavesDetails sickLeavesDetails = dBContext.SickLeavesDetails.Find(id);
            return sickLeavesDetails;
        }
        public int GetCount(long sickLeaveDetailsID)
        {
            int sickLeaveCount = dBContext.SickLeaves
                .Where(row => row.SickLeaveDetailID == sickLeaveDetailsID)
                .Count();
            return sickLeaveCount;
        }
        public List<SickLeave> GetAll(int unitID)
        {
            Tmam tmam = new Tmam() { UnitID = unitID, Date = DateTime.Today.AddDays(1) };
            long tmamID = tmamService.AddTmam(tmam);
            List<SickLeave> sickLeaves = dBContext.SickLeaves
                .Include("SickLeaveDetail")
                .Include("SickLeaveDetail.Person.Rank")
                .OrderByDescending(row => row.SickLeaveDetail.Person.RankID)
                .Where(row => row.TmamID == tmamID).ToList();
            return sickLeaves;
        }
        public long GetID(SickLeavesDetails sickLeavesDetails)
        {
            SickLeavesDetails sickLeavesDetailsTemp =
                dBContext.SickLeavesDetails
                .FirstOrDefault(row => row.DateFrom == sickLeavesDetails.DateFrom &&
                    row.DateTo == sickLeavesDetails.DateTo &&
                    row.PersonID == sickLeavesDetails.PersonID);

            return (sickLeavesDetails != null) ? sickLeavesDetails.ID : 0;
        }
        public long AddDetail(SickLeavesDetails sickLeavesDetails)
        {
            long id = GetID(sickLeavesDetails);
            if (id == 0)
            {
                dBContext.SickLeavesDetails.Add(sickLeavesDetails);
                dBContext.SaveChanges();
                return sickLeavesDetails.ID;
            }
            else
            {
                SickLeavesDetails sickLeavesDetailsTemp = dBContext.SickLeavesDetails.Find(id);
                sickLeavesDetailsTemp.Hospital = sickLeavesDetails.Hospital;
                sickLeavesDetailsTemp.Diagnosis = sickLeavesDetails.Diagnosis;
                sickLeavesDetailsTemp.HospitalDate = sickLeavesDetails.HospitalDate;
                dBContext.SaveChanges();
                return id;
            }
        }

        public long Add(SickLeave sickLeave)
        {
            sickLeave.Tmam = tmamService.GetTmam(sickLeave.TmamID);
            if (sickLeave.IsDateLogic())
            {
                personStatusService.setPersonStatus(new PersonStatus
                {
                    PersonID = sickLeave.SickLeaveDetail.PersonID,
                    TmamID = sickLeave.TmamID,
                    Status = TmamEnum.SickLeave
                });
                sickLeave.CleanNav();
                dBContext.SickLeaves.Add(sickLeave);
                dBContext.SaveChanges();
                return sickLeave.ID;
            }
            else
            {
                return -1;
            }
        }
        public void Delete(long id)
        {
            SickLeave sickLeave = Get(id);
            personStatusService.DeletePersonStatus(sickLeave.TmamID, sickLeave.SickLeaveDetail.PersonID);
            sickLeave.SickLeaveDetail = null;
            dBContext.SickLeaves.Remove(sickLeave);
            dBContext.SaveChanges();

        }
        public int getTotal(int unitID)
        {
            Tmam tmam = new Tmam { UnitID = unitID, Date = DateTime.Today.AddDays(1) };
            long tmamID = tmamService.GetTmamID(tmam);

            int totalSickLeaves = 0;
            if (tmamID > 0)
            {
                try
                {
                    totalSickLeaves = dBContext.TmamDetails.Where(row => row.TmamID == tmamID)?.ToList().Sum(row => row.sickLeave) ?? 0;
                }
                catch (Exception ex)
                {
                    totalSickLeaves = 0;
                }
            }
            return totalSickLeaves;
        }
        public int getEntered(int unitID)
        {
            Tmam tmam = new Tmam { UnitID = unitID, Date = DateTime.Today.AddDays(1) };
            long tmamID = tmamService.GetTmamID(tmam);
            int enteredSickLeaves = 0;
            if (tmamID > 0)
            {
                try
                {
                    enteredSickLeaves = dBContext.SickLeaves.Where(row => row.TmamID == tmamID).Count();
                }
                catch (Exception ex)
                {
                    enteredSickLeaves = 0;
                }
            }
            return enteredSickLeaves;
        }
    }
}