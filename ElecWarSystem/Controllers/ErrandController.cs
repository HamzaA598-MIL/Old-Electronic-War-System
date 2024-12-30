using ElecWarSystem.Models;
using ElecWarSystem.Serivces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElecWarSystem.Controllers
{
    public class ErrandController : Controller
    {
        private readonly UserService userService;
        private readonly RankService rankService;
        private readonly ErrandsService errandsService;
        private readonly TmamService tmamService;
        public ErrandController()
        {
            userService = new UserService();
            rankService = new RankService();
            errandsService = new ErrandsService();
            tmamService = new TmamService();
        }

        // GET: Errand
        public ActionResult Index()
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);

            ViewBag.ranks = rankService.GetAllRanks();
            ViewBag.unitName = userService.GetUnitName(userId);

            // Fetch and sort errands by rank name before passing to the view
            ViewBag.errandsList = errandsService.GetAll(userId)
                                .OrderBy(e => e.ErrandDetail.Person.RankID).ToList();

            ViewBag.TotalErrands = errandsService.getTotal(userId);
            ViewBag.EnteredErrands = errandsService.getEntered(userId);

            Request.Cookies["tmam-title"].Value = ((int)TmamEnum.Errand).ToString();

            return View();
        }


        public JsonResult GetErrands()
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            List<Errand> errands = errandsService.GetAll(userId);
            return Json(errands, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public long Create(Errand errand)
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            errand.TmamID = tmamService.GetTmamID(new Tmam() { UnitID = userId, Date = DateTime.Today.AddDays(1) });
            return errandsService.Add(errand);
        }

        [HttpPost]
        public void Delete(long errandID)
        {
            errandsService.Delete(errandID);
        }

       

    }
}
