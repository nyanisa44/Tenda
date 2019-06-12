using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TendaAdvisors.Models;

namespace TendaAdvisors.Controllers
{
    //TODO Why is this not api?
    public class AnnualTaxBracketsController : Controller
    {
        private ApplicationDbContext db;// = new ApplicationDbContext();

        public AnnualTaxBracketsController(ApplicationDbContext dbcontext) {
            db = dbcontext;
        }

        // GET: AnnualTaxBrackets
        public async Task<ActionResult> Index()
        {
            return View(await db.AnnualTaxBrackets.ToListAsync());
        }

        // GET: AnnualTaxBrackets/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AnnualTaxBracket annualTaxBracket = await db.AnnualTaxBrackets.FindAsync(id);
            if (annualTaxBracket == null)
            {
                return HttpNotFound();
            }
            return View(annualTaxBracket);
        }

        // GET: AnnualTaxBrackets/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AnnualTaxBrackets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "AnnualTaxBracketId,year,type,rate,MinIncome,MaxIncome,Threshold,Basic")] AnnualTaxBracket annualTaxBracket)
        {
            if (ModelState.IsValid)
            {
                db.AnnualTaxBrackets.Add(annualTaxBracket);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(annualTaxBracket);
        }

        // GET: AnnualTaxBrackets/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AnnualTaxBracket annualTaxBracket = await db.AnnualTaxBrackets.FindAsync(id);
            if (annualTaxBracket == null)
            {
                return HttpNotFound();
            }
            return View(annualTaxBracket);
        }

        // POST: AnnualTaxBrackets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AnnualTaxBracketId,year,type,rate,MinIncome,MaxIncome,Threshold,Basic")] AnnualTaxBracket annualTaxBracket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(annualTaxBracket).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(annualTaxBracket);
        }

        // GET: AnnualTaxBrackets/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AnnualTaxBracket annualTaxBracket = await db.AnnualTaxBrackets.FindAsync(id);
            if (annualTaxBracket == null)
            {
                return HttpNotFound();
            }
            return View(annualTaxBracket);
        }

        // POST: AnnualTaxBrackets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            AnnualTaxBracket annualTaxBracket = await db.AnnualTaxBrackets.FindAsync(id);
            db.AnnualTaxBrackets.Remove(annualTaxBracket);
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
