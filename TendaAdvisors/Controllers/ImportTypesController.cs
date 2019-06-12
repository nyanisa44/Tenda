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
    //[RoutePrefix("Admin/ImportTypes")]
    public class ImportTypesController : BaseApiController
    {
       // private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Admin/ImportTypes
        [Route("Admin/ImportTypes")]
        public IQueryable<ImportType> GetImportTypes()
        {
            return db.ImportTypes;
        }

        // GET: api/ImportTypes/5
        [ResponseType(typeof(ImportType))]
        [Route("Admin/ImportTypes/{id}")]
        public async Task<IHttpActionResult> GetImportType(int id)
        {
            ImportType importType = await db.ImportTypes.FindAsync(id);
            if (importType == null)
            {
                return NotFound();
            }

            return Ok(importType);
        }

        // PUT: api/ImportTypes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutImportType(int id, ImportType importType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != importType.Id)
            {
                return BadRequest();
            }

            db.Entry(importType).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImportTypeExists(id))
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

        // POST: api/ImportTypes
        [ResponseType(typeof(ImportType))]
        public async Task<IHttpActionResult> PostImportType(ImportType importType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ImportTypes.Add(importType);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = importType.Id }, importType);
        }

        // DELETE: api/ImportTypes/5
        [ResponseType(typeof(ImportType))]
        public async Task<IHttpActionResult> DeleteImportType(int id)
        {
            ImportType importType = await db.ImportTypes.FindAsync(id);
            if (importType == null)
            {
                return NotFound();
            }

            db.ImportTypes.Remove(importType);
            await db.SaveChangesAsync();

            return Ok(importType);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ImportTypeExists(int id)
        {
            return db.ImportTypes.Count(e => e.Id == id) > 0;
        }
    }
}