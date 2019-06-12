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
    public class ApplicationStatusController : BaseApiController
    {
        //private ApplicationDbContext db;// = new ApplicationDbContext();

        //public ApplicationStatusController(ApplicationDbContext dbcontext) {
        //    db = dbcontext;
        //}

        // GET: api/ApplicationStatus
        public IQueryable<ApplicationStatus> GetApplicationStatuses()
        {
            return db.ApplicationStatuses;
        }

        // GET: api/ApplicationStatus/5
        [ResponseType(typeof(ApplicationStatus))]
        public IHttpActionResult GetApplicationStatus(int id)
        {
            ApplicationStatus applicationStatus = db.ApplicationStatuses.Find(id);
            if (applicationStatus == null)
            {
                return NotFound();
            }

            return Ok(applicationStatus);
        }

        // PUT: api/ApplicationStatus/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutApplicationStatus(int id, ApplicationStatus applicationStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != applicationStatus.Id)
            {
                return BadRequest();
            }

            db.Entry(applicationStatus).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationStatusExists(id))
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

        // POST: api/ApplicationStatus
        [ResponseType(typeof(ApplicationStatus))]
        public IHttpActionResult PostApplicationStatus(ApplicationStatus applicationStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ApplicationStatuses.Add(applicationStatus);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = applicationStatus.Id }, applicationStatus);
        }

        // DELETE: api/ApplicationStatus/5
        [ResponseType(typeof(ApplicationStatus))]
        public IHttpActionResult DeleteApplicationStatus(int id)
        {
            ApplicationStatus applicationStatus = db.ApplicationStatuses.Find(id);
            if (applicationStatus == null)
            {
                return NotFound();
            }

            db.ApplicationStatuses.Remove(applicationStatus);
            db.SaveChanges();

            return Ok(applicationStatus);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ApplicationStatusExists(int id)
        {
            return db.ApplicationStatuses.Count(e => e.Id == id) > 0;
        }
    }
}