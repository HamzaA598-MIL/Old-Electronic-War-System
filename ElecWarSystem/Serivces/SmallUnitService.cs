using ElecWarSystem.Data;
using ElecWarSystem.Models;

namespace ElecWarSystem.Serivces
{
    public class SmallUnitService : IDBRepository<SmallUnit>
    {
        private readonly AppDBContext appDBContext;
        private readonly PersonService personService;
        public SmallUnitService()
        {
            appDBContext = new AppDBContext();
            personService = new PersonService();
        }
        public long? Add(SmallUnit smallUnit)
        {
            appDBContext.SmallUnits.Add(smallUnit);
            appDBContext.SaveChanges();
            return smallUnit.ID;
        }

        public void Delete(long? id)
        {
            appDBContext.SmallUnits.Remove(Find(id));
            appDBContext.SaveChanges();
        }

        public SmallUnit Find(long? id)
        {
            return appDBContext.SmallUnits.Find(id);
        }

        public void Update(long? id, SmallUnit entity)
        {
            SmallUnit smallUnit = Find(id);
            smallUnit.UnitName = entity.UnitName;
            personService.Update(smallUnit.UCID, entity.UnitCommandor);
            personService.Update(smallUnit.UOCHID, entity.UnitOperationsChief);
            appDBContext.SaveChanges();
        }
    }
}