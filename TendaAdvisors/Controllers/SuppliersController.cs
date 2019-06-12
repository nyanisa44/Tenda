using System;
using System.Collections.Generic;
using System.Data;
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
    //[Authorize]
    public class SuppliersController : BaseApiController
    {
        //private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Suppliers
        public async Task<IEnumerable<SupplierResponse>> GetSuppliers()
        {
            try
            {   
                var suppliers = await db.Suppliers.Select(x => new SupplierResponse() {
                    Id = x.Id,
                    Name = x.Name,
                    CommissionExclVatColumn = x.CommissionExclVatColumn,
                    CommissionExclVatColumnName = x.CommissionExclVatColumnName,
                    CommissionInclVatColumn = x.CommissionInclVatColumn,
                    CommissionInclVatColumnName = x.CommissionInclVatColumnName,
                    Deleted = x.Deleted.HasValue ? x.Deleted.Value : false,
                    EnrollmentDateColumn = x.EnrollmentDateColumn,
                    EnrollmentDateColumnName = x.EnrollmentDateColumnName,
                    IDNumberColumn = x.IDNumberColumn,
                    IDNumberColumnName = x.IDNumberColumnName,
                    InitialColumn = x.InitialColumn,
                    InitialColumnName = x.InitialColumnName,
                    MemberGroupCode = x.MemberGroupCode,
                    MemberNumberColumn  =x.MemberNumberColumn,
                    MemberNumberColumnName = x.MemberNumberColumnName,
                    PolicyNumberColumn = x.PolicyNumberColumn,
                    PolicyNumberColumnName = x.PolicyNumberColumnName,
                    SubscriptionDueColumn = x.SubscriptionDueColumn,
                    SubscriptionDueColumnName = x.SubscriptionDueColumnName,
                    SubscriptionReceivedColumn = x.SubscriptionReceivedColumn,
                    SubscriptionReceivedColumnName = x.SubscriptionReceivedColumnName,
                    SurnameColumn = x.SurnameColumn,
                    SurnameColumnName = x.SurnameColumnName,
                    TerminationDateColumn = x.TerminationDateColumn,
                    TerminationDateColumnName = x.TerminationDateColumnName,
                    TransactionDateColumn = x.TransactionDateColumn,
                    TransactionDateColumnName = x.TransactionDateColumnName,
                    AdvisorNameColumn = x.AdvisorNameColumn,
                    AdvisorNameColumnName = x.AdvisorNameColumnName,
                    AdvisorSurnameColumn = x.AdvisorSurnameColumn,
                    AdvisorSurnameColumnName = x.AdvisorSurnameColumnName
                } ).ToListAsync();

                foreach (var suppl in suppliers)
                {
                    suppl.Products = await db.Products.Where(p => p.SupplierId == suppl.Id).Include(x=> x.LicenseType).Select(p => new ProductResponse() {
                        Id = p.Id,
                        Name = p.Name,
                        LicenseType = new LicenseTypeResponse() {
                            Id = p.LicenseType.Id,
                            Description = p.LicenseType.Description,
                            LicenseCategoryId = p.LicenseType.LicenseCategoryId,
                            SubCategory = p.LicenseType.SubCategory
                        }
                    }).ToListAsync();
                }

                return suppliers.OrderBy(x=> x.Name);

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // GET: api/Supplier/Basic
        [Route("Supplier/Basic")]
        public async Task<IHttpActionResult> GetBasicSuppliers()
        {
            try
            {
                var suppliers = await db.Suppliers.Select(x => new BasicSupplierResponse() { Id = x.Id, Name = x.Name }).ToListAsync();
                return Ok(suppliers);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/Contacts/nameSearch
        [Route("Supplier/Basic/{nameSearch}")]
        public async Task<IHttpActionResult> GetBasicSupplier(string nameSearch)
        {
            if (nameSearch == "undefined")
            {
                return NotFound();
            }

            List<Supplier> suppliers = new List<Supplier>();

            if (nameSearch == "")
            {
                suppliers = await db.Suppliers
                    .Take(10)
                    .ToListAsync();
            }
            else
            {
                suppliers = await db.Suppliers
                    .Where(c => c.Name.Contains(nameSearch))
                    .Take(10)
                    .ToListAsync();
            }

            if (suppliers.Count == 0)
            {
                return NotFound();
            }

            return Ok(suppliers);
        }

        // GET: api/Suppliers/5
        [ResponseType(typeof(Supplier))]
        public async Task<IHttpActionResult> GetSupplier(int id)
        {
            Supplier supplier = await db.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            return Ok(supplier);
        }

        // POST: api/Suppliers/LicenseTypes
        [Route("Suppliers/LicenseTypes")]
        public async Task<IHttpActionResult> PostFetchSuppliersByLicenses(List<LicenseType> licenses)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var licenseIds = licenses.Select(m => m.Id);

            var suppliers = db.Suppliers
                .Include(c => c.Products)
                .Include(c => c.Licenses)
                .Where(
                    s => s.Licenses.All(
                        l => licenseIds.Contains(l.Id)
                ));

            if (suppliers == null)
            {
                return NotFound();
            }

            return Ok(suppliers);
        }

        // POST: api/Suppliers/LicenseTypes
        //[Route("Suppliers/LicenseTypes")]
        //public async Task<IHttpActionResult> FetchSuppliersByLicenseID(List<LicenseType> licenses)
        //{

        //}

        // GET: api/Suppliers/5/Products
        [Route("Suppliers/{id}/Products")]
        [ResponseType(typeof(List<ProductResponse>))]
        public async Task<IHttpActionResult> GetSupplierProducts(int id)
        {
            IEnumerable<ProductResponse> products = await db.Products.Where(p => p.Supplier.Id == id).Select(p => new ProductResponse()
            {
                Id = p.Id,
                Name = p.Name,
                LicenseType = new LicenseTypeResponse()
                {
                    Id = p.LicenseType.Id,
                    Description = p.LicenseType.Description,
                    LicenseCategoryId = p.LicenseType.LicenseCategoryId,
                    SubCategory = p.LicenseType.SubCategory
                }
            }).ToListAsync();
            if (products == null)
            {
                return NotFound();
            }

            return Ok(products);
        }

        // GET: api/Suppliers/5/Application
        [Route("Suppliers/{applicationId}/Application")]
        [ResponseType(typeof(List<Supplier>))]
        public async Task<IHttpActionResult> GetSuppliersByApplication(int applicationId)
        {
            var app = await db.Applications
                              .Where(p => p.Id == applicationId && p.Deleted != true)
                              .Include(b => b.Product)
                              .SingleOrDefaultAsync(); 
            
            if (app == null)
            {
                return NotFound();
            }

            var supplierId = app.Product?.SupplierId;



            var suppliers = await db.Suppliers.Where(x => x.Id == supplierId).Select(x => new SupplierResponse() { Id = x.Id, Name = x.Name} ).ToListAsync();

            //foreach (Product prod in app.Products)
            //{
            //    await db.Entry(prod).Reference(p => p.Supplier).LoadAsync();
            //    bool notFound = suppliers.Find(s => s.Id == prod.Supplier.Id) == null;
            //    if (notFound)
            //        suppliers.Add(prod.Supplier);
            //}

            return Ok(suppliers);
        }



        // PUT: api/Suppliers/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSupplier(int id, Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != supplier.Id)
            {
                return BadRequest();
            }

            db.Entry(supplier).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExists(id))
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

        // POST: api/Suppliers
        [ResponseType(typeof(Supplier))]
        public async Task<IHttpActionResult> PostSupplier(Supplier supplier)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            try
            {

                if (supplier.Products != null)
                {
                    supplier.MainContact = null;
                    supplier.Licenses = null;
                    supplier.Accounts = null;
                    supplier.Deleted = false;


                    var productsList = supplier.Products.ToList();
                    supplier.Products.Clear();

                    var originalSupplier = await db.Suppliers.FindAsync(supplier.Id);

                    if (originalSupplier != null)
                    {
                        db.Entry(originalSupplier).CurrentValues.SetValues(supplier);
                    }
                    else
                    {
                        db.Suppliers.Add(supplier);
                        await db.SaveChangesAsync();
                    }

                    originalSupplier = await db.Suppliers.FindAsync(supplier.Id);

                    foreach (Product product in productsList)
                    {
                        product.LicenseType = await db.LicenseTypes.FindAsync(product.LicenseType.Id);
                        product.LicenseTypeId = product.LicenseType.Id;

                        var originalProduct = await db.Products.FindAsync(product.Id);
                        if (originalProduct != null)
                        {
                            db.Entry(originalProduct).CurrentValues.SetValues(product);
                            originalSupplier.Products.Add(originalProduct);
                        }
                        else
                        {
                            product.CreatedDate = product.ModifiedDate = DateTime.Now;
                            product.Supplier = originalSupplier;
                            db.Entry(product).State = EntityState.Added;
                        }
                    }

                    await db.SaveChangesAsync();
                }

                return CreatedAtRoute("DefaultApi", new { id = supplier.Id }, supplier);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE: api/Suppliers/5
        [ResponseType(typeof(Supplier))]
        public async Task<IHttpActionResult> DeleteSupplier(int id)
        {
            Supplier supplier = await db.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            db.Suppliers.Remove(supplier);
            await db.SaveChangesAsync();

            return Ok(supplier);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SupplierExists(int id)
        {
            return db.Suppliers.Count(e => e.Id == id) > 0;
        }
    }
}