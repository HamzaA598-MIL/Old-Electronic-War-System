using ElecWarSystem.Data;
using ElecWarSystem.Models;
using ElecWarSystem.Models.IModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElecWarSystem.Serivces
{
    public class OutOfCountryService : IOutdoorService<OutOfCountry, OutOfCountryDetail>
    {
        private readonly AppDBContext dBContext;
        private readonly PersonStatusService personStatusService;
        private readonly TmamService tmamService;
        public OutOfCountryService()
        {
            dBContext = new AppDBContext();
            personStatusService = new PersonStatusService();
            tmamService = new TmamService();
        }
        public OutOfCountry Get(long id)
        {
            OutOfCountry outOfCountry = dBContext.OutOfCountries.Include("OutOfCountryDetail")
                .FirstOrDefault(row => row.ID == id);
            return outOfCountry;
        }
        public OutOfCountryDetail GetDetail(long id)
        {
            OutOfCountryDetail outOfCountryDetail = dBContext.OutOfCountryDetails.Find(id);
            return outOfCountryDetail;
        }
        public int GetCount(long outOfCountryDetailsID)
        {
            int outOfCountryCount = dBContext.OutOfCountries
                .Where(row => row.OutOfCountryDetailID == outOfCountryDetailsID)
                .Count();
            return outOfCountryCount;
        }
        public List<OutOfCountry> GetAll(int unitID)
        {
            Tmam tmam = new Tmam() { UnitID = unitID, Date = DateTime.Today.AddDays(1) };
            long tmamID = tmamService.AddTmam(tmam);
            List<OutOfCountry> outOfCountries = dBContext.OutOfCountries
                .Include("OutOfCountryDetail")
                .Include("OutOfCountryDetail.Person.Rank")
                .OrderByDescending(row => row.OutOfCountryDetail.Person.RankID)
                .Where(row => row.TmamID == tmamID).ToList();
            return outOfCountries;
        }
        public long GetID(OutOfCountryDetail outOfCountryDetail)
        {
            OutOfCountryDetail outOfCountryDetailsTemp =
                dBContext.OutOfCountryDetails
                .FirstOrDefault(row => row.DateFrom == outOfCountryDetail.DateFrom &&
                    row.DateTo == outOfCountryDetail.DateTo &&
                    row.PersonID == outOfCountryDetail.PersonID);

            return (outOfCountryDetail != null) ? outOfCountryDetail.ID : 0;
        }
        public long AddDetail(OutOfCountryDetail outOfCountryDetails)
        {
            long id = GetID(outOfCountryDetails);
            if (id == 0)
            {
                dBContext.OutOfCountryDetails.Add(outOfCountryDetails);
                dBContext.SaveChanges();
                return outOfCountryDetails.ID;
            }
            else
            {
                return id;
            }
        }
        public long Add(OutOfCountry outOfCountry)
        {
            outOfCountry.Tmam = tmamService.GetTmam(outOfCountry.TmamID);
            if (outOfCountry.IsDateLogic())
            {
                personStatusService.setPersonStatus(new PersonStatus
                {
                    PersonID = outOfCountry.OutOfCountryDetail.PersonID,
                    TmamID = outOfCountry.TmamID,
                    Status = TmamEnum.OutOfCountry
                });
                outOfCountry.CleanNav();
                dBContext.OutOfCountries.Add(outOfCountry);
                dBContext.SaveChanges();
                return outOfCountry.ID;
            }
            else
            {
                return -1;
            }
        }
        public void Delete(long id)
        {
            OutOfCountry outOfCountry = Get(id);
            personStatusService.DeletePersonStatus(
                outOfCountry.TmamID,
                outOfCountry.OutOfCountryDetail.PersonID);
            dBContext.OutOfCountries.Remove(outOfCountry);
            dBContext.SaveChanges();
        }
        public int getTotal(int unitID)
        {
            Tmam tmam = new Tmam { UnitID = unitID, Date = DateTime.Today.AddDays(1) };
            long tmamID = tmamService.GetTmamID(tmam);

            int totalOutOfCountrys = 0;
            if (tmamID > 0)
            {
                try
                {
                    totalOutOfCountrys = dBContext.TmamDetails.Where(row => row.TmamID == tmamID)?.ToList().Sum(row => row.outOfCountry) ?? 0;
                }
                catch (Exception ex)
                {
                    totalOutOfCountrys = 0;
                }
            }
            return totalOutOfCountrys;
        }
        public int getEntered(int unitID)
        {
            Tmam tmam = new Tmam { UnitID = unitID, Date = DateTime.Today.AddDays(1) };
            long tmamID = tmamService.GetTmamID(tmam);
            int enteredOutOfCountrys = 0;
            if (tmamID > 0)
            {
                try
                {
                    enteredOutOfCountrys = dBContext.OutOfCountries.Where(row => row.TmamID == tmamID).Count();
                }
                catch (Exception ex)
                {
                    enteredOutOfCountrys = 0;
                }
            }
            return enteredOutOfCountrys;
        }
    }
}