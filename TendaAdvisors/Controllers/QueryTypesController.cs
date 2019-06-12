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
    public class QueryTypesController : BaseApiController
    {
       // private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/QueryTypes
        public IQueryable<QueryType> GetQueryTypes()
        {
            return db.QueryTypes;
        }

        // GET: api/QueryTypes/5
        [ResponseType(typeof(QueryType))]
        public IHttpActionResult GetQueryType(int id)
        {
            QueryType queryType = db.QueryTypes.Find(id);
            if (queryType == null)
            {
                return NotFound();
            }

            return Ok(queryType);
        }

        // PUT: api/QueryTypes/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutQueryType(int id, QueryType queryType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != queryType.Id)
            {
                return BadRequest();
            }

            db.Entry(queryType).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QueryTypeExists(id))
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

        // POST: api/QueryTypes
        [ResponseType(typeof(QueryType))]
        public IHttpActionResult PostQueryType(QueryType queryType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.QueryTypes.Add(queryType);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = queryType.Id }, queryType);
        }

        // DELETE: api/QueryTypes/5
        [ResponseType(typeof(QueryType))]
        public IHttpActionResult DeleteQueryType(int id)
        {
            QueryType queryType = db.QueryTypes.Find(id);
            if (queryType == null)
            {
                return NotFound();
            }

            db.QueryTypes.Remove(queryType);
            db.SaveChanges();

            return Ok(queryType);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool QueryTypeExists(int id)
        {
            return db.QueryTypes.Count(e => e.Id == id) > 0;
        }
    }
}