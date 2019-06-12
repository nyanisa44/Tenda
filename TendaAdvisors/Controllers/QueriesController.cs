using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using TendaAdvisors.Models;
using TendaAdvisors.Models.Response;
using TendaAdvisors.Providers;

namespace TendaAdvisors.Controllers
{
    public class QueriesController : BaseApiController
    {
       // private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationUser _user
        {
            get
            {
                return HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>()
                    .FindById(HttpContext.Current.User.Identity.GetUserId());
            }
        }

        // GET: api/Queries
        [ResponseType(typeof(Application))]
        public async Task<IHttpActionResult> GetQuerys(int applicationId)
        {
            Application application = await db.Applications.FindAsync(applicationId);
            await db.Entry(application).Collection(q => q.Queries).LoadAsync();

            return Ok(application);
        }

        
        // GET: api/Queries/5
        [ResponseType(typeof(Query))]
        [HttpGet]
        [Route("Queries/{id}")]
        public async Task<IHttpActionResult> GetQuery(int id)
        {

            Query query = await db.Querys.FindAsync(id);
            
            if (query == null)
            {
                return NotFound();
            }

            await db.Entry(query).Reference(c => c.QueryType).LoadAsync();

            return Ok(query);
        }

        // GET: api/Queries/Application/5
        [ResponseType(typeof(Application))]
        [Route("Queries/Application/{id}")]
        public async Task<IHttpActionResult> GetQueriesByApplication(int id)
        {

            
            Application application = await db.Applications.FindAsync(id);
            //await db.Entry(application).Collection(c => c.Queries).LoadAsync();
            var applicationQueryReturned = await db.Querys
                                .Where(x => x.Application_Id == id)
                                .Select(c => new QueryResponse() { Id = c.Id, QueryName=c.QueryName, Note=c.Note, Application_Id=c.Application_Id, QueryType=c.QueryType }).ToListAsync();

          /*  foreach (var query in application.Queries)
            {
                db.Querys.Attach(query);
                await db.Entry(query).Reference(q => q.QueryType).LoadAsync();
                //await db.Entry(query).Reference(c => c.QueryType).LoadAsync();
            }

            if (application == null)
            {
                return NotFound();
            }*/

            return Ok(applicationQueryReturned);
        }

        // PUT: api/Queries/5
        [ResponseType(typeof(void))]
        [HttpPut]
        [Route("Queries/{id}")]
        public async Task<IHttpActionResult> PutQuery(int id, Query query)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != query.Id)
            {
                return BadRequest();
            }

            db.Entry(query).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QueryExists(id))
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

        // POST: api/Queries/
        [HttpPost]
        public async Task<IHttpActionResult> PostQuery(Query query)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Look for application
            var application = await db.Applications.FindAsync(query.Application.Id);

            //if not found
            if (application == null)
            {
                return NotFound();
            }

            // var userId = 0;
            var userUid = "unknown";
            /*  if (_user != null)
              {
                  userId = _user.AdvisorId;
                  db.Advisors.Attach(_user.Advisor);
                  userUid = _user.Id; 
              }*/

            //Attach to query
            query.Application = application;
            //query.Advisor = _user.Advisor;
            // query.UserUID = userUid.Substring(0,30);

            //Look for query
            var queryType = await db.QueryTypes.FindAsync(query.QueryType.Id);
            //if not found
            if (queryType == null)
            {
                return NotFound();
            }
            //Attach to query
            query.QueryType = queryType;

            db.Querys.Add(query);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

            //return CreatedAtRoute("DefaultApi", new { id = query.Id }, query);
            return Ok(new QueryResponse());

        }
        // DELETE: api/Queries/5
        [ResponseType(typeof(Query))]
        [HttpDelete]
        [Route("Queries/{id}")]
        public async Task<IHttpActionResult> DeleteQuery(int id)
        {
            Query query = await db.Querys.FindAsync(id);
            if (query == null)
            {
                return NotFound();
            }

            ///query.Deleted = true;
            db.Querys.Remove(query);
            await db.SaveChangesAsync();

            return Ok(query);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool QueryExists(int id)
        {
            return db.Querys.Count(e => e.Id == id) > 0;
        }
    }
}