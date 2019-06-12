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
    [Authorize]
    public class AddressTypesController : BaseApiController
    {
        //private ApplicationDbContext db;//

        //public  AddressTypesController(ApplicationDbContext dbcontext) {
        //    db = dbcontext;
        //}
        // GET: api/AddressTypes
        public async Task<IHttpActionResult> GetAddressTypes()
        {
            try
            {
                var response = await db.AddressTypes.Select(x => new AddressTypeResponse() { Id =x.Id, Name= x.Name }).ToListAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/AddressTypes/5
        [ResponseType(typeof(AddressType))]
        public async Task<IHttpActionResult> GetAddressType(int id)
        {
            AddressType addressType = await db.AddressTypes.FindAsync(id);
            if (addressType == null)
            {
                return NotFound();
            }

            return Ok(addressType);
        }

        // PUT: api/AddressTypes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAddressType(int id, AddressType addressType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != addressType.Id)
            {
                return BadRequest();
            }

            db.Entry(addressType).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressTypeExists(id))
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

        // POST: api/AddressTypes
        [ResponseType(typeof(AddressType))]
        public async Task<IHttpActionResult> PostAddressType(AddressType addressType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AddressTypes.Add(addressType);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = addressType.Id }, addressType);
        }

        // DELETE: api/AddressTypes/5
        [ResponseType(typeof(AddressType))]
        public async Task<IHttpActionResult> DeleteAddressType(int id)
        {
            AddressType addressType = await db.AddressTypes.FindAsync(id);
            if (addressType == null)
            {
                return NotFound();
            }

            db.AddressTypes.Remove(addressType);
            await db.SaveChangesAsync();

            return Ok(addressType);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AddressTypeExists(int id)
        {
            return db.AddressTypes.Count(e => e.Id == id) > 0;
        }
    }
}