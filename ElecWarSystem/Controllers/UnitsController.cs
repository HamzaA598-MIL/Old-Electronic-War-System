using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ElecWarSystem.Data;
using ElecWarSystem.Models;

namespace ElecWarSystem.Controllers
{
    public class UnitsController : Controller
    {
        private AppDBContext db = new AppDBContext();

        // GET: Units
        public async Task<ActionResult> Index()
        {
            var units = db.Units.Include(u => u.UnitCommandor).Include(u => u.UnitOperationsChief).Include(u => u.zone);
            return View(await units.ToListAsync());
        }

        // GET: Units/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Unit unit = await db.Units.FindAsync(id);
            if (unit == null)
            {
                return HttpNotFound();
            }
            return View(unit);
        }

        // GET: Units/Create
        public ActionResult Create()
        {
            ViewBag.UCID = new SelectList(db.Persons, "ID", "MilID");
            ViewBag.UOCHID = new SelectList(db.Persons, "ID", "MilID");
            ViewBag.zoneID = new SelectList(db.Zones, "ID", "ZoneName");
            return View();
        }

        // POST: Units/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<ActionResult> Create(Unit unit)
        {
            if (ModelState.IsValid)
            {
                // توليد ID يدويًا
                unit.ID = GenerateUnitID();

                db.Units.Add(unit);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    // تسجيل التفاصيل لفهم المشكلة
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    System.Diagnostics.Debug.WriteLine(ex.InnerException?.Message);
                    throw;
                }
                return RedirectToAction("Index");
            }

            ViewBag.UCID = new SelectList(db.Persons, "ID", "MilID", unit.UCID);
            ViewBag.UOCHID = new SelectList(db.Persons, "ID", "MilID", unit.UOCHID);
            ViewBag.zoneID = new SelectList(db.Zones, "ID", "ZoneName", unit.zoneID);
            return View(unit);
        }
        private int GenerateUnitID()
        {
            try
            {
                var lastID = db.Units.Max(u => (int?)u.ID) ?? 0;
                return lastID + 1;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error generating ID: " + ex.Message);
                throw;
            }
        }


        // GET: Units/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Unit unit = await db.Units.FindAsync(id);
            if (unit == null)
            {
                return HttpNotFound();
            }
            ViewBag.UCID = new SelectList(db.Persons, "ID", "MilID", unit.UCID);
            ViewBag.UOCHID = new SelectList(db.Persons, "ID", "MilID", unit.UOCHID);
            ViewBag.zoneID = new SelectList(db.Zones, "ID", "ZoneName", unit.zoneID);
            return View(unit);
        }

        // POST: Units/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,zoneID,UnitName,AllowedStrogeSize,UsedStrogeSize,Order,AltComExist,UCID,UOCHID")] Unit unit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(unit).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.UCID = new SelectList(db.Persons, "ID", "MilID", unit.UCID);
            ViewBag.UOCHID = new SelectList(db.Persons, "ID", "MilID", unit.UOCHID);
            ViewBag.zoneID = new SelectList(db.Zones, "ID", "ZoneName", unit.zoneID);
            return View(unit);
        }

        // GET: Units/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Unit unit = await db.Units.FindAsync(id);
            if (unit == null)
            {
                return HttpNotFound();
            }
            return View(unit);
        }

        // POST: Units/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Unit unit = await db.Units.FindAsync(id);
            db.Units.Remove(unit);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
