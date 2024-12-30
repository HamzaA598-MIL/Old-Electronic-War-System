using ElecWarSystem.Models;
using ElecWarSystem.Serivces;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace ElecWarSystem.Controllers
{
    public class SickLeaveController : Controller
    {
        private readonly SickLeaveService sickLeaveService;
        private readonly TmamService tmamService;
        private readonly UserService userService;
        private readonly RankService rankService;
        public SickLeaveController()
        {
            sickLeaveService = new SickLeaveService();
            tmamService = new TmamService();
            userService = new UserService();
            rankService = new RankService();
        }
        // GET: SickLeave
        [HttpGet]
        public ActionResult Index()
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            List<SickLeave> sickLeaveList = sickLeaveService.GetAll(userId);
            ViewBag.SickLeaves = sickLeaveList;
            String unitName = userService.GetUnitName(userId);
            ViewBag.unitName = unitName;
            List<Rank> ranks = rankService.GetAllRanks();
            ViewBag.ranks = ranks;
            ViewBag.sickLeaveTotal = sickLeaveService.getTotal(userId);
            ViewBag.sickLeaveEntered = sickLeaveService.getEntered(userId);
            Request.Cookies["tmam-title"].Value = ((int)TmamEnum.SickLeave).ToString();

            return View();
        }
        [HttpGet]
        public JsonResult GetSickLeave()
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            List<SickLeave> sickLeaveList = sickLeaveService.GetAll(userId);
            return Json(sickLeaveList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public long Create(SickLeave sickLeave)
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            sickLeave.TmamID = tmamService.GetTmamID(new Tmam() { UnitID = userId, Date = DateTime.Today.AddDays(1) });
            return sickLeaveService.Add(sickLeave);
        }
        [HttpPost]
        public void Delete(long id)
        {
            sickLeaveService.Delete(id);
        }
    }
}