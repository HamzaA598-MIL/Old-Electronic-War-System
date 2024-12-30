using ElecWarSystem.Models;
using ElecWarSystem.Serivces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElecWarSystem.Controllers
{
    public class OutOfCountryController : Controller
    {
        private readonly OutOfCountryService outOfCountryService;
        private readonly TmamService tmamService;
        private readonly UserService userService;
        private readonly RankService rankService;
        public OutOfCountryController()
        {
            tmamService = new TmamService();
            userService = new UserService();
            rankService = new RankService();
            outOfCountryService= new OutOfCountryService();
        }
        // GET: OutOfCountry
        [HttpGet]
        public ActionResult Index()
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            List<OutOfCountry> outOfCountries = outOfCountryService.GetAll(userId);
            ViewBag.outOfCountry = outOfCountries;
            String unitName = userService.GetUnitName(userId);
            ViewBag.unitName = unitName;
            List<Rank> ranks = rankService.GetAllRanks();
            ViewBag.ranks = ranks;
            ViewBag.outOfCountryTotal = outOfCountryService.getTotal(userId);
            ViewBag.outOfCountryEntered = outOfCountryService.getEntered(userId);
            Request.Cookies["tmam-title"].Value = ((int)TmamEnum.OutOfCountry).ToString();
            return View();
        }
        [HttpGet]
        public JsonResult GetOutOfCountries()
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            List<OutOfCountry> outOfCountries = outOfCountryService.GetAll(userId);
            return Json(outOfCountries, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public long Create(OutOfCountry outOfCountry)
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            outOfCountry.TmamID = tmamService.GetTmamID(new Tmam() { UnitID = userId, Date = DateTime.Today.AddDays(1) });
            return outOfCountryService.Add(outOfCountry);
        }
        [HttpPost]
        public void Delete(long id)
        {
            outOfCountryService.Delete(id);
        }
    }
}