using ElecWarSystem.Models;
using ElecWarSystem.Models.OutDoor;
using ElecWarSystem.Models.OutDoorDetails;
using ElecWarSystem.Serivces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace ElecWarSystem.Controllers
{
    public class CourseController : Controller
    {
        private readonly IOutingRepository<Course, CourseDetails> courseService;
        private readonly TmamService tmamService;
        private readonly UserService userService;
        private readonly RankService rankService;
        private readonly PersonStatusService personStatusService;

        public CourseController()
        {
            tmamService = new TmamService();
            userService = new UserService();
            rankService = new RankService();
            personStatusService = new PersonStatusService();
            courseService = new OutingService<Course, CourseDetails>();
        }
        [HttpGet]
        public ActionResult Index()
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);

            String unitName = userService.GetUnitName(userId);
            ViewBag.unitName = unitName;

            List<Rank> ranks = rankService.GetAllRanks();
            ViewBag.ranks = ranks;

            return View();
        }
        [HttpGet]
        public JsonResult GetCourses()
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            Tmam tmam = new Tmam() { UnitID = userId, Date = DateTime.Today.AddDays(1) };
            long tmamID = tmamService.AddTmam(tmam);

            List<Course> courses =
                courseService.GetAll(
                    row => row.TmamID == tmamID,
                    new[] { "CourseDetails.Person.Rank",
                            "CourseDetails.CommandItem"}
                ).ToList();

            return Json(courses, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public long Create(Course course)
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            Tmam tmam = new Tmam() { UnitID = userId, Date = DateTime.Today.AddDays(1) };
            course.TmamID = tmamService.GetTmamID(tmam);
            course.Tmam = tmam;
            if (course.IsDateLogic())
            {
                personStatusService.setPersonStatus(new PersonStatus
                {
                    PersonID = course.CourseDetails.PersonID,
                    TmamID = course.TmamID,
                    Status = TmamEnum.Course
                });
                course.CleanNav();
                courseService.Add(course);
                return 0;
            }
            else
            {
                return -1;
            }
        }
        [HttpPost]
        public void Delete(long id)
        {
            Course course = courseService.Get(row => row.ID == id, new[] { "CourseDetails.CommandItem" });
            long personID = course.CourseDetails.PersonID;
            personStatusService.DeletePersonStatus(course.TmamID, personID);
            courseService.Delete(row => row.ID == course.ID);
        }
        [HttpGet]
        public JsonResult GetNumbers()
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            int Total = tmamService.GetTmamDetails(userId, true).course +
                        tmamService.GetTmamDetails(userId, false).course;

            Tmam expectedTmam = new Tmam() { UnitID = userId, Date = DateTime.Today.AddDays(1) };
            long tmamID = tmamService.GetTmamID(expectedTmam);

            int entered = courseService.GetCount(row => row.TmamID == tmamID);

            Dictionary<String, int> numbers = new Dictionary<string, int>();
            numbers.Add("total", Total);
            numbers.Add("entered", entered);
            return Json(numbers, JsonRequestBehavior.AllowGet);
        }
    }
}