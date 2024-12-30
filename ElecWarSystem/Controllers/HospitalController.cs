using ElecWarSystem.Models;
using ElecWarSystem.Serivces;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace ElecWarSystem.Controllers
{
    public class HospitalController : Controller
    {
        private readonly HospitalService hospitalService;
        private readonly TmamService tmamService;
        private readonly UserService userService;
        private readonly RankService rankService;
        public HospitalController()
        {
            hospitalService = new HospitalService();
            tmamService = new TmamService();
            userService = new UserService();
            rankService = new RankService();
        }
        [HttpGet]
        public ActionResult Index()
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            List<Hospital> HospitalList = hospitalService.GetAll(userId);
            ViewBag.Hospitals = HospitalList;
            String unitName = userService.GetUnitName(userId);
            ViewBag.unitName = unitName;
            List<Rank> ranks = rankService.GetAllRanks();
            ViewBag.ranks = ranks;
            ViewBag.HospitalTotal = hospitalService.getTotal(userId);
            ViewBag.HospitalEntered = hospitalService.getEntered(userId);
            Request.Cookies["tmam-title"].Value = ((int)TmamEnum.Hospital).ToString();

            return View();
        }
        [HttpGet]
        public JsonResult GetHospital()
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            List<Hospital> HospitalList = hospitalService.GetAll(userId);
            return Json(HospitalList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public long Create(Hospital hospital)
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            
            hospital.TmamID = tmamService.GetTmamID(new Tmam() { UnitID = userId, Date = DateTime.Today.AddDays(1) });
            hospital.Tmam = new Tmam() { UnitID = userId, Date = DateTime.Today.AddDays(1) };
            return hospitalService.Add(hospital);
        }
        [HttpPost]
        public void Delete(long id)
        {
            hospitalService.Delete(id);
        }
    }
}