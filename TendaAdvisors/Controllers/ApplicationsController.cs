using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using TendaAdvisors.Models;
using TendaAdvisors.Models.Response;
using TendaAdvisors.Providers;

namespace TendaAdvisors.Controllers
{
    public class ApplicationsController : BaseApiController
    {

        private ApplicationUser _user
        {
            get
            {
                return HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>()
                    .FindById(HttpContext.Current.User.Identity.GetUserId());
            }
        }

        // GET: api/Applications
        [ResponseType(typeof(Application))]
        public IHttpActionResult GetApplicationsByAdvisorId()
        {
            //@TODO: Paging!

            if (_user == null)
            {
                return NotFound();
            }

            IEnumerable<Application> applications = new List<Application>();


            applications = db.Applications
                .Where(p => p.Deleted != true)
                .Include(b => b.Client)
                .Select(c => c);

            return Ok(applications);
        }

        // GET: api/ApplicationsIds
        [ResponseType(typeof(List<int>))]
        [Route("Applications/AllApplications/")]
        public async Task<IHttpActionResult> GetAllApplicationsIds()
        {
            //@TODO: Paging!
            List<int> applications = new List<int>();

            applications = await db.Applications
                      .Select(c => c.Id)
                      .ToListAsync();

            return Ok(applications);
        }

        // GET: api/Applications/5
        //[Route("Applications/{id}/")]
        [ResponseType(typeof(ApplicationResponse))]
        public async Task<IHttpActionResult> GetApplication(int id)
        {
            if (_user == null)
            {
                return NotFound();
            }

            if (!this.ApplicationExists(id))
            {
                return null;
            }

            try
            {
                ApplicationResponse application = await
                    (from x in db.Applications
                     join appSupp in db.ApplicationSuppliers on x.Id equals appSupp.ApplicationId
                     join supply in db.Suppliers on appSupp.SupplierId equals supply.Id
                     join adviser in db.Advisors on x.Advisor_Id equals adviser.Id
                     join contact in db.Contacts on adviser.ContactId equals contact.Id
                     where x.Deleted != true && x.Id == id
                     select new ApplicationResponse()
                     {
                         Id = x.Id,
                         ApplicationStatus_Id = x.ApplicationStatus_Id,
                         ApplicationType_Id = x.ApplicationType_Id,
                         Client_Id = x.Client_Id,
                         Product_Id = x.Product_Id,
                         Deleted = x.Deleted,
                         MemberNumber = appSupp.MemberNumber,
                         Product_Supplier_Name = supply.Name,
                         SupplierId = supply.Id,
                         Advisor_Id = adviser.Id,
                         CreatedDate = x.CreatedDate,
                         Advisor = new AdvisorResponse()
                         {
                             Id = adviser.Id,
                             IdNumber = contact.IdNumber,
                             MemberNumber = x.ApplicationNumber,
                             FirstName = contact.FirstName,
                             LastName = contact.LastName,
                             Cell1 = contact.Cell1,
                             Tel1 = contact.Tel1,
                             Email = contact.Email
                         }
                     }).FirstOrDefaultAsync();
                //Commented out this code because christine don't want applications without adviser to be defaulted to a default adviser

                if (application.Advisor != null)
                {
                    //Get current application 
                    var currentApplication = db.Applications.Find(id);

                    //Get application Adviser History 
                    var applicationIsLinked = db.ApplicationAdvisorHistory.Any(e => e.Application_Id == id);

                    //Check if the application has an adviser 
                    if (application.Advisor_Id > 0)
                    {
                        //Check if adviser effective date is not null 
                        if (!applicationIsLinked)
                        {
                            //Insert date of application as effective date 
                            var applicationadvisorhisotry = new ApplicationAdvisorHistory
                            {
                                New_Advisor = application.Advisor.Id,
                                Old_Advisor = application.Advisor.Id,
                                DateStarted = Convert.ToDateTime(currentApplication.CreatedDate),
                                Application_Id = application.Id
                            };

                            //Save ApplicationAdvisorHistory
                            try
                            {
                                db.ApplicationAdvisorHistory.Add(applicationadvisorhisotry);
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                Console.Write(ex.Message);
                            }

                        }
                    }

                }
                return Ok(application);
            }

            catch (Exception e)
            {
                throw e;
            }

        }

        [Route("Applications/complete/")]
        [ResponseType(typeof(ApplicationResponse))]
        public async Task<IHttpActionResult> GetCompleteApplications()
        {
            // if (_user == null || (_user != null && _user.AdvisorId == 0))
            if (_user == null)
            {
                return NotFound();
            }
            try
            {
                IEnumerable<ApplicationResponse> applicationComplete = new List<ApplicationResponse>();

                applicationComplete = await (from x in db.Applications
                                             join appSupp in db.ApplicationSuppliers on x.Id equals appSupp.ApplicationId
                                             join supply in db.Suppliers on appSupp.SupplierId equals supply.Id
                                             join contact in db.Contacts on x.Client_Id equals contact.Id
                                             join advisor in db.Advisors on x.Advisor_Id equals advisor.Id
                                             join advisorStatus in db.AdvisorStatuses on advisor.AdvisorStatus_Id equals advisorStatus.Id
                                             where x.Deleted != true && x.ApplicationStatus_Id == 6 && advisorStatus.Id == 5
                                             select new ApplicationResponse()
                                             {
                                                 Id = x.Id,
                                                 ApplicationStatus_Id = x.ApplicationStatus_Id,
                                                 ApplicationType_Id = x.ApplicationType_Id,
                                                 Advisor_Id = x.Advisor_Id,
                                                 AdvisorName = x.Advisor.Contact.FirstName,
                                                 AdvisorSurName = x.Advisor.Contact.LastName,
                                                 AdvisorIdnumber = x.Advisor.Contact.IdNumber,
                                                 Client_Id = x.Client_Id,
                                                 Product_Id = x.Product_Id,
                                                 Deleted = x.Deleted,
                                                 Product_Supplier_Name = supply.Name,
                                                 SupplierId = supply.Id,
                                                 Client = new ContactResponse
                                                 {
                                                     Id = contact.Id,
                                                     FirstName = contact.FirstName,
                                                     LastName = contact.LastName,
                                                     IdNumber = contact.IdNumber,
                                                     Cell1 = contact.Cell1,
                                                     Email = contact.Email,
                                                     Tel1 = contact.Tel1
                                                 }
                                             }).ToListAsync();
                return Ok(applicationComplete);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //Dashboard COMPLETE
        // GET: api/Applications/5
        [Route("Applications/completeAdvisor/")]
        [ResponseType(typeof(ApplicationResponse))]
        public async Task<IHttpActionResult> GetCompleteApplicationsForAdvisor()
        {
            if (_user == null)
            {
                return NotFound();
            }
            try
            {
                IEnumerable<ApplicationResponse> applicationComplete = new List<ApplicationResponse>();

                applicationComplete = await (from x in db.Applications
                                             join appSupp in db.ApplicationSuppliers on x.Id equals appSupp.ApplicationId
                                             join supply in db.Suppliers on appSupp.SupplierId equals supply.Id
                                             join contact in db.Contacts on x.Client_Id equals contact.Id
                                             join advisor in db.Advisors on x.Advisor_Id equals advisor.Id
                                             join advisorStatus in db.AdvisorStatuses on advisor.AdvisorStatus_Id equals advisorStatus.Id
                                             where x.Deleted != true && x.ApplicationStatus_Id == 6 && advisorStatus.Id == 5
                                             select new ApplicationResponse()
                                             {
                                                 Id = x.Id,
                                                 ApplicationStatus_Id = x.ApplicationStatus_Id,
                                                 ApplicationType_Id = x.ApplicationType_Id,
                                                 Advisor_Id = x.Advisor_Id,
                                                 AdvisorName = x.Advisor.Contact.FirstName,
                                                 AdvisorSurName = x.Advisor.Contact.LastName,
                                                 AdvisorIdnumber = x.Advisor.Contact.IdNumber,
                                                 AdvisorMemberNumber = x.ApplicationNumber,
                                                 Client_Id = x.Client_Id,
                                                 Product_Id = x.Product_Id,
                                                 Deleted = x.Deleted,
                                                 Product_Supplier_Name = supply.Name,
                                                 SupplierId = supply.Id,
                                                 Client = new ContactResponse
                                                 {
                                                     Id = contact.Id,
                                                     FirstName = contact.FirstName,
                                                     LastName = contact.LastName,
                                                     IdNumber = contact.IdNumber,
                                                     Cell1 = contact.Cell1,
                                                     Email = contact.Email,
                                                     Tel1 = contact.Tel1
                                                 }
                                             }).ToListAsync();
                return Ok(applicationComplete);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [Route("Applications/completeAdvisor/{AdviId}")]
        [ResponseType(typeof(ApplicationResponse))]
        public async Task<IHttpActionResult> GetCompleteApplicationsForAdvisor(int AdviId)
        {
            // if (_user == null || (_user != null && _user.AdvisorId == 0))
            if (_user == null)
            {
                return NotFound();
            }
            try
            {
                IEnumerable<ApplicationResponse> applicationComplete = new List<ApplicationResponse>();

                applicationComplete = await (from x in db.Applications
                                             join appSupp in db.ApplicationSuppliers on x.Id equals appSupp.ApplicationId
                                             join supply in db.Suppliers on appSupp.SupplierId equals supply.Id
                                             join contact in db.Contacts on x.Client_Id equals contact.Id
                                             join advisor in db.Advisors on x.Advisor_Id equals advisor.Id
                                             join advisorStatus in db.AdvisorStatuses on advisor.AdvisorStatus_Id equals advisorStatus.Id
                                             where x.Deleted != true && x.ApplicationStatus_Id == 6 && advisorStatus.Id == 5 && x.Advisor_Id == AdviId
                                             select new ApplicationResponse()
                                             {
                                                 Id = x.Id,
                                                 ApplicationStatus_Id = x.ApplicationStatus_Id,
                                                 ApplicationType_Id = x.ApplicationType_Id,
                                                 Advisor_Id = x.Advisor_Id,
                                                 AdvisorName = x.Advisor.Contact.FirstName,
                                                 AdvisorSurName = x.Advisor.Contact.LastName,
                                                 AdvisorIdnumber = x.Advisor.Contact.IdNumber,
                                                 AdvisorMemberNumber = x.ApplicationNumber,
                                                 Client_Id = x.Client_Id,
                                                 Product_Id = x.Product_Id,
                                                 Deleted = x.Deleted,
                                                 Product_Supplier_Name = supply.Name,
                                                 SupplierId = supply.Id,
                                                 Client = new ContactResponse
                                                 {
                                                     Id = contact.Id,
                                                     FirstName = contact.FirstName,
                                                     LastName = contact.LastName,
                                                     IdNumber = contact.IdNumber,
                                                     Cell1 = contact.Cell1,
                                                     Email = contact.Email,
                                                     Tel1 = contact.Tel1
                                                 }
                                             }).ToListAsync();
                return Ok(applicationComplete);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //Dashboard PENDING
        // GET: api/Applications/5
        [Route("Applications/pending/")]
        [ResponseType(typeof(Application))]
        public async Task<IHttpActionResult> GetPendingApplications()
        {
            if (_user == null)
            {
                return NotFound();
            }

            try
            {
                IEnumerable<ApplicationResponse> applicationPending = new List<ApplicationResponse>();

                applicationPending = await (from x in db.Applications
                                            join appSupp in db.ApplicationSuppliers on x.Id equals appSupp.ApplicationId
                                            join supply in db.Suppliers on appSupp.SupplierId equals supply.Id
                                            join contact in db.Contacts on x.Client_Id equals contact.Id
                                            join advisor in db.Advisors on x.Advisor_Id equals advisor.Id
                                            join advisorStatus in db.AdvisorStatuses on advisor.AdvisorStatus_Id equals advisorStatus.Id
                                            where x.Deleted != true && x.ApplicationStatus_Id == 2 && advisorStatus.Id == 5
                                            select new ApplicationResponse()
                                            {
                                                Id = x.Id,
                                                ApplicationStatus_Id = x.ApplicationStatus_Id,
                                                ApplicationType_Id = x.ApplicationType_Id,
                                                Advisor_Id = x.Advisor_Id,
                                                AdvisorName = x.Advisor.Contact.FirstName,
                                                AdvisorSurName = x.Advisor.Contact.LastName,
                                                AdvisorIdnumber = x.Advisor.Contact.IdNumber,
                                                AdvisorMemberNumber = x.ApplicationNumber,
                                                Client_Id = x.Client_Id,
                                                Product_Id = x.Product_Id,
                                                Deleted = x.Deleted,
                                                Product_Supplier_Name = supply.Name,
                                                SupplierId = supply.Id,
                                                Client = new ContactResponse
                                                {
                                                    Id = contact.Id,
                                                    FirstName = contact.FirstName,
                                                    LastName = contact.LastName,
                                                    IdNumber = contact.IdNumber,
                                                    Cell1 = contact.Cell1,
                                                    Email = contact.Email,
                                                    Tel1 = contact.Tel1
                                                }
                                            }).ToListAsync();
                return Ok(applicationPending);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [Route("Applications/PendingAdvisor/{AdviId}")]
        [ResponseType(typeof(Application))]
        public async Task<IHttpActionResult> GetPendingApplicationsForAdvisor(int AdviId)
        {
            if (_user == null)
            {
                return NotFound();
            }

            try
            {

                IEnumerable<ApplicationResponse> applicationPending = new List<ApplicationResponse>();

                applicationPending = await (from x in db.Applications
                                            join appSupp in db.ApplicationSuppliers on x.Id equals appSupp.ApplicationId
                                            join supply in db.Suppliers on appSupp.SupplierId equals supply.Id
                                            join contact in db.Contacts on x.Client_Id equals contact.Id
                                            join advisor in db.Advisors on x.Advisor_Id equals advisor.Id
                                            join advisorStatus in db.AdvisorStatuses on advisor.AdvisorStatus_Id equals advisorStatus.Id
                                            where x.Deleted != true && x.ApplicationStatus_Id == 2 && advisorStatus.Id == 5 && x.Advisor_Id == AdviId
                                            select new ApplicationResponse()
                                            {
                                                Id = x.Id,
                                                ApplicationStatus_Id = x.ApplicationStatus_Id,
                                                ApplicationType_Id = x.ApplicationType_Id,
                                                Advisor_Id = x.Advisor_Id,
                                                AdvisorName = x.Advisor.Contact.FirstName,
                                                AdvisorSurName = x.Advisor.Contact.LastName,
                                                AdvisorIdnumber = x.Advisor.Contact.IdNumber,
                                                AdvisorMemberNumber = x.ApplicationNumber,
                                                Client_Id = x.Client_Id,
                                                Product_Id = x.Product_Id,
                                                Deleted = x.Deleted,
                                                Product_Supplier_Name = supply.Name,
                                                SupplierId = supply.Id,
                                                Client = new ContactResponse
                                                {
                                                    Id = contact.Id,
                                                    FirstName = contact.FirstName,
                                                    LastName = contact.LastName,
                                                    IdNumber = contact.IdNumber,
                                                    Cell1 = contact.Cell1,
                                                    Email = contact.Email,
                                                    Tel1 = contact.Tel1
                                                }
                                            }).ToListAsync();
                return Ok(applicationPending);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [Route("Applications/new")]
        [ResponseType(typeof(Application))]
        public async Task<IHttpActionResult> GetNewApplications()
        {
            if (_user == null)
            {
                return NotFound();
            }

            try
            {

                IEnumerable<ApplicationResponse> newApplications = new List<ApplicationResponse>();

                newApplications = await (from x in db.Applications
                                         join appSupp in db.ApplicationSuppliers on x.Id equals appSupp.ApplicationId
                                         join supply in db.Suppliers on appSupp.SupplierId equals supply.Id
                                         join contact in db.Contacts on x.Client_Id equals contact.Id
                                         join advisor in db.Advisors on x.Advisor_Id equals advisor.Id
                                         join advisorStatus in db.AdvisorStatuses on advisor.AdvisorStatus_Id equals advisorStatus.Id
                                         where x.Deleted != true && x.ApplicationStatus_Id == 1 && advisorStatus.Id == 5
                                         select new ApplicationResponse()
                                         {
                                             Id = x.Id,
                                             ApplicationStatus_Id = x.ApplicationStatus_Id,
                                             ApplicationType_Id = x.ApplicationType_Id,
                                             Advisor_Id = x.Advisor_Id,
                                             AdvisorName = x.Advisor.Contact.FirstName,
                                             AdvisorSurName = x.Advisor.Contact.LastName,
                                             AdvisorIdnumber = x.Advisor.Contact.IdNumber,
                                             AdvisorMemberNumber = x.ApplicationNumber,
                                             Client_Id = x.Client_Id,
                                             Product_Id = x.Product_Id,
                                             Deleted = x.Deleted,
                                             Product_Supplier_Name = supply.Name,
                                             SupplierId = supply.Id,
                                             Client = new ContactResponse
                                             {
                                                 Id = contact.Id,
                                                 FirstName = contact.FirstName,
                                                 LastName = contact.LastName,
                                                 IdNumber = contact.IdNumber,
                                                 Cell1 = contact.Cell1,
                                                 Email = contact.Email,
                                                 Tel1 = contact.Tel1
                                             }
                                         }).ToListAsync();
                return Ok(newApplications);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [Route("Applications/NewAdvisor/{AdviId}")]
        [ResponseType(typeof(Application))]
        public async Task<IHttpActionResult> GetNewApplicationsForAdvisor(int AdviId)
        {
            if (_user == null)
            {
                return NotFound();
            }

            try
            {
                IEnumerable<ApplicationResponse> newApplications = new List<ApplicationResponse>();

                newApplications = await (from x in db.Applications
                                         join appSupp in db.ApplicationSuppliers on x.Id equals appSupp.ApplicationId
                                         join supply in db.Suppliers on appSupp.SupplierId equals supply.Id
                                         join contact in db.Contacts on x.Client_Id equals contact.Id
                                         join advisor in db.Advisors on x.Advisor_Id equals advisor.Id
                                         join advisorStatus in db.AdvisorStatuses on advisor.AdvisorStatus_Id equals advisorStatus.Id
                                         where x.Deleted != true && x.ApplicationStatus_Id == 1 && advisorStatus.Id == 5 && x.Advisor_Id == AdviId
                                         select new ApplicationResponse()
                                         {
                                             Id = x.Id,
                                             ApplicationStatus_Id = x.ApplicationStatus_Id,
                                             ApplicationType_Id = x.ApplicationType_Id,
                                             Advisor_Id = x.Advisor_Id,
                                             AdvisorName = x.Advisor.Contact.FirstName,
                                             AdvisorSurName = x.Advisor.Contact.LastName,
                                             AdvisorIdnumber = x.Advisor.Contact.IdNumber,
                                             AdvisorMemberNumber = x.ApplicationNumber,
                                             Client_Id = x.Client_Id,
                                             Product_Id = x.Product_Id,
                                             Deleted = x.Deleted,
                                             Product_Supplier_Name = supply.Name,
                                             SupplierId = supply.Id,
                                             Client = new ContactResponse
                                             {
                                                 Id = contact.Id,
                                                 FirstName = contact.FirstName,
                                                 LastName = contact.LastName,
                                                 IdNumber = contact.IdNumber,
                                                 Cell1 = contact.Cell1,
                                                 Email = contact.Email,
                                                 Tel1 = contact.Tel1
                                             }
                                         }).ToListAsync();
                return Ok(newApplications);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // GET: api/Application/PutApplicationStatus/
        [HttpPut]
        [Route("Application/PutApplicationStatus/{applicationId}/{applicationStatus}")]
        public async Task<IHttpActionResult> PutApplicationStatus(int applicationId, int applicationStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                Application app = db.Applications.Find(applicationId);
                ApplicationStatus appStat = db.ApplicationStatuses.Find(applicationStatus);
                if (app != null)
                {
                    app.ApplicationStatus = appStat;
                    db.Entry(app).CurrentValues.SetValues(app);
                }

                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: api/Applications/5/Documents
        [Route("Applications/{id}/Documents")]
        [ResponseType(typeof(IQueryable<DocumentTypeResponse>))]
        public async Task<IHttpActionResult> GetApplicationDocuments(int id)
        {

            if (!this.ApplicationExists(id))
            {
                return null;
            }

            IEnumerable<DocumentTypeResponse> applicationDocuments = 
                await (from doc in db.ApplicationDocuments
                       join app in db.Applications on doc.ApplicationId equals app.Id
                       where app.Id == id
                       select new DocumentTypeResponse()
                       {
                           Id = doc.Id,
                           Name = doc.OriginalFileName,
                           DocumentTypeName = doc.DocumentType.Name,
                           DocumentTypeId = doc.DocumentTypeId,
                           Uploaded = doc.Uploaded,
                           ValidFromDate = doc.ValidFromDate,
                           ValidToDate = doc.ValidToDate,
                           Title = doc.Title,
                           IsExpired = doc.IsExpired
                       }).ToListAsync();
            return Ok(applicationDocuments);
        }

        // GET: api/Applications/ApplicationDocumentsByApplicationId/{id}
        [Route("Applications/ApplicationDocumentsByApplicationId/{id}")]
        [ResponseType(typeof(IQueryable<DocumentTypeResponse>))]
        public async Task<IHttpActionResult> GetApplicationDocumentsByApplicationId(int id)
        {
            if (!this.ApplicationExists(id))
            {
                return null;
            }

            IEnumerable<DocumentTypeResponse>  applicationDocuments = 
                await (from doc in db.ApplicationDocuments
                       join app in db.Applications on doc.ApplicationId equals app.Id
                       where app.Id == id
                       select new DocumentTypeResponse()
                       {
                           Id = doc.Id,
                           ApplicationId =app.Id,
                           Name = doc.OriginalFileName,
                           DocumentTypeName = doc.DocumentType.Name,
                           DocumentTypeId = doc.DocumentTypeId,
                           Uploaded = doc.Uploaded,
                           ValidFromDate = doc.ValidFromDate,
                           ValidToDate = doc.ValidToDate,
                           Title = doc.Title,
                           IsExpired = doc.IsExpired
                       }).OrderBy(a=>a.DocumentTypeName).ToListAsync();
            return Ok(applicationDocuments);
        }

        [Route("Applications/clientId/{id}")]
        [ResponseType(typeof(IQueryable<ApplicationResponse>))]
        public async Task<IHttpActionResult> GetApplicationsbyClientId(int id)
        {

            IEnumerable<ApplicationResponse> applications = new List<ApplicationResponse>();

            applications = await (from x in db.Applications
                                  join appSupp in db.ApplicationSuppliers on x.Id equals appSupp.ApplicationId
                                  join supply in db.Suppliers on appSupp.SupplierId equals supply.Id
                                  where x.Deleted != true && x.Client_Id == id
                                  select new ApplicationResponse()
                                  {
                                      Id = x.Id,
                                      ApplicationStatus_Id = x.ApplicationStatus_Id,
                                      ApplicationType_Id = x.ApplicationType_Id,
                                      Client_Id = x.Client_Id,
                                      Product_Id = x.Product_Id,
                                      Deleted = x.Deleted,
                                      MemberNumber = appSupp.MemberNumber,
                                      Product_Supplier_Name = supply.Name,
                                      SupplierId = supply.Id,
                                      Advisor_Id = x. Advisor_Id}                           
                                  ).ToListAsync();
            return Ok(applications);
        }

        // PUT: api/Applications/5/Products/
        [ResponseType(typeof(void))]
        // [HttpPut]
        [Route("Applications/{applicationId}/Products")]
        public async Task<IHttpActionResult> PutApplicationProducts(int applicationId, BasicApplicationResponse applicationResponse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var originalApplication = db.Applications
                .Include(c => c.ApplicationSuppliers)
                .FirstOrDefault(x => x.Id == applicationId);

            var ApplicationSuppliers = originalApplication.ApplicationSuppliers.ToList();
            ApplicationSuppliers.ForEach(x => originalApplication.ApplicationSuppliers.Remove(x));
            var productList = await db.Products.Where(entry => entry.SupplierId == applicationResponse.SupplierId).ToListAsync();
            
            var product = await db.Products.FirstOrDefaultAsync(p => p.Id == applicationResponse.Product_Id);
            
            originalApplication.Product = product;
            originalApplication.ApplicationSuppliers.Add(
                new ApplicationSupplier
                {
                    ApplicationId = applicationResponse.Id,
                    SupplierId = Convert.ToInt32(applicationResponse.SupplierId),
                    MemberNumber = applicationResponse.MemberNumber
                });

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (!ApplicationExists(applicationId))
                {
                    return NotFound();
                }
                else
                {
                    throw e;
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/Applications/5
        [ResponseType(typeof(void))]
        [Route("Applications/putApplication/{id}/")]
        [HttpPut] 
        public async Task<IHttpActionResult> PutApplication(int id, ApplicationResponse application)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != application.Id)
            {
                return BadRequest();
            }

            //This item was saved, so update modified time.
            application.ModifiedDate = DateTime.Now;

            Application originalApplication = await db.Applications
                .Include(a => a.Advisor)
                .Include(a => a.ApplicationType)
                .Include(a => a.ApplicationStatus)
                .FirstOrDefaultAsync(c => c.Id == application.Id);

            if (application.ApplicationStatus_Id != null)
            {
                if(application.ApplicationStatus_Id != 6)
                { 
                    var AppStat = await db.ApplicationStatuses.FindAsync(application.ApplicationStatus_Id);
                    if (AppStat != null)
                    {
                        originalApplication.ApplicationStatus = AppStat;
                    }
                }
                else
                {
                    originalApplication.ApplicationStatus = await db.ApplicationStatuses.FirstOrDefaultAsync(s => s.Status == "New");
                }
            }

            if (application.ApplicationType_Id  != null)
            {
                var appTypeType = await db.ApplicationTypes.FindAsync(application.ApplicationType_Id);
                if (appTypeType != null)
                {
                    originalApplication.ApplicationType = appTypeType;               
                }
            }

            if (application.Advisor != null)
            {
                var advisor = await db.Advisors.FindAsync(application.Advisor.Id);
                if (advisor != null)
                {
                    originalApplication.Advisor = advisor;
                    originalApplication.Advisor_Id = advisor.Id;
                }
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (!ApplicationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw e;
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Applications
        [HttpPost]
        [ResponseType(typeof(Application))]
        public async Task<IHttpActionResult> PostApplication(Application application)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_user == null || (_user != null && _user.AdvisorId == 0))
            {
                return NotFound();
            }

            //This item was saved, so update created & modified time.
            application.ModifiedDate = application.ModifiedDate ?? DateTime.Now;
            application.CreatedDate = application.CreatedDate ?? DateTime.Now;

            //Christine Asked requested that no application should Have default adviser i.e should stay null
            if (application.Advisor.Id == 0)
            {
                //if advisor not set link logged in user
                application.Advisor = db.Advisors.Find(_user.AdvisorId);
            }

            //application.ApplicationType = db.ApplicationTypes.First(a => a.Title=="New Application");
            application.Deleted = false;
            application.ApplicationDocuments = null;
            application.Product = null;
            application.Queries = null;
            application.ApplicationSuppliers = null;

            if (application.Client != null)
            {
                var client =  db.Contacts.Find(application.Client.Id);
                if (client != null)
                {
                    application.Client = client;
                }

                if (application.Client.ContactTitle == null)
                {
                    //Use default contact type
                    var title = await db.ContactTitles.FindAsync(1);
                    if (title != null)
                    {
                        application.Client.ContactTitle = title;
                    }
                }
                else
                {
                    //Use given contact type
                    var title = await db.ContactTitles.FindAsync(application.Client.ContactTitle.Id);

                    if (title != null)
                    {
                        application.Client.ContactTitle = title;
                    }
                    else
                    {
                        title = await db.ContactTitles.FindAsync(1);

                        if (title != null)
                        {
                            application.Client.ContactTitle = title;
                        }
                    }
                }
            }

            if (application.Advisor != null)
            {
                var advisorId = db.Advisors.Where(x => x.Id == application.Advisor.Id && x.AdvisorType_Id==2).FirstOrDefault();

                if (advisorId != null)
                {
                    application.Advisor = advisorId;
                    // Stop changing adviser's effective date to current date of application
                    // application.Advisor.EffectiveStartDate = DateTime.Now;
                     
                    var applicationadvisorhistory = new ApplicationAdvisorHistory
                    {
                        New_Advisor = advisorId.Id,
                        Old_Advisor = advisorId.Id,
                        DateStarted = application.ModifiedDate ?? DateTime.Now,
                        Application_Id = application.Id,
                    };

                    //Save ApplicationAdvisorHistory
                    db.ApplicationAdvisorHistory.Add(applicationadvisorhistory);
                }
            }

            if (application.ApplicationStatus == null)
            {
                application.ApplicationStatus = db.ApplicationStatuses.First(s => s.Status == "New");
            }
            else
            {
                var appStatusType =  db.ApplicationStatuses.Find(application.ApplicationStatus.Id);
                if (appStatusType != null)
                {
                    application.ApplicationStatus = appStatusType;
                }
            }

            if (application.ApplicationType == null)
            {
                application.ApplicationType = db.ApplicationTypes.First(s => s.Title == "New Application");
            }
            else
            { 
                var appType=  db.ApplicationTypes.Find(application.ApplicationType.Id);
                if (appType != null)
                {
                    application.ApplicationType = appType;
                }
            }

            if (application.Client != null && application.Client.Addresses != null)
            {
                var addressList = application.Client.Addresses.ToList();

                foreach (Address address in addressList)
                {
                    //AddressType
                    if (address.AddressType != null)
                    {
                        var advContactAddressType = db.AddressTypes.Find(address.AddressType.Id);
                        if (advContactAddressType != null)
                        {
                            address.AddressType = advContactAddressType;
                        }
                    }

                    //Province
                    if (address.Province != null)
                    {
                        var advContactAddressProvince =  db.Provinces.Find(address.Province.Id);
                        if (advContactAddressProvince != null)
                        {
                            address.Province = advContactAddressProvince;
                        }
                    }

                    //Country
                    if (address.Country != null)
                    {
                        var advContactAddressCountry = db.Countries.Find(address.Country.Id);
                        if (advContactAddressCountry != null)
                        {
                            address.Country = advContactAddressCountry;
                        }
                    }
                }
            }
            db.Applications.Add(application);
            db.Entry(application).State = EntityState.Added;

            if (application.ApplicationStatus != null)
            {
                db.Entry(application.ApplicationStatus).State = EntityState.Unchanged;
            }

            if (db.Entry(application.Client).Entity.Id != 0)
            {
                db.Entry(application.Client).State = EntityState.Modified;
            }
            else
            {
                db.Entry(application.Client).State = EntityState.Added;
            }

            //----------Address------------------------------------------

            if (application.Client != null && application.Client.Addresses != null)
            {
                var addresses = application.Client.Addresses.ToList();
                foreach (Address address in addresses)
                {
                    if (address.Id != 0)
                        db.Entry(address).State = EntityState.Modified;
                    else
                        db.Entry(address).State = EntityState.Added;
                }
            }


            //Check if User is already in the exception list 
            //Get user from the Exception List 
            var notExistingUser = db.UnmatchedCommissions.FirstOrDefault(e => (e.MemberSearchValue == application.Client.IdNumber) || (e.MemberSearchValue == application.ApplicationNumber));

            if (notExistingUser != null)
            {
                //Update client with adviser 
                notExistingUser.AdvisorName = application.Advisor.User.FullName;
            }

            try
            {
                 await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }

            return CreatedAtRoute("DefaultApi", new { id = application.Id }, application);
        }

        // DELETE: api/Applications/5
        [Route("Application/{id}")]
        [ResponseType(typeof(Application))]
        public async Task<IHttpActionResult> DeleteApplication(int id)
        {
            List<ApplicationAdvisorHistory> applicationHistory = await db.ApplicationAdvisorHistory
                .Where(match => match.Application_Id == id)
                .ToListAsync();

            Application application = await db.Applications
                .Where(app => app.Id == id)
                .FirstOrDefaultAsync();

            if (application == null)
            {
                return NotFound();
            }

            try
            {
                if (applicationHistory != null)
                {
                    foreach (ApplicationAdvisorHistory entry in applicationHistory)
                    {
                        db.ApplicationAdvisorHistory.Remove(entry);
                        db.Entry(entry).State = EntityState.Deleted;
                    }
                }

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }

            try
            {
                db.Entry(application).State = EntityState.Deleted;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }

            return Ok(application);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ApplicationExists(int id)
        {
            return db.Applications.Count(e => e.Id == id) > 0;
        }

        [Route("Application/GetApplicationHistory/{applicationId}")]
        public async Task<IHttpActionResult> GetApplicationAdvisorHistory(int applicationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var applicationAdvisorHistory = await
                     db.ApplicationAdvisorHistory
                     .Where(e => e.Application_Id == applicationId)
                     .Join(db.Advisors,
                     AppAdviHist => AppAdviHist.New_Advisor,
                     newAdvi => newAdvi.Id,
                     (AppAdviHist , Advi) => new {
                         AppAdviHist.DateStarted,
                         AppAdviHist.DateEnded,
                         NewAdviContactId = Advi.ContactId
                     })
                     .Join(db.Contacts,
                     AppAdviHist => AppAdviHist.NewAdviContactId,
                     newAdviCont => newAdviCont.Id,
                     (AppAdviHist, newAdviCont) => new {
                         AppAdviHist.DateStarted,
                         AppAdviHist.DateEnded,
                         newAdviName = newAdviCont.FirstName + " " +newAdviCont.LastName,
                         newAdviIdNum = newAdviCont.IdNumber
                     })
                     .Select(x => new {
                         x.DateStarted,
                         x.DateEnded,
                         x.newAdviName,
                         x.newAdviIdNum
                     }).ToListAsync();

                return Ok(applicationAdvisorHistory);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("Application/GetApplicationAdvisorEditHistory/{applicationId}")]
        public async Task<IHttpActionResult> GetApplicationAdvisorEditHistory(int applicationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var applicationAdvisorEditHistory = await
                     db.ApplicationAdvisorEditHistory
                     .Where(e => e.ApplicationId == applicationId)
                     .Join(db.Advisors,
                     AppAdviHist => AppAdviHist.AdvisorId,
                     newAdvi => newAdvi.Id,
                     (AppAdviHist, Advi) => new {
                         AppAdviHist.DateEdited,
                         AdvisorContactId = Advi.ContactId
                     })
                     .Join(db.Contacts,
                     AppAdviHist => AppAdviHist.AdvisorContactId,
                     newAdviCont => newAdviCont.Id,
                     (AppAdviHist, newAdviCont) => new {
                         AppAdviHist.DateEdited,
                         AdvisorName = newAdviCont.FirstName + " " + newAdviCont.LastName,
                         AdvisorIdNumber = newAdviCont.IdNumber
                     })
                     .Select(x => new {
                         x.DateEdited,
                         x.AdvisorName,
                         x.AdvisorIdNumber
                     })
                     .ToListAsync();
                if (applicationAdvisorEditHistory != null)
                {
                    return Ok(applicationAdvisorEditHistory);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("Application/PostApplicationAvisorEditHistory/{applicationId}")]
        public IHttpActionResult PostApplicationAvisorEditHistory(int applicationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_user == null || (_user != null && _user.AdvisorId == 0))
            {
                return NotFound();
            }

            // This item's modified time.
            DateTime ModifiedDate = DateTime.Now;

            ApplicationAdvisorEditHistory ApplicationAdvisorEditHistoryEntry = new ApplicationAdvisorEditHistory
            {
                DateEdited = ModifiedDate,
                AdvisorId = _user.AdvisorId,
                ApplicationId = applicationId,
            };

            db.ApplicationAdvisorEditHistory.Add(ApplicationAdvisorEditHistoryEntry);
            db.SaveChanges();

            return Ok();
        }

        [HttpGet]
        [Route("Application/CheckClientDuplicateSupplier/{clientId}/{supplierId}")]
        public IHttpActionResult CheckClientDuplicateSupplier(int clientId, int supplierId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (clientId != 0)
            {
                List<Application> applications = db.Applications
                    .Where(match => match.Client_Id == clientId && match.Deleted != true)
                    .ToList();

                // initialise it with the incomming supplier id
                // So when there are duplicates it could be found faster 
                List<int> clientCurrentSuppliers = new List<int>
                {
                    supplierId
                };

                if (applications.Count != 0 || applications != null)
                {
                    foreach(Application app in applications)
                    {
                        var productId = app.Product_Id;

                        int dbSupplierId = db.Products
                            .Where(prod => prod.Id == productId)
                            .Select(sup => sup.SupplierId).FirstOrDefault();

                        if(dbSupplierId == 0)
                        {
                            continue;
                        }
                        else if (clientCurrentSuppliers.Contains(dbSupplierId))
                        {
                            // Returning true to not allow the new application
                            // since it has an application with current supplier 
                            return Ok(true);
                        }
                        else
                        {
                            // The supplier as not been added to the list.
                            clientCurrentSuppliers.Add(dbSupplierId);
                        }
                    }
                }
                // The foreach would have returned if there were duplicate suppliers
                // So by default if no duplucates were found then return false
                return Ok(false);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
