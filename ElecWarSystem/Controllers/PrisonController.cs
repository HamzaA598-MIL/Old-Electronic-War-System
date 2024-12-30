using ElecWarSystem.Models;
using ElecWarSystem.Serivces;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;

namespace ElecWarSystem.Controllers
{
    public class PrisonController : Controller
    {
        private readonly RankService rankService;
        private readonly PrisonService prisonService;
        private readonly TmamService tmamService;
        private readonly UserService userService;
        public PrisonController()
        {
            rankService = new RankService();
            prisonService = new PrisonService();
            tmamService = new TmamService();
            userService = new UserService();
        }
        [HttpGet]
        public ActionResult Index()
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            ViewBag.prisonsList = prisonService.GetAll(userId);
            ViewBag.ranks = rankService.GetAllRanks();
            ViewBag.unitName = userService.GetUnitName(userId);
            ViewBag.TotalPrisons = prisonService.getTotal(userId);
            ViewBag.EnteredPrisons = prisonService.getEntered(userId);
            Request.Cookies["tmam-title"].Value = ((int)TmamEnum.Prison).ToString();

            return View();
        }
        [HttpGet]
        public JsonResult GetPrisons()
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            List<Prison> prisons = prisonService.GetAll(userId);
            return Json(prisons, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public long Create(Prison prison)
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            prison.TmamID = tmamService.GetTmamID(new Tmam() { UnitID = userId, Date = DateTime.Today.AddDays(1) });
            return prisonService.Add(prison);
        }
        [HttpPost]
        public void Delete(long id)
        {
            prisonService.Delete(id);
        }
    }
}