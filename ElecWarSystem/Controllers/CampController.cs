using ElecWarSystem.Models;
using ElecWarSystem.Serivces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElecWarSystem.Controllers
{
    public class CampController : Controller
    {
        private readonly CampService CampService;
        private readonly TmamService tmamService;
        private readonly UserService userService;
        private readonly RankService rankService;
        public CampController()
        {
            tmamService = new TmamService();
            userService = new UserService();
            rankService = new RankService();
            CampService = new CampService();
        }
        [HttpGet]
        public ActionResult Index()
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            List<Camp> Camps = CampService.GetAll(userId);
            ViewBag.Camp = Camps;
            String unitName = userService.GetUnitName(userId);
            ViewBag.unitName = unitName;
            List<Rank> ranks = rankService.GetAllRanks();
            ViewBag.ranks = ranks;
            ViewBag.CampTotal = CampService.getTotal(userId);
            ViewBag.CampEntered = CampService.getEntered(userId);
            Request.Cookies["tmam-title"].Value = ((int)TmamEnum.Camp).ToString();
            return View();
        }
        [HttpGet]
        public JsonResult GetCamps()
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            List<Camp> Camps = CampService.GetAll(userId);
            return Json(Camps, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public long Create(Camp Camp)
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            Camp.TmamID = tmamService.GetTmamID(new Tmam() { UnitID = userId, Date = DateTime.Today.AddDays(1) });
            return CampService.Add(Camp);
        }
        [HttpPost]
        public void Delete(long id)
        {
            CampService.Delete(id);
        }
    }
}