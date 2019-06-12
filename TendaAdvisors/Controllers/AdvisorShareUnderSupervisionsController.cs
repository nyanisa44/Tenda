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
using System.Threading.Tasks;

using TendaAdvisors.Providers;
using TendaAdvisors.Models.DTO;
using TendaAdvisors.Models.Response;

namespace TendaAdvisors.Controllers
{
    public class AdvisorShareUnderSupervisionsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/AdvisorShareUnderSupervisions
        public IQueryable<AdvisorShareUnderSupervision> GetAdvisorShareUnderSupervisions()
        {
            return db.AdvisorShareUnderSupervisions;
        }

        // GET: api/AdvisorShareUnderSupervisions/5
        [ResponseType(typeof(AdvisorShareUnderSupervision))]
        [Route("AdvisorShareUnderSupervisions/AdvisorLicenseTypes/{advisorId}")]
        public IHttpActionResult GetAdvisorShareUnderSupervision(int advisorId)
        {

            var result = new List<AdvisorShareDTO>();
            try
            {
                List<AdvisorShareUnderSupervision> ltypes = db.AdvisorShareUnderSupervisions.Where(l => l.AdvisorId == advisorId).ToList();
                foreach (var lt in ltypes)
                {
                    LicenseType license = db.LicenseTypes.Find(lt.LicenseTypeId);

                    result.Add(new AdvisorShareDTO { AdvisorId = lt.AdvisorId, LicenseTypeName = license.Description, supplier = lt.supplier, product = lt.product, Share = lt.Share, validCommissionFromDate = lt.validCommissionFromDate, validCommissionToDate = lt.validCommissionToDate, LicenseTypeId = lt.LicenseTypeId, UnderSupervision = lt.underSupervision, Advisor = lt.Advisor, ValidFromDate = lt.ValidFromDate, ValidToDate = lt.ValidToDate });
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }

            //return Ok(ltypes);
        }


        [Route("AdvisorShareUnderSupervisions/AdvisorLicenseTypes/Post/")]
        [AcceptVerbs("POST")]
        public async Task PostAdvisorLicenseType(AdvisorShareUnderSupervision licenseType)
        {
            await Task.Yield();

            AdvisorShareUnderSupervision at = db.AdvisorShareUnderSupervisions.Find(licenseType.AdvisorId, licenseType.LicenseTypeId, licenseType.supplier, licenseType.product);


            try
            {


                if (at == null)
                {
                    db.AdvisorShareUnderSupervisions.Add(licenseType);
                    //db.Entry(at).CurrentValues.SetValues(at);

                }

                else
                {
                    db.Entry(at).CurrentValues.SetValues(licenseType);

                }


                db.SaveChanges();
            }
            catch (Exception e)
            {

            }

        }

        /* GET: api/AdvisorShareUnderSupervisions/AdvisorLicenseTypes/5
        [Route("AdvisorShareUnderSupervisions/AdvisorLicenseTypes/{advisorId}")]
        public async Task<IHttpActionResult> GetAdvisorLicenseTypes(int? advisorId = null)
        {
            var result = new List<AdvisorShareDTO>();

            try
            {
                List<AdvisorShareUnderSupervision> ltypes = db.AdvisorShareUnderSupervisions.Where(l => l.AdvisorId == advisorId).ToList();
                foreach (var lt in ltypes)
                {
                    LicenseType license = db.LicenseTypes.Find(lt.LicenseTypeId);

                    result.Add(new AdvisorShareDTO { AdvisorId = lt.AdvisorId, LicenseTypeName = license.Description,supplier=lt.supplier,product=lt.product, Share = lt.Share, validCommissionFromDate = lt.validCommissionFromDate, validCommissionToDate = lt.validCommissionToDate, LicenseTypeId = lt.LicenseTypeId, UnderSupervision = lt.underSupervision, Advisor = lt.Advisor, ValidFromDate = lt.ValidFromDate, ValidToDate = lt.ValidToDate });
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }

            return Ok(result);
        }*/

        // PUT: api/AdvisorShareUnderSupervisions/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAdvisorShareUnderSupervision(int id, AdvisorShareUnderSupervision advisorShareUnderSupervision)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != advisorShareUnderSupervision.AdvisorId)
            {
                return BadRequest();
            }

            db.Entry(advisorShareUnderSupervision).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdvisorShareUnderSupervisionExists(id))
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







        [Route("AdvisorShareUnderSupervisions/AdvisorLicenseTypes/Post/{advisorId}/{licenseId}/{supplierName}/{productName}/{share}/{validCommissionFromDate}/{validCommissionToDate}/{underSupervision}/{advisorSupervisionID}/{fromSupervision}/{toSupervision}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PostAdvisorShare(int advisorId, int licenseId, string supplierName, string productName, double share, DateTime? validCommissionFromDate, DateTime? validCommissionToDate, bool underSupervision, int advisorSupervisionID, DateTime? fromSupervision, DateTime? toSupervision)
        {

            if (validCommissionFromDate == null || validCommissionToDate == null || fromSupervision == null || toSupervision == null)
            {

            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
            }
            try
            {

                AdvisorShareUnderSupervision at = new AdvisorShareUnderSupervision();

                if (!(share >= 0 && share < 100))
                {
                    return BadRequest();
                }
                int supplierId = db.Suppliers.FirstOrDefault(e => e.Name == supplierName).Id;
                at.AdvisorId = advisorId;
                at.LicenseTypeId = licenseId;
                at.supplier = supplierName;
                at.product = productName;
                at.Share = share;
                at.underSupervision = underSupervision;
                at.Advisor = advisorSupervisionID;
                at.ValidFromDate = fromSupervision;
                at.ValidToDate = toSupervision;
                at.validCommissionFromDate = validCommissionFromDate;
                at.validCommissionToDate = validCommissionToDate;
                at.SupplierId = supplierId;
                //db.Entry(at).CurrentValues.SetValues(at);
                db.AdvisorShareUnderSupervisions.Add(at);


            }
            catch (Exception e)
            {
                throw e;
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;

            }


            return StatusCode(HttpStatusCode.NoContent);

        }


        // POST: api/AdvisorShareUnderSupervisions
        [ResponseType(typeof(AdvisorShareUnderSupervision))]
        public IHttpActionResult PostAdvisorShareUnderSupervision(AdvisorShareUnderSupervision advisorShareUnderSupervision)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AdvisorShareUnderSupervisions.Add(advisorShareUnderSupervision);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (AdvisorShareUnderSupervisionExists(advisorShareUnderSupervision.AdvisorId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = advisorShareUnderSupervision.AdvisorId }, advisorShareUnderSupervision);
        }

        // DELETE: api/AdvisorShareUnderSupervisions/5
        [ResponseType(typeof(AdvisorShareUnderSupervision))]
        public IHttpActionResult DeleteAdvisorShareUnderSupervision(int id)
        {
            AdvisorShareUnderSupervision advisorShareUnderSupervision = db.AdvisorShareUnderSupervisions.Find(id);
            if (advisorShareUnderSupervision == null)
            {
                return NotFound();
            }

            db.AdvisorShareUnderSupervisions.Remove(advisorShareUnderSupervision);
            db.SaveChanges();

            return Ok(advisorShareUnderSupervision);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AdvisorShareUnderSupervisionExists(int id)
        {
            return db.AdvisorShareUnderSupervisions.Count(e => e.AdvisorId == id) > 0;
        }

        [AcceptVerbs("POST")]
        [Route("AdvisorShareUnderSupervisions/AdvisorLicenseTypes")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PostAdvisorShareObject(AdvisorLicenseResponse alr)
        {
            try
            {
                var licenseType = await db.LicenseTypes.Where(x => x.Description == alr.LicenseType || x.Id == alr.LicenseTypeId).FirstOrDefaultAsync();
                DateTime MinDate = new DateTime(0001, 01, 01);
                DateTime SQLMinDate = new DateTime(1901, 01, 01);
                var supervisorAdviserId = await db.Advisors.Where(x => x.ContactId == alr.SupervisorId).FirstOrDefaultAsync();
                var license = await db.AdvisorShareUnderSupervisions.SingleOrDefaultAsync(x => x.AdvisorId == alr.AdvisorId && x.supplier == alr.Supplier );
                var Supplier = await db.Suppliers.FirstOrDefaultAsync(e => e.Name == alr.Supplier);

                if (license != null)
                {
                    license.Advisor = alr.UnderSupervision ? supervisorAdviserId.Id : 53;
                    license.AdvisorId = alr.AdvisorId;
                    license.LicenseTypeId = licenseType.Id;
                    license.product = alr.Product;
                    license.Share = alr.Share;
                    license.supplier = alr.Supplier;
                    license.underSupervision = alr.UnderSupervision;
                    license.validCommissionFromDate = alr.ValidCommissionFromDate == DateTime.MinValue ? SQLMinDate : alr.ValidCommissionFromDate;
                    license.validCommissionToDate = alr.ValidCommissionToDate == null? null: alr.ValidCommissionToDate;
                    license.ValidFromDate = alr.UnderSupervision ? alr.ValidFromDate  : DateTime.Today;
                    license.ValidToDate = alr.UnderSupervision ? alr.ValidToDate : DateTime.Today;
                    license.SupplierId = Supplier.Id;
                }
                else
                {
                    license = new AdvisorShareUnderSupervision()
                    {
                        Advisor = alr.UnderSupervision ? supervisorAdviserId.Id : 53,
                        AdvisorId=alr.AdvisorId,
                        LicenseTypeId = licenseType.Id,
                        product = alr.Product,
                        Share = alr.Share,
                        supplier = alr.Supplier,
                        underSupervision = alr.UnderSupervision,
                    validCommissionFromDate = alr.ValidCommissionFromDate == DateTime.MinValue ? SQLMinDate : alr.ValidCommissionFromDate,
                    validCommissionToDate = alr.ValidCommissionToDate == null ? null : alr.ValidCommissionToDate,
                    ValidFromDate = alr.ValidFromDate == DateTime.MinValue ? SQLMinDate : alr.ValidFromDate,
                    ValidToDate = alr.ValidToDate == DateTime.MinValue ? SQLMinDate : alr.ValidToDate,
                    SupplierId = Supplier.Id
                    };

                    db.AdvisorShareUnderSupervisions.Add(license);
                //var licenses = await db.AdvisorShareUnderSupervisions.Where(x => x.AdvisorId == alr.AdvisorId && x.supplier==alr.Supplier && x.product==alr.Product).ToListAsync();
                }

                /*if (license.Count > 0)

                    db.Entry(newLicense).CurrentValues.SetValues(newLicense);
                else
                    db.AdvisorShareUnderSupervisions.Add(newLicense);*/

                await db.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
    }
}