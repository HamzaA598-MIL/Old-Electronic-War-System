using ElecWarSystem.Data;
using ElecWarSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElecWarSystem.Serivces
{
    public class HospitalService : IOutdoorService<Hospital, HospitalDetails>
    {
        private readonly AppDBContext dBContext;
        private readonly PersonService personService;
        private readonly TmamService tmamService;
        private readonly PersonStatusService personStatusService;
        public HospitalService()
        {
            dBContext = new AppDBContext();
            personService = new PersonService();
            tmamService = new TmamService();
            personStatusService = new PersonStatusService();
        }
        public Hospital Get(long id)
        {
            Hospital Hospital = dBContext.Hospitals.FirstOrDefault(row => row.ID == id);
            return Hospital;
        }
        public HospitalDetails GetDetail(long id)
        {
            HospitalDetails HospitalsDetails = dBContext.HospitalDetails.Find(id);
            return HospitalsDetails;
        }
        public int GetCount(long HospitalDetailsID)
        {
            int HospitalCount = dBContext.Hospitals
                .Where(row => row.HospitalDetailID == HospitalDetailsID)
                .Count();
            return HospitalCount;
        }
        public List<Hospital> GetAll(int unitID)
        {
            Tmam tmam = new Tmam() { UnitID = unitID, Date = DateTime.Today.AddDays(1) };
            long tmamID = tmamService.AddTmam(tmam);
            List<Hospital> Hospitals = dBContext.Hospitals
                .Include("HospitalDetails")
                .Include("HospitalDetails.Person.Rank")
                .OrderByDescending(row => row.HospitalDetails.Person.RankID)
                .Where(row => row.TmamID == tmamID).ToList();
            return Hospitals;
        }
        public long GetID(HospitalDetails HospitalsDetails)
        {
            HospitalDetails HospitalsDetailsTemp =
                dBContext.HospitalDetails
                .FirstOrDefault(row => row.PersonID == HospitalsDetails.PersonID && row.DateFrom == HospitalsDetails.DateFrom);

            return HospitalsDetailsTemp?.ID ?? 0;
        }
        public long AddDetail(HospitalDetails HospitalsDetails)
        {
            long id = GetID(HospitalsDetails);
            if (id == 0)
            {
                dBContext.HospitalDetails.Add(HospitalsDetails);
                dBContext.SaveChanges();
                return HospitalsDetails.ID;
            }
            else
            {
                return id;
            }
        }
        public long Add(Hospital Hospital)
        {
            if (Hospital.IsDateLogic())
            {
                Hospital.HospitalDetailID = AddDetail(Hospital.HospitalDetails);
                long personID = Hospital.HospitalDetails.PersonID;
                Hospital.CleanNav();
                dBContext.Hospitals.Add(Hospital);
                dBContext.SaveChanges();
                if (personService.PersonIsLeader(personID) != 0)
                {
                    personStatusService.setPersonStatus(new PersonStatus
                    {
                        PersonID = personID,
                        TmamID = Hospital.TmamID,
                        Status = TmamEnum.Hospital
                    });
                }
                    
                return Hospital.ID;
            }
            else
            {
                return -1;
            }
        }
        public void Delete(long id)
        {
            Hospital Hospital = Get(id);
            long HospitalDetailsID = Hospital.HospitalDetailID;
            HospitalDetails HospitalsDetails = GetDetail(HospitalDetailsID);
            long personID = HospitalsDetails.PersonID;
            personStatusService.DeletePersonStatus(Hospital.TmamID, personID);

            if (GetCount(HospitalDetailsID) == 1)
            {
                dBContext.HospitalDetails.Remove(HospitalsDetails);
            }
            else
            {
                dBContext.Hospitals.Remove(Hospital);
            }
            dBContext.SaveChanges();
        }
        public int getTotal(int unitID)
        {
            Tmam tmam = new Tmam { UnitID = unitID, Date = DateTime.Today.AddDays(1) };
            long tmamID = tmamService.GetTmamID(tmam);

            int totalHospitals = 0;
            if (tmamID > 0)
            {
                totalHospitals = dBContext.TmamDetails.Where(row => row.TmamID == tmamID)?.ToList().Sum(row => row.hospital) ?? 0;
            }
            return totalHospitals;
        }//
        public int getEntered(int unitID)
        {
            Tmam tmam = new Tmam { UnitID = unitID, Date = DateTime.Today.AddDays(1) };
            long tmamID = tmamService.GetTmamID(tmam);
            int enteredHospitals = 0;
            if (tmamID > 0)
            {
                try
                {
                    enteredHospitals = dBContext.Hospitals.Where(row => row.TmamID == tmamID).Count();
                }
                catch (Exception ex)
                {
                    enteredHospitals = 0;
                }
            }
            return enteredHospitals;
        }
    }
}