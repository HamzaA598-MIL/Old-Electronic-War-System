using ElecWarSystem.Data;
using ElecWarSystem.Models;
using System.Linq;

namespace ElecWarSystem.Serivces
{
    public class PersonStatusService
    {
        private readonly AppDBContext dBContext;
        private readonly PersonService personService;
        public PersonStatusService()
        {
            dBContext = new AppDBContext();
            personService = new PersonService();
        }

        public void setPersonStatus(PersonStatus personStatus)
        {
            dBContext.PersonStatus.Add(personStatus);
            dBContext.SaveChanges();
            personService.setStatus(personStatus.PersonID);
        }
        public TmamEnum getPersonStatus(long tmamID ,long personID)
        {
            PersonStatus personStatus = dBContext.PersonStatus.FirstOrDefault(row => 
                row.PersonID == personID &&
                row.TmamID == tmamID);
            if (personStatus == null)
            {
                return TmamEnum.Exist;
            }
            return personStatus.Status;
        }
        public void DeletePersonStatus(long tmamID, long personID)
        {
            if(tmamID != 0 && personID != 0)
            {
                PersonStatus personStatus = dBContext.PersonStatus.FirstOrDefault(row =>
                                        row.TmamID == tmamID &&
                                        row.PersonID == personID);
                if (personStatus != null)
                {
                    dBContext.PersonStatus.Remove(personStatus);
                    dBContext.SaveChanges();
                }
            }
        }
    }
}