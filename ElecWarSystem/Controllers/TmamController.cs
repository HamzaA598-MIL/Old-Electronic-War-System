using ElecWarSystem.Models;
using ElecWarSystem.ReportFactory;
using ElecWarSystem.Serivces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;

namespace ElecWarSystem.Controllers
{
    public class TmamController : Controller
    {
        private readonly TmamService tmamService;
        private readonly RankService rankService;
        private readonly UserService userService;
        private readonly PersonService personService;
        public TmamController()
        {
            tmamService = new TmamService();
            rankService = new RankService();
            userService = new UserService();
            personService = new PersonService();
        }
        // GET: Tmam
        [HttpGet]
        public ActionResult Index(int pg)
        {
            ViewBag.pg = pg;
            int userId = int.Parse(Request.Cookies["userID"].Value);
            ViewBag.unitName = userService.GetUnitName(userId);
            bool isOfficers = true;
            switch (pg)
            {
                case 1:
                    isOfficers = true;
                    Request.Cookies["tmam-title"].Value = ((int)TmamEnum.Officers).ToString();
                    Person altCommandor = tmamService.GetAltCommandorToday(userId);
                    ViewBag.AltCommandor = altCommandor;
                    ViewBag.AltcommandorPersons = personService.GetPersonOfRank(userId, (int)altCommandor.RankID);
                    break;
                case 2:
                    isOfficers = false;
                    Request.Cookies["tmam-title"].Value = ((int)TmamEnum.OtherRanks).ToString();
                    break;
                default:
                    break;
            }
            
            ViewBag.TmamDetail = tmamService.GetTmamDetails(userId, isOfficers);
            ViewBag.ranks = rankService.GetRanksOf(1);
            return View();
        }
        [HttpPost]
        public void AddTmamDetail(TmamDetail tmamDetail)
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            tmamDetail.Tmam.Date = DateTime.Today.AddDays(1);
            tmamDetail.Tmam.UnitID = userId;
            tmamService.AddTmamDetail(tmamDetail);
        }
        public ActionResult Review()
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            ViewBag.unitName = userService.GetUnitName(userId);
            ViewBag.tmam = tmamService.GetTmamWithAllDetails(userId);
            ViewBag.LeaderTmam = tmamService.GetLeaderTmamPerUnit(userId);
            
            Tmam tmam = tmamService.GetTmam(userId);

            Tmam tmm = tmamService.GetTmam(userId);
            bool isSubmited = false;
            if (tmam != null)
            {
                isSubmited = tmm.Submitted;
            }
            Response.Cookies.Add(new HttpCookie("tmamSubmitStatus") { Value = isSubmited.ToString() });
            
            Response.Cookies.Add(new HttpCookie("tmam-title") { Value = ((int)TmamEnum.AllTmam).ToString() });
            return View();
        }
        public ActionResult ReviewReport()
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            var tmam = tmamService.GetTmamWithAllDetails(userId);
            var LeaderTmam = tmamService.GetLeaderTmamPerUnit(userId);
            string title = Utilites.numbersA2E($"تمام {tmam.Unit.UnitName}");
            ReviewReport reviewReport = new ReviewReport(tmam, LeaderTmam, tmam.Date, title);
            byte[] bytes = reviewReport.PrepareReport();
            return File(bytes, "application/pdf", $"{title}.pdf");
        }
        [HttpPost]
        public String SubmitTmam()
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            return tmamService.SubmitTmam(userId);
        }
        [HttpGet]
        public JsonResult GetTmamStatus()
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            Dictionary<String, bool> tmamStatus = tmamService.GetTmamStatus(userId);
            return Json(tmamStatus, JsonRequestBehavior.AllowGet);
        }
    }
}