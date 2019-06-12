using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TendaAdvisors.Models;

namespace TendaAdvisors.Controllers
{
    public class AdvisorSupplierCodesController : BaseApiController
    {
        //private ApplicationDbContext db;// = new ApplicationDbContext();

        //public AdvisorSupplierCodesController(ApplicationDbContext dbcontect) {
        //    db = dbcontect;
        //}

        // GET: api/AdvisorSupplierCodes
        public IQueryable<AdvisorSupplierCode> GetAdvisorSupplierCodes()
        {
            return db.AdvisorSupplierCodes;
        }

        // GET: api/AdvisorSupplierCodes/5
        [ResponseType(typeof(AdvisorSupplierCode))]
        public async Task<IHttpActionResult> GetAdvisorSupplierCode(int id)
        {
            AdvisorSupplierCode advisorSupplierCode = await db.AdvisorSupplierCodes.FindAsync(id);
            if (advisorSupplierCode == null)
            {
                return NotFound();
            }

            return Ok(advisorSupplierCode);
        }

        // PUT: api/AdvisorSupplierCodes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAdvisorSupplierCodeOne(int id, AdvisorSupplierCode advisorSupplierCode)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != advisorSupplierCode.SupplierId)
            {
                return BadRequest();
            }

            db.Entry(advisorSupplierCode).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdvisorSupplierCodeExists(id))
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

        // POST: api/AdvisorSupplierCodes
       [ResponseType(typeof(AdvisorSupplierCode))]
        public async Task<IHttpActionResult> PostAdvisorSupplierCode(AdvisorSupplierCode advisorSupplierCode)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AdvisorSupplierCodes.Add(advisorSupplierCode);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AdvisorSupplierCodeExists(advisorSupplierCode.SupplierId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = advisorSupplierCode.SupplierId }, advisorSupplierCode);
        }


      

        // DELETE: api/AdvisorSupplierCodes/5
        [ResponseType(typeof(AdvisorSupplierCode))]
        public async Task<IHttpActionResult> DeleteAdvisorSupplierCode(int id)
        {
            AdvisorSupplierCode advisorSupplierCode = await db.AdvisorSupplierCodes.FindAsync(id);
            if (advisorSupplierCode == null)
            {
                return NotFound();
            }

            db.AdvisorSupplierCodes.Remove(advisorSupplierCode);
            await db.SaveChangesAsync();

            return Ok(advisorSupplierCode);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AdvisorSupplierCodeExists(int id)
        {
            return db.AdvisorSupplierCodes.Count(e => e.SupplierId == id) > 0;
        }
    }
}