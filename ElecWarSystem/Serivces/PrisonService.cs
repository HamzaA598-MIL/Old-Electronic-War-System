using ElecWarSystem.Data;
using ElecWarSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElecWarSystem.Serivces
{
    public class PrisonService : IOutdoorService<Prison, PrisonDetail>
    {
        private readonly AppDBContext dBContext;
        private readonly PersonStatusService personStatusService;
        private readonly TmamService tmamService;
        public PrisonService()
        {
            dBContext = new AppDBContext();
            personStatusService = new PersonStatusService();
            tmamService = new TmamService();
        }
        public Prison Get(long id)
        {
            Prison prison = dBContext.Prisons
                .FirstOrDefault(row => row.ID == id);
            return prison;
        }
        public PrisonDetail GetDetail(long id)
        {
            PrisonDetail PrisonDetail = dBContext.PrisonDetails.Find(id);
            return PrisonDetail;
        }
        public List<Prison> GetAll(int unitID)
        {
            Tmam tmam = new Tmam() { UnitID = unitID, Date = DateTime.Today.AddDays(1) };
            long tmamID = tmamService.AddTmam(tmam);
            List<Prison> Prisons = dBContext.Prisons
                .Include("PrisonDetails")
                .Include("PrisonDetails.Person.Rank")
                .Include("PrisonDetails.CommandItem")
                .OrderByDescending(row => row.PrisonDetails.Person.Rank.RankName)
                .Where(row => row.TmamID == tmamID).ToList();
            return Prisons;
        }
        public long GetID(PrisonDetail PrisonDetail)
        {
            PrisonDetail PrisonDetailTemp =
                dBContext.PrisonDetails
                .FirstOrDefault(row => row.DateFrom == PrisonDetail.DateFrom &&
                    row.DateTo == PrisonDetail.DateTo &&
                    row.PersonID == PrisonDetail.PersonID);
            long PrisonID = 0;
            if (PrisonDetailTemp != null)
            {
                PrisonID = PrisonDetailTemp.ID;
            }
            return PrisonID;
        }

        public long AddDetail(PrisonDetail PrisonDetail)
        {
            long id = GetID(PrisonDetail);

            if (id == 0)
            {
                dBContext.PrisonDetails.Add(PrisonDetail);
                dBContext.SaveChanges();
                id = PrisonDetail.ID;
            }
            return id;
        }
        public bool IsDatesLogic(Prison Prison)
        {
            Tmam tmam = tmamService.GetTmam(Prison.TmamID);
            bool result = Prison.PrisonDetails.DateFrom <= tmam.Date &&
                Prison.PrisonDetails.DateTo > tmam.Date;
            return result;
        }
        public long Add(Prison Prison)
        {
            if (IsDatesLogic(Prison))
            {
                Prison.PrisonDetailID = AddDetail(Prison.PrisonDetails);
                long personID = Prison.PrisonDetails.PersonID;
                Prison.PrisonDetails = null;
                Prison.Tmam = null;
                dBContext.Prisons.Add(Prison);
                dBContext.SaveChanges();
                personStatusService.setPersonStatus(new PersonStatus
                {
                    PersonID = personID,
                    TmamID = Prison.TmamID,
                    Status = TmamEnum.Prison
                });
                return Prison.ID;
            }
            else
            {
                return -1;
            }
        }
        public int GetCount(long PrisonDetailID)
        {
            int PrisonCount = dBContext.Prisons
                .Where(row => row.PrisonDetailID == PrisonDetailID)
                .Count();
            return PrisonCount;
        }
        public void Delete(long id)
        {
            Prison Prison = Get(id);
            if (Prison == null)
            {
                throw new Exception($"Prison with ID {id} not found.");
            }

            dBContext.Prisons.Remove(Prison);
            dBContext.SaveChanges();

            if (Prison.PrisonDetailID != null)
            {
                PrisonDetail PrisonDetail = GetDetail(Prison.PrisonDetailID);
                if (PrisonDetail != null)
                {
                    if (GetCount(Prison.PrisonDetailID) == 1)
                    {
                        dBContext.PrisonDetails.Remove(PrisonDetail);

                        if (PrisonDetail.CommandItemID != null)
                        {
                            CommandItem commandItem = dBContext.CommandItems.Find(PrisonDetail.CommandItemID);
                            if (commandItem != null)
                            {
                                dBContext.CommandItems.Remove(commandItem);
                            }
                        }
                    }
                }
            }

            if (Prison.TmamID != null && Prison.PrisonDetails?.PersonID != null)
            {
                personStatusService.DeletePersonStatus(Prison.TmamID, Prison.PrisonDetails.PersonID);
            }
        }

        public int getTotal(int unitID)
        {
            Tmam tmam = new Tmam { UnitID = unitID, Date = DateTime.Today.AddDays(1) };
            long tmamID = tmamService.GetTmamID(tmam);
            int totalPrisons = 0;
            if (tmamID > 0)
            {
                totalPrisons = dBContext.TmamDetails.Where(row => row.TmamID == tmamID)?.ToList().Sum(row => row.prison) ?? 0;               
            }
            return totalPrisons;
        }
        public int getEntered(int unitID)
        {
            Tmam tmam = new Tmam { UnitID = unitID, Date = DateTime.Today.AddDays(1) };
            long tmamID = tmamService.GetTmamID(tmam);
            int enteredPrisons = 0;
            if (tmamID > 0)
            {
                try
                {
                    enteredPrisons = dBContext.Prisons.Where(row => row.TmamID == tmamID).Count();
                }
                catch (Exception ex)
                {
                    enteredPrisons = 0;
                }
            }
            return enteredPrisons;
        }

    }
}