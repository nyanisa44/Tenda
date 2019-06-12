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
using TendaAdvisors.Models.Response;

namespace TendaAdvisors.Controllers
{
    [Authorize]
    public class LicensesController : BaseApiController
    {
       // private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Licenses/LicenseTypes
        [Route("Licenses/LicenseTypes/{categoryId}")]
        public IQueryable<LicenseType> GetLicenseTypesByCategory(int? categoryId = 0)
        {
            if (categoryId <= 0)
                return db.LicenseTypes;
            else {
                return db.LicenseTypes.Where(l => l.LicenseCategoryId == categoryId);
            }
        }

        // GET: api/Licenses/LicenseCategories
        [Route("Licenses/LicenseCategories")]
        public IQueryable<LicenseCategory> GetLicenseCategories()
        {
            return db.LicenseCategories.Include(c => c.LicenseTypes);
        }

        [Route("Licenses/Licences")]
        async Task<IHttpActionResult> GetLicences()
        {
            var Licences = await (from x in db.LicenseTypes
                                         join cat in db.LicenseCategories on x.LicenseCategoryId equals cat.Id
                                         select new LicenseTypeResponse() { Id = x.Id, LicenseCategoryId = cat.Id, Description = x.Description, SubCategory = x.SubCategory }
                                         ).ToListAsync();

            return Ok(Licences);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}