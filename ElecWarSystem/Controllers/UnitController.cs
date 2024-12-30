using ElecWarSystem.Data;
using ElecWarSystem.Models;
using ElecWarSystem.Serivces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElecWarSystem.Controllers
{
    public class UnitController : Controller
    {
        private readonly AppDBContext dBContext;
        private readonly PersonService personService;
        private readonly SmallUnitService smallUnitService;
        private readonly UnitService unitService;
        public UnitController()
        {
            dBContext = new AppDBContext();
            personService = new PersonService();
            smallUnitService = new SmallUnitService();
            unitService = new UnitService();
        }
        // GET: Unit
        public ActionResult DataEntry(int pg)
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            Unit unit = dBContext.Units
                .Include("UnitCommandor")
                .Include("UnitOperationsChief")
                .Include("SmallUnits")
                .Include("SmallUnits.UnitCommandor")
                .Include("SmallUnits.UnitOperationsChief")
                .FirstOrDefault(row => row.ID == userId);
            ViewBag.unit = unit;
            ViewBag.pg = pg;
            List<Rank> ranks = new List<Rank>()
            {
                new Rank{ ID = 0 , RankName = "", RankType = 1}
            };
            ranks.AddRange(dBContext.Ranks.Where(row => row.RankType == ((pg == 1 || pg == 5 ? 1 : pg - 1))).ToList());
            
            switch (pg)
            {
                case 1:
                    break;
                case 2:
                case 3:
                case 4:
                    ViewBag.Persons = dBContext.Persons.Include("Rank").Where(row => row.UnitID == userId && row.Rank.RankType == pg - 1).OrderBy(row => row.Rank.ID).ToList();
                    break;
                case 5:
                    ViewBag.suCount = unit.SmallUnits.Count;
                    break;
                default:
                    break;
            }
            ViewBag.ranks = ranks;
            return View(unit);
        }
        [HttpPost]
        public ActionResult AddUnitLeadership(Unit unitTemp, int SmallUnitCount)
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            Unit unit = dBContext.Units
                .Include("UnitCommandor")
                .Include("UnitOperationsChief")
                .FirstOrDefault(row => row.ID == userId);

            if (unit.UCID == null)
            {
                unitTemp.UnitCommandor.UnitID = userId;
                unit.UCID = personService.Add(unitTemp.UnitCommandor);
            }
            else
            {
                personService.Update(unit.UCID, unitTemp.UnitCommandor);
            }
            if (unit.UOCHID == null)
            {
                unitTemp.UnitOperationsChief.UnitID = userId;
                unit.UOCHID = personService.Add(unitTemp.UnitOperationsChief);
            }
            else
            {
                personService.Update(unit.UOCHID, unitTemp.UnitOperationsChief);
                unit.UnitOperationsChief.RankID = unitTemp.UnitOperationsChief.RankID;
                unit.UnitOperationsChief.FullName = unitTemp.UnitOperationsChief.FullName;
            }
            dBContext.SaveChanges();
            TempData["SmallUnitCount"] = SmallUnitCount;
            Response.Cookies.Add(new HttpCookie("SmallUnitCount") { Value = SmallUnitCount.ToString() });
            return RedirectToAction("DataEntry", new { pg = 5 });
        }

        [HttpPost]
        public ActionResult AddSmallUnit(Unit unitTemp)
        {
            string[] smallUnitsName = new string[] { "ك 1", "ك 2", "ك 3" };
            int userId = int.Parse(Request.Cookies["userID"].Value);
            Unit unit = dBContext.Units
                .Include("SmallUnits")
                .Include("SmallUnits.UnitCommandor")
                .Include("SmallUnits.UnitOperationsChief")
                .FirstOrDefault(row => row.ID == userId);
            int i = 0;
            foreach (SmallUnit smallUnit in unitTemp.SmallUnits)
            {
                smallUnit.UnitName = smallUnitsName[i];
                smallUnit.ParentUnitID = userId;
                smallUnit.UnitCommandor.UnitID = userId;
                smallUnit.UnitOperationsChief.UnitID = userId;

                if (i < unit.SmallUnits.Count)
                {
                    smallUnitService.Update(unit.SmallUnits[i].ID, smallUnit);
                }
                else
                {
                    unit.SmallUnits.Add(smallUnit);
                }
                i++;
                try
                {
                    dBContext.SaveChanges();
                }
                catch (Exception ex)
                {

                }
            }

            return RedirectToAction("DataEntry", new { pg = 2 });
        }

        [HttpGet]
        public JsonResult GetUnitsByZone(int zoneID)
        {
            List<Unit> units = unitService.GetByZone(zoneID);
            return Json(units, JsonRequestBehavior.AllowGet);
        }
    }
}