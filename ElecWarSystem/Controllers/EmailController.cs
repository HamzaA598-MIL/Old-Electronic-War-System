using ElecWarSystem.Data;
using ElecWarSystem.Models;
using ElecWarSystem.Serivces;
using ElecWarSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using Reciever = ElecWarSystem.Models.Reciever;
using Microsoft.Ajax.Utilities;
using static System.Web.Razor.Parser.SyntaxConstants;
using iText.Kernel.Geom;

namespace ElecWarSystem.Controllers
{
    public class EmailController : Controller
    {
        private readonly AppDBContext dBContext;
        private readonly UserService userService;
        private readonly EmailService emailService;
        private readonly StorageManager storageManager;
        private readonly TmamGatheringService tmamGatheringServices;
        private String[] dangerExtension;

        public EmailController()
        {
            dBContext = new AppDBContext();
            userService = new UserService();
            emailService = new EmailService();
            storageManager = new StorageManager();
            tmamGatheringServices = new TmamGatheringService();
            dangerExtension = new[]{ "pdf", "dot", "dotx", "docm", "docx",
                "doc", "png", "jpeg", "jpg", "tif","pptx","pptm",
                "ppt", "potx", "accdb", "mdb", "xlsx", "xls", "xlsm",
                "csv", "zip", "rar", "mp3","aac", "oog", "wav", "mp4",
                "mov", "wmv", "avi", "mkv"};
        }

        // GET: Emails
        public ActionResult Index()
        {
            if (Request.Cookies["userID"] != null)
            {
                int unitId = int.Parse(Request.Cookies["userID"].Value);
                ViewBag.Capacity = storageManager.getCapacityPerUnit(unitId);
                ViewBag.Used = storageManager.getUsedPerUnit(unitId);
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        public JsonResult GetEmails(bool export)
        {
            if (Request.Cookies["userID"] != null)
            {
                int userId = int.Parse(Request.Cookies["userID"].Value);
                if (export)
                {
                    List<Email> emails = emailService.GetExportedEmails(userId);
                    return Json(emails, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<Reciever> recievers = emailService.GetRecievers(userId);
                    return Json(recievers, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("UnAuthorized Request!");
            }
        }

        // GET: Email/Details/5
        public ActionResult Details(int id)
        {
            Email email = dBContext.Emails.Include("Documents").Include("Recievers.RecieverUser").Include("Sender").FirstOrDefault(row => row.ID == id);

            int userId = int.Parse(Request.Cookies["userID"].Value);
            Reciever reciever = email.Recievers.FirstOrDefault(row => row.RecieverID == userId);
            var unitComandor = tmamGatheringServices.GetAllAltCommandor();
            if (reciever != null)
            {
                if (!reciever.Readed)
                {
                    reciever.Readed = true;
                    reciever.ViewTime = DateTime.Now;
                    dBContext.SaveChanges();
                }
            }
            return View(email);
        }

        [HttpPost]
        public JsonResult UpdateViewTime(long emailId)
        {
            var email = dBContext.Emails.Include("Recievers").FirstOrDefault(e => e.ID == emailId);
            int userId = int.Parse(Request.Cookies["userID"].Value);

            if (email != null)
            {
                var reciever = email.Recievers.FirstOrDefault(r => r.RecieverID == userId);
                if (reciever != null)
                {
                    reciever.ViewTime = DateTime.Now; // تحديث وقت المشاهدة
                    reciever.Readed = true; // وضع علامة على البريد كمقروء
                    dBContext.SaveChanges(); // حفظ التغييرات
                }
            }

            return Json(new { success = true });
        }

        // GET: Email/Create
        public ActionResult Create()
        {
            EmailViewModel emailViewModel = new EmailViewModel();
            emailViewModel.Email = new Email();
            return View(emailViewModel);
        }

        [HttpGet]
        public JsonResult CountOfUnReadEmails()
        {
            int userId = int.Parse(Request.Cookies["userID"]?.Value);
            int unreadCount = emailService.GetCountOfUnReadEmails(userId);
            return Json(unreadCount, JsonRequestBehavior.AllowGet);
        }

        // POST: Email/Create
        [HttpPost]
        public ActionResult Create(EmailViewModel emailViewModel, IEnumerable<HttpPostedFileBase> files)
        {
            int userId = int.Parse(Request.Cookies["userID"]?.Value);
            emailViewModel.Email.Documents = new List<Document>();
            emailViewModel.Email.SenderUserID = userId;
            emailViewModel.Email.SendDateTime = DateTime.Now;

            List<HttpPostedFileBase> attachment = files.ToList();
            string unitName = userService.GetUnitName(userId); // تأكد من الحصول على معرف الوحدة بناءً على معرف المستخدم
            string serverPath = System.IO.Path.Combine(@"D:\\FileShare", unitName);
            long allContentSize = 0;

            // تأكد من إنشاء المجلد إذا لم يكن موجوداً
            if (!Directory.Exists(serverPath))
            {
                Directory.CreateDirectory(serverPath);
            }

            foreach (string id in emailViewModel.RecIds)
            {
                emailViewModel.Email.Recievers.Add(new Reciever { RecieverID = int.Parse(id) });
            }

            Dictionary<string, HttpPostedFileBase> filePaths = new Dictionary<string, HttpPostedFileBase>();

            int fileCounter = 1; // عداد لتمييز الملفات المكررة
            string dateFormat = DateTime.Now.ToString("yyyyMMdd"); // تنسيق التاريخ

            foreach (HttpPostedFileBase file in attachment)
            {
                // استخدم اسم الفاكس أو البريد كاسم للملف
                string faxName = emailViewModel.Email.Subject ?? "DefaultFaxName";
                faxName = string.Join("_", faxName.Split(System.IO.Path.GetInvalidFileNameChars())); // إزالة الأحرف غير المسموح بها في اسم الملف

                string fileName = file.FileName; // اسم الملف الأصلي الذي سيتم استخدامه في النظام
                string[] fileAtt = fileName.Split('.');

                if (!dangerExtension.Contains(fileAtt.Last()))
                {
                    emailViewModel.Message = $"عفواً لا يمكنك إرسال ملفات بإمتداد ({fileAtt.Last()}) حسب تعليمات الأمن السيبرانى";
                    return View(emailViewModel);
                }
                else
                {
                    string fileExtension = fileAtt.Last();

                    // حدد الاسم المبدئي للملف مع إضافة التاريخ على "File Share"
                    string finalFileName = $"{faxName}_{dateFormat}.{fileExtension}";
                    string filePath = System.IO.Path.Combine(serverPath, finalFileName);

                    // تحقق مما إذا كان هناك ملف بنفس الاسم وأضف رقم تسلسلي إذا لزم الأمر
                    while (filePaths.ContainsKey(filePath))
                    {
                        finalFileName = $"{faxName}_{dateFormat}_{fileCounter}.{fileExtension}";
                        filePath = System.IO.Path.Combine(serverPath, finalFileName);
                        fileCounter++;
                    }

                    allContentSize += file.ContentLength;
                    filePaths[filePath] = file; // حفظ الملف في المسار

                    // إضافة الملف إلى قائمة المستندات باستخدام الاسم الأصلي كما هو للعرض في النظام
                    emailViewModel.Email.Documents.Add(new Document
                    {
                        FileName = fileName, // عرض الاسم الأصلي كما هو في النظام
                        FileExtension = fileExtension,
                        FilePath = filePath // المسار الذي تم حفظ الملف فيه (مع التاريخ في الاسم)
                    });
                }
            }




            if (storageManager.increaseUsed(userId, allContentSize))
            {
                // Generate new sequential ID for the email

                dBContext.Emails.Add(emailViewModel.Email);
                dBContext.SaveChanges();
                foreach (var filepath in filePaths)
                {
                    filepath.Value.SaveAs(filepath.Key);
                }
                return RedirectToAction("Index", "Email");
            }
            else
            {
                emailViewModel.Message = "عفواً المساحة المتاحة لك لا تكفى!!";
                return View(emailViewModel);
            }
        }

        [HttpPost]
        public void StarEmail(int id)
        {
            int userID = int.Parse(Request.Cookies["userID"].Value);
            Reciever reciever = dBContext.Recievers.FirstOrDefault(row => row.EmailID == id && row.RecieverID == userID);
            reciever.Starred = !reciever.Starred;
            dBContext.SaveChanges();
        }

        // POST: Email/Delete/5
        [HttpPost]
        public JsonResult Delete(int id)
        {
            int status = 0;
            if (Request.Cookies["userID"]?.Value != null)
            {
                int userId = int.Parse(Request.Cookies["userID"]?.Value);
                long contentSize = 0;
                Email email = dBContext.Emails.Include("Documents").FirstOrDefault(row => row.SenderUserID == userId && row.ID == id);
                foreach (Document document in email?.Documents)
                {
                    if (System.IO.File.Exists($@"{document.FilePath}"))
                    {
                        contentSize += new FileInfo(document.FilePath).Length; //System.IO.File.ReadAllBytes().Length;
                        System.IO.File.Delete($@"{document.FilePath}");
                    }
                }
                dBContext.Emails.Remove(email);
                dBContext.SaveChanges();
                storageManager.decreaseUsed(userId, contentSize);
                status = 200;
            }
            else
            {
                status = 404;
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }



    }
}
