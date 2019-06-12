using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using TendaAdvisors.Models;
using TendaAdvisors.Models.Response;
using System.Threading.Tasks;

namespace TendaAdvisors.Controllers
{
    public class BankBranchCodesController : BaseApiController
    {
        //private ApplicationDbContext db;// = new ApplicationDbContext();

        //public BankBranchCodesController(ApplicationDbContext dbcontext) {
        //    db = dbcontext;
        //}


        // GET: api/BankNames
        public async Task<IHttpActionResult> GetBankBranchCodes()
        {
            try
            {
                var response = await db.BankBranchCodes.Select(x => new BankBranchCodeResponse() { Id = x.Id, Name = x.Name }).ToListAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        // GET: api/BankBranchCodes/5
        [ResponseType(typeof(BankBranchCodes))]
        public IHttpActionResult GetBankBranchCodes(int id)
        {
            BankBranchCodes bankBranchCodes = db.BankBranchCodes.Find(id);
            if (bankBranchCodes == null)
            {
                return NotFound();
            }

            return Ok(bankBranchCodes);
        }

        // PUT: api/BankBranchCodes/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutBankBranchCodes(int id, BankBranchCodes bankBranchCodes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != bankBranchCodes.Id)
            {
                return BadRequest();
            }

            db.Entry(bankBranchCodes).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankBranchCodesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/BankBranchCodes
        [ResponseType(typeof(BankBranchCodes))]
        public IHttpActionResult PostBankBranchCodes(BankBranchCodes bankBranchCodes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.BankBranchCodes.Add(bankBranchCodes);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = bankBranchCodes.Id }, bankBranchCodes);
        }

        // DELETE: api/BankBranchCodes/5
        [ResponseType(typeof(BankBranchCodes))]
        public IHttpActionResult DeleteBankBranchCodes(int id)
        {
            BankBranchCodes bankBranchCodes = db.BankBranchCodes.Find(id);
            if (bankBranchCodes == null)
            {
                return NotFound();
            }

            db.BankBranchCodes.Remove(bankBranchCodes);
            db.SaveChanges();

            return Ok(bankBranchCodes);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BankBranchCodesExists(int id)
        {
            return db.BankBranchCodes.Count(e => e.Id == id) > 0;
        }
    }
}