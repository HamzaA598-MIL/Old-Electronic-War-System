using ElecWarSystem.Data;
using ElecWarSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace ElecWarSystem.Serivces
{
    public class ZoneService
    {
        private readonly AppDBContext appDBContext;
        public ZoneService()
        {
            appDBContext = new AppDBContext();
        }
        public List<Zone> GetAll()
        {
            List<Zone> zones = new List<Zone>();
            zones.Add(new Zone { ID = 0, ZoneName = "", ZoneAlias = "" });
            zones.AddRange(appDBContext.Zones.ToList());
            return zones;
        }
        public List<Zone> GetZones()
        {
            List<Zone> zones = appDBContext.Zones
                                           .OrderBy(z => z.Order) // ترتيب حسب الحقل Order
                                           .ToList();
            return zones;
        }

    }
}