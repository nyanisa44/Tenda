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
    public class ContactTitlesController : BaseApiController
    {
        // private ApplicationDbContext db = new ApplicationDbContext();


        // GET: api/ContactTitle
        public async Task<IHttpActionResult> GetContactTitles()
        {
            try
            {
                var response = await db.ContactTitles.Select(x => new ContactTitleResponse() { Id = x.Id, Name = x.Name }).ToListAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }



        // GET: api/ContactTitles/5
        [ResponseType(typeof(ContactTitles))]
        public IHttpActionResult GetContactTitles(int id)
        {
            ContactTitles contactTitles = db.ContactTitles.Find(id);
            if (contactTitles == null)
            {
                return NotFound();
            }

            return Ok(contactTitles);
        }

        // PUT: api/ContactTitles/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutContactTitles(int id, ContactTitles contactTitles)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != contactTitles.Id)
            {
                return BadRequest();
            }

            db.Entry(contactTitles).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactTitlesExists(id))
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

        // POST: api/ContactTitles
        [ResponseType(typeof(ContactTitles))]
        public IHttpActionResult PostContactTitles(ContactTitles contactTitles)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ContactTitles.Add(contactTitles);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = contactTitles.Id }, contactTitles);
        }

        // DELETE: api/ContactTitles/5
        [ResponseType(typeof(ContactTitles))]
        public IHttpActionResult DeleteContactTitles(int id)
        {
            ContactTitles contactTitles = db.ContactTitles.Find(id);
            if (contactTitles == null)
            {
                return NotFound();
            }

            db.ContactTitles.Remove(contactTitles);
            db.SaveChanges();

            return Ok(contactTitles);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ContactTitlesExists(int id)
        {
            return db.ContactTitles.Count(e => e.Id == id) > 0;
        }
    }
}