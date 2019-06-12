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
using System.Threading.Tasks;
using TendaAdvisors.Models.Response;

namespace TendaAdvisors.Controllers
{
    public class UnmatchedCommissionsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/UnmatchedCommissions
        public IQueryable<UnmatchedCommissions> GetUnmatchedCommissions()
        {
            return db.UnmatchedCommissions;
        }



        [Route("unmatchedCommisions/All")]
        public async Task<IHttpActionResult> GetUnmatchedCommissionsAll()
        {
            try
            {
                var unmatchedCommisions = await db.UnmatchedCommissions
                                   .Select(c => new   UnmatchedCommissionsResponse() { Id = c.Id, Initial = c.Initial,
                                       Surname = c.Surname,
                                       MemberNumber =c.MemberSearchValue,
                                       supplierName = c.supplierName,
                                       Reason = c.Reasons ,
                                       AdvisorName = c.AdvisorName
                                   }).ToListAsync();

                return Ok(unmatchedCommisions);
            }

            catch (Exception e)
            {
                throw e;
            }
        }

        // GET: api/UnmatchedCommissions/5
        [ResponseType(typeof(UnmatchedCommissions))]
        public IHttpActionResult GetUnmatchedCommissions(int id)
        {
            UnmatchedCommissions unmatchedCommissions = db.UnmatchedCommissions.Find(id);
            if (unmatchedCommissions == null)
            {
                return NotFound();
            }

            return Ok(unmatchedCommissions);
        }

        // PUT: api/UnmatchedCommissions/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUnmatchedCommissions(int id, UnmatchedCommissions unmatchedCommissions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != unmatchedCommissions.Id)
            {
                return BadRequest();
            }

            db.Entry(unmatchedCommissions).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UnmatchedCommissionsExists(id))
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

        // POST: api/UnmatchedCommissions
        [ResponseType(typeof(UnmatchedCommissions))]
        public IHttpActionResult PostUnmatchedCommissions(UnmatchedCommissions unmatchedCommissions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.UnmatchedCommissions.Add(unmatchedCommissions);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = unmatchedCommissions.Id }, unmatchedCommissions);
        }

        // DELETE: api/UnmatchedCommissions/5
        [ResponseType(typeof(UnmatchedCommissions))]
        public IHttpActionResult DeleteUnmatchedCommissions(int id)
        {
            UnmatchedCommissions unmatchedCommissions = db.UnmatchedCommissions.Find(id);
            if (unmatchedCommissions == null)
            {
                return NotFound();
            }

            db.UnmatchedCommissions.Remove(unmatchedCommissions);
            db.SaveChanges();

            return Ok(unmatchedCommissions);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UnmatchedCommissionsExists(int id)
        {
            return db.UnmatchedCommissions.Count(e => e.Id == id) > 0;
        }
    }
}