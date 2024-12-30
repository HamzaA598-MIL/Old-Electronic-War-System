using ElecWarSystem.Models;
using ElecWarSystem.ReportFactory;
using ElecWarSystem.Serivces;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Web.Mvc;

namespace ElecWarSystem.Controllers
{
    public class SavedTmamController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }


    }
}
