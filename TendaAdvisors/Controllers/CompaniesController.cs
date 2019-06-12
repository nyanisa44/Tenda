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
    [Authorize]
    public class CompaniesController : BaseApiController
    {
    //    private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Companies
        public IQueryable<Company> GetCompanies()
        {
            return db.Companies
                .Include(c => c.ContactDetails)
                .Take(10);
        }

        // GET: api/Companies/5
        [ResponseType(typeof(Company))]
        public async Task<IHttpActionResult> GetCompany(int id)
        {
            Company company = await db.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            return Ok(company);
        }

        // GET: api/Companies/Tenda
        [Route("Companies/{nameSearch}")]
        public async Task<IHttpActionResult> GetCompany(string nameSearch)
        {
            if (nameSearch == "undefined")
            {
                return NotFound();
            }
            
            List<Company> companies = new List<Company>();

            if (nameSearch == "")
            {
                companies = await db.Companies
                    .Include(c => c.ContactDetails)
                    .Take(10)
                    .ToListAsync();
            }
            else
            {
                companies = await db.Companies
                    .Include(c => c.ContactDetails)
                    .Where(c => c.Name.Contains(nameSearch))
                    .Take(10)
                    .ToListAsync();
            }

            if (companies.Count == 0)
            {
                return NotFound();
            }

            return Ok(companies);
        }

        // PUT: api/Companies/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCompany(int id, Company company)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != company.Id)
            {
                return BadRequest();
            }

            db.Entry(company).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
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

        // POST: api/Companies
        [ResponseType(typeof(Company))]
        public async Task<IHttpActionResult> PostCompany(Company company)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Companies.Add(company);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = company.Id }, company);
        }

        // DELETE: api/Companies/5
        [ResponseType(typeof(Company))]
        public async Task<IHttpActionResult> DeleteCompany(int id)
        {
            Company company = await db.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            db.Companies.Remove(company);
            await db.SaveChangesAsync();

            return Ok(company);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CompanyExists(int id)
        {
            return db.Companies.Count(e => e.Id == id) > 0;
        }
    }
}