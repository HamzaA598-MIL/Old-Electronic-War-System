using ElecWarSystem.Models;
using ElecWarSystem.Serivces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;

namespace ElecWarSystem.Controllers
{
    public class AbsenceController : Controller
    {
        private readonly UserService userService;
        private readonly RankService rankService;
        private readonly AbsenceService AbsencesService;
        private readonly TmamService tmamService;
        public AbsenceController()
        {
            userService = new UserService();
            rankService = new RankService();
            AbsencesService = new AbsenceService();
            tmamService = new TmamService();
        }
        public ActionResult Index()
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            ViewBag.ranks = rankService.GetAllRanks();
            ViewBag.unitName = userService.GetUnitName(userId);
            ViewBag.AbsencesList = AbsencesService.GetAll(userId);
            ViewBag.TotalAbsences = AbsencesService.getTotal(userId);
            ViewBag.EnteredAbsences = AbsencesService.getEntered(userId);
            Request.Cookies["tmam-title"].Value = ((int)TmamEnum.Absence).ToString();
            
            return View();
        }
        public JsonResult GetAbsences()
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            List<Absence> Absences = AbsencesService.GetAll(userId);
            return Json(Absences, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public long Create(Absence Absence)
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            Absence.TmamID = tmamService.GetTmamID(new Tmam() { UnitID = userId, Date = DateTime.Today.AddDays(1) });
            return AbsencesService.Add(Absence);
        }
        [HttpPost]
        public void Delete(long AbsenceID)
        {
            AbsencesService.Delete(AbsenceID);
        }
    }
}