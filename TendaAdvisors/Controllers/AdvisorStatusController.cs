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

namespace TendaAdvisors.Controllers
{
    public class AdvisorStatusController : BaseApiController
    {
        //private ApplicationDbContext db;// = new ApplicationDbContext();

        //public AdvisorStatusController(ApplicationDbContext dbcontext) {
        //    db = dbcontext;
        //}

        // GET: api/AdvisorStatus
        public IQueryable<AdvisorStatus> GetAdvisorStatuses()
        {
            return db.AdvisorStatuses;
        }

        // GET: api/AdvisorStatus/5
        [ResponseType(typeof(AdvisorStatus))]
        public IHttpActionResult GetAdvisorStatus(int id)
        {
            AdvisorStatus advisorStatus = db.AdvisorStatuses.Find(id);
            if (advisorStatus == null)
            {
                return NotFound();
            }

            return Ok(advisorStatus);
        }

        // PUT: api/AdvisorStatus/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAdvisorStatus(int id, AdvisorStatus advisorStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != advisorStatus.Id)
            {
                return BadRequest();
            }

            db.Entry(advisorStatus).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdvisorStatusExists(id))
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

        // POST: api/AdvisorStatus
        [ResponseType(typeof(AdvisorStatus))]
        public IHttpActionResult PostAdvisorStatus(AdvisorStatus advisorStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AdvisorStatuses.Add(advisorStatus);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = advisorStatus.Id }, advisorStatus);
        }

        // DELETE: api/AdvisorStatus/5
        [ResponseType(typeof(AdvisorStatus))]
        public IHttpActionResult DeleteAdvisorStatus(int id)
        {
            AdvisorStatus advisorStatus = db.AdvisorStatuses.Find(id);
            if (advisorStatus == null)
            {
                return NotFound();
            }

            db.AdvisorStatuses.Remove(advisorStatus);
            db.SaveChanges();

            return Ok(advisorStatus);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AdvisorStatusExists(int id)
        {
            return db.AdvisorStatuses.Count(e => e.Id == id) > 0;
        }
    }
}