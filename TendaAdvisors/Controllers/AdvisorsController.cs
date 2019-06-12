using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using TendaAdvisors.Models;
using TendaAdvisors.Models.DTO;
using TendaAdvisors.Models.Response;
using TendaAdvisors.Providers;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System.Net.Mail;
using System.Configuration;
using System.Net.Mime;
using System.Text;

namespace TendaAdvisors.Controllers
{

    public class AdvisorsDTO
    {
        public IQueryable<GetAdvisorsResp> Advisors { get; set; }
        public int Count { get; set; }
    }

    public class AdvisorListItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
    }
    //[Authorize]
    public class AdvisorsController : BaseApiController
    {
        //private ApplicationDbContext db;// = new ApplicationDbContext();

        //public AdvisorsController(ApplicationDbContext dbcontext) {
        //    db = dbcontext;
        //}
        private readonly string workingFolder = HttpRuntime.AppDomainAppPath + @"Uploads\ExportFiles";
        private ApplicationUser _user
        {
            get
            {
                return HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>()
                    .FindById(HttpContext.Current.User.Identity.GetUserId());
            }
        }


        // GET: api/Advisors
        [Route("Advisors/All")]
        public List<AdvisorListItem> GetAllAdvisors()
        {
            var adv = db.Advisors.Include(c => c.Contact).Select(a => new AdvisorListItem()
            {
                Id = a.Id,
                Name = (a.Contact.FirstName + " " + a.Contact.LastName + " (" + a.FsbCode + "/" + a.CmsCode + ")")
            }).ToList();
            return adv;
        }

        // GET: api/Advisors
        [Route("Advisors/typeTwo/All")]
        public List<AdvisorListItem> GetAllAdvisorsTypeTwo()
        {
            var adv = db.Advisors
                .Where(a => a.AdvisorType_Id == 2)
                .Include(c => c.Contact).Select(a => new AdvisorListItem()
                {
                    Id = a.Id,
                    Name = (a.Contact.FirstName + " " + a.Contact.LastName + " (" + a.FsbCode + "/" + a.CmsCode + ")"),
                    EffectiveStartDate = a.EffectiveStartDate,
                    EffectiveEndDate = a.EffectiveEndDate
                }).ToList();

            for (int i = 0; i < adv.Count; i++)
            {
                if (adv[i].EffectiveStartDate != null && adv[i].EffectiveStartDate > DateTime.Now)
                {
                    adv.Remove(adv[i]);
                    continue;
                }

                if (adv[i].EffectiveEndDate != null && adv[i].EffectiveEndDate <= DateTime.Now)
                {
                    adv.Remove(adv[i]);
                    continue;
                }
            }



            return adv;
        }


        // GET: api/Advisors
        [Route("Advisors/Id/AdisorTypeAdvisor/{contactId}")]
        public async Task<IHttpActionResult> GetAdvisorIdTypeAdvisor(int contactId)
        {
            try
            {
                var advisorId = await db.Advisors.Where(c => c.ContactId == contactId && c.AdvisorType_Id == 2).Select(x => x.Id).FirstOrDefaultAsync();
                return Ok(advisorId);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // GET: api/Advisors
        [Route("Advisors/Id/{contactId}")]
        public async Task<IHttpActionResult> GetAdvisorId(int contactId)
        {
            try
            {
                var advisorId = await db.Advisors.Where(c => c.ContactId == contactId).Select(x => x.Id).FirstOrDefaultAsync();
                return Ok(advisorId);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // GET: api/Advisors
        [Route("Advisors/Id/{contactId}/AdvisorTypeID/{advisorTypeId}")]
        public async Task<IHttpActionResult> GetAdvisorIdOfAdvisorType(int contactId, int advisorTypeID)
        {
            try
            {
                var advisorId = await db.Advisors.Where(c => c.ContactId == contactId && c.AdvisorType_Id == advisorTypeID).Select(x => x.Id).FirstOrDefaultAsync();
                return Ok(advisorId);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // GET: api/Advisors
        [Route("Advisors/AdviserType/Id/{contactId}")]
        public async Task<IHttpActionResult> GetAdvisorCompanyId(int contactId)
        {
            try
            {
                var advisorId = await db.Advisors.Where(c => c.ContactId == contactId && c.AdvisorType.Id == 5).Select(x => x.Id).FirstOrDefaultAsync();
                return Ok(advisorId);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }


        //COMPANY
        [Route("Advisors/Company")]
        public async Task<IHttpActionResult> GetAdvisorsCompany()
        {
            try
            {
                var advisors = await db.Advisors.Where(x => x.AdvisorType_Id == 5 && x.Deleted != true).Select(c => c.ContactId).ToListAsync();

                var advisorContacts = await db.Contacts
                                   .Where(p => advisors.Contains(p.Id))
                                   .Select(c => new CompanyAdvisorResponse() { Id = c.Id, Name = c.FirstName }).ToListAsync();

                return Ok(advisorContacts);
            }

            catch (Exception e)
            {
                throw e;
            }

        }

        //NOTCOMPANY
        [Route("Advisors/NotCompany")]
        public async Task<IHttpActionResult> GetAdvisorsNotCompany()
        {
            try
            {
                var companyAdvisors = await db.Advisors.Where(x => x.AdvisorType_Id == 5).Select(c => c.ContactId).ToListAsync();

                var advisorContacts = await db.Contacts
                                   .Where(p => !companyAdvisors.Contains(p.Id))
                                   .Select(c => new NotCompanyContactsResponse() { Id = c.Id }).ToListAsync();

                return Ok(advisorContacts);
            }

            catch (Exception e)
            {
                throw e;
            }

        }




        //ADVISORS
        [Route("Advisors/Admin")]
        [ResponseType(typeof(Advisor))]
        public async Task<IHttpActionResult> GetAdvisorsAdmin()
        {
            try
            {
                var advisors = await db.Advisors.Where(x => x.AdvisorType_Id == 6).Select(c => c.ContactId).ToListAsync();

                var advisorContacts = await db.Contacts
                                   .Where(p => advisors.Contains(p.Id))
                                   .Select(c => new KeyCompanyAdvisorResponse() { Id = c.Id, Name = c.FirstName, LastName = c.LastName, IdNumber = c.IdNumber}).ToListAsync();

                return Ok(advisorContacts);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }



        //ADVISORS
        [Route("Advisors/Advisor")]
        [ResponseType(typeof(Advisor))]
        public async Task<IHttpActionResult> GetAdvisorsAdvisor()
        {
            try
            {
                var advisorContacts = await (from c in db.Contacts
                                             join add in db.Advisors on c.Id equals add.ContactId
                                             select new AdvisorCompanyAdvisorResponse()
                                             {
                                                 Id = c.Id,
                                                 Name = c.FirstName,
                                                 LastName = c.LastName,
                                                 IdNumber = c.IdNumber,
                                                 Advisor_Id = add.Id,
                                                 AdvisorType_Id = add.AdvisorType_Id ?? 0
                                             }).ToListAsync();
                return Ok(advisorContacts);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }


        //ADVISORS
        [Route("Advisors/Advisor/advisorType/Advisor")]
        [ResponseType(typeof(Advisor))]
        public async Task<IHttpActionResult> GetAdvisorsAdvisorTypeAdvisor()
        {
            try
            {
                var advisorContacts = await (from c in db.Contacts
                                             join add in db.Advisors on c.Id equals add.ContactId
                                             where add.AdvisorType_Id == 2
                                             select new AdvisorCompanyAdvisorResponse()
                                             {
                                                 Id = c.Id,
                                                 Name = c.FirstName,
                                                 LastName = c.LastName,
                                                 IdNumber = c.IdNumber,
                                                 Advisor_Id = add.Id,
                                                 AdvisorType_Id = add.AdvisorType_Id ?? 0
                                             }).ToListAsync();
                return Ok(advisorContacts);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        //ADVISORS
        [Route("Advisors/Advisor/{id}")]
        [ResponseType(typeof(Advisor))]
        public async Task<IHttpActionResult> GetAdvisorsAdvisorId(int id)
        {
            try
            {
                var advisors = await db.Advisors.Where(x => x.Id == id && x.AdvisorType_Id == 2).Select(c => c.ContactId).ToListAsync();
                var advisorContacts = await db.Contacts
                                   .Where(p => advisors.Contains(p.Id))
                                   .Select(c => new AdvisorCompanyAdvisorResponse()
                                   {
                                       Id = c.Id,
                                       Name = c.FirstName,
                                       LastName = c.LastName,
                                       IdNumber = c.IdNumber,
                                   }).ToListAsync();
                return Ok(advisorContacts);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }


        //ADVISORS
        [Route("Advisors/Advisor/string/{nameSearch}")]
        [ResponseType(typeof(Advisor))]
        public async Task<IHttpActionResult> GetAdvisorsAdvisorByString(string nameSearch)
        {
            try
            {
                if (nameSearch == "undefined" || nameSearch == "")
                {
                    return NotFound();
                }

                var advisorContacts = await (from advisor in db.Advisors
                                             join c in db.Contacts on advisor.ContactId equals c.Id
                                             where (
                                             (c.IdNumber.Contains(nameSearch)) || 
                                             (c.FirstName.Equals(nameSearch)) || 
                                             (c.LastName.Equals(nameSearch)) && 
                                             (advisor.AdvisorType_Id == 2))
                                             select new AdvisorCompanyAdvisorResponse
                                             {
                                                 Id = c.Id,
                                                 Name = c.FirstName,
                                                 LastName = c.LastName,
                                                 IdNumber = c.IdNumber,
                                                 Advisor_Id = advisor.Id,
                                                 AdvisorType_Id = advisor.AdvisorType_Id
                                             }).ToListAsync();

                return Ok(advisorContacts);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        //ADVISORS
        [Route("Advisors/similar/{nameSearch}")]
        [ResponseType(typeof(Advisor))]
        public async Task<IHttpActionResult> GetSimilarAdvisor(string nameSearch)
        {
            //namesearch : ID Name  lastname
            //          0: ID 1: name 2: lastname
            string[] s = nameSearch.Split(' ');
            string id = s[0];

            try
            {
                if (nameSearch == "undefined" || nameSearch == "")
                {
                    return NotFound();
                }
                //  || c.MemberId.Contains(id)) 
                var advisorContacts = await (from advisor in db.Advisors
                                             join c in db.Contacts on advisor.ContactId equals c.Id
                                             where ((c.IdNumber.Contains(id) && (advisor.AdvisorType_Id == 2)))
                                             select new AdvisorCompanyAdvisorResponse
                                             {
                                                 Id = c.Id,
                                                 Name = c.FirstName,
                                                 LastName = c.LastName,
                                                 IdNumber = c.IdNumber,                                               
                                                 Advisor_Id = advisor.Id,
                                                 AdvisorType_Id = advisor.AdvisorType_Id
                                             }).ToListAsync();
                return Ok(advisorContacts);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return NotFound();
            }
        }

        //ADVISORSID
        [Route("Advisors/AdvisorId")]
        [ResponseType(typeof(Advisor))]
        public async Task<IHttpActionResult> GetAdvisorsAdvisorID()
        {
            try
            {
                var advisors = await db.Advisors
                                   .Where(x => x.AdvisorType_Id == 2)
                                   .Select(c => new AdvisorCompanyAdvisorResponseId() { Id = c.Id }).ToListAsync();

                return Ok(advisors);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        //ADVISORS
        [Route("Advisors/Director")]
        [ResponseType(typeof(Advisor))]
        public async Task<IHttpActionResult> GetAdvisorsDirector()
        {
            try
            {
                var advisors = await db.Advisors.Where(x => x.AdvisorType_Id == 7).Select(c => c.ContactId).ToListAsync();
                var advisorContacts = await db.Contacts
                                   .Where(p => advisors.Contains(p.Id))
                                   .Select(c => new KeyCompanyAdvisorResponse()
                                   {
                                       Id = c.Id,
                                       Name = c.FirstName,
                                       LastName = c.LastName,
                                       IdNumber = c.IdNumber,
                                   }).ToListAsync();
                return Ok(advisorContacts);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }


        //ADVISORS
        [Route("Advisors/Key")]
        [ResponseType(typeof(Advisor))]
        public async Task<IHttpActionResult> GetAdvisorsKey()
        {
            try
            {
                var advisors = await db.Advisors.Where(x => x.AdvisorType_Id == 1).Select(c => c.ContactId).ToListAsync();

                var advisorContacts = await db.Contacts
                                   .Where(p => advisors.Contains(p.Id))
                                   .Select(c => new KeyCompanyAdvisorResponse()
                                   {
                                       Id = c.Id,
                                       Name = c.FirstName,
                                       LastName = c.LastName,
                                       IdNumber = c.IdNumber,
                                   }).ToListAsync();

                return Ok(advisorContacts);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        //ADVISORSID
        [Route("Advisors/KeyId")]
        [ResponseType(typeof(Advisor))]
        public async Task<IHttpActionResult> GetAdvisorsKeyId()
        {
            try
            {
                var advisors = await db.Advisors
                                   .Where(x => x.AdvisorType_Id == 1)
                                   .Select(c => new KeyCompanyAdvisorResponseId() { Id = c.Id }).ToListAsync();

                return Ok(advisors);
            }

            catch (Exception e)
            {
                throw e;
            }
        }

        //ADVISORSID
        [Route("Advisors/NotAdvisorTypeComapany")]
        [ResponseType(typeof(Advisor))]
        public async Task<IHttpActionResult> GetAdvisorsNotAdvisorTypeComapany()
        {


            try
            {
                var advisors = await db.Advisors.Where(x => x.AdvisorType_Id != 5).Select(c => c.ContactId).ToListAsync();

                var advisorContacts = await db.Contacts
                                 .Where(p => advisors.Contains(p.Id))
                                 .Select(c => new KeyCompanyAdvisorResponseId() { Id = c.Id }).ToListAsync();

                return Ok(advisorContacts);

            }

            catch (Exception e)
            {
                throw e;
            }

        }

        //ADVISORSID
        [Route("Advisors/NotAdvisorTypeComapany/IdNumber")]
        [ResponseType(typeof(Advisor))]
        public async Task<IHttpActionResult> GetAdvisorsNotAdvisorTypeComapanyDisplayIdNumber()
        {
            try
            {
                var advisors = await db.Advisors.Where(x => x.AdvisorType_Id != 5).Select(c => c.ContactId).ToListAsync();

                var advisorContacts = await db.Contacts
                                 .Where(p => advisors.Contains(p.Id))
                                 .Select(c => new KeyCompanyAdvisorResponse()
                                 {
                                     Id = c.Id,
                                     Name = c.FirstName,
                                     LastName = c.LastName,
                                     IdNumber = c.IdNumber,
                                 }).ToListAsync();
                return Ok(advisorContacts);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        //COMPANY
        [Route("Advisors/AdvisorType/Advisor")]
        public async Task<IHttpActionResult> GetAdvisorOfAdvisorTypeAdvisor()
        {
            try
            {
                var advisors = await db.Advisors.Where(x => x.AdvisorType_Id == 2 && x.Deleted != true).Select(c => c.ContactId).ToListAsync();
                var advisorContacts = await db.Contacts
                                   .Where(p => advisors.Contains(p.Id))
                                   .Select(c => new KeyCompanyAdvisorResponse()
                                   {
                                       Id = c.Id,
                                       Name = c.FirstName,
                                       LastName = c.LastName,
                                       IdNumber = c.IdNumber,
                                   }).ToListAsync();
                return Ok(advisorContacts);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // GET: api/Advisors
        [Route("Advisors/{pageIndex}/{pageSize}")]
        public AdvisorsDTO GetAdvisors(int? pageIndex = 1, int? pageSize = 10)
        {
            pageIndex = Math.Abs((int)pageIndex);
            pageSize = Math.Abs((int)pageSize);
            pageSize = pageSize > 1000 ? 1000 : pageSize;

            AdvisorsDTO advisorsDTO = new AdvisorsDTO
            {
                Count = db.Advisors.Where(c => c.Deleted != true).Count()
            };

            int totalPages = (int)Math.Ceiling((decimal)(int)advisorsDTO.Count / (int)pageSize);

            pageIndex = pageIndex > totalPages ? totalPages : pageIndex;

            advisorsDTO.Advisors = (from Advi in db.Advisors
                                    join Cont in db.Contacts on Advi.ContactId equals Cont.Id
                                    join AdviStat in db.AdvisorStatuses on Advi.AdvisorStatus_Id equals AdviStat.Id
                                    join AdviType in db.AdvisorTypes on Advi.AdvisorType_Id equals AdviType.Id
                                    select new GetAdvisorsResp
                                    {
                                        AccountNumber = Advi.AccountNumber,
                                        AccountType = Advi.AccountType,
                                        AdvisorStatus = AdviStat,
                                        AdvisorType = AdviType,
                                        Allowance = Advi.Allowance,
                                        CmsCode = Advi.CmsCode,
                                        ComplianceOffficerNumber = Advi.ComplianceOffficerNumber,
                                        Contact = Cont,
                                        CreatedDate = Advi.CreatedDate,
                                        DateAuthorized = Advi.DateAuthorized,
                                        Deleted = Advi.Deleted,
                                        EffectiveEndDate = Advi.EffectiveEndDate,
                                        EffectiveStartDate = Advi.EffectiveStartDate,
                                        FsbCode = Advi.FsbCode,
                                        Id = Advi.Id,
                                        IsActive = Advi.IsActive,
                                        IsKeyIndividual = Advi.IsKeyIndividual,
                                        LastLoginDate = Advi.LastLoginDate,
                                        ComplianceOfficer = Advi.ComplianceOfficer,
                                        ModifiedDate = Advi.ModifiedDate,
                                        RegNumber = Advi.RegNumber,
                                        TaxDirectiveRate = Advi.TaxDirectiveRate
                                    }).AsQueryable();

            return advisorsDTO;
        }

        // GET: api/Advisor/5
        [ResponseType(typeof(Advisor))]
        [Route("Advisor/{id}")]
        public async Task<IHttpActionResult> GetAdvisor(int id)
        {
            if (id == 0)
            {
                id = _user.AdvisorId;
            }

            if (!this.AdvisorExists(id))
            {
                return null;
            }

            try
            {
                var advisor = await db.Advisors.Where(p => p.Id == id && p.Deleted != true && p.AdvisorType_Id != null).FirstOrDefaultAsync();

                var contact = await db.Contacts.Where(c => c.Id == advisor.ContactId).FirstOrDefaultAsync();

                var address = await db.Addresses.Where(a => a.Contact_Id == contact.Id).FirstOrDefaultAsync();

                var docuents = await db.AdvisorDocuments.Where(x => x.Advisor_Id == advisor.Id && x.Deleted != true)
                    .Select(d => new AdvisorDocumentResponse()
                    {
                        Id = d.Id,
                        DocumentType = db.DocumentTypes.FirstOrDefault(x => x.Id == d.DocumentTypeId).Name,
                        DocumentType_Id = d.DocumentTypeId,
                        ValidFromDate = d.ValidFromDate,
                        ValidToDate = d.ValidToDate,
                        Title = d.Title,
                        Deleted = d.Deleted
                    }).OrderBy(a => a.DocumentType).ToListAsync();



                var advisorResponse = new AdvisorResponse()
                {
                    Id = advisor.Id,
                    AdvisorType_Id = advisor.AdvisorType_Id ?? 0,
                    FsbCode = advisor.FsbCode,
                    RegNumber = advisor.RegNumber,
                    DateAuthorized = advisor.DateAuthorized,
                    ComplianceOfficer = advisor.ComplianceOfficer,
                    ComplianceOfficerNumber = advisor.ComplianceOffficerNumber,
                    AddressType = db.AddressTypes.FirstOrDefault(x => x.Id == address.AddressType_Id).Name,
                    AddressTyper_Id = address.AddressType_Id ?? 0,
                    AdvisorDocuments = docuents,
                    AdvisorType = db.AdvisorTypes.FirstOrDefault(x => x.Id == advisor.AdvisorType_Id.Value).Title,
                    Cell1 = contact.Cell1,
                    Cell2 = contact.Cell2,
                    City = address.City,
                    Country = address.Country_Id.HasValue ? db.Countries.FirstOrDefault(x => x.Id == address.Country_Id).Name : "NA",
                    Country_id = address.Country_Id ?? 0,
                    AddressDescription = address.Description,
                    ContactId = contact.Id,
                    Email = contact.Email,
                    Email2 = contact.Email2,
                    FirstName = contact.FirstName,
                    LastName = contact.LastName,
                    MapUrl = address.MapUrl,
                    PostalCode = address.PostalCode,
                    Province_Id = address.Province_Id ?? 0,
                    Province = address.Province_Id.HasValue ? db.Provinces.FirstOrDefault(x => x.Id == address.Province_Id.Value).Name : "NA",
                    Street1 = address.Street1,
                    Street2 = address.Street2,
                    Street3 = address.Street3,
                    Suburb = address.Suburb,
                    Tel1 = contact.Tel1,
                    Tel2 = contact.Tel2,
                    EffectiveStartDate = advisor.EffectiveStartDate,
                    EffectiveEndDate = advisor.EffectiveEndDate
                };

                var check = advisorResponse;

                return Ok(advisorResponse);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        [AcceptVerbs("GET")]
        [Route("Advisor/User/{contactId}")]
        public async Task<IHttpActionResult> GetAdvisorUser(int contactId)
        {
            var user = await db.Advisors.Where(x => x.ContactId == contactId).Select(x => x.User).FirstOrDefaultAsync();

            if (user != null)
                return Ok(user);

            return InternalServerError();
        }

        // GET: api/Advisor/5
        [ResponseType(typeof(Advisor))]
        [Route("AdvisorProfile/{id}")]
        public async Task<IHttpActionResult> GetAdvisorProfile(int id)
        {
            try
            {
                if (id == 0)
                {
                    id = _user.AdvisorId;
                }

                if (!this.AdvisorExists(id))
                {
                    return null;
                }
                var advisor = await db.Advisors.Where(p => p.Id == id && p.Deleted != true && p.AdvisorType_Id != null).FirstOrDefaultAsync();
                var contact = await db.Contacts.Where(c => c.Id == advisor.ContactId).FirstOrDefaultAsync();

                var advisorProfileResponse = new ProfileDetailsResponse()
                {
                    Id = advisor.Id,
                    Cell1 = contact.Cell1,
                    Cell2 = contact.Cell2,
                    ContactId = contact.Id,
                    Email = contact.Email,
                    Email2 = contact.Email2,
                    FirstName = contact.FirstName,
                    LastName = contact.LastName,
                    Tel1 = contact.Tel1,
                    Tel2 = contact.Tel2,
                    IdNumber = contact.IdNumber,
                    ContactTitle_Id = contact.ContactTitle_Id ?? 1,
                    ContactTitle_Name = contact.ContactTitle_Id.HasValue ? db.ContactTitles.FirstOrDefault(x => x.Id == contact.ContactTitle_Id).Name : "NA"
                };
                return Ok(advisorProfileResponse);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // GET: api/Application/5/Documents
        [Route("Advisor/Documents/{id}")]
        //[ResponseType(typeof(IQueryable<AdvisorDocument>))]
        public async Task<IHttpActionResult> GetAdvisorDocuments(int id)
        {
            try
            {
                var documents = await db.AdvisorDocuments.Where(x => x.Advisor_Id == id)
                                        .Select(x => new AdvisorDocumentsResponse()
                                        {
                                            DocumentId = x.Id,
                                            DocumentTypeId = x.DocumentTypeId,
                                            DocumentTitle = x.Title,
                                            Uploaded = x.Uploaded,
                                            DocumentTypeName = x.DocumentType.Name,
                                            DocumenTypeLocation = x.Location,
                                            ValidFromDate = x.ValidFromDate.Value,
                                            ValidToDate = x.ValidToDate.Value,
                                            Expired = (x.ValidToDate.HasValue && x.ValidToDate.Value < DateTime.Today),
                                            AdvisorId = x.Advisor_Id,
                                            Deleted = x.Deleted,
                                        })
                                        .Where(e => e.Deleted == false)
                                        .OrderBy(a => a.DocumentTypeName).ToListAsync();

                return Ok(documents);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }





        // GET: api/Advisor/AdvisorLicenseTypes/5
        [AcceptVerbs("PUT")]
        [Route("Advisor/PutAdvisorStatus/{advisorId}/{advisorStatus}")]
        public async Task<IHttpActionResult> PutAdvisorStatus(int advisorId, int advisorStatus)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                Advisor adv = db.Advisors.Find(advisorId);
                AdvisorStatus advStat = db.AdvisorStatuses.Find(advisorStatus);
                if (adv != null)
                {

                    adv.AdvisorStatus = advStat;


                    db.Entry(adv).CurrentValues.SetValues(adv);

                }
            }
            catch (Exception e)
            {
                throw e;
            }
            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;

            }

            return StatusCode(HttpStatusCode.NoContent);


        }


        // GET: api/Advisor/5/Commission/1/1/20160601/20160701
        [Route("Advisor/{id}/Commission/{pageIndex}/{pageSize}/{dateFrom}/{dateTo}")]
        //public async Task<IHttpActionResult> GetAdvisorDocuments(int id)
        public async Task<IHttpActionResult> GetAdvisorCommissionList(int id, int? pageIndex = null, int? pageSize = null, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            if (id <= 0)
            {
                var _user = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());

                if (_user != null && !User.IsInRole("Admin"))
                {
                    id = _user.AdvisorId;
                    if (!this.AdvisorExists(id))
                    {
                        return null;
                    }
                }
            }

            pageIndex = Math.Abs((int)pageIndex);
            pageSize = Math.Abs((int)pageSize);
            pageSize = pageSize > 1000 ? 1000 : pageSize;
            CommissionDTO commissionDTO = new CommissionDTO();

            try
            {
                var applicationHistory = await db.ApplicationAdvisorHistory
                    .Where(e => e.DateStarted <= dateTo && ((e.DateEnded != null) ? e.DateEnded >= dateTo : true))
                    .OrderBy(e => e.Application_Id)
                    .GroupBy(e => e.Application_Id)
                    .ToListAsync();

                var appsHist = new List<ApplicationAdvisorHistory>();

                foreach (var item in applicationHistory)
                {
                    appsHist.Add(item.Last());
                }
                
                var adviHist = (from x in appsHist
                                join y in db.Applications on x.Application_Id equals y.Id
                                join o in db.CommissionStatement on y.Client_Id equals o.ClientId
                                join w in db.Products on o.SupplierId equals w.SupplierId
                                join z in db.Suppliers on w.SupplierId equals z.Id
                                select new
                                {
                                    prodId = y.Id,
                                    ClientId = y.Client_Id,
                                    SupId = z.Id
                                })
                                .OrderBy(e => e.ClientId)
                                .Distinct()
                                .AsEnumerable();

                var suppliersShares = (from x in db.Suppliers
                                       join y in db.AdvisorShareUnderSupervisions.Where(e => e.AdvisorId == id)
                                       on x.Name equals y.supplier
                                       select new
                                       {
                                           share = (decimal?)(int?)y.Share,
                                           SupId = x.Id,
                                           AdivId = y.AdvisorId
                                       })
                                       .Where(e => e.share != null)
                                       .Distinct()
                                       .AsEnumerable();

                var comms = db.CommissionStatement.Where(e =>
                                    e.AdvisorId == id &&
                                   e.ApprovalStatus == "APPROVED" &&
                                   e.ApprovalDateFrom >= dateFrom &&
                                   e.ApprovalDateTo <= dateTo)
                                   .OrderBy(e=> e.ClientId)
                                   .AsEnumerable();

                var commissions = (from c in comms
                                   join s in db.Suppliers on c.SupplierId equals s.Id
                                   join h in adviHist on c.ClientId equals h.ClientId
                                   join d in suppliersShares on c.SupplierId equals d.SupId
                                   join app in db.Applications.Where(e => e.Advisor_Id == id) on c.ClientId equals app.Client_Id
                                   join Apptype in db.ApplicationTypes on app.ApplicationType_Id equals Apptype.Id
                                   join Client in db.Contacts on c.ClientId equals Client.Id
                                   where (c.ProductId == app.Product_Id && c.ClientId == Client.Id && c.AdvisorId == id && c.SupplierId == s.Id)
                                   select new 
                                   {
                                       Surname = c.Surname + "(" + c.MemberSearchValue + ")",
                                       c.Id,
                                       c.Initial,
                                       c.MemberNumber,
                                       c.MemberSearchKey,
                                       c.TerminationDate,
                                       c.TransactionDate,
                                       AdvisorCommission = (c.CommissionExclVAT * (d.share / 100)),
                                       CompanyCommission = (c.CommissionExclVAT * (1.00M - ((d.share) / 100))),
                                       c.CommissionExclVAT,
                                       c.CommissionInclVAT,
                                       AdvisorTax = (c.AdvisorTax),
                                       AdvisorTaxRate = (c.AdvisorTaxRate),
                                       c.AdvisorId,
                                       c.ClientId,
                                       c.ProductId,
                                       supplierName = s.Name,
                                       SupplierId = s.Id,

                                   })
                                .OrderBy(c => c.TransactionDate)
                                .Distinct()
                                .ToList();

                var AdviComm = commissions.Sum(e => e.AdvisorCommission);

                return Ok(commissions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }


        // GET: api/Advisor/5/CommissionReport/20160601/20160701
        [Route("Advisor/{id}/CommissionReport/{dateFrom}/{dateTo}")]
        public async Task<HttpResponseMessage> CommissionReport(int id, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            try
            {

                string userId = "unknown";
                var _user = await HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>()
                    .FindByIdAsync(HttpContext.Current.User.Identity.GetUserId());

                if (_user != null)
                {
                    userId = _user.Id;
                }

                string fileName = (
                    workingFolder + "\\"
                    + userId + "\\"
                    + (DateTime.Now.Ticks.ToString()).Replace(@"/", "-")
                    + "CommissionExport"
                    );

                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }

                if (File.Exists(fileName))
                {
                    Console.WriteLine(fileName + " already exists!");
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                FileStream fs = new FileStream(fileName.Replace(@"\\", @"\"), FileMode.CreateNew);

                var applicationHistory = db.ApplicationAdvisorHistory.Where(
                                e => e.DateStarted <= dateTo &&
                               ((e.DateEnded != null) ? e.DateEnded >= dateTo : true))
                               .OrderBy(e => e.DateStarted)
                               .GroupBy(e => e.Application_Id)
                               .ToList().Distinct();


                var appsHist = new List<ApplicationAdvisorHistory>();

                foreach (var item in applicationHistory)
                {
                    appsHist.Add(item.Last());
                }


                var adviHist = (from x in appsHist
                                join y in db.Applications on x.Application_Id equals y.Id
                                join o in db.CommissionStatement on y.Client_Id equals o.ClientId
                                join w in db.Products on o.SupplierId equals w.SupplierId
                                join z in db.Suppliers on w.SupplierId equals z.Id
                                select new
                                {
                                    prodId = y.Id,
                                    ClientId = y.Client_Id,
                                    SupId = z.Id
                                })
                                .Distinct()
                                .AsEnumerable();


                var suppliersShares = (from x in db.Suppliers
                                       join y in db.AdvisorShareUnderSupervisions.Where(e => e.AdvisorId == id)
                                       on x.Name equals y.supplier
                                       select new
                                       {
                                           share = (decimal?)(int?)y.Share,
                                           SupId = x.Id,
                                           AdivId = y.AdvisorId
                                       })
                                       .Where(e => e.share != null)
                                       .Distinct()
                                       .GroupBy(e => e.AdivId)
                                       .AsEnumerable();

                var comms = db.CommissionStatement.Where(e =>
                                    e.AdvisorId == id &&
                                   e.ApprovalStatus == "APPROVED" &&
                                   e.ApprovalDateFrom >= dateFrom &&
                                   e.ApprovalDateTo <= dateTo).AsEnumerable();

                var commsFiltered = (from c in comms
                               select new
                               {
                                   c.Id,
                                   c.Surname,
                                   c.AdvisorId,
                                   c.MemberNumber,
                                   c.MemberSearchKey,
                                   c.TransactionDate,
                                   c.SupplierId,
                                   c.Initial,
                                   c.CommissionExclVAT,
                                   c.CommissionInclVAT,
                                   c.AdvisorTax,
                                   c.AdvisorTaxRate,
                                   c.ClientId,
                                   c.ProductId,
                                   AdvisorVat = (c.AdvisorTax),//<--
                                   Date = c.TransactionDate,
                               }
               )
               .OrderBy(v => v.Surname)
               .AsEnumerable()
               .Distinct();


                var Comission = (from c in commsFiltered
                                 join s in db.Suppliers on c.SupplierId equals s.Id
                                 join h in adviHist on c.ClientId equals h.ClientId
                                 join d in suppliersShares on c.AdvisorId equals d.Key
                                 join a in db.Advisors on c.AdvisorId equals a.Id
                                 join cont in db.Contacts on a.ContactId equals cont.Id
                                 join app in db.Applications.Where(e => e.Advisor_Id == id) on c.ClientId equals app.Client_Id
                                 join Apptype in db.ApplicationTypes on app.ApplicationType_Id equals Apptype.Id
                                 //where (c.ProductId == app.Product_Id && c.ClientId == cont.Id && c.AdvisorId == id && c.SupplierId == s.Id)
                                 select new 
                                 {
                                     c.Surname,
                                     c.Initial,
                                     c.MemberNumber,
                                     c.MemberSearchKey,
                                     c.TransactionDate,
                                     AdvisorCommission = (c.CommissionExclVAT * (d.FirstOrDefault(e => e.AdivId == c.AdvisorId)?.share / 100)),
                                     CompanyCommission = (c.CommissionExclVAT * (1.00M - ((d.FirstOrDefault(e => e.AdivId == c.AdvisorId).share) / 100))),
                                     c.CommissionExclVAT,
                                     c.CommissionInclVAT,
                                     AdvisorTax = (c.AdvisorTax),
                                     AdvisorTaxRate = (c.AdvisorTaxRate),
                                     c.AdvisorId,
                                     c.ClientId,
                                     c.ProductId,
                                     supplierName = s.Name,
                                     AdvisorLastname = cont.LastName,
                                     AdvisorName = cont.FirstName,
                                     c.Id,
                                     SupplierId = s.Id,
                                 })
                                 .OrderBy(c => c.TransactionDate)
                                 .OrderBy(c => c.Surname)
                                 .Distinct()
                                 .ToList();


                try
                {
                    var commReportResp = new CommReportResp()
                    {
                        AdvisorCommision = Comission.Sum(e => e.AdvisorCommission),
                        AdvisorLastname = Comission.FirstOrDefault().AdvisorLastname,
                        AdvisorName = Comission.FirstOrDefault().AdvisorName,
                        Date = Comission.FirstOrDefault().TransactionDate
                    };

                    string specifier = "F";
                    CultureInfo culture = CultureInfo.CreateSpecificCulture("en-CA");

                    string csv = $"ADVISER,DATE,COMMISSION{Environment.NewLine}{string.Concat($"{commReportResp.AdvisorName} {commReportResp.AdvisorLastname},{String.Format("{0:dd-MMM-yyyy}", commReportResp.Date)},{commReportResp.AdvisorCommision.Value.ToString(specifier,culture)}{Environment.NewLine}")}";


                    var tempFile = new System.Text.UTF8Encoding().GetBytes(csv);

                    await fs.WriteAsync(tempFile, 0, tempFile.Length);
                    fs.Close();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                MemoryStream responseStream = new MemoryStream();
                Stream fileStream = File.Open(fileName, FileMode.Open);
                bool fullContent = true;
                if (this.Request.Headers.Range != null)
                {
                    fullContent = false;

                    // Currently we only support a single range.
                    RangeItemHeaderValue range = this.Request.Headers.Range.Ranges.First();

                    // From specified, so seek to the requested position.
                    if (range.From != null)
                    {
                        fileStream.Seek(range.From.Value, SeekOrigin.Begin);

                        // In this case, actually the complete file will be returned.
                        if (range.From == 0 && (range.To == null || range.To >= fileStream.Length))
                        {
                            fileStream.CopyTo(responseStream);
                            fullContent = true;
                        }
                    }
                    if (range.To != null)
                    {
                        // 10-20, return the range.
                        if (range.From != null)
                        {
                            long? rangeLength = range.To - range.From;
                            int length = (int)Math.Min(rangeLength.Value, fileStream.Length - range.From.Value);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                        // -20, return the bytes from beginning to the specified value.
                        else
                        {
                            int length = (int)Math.Min(range.To.Value, fileStream.Length);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                    }
                    // No Range.To
                    else
                    {
                        // 10-, return from the specified value to the end of file.
                        if (range.From != null)
                        {
                            if (range.From < fileStream.Length)
                            {
                                int length = (int)(fileStream.Length - range.From.Value);
                                byte[] buffer = new byte[length];
                                fileStream.Read(buffer, 0, length);
                                responseStream.Write(buffer, 0, length);
                            }
                        }
                    }
                }
                // No Range header. Return the complete file.
                else
                {
                    fileStream.CopyTo(responseStream);
                }
                fileStream.Close();
                responseStream.Position = 0;

                HttpResponseMessage response = new HttpResponseMessage
                {
                    StatusCode = fullContent ? HttpStatusCode.OK : HttpStatusCode.PartialContent,
                    Content = new StreamContent(responseStream)
                };
                return response;
            }
            catch 
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

        }

        // GET: api/Advisor/LicenseCategories/5
        [Route("Advisor/LicenseCategories/{advisorTypeId}")]
        public async Task<IHttpActionResult> GetLicenseCategories(int? advisorTypeId = null)
        {
            var licenseCategories = new List<LicenseCategory>();

            if (advisorTypeId == null)
            {
                try
                {
                    licenseCategories = await db.LicenseCategories.ToListAsync();
                }
                catch
                {
                    return InternalServerError();
                }

                if (licenseCategories == null)
                {
                    return NotFound();
                }

                return Ok(licenseCategories);
            }

            return Ok(licenseCategories);
        }

        // GET: api/Advisor/LicenseTypes/5
        [Route("Advisor/LicenseTypes/{licenseCategoryId}")]
        public async Task<IHttpActionResult> GetLicenseTypes(int? licenseCategoryId = null)
        {
            var licenseTypes = new List<LicenseType>();

            try
            {
                if (licenseCategoryId == null)
                {
                    licenseTypes = await db.LicenseTypes.ToListAsync();
                }
                else
                {
                    licenseTypes = await db.LicenseTypes.Where(l => l.LicenseCategoryId == (int)licenseCategoryId).ToListAsync();
                }
            }
            catch
            {
                return InternalServerError();
            }

            if (licenseTypes == null)
            {
                return NotFound();
            }

            return Ok(licenseTypes);
        }
        // GET: api/Advisor/AdvisorLicenseTypes/5
        [Route("Advisor/AdvisorLicenseTypes/{advisorId}")]
        public async Task<IHttpActionResult> GetAdvisorLicenseTypes(int? advisorId = null)
        {
            var result = new List<AdvisorShareDTO>();

            try
            {
                List<Advisor2LicenseTypes> ltypes = await db.AdvisorLicenseTypes
                    .Where(l => l.AdvisorId == advisorId)
                    .ToListAsync();

                foreach (var lt in ltypes)
                {
                    LicenseType license = db.LicenseTypes.Find(lt.LicenseTypeId);
                    result.Add(
                        new AdvisorShareDTO {
                            AdvisorId = lt.AdvisorId,
                            LicenseTypeName = license.Description,
                            Share = lt.Share,
                            validCommissionFromDate = lt.validCommissionFromDate,
                            validCommissionToDate = lt.validCommissionToDate,
                            LicenseTypeId = lt.LicenseTypeId,
                            UnderSupervision = lt.underSupervision,
                            Advisor = lt.Advisor,
                            ValidFromDate = lt.ValidFromDate,
                            ValidToDate = lt.ValidToDate
                        }
                    );
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        // GET: api/Advisor/AdvisorLicenseProducts/5/1
        [Route("Advisor/AdvisorLicensedProducts/{advisorId}/{licenseId}")]
        public async Task<IHttpActionResult> GetAdvisorLicensedProducts(int? advisorId = null, int? licenseId = null)
        {
            var result = new List<Product>();

            try
            {
                Advisor2LicenseTypes ltype = await db.AdvisorLicenseTypes
                    .FirstOrDefaultAsync(l => l.AdvisorId == advisorId && l.LicenseTypeId == licenseId);
                LicenseType lt = await db.LicenseTypes.Include("Products")
                    .FirstOrDefaultAsync(l => l.Id == ltype.LicenseTypeId);
                result = lt.Products.ToList();

                return Ok(result);
            }
            catch
            {
                return InternalServerError();
            }
        }

        // : api/Advisor/1/CommissionStatement/1      
        [AcceptVerbs("GET", "POST")]
        [Route("Advisor/{advisorId}/CommissionStatement/{CommisionStatementId}")]
        public CommissionStatement AdvisorToCommision(int? advisorId = null, int? CommisionStatementId = null)
        {
            CommissionStatement commToUpdate;
            if (advisorId > 0)
            {
                try
                {
                    commToUpdate = db.CommissionStatement.Where(c => c.Id == CommisionStatementId).FirstOrDefault();
                    if (commToUpdate != null)
                    {
                        commToUpdate.AdvisorId = advisorId;
                        db.Entry(commToUpdate).CurrentValues.SetValues(commToUpdate);
                        db.SaveChanges();
                    }
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            return commToUpdate;

        }

        [Route("Advisor/AdvisorAllowance/Put/{advisorId}/{allowance}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAdvisorAllowance(int advisorId, double allowance)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Advisor at = db.Advisors.Find(advisorId);
                if (at != null)
                {
                    at.Allowance = allowance;
                    db.Entry(at).CurrentValues.SetValues(at);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("Advisor/AdvisorLicenseTypes/Put/{advisorId}/{licenseId}/{supplierName}/{productName}/{share}/{validCommissionFromDate}/{validCommissionToDate}/{underSupervision}/{advisorSupervisionID}/{fromSupervision}/{toSupervision}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAdvisorShare(int advisorId, int licenseId, string supplierName, string productName, double share, DateTime? validCommissionFromDate, DateTime? validCommissionToDate, bool underSupervision, int advisorSupervisionID, DateTime? fromSupervision, DateTime? toSupervision)
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

                AdvisorShareUnderSupervision at = db.AdvisorShareUnderSupervisions.Find(advisorId, licenseId, supplierName, productName);

                if (at == null)
                {

                }
                if (at != null)
                {
                    if (!(share >= 0 && share < 100))
                    {
                        return BadRequest();
                    }

                    at.Share = share;
                    at.supplier = supplierName;
                    at.product = productName;
                    at.underSupervision = underSupervision;
                    at.Advisor = advisorSupervisionID;
                    at.ValidFromDate = fromSupervision;
                    at.ValidToDate = toSupervision;
                    at.validCommissionFromDate = validCommissionFromDate;
                    at.validCommissionToDate = validCommissionToDate;
                    db.Entry(at).CurrentValues.SetValues(at);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("Advisor/AdvisorSupplierCode/Put/{advisorId}/{supplierId}")]
        // PUT: api/AdvisorSupplierCodes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAdvisorSupplierCode(int advisorId, int supplierId, AdvisorSupplierCode advisorSupplierCode)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AdvisorSupplierCode at = db.AdvisorSupplierCodes.Find(advisorId, supplierId);

            if (at == null)
            {
                //at.AdvisorId = advisorId;
                //at.SupplierId = supplierId;
                //at.AdvisorCode = Code;
                //db.Entry(at).CurrentValues.SetValues(at);

            }
            db.Entry(advisorSupplierCode).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //  if (!AdvisorSupplierCodeExists(at))
                //{
                //   return NotFound();
                // }
                //else
                //{
                throw;
                //}
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("Advisor/AdvisorSupplierCode/Post/{advisorId}/{supplierId}")]
        [ResponseType(typeof(void))]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostSendAdviserMemberCodeOne(int advisorId, int supplierId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                AdvisorSupplierCode at = db.AdvisorSupplierCodes.Find(advisorId, supplierId);
                if (at == null)
                {
                    at.AdvisorId = advisorId;
                    at.SupplierId = supplierId;
                    //at.AdvisorCode = Code;
                    db.Entry(at).CurrentValues.SetValues(at);
                }
                else
                {
                    //at.AdvisorCode = Code;
                    db.Entry(at).CurrentValues.SetValues(at);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/AdvisorSupplierCodes
        [AcceptVerbs("POST")]
        [Route("Advisor/AdvisorSupplierCode/")]
        [ResponseType(typeof(AdvisorSupplierCode))]
        public async Task<IHttpActionResult> PostSendAdviserMemberCode(AdvisorSupplierCode advisorSupplierCode)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AdvisorSupplierCodes.Add(advisorSupplierCode);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return Ok(advisorSupplierCode);
        }

        private bool AdvisorSupplierCodeExists(int id)
        {
            return db.AdvisorSupplierCodes.Count(e => e.SupplierId == id) > 0;
        }

        // PUT: api/Advisor/5
        [Route("Advisor/{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAdvisor(int id, AdvisorResponse advisor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if (id != advisor.Id)
            {
                return BadRequest();
            }
            
            advisor.AdvisorDocuments = null;

            var existingAdvisor = db.Advisors.Find(advisor.Id);
            
            if (existingAdvisor != null)
            {
                existingAdvisor.ComplianceOffficerNumber = advisor.ComplianceOfficerNumber;
                db.Entry(existingAdvisor).CurrentValues.SetValues(advisor);
            };

            if (advisor.AdvisorType != null)
            {
                var AddType = await db.AdvisorTypes.FindAsync(advisor.AdvisorType_Id);
                if (AddType != null)
                {
                    existingAdvisor.AdvisorType = AddType;
                }
            }

            var contact = db.Contacts.Find(advisor.ContactId);
            if (contact != null)
            {

                var originalAddresses = db.Addresses.Include(a => a.AddressType)
                    .Where(c => c.Contact.Id == advisor.ContactId);
                var AddressType = await db.AddressTypes.FindAsync(advisor.AddressTyper_Id);
                var Province = await db.Provinces.FindAsync(advisor.Province_Id);
                var Country = await db.Countries.FindAsync(advisor.Country_id);

                foreach (Address originalAddress in originalAddresses)
                {
                    originalAddress.AddressType = AddressType;
                    originalAddress.Description = advisor.AddressDescription;
                    originalAddress.Street1 = advisor.Street1;
                    originalAddress.Street2 = advisor.Street2;
                    originalAddress.Street3 = advisor.Street3;
                    originalAddress.Suburb = advisor.Suburb;
                    originalAddress.PostalCode = advisor.PostalCode;
                    originalAddress.City = advisor.City;
                    originalAddress.Province = Province;
                    originalAddress.Country = Country;
                    originalAddress.MapUrl = advisor.MapUrl;
                }

                contact.FirstName = advisor.FirstName;
                contact.LastName = advisor.LastName;
                contact.Email = advisor.Email;
                contact.Email2 = advisor.Email2;
                contact.Tel1 = advisor.Tel1;
                contact.Tel2 = advisor.Tel2;
                contact.Cell1 = advisor.Cell1;
                contact.Cell2 = advisor.Cell2;
            }
            else
            {
                db.Entry(contact).State = EntityState.Added;
            }

            //Add advisors should have users created on the system
            if (existingAdvisor.User != null)
            {
                existingAdvisor.User.AdvisorId = advisor.Id;
                existingAdvisor.User.Email = advisor.Email;
                await UpdateCreateUser(existingAdvisor.User);
            }
            
            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (!AdvisorExists(id))
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
        
        // PUT: api/Advisor/5
        [Route("AdvisorProfile/{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAdvisorProfile(int id, ProfileDetailsResponse advisor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != advisor.Id)
            {
                return BadRequest();
            }

            var existingAdvisor = db.Advisors.Find(advisor.Id);
            
            if (existingAdvisor != null)
            {
                db.Entry(existingAdvisor).CurrentValues.SetValues(advisor);
            };

            var contact = await db.Contacts.FindAsync(advisor.ContactId);
            var contactTitle = await db.ContactTitles.FindAsync(advisor.ContactTitle_Id);
            if (contact != null)
            {
                contact.FirstName = advisor.FirstName;
                contact.LastName = advisor.LastName;
                contact.Email = advisor.Email;
                contact.Email2 = advisor.Email2;
                contact.Tel1 = advisor.Tel1;
                contact.Tel2 = advisor.Tel2;
                contact.Cell1 = advisor.Cell1;
                contact.Cell2 = advisor.Cell2;
                contact.IdNumber = advisor.IdNumber;
                contact.ContactTitle = contactTitle;
            }
            else
            {
                db.Entry(contact).State = EntityState.Added;
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!AdvisorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw ex;
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }



        // POST: api/Advisor
        [Route("Advisor")]
        [ResponseType(typeof(Advisor))]
        public async Task<IHttpActionResult> PostAdvisor(Advisor advisor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                advisor.AdvisorDocuments = null;
                advisor.Deleted = false;
                advisor.CreatedDate = DateTime.Now;
                advisor.ModifiedDate = advisor.CreatedDate;
                advisor.IsActive = true;
                advisor.Querys = null;
                if (advisor.CmsCode == null || advisor.CmsCode.Length < 3)
                {
                    advisor.CmsCode = "";
                }




                List<Address> addressList = new List<Address>();
                if (advisor.Contact.Addresses != null)
                    addressList = advisor.Contact.Addresses.ToList();

                foreach (Address address in addressList)
                {
                    //AddressType
                    if (address.AddressType != null)
                    {
                        var advContactAddressType = await db.AddressTypes.FindAsync(address.AddressType.Id);
                        if (advContactAddressType != null)
                        {
                            address.AddressType = advContactAddressType;
                        }
                    }

                    //Province
                    if (address.Province != null)
                    {
                        var advContactAddressProvince = await db.Provinces.FindAsync(address.Province.Id);
                        if (advContactAddressProvince != null)
                        {
                            address.Province = advContactAddressProvince;
                        }
                    }

                    //Country
                    if (address.Country != null)
                    {
                        var advContactAddressCountry = await db.Countries.FindAsync(address.Country.Id);
                        if (advContactAddressCountry != null)
                        {
                            address.Country = advContactAddressCountry;
                        }
                    }
                }

                //Contact
                //check if contact exists
                // || (advisor.Contact.MemberId?.Length > 0)
                if (advisor.Contact != null && (advisor.Contact.IdNumber?.Length > 0))
                {
                    //  || (c.MemberId == advisor.Contact.MemberId)
                    int found = db.Contacts.Where(c => (c.IdNumber == advisor.Contact.IdNumber)).Count();

                    if (found == 0)
                    {
                        if (advisor.Contact.ContactType != null && advisor.Contact.ContactType.Id > 0)
                        {
                            var advisorContactType = await db.ContactTypes.FindAsync(advisor.Contact.ContactType.Id);
                            if (advisorContactType != null)
                            {
                                advisor.Contact.ContactType = advisorContactType;
                            }
                        }
                        else
                        {
                            advisor.Contact.ContactType.Id = 1;
                        }

                        if (advisor.Contact.ContactTitle != null)
                        {
                            var advisorTitle = await db.ContactTitles.FindAsync(advisor.Contact.ContactTitle.Id);
                            if (advisorTitle != null)
                            {
                                advisor.Contact.ContactTitle = advisorTitle;
                            }
                        }
                        else
                        {
                            advisor.Contact.ContactTitle.Id = 1;
                        }
                        db.Entry(advisor.Contact).State = EntityState.Added;
                    }
                    else
                    {
                        // || (a.MemberId == advisor.Contact.MemberId)
                        Contact contact = await db.Contacts.Where(a => (a.IdNumber == advisor.Contact.IdNumber)).FirstAsync();
                        advisor.Contact.Id = contact.Id;
                        db.Entry(contact).CurrentValues.SetValues(advisor.Contact);
                        advisor.Contact = contact;
                    }
                }


                else
                {
                    if (advisor.Contact != null)
                    {

                        //var advisorContact = await db.Contacts.FindAsync(advisor.Contact.Id);


                        // var advContactAddress = db.Entry(advisor).Reference(a => a.Contact.Addresses).CurrentValue = advisor.Contact.Addresses;

                        //if (advisorContact != null)
                        //{

                        //Contact Type
                        if (advisor.Contact.ContactType != null && advisor.Contact.ContactType.Id > 0)
                        {
                            var advisorContactType = await db.ContactTypes.FindAsync(advisor.Contact.ContactType.Id);
                            if (advisorContactType != null)
                            {
                                advisor.Contact.ContactType = advisorContactType;
                            }
                        }
                        else
                        {
                            advisor.Contact.ContactType.Id = 1;
                        }

                        if (advisor.Contact.ContactTitle != null)
                        {
                            var advisorTitle = await db.ContactTitles.FindAsync(advisor.Contact.ContactTitle.Id);
                            if (advisorTitle != null)
                            {
                                advisor.Contact.ContactTitle = advisorTitle;
                            }

                        }
                        //else
                        //{
                        //    advisorContact.ContactTitle.Id = 1;
                        //}

                        //advisor.Contact = advisorContact;


                        // }
                        /// else
                        //{
                        // db.Entry(advisor.Contact).CurrentValues.SetValues(advisor.Contact);
                        //db.Entry(advisor.Contact).State = EntityState.Added;

                        // }
                    }

                }

                //AdvisorType
                if (advisor.AdvisorType != null)
                {
                    var advisorType = await db.AdvisorTypes.FindAsync(advisor.AdvisorType.Id);
                    advisor.AdvisorType = advisorType;
                }

                //AdvisorStatus
                if (advisor.AdvisorStatus != null && advisor.AdvisorStatus.Id > 0)
                {
                    var advisorStatus = await db.AdvisorStatuses.FindAsync(advisor.AdvisorStatus.Id);
                    advisor.AdvisorStatus = advisorStatus;
                }

                //Company
                if (advisor.Company != null && advisor.Company != null)
                {
                    var advCompany = await db.Companies.FindAsync(advisor.Company.Id);
                    advisor.Company = advCompany;
                }

                //Licenses
                List<LicenseType> licenseList = new List<LicenseType>();
                if (advisor.Licenses != null)
                {
                    licenseList = advisor.Licenses.ToList();
                    advisor.Licenses.Clear();
                    foreach (LicenseType license in licenseList)
                    {
                        var originalLicense = await db.LicenseTypes.FindAsync(license.Id);
                        advisor.Licenses.Add(originalLicense);
                    }
                }

                if (advisor.BankName != null)
                {
                    var bankBranch = await db.BankName.FindAsync(advisor.BankName.Id);
                    if (bankBranch != null)
                    {
                        advisor.BankName = bankBranch;
                    }
                }

                if (advisor.BranchCode != null)
                {
                    var bankBranchCode = await db.BankBranchCodes.FindAsync(advisor.BranchCode.Id);
                    if (bankBranchCode != null)
                    {
                        advisor.BranchCode = bankBranchCode;
                    }
                }

                //Save
                db.Entry(advisor).State = EntityState.Added;

                advisor.User = new ViewModels.User
                {
                    FirstName = advisor.Contact.FirstName,
                    LastName = "AdminUser",
                    Username = advisor.User.Username,
                    Password = advisor.User.Password,
                    IsAdmin = advisor.User.IsAdmin
                };

                await db.SaveChangesAsync();

                int id = db.Advisors.Max(i => i.Id);
                foreach (LicenseType license in licenseList)
                {
                    var originalLicense = await db.LicenseTypes.FindAsync(license.Id);
                    advisor.Licenses.Add(originalLicense);
                    db.AdvisorLicenseTypes.Add(new Advisor2LicenseTypes { AdvisorId = id, LicenseTypeId = license.Id });
                }


                //Add advisors should have users created on the system
                if (advisor.User != null)
                {
                    advisor.User.AdvisorId = advisor.Id;
                    advisor.User.Email = advisor.Contact.Email;
                    await UpdateCreateUser(advisor.User);
                }



                await db.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return Ok(advisor);
        }

        //User management
        public async Task<ViewModels.User> UpdateCreateUser(TendaAdvisors.ViewModels.User user)
        {
            // Find the sustem id for supervisor role, 
            // we need to create a helper class for this
            string adminId = "";
            string advisorId = "";
            using (var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext())))
            {
                if (rm.RoleExists("Admin") == true)
                {
                    adminId = rm.FindByName("Admin").Id;
                }

                if (rm.RoleExists("Advisor") == true)
                {
                    advisorId = rm.FindByName("Advisor").Id;
                }
            }

            var ctx = Request.GetOwinContext();
            var userManager = ctx.Get<ApplicationUserManager>();
            var rolesManager = ctx.GetUserManager<ApplicationRoleManager>();

            ApplicationUser userLogin = await userManager.FindByNameAsync(user.Username);
            ApplicationDbContext context = new ApplicationDbContext();

            if (userLogin != null)
            {
                userLogin.AdvisorId = user.AdvisorId;
                userLogin.Email = user.Email;
                userLogin.UserName = user.Username;
                userLogin.PasswordHash = userManager.PasswordHasher.HashPassword(user.Password);
                string resetToken = await userManager.GeneratePasswordResetTokenAsync(userLogin.Id);
                IdentityResult passwordChangeResult = await userManager.ResetPasswordAsync(userLogin.Id, resetToken, user.Password);
                
                //persist user changes
                await userManager.UpdateAsync(userLogin);
            }
            else
            {
                IdentityResult res = await userManager.CreateAsync(new ApplicationUser()
                {
                    AdvisorId = user.AdvisorId,
                    UserName = user.Username,
                    Email = user.Email,
                    JoinDate = DateTime.Now,
                    LockoutEnabled = false,
                    EmailConfirmed = true,
                    PasswordHash = userManager.PasswordHasher.HashPassword(user.Password)
                }, user.Password);
            }
            userLogin = await userManager.FindByNameAsync(user.Username);

            if (user.IsAdmin)
            {
                userManager.AddToRole(userLogin.Id, "Admin");
                if (userManager.IsInRole(userLogin.Id, "Advisor"))
                {
                    userManager.RemoveFromRole(userLogin.Id, "Advisor");
                }
            }
            else
            {
                //If the user is an advisor add this role
                userManager.AddToRole(userLogin.Id, "Advisor");
                if (userManager.IsInRole(userLogin.Id, "Admin"))
                {
                    userManager.RemoveFromRole(userLogin.Id, "Admin");
                }
            }

            return user;
        }

        // DELETE: api/Advisor/5
        [AllowAnonymous]
        [HttpDelete]
        [Route("Advisor/{id}")]
        [ResponseType(typeof(Advisor))]
        public async Task<IHttpActionResult> DeleteAdvisor(int id)
        {
            Advisor advisor = await db.Advisors.FindAsync(id);
            if (advisor == null)
            {
                return NotFound();
            }

            advisor.Deleted = true;
            db.Entry(advisor).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return Ok(advisor);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AdvisorExists(int id)
        {
            return db.Advisors.Count(e => e.Id == id) > 0;
        }

        // GET: api/Advisor/5
        [ResponseType(typeof(Advisor))]
        [Route("Advisor/Details/{id}")]
        public async Task<IHttpActionResult> GetAdvisorDetails(int id)
        {
            try
            {
                var advisor = await db.Advisors.Where(a => a.Id == id)
                                      .Select(b => new
                                      {
                                          b.Id,
                                          b.AccountType,
                                          b.AccountNumber,
                                          b.Allowance,
                                          b.TaxDirectiveRate,
                                          b.AdvisorStatus_Id,
                                          b.AdvisorType_Id,
                                          b.BankName_Id,
                                          b.BranchCode_Id,
                                          b.ContactId,
                                          b.ContactTitle_Id,
                                          b.User,
                                          b.EffectiveStartDate,
                                          b.EffectiveEndDate,
                                          b.CmsCode
                                      }).FirstOrDefaultAsync();
                var applications = await db.Applications.Where(c => c.Advisor_Id == id)
                                           .Select(d => new
                                           {
                                               d.Client.FirstName,
                                               d.Client.LastName,
                                               d.Client.IdNumber,
                                               d.ApplicationNumber, // member number 
                                           }).ToListAsync();
                var bank = await db.BankName.Where(c => c.Id == advisor.BankName_Id.Value)
                                       .Select(e => new { e.Id, e.Name }).FirstOrDefaultAsync();
                var branchCode = await db.BankBranchCodes.Where(c => c.Id == advisor.BranchCode_Id)
                                         .Select(f => new { f.Id, f.Name }).FirstOrDefaultAsync();
                var contact = await db.Contacts.Where(g => g.Id == advisor.ContactId)
                                      .Select(h => new
                                      {
                                          h.photoUrl,
                                          h.FirstName,
                                          h.LastName,
                                          h.JobTitle,
                                          h.Cell1,
                                          h.Cell2,
                                          h.Tel1,
                                          h.Tel2,
                                          h.Email,
                                          h.Email2,
                                          h.IdNumber,
                                      }).FirstOrDefaultAsync();
                var address = await db.Addresses.Where(a => a.Contact_Id == advisor.ContactId)
                                        .Select(b => new
                                        {
                                            b.AddressType_Id,
                                            b.Country_Id,
                                            b.Description,
                                            b.Street1,
                                            b.Street2,
                                            b.Street3,
                                            b.Suburb,
                                            b.PostalCode,
                                            b.City,
                                            b.MapUrl,
                                            b.Province_Id
                                        }).FirstOrDefaultAsync();

                var addressType = await db.AddressTypes.Where(x => x.Id == address.AddressType_Id).FirstOrDefaultAsync();
                var province = await db.Provinces.Where(x => x.Id == address.Province_Id).FirstOrDefaultAsync();
                var country = await db.Countries.Where(x => x.Id == address.Country_Id).FirstOrDefaultAsync();

                var contactTitle = await db.ContactTitles.Where(c => c.Id == advisor.ContactTitle_Id)
                                           .Select(x => new { x.Id, x.Name }).FirstOrDefaultAsync();
                var user = await db.Users.Where(g => g.AdvisorId == id)
                                   .Select(h => new
                                   {
                                       h.UserName,
                                       h.PasswordHash
                                   }).FirstOrDefaultAsync();
                var company = await db.Companies.Where(c => c.ContactDetails_Id == advisor.ContactId)
                                      .Select(x => new
                                      {
                                          x.Name,
                                          x.ImageUrl,
                                          x.VatNumber
                                      }).FirstOrDefaultAsync();
                var role = "Advisor";
                using (var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext())))
                {
                    if (rm.RoleExists("Admin") == true)
                    {
                        role = rm.FindByName("Admin").Id;

                    }

                    if (rm.RoleExists("Advisor") == true)
                    {
                        role = rm.FindByName("Advisor").Id;

                    }
                }
                var advisorStatus = await db.AdvisorStatuses.Where(i => i.Id == advisor.AdvisorStatus_Id)
                                            .Select(j => new { j.Id, j.Name }).FirstOrDefaultAsync();
                var advisorType = await db.AdvisorTypes.Where(i => i.Id == advisor.AdvisorType_Id)
                                            .Select(j => new { j.Id, j.Title }).FirstOrDefaultAsync();


                AdvisorDetailResponse response = new AdvisorDetailResponse()
                {
                    AdvisorId = advisor.Id,
                    AccountType = advisor.AccountType ?? "",
                    AccountNumber = advisor.AccountNumber ?? "",
                    Allowance = advisor.Allowance,
                    TaxDirectiveRate = advisor.TaxDirectiveRate,
                    EffectiveStartDate = advisor.EffectiveStartDate,
                    EffectiveEndDate = advisor.EffectiveEndDate,
                    //ApplicationsIds

                    AdvisorStatusName = advisorStatus.Name ?? "",
                    AdvisorTypeId = advisor.AdvisorType_Id ?? 0,
                    AdvisorTypeTitle = advisorType.Title ?? "",

                    BankName = bank?.Name ?? "",
                    BranchCodeName = branchCode?.Name ?? "",

                    CompanyName = company?.Name ?? "",
                    CompanyImageUrl = company?.ImageUrl ?? "",
                    CompanyVatNumber = company?.VatNumber ?? "",

                    PhotoUrl = contact.photoUrl ?? "",
                    FirstName = contact.FirstName ?? "",
                    LastName = contact.LastName ?? "",
                    JobTitle = contact.JobTitle ?? "",
                    Cell1 = contact.Cell1 ?? "",
                    Cell2 = contact.Cell2 ?? "",
                    Tel1 = contact.Tel1 ?? "",
                    Tel2 = contact.Tel2 ?? "",
                    Email = contact.Email ?? "",
                    Email2 = contact.Email2 ?? "",
                    ContactTitleName = contactTitle?.Name ?? "",
                    IdNumber = contact.IdNumber ?? "",

                    AddressType_name = addressType.Name ?? "", 
                    AddressDescription = address.Description ?? "",
                    Street1 = address.Street1 ?? "",
                    Street2 = address.Street2 ?? "",
                    Street3 = address.Street3 ?? "",
                    Suburb = address.Suburb ?? "",
                    PostalCode = address.PostalCode ?? "",
                    City = address.City ?? "",
                    Province_name  = province.Name  ?? "",
                    Country_name = country.Name ?? "",
                    MapUrl = address.MapUrl ?? "",


                    Username = advisor.User.Username ?? "",
                    Password = advisor.User.Password ?? "",
                    IsAdmin = advisor.User.IsAdmin,
                    CmsCode = advisor.CmsCode
                };

                return Ok(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        [ResponseType(typeof(Advisor))]
        [Route("Advisors/Licenses/{id}")]
        public async Task<IHttpActionResult> GetAdvisorLicenses(int id)
        {
            try
            {
                var licenses = await db.AdvisorShareUnderSupervisions
                    .Where(x => x.AdvisorId == id)
                    .Select(x => new
                    {
                        x.AdvisorId,
                        x.product,
                        x.Share,
                        x.supplier,
                        x.validCommissionFromDate,
                        x.validCommissionToDate,
                        x.ValidFromDate,
                        x.ValidToDate,
                        x.Advisor,
                        x.LicenseTypeId,
                        x.underSupervision
                    })
                    .OrderBy(e => e.validCommissionFromDate)
                    .ToListAsync();

                var response = new List<AdvisorLicenseResponse>();
                foreach (var license in licenses)
                {
                    var licenseType = (await db.LicenseTypes.Where(x => x.Id == license.LicenseTypeId).FirstOrDefaultAsync()).Description;
                    var supervisor = await db.Advisors.Where(x => x.Id == license.Advisor)
                                             .Select(x => x.Contact)
                                             .Select(x => new
                                             {
                                                 x.FirstName,
                                                 x.LastName
                                             }).FirstOrDefaultAsync();

                    response.Add(new AdvisorLicenseResponse()
                    {
                        AdvisorId = id,
                        SupervisorId = license.Advisor,
                        Share = license.Share,
                        LicenseTypeId = license.LicenseTypeId,
                        LicenseType = licenseType,
                        Product = license.product,
                        UnderSupervision = license.underSupervision,
                        Supplier = license.supplier,
                        ValidCommissionFromDate = license.validCommissionFromDate != null ? license.validCommissionFromDate.Value : Convert.ToDateTime(license.ValidFromDate),
                        ValidCommissionToDate = license.validCommissionToDate,
                        ValidFromDate = license.ValidFromDate.Value,
                        ValidToDate = license.ValidToDate.Value,
                        SupervisorFirstName = license.underSupervision ? supervisor.FirstName : "",
                        SupervisorLastName = license.underSupervision ? supervisor.LastName : ""
                    });
                }
                return Ok(response);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [AcceptVerbs("POST")]
        [Route("Advisor/Details/")]
        public async Task<IHttpActionResult> PostAdvisorDetails(AdvisorDetailResponse adr)
        {
            var ctx = Request.GetOwinContext();
            var userManager = ctx.Get<ApplicationUserManager>();
            var rolesManager = ctx.GetUserManager<ApplicationRoleManager>();

            ApplicationUser userLogin = await userManager.FindByEmailAsync(adr.Email);
            ApplicationDbContext context = new ApplicationDbContext();
            IdentityUserRole userRole = userLogin.Roles.FirstOrDefault();
            IdentityRole role = rolesManager.FindByName("Admin");
            IdentityRole advisorRole = rolesManager.FindByName("Advisor");
            AdvisorType adminTypeId = context.AdvisorTypes.Where(c => c.Title == "Admin").FirstOrDefault();
            AdvisorType advisorTypeId = context.AdvisorTypes.Where(c => c.Title == "Adviser").FirstOrDefault();

            var advisor = await db.Advisors.FindAsync(adr.AdvisorId);
            var contact = await db.Contacts.FindAsync(advisor.ContactId);
            AdvisorType getAdvisorTypeId = new AdvisorType();
            var address = await db.Addresses.Where(a => a.Contact_Id == advisor.ContactId).FirstOrDefaultAsync();

            getAdvisorTypeId = adr.IsAdmin ? adminTypeId : advisorTypeId;

            //var advisorType = await db.AdvisorTypes.Where(x => x.Title == adr.AdvisorTypeTitle).FirstOrDefaultAsync();
            advisor.AdvisorType_Id = getAdvisorTypeId.Id; //advisorType.Id;
            advisor.AccountNumber = adr.AccountNumber ?? "";
            advisor.AccountType = adr.AccountType ?? "";
            advisor.Allowance = adr.Allowance;
            advisor.TaxDirectiveRate = adr.TaxDirectiveRate;
            advisor.AdvisorStatus_Id = (await db.AdvisorStatuses.Where(x => x.Name == adr.AdvisorStatusName).FirstOrDefaultAsync()).Id;
            advisor.BankName_Id = (await db.BankName.Where(x => x.Name == adr.BankName).FirstOrDefaultAsync()).Id;
            advisor.BranchCode_Id = (await db.BankBranchCodes.Where(x => x.Name == adr.BranchCodeName).FirstOrDefaultAsync()).Id;
            advisor.EffectiveStartDate = adr.EffectiveStartDate;
            advisor.EffectiveEndDate = adr.EffectiveEndDate;
            advisor.CmsCode = adr.CmsCode;
            if (!string.IsNullOrEmpty(adr.ContactTitleName))
            {
                advisor.ContactTitle_Id = (await db.ContactTitles.Where(x => x.Name == adr.ContactTitleName).FirstOrDefaultAsync()).Id;
            }
            contact.IdNumber = adr.IdNumber ?? "";
            contact.photoUrl = adr.PhotoUrl ?? "";
            contact.FirstName = adr.FirstName ?? "";
            contact.LastName = adr.LastName ?? "";
            contact.JobTitle = adr.JobTitle ?? "";
            contact.Cell1 = adr.Cell1 ?? "";
            contact.Cell2 = adr.Cell2 ?? "";
            contact.Tel1 = adr.Tel1 ?? "";
            contact.Tel2 = adr.Tel2 ?? "";
            contact.Email = adr.Email ?? "";
            contact.Email2 = adr.Email2 ?? "";
            address.Contact_Id = advisor.ContactId;
            address.AddressType_Id =  (await db.AddressTypes.Where(x => x.Name == adr.AddressType_name).FirstOrDefaultAsync()).Id;   // <---  drop down  adr.AddressTyper_Id ?
            address.Description = adr.AddressDescription ?? "";
            address.Street1 = adr.Street1 ?? "";
            address.Street2 = adr.Street2 ?? "";
            address.Street3 = adr.Street3 ?? "";
            address.Suburb = adr.Suburb ?? "";
            address.PostalCode = adr.PostalCode ?? "";
            address.City = adr.City ?? "";
            address.Province_Id = (await db.Provinces.Where(x => x.Name == adr.Province_name).FirstOrDefaultAsync()).Id;   // <---- drop down use adr
            address.Country_Id = (await db.Countries.Where(x => x.Name == adr.Country_name).FirstOrDefaultAsync()).Id;  //<-- drop down
            address.MapUrl = adr.MapUrl ?? "";
            advisor.User = new ViewModels.User
            {
                AdvisorId = adr.AdvisorId,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Username = adr.Username ?? "",
                Password = adr.Password ?? "",
                IsAdmin = adr.IsAdmin,
                Email = adr.Email
            };

            if (advisor.User != null)
                await UpdateCreateUser(advisor.User);

            db.Entry(advisor).CurrentValues.SetValues(advisor);
            db.Entry(contact).CurrentValues.SetValues(contact);
            db.Entry(address).CurrentValues.SetValues(address);
            
            await db.SaveChangesAsync();
            return Ok();
        }

        // commissionSummaryListVIP
        [Route("Advisor/CommissionSummary/{dateFromVip}/{dateToVip}")]
        public async Task<HttpResponseMessage> CommissionSummary(DateTime dateFromVip, DateTime dateToVip)
        {
            try
            {
                string userId = "unknown";
                var _user = await HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>()
                    .FindByIdAsync(HttpContext.Current.User.Identity.GetUserId());

                if (_user != null)
                {
                    userId = _user.Id;
                }

                string fileName = (
                    workingFolder + "\\"
                    + userId + "\\"
                    + (DateTime.Now.Ticks.ToString()).Replace(@"/", "-")
                    + "CommissionExport"
                    );

                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }

                if (File.Exists(fileName))
                {
                    Console.WriteLine(fileName + " already exists!");
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                FileStream fs = new FileStream(fileName.Replace(@"\\", @"\"), FileMode.CreateNew);

                var applicationHistory = await db.ApplicationAdvisorHistory
                    .Where(e => e.DateStarted <= dateToVip && ((e.DateEnded != null) ? e.DateEnded >= dateToVip : true))
                    .OrderBy(e => e.DateStarted)
                    .GroupBy(e => e.Application_Id)
                    .ToListAsync();

                var appsHist = new List<ApplicationAdvisorHistory>();

                foreach (var item in applicationHistory)
                {
                    appsHist.Add(item.Last());
                }

                var adviHist = (from x in appsHist
                                join y in db.Applications on x.Application_Id equals y.Id
                                join o in db.CommissionStatement on y.Client_Id equals o.ClientId
                                join w in db.Products on o.SupplierId equals w.SupplierId
                                join z in db.Suppliers on w.SupplierId equals z.Id
                                select new
                                {
                                    prodId = y.Id,
                                    ClientId = y.Client_Id,
                                    SupId = z.Id
                                })
                                .OrderBy(x => x.prodId)
                                .Distinct()
                                .ToList();

                var suppliersShares = (from x in db.AdvisorShareUnderSupervisions
                                       join y in db.Suppliers on x.SupplierId equals y.Id
                                       select new
                                       {
                                           share = x.Share,
                                           SupId = y.Id,
                                           AdviId = x.AdvisorId
                                       })
                                       .Distinct()
                                       .GroupBy(e => e.AdviId)
                                       .ToList();


                var comms = db.CommissionStatement.Where(e =>
                                   e.ApprovalStatus == "APPROVED" &&
                                   e.ApprovalDateFrom >= dateFromVip &&
                                   e.ApprovalDateTo <= dateToVip).AsEnumerable();

                var commses = (from c in comms
                               select new
                               {
                                   c.Id,
                                   c.Surname,
                                   c.AdvisorId,
                                   c.MemberNumber,
                                   c.MemberSearchKey,
                                   c.TransactionDate,
                                   c.SupplierId,
                                   c.Initial,
                                   c.CommissionExclVAT,
                                   c.CommissionInclVAT,
                                   c.AdvisorTax,
                                   c.AdvisorTaxRate,
                                   c.ClientId,
                                   c.ProductId,
                                   AdvisorVat = c.AdvisorTaxRate ,
                                   Date = c.TransactionDate
                               })
                               .OrderBy(v => v.MemberNumber)
                               .Distinct()
                               .AsEnumerable();

                string specifier = "F";
                CultureInfo culture = CultureInfo.CreateSpecificCulture("en-CA");

                var data = (from commission in commses
                            join supplier in db.Suppliers
                                on commission.SupplierId equals supplier.Id
                            join history in adviHist
                                on commission.ClientId equals history.ClientId
                            join advisors in db.Advisors
                                on commission.AdvisorId equals advisors.Id
                            join shares in suppliersShares
                                on commission.AdvisorId equals shares.Key                   
                            join cont in db.Contacts
                                on advisors.ContactId equals cont.Id
                            join app in db.Applications
                                on commission.ClientId equals app.Client_Id
                            join apptype in db.ApplicationTypes
                                on app.ApplicationType_Id equals apptype.Id
                            join bank in db.BankName
                                on advisors.BankName_Id equals bank.Id
                            join branch in db.BankBranchCodes
                                on advisors.BranchCode_Id equals branch.Id
                            select new
                            {
                                appID = commission.ProductId,
                                id = commission.Id,
                                surname = commission.Surname,
                                initial = commission.Initial,
                                applicationAdvisorId = app.Advisor_Id,
                                memberSearchKey = commission.MemberSearchKey,
                                transactionDate = commission.TransactionDate,
                                advisorCommission = (
                                    (double)commission.CommissionExclVAT *
                                    (shares.FirstOrDefault(e => e.SupId == commission.SupplierId)?.share == null ? 0: shares.FirstOrDefault(e => e.SupId == commission.SupplierId)?.share) / 100),
                                commissionExclVAT = commission.CommissionExclVAT,
                                commissionInclVAT = commission.CommissionInclVAT,
                                advisorTax = (commission.AdvisorTax),
                                advisorTaxRate = (commission.AdvisorTaxRate),
                                advisorId =  commission.AdvisorId,
                                commission.ClientId,
                                commission.ProductId,
                                supplierName = supplier.Name,
                                advisorLastname = cont.LastName,
                                advisorName = cont.FirstName,
                                advisorIDNumber = cont.IdNumber,
                                advisorMemberId = app.ApplicationNumber,
                                advisorBrancCode = branch.Name,
                                advisorBranchName = bank.Name,
                                adviAccountNo = advisors.AccountNumber,
                                advisorVat = commission.AdvisorTaxRate,                                
                                supplierId = supplier.Id,
                                supplier = supplier.Name,
                                date = commission.TransactionDate,
                                
                            })
                            .OrderBy(c => c.transactionDate)
                            .OrderBy(c => c.surname)
                            .Distinct()
                            .GroupBy(e => e.advisorId)
                            .ToList();

                string concatString = "";

                foreach (var re in data)
                {
                    concatString += string.Concat($"{re.FirstOrDefault().advisorName} " +
                                                   $"{re.FirstOrDefault().advisorLastname}" +
                                                   $",{re.FirstOrDefault().advisorIDNumber}" +
                                                   $",{re.FirstOrDefault().advisorMemberId}" +
                                                   $",{re.FirstOrDefault().advisorBranchName}" +
                                                   $",{re.FirstOrDefault().advisorBrancCode}" +
                                                   $",{re.FirstOrDefault().adviAccountNo}" +
                                                   $",{re.Where(e => e.applicationAdvisorId == re.Key).Sum(e => e.commissionExclVAT).Value.ToString(specifier, culture)}" +
                                                   $",{re.Where(e => e.applicationAdvisorId == re.Key).Sum(e => e.advisorCommission).Value.ToString(specifier, culture)}" +
                                                   $"{Environment.NewLine}"
                   );
                }
                

                string csv = $"ADVISER,ID NUMBER,MEMBER NUMBER,BANK NAME,BRANCH CODE,ACCOUNT NO,Total EXCL VAT,COMMISSION{Environment.NewLine}{concatString}{Environment.NewLine}";

                var tempFile = new System.Text.UTF8Encoding().GetBytes(csv);

                await fs.WriteAsync(tempFile, 0, tempFile.Length);
                fs.Close();

                MemoryStream responseStream = new MemoryStream();
                Stream fileStream = File.Open(fileName, FileMode.Open);
                bool fullContent = true;
                if (this.Request.Headers.Range != null)
                {
                    fullContent = false;

                    // Currently we only support a single range.
                    RangeItemHeaderValue range = this.Request.Headers.Range.Ranges.First();
                    
                    // From specified, so seek to the requested position.
                    if (range.From != null)
                    {
                        fileStream.Seek(range.From.Value, SeekOrigin.Begin);

                        // In this case, actually the complete file will be returned.
                        if (range.From == 0 && (range.To == null || range.To >= fileStream.Length))
                        {
                            fileStream.CopyTo(responseStream);
                            fullContent = true;
                        }
                    }
                    if (range.To != null)
                    {
                        // 10-20, return the range.
                        if (range.From != null)
                        {
                            long? rangeLength = range.To - range.From;
                            int length = (int)Math.Min(rangeLength.Value, fileStream.Length - range.From.Value);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                        // -20, return the bytes from beginning to the specified value.
                        else
                        {
                            int length = (int)Math.Min(range.To.Value, fileStream.Length);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                    }
                    // No Range.To
                    else
                    {
                        // 10-, return from the specified value to the end of file.
                        if (range.From != null)
                        {
                            if (range.From < fileStream.Length)
                            {
                                int length = (int)(fileStream.Length - range.From.Value);
                                byte[] buffer = new byte[length];
                                fileStream.Read(buffer, 0, length);
                                responseStream.Write(buffer, 0, length);
                            }
                        }
                    }
                }
                // No Range header. Return the complete file.
                else
                {
                    fileStream.CopyTo(responseStream);
                }
                fileStream.Close();
                responseStream.Position = 0;

                HttpResponseMessage response = new HttpResponseMessage
                {
                    StatusCode = fullContent ? HttpStatusCode.OK : HttpStatusCode.PartialContent,
                    Content = new StreamContent(responseStream)
                };
                return response;
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [Route("Advisor/RepList/")]
        public async Task<HttpResponseMessage> RepList()
        {
            try
            {

                string userId = "unknown";
                var _user = await HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>()
                    .FindByIdAsync(HttpContext.Current.User.Identity.GetUserId());

                if (_user != null)
                {
                    userId = _user.Id;
                }

                string fileName = (
                    workingFolder + "\\"
                    + userId + "\\"
                    + (DateTime.Now.Ticks.ToString()).Replace(@"/", "-")
                    + "CommissionExport"
                    );

                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }

                if (File.Exists(fileName))
                {
                    Console.WriteLine(fileName + " already exists!");
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                FileStream fs = new FileStream(fileName.Replace(@"\\", @"\"), FileMode.CreateNew);

                var advContacts = db.Advisors.Where(x => x.Id != 53)
                    .Join(db.Contacts,
                    advisor => advisor.ContactId,
                    contact => contact.Id,
                    (advisor, contact) => new
                    {
                        AdviId = advisor.Id,
                        AdviName = contact.FirstName,
                        AdviSurname = contact.LastName,
                        AdviIdNumer = contact.IdNumber,
                        AdviBrCode = advisor.CmsCode,
                        Role = advisor.AdvisorType,
                        Status = advisor.AdvisorStatus
                    })
                    .Join(db.Users,
                          contacts => contacts.AdviId,
                          Users => Users.AdvisorId,
                          (contacts, User) => new
                          {
                              contacts.AdviId,
                              contacts.AdviName,
                              contacts.AdviSurname,
                              contacts.AdviIdNumer,
                              contacts.AdviBrCode,
                              Role = User.Advisor.AdvisorType.Title,
                              Status = User.Advisor.AdvisorStatus.Name
                          });

                var advWithDocuments = advContacts
                    .GroupJoin(db.AdvisorDocuments.Where(d => d.DocumentTypeId == 10),
                    advisor => advisor.AdviId,
                    docu => docu.Advisor_Id,
                    (advisor, docu) => new
                    {
                        advisor.AdviId,
                        advisor.AdviName,
                        advisor.AdviSurname,
                        advisor.AdviIdNumer,
                        AdviDocEffDate = docu.FirstOrDefault(e => e.Advisor_Id == advisor.AdviId).ValidFromDate ?? DateTime.MinValue,
                        AdviDocEffToDate = docu.FirstOrDefault(e => e.Advisor_Id == advisor.AdviId).ValidToDate ?? DateTime.MinValue,
                        advisor.AdviBrCode,
                        advisor.Role,
                        advisor.Status
                    });

                var AdviWithDocUnderSupervision = db.AdvisorShareUnderSupervisions
                    .GroupJoin(advWithDocuments,
                    super => super.AdvisorId,
                    advisor => advisor.AdviId,
                    (super, advisor) => new
                    {
                        SuperVisorId = super.Advisor,
                        SupEffFrom = super.ValidFromDate,
                        SuppEffTo = super.ValidToDate,
                        super.LicenseTypeId,
                        UnderSupervision = super.underSupervision,
                        super.AdvisorId,                        
                    });

                var AdvisorSupers = AdviWithDocUnderSupervision
                    .Join(db.LicenseTypes,
                a => a.LicenseTypeId,
                l => l.Id,
                (a, l) => new
                {
                    a.SuperVisorId,
                    a.SupEffFrom,
                    a.SuppEffTo,
                    LicenseType = l.Description,
                    a.UnderSupervision,
                    a.AdvisorId
                });

                var advisorsupps = AdvisorSupers
                    .Join(db.Advisors,
                    supervisor => supervisor.SuperVisorId,
                    advisor => advisor.Id,
                    (supervisor, advisor) => new
                    {
                        advisor.ContactId,
                        supervisor.SuperVisorId,
                        supervisor.SupEffFrom,
                        supervisor.SuppEffTo,
                        supervisor.LicenseType,
                        supervisor.UnderSupervision,
                        supervisor.AdvisorId
                    });

                var AdvisorSuprDetails = advisorsupps
                    .Join(db.Contacts,
                    a => a.ContactId,
                    c => c.Id,
                    (a, c) => new
                    {
                        a.SuperVisorId,
                        a.SupEffFrom,
                        a.SuppEffTo,
                        a.LicenseType,
                        a.UnderSupervision,
                        a.AdvisorId,
                        SupervisorName = c.FirstName,
                        SupervisorLastname = c.LastName
                    });

                var Data = advWithDocuments.GroupJoin(AdvisorSuprDetails,
                    advisor => advisor.AdviId,
                    supervisor => supervisor.AdvisorId,
                    (advisor, result) => new RepListResponse
                    {
                        AdviBrCode = advisor.AdviBrCode,
                        AdviId = advisor.AdviId,
                        AdviIdNumer = advisor.AdviIdNumer,
                        AdviName = advisor.AdviName,
                        AdviSurname = advisor.AdviSurname,
                        EffFrom = advisor.AdviDocEffDate,
                        EffTo = advisor.AdviDocEffToDate,
                        LicenseType = result.FirstOrDefault(x => x.AdvisorId == advisor.AdviId).LicenseType ?? "No License Type",
                        SupEffFrom = result.FirstOrDefault(x => x.AdvisorId == advisor.AdviId).SupEffFrom ?? DateTime.MinValue,
                        SupEffTo = result.FirstOrDefault(x => x.AdvisorId == advisor.AdviId).SuppEffTo ?? DateTime.MinValue,
                        SupervisorId = result.FirstOrDefault(x => x.AdvisorId == advisor.AdviId).SuperVisorId,
                        SupervisorName = result.FirstOrDefault(x => x.AdvisorId == advisor.AdviId).SupervisorName ?? "No Supervisor",
                        SupervisorSurname = result.FirstOrDefault(x => x.AdvisorId == advisor.AdviId).SupervisorLastname ?? "",
                        UnderSupervision = (result.FirstOrDefault(x => x.AdvisorId == advisor.AdviId).UnderSupervision)? false : result.FirstOrDefault(x => x.AdvisorId == advisor.AdviId).UnderSupervision,
                        Role = advisor.Role,
                        Status = advisor.Status
                    })
                    .ToList();
                
                string csv = $"ADVISER NAME,ID NUMBER,UNDER SUPERVISION,SUPERVISION EFF DATE,SUPERVISION END DATE,SUPERVISOR NAME,LICENSE TYPE,BR NUMBER,EFFECTIVE DATE,END DATE,Role,Status{Environment.NewLine}{string.Concat(from re in Data select $"{re.AdviName} {re.AdviSurname},{re.AdviIdNumer},{re.UnderSupervision.ToString().ToUpper()},{String.Format("{0:dd-MMM-yyyy}", re.SupEffFrom)},{String.Format("{0:dd-MMM-yyyy}", re.SupEffTo)},{re.SupervisorName} {re.SupervisorSurname},{re.LicenseType},{re.AdviBrCode},{String.Format("{0:dd-MMM-yyyy}", re.EffFrom)},{String.Format("{0:dd-MMM-yyyy}", re.EffTo)},{re.Role},{re.Status}{Environment.NewLine}")}";
                csv = csv.Replace(String.Format("{0:MMM-yyyy}", DateTime.MinValue), "");
                csv = csv.Replace("Jan-1901", "");

                var tempFile = new System.Text.UTF8Encoding().GetBytes(csv);

                await fs.WriteAsync(tempFile, 0, tempFile.Length);
                fs.Close();


                MemoryStream responseStream = new MemoryStream();
                Stream fileStream = File.Open(fileName, FileMode.Open);
                bool fullContent = true;
                if (this.Request.Headers.Range != null)
                {
                    fullContent = false;

                    // Currently we only support a single range.
                    RangeItemHeaderValue range = this.Request.Headers.Range.Ranges.First();


                    // From specified, so seek to the requested position.
                    if (range.From != null)
                    {
                        fileStream.Seek(range.From.Value, SeekOrigin.Begin);

                        // In this case, actually the complete file will be returned.
                        if (range.From == 0 && (range.To == null || range.To >= fileStream.Length))
                        {
                            fileStream.CopyTo(responseStream);
                            fullContent = true;
                        }
                    }
                    if (range.To != null)
                    {
                        // 10-20, return the range.
                        if (range.From != null)
                        {
                            long? rangeLength = range.To - range.From;
                            int length = (int)Math.Min(rangeLength.Value, fileStream.Length - range.From.Value);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                        // -20, return the bytes from beginning to the specified value.
                        else
                        {
                            int length = (int)Math.Min(range.To.Value, fileStream.Length);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                    }
                    // No Range.To
                    else
                    {
                        // 10-, return from the specified value to the end of file.
                        if (range.From != null)
                        {
                            if (range.From < fileStream.Length)
                            {
                                int length = (int)(fileStream.Length - range.From.Value);
                                byte[] buffer = new byte[length];
                                fileStream.Read(buffer, 0, length);
                                responseStream.Write(buffer, 0, length);
                            }
                        }
                    }
                }
                // No Range header. Return the complete file.
                else
                {
                    fileStream.CopyTo(responseStream);
                }
                fileStream.Close();
                responseStream.Position = 0;



                HttpResponseMessage response = new HttpResponseMessage
                {
                    StatusCode = fullContent ? HttpStatusCode.OK : HttpStatusCode.PartialContent,
                    Content = new StreamContent(responseStream)
                };
                return response;
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [Route("Advisor/TotalBusiness/{dateFrom}/{dateTo}/{ProductId}")]
        public async Task<HttpResponseMessage> TotalBusiness(DateTime? dateFrom = null, DateTime? dateTo = null, int? ProductId = null)
        {
            try
            {

                string userId = "unknown";
                var _user = await HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>()
                    .FindByIdAsync(HttpContext.Current.User.Identity.GetUserId());

                if (_user != null)
                {
                    userId = _user.Id;
                }

                string fileName = (
                    workingFolder + "\\"
                    + userId + "\\"
                    + (DateTime.Now.Ticks.ToString()).Replace(@"/", "-")
                    + "CommissionExport"
                    );

                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }

                if (File.Exists(fileName))
                {
                    Console.WriteLine(fileName + " already exists!");
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                FileStream fs = new FileStream(fileName.Replace(@"\\", @"\"), FileMode.CreateNew);

                var applicationSupplier = db.ApplicationSuppliers.Where(e => e.SupplierId == ProductId).Select(e => e.ApplicationId).ToList();

                var application = from apps in db.Applications
                                  where applicationSupplier.Contains(apps.Id)
                                  select apps;



                var data = await application
                    .Include(e => e.ApplicationDocuments.Select(x => x.DocumentTypeId))
                    .Include(e => e.ApplicationDocuments.Select(x => x.Uploaded))
                    .Include(e => e.ApplicationDocuments.Select(x => x.ApplicationId))
                    .Where(c => c.CreatedDate < dateTo && c.CreatedDate >= dateFrom)
                    .Join(db.Advisors,
                    App => App.Advisor_Id,
                    Advi => Advi.Id,
                    (App, Advi) => new
                    {
                        Month = App.CreatedDate,
                        DateRecieved = App.CreatedDate,
                        AdviContactId = Advi.ContactId,
                        ProductId = App.Product_Id,
                        ApplicationTypeId = App.ApplicationType_Id,
                        ApplicationStatusId = App.ApplicationStatus_Id,
                        ApplicationId = App.Id,
                        ClientId = App.Client_Id,
                        App.ApplicationNumber,
                        ApplicationDoc = App.ApplicationDocuments.Where(e => e.Uploaded == true && e.DocumentTypeId == 37).Any(),
                        AdviceRecord = App.ApplicationDocuments.Where(e => e.Uploaded == true && e.DocumentTypeId == 38).Any(),
                        DisclosureDoc = App.ApplicationDocuments.Where(e => e.Uploaded == true && e.DocumentTypeId == 39).Any(),
                        AdvisorAppointment = App.ApplicationDocuments.Where(e => e.Uploaded == true && e.DocumentTypeId == 7).Any(),
                    })
                    .Join(db.Contacts,
                    App => App.AdviContactId,
                    Advi => Advi.Id,
                    (App, Advi) => new
                    {
                        App.Month,
                        App.DateRecieved,
                        AdvisorFirstName = Advi.FirstName,
                        AdvisorLastName = Advi.LastName,
                        AdvisorIdNumber = Advi.IdNumber,
                        AdvisorMemberId = App.ApplicationNumber,
                        App.ProductId,
                        App.ApplicationTypeId,
                        App.ApplicationStatusId,
                        App.ClientId,
                        App.ApplicationId,
                        App.ApplicationDoc,
                        App.AdviceRecord,
                        App.DisclosureDoc,
                        App.AdvisorAppointment
                    })
                    .Join(db.Products,
                    App => App.ProductId,
                    Prod => Prod.Id,
                    (App, Prod) => new
                    {
                        App.Month,
                        App.DateRecieved,
                        App.AdvisorFirstName,
                        App.AdvisorLastName,
                        App.ApplicationTypeId,
                        Prod.SupplierId,
                        BenifitOption = Prod.Name,
                        App.ClientId,
                        App.AdvisorIdNumber,
                        App.AdvisorMemberId,
                        App.ApplicationStatusId,
                        App.ApplicationDoc,
                        App.AdviceRecord,
                        App.DisclosureDoc,
                        App.AdvisorAppointment,
                        App.ApplicationId
                    })
                    .Join(db.ApplicationTypes,
                    App => App.ApplicationTypeId,
                    AppType => AppType.Id,
                    (App, AppType) => new
                    {
                        App.Month,
                        App.DateRecieved,
                        App.AdvisorFirstName,
                        App.AdvisorLastName,
                        ApplicationType = AppType.Title,
                        App.SupplierId,
                        App.BenifitOption,
                        App.ClientId,
                        App.AdvisorIdNumber,
                        App.AdvisorMemberId,
                        App.ApplicationStatusId,
                        App.ApplicationDoc,
                        App.AdviceRecord,
                        App.DisclosureDoc,
                        App.AdvisorAppointment,
                        App.ApplicationId
                    })
                    .Join(db.ApplicationStatuses,
                      App => App.ApplicationStatusId,
                      AppStatus => AppStatus.Id,
                      (App, AppStatus) => new
                      {
                          App.Month,
                          App.DateRecieved,
                          App.AdvisorFirstName,
                          App.AdvisorLastName,
                          App.ApplicationType,
                          App.SupplierId,
                          App.BenifitOption,
                          App.ClientId,
                          App.AdvisorIdNumber,
                          App.AdvisorMemberId,
                          ApplicationStatus = AppStatus.Status,
                          App.ApplicationDoc,
                          App.AdviceRecord,
                          App.DisclosureDoc,
                          App.AdvisorAppointment,
                          App.ApplicationId
                      })
                      .Join(db.Suppliers,
                      App => App.SupplierId,
                      Sup => Sup.Id,
                      (App, Sup) => new
                      {
                          App.Month,
                          App.DateRecieved,
                          App.AdvisorFirstName,
                          App.AdvisorLastName,
                          App.ApplicationType,
                          SupplierName = Sup.Name,
                          App.BenifitOption,
                          App.ClientId,
                          App.AdvisorIdNumber,
                          App.AdvisorMemberId,
                          App.ApplicationStatus,
                          App.ApplicationDoc,
                          App.AdviceRecord,
                          App.DisclosureDoc,
                          App.AdvisorAppointment,
                          App.ApplicationId
                      })
                    .Join(db.Contacts,
                     App => App.ClientId,
                     client => client.Id,
                     (App, client) => new
                     {
                         App.Month,
                         App.DateRecieved,
                         App.AdvisorFirstName,
                         App.AdvisorLastName,
                         App.ApplicationType,
                         App.SupplierName,
                         App.BenifitOption,
                         SupplierInitial = client.FirstName,
                         SupplierSurname = client.LastName,
                         App.AdvisorIdNumber,
                         App.AdvisorMemberId,
                         Status = App.ApplicationStatus,
                         EnrolmentDate = client.CreatedDate,
                         clientIdNumber  = client.IdNumber,
                         App.ApplicationStatus,
                         App.ApplicationDoc,
                         App.AdviceRecord,
                         App.DisclosureDoc,
                         App.AdvisorAppointment,
                         App.ApplicationId
                     })
                    .Select(x => new
                    {
                        x.Month,
                        x.DateRecieved,
                        x.AdvisorFirstName,
                        x.AdvisorLastName,
                        x.ApplicationType,
                        Supplier = x.SupplierName,
                        x.BenifitOption,
                        Name = x.SupplierInitial,
                        Surname = x.SupplierSurname,
                        IdNumber = x.clientIdNumber,
                        MemberId = x.AdvisorMemberId,
                        x.EnrolmentDate,
                        x.AdvisorAppointment,
                        x.ApplicationDoc,
                        x.AdviceRecord,
                        x.DisclosureDoc,
                        Status = x.ApplicationStatus,
                    })
                    .ToListAsync();

                string csv = $"MONTH,DATE RECIEVED,ADVISER NAME,APPLICATION TYPE,SUPPLIER,BENIFIT OPTION,NAME,SURNAME,ID NUMBER, MEMBER ID,ENROLMENT DATE,ADVISER APPOINTMENT RECIEVED,APPLICATION RECIEVED,ADVICE RECORD RECIEVED,DISCLOSURE DOC,STATUS{Environment.NewLine}{string.Concat(from re in data select $"{String.Format("{0:MMM-yyyy}", re.Month)},{String.Format("{0:dd-MMM-yyyy}",re.DateRecieved)},{re.AdvisorFirstName}  {re.AdvisorLastName},{re.ApplicationType},{re.Supplier},{re.BenifitOption},{re.Name},{re.Surname},{re.IdNumber},{re.MemberId},{String.Format("{0:dd-MMM-yyyy}", re.EnrolmentDate)},{re.AdvisorAppointment},{re.ApplicationDoc},{re.AdviceRecord},{re.DisclosureDoc},{re.Status}{Environment.NewLine}")}";

                var tempFile = new System.Text.UTF8Encoding().GetBytes(csv);

                await fs.WriteAsync(tempFile, 0, tempFile.Length);
                fs.Close();


                MemoryStream responseStream = new MemoryStream();
                Stream fileStream = File.Open(fileName, FileMode.Open);
                bool fullContent = true;
                if (this.Request.Headers.Range != null)
                {
                    fullContent = false;

                    // Currently we only support a single range.
                    RangeItemHeaderValue range = this.Request.Headers.Range.Ranges.First();


                    // From specified, so seek to the requested position.
                    if (range.From != null)
                    {
                        fileStream.Seek(range.From.Value, SeekOrigin.Begin);

                        // In this case, actually the complete file will be returned.
                        if (range.From == 0 && (range.To == null || range.To >= fileStream.Length))
                        {
                            fileStream.CopyTo(responseStream);
                            fullContent = true;
                        }
                    }
                    if (range.To != null)
                    {
                        // 10-20, return the range.
                        if (range.From != null)
                        {
                            long? rangeLength = range.To - range.From;
                            int length = (int)Math.Min(rangeLength.Value, fileStream.Length - range.From.Value);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                        // -20, return the bytes from beginning to the specified value.
                        else
                        {
                            int length = (int)Math.Min(range.To.Value, fileStream.Length);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                    }
                    // No Range.To
                    else
                    {
                        // 10-, return from the specified value to the end of file.
                        if (range.From != null)
                        {
                            if (range.From < fileStream.Length)
                            {
                                int length = (int)(fileStream.Length - range.From.Value);
                                byte[] buffer = new byte[length];
                                fileStream.Read(buffer, 0, length);
                                responseStream.Write(buffer, 0, length);
                            }
                        }
                    }
                }
                // No Range header. Return the complete file.
                else
                {
                    fileStream.CopyTo(responseStream);
                }
                fileStream.Close();
                responseStream.Position = 0;



                HttpResponseMessage response = new HttpResponseMessage
                {
                    StatusCode = fullContent ? HttpStatusCode.OK : HttpStatusCode.PartialContent,
                    Content = new StreamContent(responseStream)
                };
                return response;
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [Route("Advisor/{Id}/TotalBusiness/{dateFrom}/{dateTo}/{ProductId}/{StatusId}")]
        public async Task<HttpResponseMessage> TotalBusinessPerConsultantPerStatus(int Id, DateTime? dateFrom = null, DateTime? dateTo = null, int? ProductId = null, int? StatusId = null)
        {
            try
            {
                string userId = "unknown";
                var _user = await HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>()
                    .FindByIdAsync(HttpContext.Current.User.Identity.GetUserId());

                if (_user != null)
                {
                    userId = _user.Id;
                }

                string fileName = (
                    workingFolder + "\\"
                    + userId + "\\"
                    + (DateTime.Now.Ticks.ToString()).Replace(@"/", "-")
                    + "CommissionExport"
                    );

                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }

                if (File.Exists(fileName))
                {
                    Console.WriteLine(fileName + " already exists!");
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                FileStream fs = new FileStream(fileName.Replace(@"\\", @"\"), FileMode.CreateNew);
                string csv = "";
                if (StatusId == null && ProductId == null)
                {
                    var data = await db.Applications
                      .Include(e => e.ApplicationDocuments)
                      .Where(
                            c => c.CreatedDate < dateTo &&
                            c.CreatedDate >= dateFrom &&
                            c.Advisor_Id == Id &&
                            c.Deleted != true)
                      .Join(db.Advisors,
                          App => App.Advisor_Id,
                          Advi => Advi.Id,
                          (App, Advi) => new
                          {
                              Month = App.CreatedDate,
                              DateRecieved = App.CreatedDate,
                              AdviContactId = Advi.ContactId,
                              ProductId = App.Product_Id,
                              ApplicationTypeId = App.ApplicationType_Id,
                              ApplicationStatusId = App.ApplicationStatus_Id,
                              ApplicationId = App.Id,
                              ClientId = App.Client_Id,
                              App.ApplicationNumber,
                              ApplicationDoc = App.ApplicationDocuments.Where(e => e.Uploaded == true && e.DocumentTypeId == 37).Any(),
                              AdviceRecord = App.ApplicationDocuments.Where(e => e.Uploaded == true && e.DocumentTypeId == 38).Any(),
                              DisclosureDoc = App.ApplicationDocuments.Where(e => e.Uploaded == true && e.DocumentTypeId == 39).Any(),
                              AdvisorAppointment = App.ApplicationDocuments.Where(e => e.Uploaded == true && e.DocumentTypeId == 7).Any(),
                              Advi.EffectiveStartDate,
                              Advi.EffectiveEndDate
                          })
                      .Join(db.Contacts,
                          App => App.AdviContactId,
                          Advi => Advi.Id,
                          (App, Advi) => new
                          {
                              App.Month,
                              App.DateRecieved,
                              AdvisorFirstName = Advi.FirstName,
                              AdvisorLastName = Advi.LastName,
                              AdvisorIdNumber = Advi.IdNumber,
                              AdvisorMemberId = App.ApplicationNumber,
                              App.ProductId,
                              App.ApplicationTypeId,
                              App.ApplicationStatusId,
                              App.ClientId,
                              App.ApplicationId,
                              App.ApplicationDoc,
                              App.AdviceRecord,
                              App.DisclosureDoc,
                              App.AdvisorAppointment,
                              App.EffectiveStartDate,
                              App.EffectiveEndDate
                          })
                      .Join(db.Products,
                          App => App.ProductId,
                          Prod => Prod.Id,
                          (App, Prod) => new
                          {
                              App.Month,
                              App.DateRecieved,
                              App.AdvisorFirstName,
                              App.AdvisorLastName,
                              App.ApplicationTypeId,
                              Prod.SupplierId,
                              BenifitOption = Prod.Name,
                              App.ClientId,
                              App.AdvisorIdNumber,
                              App.AdvisorMemberId,
                              App.ApplicationStatusId,
                              App.ApplicationDoc,
                              App.AdviceRecord,
                              App.DisclosureDoc,
                              App.AdvisorAppointment,
                              App.ApplicationId,
                              App.EffectiveStartDate,
                              App.EffectiveEndDate
                            })
                       .Join(db.Suppliers,
                            App => App.SupplierId,
                            Sup => Sup.Id,
                            (App, Sup) => new
                            {
                                App.Month,
                                App.ApplicationStatusId,
                                App.ApplicationTypeId,
                                App.DateRecieved,
                                App.AdvisorFirstName,
                                App.AdvisorLastName,
                                SupplierName = Sup.Name,
                                App.BenifitOption,
                                App.ClientId,
                                App.AdvisorIdNumber,
                                App.AdvisorMemberId,
                                App.ApplicationDoc,
                                App.AdviceRecord,
                                App.DisclosureDoc,
                                App.AdvisorAppointment,
                                App.ApplicationId,
                                App.EffectiveStartDate,
                                App.EffectiveEndDate
                            })
                      .Join(db.ApplicationTypes,
                          App => App.ApplicationTypeId,
                          AppType => AppType.Id,
                          (App, AppType) => new
                          {
                              App.Month,
                              App.DateRecieved,
                              App.ApplicationStatusId,
                              App.AdvisorFirstName,
                              App.AdvisorLastName,
                              ApplicationType = AppType.Title,
                              App.SupplierName,
                              App.BenifitOption,
                              App.ClientId,
                              App.AdvisorIdNumber,
                              App.AdvisorMemberId,
                              App.ApplicationDoc,
                              App.AdviceRecord,
                              App.DisclosureDoc,
                              App.AdvisorAppointment,
                              App.ApplicationId,
                              App.EffectiveStartDate,
                              App.EffectiveEndDate
                          })
                      .Join(db.ApplicationStatuses,
                        App => App.ApplicationStatusId,
                        AppStatus => AppStatus.Id,
                        (App, AppStatus) => new
                        {
                            App.Month,
                            App.DateRecieved,
                            App.AdvisorFirstName,
                            App.AdvisorLastName,
                            App.ApplicationType,
                            App.SupplierName,
                            App.BenifitOption,
                            App.ClientId,
                            App.AdvisorIdNumber,
                            App.AdvisorMemberId,
                            ApplicationStatus = AppStatus.Status,
                            App.ApplicationDoc,
                            App.AdviceRecord,
                            App.DisclosureDoc,
                            App.AdvisorAppointment,
                            App.ApplicationId,
                            App.EffectiveStartDate,
                            App.EffectiveEndDate
                        })
                      .Join(db.Contacts,
                           App => App.ClientId,
                           client => client.Id,
                           (App, client) => new
                           {
                               App.Month,
                               App.DateRecieved,
                               App.AdvisorFirstName,
                               App.AdvisorLastName,
                               App.ApplicationType,
                               App.SupplierName,
                               App.BenifitOption,
                               SupplierInitial = client.Initial,
                               SupplierSurname = client.LastName,
                               ClientIdNumber = client.IdNumber,
                               ClientMemberId = App.AdvisorMemberId, // client.MemberId,
                               Status = App.ApplicationStatus,
                               EnrolmentDate = client.CreatedDate,
                               App.ApplicationStatus,
                               App.ApplicationDoc,
                               App.AdviceRecord,
                               App.DisclosureDoc,
                               App.AdvisorAppointment,
                               App.ApplicationId,
                               App.EffectiveStartDate,
                               App.EffectiveEndDate
                           })
                      .Select(x => new
                      {
                          x.Month,
                          x.DateRecieved,
                          x.AdvisorFirstName,
                          x.AdvisorLastName,
                          x.ApplicationType,
                          Supplier = x.SupplierName,
                          x.BenifitOption,
                          Name = x.SupplierInitial,
                          Surname = x.SupplierSurname,
                          IdNumber = x.ClientIdNumber,
                          MemberId = x.ClientMemberId,
                          x.EnrolmentDate,
                          x.AdvisorAppointment,
                          x.ApplicationDoc,
                          x.AdviceRecord,
                          x.DisclosureDoc,
                          Status = x.ApplicationStatus,
                          x.EffectiveStartDate,
                          x.EffectiveEndDate
                      })
                      .ToListAsync();

                    var other = data.Where(e => (e.Status != "Complete") && (e.Status != "New") && (e.Status != "Pending")).ToList();

                    csv = $"MONTH,DATE RECIEVED,ADVISER NAME,APPLICATION TYPE,SUPPLIER,BENIFIT OPTION,NAME,SURNAME,ID NUMBER,MEMBER ID,ENROLMENT DATE,ADVISER APPOINTMENT RECIEVED,APPLICATION RECIEVED,ADVICE RECORD RECIEVED,DISCLOSURE DOC,STATUS{Environment.NewLine}{string.Concat(from re in data select $"{String.Format("{0:MMM-yyyy}", re.Month)},{String.Format("{0:dd-MMM-yyyy}", re.DateRecieved)},{re.AdvisorFirstName}  {re.AdvisorLastName},{re.ApplicationType},{re.Supplier},{re.BenifitOption},{re.Name},{re.Surname},{re.IdNumber},{re.MemberId},{String.Format("{0:dd-MMM-yyyy}", re.EnrolmentDate)},{re.AdvisorAppointment},{re.ApplicationDoc},{re.AdviceRecord},{re.DisclosureDoc},{re.Status}{Environment.NewLine}")}";
                }
                else if (StatusId == null)
                {
                    var data = await db.Applications
                      .Include(e => e.ApplicationDocuments)
                      .Where(
                            c => c.CreatedDate < dateTo &&
                            c.CreatedDate >= dateFrom &&
                            c.ApplicationSuppliers.FirstOrDefault().SupplierId == ProductId &&
                            c.Advisor_Id == Id &&
                            c.Deleted != true)
                      .Join(db.Advisors,
                            App => App.Advisor_Id,
                            Advi => Advi.Id,
                            (App, Advi) => new
                            {
                                Month = App.CreatedDate,
                                DateRecieved = App.CreatedDate,
                                AdviContactId = Advi.ContactId,
                                AdviMemberNumber = App.ApplicationNumber,
                                ProductId = App.Product_Id,
                                ApplicationTypeId = App.ApplicationType_Id,
                                ApplicationStatusId = App.ApplicationStatus_Id,
                                ApplicationId = App.Id,
                                ClientId = App.Client_Id,
                                ApplicationDoc = App.ApplicationDocuments.Where(e => e.Uploaded == true && e.DocumentTypeId == 37).Any(),
                                AdviceRecord = App.ApplicationDocuments.Where(e => e.Uploaded == true && e.DocumentTypeId == 38).Any(),
                                DisclosureDoc = App.ApplicationDocuments.Where(e => e.Uploaded == true && e.DocumentTypeId == 39).Any(),
                                AdvisorAppointment = App.ApplicationDocuments.Where(e => e.Uploaded == true && e.DocumentTypeId == 7).Any(),
                                Advi.EffectiveStartDate,
                                Advi.EffectiveEndDate
                            })
                      .Join(db.Contacts,
                            App => App.AdviContactId,
                            Advi => Advi.Id,
                            (App, Advi) => new
                            {
                                App.Month,
                                App.DateRecieved,
                                AdvisorFirstName = Advi.FirstName,
                                AdvisorLastName = Advi.LastName,
                                AdvisorIdNumber = Advi.IdNumber,
                                AdvisorMemberId = App.AdviMemberNumber,
                                App.ProductId,
                                App.ApplicationTypeId,
                                App.ApplicationStatusId,
                                App.ClientId,
                                App.ApplicationId,
                                App.ApplicationDoc,
                                App.AdviceRecord,
                                App.DisclosureDoc,
                                App.AdvisorAppointment,
                                App.EffectiveStartDate,
                                App.EffectiveEndDate
                            })
                      .Join(db.Products,
                            App => App.ProductId,
                            Prod => Prod.Id,
                            (App, Prod) => new
                            {
                                App.Month,
                                App.DateRecieved,
                                App.AdvisorFirstName,
                                App.AdvisorLastName,
                                App.ApplicationTypeId,
                                Prod.SupplierId,
                                BenifitOption = Prod.Name,
                                App.ClientId,
                                App.AdvisorIdNumber,
                                App.AdvisorMemberId,
                                App.ApplicationStatusId,
                                App.ApplicationDoc,
                                App.AdviceRecord,
                                App.DisclosureDoc,
                                App.AdvisorAppointment,
                                App.ApplicationId,
                                App.EffectiveStartDate,
                                App.EffectiveEndDate
                              })
                      .Join(db.ApplicationTypes,
                            App => App.ApplicationTypeId,
                            AppType => AppType.Id,
                            (App, AppType) => new
                            {
                                App.Month,
                                App.DateRecieved,
                                App.AdvisorFirstName,
                                App.AdvisorLastName,
                                ApplicationType = AppType.Title,
                                App.SupplierId,
                                App.BenifitOption,
                                App.ClientId,
                                App.AdvisorIdNumber,
                                App.AdvisorMemberId,
                                App.ApplicationStatusId,
                                App.ApplicationDoc,
                                App.AdviceRecord,
                                App.DisclosureDoc,
                                App.AdvisorAppointment,
                                App.ApplicationId,
                                App.EffectiveStartDate,
                                App.EffectiveEndDate
                            })
                      .Join(db.ApplicationStatuses,
                            App => App.ApplicationStatusId,
                            AppStatus => AppStatus.Id,
                            (App, AppStatus) => new
                            {
                                App.Month,
                                App.DateRecieved,
                                App.AdvisorFirstName,
                                App.AdvisorLastName,
                                App.ApplicationType,
                                App.SupplierId,
                                App.BenifitOption,
                                App.ClientId,
                                App.AdvisorIdNumber,
                                App.AdvisorMemberId,
                                ApplicationStatus = AppStatus.Status,
                                App.ApplicationDoc,
                                App.AdviceRecord,
                                App.DisclosureDoc,
                                App.AdvisorAppointment,
                                App.ApplicationId,
                                App.EffectiveStartDate,
                                App.EffectiveEndDate
                            })
                        .Join(db.Suppliers,
                            App => App.SupplierId,
                            Sup => Sup.Id,
                            (App, Sup) => new
                            {
                                App.Month,
                                App.DateRecieved,
                                App.AdvisorFirstName,
                                App.AdvisorLastName,
                                App.ApplicationType,
                                SupplierName = Sup.Name,
                                App.BenifitOption,
                                App.ClientId,
                                App.AdvisorIdNumber,
                                App.AdvisorMemberId,
                                App.ApplicationStatus,
                                App.ApplicationDoc,
                                App.AdviceRecord,
                                App.DisclosureDoc,
                                App.AdvisorAppointment,
                                App.ApplicationId,
                                App.EffectiveStartDate,
                                App.EffectiveEndDate,
                            })
                      .Join(db.Contacts,
                           App => App.ClientId,
                           client => client.Id,
                           (App, client) => new
                           {
                               App.Month,
                               App.DateRecieved,
                               App.AdvisorFirstName,
                               App.AdvisorLastName,
                               App.ApplicationType,
                               App.SupplierName,
                               App.BenifitOption,
                               SupplierInitial = client.Initial,
                               SupplierSurname = client.LastName,
                               SupplierID = client.IdNumber,
                               SupplierMemberId = App.AdvisorMemberId,
                               App.AdvisorIdNumber,
                               Status = App.ApplicationStatus,
                               EnrolmentDate = client.CreatedDate,
                               App.ApplicationStatus,
                               App.ApplicationDoc,
                               App.AdviceRecord,
                               App.DisclosureDoc,
                               App.AdvisorAppointment,
                               App.ApplicationId,
                               App.EffectiveStartDate,
                               App.EffectiveEndDate
                           })
                      .Select(x => new
                      {
                          x.Month,
                          x.DateRecieved,
                          x.AdvisorFirstName,
                          x.AdvisorLastName,
                          x.ApplicationType,
                          Supplier = x.SupplierName,
                          x.BenifitOption,
                          Name = x.SupplierInitial,
                          Surname = x.SupplierSurname,
                          IdNumber = x.SupplierID,
                          MemberId = x.SupplierMemberId,
                          x.EnrolmentDate,
                          x.AdvisorAppointment,
                          x.ApplicationDoc,
                          x.AdviceRecord,
                          x.DisclosureDoc,
                          Status = x.ApplicationStatus,
                          x.EffectiveStartDate,
                          x.EffectiveEndDate
                      }).ToListAsync();

                    csv = $"MONTH,DATE RECIEVED,ADVISER NAME,APPLICATION TYPE,SUPPLIER,BENIFIT OPTION,NAME,SURNAME,ID NUMBER,MEMBER ID,ENROLMENT DATE,ADVISER APPOINTMENT RECIEVED,APPLICATION RECIEVED,ADVICE RECORD RECIEVED,DISCLOSURE DOC,STATUS{Environment.NewLine}{string.Concat(from re in data select $"{String.Format("{0:MMM-yyyy}", re.Month)},{String.Format("{0:dd-MMM-yyyy}", re.DateRecieved)},{re.AdvisorFirstName}  {re.AdvisorLastName},{re.ApplicationType},{re.Supplier},{re.BenifitOption},{re.Name},{re.Surname},{re.IdNumber},{re.MemberId},{String.Format("{0:dd-MMM-yyyy}", re.EnrolmentDate)},{re.AdvisorAppointment},{re.ApplicationDoc},{re.AdviceRecord},{re.DisclosureDoc},{re.Status}{Environment.NewLine}")}";
                }
                else if (ProductId == null)
                {
                    var data = await db.Applications
                      .Include(e => e.ApplicationDocuments)
                      .Where( c => c.CreatedDate < dateTo &&
                            c.CreatedDate >= dateFrom &&
                            c.ApplicationStatus_Id == StatusId &&
                            c.Advisor_Id == Id &&
                             c.Deleted != true)
                      .Join(db.Advisors,
                          App => App.Advisor_Id,
                          Advi => Advi.Id,
                          (App, Advi) => new
                          {
                              Month = App.CreatedDate,
                              DateRecieved = App.CreatedDate,
                              AdviContactId = Advi.ContactId,
                              ProductId = App.Product_Id,
                              ApplicationTypeId = App.ApplicationType_Id,
                              ApplicationStatusId = App.ApplicationStatus_Id,
                              ApplicationId = App.Id,
                              App.ApplicationNumber,
                              ClientId = App.Client_Id,
                              ApplicationDoc = App.ApplicationDocuments.Where(e => e.Uploaded == true && e.DocumentTypeId == 37).Any(),
                              AdviceRecord = App.ApplicationDocuments.Where(e => e.Uploaded == true && e.DocumentTypeId == 38).Any(),
                              DisclosureDoc = App.ApplicationDocuments.Where(e => e.Uploaded == true && e.DocumentTypeId == 39).Any(),
                              AdvisorAppointment = App.ApplicationDocuments.Where(e => e.Uploaded == true && e.DocumentTypeId == 7).Any(),
                              Advi.EffectiveStartDate,
                              Advi.EffectiveEndDate
                          })
                      .Join(db.Contacts,
                          App => App.AdviContactId,
                          Advi => Advi.Id,
                          (App, Advi) => new
                          {
                              App.Month,
                              App.DateRecieved,
                              AdvisorFirstName = Advi.FirstName,
                              AdvisorLastName = Advi.LastName,
                              AdvisorIdNumber = Advi.IdNumber,
                              AdvisorMemberId =  App.ApplicationNumber,
                              App.ProductId,
                              App.ApplicationTypeId,
                              App.ApplicationStatusId,
                              App.ClientId,
                              App.ApplicationId,
                              App.ApplicationDoc,
                              App.AdviceRecord,
                              App.DisclosureDoc,
                              App.AdvisorAppointment,
                              App.EffectiveStartDate,
                              App.EffectiveEndDate
                          })
                      .Join(db.Products,
                          App => App.ProductId,
                          Prod => Prod.Id,
                          (App, Prod) => new
                          {
                              App.Month,
                              App.DateRecieved,
                              App.AdvisorFirstName,
                              App.AdvisorLastName,
                              App.ApplicationTypeId,
                              Prod.SupplierId,
                              BenifitOption = Prod.Name,
                              App.ClientId,
                              App.AdvisorIdNumber,
                              App.AdvisorMemberId,
                              App.ApplicationStatusId,
                              App.ApplicationDoc,
                              App.AdviceRecord,
                              App.DisclosureDoc,
                              App.AdvisorAppointment,
                              App.ApplicationId,
                              App.EffectiveStartDate,
                              App.EffectiveEndDate
                          })
                      .Join(db.ApplicationTypes,
                          App => App.ApplicationTypeId,
                          AppType => AppType.Id,
                          (App, AppType) => new
                          {
                              App.Month,
                              App.DateRecieved,
                              App.AdvisorFirstName,
                              App.AdvisorLastName,
                              ApplicationType = AppType.Title,
                              App.SupplierId,
                              App.BenifitOption,
                              App.ClientId,
                              App.AdvisorIdNumber,
                              App.AdvisorMemberId,
                              App.ApplicationStatusId,
                              App.ApplicationDoc,
                              App.AdviceRecord,
                              App.DisclosureDoc,
                              App.AdvisorAppointment,
                              App.ApplicationId,
                              App.EffectiveStartDate,
                              App.EffectiveEndDate
                          })
                      .Join(db.ApplicationStatuses,
                        App => App.ApplicationStatusId,
                        AppStatus => AppStatus.Id,
                        (App, AppStatus) => new
                        {
                            App.Month,
                            App.DateRecieved,
                            App.AdvisorFirstName,
                            App.AdvisorLastName,
                            App.ApplicationType,
                            App.SupplierId,
                            App.BenifitOption,
                            App.ClientId,
                            App.AdvisorIdNumber,
                            App.AdvisorMemberId,
                            ApplicationStatus = AppStatus.Status,
                            App.ApplicationDoc,
                            App.AdviceRecord,
                            App.DisclosureDoc,
                            App.AdvisorAppointment,
                            App.ApplicationId,
                            App.EffectiveStartDate,
                            App.EffectiveEndDate
                        })
                        .Join(db.Suppliers,
                            App => App.SupplierId,
                            Sup => Sup.Id,
                            (App, Sup) => new
                            {
                                App.Month,
                                App.DateRecieved,
                                App.AdvisorFirstName,
                                App.AdvisorLastName,
                                App.ApplicationType,
                                SupplierName = Sup.Name,
                                App.BenifitOption,
                                App.ClientId,
                                App.AdvisorIdNumber,
                                App.AdvisorMemberId,
                                App.ApplicationStatus,
                                App.ApplicationDoc,
                                App.AdviceRecord,
                                App.DisclosureDoc,
                                App.AdvisorAppointment,
                                App.ApplicationId,
                                App.EffectiveStartDate,
                                App.EffectiveEndDate
                            })
                      .Join(db.Contacts,
                           App => App.ClientId,
                           client => client.Id,
                           (App, client) => new
                           {
                               App.Month,
                               App.DateRecieved,
                               App.AdvisorFirstName,
                               App.AdvisorLastName,
                               App.ApplicationType,
                               App.SupplierName,
                               App.BenifitOption,
                               SupplierInitial = client.Initial,
                               SupplierSurname = client.LastName,
                               ClientIdNumber = client.IdNumber,
                               ClientMemberId = App.AdvisorMemberId,
                               Status = App.ApplicationStatus,
                               EnrolmentDate = client.CreatedDate,
                               App.ApplicationStatus,
                               App.ApplicationDoc,
                               App.AdviceRecord,
                               App.DisclosureDoc,
                               App.AdvisorAppointment,
                               App.ApplicationId,
                               App.EffectiveStartDate,
                               App.EffectiveEndDate
                           })
                      .Select(x => new
                      {
                          x.Month,
                          x.DateRecieved,
                          x.AdvisorFirstName,
                          x.AdvisorLastName,
                          x.ApplicationType,
                          Supplier = x.SupplierName,
                          x.BenifitOption,
                          Name = x.SupplierInitial,
                          Surname = x.SupplierSurname,
                          IdNumber = x.ClientIdNumber,
                          MemberId = x.ClientMemberId,
                          x.EnrolmentDate,
                          x.AdvisorAppointment,
                          x.ApplicationDoc,
                          x.AdviceRecord,
                          x.DisclosureDoc,
                          Status = x.ApplicationStatus,
                          x.EffectiveStartDate,
                          x.EffectiveEndDate
                      }).ToListAsync();

                    csv = $"MONTH,DATE RECIEVED,ADVISER NAME,APPLICATION TYPE,SUPPLIER,BENIFIT OPTION,NAME,SURNAME,ID NUMBER,MEMBER ID,ENROLMENT DATE,ADVISER APPOINTMENT RECIEVED,APPLICATION RECIEVED,ADVICE RECORD RECIEVED,DISCLOSURE DOC,STATUS{Environment.NewLine}{string.Concat(from re in data select $"{String.Format("{0:MMM-yyyy}", re.Month)},{String.Format("{0:dd-MMM-yyyy}", re.DateRecieved)},{re.AdvisorFirstName}  {re.AdvisorLastName},{re.ApplicationType},{re.Supplier},{re.BenifitOption},{re.Name},{re.Surname},{re.IdNumber},{re.MemberId},{String.Format("{0:dd-MMM-yyyy}", re.EnrolmentDate)},{re.AdvisorAppointment},{re.ApplicationDoc},{re.AdviceRecord},{re.DisclosureDoc},{re.Status}{Environment.NewLine}")}";
                }
                else
                {
                    var data = await db.Applications
                    .Include(e => e.ApplicationDocuments)
                    .Where(
                        c => c.CreatedDate < dateTo 
                        && c.CreatedDate >= dateFrom 
                        && c.ApplicationSuppliers.FirstOrDefault().SupplierId == ProductId 
                        && c.ApplicationStatus_Id == StatusId
                        && c.Advisor_Id == Id)
                    .Join(db.Advisors,
                        App => App.Advisor_Id,
                        Advi => Advi.Id,
                        (App, Advi) => new
                        {
                            Month = App.CreatedDate,
                            DateRecieved = App.CreatedDate,
                            AdviContactId = Advi.ContactId,
                            ProductId = App.Product_Id,
                            ApplicationTypeId = App.ApplicationType_Id,
                            ApplicationStatusId = App.ApplicationStatus_Id,
                            ApplicationId = App.Id,
                            App.ApplicationNumber,
                            ClientId = App.Client_Id,
                            ApplicationDoc = App.ApplicationDocuments.Where(e => e.Uploaded == true && e.DocumentTypeId == 37).Any(),
                            AdviceRecord = App.ApplicationDocuments.Where(e => e.Uploaded == true && e.DocumentTypeId == 38).Any(),
                            DisclosureDoc = App.ApplicationDocuments.Where(e => e.Uploaded == true && e.DocumentTypeId == 39).Any(),
                            AdvisorAppointment = App.ApplicationDocuments.Where(e => e.Uploaded == true && e.DocumentTypeId == 7).Any(),
                            Advi.EffectiveStartDate,
                            Advi.EffectiveEndDate
                        })
                    .Join(db.Contacts,
                        App => App.AdviContactId,
                        Advi => Advi.Id,
                        (App, Advi) => new
                        {
                            App.Month,
                            App.DateRecieved,
                            AdvisorFirstName = Advi.FirstName,
                            AdvisorLastName = Advi.LastName,
                            AdvisorIdNumber = Advi.IdNumber,
                            AdvisorMemberId = App.ApplicationNumber,
                            App.ProductId,
                            App.ApplicationTypeId,
                            App.ApplicationStatusId,
                            App.ClientId,
                            App.ApplicationId,
                            App.ApplicationDoc,
                            App.AdviceRecord,
                            App.DisclosureDoc,
                            App.AdvisorAppointment,
                            App.EffectiveStartDate,
                            App.EffectiveEndDate
                        })
                    .Join(db.Products,
                        App => App.ProductId,
                        Prod => Prod.Id,
                        (App, Prod) => new
                        {
                            App.Month,
                            App.DateRecieved,
                            App.AdvisorFirstName,
                            App.AdvisorLastName,
                            App.ApplicationTypeId,
                            Prod.SupplierId,
                            BenifitOption = Prod.Name,
                            App.ClientId,
                            App.AdvisorIdNumber,
                            App.AdvisorMemberId,
                            App.ApplicationStatusId,
                            App.ApplicationDoc,
                            App.AdviceRecord,
                            App.DisclosureDoc,
                            App.AdvisorAppointment,
                            App.ApplicationId,
                            App.EffectiveStartDate,
                            App.EffectiveEndDate
                        })
                    .Join(db.ApplicationTypes,
                        App => App.ApplicationTypeId,
                        AppType => AppType.Id,
                        (App, AppType) => new
                        {
                            App.Month,
                            App.DateRecieved,
                            App.AdvisorFirstName,
                            App.AdvisorLastName,
                            ApplicationType = AppType.Title,
                            App.SupplierId,
                            App.BenifitOption,
                            App.ClientId,
                            App.AdvisorIdNumber,
                            App.AdvisorMemberId,
                            App.ApplicationStatusId,
                            App.ApplicationDoc,
                            App.AdviceRecord,
                            App.DisclosureDoc,
                            App.AdvisorAppointment,
                            App.ApplicationId,
                            App.EffectiveStartDate,
                            App.EffectiveEndDate
                        })
                    .Join(db.ApplicationStatuses,
                          App => App.ApplicationStatusId,
                          AppStatus => AppStatus.Id,
                          (App, AppStatus) => new
                          {
                              App.Month,
                              App.DateRecieved,
                              App.AdvisorFirstName,
                              App.AdvisorLastName,
                              App.ApplicationType,
                              App.SupplierId,
                              App.BenifitOption,
                              App.ClientId,
                              App.AdvisorIdNumber,
                              App.AdvisorMemberId,
                              ApplicationStatus = AppStatus.Status,
                              App.ApplicationDoc,
                              App.AdviceRecord,
                              App.DisclosureDoc,
                              App.AdvisorAppointment,
                              App.ApplicationId,
                              App.EffectiveStartDate,
                              App.EffectiveEndDate
                          })
                      .Join(db.Suppliers,
                          App => App.SupplierId,
                          Sup => Sup.Id,
                          (App, Sup) => new
                          {
                              App.Month,
                              App.DateRecieved,
                              App.AdvisorFirstName,
                              App.AdvisorLastName,
                              App.ApplicationType,
                              SupplierName = Sup.Name,
                              App.BenifitOption,
                              App.ClientId,
                              App.AdvisorIdNumber,
                              App.AdvisorMemberId,
                              App.ApplicationStatus,
                              App.ApplicationDoc,
                              App.AdviceRecord,
                              App.DisclosureDoc,
                              App.AdvisorAppointment,
                              App.ApplicationId,
                              App.EffectiveStartDate,
                              App.EffectiveEndDate
                          })
                    .Join(db.Contacts,
                         App => App.ClientId,
                         client => client.Id,
                         (App, client) => new
                         {
                             App.Month,
                             App.DateRecieved,
                             App.AdvisorFirstName,
                             App.AdvisorLastName,
                             App.ApplicationType,
                             App.SupplierName,
                             App.BenifitOption,
                             SupplierInitial = client.Initial,
                             SupplierSurname = client.LastName,
                             ClientIdNumber = client.IdNumber,
                             ClientMemberId = App.AdvisorMemberId,
                             Status = App.ApplicationStatus,
                             EnrolmentDate = client.CreatedDate,
                             App.ApplicationStatus,
                             App.ApplicationDoc,
                             App.AdviceRecord,
                             App.DisclosureDoc,
                             App.AdvisorAppointment,
                             App.ApplicationId,
                             App.EffectiveStartDate,
                             App.EffectiveEndDate
                         })
                    .Select(x => new
                    {
                        x.Month,
                        x.DateRecieved,
                        x.AdvisorFirstName,
                        x.AdvisorLastName,
                        x.ApplicationType,
                        Supplier = x.SupplierName,
                        x.BenifitOption,
                        Name = x.SupplierInitial,
                        Surname = x.SupplierSurname,
                        IdNumber = x.ClientIdNumber,
                        MemberId = x.ClientMemberId,
                        x.EnrolmentDate,
                        x.AdvisorAppointment,
                        x.ApplicationDoc,
                        x.AdviceRecord,
                        x.DisclosureDoc,
                        Status = x.ApplicationStatus,
                        x.EffectiveStartDate,
                        x.EffectiveEndDate
                    })
                    .ToListAsync();
                    csv = $"MONTH,DATE RECIEVED,ADVISER NAME,APPLICATION TYPE,SUPPLIER,BENIFIT OPTION,NAME,SURNAME,ID NUMBER,MEMBER NUMBER,ENROLMENT DATE,ADVISER APPOINTMENT RECIEVED,APPLICATION RECIEVED,ADVICE RECORD RECIEVED,DISCLOSURE DOC,STATUS{Environment.NewLine}{string.Concat(from re in data select $"{String.Format("{0:MMM-yyyy}", re.Month)},{String.Format("{0:dd-MMM-yyyy}", re.DateRecieved)},{re.AdvisorFirstName}  {re.AdvisorLastName},{re.ApplicationType},{re.Supplier},{re.BenifitOption},{re.Name},{re.Surname},{re.IdNumber},{re.MemberId},{String.Format("{0:dd-MMM-yyyy}", re.EnrolmentDate)},{re.AdvisorAppointment},{re.ApplicationDoc},{re.AdviceRecord},{re.DisclosureDoc},{re.Status}{Environment.NewLine}")}";
                }

                var tempFile = new UTF8Encoding().GetBytes(csv);

                await fs.WriteAsync(tempFile, 0, tempFile.Length);
                fs.Close();

                Stream fileStream = File.Open(fileName, FileMode.Open);
                MemoryStream responseStream = new MemoryStream();
                bool fullContent = true;
                if (this.Request.Headers.Range != null)
                {
                    fullContent = false;

                    // Currently we only support a single range.
                    RangeItemHeaderValue range = this.Request.Headers.Range.Ranges.First();

                    // From specified, so seek to the requested position.
                    if (range.From != null)
                    {
                        fileStream.Seek(range.From.Value, SeekOrigin.Begin);

                        // In this case, actually the complete file will be returned.
                        if (range.From == 0 && (range.To == null || range.To >= fileStream.Length))
                        {
                            fileStream.CopyTo(responseStream);
                            fullContent = true;
                        }
                    }
                    if (range.To != null)
                    {
                        // 10-20, return the range.
                        if (range.From != null)
                        {
                            long? rangeLength = range.To - range.From;
                            int length = (int)Math.Min(rangeLength.Value, fileStream.Length - range.From.Value);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                        // -20, return the bytes from beginning to the specified value.
                        else
                        {
                            int length = (int)Math.Min(range.To.Value, fileStream.Length);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                    }
                    // No Range.To
                    else
                    {
                        // 10-, return from the specified value to the end of file.
                        if (range.From != null)
                        {
                            if (range.From < fileStream.Length)
                            {
                                int length = (int)(fileStream.Length - range.From.Value);
                                byte[] buffer = new byte[length];
                                fileStream.Read(buffer, 0, length);
                                responseStream.Write(buffer, 0, length);
                            }
                        }
                    }
                }
                // No Range header. Return the complete file.
                else
                {
                    fileStream.CopyTo(responseStream);
                }
                fileStream.Close();
                responseStream.Position = 0;

                HttpResponseMessage response = new HttpResponseMessage
                {
                    StatusCode = fullContent ? HttpStatusCode.OK : HttpStatusCode.PartialContent,
                    Content = new StreamContent(responseStream)
                };
                return response;
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [Route("Advisor/CompanyDetailsRep/{Id}")]
        public async Task<HttpResponseMessage> CompanyDetailsRep(int Id)
        {
            try
            {

                string userId = "unknown";
                var _user = await HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>()
                    .FindByIdAsync(HttpContext.Current.User.Identity.GetUserId());

                if (_user != null)
                {
                    userId = _user.Id;
                }

                string fileName = (
                    workingFolder + "\\"
                    + userId + "\\"
                    + (DateTime.Now.Ticks.ToString()).Replace(@"/", "-")
                    + "CommissionExport"
                    );

                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }

                if (File.Exists(fileName))
                {
                    Console.WriteLine(fileName + " already exists!");
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                FileStream fs = new FileStream(fileName.Replace(@"\\", @"\"), FileMode.CreateNew);

                var data = await db.Advisors
                    .Where(e => e.AdvisorType_Id == 5)
                    .Join(db.AdvisorTypes,
                    Advi => Advi.AdvisorType_Id,
                    AdviTypes => AdviTypes.Id,
                    (Advi, AdviTypes) => new
                    {
                        FspNo = Advi.FsbCode,
                        RegNo = Advi.RegNumber,
                        DateAuth = Advi.DateAuthorized,
                        Advi.ComplianceOfficer,
                        ComplianceOfficerNo = Advi.ComplianceOffficerNumber,
                        FspType = AdviTypes.Title,
                        Advi.ContactId
                    })
                    .Join(db.Contacts
                    .Where(e => e.Id == Id),
                    Advi => Advi.ContactId,
                    Cont => Cont.Id,
                    (Advi, Cont) => new
                    {
                        Advi.FspType,
                        Advi.FspNo,
                        FspName = Cont.FirstName,
                        Advi.RegNo,
                        Advi.DateAuth,
                        Cont.Cell1,
                        Cont.Cell2,
                        Cont.Tel1,
                        Cont.Tel2,
                        Email1 = Cont.Email,
                        Cont.Email2,
                        Advi.ComplianceOfficer,
                        Advi.ComplianceOfficerNo,
                        ContactId = Cont.Id
                    })
                    .Join(db.Addresses
                    .Include(e => e.Province)
                    .Include(e => e.AddressType)
                    .Include(e => e.Country),
                    Advi => Advi.ContactId,
                    Addr => Addr.Contact_Id,
                    (Advi, Addr) => new
                    {
                        Advi.FspType,
                        Advi.FspNo,
                        Advi.FspName,
                        Advi.RegNo,
                        Advi.DateAuth,
                        AddressType = Addr.AddressType.Name,
                        Advi.Cell1,
                        Advi.Cell2,
                        Advi.Tel1,
                        Advi.Tel2,
                        Advi.Email1,
                        Advi.Email2,
                        Advi.ComplianceOfficer,
                        Advi.ComplianceOfficerNo,
                        Addr.Description,
                        Addr.Street1,
                        Addr.Street2,
                        Addr.Street3,
                        Addr.Suburb,
                        Addr.PostalCode,
                        Addr.City,
                        Addr.MapUrl,
                        Province = Addr.Province.Name,
                        Country = Addr.Country.Name
                    })
                    .Select(
                    x => new
                    {
                        x.FspType,
                        x.FspNo,
                        x.FspName,
                        x.RegNo,
                        x.DateAuth,
                        x.AddressType,
                        x.Cell1,
                        x.Cell2,
                        x.Tel1,
                        x.Tel2,
                        x.Email1,
                        x.Email2,
                        x.ComplianceOfficer,
                        x.ComplianceOfficerNo,
                        x.Description,
                        x.Street1,
                        x.Street2,
                        x.Street3,
                        x.Suburb,
                        x.PostalCode,
                        x.City,
                        x.MapUrl,
                        x.Province,
                        x.Country
                    })
                    .ToListAsync();

                string csv = $"FSP TYPE,FSP NO,FSP NAME,REG NO,DATE AUTH,CELL1,CELL2,TEL1,TEL2,EMAIL1,EMAIL2,COMPLIANCE OFFICER,COMPLIANCE OFFICER NO,ADDRESS TYPE,DESCRIPTION,STREET1,STREET2,STREET3,SUBURB,POSTAL CODE,CITY,MAP URL,PROVINCE,COUNTRY{Environment.NewLine}{string.Concat(from re in data select $"{re.FspType},{re.FspNo},{re.FspName},{re.RegNo},{re.DateAuth},{re.Cell1},{re.Cell2},{re.Tel1},{re.Tel2},{re.Email1},{re.Email2},{re.ComplianceOfficer},{re.ComplianceOfficerNo},{re.AddressType},{re.Description},{re.Street1},{re.Street2},{re.Street3},{re.Suburb},{re.PostalCode},{re.City},{re.MapUrl},{re.Province},{re.Country}{Environment.NewLine}")}";
                var tempFile = new System.Text.UTF8Encoding().GetBytes(csv);

                await fs.WriteAsync(tempFile, 0, tempFile.Length);
                fs.Close();


                MemoryStream responseStream = new MemoryStream();
                Stream fileStream = File.Open(fileName, FileMode.Open);
                bool fullContent = true;
                if (this.Request.Headers.Range != null)
                {
                    fullContent = false;

                    // Currently we only support a single range.
                    RangeItemHeaderValue range = this.Request.Headers.Range.Ranges.First();

                    // From specified, so seek to the requested position.
                    if (range.From != null)
                    {
                        fileStream.Seek(range.From.Value, SeekOrigin.Begin);

                        // In this case, actually the complete file will be returned.
                        if (range.From == 0 && (range.To == null || range.To >= fileStream.Length))
                        {
                            fileStream.CopyTo(responseStream);
                            fullContent = true;
                        }
                    }
                    if (range.To != null)
                    {
                        // 10-20, return the range.
                        if (range.From != null)
                        {
                            long? rangeLength = range.To - range.From;
                            int length = (int)Math.Min(rangeLength.Value, fileStream.Length - range.From.Value);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                        // -20, return the bytes from beginning to the specified value.
                        else
                        {
                            int length = (int)Math.Min(range.To.Value, fileStream.Length);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                    }
                    // No Range.To
                    else
                    {
                        // 10-, return from the specified value to the end of file.
                        if (range.From != null)
                        {
                            if (range.From < fileStream.Length)
                            {
                                int length = (int)(fileStream.Length - range.From.Value);
                                byte[] buffer = new byte[length];
                                fileStream.Read(buffer, 0, length);
                                responseStream.Write(buffer, 0, length);
                            }
                        }
                    }
                }
                // No Range header. Return the complete file.
                else
                {
                    fileStream.CopyTo(responseStream);
                }
                fileStream.Close();
                responseStream.Position = 0;



                HttpResponseMessage response = new HttpResponseMessage
                {
                    StatusCode = fullContent ? HttpStatusCode.OK : HttpStatusCode.PartialContent,
                    Content = new StreamContent(responseStream)
                };
                return response;
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        // TODO: Link application number to this (if needed)
        [Route("Advisor/CompanyDetailsAdvisorsRep")]
        public async Task<HttpResponseMessage> CompanyDetailsAdvisorsRep()
        {
            try
            {

                string userId = "unknown";
                var _user = await HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>()
                    .FindByIdAsync(HttpContext.Current.User.Identity.GetUserId());

                if (_user != null)
                {
                    userId = _user.Id;
                }

                string fileName = (
                    workingFolder + "\\"
                    + userId + "\\"
                    + (DateTime.Now.Ticks.ToString()).Replace(@"/", "-")
                    + "CommissionExport"
                    );

                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }

                if (File.Exists(fileName))
                {
                    Console.WriteLine(fileName + " already exists!");
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                FileStream fs = new FileStream(fileName.Replace(@"\\", @"\"), FileMode.CreateNew);

                var data = await db.Advisors
                    .Where(e => e.AdvisorType_Id == 2)
                    .Join(db.Contacts,
                    Advi => Advi.ContactId,
                    Cont => Cont.Id,
                    (Advi, Cont) => new
                    {
                        AdvisorId = Advi.Id,
                        AdvisorName = Cont.FirstName,
                        AdvisorSurname = Cont.LastName,
                        AdvisorIdNumber = Cont.IdNumber,
                        AdvisorClientId = Cont.Id,
                    })
                    .Select(
                    x => new
                    {
                        x.AdvisorId,
                        x.AdvisorName,
                        x.AdvisorSurname,
                        AdviorIdNumber = x.AdvisorIdNumber,
                    })
                    .OrderBy(e => e.AdvisorId)
                    .ToListAsync();

                string csv = $"ADVISER NAME,ID NUMBER{Environment.NewLine}{string.Concat(from re in data select $"{re.AdvisorName} {re.AdvisorSurname},{re.AdviorIdNumber}{Environment.NewLine}")}";
                var tempFile = new System.Text.UTF8Encoding().GetBytes(csv);

                await fs.WriteAsync(tempFile, 0, tempFile.Length);
                fs.Close();


                MemoryStream responseStream = new MemoryStream();
                Stream fileStream = File.Open(fileName, FileMode.Open);
                bool fullContent = true;
                if (this.Request.Headers.Range != null)
                {
                    fullContent = false;

                    // Currently we only support a single range.
                    RangeItemHeaderValue range = this.Request.Headers.Range.Ranges.First();


                    // From specified, so seek to the requested position.
                    if (range.From != null)
                    {
                        fileStream.Seek(range.From.Value, SeekOrigin.Begin);

                        // In this case, actually the complete file will be returned.
                        if (range.From == 0 && (range.To == null || range.To >= fileStream.Length))
                        {
                            fileStream.CopyTo(responseStream);
                            fullContent = true;
                        }
                    }
                    if (range.To != null)
                    {
                        // 10-20, return the range.
                        if (range.From != null)
                        {
                            long? rangeLength = range.To - range.From;
                            int length = (int)Math.Min(rangeLength.Value, fileStream.Length - range.From.Value);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                        // -20, return the bytes from beginning to the specified value.
                        else
                        {
                            int length = (int)Math.Min(range.To.Value, fileStream.Length);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                    }
                    // No Range.To
                    else
                    {
                        // 10-, return from the specified value to the end of file.
                        if (range.From != null)
                        {
                            if (range.From < fileStream.Length)
                            {
                                int length = (int)(fileStream.Length - range.From.Value);
                                byte[] buffer = new byte[length];
                                fileStream.Read(buffer, 0, length);
                                responseStream.Write(buffer, 0, length);
                            }
                        }
                    }
                }
                // No Range header. Return the complete file.
                else
                {
                    fileStream.CopyTo(responseStream);
                }
                fileStream.Close();
                responseStream.Position = 0;



                HttpResponseMessage response = new HttpResponseMessage
                {
                    StatusCode = fullContent ? HttpStatusCode.OK : HttpStatusCode.PartialContent,
                    Content = new StreamContent(responseStream)
                };
                return response;
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [Route("Advisor/CompanyDetailsKeyIndividualsRep")]
        public async Task<HttpResponseMessage> CompanyDetailsKeyPersonRep()
        {
            try
            {

                string userId = "unknown";
                var _user = await HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>()
                    .FindByIdAsync(HttpContext.Current.User.Identity.GetUserId());

                if (_user != null)
                {
                    userId = _user.Id;
                }

                string fileName = (
                    workingFolder + "\\"
                    + userId + "\\"
                    + (DateTime.Now.Ticks.ToString()).Replace(@"/", "-")
                    + "CommissionExport"
                    );

                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }

                if (File.Exists(fileName))
                {
                    Console.WriteLine(fileName + " already exists!");
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                FileStream fs = new FileStream(fileName.Replace(@"\\", @"\"), FileMode.CreateNew);

                var data = await db.Advisors
                    .Where(e => e.AdvisorType_Id == 1)
                    .Join(db.Contacts,
                    Advi => Advi.ContactId,
                    Cont => Cont.Id,
                    (Advi, Cont) => new
                    {
                        AdvisorName = Cont.FirstName,
                        AdvisorSurname = Cont.LastName,
                        AdvisorIdNumber = Cont.IdNumber,
                    })
                    .Select(
                    x => new
                    {
                        AdvicsorName = x.AdvisorName,
                        x.AdvisorSurname,
                        x.AdvisorIdNumber,

                    })
                    .ToListAsync();

                string csv = $"KEY PERSON NAME,ID NUMBER{Environment.NewLine}{string.Concat(from re in data select $"{re.AdvicsorName} {re.AdvisorSurname},{re.AdvisorIdNumber}{Environment.NewLine}")}";
                var tempFile = new System.Text.UTF8Encoding().GetBytes(csv);

                await fs.WriteAsync(tempFile, 0, tempFile.Length);
                fs.Close();


                MemoryStream responseStream = new MemoryStream();
                Stream fileStream = File.Open(fileName, FileMode.Open);
                bool fullContent = true;
                if (this.Request.Headers.Range != null)
                {
                    fullContent = false;

                    // Currently we only support a single range.
                    RangeItemHeaderValue range = this.Request.Headers.Range.Ranges.First();


                    // From specified, so seek to the requested position.
                    if (range.From != null)
                    {
                        fileStream.Seek(range.From.Value, SeekOrigin.Begin);

                        // In this case, actually the complete file will be returned.
                        if (range.From == 0 && (range.To == null || range.To >= fileStream.Length))
                        {
                            fileStream.CopyTo(responseStream);
                            fullContent = true;
                        }
                    }
                    if (range.To != null)
                    {
                        // 10-20, return the range.
                        if (range.From != null)
                        {
                            long? rangeLength = range.To - range.From;
                            int length = (int)Math.Min(rangeLength.Value, fileStream.Length - range.From.Value);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                        // -20, return the bytes from beginning to the specified value.
                        else
                        {
                            int length = (int)Math.Min(range.To.Value, fileStream.Length);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                    }
                    // No Range.To
                    else
                    {
                        // 10-, return from the specified value to the end of file.
                        if (range.From != null)
                        {
                            if (range.From < fileStream.Length)
                            {
                                int length = (int)(fileStream.Length - range.From.Value);
                                byte[] buffer = new byte[length];
                                fileStream.Read(buffer, 0, length);
                                responseStream.Write(buffer, 0, length);
                            }
                        }
                    }
                }
                // No Range header. Return the complete file.
                else
                {
                    fileStream.CopyTo(responseStream);
                }
                fileStream.Close();
                responseStream.Position = 0;



                HttpResponseMessage response = new HttpResponseMessage
                {
                    StatusCode = fullContent ? HttpStatusCode.OK : HttpStatusCode.PartialContent,
                    Content = new StreamContent(responseStream)
                };
                return response;
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [Route("Advisor/CompanyDetailsLicences")]
        public async Task<HttpResponseMessage> CompanyDetailsLicences()
        {
            try
            {

                string userId = "unknown";
                var _user = await HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>()
                    .FindByIdAsync(HttpContext.Current.User.Identity.GetUserId());

                if (_user != null)
                {
                    userId = _user.Id;
                }

                string fileName = (
                    workingFolder + "\\"
                    + userId + "\\"
                    + (DateTime.Now.Ticks.ToString()).Replace(@"/", "-")
                    + "CommissionExport"
                    );

                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }

                if (File.Exists(fileName))
                {
                    Console.WriteLine(fileName + " already exists!");
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                FileStream fs = new FileStream(fileName.Replace(@"\\", @"\"), FileMode.CreateNew);

                var data = await db.LicenseTypes
                    .Where(e => e.LicenseCategoryId == 1 && (e.SubCategory == "1" || e.SubCategory == "2" || e.SubCategory == "3" || e.SubCategory == "4" || e.SubCategory == "5" || e.SubCategory == "6" || e.SubCategory == "7" || e.SubCategory == "14" || e.SubCategory == "16" || e.SubCategory == "20"))
                    .Select(
                    x => new
                    {
                        LicenceDescription = x.Description
                    })
                    .Distinct()
                    .ToListAsync();

                string csv = $"LICENCE{Environment.NewLine}{string.Concat(from re in data select $"{re.LicenceDescription}{Environment.NewLine}")}";
                var tempFile = new System.Text.UTF8Encoding().GetBytes(csv);

                await fs.WriteAsync(tempFile, 0, tempFile.Length);
                fs.Close();


                MemoryStream responseStream = new MemoryStream();
                Stream fileStream = File.Open(fileName, FileMode.Open);
                bool fullContent = true;
                if (this.Request.Headers.Range != null)
                {
                    fullContent = false;

                    // Currently we only support a single range.
                    RangeItemHeaderValue range = this.Request.Headers.Range.Ranges.First();


                    // From specified, so seek to the requested position.
                    if (range.From != null)
                    {
                        fileStream.Seek(range.From.Value, SeekOrigin.Begin);

                        // In this case, actually the complete file will be returned.
                        if (range.From == 0 && (range.To == null || range.To >= fileStream.Length))
                        {
                            fileStream.CopyTo(responseStream);
                            fullContent = true;
                        }
                    }
                    if (range.To != null)
                    {
                        // 10-20, return the range.
                        if (range.From != null)
                        {
                            long? rangeLength = range.To - range.From;
                            int length = (int)Math.Min(rangeLength.Value, fileStream.Length - range.From.Value);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                        // -20, return the bytes from beginning to the specified value.
                        else
                        {
                            int length = (int)Math.Min(range.To.Value, fileStream.Length);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                    }
                    // No Range.To
                    else
                    {
                        // 10-, return from the specified value to the end of file.
                        if (range.From != null)
                        {
                            if (range.From < fileStream.Length)
                            {
                                int length = (int)(fileStream.Length - range.From.Value);
                                byte[] buffer = new byte[length];
                                fileStream.Read(buffer, 0, length);
                                responseStream.Write(buffer, 0, length);
                            }
                        }
                    }
                }
                // No Range header. Return the complete file.
                else
                {
                    fileStream.CopyTo(responseStream);
                }
                fileStream.Close();
                responseStream.Position = 0;
                HttpResponseMessage response = new HttpResponseMessage
                {
                    StatusCode = fullContent ? HttpStatusCode.OK : HttpStatusCode.PartialContent,
                    Content = new StreamContent(responseStream)
                };
                return response;
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [Route("Advisor/CompanyDetailsDocuments/{Id}")]
        public async Task<HttpResponseMessage> CompanyDetailsDocuments(int Id)
        {
            try
            {

                string userId = "unknown";
                var _user = await HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>()
                    .FindByIdAsync(HttpContext.Current.User.Identity.GetUserId());

                if (_user != null)
                {
                    userId = _user.Id;
                }

                string fileName = (
                    workingFolder + "\\"
                    + userId + "\\"
                    + (DateTime.Now.Ticks.ToString()).Replace(@"/", "-")
                    + "CommissionExport"
                    );

                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }

                if (File.Exists(fileName))
                {
                    Console.WriteLine(fileName + " already exists!");
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                FileStream fs = new FileStream(fileName.Replace(@"\\", @"\"), FileMode.CreateNew);

                var data = await db.Advisors
                    .Where(e => e.ContactId == Id)
                    .Join(db.AdvisorDocuments,
                    Advi => Advi.Id,
                    AdviDoc => AdviDoc.Advisor_Id,
                    (Advi, AdviDoc) => new
                    {
                        AdviDoc.DocumentTypeId,
                        ValidFrom = AdviDoc.ValidFromDate,
                        ValidTo = AdviDoc.ValidToDate,
                        Note = AdviDoc.Title
                    })
                    .Join(db.DocumentTypes,
                    AdviDoc => AdviDoc.DocumentTypeId,
                    AdviDocType => AdviDocType.Id,
                    (AdviDoc, AdviDocType) => new
                    {
                        AdviDocType = AdviDocType.Name,
                        AdviDoc.ValidFrom,
                        AdviDoc.ValidTo,
                        AdviDoc.Note
                    })
                    .Select(
                    x => new
                    {
                        x.AdviDocType,
                        x.ValidFrom,
                        x.ValidTo,
                        x.Note
                    })
                    .ToListAsync();

                string csv = $"DOCUMENT TYPE, VALID FROM , VALID TO, NOTE{Environment.NewLine}{string.Concat(from re in data select $"{re.AdviDocType},{re.ValidFrom},{re.ValidTo},{re.Note}{Environment.NewLine}")}";

                var tempFile = new System.Text.UTF8Encoding().GetBytes(csv);

                await fs.WriteAsync(tempFile, 0, tempFile.Length);
                fs.Close();


                MemoryStream responseStream = new MemoryStream();
                Stream fileStream = File.Open(fileName, FileMode.Open);
                bool fullContent = true;
                if (this.Request.Headers.Range != null)
                {
                    fullContent = false;

                    // Currently we only support a single range.
                    RangeItemHeaderValue range = this.Request.Headers.Range.Ranges.First();


                    // From specified, so seek to the requested position.
                    if (range.From != null)
                    {
                        fileStream.Seek(range.From.Value, SeekOrigin.Begin);

                        // In this case, actually the complete file will be returned.
                        if (range.From == 0 && (range.To == null || range.To >= fileStream.Length))
                        {
                            fileStream.CopyTo(responseStream);
                            fullContent = true;
                        }
                    }
                    if (range.To != null)
                    {
                        // 10-20, return the range.
                        if (range.From != null)
                        {
                            long? rangeLength = range.To - range.From;
                            int length = (int)Math.Min(rangeLength.Value, fileStream.Length - range.From.Value);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                        // -20, return the bytes from beginning to the specified value.
                        else
                        {
                            int length = (int)Math.Min(range.To.Value, fileStream.Length);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                    }
                    // No Range.To
                    else
                    {
                        // 10-, return from the specified value to the end of file.
                        if (range.From != null)
                        {
                            if (range.From < fileStream.Length)
                            {
                                int length = (int)(fileStream.Length - range.From.Value);
                                byte[] buffer = new byte[length];
                                fileStream.Read(buffer, 0, length);
                                responseStream.Write(buffer, 0, length);
                            }
                        }
                    }
                }
                // No Range header. Return the complete file.
                else
                {
                    fileStream.CopyTo(responseStream);
                }
                fileStream.Close();
                responseStream.Position = 0;

                HttpResponseMessage response = new HttpResponseMessage
                {
                    StatusCode = fullContent ? HttpStatusCode.OK : HttpStatusCode.PartialContent,
                    Content = new StreamContent(responseStream)
                };
                return response;
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [Route("Advisor/ExceptionList/")]
        public async Task<HttpResponseMessage> ExceptionList()
        {
            try
            {
                string userId = "unknown";
                var _user = await HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>()
                    .FindByIdAsync(HttpContext.Current.User.Identity.GetUserId());

                if (_user != null)
                {
                    userId = _user.Id;
                }

                string fileName = (
                    workingFolder + "\\"
                    + userId + "\\"
                    + (DateTime.Now.Ticks.ToString()).Replace(@"/", "-")
                    + "CommissionExport"
                    );

                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }

                if (File.Exists(fileName))
                {
                    Console.WriteLine(fileName + " already exists!");
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
                
                FileStream fs = new FileStream(fileName.Replace(@"\\", @"\"), FileMode.CreateNew);

                var data = db.UnmatchedCommissions
                    .Select(x => new
                    {
             
                        x.Initial,
                        x.Surname,
                        IDNumber = x.MemberSearchValue,
                        SupplierName = x.supplierName,
                        Reason = x.Reasons,
                        x.AdvisorName,
                        x.MemberNumber
                    })
                    .ToList();

                string csv = $"ADVISER NAME, INITIAL, SURNAME,ID NUMBER, MEMBER NUMBER, SUPPLIER NAME, REASONS{Environment.NewLine}{string.Concat(from re in data select $"{re.AdvisorName},{re.Initial},{ re.Surname.Replace(',', ' ')},{re.IDNumber},{re.MemberNumber},{re.SupplierName},{re.Reason}{Environment.NewLine}")}";
                var tempFile = new UTF8Encoding().GetBytes(csv);

                await fs.WriteAsync(tempFile, 0, tempFile.Length);
                fs.Close();


                MemoryStream responseStream = new MemoryStream();
                Stream fileStream = File.Open(fileName, FileMode.Open);
                bool fullContent = true;
                if (this.Request.Headers.Range != null)
                {
                    fullContent = false;

                    // Currently we only support a single range.
                    RangeItemHeaderValue range = this.Request.Headers.Range.Ranges.First();


                    // From specified, so seek to the requested position.
                    if (range.From != null)
                    {
                        fileStream.Seek(range.From.Value, SeekOrigin.Begin);

                        // In this case, actually the complete file will be returned.
                        if (range.From == 0 && (range.To == null || range.To >= fileStream.Length))
                        {
                            fileStream.CopyTo(responseStream);
                            fullContent = true;
                        }
                    }
                    if (range.To != null)
                    {
                        // 10-20, return the range.
                        if (range.From != null)
                        {
                            long? rangeLength = range.To - range.From;
                            int length = (int)Math.Min(rangeLength.Value, fileStream.Length - range.From.Value);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                        // -20, return the bytes from beginning to the specified value.
                        else
                        {
                            int length = (int)Math.Min(range.To.Value, fileStream.Length);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                    }
                    // No Range.To
                    else
                    {
                        // 10-, return from the specified value to the end of file.
                        if (range.From != null)
                        {
                            if (range.From < fileStream.Length)
                            {
                                int length = (int)(fileStream.Length - range.From.Value);
                                byte[] buffer = new byte[length];
                                fileStream.Read(buffer, 0, length);
                                responseStream.Write(buffer, 0, length);
                            }
                        }
                    }
                }
                // No Range header. Return the complete file.
                else
                {
                    fileStream.CopyTo(responseStream);
                }
                fileStream.Close();
                responseStream.Position = 0;



                HttpResponseMessage response = new HttpResponseMessage
                {
                    StatusCode = fullContent ? HttpStatusCode.OK : HttpStatusCode.PartialContent,
                    Content = new StreamContent(responseStream)
                };
                return response;
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }


        [Route("Advisor/TotalBusiness/{dateFrom}/{dateTo}")]
        public async Task<HttpResponseMessage> ApplicationsReport(DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            try
            {

                string userId = "unknown";
                var _user = await HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>()
                    .FindByIdAsync(HttpContext.Current.User.Identity.GetUserId());

                if (_user != null)
                {
                    userId = _user.Id;
                }

                string fileName = (
                    workingFolder + "\\"
                    + userId + "\\"
                    + (DateTime.Now.Ticks.ToString()).Replace(@"/", "-")
                    + "CommissionExport"
                    );

                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }

                if (File.Exists(fileName))
                {
                    Console.WriteLine(fileName + " already exists!");
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                FileStream fs = new FileStream(fileName.Replace(@"\\", @"\"), FileMode.CreateNew);

                var data = await (from x in db.Applications
                                  join appSupp in db.ApplicationSuppliers on x.Id equals appSupp.ApplicationId
                                  join supply in db.Suppliers on appSupp.SupplierId equals supply.Id
                                  join contact in db.Contacts on x.Client_Id equals contact.Id
                                  join advisor in db.Advisors on x.Advisor_Id equals advisor.Id
                                  join advisorStatus in db.AdvisorStatuses on advisor.AdvisorStatus_Id equals advisorStatus.Id
                                  where x.Deleted != true && advisorStatus.Id == 5 && x.CreatedDate >= dateFrom && x.CreatedDate <= dateTo
                                  select new ApplicationReportResponse
                                  {
                                      Month = x.CreatedDate,
                                      DateRecieved = x.CreatedDate,
                                      AdvisorFirstName = x.Advisor.Contact.FirstName,
                                      AdvisorLastName = x.Advisor.Contact.LastName,
                                      ApplicationType = x.ApplicationType.Title,
                                      Supplier = appSupp.Supplier.Name,
                                      BenifitOption = x.Product.Name,
                                      Name = contact.FirstName,
                                      Surname = contact.LastName,
                                      IdNumber = contact.IdNumber,
                                      MemberNumber = x.ApplicationNumber,
                                      EnrolmentDate = contact.CreatedDate,
                                      ApplicationDoc = x.ApplicationDocuments.Where(e => e.Uploaded == true && e.DocumentTypeId == 37).Any(),
                                      AdviceRecord = x.ApplicationDocuments.Where(e => e.Uploaded == true && e.DocumentTypeId == 38).Any(),
                                      DisclosureDoc = x.ApplicationDocuments.Where(e => e.Uploaded == true && e.DocumentTypeId == 39).Any(),
                                      AdvisorAppointment = x.ApplicationDocuments.Where(e => e.Uploaded == true && e.DocumentTypeId == 7).Any(),
                                      Status = x.ApplicationStatus.Status
                                  }
                                 ).ToListAsync();

                string csv = $"MONTH,DATE RECIEVED,ADVISER NAME,APPLICATION TYPE,SUPPLIER,BENIFIT OPTION,NAME,SURNAME,ID NUMBER,MEMBER NUMBER,ENROLMENT DATE,ADVISER APPOINTMENT RECIEVED,APPLICATION RECIEVED,ADVICE RECORD RECIEVED,DISCLOSURE DOC,STATUS{Environment.NewLine}{string.Concat(from re in data select $"{String.Format("{0:MMM-yyyy}", re.Month)},{String.Format("{0:dd-MMM-yyyy}", re.DateRecieved)},{re.AdvisorFirstName}  {re.AdvisorLastName},{re.ApplicationType},{re.Supplier},{re.BenifitOption},{re.Name},{re.Surname},{re.IdNumber},{re.MemberNumber},{String.Format("{0:dd-MMM-yyyy}", re.EnrolmentDate)},{re.AdvisorAppointment},{re.ApplicationDoc},{re.AdviceRecord},{re.DisclosureDoc},{re.Status}{Environment.NewLine}")}";

                var tempFile = new System.Text.UTF8Encoding().GetBytes(csv);

                await fs.WriteAsync(tempFile, 0, tempFile.Length);
                fs.Close();

                MemoryStream responseStream = new MemoryStream();
                Stream fileStream = File.Open(fileName, FileMode.Open);
                bool fullContent = true;
                if (this.Request.Headers.Range != null)
                {
                    fullContent = false;
                    // Currently we only support a single range.
                    RangeItemHeaderValue range = this.Request.Headers.Range.Ranges.First();
                    
                    // From specified, so seek to the requested position.
                    if (range.From != null)
                    {
                        fileStream.Seek(range.From.Value, SeekOrigin.Begin);

                        // In this case, actually the complete file will be returned.
                        if (range.From == 0 && (range.To == null || range.To >= fileStream.Length))
                        {
                            fileStream.CopyTo(responseStream);
                            fullContent = true;
                        }
                    }
                    if (range.To != null)
                    {
                        // 10-20, return the range.
                        if (range.From != null)
                        {
                            long? rangeLength = range.To - range.From;
                            int length = (int)Math.Min(rangeLength.Value, fileStream.Length - range.From.Value);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                        // -20, return the bytes from beginning to the specified value.
                        else
                        {
                            int length = (int)Math.Min(range.To.Value, fileStream.Length);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                    }
                    // No Range.To
                    else
                    {
                        // 10-, return from the specified value to the end of file.
                        if (range.From != null)
                        {
                            if (range.From < fileStream.Length)
                            {
                                int length = (int)(fileStream.Length - range.From.Value);
                                byte[] buffer = new byte[length];
                                fileStream.Read(buffer, 0, length);
                                responseStream.Write(buffer, 0, length);
                            }
                        }
                    }
                }
                // No Range header. Return the complete file.
                else
                {
                    fileStream.CopyTo(responseStream);
                }
                fileStream.Close();
                responseStream.Position = 0;

                HttpResponseMessage response = new HttpResponseMessage
                {
                    StatusCode = fullContent ? HttpStatusCode.OK : HttpStatusCode.PartialContent,
                    Content = new StreamContent(responseStream)
                };
                return response;
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [Route("Advisor/{advisorId}/CommissionTransactionRep/{dateFrom}/{dateTo}")]
        public async Task<HttpResponseMessage> CommissionTransactionRep(int advisorId, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            try
            {
                string userId = "unknown";
                var _user = await HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>()
                    .FindByIdAsync(HttpContext.Current.User.Identity.GetUserId());

                if (_user != null)
                {
                    userId = _user.Id;
                }

                string fileName = (
                    workingFolder + "\\"
                    + userId + "\\"
                    + (DateTime.Now.Ticks.ToString()).Replace(@"/", "-")
                    + "CommissionExport"
                    );

                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }

                if (File.Exists(fileName))
                {
                    Console.WriteLine(fileName + " already exists!");
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                FileStream fs = new FileStream(fileName.Replace(@"\\", @"\"), FileMode.CreateNew);
                
                var applicationHistory = await db.ApplicationAdvisorHistory
                    .Where(e => e.DateStarted <= dateTo && ((e.DateEnded != null) ? e.DateEnded >= dateTo : true))
                    .OrderBy(e => e.DateStarted)
                    .GroupBy(e => e.Application_Id)
                    .ToListAsync();

                var appsHist = new List<ApplicationAdvisorHistory>();

                foreach (var item in applicationHistory)
                {
                    appsHist.Add(item.Last());
                }

                var adviHist = (from x in appsHist
                                join y in db.Applications on x.Application_Id equals y.Id
                                join o in db.CommissionStatement on y.Client_Id equals o.ClientId
                                join w in db.Products on o.SupplierId equals w.SupplierId
                                join z in db.Suppliers on w.SupplierId equals z.Id
                                where z.Id == o.SupplierId && y.Product_Id == o.ProductId
                                select new
                                {
                                    appID = y.Id,
                                    prodId = w.Id,
                                    clientId = y.Client_Id,
                                    supId = z.Id,
                                    commId = o.Id
                                })
                                .Distinct()
                                .AsEnumerable();

                var suppliersShares = (from x in db.Suppliers
                                       join y in db.AdvisorShareUnderSupervisions
                                        .Where(e => e.AdvisorId == advisorId)
                                       on x.Id equals y.SupplierId
                                       select new
                                       {
                                           share = (decimal?)(int?)y.Share,
                                           SupId = x.Id,
                                           AdivId = y.AdvisorId
                                       })
                                       .Where(e => e.share != null)
                                       .Distinct()
                                       .GroupBy(e => e.AdivId)
                                       .AsEnumerable();

                var comms = db.CommissionStatement
                    .Where(e =>
                            e.ApprovalStatus == "APPROVED" &&
                            e.ApprovalDateFrom >= dateFrom &&
                            e.ApprovalDateTo <= dateTo &&
                            e.AdvisorId == advisorId)
                    .ToList();
                
                var commses = (from c in comms
                               select new
                               {
                                   c.Id,
                                   c.Surname,
                                   c.AdvisorId,
                                   c.MemberNumber,
                                   c.MemberSearchKey,
                                   c.TransactionDate,
                                   c.SupplierId,
                                   c.Initial,
                                   c.CommissionExclVAT,
                                   c.CommissionInclVAT,
                                   c.AdvisorTax,
                                   c.AdvisorTaxRate,
                                   c.ClientId,
                                   c.ProductId,
                                   AdvisorVat = c.AdvisorTaxRate ,
                                   Date = c.TransactionDate
                               })
                               .OrderBy(v => v.MemberNumber)
                               .Distinct()
                               .AsEnumerable();

                var data = (from commission in commses
                            join supplier in db.Suppliers
                                on commission.SupplierId equals supplier.Id
                            join history in adviHist
                                on commission.ClientId equals history.clientId
                            join shares in suppliersShares
                                on commission.AdvisorId equals shares.Key
                            join app in db.Applications
                                .Where(e => e.Advisor_Id == advisorId)
                                on commission.ClientId equals app.Client_Id //h.appID equals app.Id
                            join Apptype in db.ApplicationTypes on
                                app.ApplicationType_Id equals Apptype.Id
                            join client in db.Contacts
                                on commission.ClientId equals client.Id
                            select new
                            {
                                appID = commission.ProductId,
                                id = commission.Id,
                                surname = commission.Surname,
                                initial = commission.Initial,
                                memberSearchKey = commission.MemberSearchKey,
                                transactionDate = commission.TransactionDate,
                                advisorCommission = (
                                    commission.CommissionExclVAT * (shares
                                    .FirstOrDefault(e => e.SupId == commission.SupplierId).share / 100)),
                                companyCommission = (
                                    commission.CommissionExclVAT * (1.00M - ((shares
                                    .FirstOrDefault(e => e.SupId == commission.SupplierId).share) / 100))),
                                commissionExclVAT = commission.CommissionExclVAT,
                                commissionInclVAT = commission.CommissionInclVAT,
                                advisorTax = (commission.AdvisorTax),
                                advisorTaxRate = (commission.AdvisorTaxRate),
                                advisorId = commission.AdvisorId,
                                clientId = commission.ClientId,
                                productId = commission.ProductId,
                                supplierName = supplier.Name,
                                supplierId = supplier.Id,
                                supplier = supplier.Name,
                                client = client.FirstName + " " + client.LastName,
                                client.IdNumber,
                                memberNumber = app.ApplicationNumber,
                                date = commission.TransactionDate,
                                comm = (commission.CommissionExclVAT * (((shares.FirstOrDefault(e => e.SupId == commission.SupplierId).share == null) ? 0.0M : shares.FirstOrDefault(e => e.SupId == commission.SupplierId).share) / 100)),
                                applicationType = Apptype.Title,
                            })
                            .OrderBy(c => c.date)
                            .OrderBy(c => c.surname)
                            .Distinct()
                            .GroupBy(e => e.supplierId)
                            .ToList();

                string csv = "";

                if (data.Count == 0)
                {
                    csv = $"CLIENT, CLIENT ID ,MEMBER NUMBER, DATE, SUPPLIER, APP TYPE, COMMISSION";
                }
                else
                {
                    decimal? TOTALCOMM = 0.0M;

                    string specifier = "F";
                    CultureInfo culture = CultureInfo.CreateSpecificCulture("en-CA");

                    string concatString = "";
                    foreach (var item in data)
                    {
                        concatString += string.Concat(from re in item
                                                      select $"{re.client.Replace(',', ' ')}" +
                                                       $",{re.memberSearchKey}" +
                                                       $",{re.memberNumber}" +
                                                       $",{String.Format("{0:dd-MMM-yyyy}", re.date)}" +
                                                       $",{re.supplier}" +
                                                       $",{re.applicationType}" +
                                                       $",{re.comm.Value.ToString(specifier, culture)}" +
                                                       $"{Environment.NewLine}"
                             );
                        concatString += string.Concat(
                            $"{Environment.NewLine}TOTAL COMISSION FOR {item.FirstOrDefault().supplier.ToUpper()},,,,,{ item.Sum(e => e.comm).Value.ToString(specifier, culture)}{Environment.NewLine}{Environment.NewLine}");
                        TOTALCOMM += item.Sum(e => e.comm);
                    };
                    concatString += string.Concat($"{Environment.NewLine}TOTAL COMISSION,,,,,{TOTALCOMM.Value.ToString(specifier, culture)}");
                    csv = $"CLIENT, CLIENT ID, MEMBER NUMBER, DATE, SUPPLIER, APP TYPE, COMMISSION{Environment.NewLine}{concatString}{Environment.NewLine}";
                }

                var tempFile = new System.Text.UTF8Encoding().GetBytes(csv);

                await fs.WriteAsync(tempFile, 0, tempFile.Length);
                fs.Close();


                MemoryStream responseStream = new MemoryStream();
                Stream fileStream = File.Open(fileName, FileMode.Open);
                bool fullContent = true;
                if (this.Request.Headers.Range != null)
                {
                    fullContent = false;

                    // Currently we only support a single range.
                    RangeItemHeaderValue range = this.Request.Headers.Range.Ranges.First();


                    // From specified, so seek to the requested position.
                    if (range.From != null)
                    {
                        fileStream.Seek(range.From.Value, SeekOrigin.Begin);

                        // In this case, actually the complete file will be returned.
                        if (range.From == 0 && (range.To == null || range.To >= fileStream.Length))
                        {
                            fileStream.CopyTo(responseStream);
                            fullContent = true;
                        }
                    }
                    if (range.To != null)
                    {
                        // 10-20, return the range.
                        if (range.From != null)
                        {
                            long? rangeLength = range.To - range.From;
                            int length = (int)Math.Min(rangeLength.Value, fileStream.Length - range.From.Value);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                        // -20, return the bytes from beginning to the specified value.
                        else
                        {
                            int length = (int)Math.Min(range.To.Value, fileStream.Length);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                    }
                    // No Range.To
                    else
                    {
                        // 10-, return from the specified value to the end of file.
                        if (range.From != null)
                        {
                            if (range.From < fileStream.Length)
                            {
                                int length = (int)(fileStream.Length - range.From.Value);
                                byte[] buffer = new byte[length];
                                fileStream.Read(buffer, 0, length);
                                responseStream.Write(buffer, 0, length);
                            }
                        }
                    }
                }
                // No Range header. Return the complete file.
                else
                {
                    fileStream.CopyTo(responseStream);
                }
                fileStream.Close();
                responseStream.Position = 0;



                HttpResponseMessage response = new HttpResponseMessage
                {
                    StatusCode = fullContent ? HttpStatusCode.OK : HttpStatusCode.PartialContent,
                    Content = new StreamContent(responseStream)
                };
                return response;
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [Route("Advisor/{Id}/ExceptionListDelete")]
        public async Task<IHttpActionResult> ExceptionListDelete(int Id)
        {
            try
            {
                bool checkId = false;
                string ResponseMessage = "";
                bool checkApplication = false;

                var Ids = await db.UnmatchedCommissions
                    .Where(e => e.Id == Id)
                    .Join(db.Contacts,
                    UMC => UMC.MemberNumber,
                    Cont => Cont.IdNumber,
                    (UMC, Cont) => new
                    {
                        Supplier = UMC.supplierName,
                        Client_Id = Cont.Id,
                    })
                .Join(db.Suppliers,
                UMC => UMC.Supplier,
                Sup => Sup.Name,
                (UMC, Sup) => new
                {
                    UMC.Client_Id,
                    Supplier_Id = Sup.Id
                })
                .Join(db.Products,
                UMC => UMC.Supplier_Id,
                Prod => Prod.SupplierId,
                (UMC, Prod) => new
                {
                    UMC.Client_Id,
                    Product_Id = Prod.Id
                }).ToListAsync();

                if (Ids.Count > 0)
                {


                    checkId = true;
                    int ProductId = Ids.FirstOrDefault().Product_Id;
                    int ClientId = Ids.FirstOrDefault().Client_Id;

                    checkApplication = db.Applications.Where(e => e.Client_Id == ClientId
                    && e.Product_Id == ProductId && e.Deleted == false).Any();

                }
                else
                {
                    ResponseMessage = "Contact does not exist";
                    return Ok(ResponseMessage);
                }



                if (checkApplication == false)
                {
                    ResponseMessage = "No application linked to client";
                    return Ok(ResponseMessage);
                }

                if (checkId && checkApplication)
                {
                    var UMC = db.UnmatchedCommissions.Where(e => e.Id == Id).FirstOrDefault();
                    db.UnmatchedCommissions.Remove(UMC);
                    db.SaveChanges();
                    ResponseMessage = "DELETED";
                }

                return Ok(ResponseMessage);


            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [Route("Advisor/DeleteDupAdviDocs/{Id}/{AdviId}/{DocType}")]
        public async Task<IHttpActionResult> DeleteDupAdviDocs(int Id, int AdviId, int DocType)
        {
            try
            {
                int DocsCount = await db.AdvisorDocuments
                    .Where(e => e.DocumentTypeId == DocType && e.Advisor_Id == AdviId)
                    .CountAsync();

                if (DocsCount > 1)
                {
                    var AdviDoc = await db.AdvisorDocuments.FirstOrDefaultAsync(e => e.Id == Id);
                    AdviDoc.Deleted = true;
                    await db.SaveChangesAsync();

                    return Ok("Deleted");
                }

                return NotFound();

            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [ResponseType(typeof(Advisor))]
        [Route("Advisor/AdvisorByApplicationId/{id}")]
        public async Task<IHttpActionResult> GetAdvisorByApplicationId(int id)
        {
            try
            {
                var response = await db.Applications
                     .Where(e => e.Id == id)
                     .Select(match => match.Advisor_Id)
                     .FirstOrDefaultAsync();

                return Ok(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        [Route("Advisor/MailAdvisorComission/{id}/{dateFrom}/{dateTo}")]
        public async Task<IHttpActionResult> MailAdvisorComission(int Id, DateTime? dateFrom = null, DateTime? dateTo = null)
        {

            try
            {

                string userId = "unknown";
                var _user = await HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>()
                    .FindByIdAsync(HttpContext.Current.User.Identity.GetUserId());

                if (_user != null)
                {
                    userId = _user.Id;
                }

                string fileName = (
                    workingFolder + "\\"
                    + userId + "\\"
                    + (DateTime.Now.Ticks.ToString()).Replace(@"/", "-")
                    + "CommissionExport.csv"
                    );

                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }

                if (File.Exists(fileName))
                {
                    Console.WriteLine(fileName + " already exists!");
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                FileStream fs = new FileStream(fileName.Replace(@"\\", @"\"), FileMode.CreateNew);

                var applicationHistory = db.ApplicationAdvisorHistory.Where(
                                                   e => e.DateStarted <= dateTo &&
                                                  ((e.DateEnded != null) ? e.DateEnded >= dateTo : true))
                                                  .OrderBy(e => e.DateStarted)
                                                  .GroupBy(e => e.Application_Id)
                                                  .ToList()
                                                  .Distinct();


                var appsHist = new List<ApplicationAdvisorHistory>();

                foreach (var item in applicationHistory)
                {
                    appsHist.Add(item.Last());
                }


                var adviHist = (from x in appsHist
                                join y in db.Applications on x.Application_Id equals y.Id
                                join o in db.CommissionStatement on y.Client_Id equals o.ClientId
                                join w in db.Products on o.SupplierId equals w.SupplierId
                                join z in db.Suppliers on w.SupplierId equals z.Id
                                where z.Id == o.SupplierId && y.Product_Id == o.ProductId
                                select new
                                {
                                    appID = y.Id,
                                    prodId = w.Id,
                                    ClientId = y.Client_Id,
                                    SupId = z.Id,
                                    CommId = o.Id
                                })
                                .Distinct()
                                .AsEnumerable();

                var suppliersShares = (from x in db.Suppliers
                                       join y in db.AdvisorShareUnderSupervisions.Where(e => e.AdvisorId == Id)
                                       on x.Id equals y.SupplierId
                                       select new
                                       {
                                           share = (decimal?)(int?)y.Share,
                                           SupId = x.Id,
                                           AdivId = y.AdvisorId
                                       })
                                       .Where(e => e.share != null)
                                       .Distinct()
                                       .GroupBy(e => e.AdivId)
                                       .AsEnumerable();

                var comms = db.CommissionStatement.Where(e =>
                                 e.ApprovalStatus == "APPROVED" &&
                                 e.ApprovalDateFrom >= dateFrom &&
                                 e.ApprovalDateTo <= dateTo
                                 && e.AdvisorId == Id)
                                 .ToList();

                var data = (from commission in comms
                            join supplier in db.Suppliers
                                on commission.SupplierId equals supplier.Id
                            join history in adviHist
                                on commission.Id equals history.CommId
                            join shares in suppliersShares
                                on commission.AdvisorId equals shares.Key
                            join app in db.Applications
                                .Where(e => e.Advisor_Id == Id)
                                on commission.ClientId equals app.Client_Id
                            join Apptype in db.ApplicationTypes
                                on app.ApplicationType_Id equals Apptype.Id
                            join Client in db.Contacts
                                on commission.ClientId equals Client.Id
                            select new
                            {
                                commission.Surname,
                                commission.Id,
                                commission.Initial,
                                commission.MemberNumber,
                                commission.MemberSearchKey,
                                commission.TransactionDate,
                                AdvisorCommission = (commission.CommissionExclVAT * (shares.FirstOrDefault(e => e.SupId == commission.SupplierId).share / 100)),
                                CompanyCommission = (commission.CommissionExclVAT * (1.00M - ((shares.FirstOrDefault(e => e.SupId == commission.SupplierId).share) / 100))),
                                commission.CommissionExclVAT,
                                commission.CommissionInclVAT,
                                AdvisorTax = (commission.AdvisorTax),
                                AdvisorTaxRate = (commission.AdvisorTaxRate),
                                commission.AdvisorId,
                                commission.ClientId,
                                commission.ProductId,
                                supplierName = supplier.Name,
                                SupplierId = supplier.Id,
                                Client = Client.FirstName + " " + Client.LastName,
                                Client.IdNumber,
                                MemberId = app.ApplicationNumber,
                                Date = commission.TransactionDate,
                                Comm = (commission.CommissionExclVAT * (((shares.FirstOrDefault(e => e.SupId == commission.SupplierId).share == null) ? 0.0M : shares.FirstOrDefault(e => e.SupId == commission.SupplierId).share) / 100)),
                                ApplicationType = Apptype.Title
                            })
                            .OrderBy(c => c.Date)
                            .OrderBy(c => c.Surname)
                            .Distinct()
                            .GroupBy(e => e.SupplierId)
                            .ToList();
                
                string csv = "";

                if (data.Count == 0)
                {
                    csv = $"CLIENT,CLIENT ID,MEMBER ID,DATE,SUPPLIER,APP TYPE,COMM";
                }
                else
                {
                    decimal? TOTALCOMM = 0.0M;

                    string specifier = "F";
                    CultureInfo culture = CultureInfo.CreateSpecificCulture("en-CA");

                    string concatString = "";
                    foreach (var item in data)
                    {
                        concatString += string.Concat(from re in item
                                                      select $"{re.Client.Replace(',', ' ')}" +
                                                       $",{re.IdNumber}" +
                                                       $",{re.MemberId}" +
                                                       $",{String.Format("{0:dd-MMM-yyyy}", re.Date)}" +
                                                       $",{re.supplierName}" +
                                                       $",{re.ApplicationType}" +
                                                       $",{re.Comm.Value.ToString(specifier, culture)}" +
                                                       $"{Environment.NewLine}"
                             );
                        concatString += string.Concat($"{Environment.NewLine}TOTAL COMISSION FOR {item.FirstOrDefault().supplierName.ToUpper()},,,,,{ item.Sum(e => e.Comm).Value.ToString(specifier, culture)}{Environment.NewLine}{Environment.NewLine}");
                        TOTALCOMM += item.Sum(e => e.Comm);
                    };
                    concatString += string.Concat($"{Environment.NewLine}TOTAL COMISSION,,,,,{TOTALCOMM.Value.ToString(specifier, culture)}");
                    csv = $"CLIENT,CLIENT ID,MEMBER ID,DATE,SUPPLIER,APP TYPE,COMM{Environment.NewLine}{concatString}{Environment.NewLine}";
                }

                var tempFile = new System.Text.UTF8Encoding().GetBytes(csv);
                await fs.WriteAsync(tempFile, 0, tempFile.Length);
                fs.Close();

                MemoryStream responseStream = new MemoryStream();
                Stream fileStream = File.Open(fileName, FileMode.Open);
                bool fullContent = true;
                if (this.Request.Headers.Range != null)
                {
                    fullContent = false;

                    // Currently we only support a single range.
                    RangeItemHeaderValue range = this.Request.Headers.Range.Ranges.First();


                    // From specified, so seek to the requested position.
                    if (range.From != null)
                    {
                        fileStream.Seek(range.From.Value, SeekOrigin.Begin);

                        // In this case, actually the complete file will be returned.
                        if (range.From == 0 && (range.To == null || range.To >= fileStream.Length))
                        {
                            fileStream.CopyTo(responseStream);
                            fullContent = true;
                        }
                    }
                    if (range.To != null)
                    {
                        // 10-20, return the range.
                        if (range.From != null)
                        {
                            long? rangeLength = range.To - range.From;
                            int length = (int)Math.Min(rangeLength.Value, fileStream.Length - range.From.Value);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                        // -20, return the bytes from beginning to the specified value.
                        else
                        {
                            int length = (int)Math.Min(range.To.Value, fileStream.Length);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                    }
                    // No Range.To
                    else
                    {
                        // 10-, return from the specified value to the end of file.
                        if (range.From != null)
                        {
                            if (range.From < fileStream.Length)
                            {
                                int length = (int)(fileStream.Length - range.From.Value);
                                byte[] buffer = new byte[length];
                                fileStream.Read(buffer, 0, length);
                                responseStream.Write(buffer, 0, length);
                            }
                        }
                    }
                }
                // No Range header. Return the complete file.
                else
                {
                    fileStream.CopyTo(responseStream);
                }
                fileStream.Close();
                responseStream.Position = 0;

                HttpResponseMessage response = new HttpResponseMessage
                {
                    StatusCode = fullContent ? HttpStatusCode.OK : HttpStatusCode.PartialContent,
                    Content = new StreamContent(responseStream)
                };

                int AdviContId = db.Advisors.FirstOrDefault(e => e.Id == Id).ContactId;
                string AdviEmail = db.Contacts.FirstOrDefault(e => e.Id == AdviContId).Email;

                SmtpClient smtpClient = new SmtpClient
                {
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(ConfigurationSettings.AppSettings["smtpuser"], ConfigurationSettings.AppSettings["smtppassword"]),
                    Port = 587,
                    Host = "smtp.office365.com",
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = true
                };

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(ConfigurationSettings.AppSettings["smtpuser"])
                };
                Attachment attachment = new Attachment(fileName, "text/csv");
                mail.Attachments.Add(attachment);
                mail.Subject = "Adviser commission";
                mail.IsBodyHtml = true;
                mail.Body = "<p>Good day, <br> <br>" +
                "Enclosed your summary of commissions payable.  Please send any queries to me for correction on the next payment. <br><br>" +
                "You are welcome to contact me should you require any further assistance <br> <br>" +
                "Kind regards <br>" +
                "<br>" +
                "Christine Edwards" +
                "</p> <br>";
                mail.CC.Add(new MailAddress("Christine@tendaonline.co.za"));
                mail.To.Add(new MailAddress(AdviEmail));
                smtpClient.Send(mail);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        [Route("Advisor/PostApplicationAdvisorHisotry/{applicationId}/{NewAdvisor}/{OldAdvisor}/{effectiveStartDate}/{effectiveEndDate}")]
        public async Task<IHttpActionResult> Postapplicationadvisorhisotry(int applicationId, int NewAdvisor, int OldAdvisor, DateTime effectiveStartDate, DateTime? effectiveEndDate)
        {
            var exceptionList = 
                from ex in db.UnmatchedCommissions
                join cont in db.Contacts on ex.MemberSearchValue equals cont.IdNumber
                join app in db.Applications on cont.Id equals app.Client_Id
                join adv in db.Advisors on app.Advisor_Id equals adv.Id
                where app.Id == applicationId && ex.MemberSearchValue == cont.IdNumber
                select new
                {
                    exceptionId = ex.Id,
                    adviserFullname = adv.User.FullName,
                    IDNumber = cont.IdNumber
                };

            if(exceptionList.Count()> 0)
            {
                // Update un-matched commissions
                // Get exception list 
                var unMacthedCommission = db.UnmatchedCommissions.Find(exceptionList.FirstOrDefault().exceptionId);

                if(unMacthedCommission != null)
                {
                    unMacthedCommission.AdvisorName = exceptionList.FirstOrDefault().adviserFullname;
                }

            }
            // Check if Application is in unmatched commissions 
            // if (db.UnmatchedCommissions.Where(c => c.MemberSearchValue == currentApplication.Client_Id))

            var currentApplication = db.Applications.FirstOrDefault(e => e.Id == applicationId);

            try
            {
                var HistoryFromApplication =
                    db.ApplicationAdvisorHistory
                    .Where(e => e.Application_Id == applicationId)
                    .OrderBy(e => e.DateStarted)
                    .ToList();

                var applicationadvisorhistory = new ApplicationAdvisorHistory
                {
                    New_Advisor = NewAdvisor,
                    Old_Advisor = OldAdvisor,
                    DateStarted = effectiveStartDate == null ? DateTime.Now : effectiveStartDate,
                    DateEnded = effectiveEndDate,
                    Application_Id = applicationId,
                };
                
                // Add applciation 
                db.ApplicationAdvisorHistory.Add(applicationadvisorhistory);
                await db.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return Ok();
        }

        [HttpPut]
        [Route("Advisor/PutApplicationAdvisorHistory/{applicationId}/{effectiveStartDate}/{effectiveEndDate}/{advisorId}")]
        public async Task<IHttpActionResult> PutApplicationAdvisorHistory(int applicationId, DateTime effectiveStartDate, DateTime? effectiveEndDate, int advisorId)
        {
            try
            {
                ApplicationAdvisorHistory HistoryFromApplication =
                    db.ApplicationAdvisorHistory
                    .Where(e => e.Application_Id == applicationId).ToList().LastOrDefault();

                Boolean addNewEntry = false;

                //Update ApplicationAdviserHistory
                if (HistoryFromApplication != null)
                {
                    HistoryFromApplication.DateStarted = effectiveStartDate;
                    HistoryFromApplication.DateEnded = effectiveEndDate;
                    
                    if (HistoryFromApplication.New_Advisor != advisorId && advisorId != 0)
                    {
                        addNewEntry = true;
                    }
                    db.Entry(HistoryFromApplication).State = EntityState.Modified;
                }

                if (addNewEntry)
                {
                    ApplicationAdvisorHistory newEntry = new ApplicationAdvisorHistory
                    {
                        DateStarted = effectiveStartDate  == null ? DateTime.Now : effectiveStartDate,
                        DateEnded = null,
                        New_Advisor = advisorId,
                        Old_Advisor = HistoryFromApplication.Old_Advisor,
                        Application_Id = applicationId,
                    };
                    db.ApplicationAdvisorHistory.Add(newEntry);
                }

                await db.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return Ok();
        }
    }
}
