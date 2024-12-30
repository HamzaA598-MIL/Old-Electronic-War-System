using ElecWarSystem.Data;
using ElecWarSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace ElecWarSystem.Serivces
{
    public class UnitService
    {
        private readonly AppDBContext appDBContext;
        public UnitService()
        {
            appDBContext = new AppDBContext();
        }


        public List<Unit> GetByZone(int zoneID)
        {
            List<Unit> units = appDBContext.Units.Where(row => row.zoneID == zoneID && row.Order < 50).ToList().OrderBy(u => u.Order)
                    .ToList();
            return units;
        }
        public Unit GetUnit(int id)
        {
            return appDBContext.Units.Find(id);
        }
    }
}