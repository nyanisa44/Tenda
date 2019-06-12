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
    public class ApplicationTypesController : BaseApiController
    {
        //private ApplicationDbContext db;// = new ApplicationDbContext();


        //public ApplicationTypesController(ApplicationDbContext dbcontext) {

        //    db = dbcontext;

        //}
        // GET: api/ApplicationTypes
        public IQueryable<ApplicationType> GetApplicationTypes()
        {
            return db.ApplicationTypes.Include(c=>c.DocumentTypes);
        }

        // GET: api/ApplicationTypes/5
        [ResponseType(typeof(ApplicationType))]
        public IHttpActionResult GetApplicationType(int id)
        {
            ApplicationType applicationType = db.ApplicationTypes.Find(id);
            if (applicationType == null)
            {
                return NotFound();
            }

            return Ok(applicationType);
        }

        // PUT: api/ApplicationTypes/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutApplicationType(int id, ApplicationType applicationType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != applicationType.Id)
            {
                return BadRequest();
            }

            db.Entry(applicationType).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationTypeExists(id))
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

        // POST: api/ApplicationTypes
        [ResponseType(typeof(ApplicationType))]
        public IHttpActionResult PostApplicationType(ApplicationType applicationType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ApplicationTypes.Add(applicationType);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = applicationType.Id }, applicationType);
        }

        // DELETE: api/ApplicationTypes/5
        [ResponseType(typeof(ApplicationType))]
        public IHttpActionResult DeleteApplicationType(int id)
        {
            ApplicationType applicationType = db.ApplicationTypes.Find(id);
            if (applicationType == null)
            {
                return NotFound();
            }

            db.ApplicationTypes.Remove(applicationType);
            db.SaveChanges();

            return Ok(applicationType);
        }    

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ApplicationTypeExists(int id)
        {
            return db.ApplicationTypes.Count(e => e.Id == id) > 0;
        }
    }
}