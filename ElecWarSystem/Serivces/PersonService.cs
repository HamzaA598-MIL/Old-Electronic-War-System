using ElecWarSystem.Data;
using ElecWarSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElecWarSystem.Serivces
{
    public class PersonService : IDBRepository<Person>
    {
        private AppDBContext dBContext;
        
        
        
        
        
        public PersonService()
        {
            dBContext = new AppDBContext();
        }
        public List<Person> GetAllPerson(int unitID)
        {
            List<Person> persons = dBContext.Persons
                .Where(row => row.UnitID == unitID)

                .ToList();
            return persons;
        }
        public List<Person> GetPersons(int unitID, int type)
        {
            List<Person> persons = dBContext.Persons
                .Include("Rank")
                .Where(row => row.UnitID == unitID && row.Rank.RankType == type && !row.Status)
                .OrderBy(row => row.RankID).ThenBy(row => row.FullName)
                .ToList();
            return persons;
        }
        public int GetPersonsCount(int unitID, int type)
        {
            int count = dBContext.Persons.Where(row => row.UnitID == unitID && row.Rank.RankType == type).Count();

            return count;
        }

        public Person Find(long? id)
        {
            Person person = dBContext.Persons.FirstOrDefault(row => row.ID == id);
            return person;
        }
        public long? Add(Person person)
        {
            dBContext.Persons.Add(person);
            dBContext.SaveChanges();
            return person.ID;
        }
        public List<Person> GetPersonOfRank(int userId, int rankId)
        {

            List<Person> persons = dBContext.Persons
                .Where(row => row.RankID == rankId && row.UnitID == userId)
                .OrderBy(row => row.FullName)
                .ToList();
            return persons;
        }
        public void Update(long? id, Person person)
        {
            Person personTemp = dBContext.Persons.FirstOrDefault(row => row.ID == id);
            personTemp.RankID = person.RankID;
            personTemp.FullName = person.FullName;
            personTemp.MilID = person.MilID;
            personTemp.onDuty = person.onDuty;
            dBContext.SaveChanges();
        }

        public bool Delete(long? id)
        {
            Person person = dBContext.Persons.FirstOrDefault(row => row.ID == id);
            int result = this.PersonIsLeader(person.ID);
            if (result == 0)
            {
                List<Tmam> tmams = dBContext.Tmams.Where(row => row.AltCommanderID == id && row.Date < DateTime.Today ).ToList();
                dBContext.Tmams.RemoveRange(tmams);
                dBContext.SaveChanges();
                dBContext.Persons.Remove(person);
                dBContext.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public int PersonIsLeader(long id)
        {
            int result = 0;
            Unit unit = dBContext.Persons.Include("Unit.SmallUnits").FirstOrDefault(row => row.ID == id).Unit;
            if (id == unit.UCID)
                result = 1;
            else if (id == unit.UOCHID)
                result = 2;
            else
            {
                for (int i = 0; i < unit.SmallUnits.Count; i++)
                {
                    if (id == unit.SmallUnits[i].UCID)
                    {
                        result = i + 3;
                        break;
                    }
                    else if (id == unit.SmallUnits[i].UOCHID)
                    {
                        result = i + 4;
                        break;
                    }
                }
            }
            return result;
        }
        
        void IDBRepository<Person>.Delete(long? id)
        {
            throw new NotImplementedException();
        }
        public void ResetPersonsStatus(int unitId)
        {
            List<Person> people = GetAllPerson(unitId);
            foreach (Person person in people)
            {
                person.Status = false;
            }
            dBContext.SaveChanges();
        }
        public void setStatus(long personID)
        {
            Person person = dBContext.Persons.Find(personID);
            person.Status = true;
            dBContext.SaveChanges();
        }
        public void unCheckStatus(long personID)
        {
            Person person = dBContext.Persons.Find(personID);
            person.Status = false;
            dBContext.SaveChanges();
        }
    }
}