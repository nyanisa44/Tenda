using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TendaAdvisors.Models;
using TendaAdvisors.Models.Response;

namespace TendaAdvisors.Controllers
{
    public class AdvisorTypesController : BaseApiController
    {
        //private ApplicationDbContext db;//= new ApplicationDbContext();
        //public AdvisorTypesController(ApplicationDbContext dbcontext) {
        //    db = dbcontext;
        //}


        // GET: api/AdvisorTypes
        public async Task<IHttpActionResult> GetAdvisorTypes()
        {
            try
            {
                
                //var listOfStudents = db.Student.Select(x => new { Id = x.Id, Name = x.StudentName, Subjects = x.StudentsSubjects.Select(y => y.Subject) });


                var advisorTypes = await db.AdvisorTypes
                    .Select(x => new AdvisorTypeResponse() {
                        Id = x.Id,
                        Title = x.Title,
                        DocumentTypes = x.DocumentTypes.Select(d => new DocumentTypeResponse() { Id = d.Id, Name = d.Name } ).ToList()
                        })
                    .ToListAsync();
                
                return Ok(advisorTypes);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/AdvisorTypes/5
        [ResponseType(typeof(AdvisorType))]
        public IHttpActionResult GetAdvisorType(int id)
        {
            AdvisorType advisorType = db.AdvisorTypes.Find(id);
            if (advisorType == null)
            {
                return NotFound();
            }

            return Ok(advisorType);
        }

        // PUT: api/AdvisorTypes/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAdvisorType(int id, AdvisorType advisorType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != advisorType.Id)
            {
                return BadRequest();
            }

            db.Entry(advisorType).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdvisorTypeExists(id))
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

        // POST: api/AdvisorTypes
        [ResponseType(typeof(AdvisorType))]
        public IHttpActionResult PostAdvisorType(AdvisorType advisorType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AdvisorTypes.Add(advisorType);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = advisorType.Id }, advisorType);
        }

        // DELETE: api/AdvisorTypes/5
        [ResponseType(typeof(AdvisorType))]
        public IHttpActionResult DeleteAdvisorType(int id)
        {
            AdvisorType advisorType = db.AdvisorTypes.Find(id);
            if (advisorType == null)
            {
                return NotFound();
            }

            db.AdvisorTypes.Remove(advisorType);
            db.SaveChanges();

            return Ok(advisorType);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AdvisorTypeExists(int id)
        {
            return db.AdvisorTypes.Count(e => e.Id == id) > 0;
        }
    }
}