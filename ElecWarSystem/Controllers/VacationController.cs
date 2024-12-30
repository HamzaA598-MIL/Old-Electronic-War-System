using ElecWarSystem.Models;
using ElecWarSystem.Serivces;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace ElecWarSystem.Controllers
{
    public class VacationController : Controller
    {
        private readonly UserService userService;
        private readonly RankService rankService;
        private readonly VacationService VacationService;
        private readonly TmamService tmamService;
        public VacationController()
        {
            userService = new UserService();
            rankService = new RankService();
            VacationService = new VacationService();
            tmamService = new TmamService();
        }
        // GET: Vacation
        public ActionResult Index()
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            ViewBag.ranks = rankService.GetAllRanks();
            ViewBag.unitName = userService.GetUnitName(userId);
            ViewBag.Vacations = VacationService.GetAll(userId);
            ViewBag.VacationTotal = VacationService.getTotal(userId);
            ViewBag.VacationEntered = VacationService.getEntered(userId);
            Request.Cookies["tmam-title"].Value = ((int)TmamEnum.Vacation).ToString();

            return View();
        }
        public JsonResult GetVacation()
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            List<Vacation> Vacations = VacationService.GetAll(userId);
            return Json(Vacations, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public long Create(Vacation Vacation)
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            Vacation.TmamID = tmamService.GetTmamID(new Tmam() { UnitID = userId, Date = DateTime.Today.AddDays(1) });
            long ID = VacationService.Add(Vacation);
            return ID;
        }
        [HttpPost]
        public void Delete(long VacationID)
        {
            VacationService.Delete(VacationID);
        }

    }
}