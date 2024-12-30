using ElecWarSystem.Models;
using ElecWarSystem.Serivces;
using System;
using System.Diagnostics;
using System.Web.Mvc;

namespace ElecWarSystem.Controllers
{
    public class TmamGatheringController : Controller
    {
        private TmamService tmamService;
        private TmamGatheringService tmamGatheringService;
        public TmamGatheringController()
        {
            tmamService = new TmamService();
            tmamGatheringService = new TmamGatheringService();
        }
        public void initViewer()
        {
            UserRoles userRoles = (UserRoles)byte.Parse(Request.Cookies["Roles"].Value);

            if ((userRoles & UserRoles.Viewer) == UserRoles.Viewer &&
                    (userRoles & UserRoles.Admin) != UserRoles.Admin)
            {
                DateTime dateTime = DateTime.Today;
                tmamGatheringService = new TmamGatheringService(dateTime);
                tmamService = new TmamService(dateTime);
            }
        }
        [HttpGet]
        public ActionResult LeaderShip()
        {
            UserRoles userRoles = (UserRoles)byte.Parse(Request.Cookies["Roles"].Value);
            
            initViewer();
            
            if ((userRoles & UserRoles.Viewer) == UserRoles.Viewer ||
                (userRoles & UserRoles.Admin) == UserRoles.Admin)
            {
                ViewBag.leadersTmams = tmamGatheringService.GetAllLeaderTmam();
                ViewBag.altCommandors = tmamGatheringService.GetAllAltCommandor();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpGet]
        public ActionResult RecievedTmam()
        {
            ViewBag.SubmittedTmams = tmamGatheringService.GetTmamsSubmitted();
            return View();
        }
        [HttpGet]
        public ActionResult Officers()
        {
            initViewer();
            ViewBag.ZoneUnitsTmam = tmamGatheringService.GetOfficersTmam(IsOfficers: true);
            return View();
        }
        [HttpGet]
        public ActionResult NonOfficers()
        {
            initViewer();
            ViewBag.ZoneUnitsTmam = tmamGatheringService.GetOfficersTmam(IsOfficers: false);
            return View();
        }
        [HttpGet]
        public ActionResult SickLeaves()
        {
            initViewer();
            ViewBag.sickLeaves = tmamGatheringService.GetSickLeavesTmam();
            return View();
        }

        
        [HttpGet]
        public ActionResult Errands()
        {
            initViewer();

            ViewBag.Errands = tmamGatheringService.GetErrandsTmam();
            return View();
        }
        [HttpGet]
        public ActionResult Prison()
        {
            initViewer();
            ViewBag.Prisons = tmamGatheringService.GetPrisonsTmam();
            return View();
        }
        [HttpGet]
        public ActionResult Absence()
        {
            initViewer();
            ViewBag.Absences = tmamGatheringService.GetAbsencesTmam();
            return View();
        }
        [HttpGet]
        public ActionResult Hospital()
        {
            initViewer();
            ViewBag.Hospitals = tmamGatheringService.GetHospitalsTmam();
            return View();
        }
        public ActionResult OutOfCountry()
        {
            initViewer();
            ViewBag.OutOfCountries = tmamGatheringService.GetOutOfCountriesTmam();
            return View();
        }
        public ActionResult Camps()
        {
            initViewer();
            ViewBag.Camps = tmamGatheringService.GetCampsTmam();
            return View();
        }
        public ActionResult Courses()
        {
            initViewer();
            ViewBag.Courses = tmamGatheringService.GetCoursesTmam();
            return View();
        }
        [HttpGet]
        public ActionResult TmamDetails(int id)
        {
            ViewBag.tmam = tmamService.GetTmamWithAllDetails(id);
            ViewBag.LeaderTmam = tmamService.GetLeaderTmamPerUnit(id);
            return View();
        }
        [HttpPost]
        public void MakeTmamRecive(int unitID)
        {
            tmamService.ReciveTmam(unitID);
        }
        [HttpPost]
        public void MakeTmamReturn(int unitID)
        {
            tmamService.ReturnTmam(unitID);
        }
    }
}