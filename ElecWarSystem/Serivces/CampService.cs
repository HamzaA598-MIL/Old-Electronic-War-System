using ElecWarSystem.Data;
using ElecWarSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElecWarSystem.Serivces
{
    public class CampService : IOutdoorService<Camp, CampDetail>
    {
        private readonly AppDBContext dBContext;
        private readonly PersonService personService;
        private readonly TmamService tmamService;
        private readonly PersonStatusService personStatusService ;
        public CampService()
        {
            dBContext = new AppDBContext();
            personService = new PersonService();
            tmamService = new TmamService();
            personStatusService = new PersonStatusService();
        }
        public Camp Get(long id)
        {
            Camp Camp = dBContext.Camps.FirstOrDefault(row => row.ID ==id);
            return Camp;
        }
        public CampDetail GetDetail(long id)
        {
            CampDetail CampDetail = dBContext.CampDetails.Find(id);
            return CampDetail;
        }
        public int GetCount(long CampDetailsID)
        {
            int CampCount = dBContext.Camps
                .Where(row => row.CampDetailID == CampDetailsID)
                .Count();
            return CampCount;
        }
        public List<Camp> GetAll(int unitID)
        {
            Tmam tmam = new Tmam() { UnitID = unitID, Date = DateTime.Today.AddDays(1) };
            long tmamID = tmamService.AddTmam(tmam);
            List<Camp> Camps = dBContext.Camps
                .Include("CampDetail")
                .Include("CampDetail.Person.Rank")
                .OrderByDescending(row => row.CampDetail.Person.RankID)
                .Where(row => row.TmamID == tmamID).ToList();
            return Camps;
        }
        public long GetID(CampDetail CampDetail)
        {
            CampDetail CampDetailsTemp =
                dBContext.CampDetails
                .FirstOrDefault(row => row.DateFrom == CampDetail.DateFrom &&
                    row.DateTo == CampDetail.DateTo &&
                    row.PersonID == CampDetail.PersonID);

            return (CampDetail != null) ? CampDetail.ID : 0;
        }
        public long AddDetail(CampDetail CampDetails)
        {
            long id = GetID(CampDetails);
            if (id == 0)
            {
                dBContext.CampDetails.Add(CampDetails);
                dBContext.SaveChanges();
                return CampDetails.ID;
            }
            else
            {
                return id;
            }
        }
        public long Add(Camp Camp)
        {
            Camp.Tmam = tmamService.GetTmam(Camp.TmamID);
            if (Camp.IsDateLogic())
            {
                Camp.CampDetailID = AddDetail(Camp.CampDetail);
                long personID = Camp.CampDetail.PersonID;
                Camp.CleanNav();
                dBContext.Camps.Add(Camp);
                dBContext.SaveChanges();
                
                if (personService.PersonIsLeader(personID) != 0)
                {
                    personStatusService.setPersonStatus(new PersonStatus
                    {
                        PersonID = personID,
                        TmamID = Camp.TmamID,
                        Status = TmamEnum.Camp
                    });
                }
                return Camp.ID;
            }
            else
            {
                return -1;
            }
        }
        public void Delete(long id)
        {
            Camp Camp = Get(id);
            long CampDetailsID = Camp.CampDetailID;
            CampDetail CampDetail = GetDetail(CampDetailsID);
            long personID = CampDetail.PersonID;
            personStatusService.DeletePersonStatus(Camp.TmamID, personID);

            if (GetCount(CampDetailsID) == 1)
            {
                DeleteDetails(CampDetail);
            }
            else
            {
                dBContext.Camps.Remove(Camp);
            }
            dBContext.SaveChanges();
        }
        public void DeleteDetails(CampDetail CampDetail)
        {
            dBContext.CampDetails.Remove(CampDetail);
            dBContext.SaveChanges();
        }
        public int getTotal(int unitID)
        {
            Tmam tmam = new Tmam { UnitID = unitID, Date = DateTime.Today.AddDays(1) };
            long tmamID = tmamService.GetTmamID(tmam);

            int totalCamps = 0;
            if (tmamID > 0)
            {
                try
                {
                    totalCamps = dBContext.TmamDetails.Where(row => row.TmamID == tmamID)?.ToList().Sum(row => row.outdoorCamp) ?? 0;
                }
                catch (Exception ex)
                {
                    totalCamps = 0;
                }
            }
            return totalCamps;
        }
        public int getEntered(int unitID)
        {
            Tmam tmam = new Tmam { UnitID = unitID, Date = DateTime.Today.AddDays(1) };
            long tmamID = tmamService.GetTmamID(tmam);
            int enteredCamps = 0;
            if (tmamID > 0)
            {
                try
                {
                    enteredCamps = dBContext.Camps.Where(row => row.TmamID == tmamID).Count();
                }
                catch (Exception ex)
                {
                    enteredCamps = 0;
                }
            }
            return enteredCamps;
        }
    }
}