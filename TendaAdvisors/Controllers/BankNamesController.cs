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
    public class BankNamesController : BaseApiController
    {
        

        // GET: api/BankNames
        public async Task<IHttpActionResult> GetBankName()
        {
            try
            {
                var response = await db.BankName.Select(x => new BankNameResponse() { Id = x.Id, Name = x.Name }).ToListAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/BankNames/5
        [ResponseType(typeof(BankNames))]
        public IHttpActionResult GetBankNames(int id)
        {
            BankNames bankNames = db.BankName.Find(id);
            if (bankNames == null)
            {
                return NotFound();
            }

            return Ok(bankNames);
        }

        // PUT: api/BankNames/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutBankNames(int id, BankNames bankNames)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != bankNames.Id)
            {
                return BadRequest();
            }

            db.Entry(bankNames).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankNamesExists(id))
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

        // POST: api/BankNames
        [ResponseType(typeof(BankNames))]
        public IHttpActionResult PostBankNames(BankNames bankNames)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.BankName.Add(bankNames);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = bankNames.Id }, bankNames);
        }

        // DELETE: api/BankNames/5
        [ResponseType(typeof(BankNames))]
        public IHttpActionResult DeleteBankNames(int id)
        {
            BankNames bankNames = db.BankName.Find(id);
            if (bankNames == null)
            {
                return NotFound();
            }

            db.BankName.Remove(bankNames);
            db.SaveChanges();

            return Ok(bankNames);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BankNamesExists(int id)
        {
            return db.BankName.Count(e => e.Id == id) > 0;
        }
    }
}