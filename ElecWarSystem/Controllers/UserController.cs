using ElecWarSystem.Models;
using ElecWarSystem.Serivces;
using System.Web.Mvc;
using System.Web;
using System;

public class UserController : Controller
{
    private readonly UserService userService;
    private readonly ZoneService zoneService;
    private readonly TmamService tmamService;

    public UserController()
    {
        userService = new UserService();
        zoneService = new ZoneService();
        tmamService = new TmamService();
    }
    [HttpGet]
    public ActionResult Create()
    {
        ViewBag.Zones = zoneService.GetAll();
        return View();
    }
    [HttpPost]
    public bool CreateAccount(User user, string confirmPassword,byte Role)
    {
        user.Roles = (UserRoles)Role;
        int result = userService.CreateNewUser(user, confirmPassword);
        return (result >= 1);
    }
    // Login View
    [HttpGet]
    public ActionResult Login()
    {
        return View();
    }

    // Login Validation (AJAX)
    [HttpPost]
    public JsonResult Login(string username, string password)
    {
        try
        {
            User user = userService.AuthenticateUser(username, password);
            int unitID = user != null ? user.UnitID : -1;
            var userExists = userService.UserNameIsExist(username, unitID);

            if (!userExists)
            {
                return Json(new { status = -2, message = "اسم المستخدم غير موجود" });
            }

            if (user != null)
            {
                // إنشاء ملفات تعريف الارتباط إذا كان المستخدم صحيحًا
                Response.Cookies.Add(new HttpCookie("userID") { Value = user.Unit.ID.ToString() });
                Response.Cookies.Add(new HttpCookie("userName") { Value = username });
                Response.Cookies.Add(new HttpCookie("Roles") { Value = ((byte)user.Roles).ToString() });
                Response.Cookies.Add(new HttpCookie("unitName") { Value = user.Unit.UnitName.ToString() });

                string redirectUrl;

                // توجيه المستخدم بناءً على دوره
                if (user.Roles == UserRoles.Analyzer)
                {
                    redirectUrl = Url.Action("Index", "Email");
                }
                else if ((user.Roles & UserRoles.Admin) == UserRoles.Admin ||
                         (user.Roles & UserRoles.Viewer) == UserRoles.Viewer)
                {
                    redirectUrl = Url.Action("LeaderShip", "TmamGathering");
                }
                else
                {
                    redirectUrl = Url.Action("Review", "Tmam");
                }

                // إرجاع استجابة JSON مع الرابط
                return Json(new { status = 1, redirectUrl = redirectUrl });
            }
            else
            {
                // كلمة المرور غير صحيحة
                return Json(new { status = 0, message = "كلمة المرور غير صحيحة" });
            }
        }
        catch (Exception ex)
        {
            // في حالة حدوث خطأ، يتم إرجاع رسالة تفيد بوجود مشكلة
            return Json(new { status = -1, message = "حدث خطأ أثناء محاولة تسجيل الدخول: " + ex.Message });
        }
    }



    // Logout Action
    [HttpGet]
    public ActionResult Logout()
    {
        Response.Cookies.Clear();
        return RedirectToAction("Login", "User");
    }
}
