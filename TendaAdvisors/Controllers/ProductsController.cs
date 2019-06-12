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
    //[Authorize]
    //[RoutePrefix("Products")]
    public class ProductsController : BaseApiController
    {
       // private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Products
        public IQueryable<Product> GetProducts()
        {
            return db.Products;
        }


        [Route("Product/all/")]
        [ResponseType(typeof(ApplicationResponse))]
        public async Task<IHttpActionResult> GetAllProducts()
        {
            IEnumerable<ProductResponse> products = await (from prods in db.Products
                                                           select new ProductResponse() {
                                                           Id = prods.Id, Name = prods.Name}
                                                          ).ToListAsync();

            return Ok(products);
        }

        // GET: api/Contacts/nameSearch
        [Route("Product/Basic/{nameSearch}/{supplierID}")]
        public async Task<IHttpActionResult> GetBasicProduct(string nameSearch,int supplierID)
        {
            if (nameSearch == "undefined")
            {
                return NotFound();
            }

            List<Product> products = new List<Product>();

            if (nameSearch == "")
            {
                products = await db.Products
                    .Where(c => c.Supplier.Id == supplierID)
                    .Take(10)
                    .ToListAsync();
            }
            else
            {
                products = await db.Products
                    .Where(c => c.Name.Contains(nameSearch) && c.Supplier.Id==supplierID)
                    .Take(10)
                    .ToListAsync();
            }

            if (products.Count == 0)
            {
                return NotFound();
            }

            return Ok(products);
        }
        // GET: api/Products/5
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> GetProduct(int id)
        {
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await db.Entry(product.Supplier).ReloadAsync();

            return Ok(product);
        }

        // PUT: api/Products/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.Id)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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


        // POST: api/Products/LicenseTypes
        [Route("Products/LicenseTypes/{id}")]
        public async Task<IHttpActionResult> FetchProductsByLicenseTypes(List<LicenseType> licenseTypes,int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var licenseTypeIds = licenseTypes.Select(m => m.Id);
         

            List<ProductResponse> products = await (from p in db.Products
                                                    where p.SupplierId == id && licenseTypeIds.Contains(p.LicenseType.Id)
                                                    select new ProductResponse()
                                                    {
                                                        Id = p.Id,
                                                        Name = p.Name,
                                                        LicenseType = new LicenseTypeResponse()
                                                        { Id = p.LicenseTypeId, Description = p.LicenseType.Description, LicenseCategoryId = p.LicenseType.LicenseCategoryId, SubCategory = p.LicenseType.SubCategory,SupplierName=p.Supplier.Name }
                                                    }).ToListAsync();

            if (products == null)
            {
                return NotFound();
            }

            return Ok(products);
        }

        // POST: api/Products/LicenseTypesID/{id}/licenseId/{licenseTypeId}
        [Route("Products/LicenseTypesId/{id}/licenseId/{licenseTypeId}")]
        public async Task<IHttpActionResult> FetchProductsByLicenseTypesID(int id, int licenseTypeId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<ProductResponse> products = await (from p in db.Products
                                  where p.SupplierId == id && p.LicenseTypeId == licenseTypeId
                                  select new ProductResponse() { Id = p.Id, Name = p.Name, LicenseType = new LicenseTypeResponse()
                                  {Id = p.LicenseTypeId, Description = p.LicenseType.Description, LicenseCategoryId = p.LicenseType.LicenseCategoryId, SubCategory = p.LicenseType.SubCategory }
                                  }).ToListAsync();

            return Ok(products);
        }

        // POST: api/Products
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            product.Applications = null;

            if (product.Supplier != null) {
                product.Supplier = await db.Suppliers.FindAsync(product.Supplier.Id);
            }

            if (product.LicenseType != null) {
                product.LicenseType = await db.LicenseTypes.FindAsync(product.LicenseType.Id);
            }

            if (product.Id > 0)
            {
                var originalProduct = db.Products.Find(product.Id);
                db.Entry(originalProduct).CurrentValues.SetValues(product);
            }
            else
            {
                db.Products.Add(product);
            }
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        //This removes a product type, not the link between an application and a product.
        //Use DeleteProductFromApplication to unlink a product.
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> DeleteProduct(int id)
        {
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            await db.SaveChangesAsync();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.Id == id) > 0;
        }
    }
}