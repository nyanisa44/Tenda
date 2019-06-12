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
    [Authorize]
    //[RoutePrefix("contacts")]
    public class ContactsController : BaseApiController
    {
        // GET: api/Contacts
        public async Task<IHttpActionResult> GetContacts()
        {
            var contacts = await db.Contacts.Select(x => new
                    {
                       x.Id,
                       x.FirstName,
                       x.LastName,
                       x.IdNumber,
                    }).OrderBy(c => c.LastName).ThenBy(c=>c.FirstName).ToListAsync();

            return Ok(contacts);
        }

        // GET: api/Contacts/SAId
        [Route("Contacts/SAId/{saId}")]
        public IQueryable<Contact> GetContactsBySAId(string saId)
        {
            return db.Contacts.Include(c => c.Addresses).Where(c => c.IdNumber.Contains(saId)).Take(10);
        }

        // GET: api/Contacts/5
        [ResponseType(typeof(Contact))]
        [Route("Contacts/{id}")]
        public async Task<IHttpActionResult> GetContact(int id)
        {
            if (!this.ContactExists(id))
            {
                return null;
            }

            var contact = await db.Contacts
                  .Where(p => p.Id == id)
                  .Include(c => c.Addresses)
                  .Include(c => c.Addresses.Select(a => a.Province))
                  .Include(c => c.Addresses.Select(a => a.Country))
                  .Include(c => c.Addresses.Select(a => a.AddressType))
                  .Include(c => c.ContactType)
                  .Include(c=>c.ContactTitle)
                  .OrderBy(a => a.Id)
                  .Select(c => c)
                  .SingleOrDefaultAsync();

            return Ok(contact);
        }

        // GET: api/Contacts/5
        [ResponseType(typeof(Contact))]
        [Route("ContactsClients/{id}")]
        public async Task<IHttpActionResult> GetContactClients(int id)
        {
            if (!ContactExists(id))
            {
                return null;
            }
            
            try
            {
                var contact = await db.Contacts.Where(c => c.Id == id ).FirstOrDefaultAsync();
                var contactAddress_Id = contact.Id;
                var address = await db.Addresses.Where(a => a.Contact_Id == contactAddress_Id).FirstOrDefaultAsync();
                ClientsResponse contactClientResponse = new ClientsResponse();

                //add for contact 
                if(contact != null)
                {
                    contactClientResponse.Cell1 = contact.Cell1;
                    contactClientResponse.Cell2 = contact.Cell2;
                    contactClientResponse.ContactId = contact.Id;
                    contactClientResponse.Email = contact.Email;
                    contactClientResponse.Email2 = contact.Email2;
                    contactClientResponse.FirstName = contact.FirstName;
                    contactClientResponse.LastName = contact.LastName;
                    contactClientResponse.MiddleName = contact.MiddleName;
                    contactClientResponse.Tel1 = contact.Tel1;
                    contactClientResponse.Tel2 = contact.Tel2;
                    contactClientResponse.JobTitle = contact.JobTitle;
                    contactClientResponse.IdNumber = contact.IdNumber;
                    contactClientResponse.ContactTitle_Id = contact.ContactTitle_Id.HasValue ? contact.ContactTitle_Id.Value : 1;
                    contactClientResponse.ContactTitle_Name = db.ContactTitles.FirstOrDefault(x => x.Id == contactClientResponse.ContactTitle_Id).Name;
                }

                if(address != null)
                {
                    contactClientResponse.MapUrl = address.MapUrl;
                    contactClientResponse.PostalCode = address.PostalCode;
                    contactClientResponse.Province_Id = address.Province_Id.HasValue ? address.Province_Id.Value : 1;
                    contactClientResponse.Province = db.Provinces.FirstOrDefault(x => x.Id == contactClientResponse.Province_Id).Name;
                    contactClientResponse.Street1 = address.Street1;
                    contactClientResponse.Street2 = address.Street2;
                    contactClientResponse.Street3 = address.Street3;
                    contactClientResponse.Suburb = address.Suburb;
                    contactClientResponse.City = address.City;
                    contactClientResponse.Country_id = address.Country_Id.HasValue ? address.Country_Id.Value : 1;
                    contactClientResponse.Country = db.Countries.FirstOrDefault(x => x.Id == contactClientResponse.Country_id).Name;
                    contactClientResponse.AddressTyper_Id = address.AddressType_Id.HasValue ? address.AddressType_Id.Value : 1;
                    contactClientResponse.AddressType = db.AddressTypes.FirstOrDefault(x => x.Id == contactClientResponse.AddressTyper_Id).Name;
                    contactClientResponse.AddressDescription = address.Description;
                }
                return Ok(contactClientResponse);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // GET: api/Contacts/nameSearch
        [Route("Contacts/Basic/{nameSearch}")]
        public async Task<IHttpActionResult> GetBasicContact(string nameSearch)
        {
            if (nameSearch == "undefined")
            {
                return NotFound();
            }
           
            List<BasicContactResponse> clients = new List<BasicContactResponse>();
            if (nameSearch == "")
            {
                clients = await db.Contacts
                               .Take(10)
                               .Select(x => new BasicContactResponse()
                               {
                                   Id = x.Id,
                                   FirstName = x.FirstName,
                                   LastName = x.LastName,
                                   IdNumber = x.IdNumber,
                                   Email = x.Email,
                               }).ToListAsync();
            }
            else
            {
                clients = await db.Contacts
                         .Where(c => c.FirstName.Contains(nameSearch) || c.LastName.Contains(nameSearch) || c.IdNumber.Contains(nameSearch))
                         .Take(10)
                         .Select(x => new BasicContactResponse()
                         {
                             Id = x.Id,
                             FirstName = x.FirstName,
                             LastName = x.LastName,
                             IdNumber = x.IdNumber,
                             Email = x.Email,
                         }).ToListAsync();
            }

            if (clients.Count == 0)
            {
                return NotFound();
            }
            return Ok(clients);
        }

        // GET: api/Contacts/nameSearch
        [Route("Contacts/Basic/company/{nameSearch}")]
        public async Task<IHttpActionResult> GetBasicContactCompany(string nameSearch)
        {
            try
            {
                if (nameSearch == "undefined" || nameSearch == "")
                {
                    return NotFound();
                }

                List<BasicContactResponse> clients = new List<BasicContactResponse>();

                if (nameSearch == "")
                {
                    clients = await db.Contacts
                        .Take(10)
                        .Select(x => new BasicContactResponse()
                        {
                            Id = x.Id,
                            FirstName = x.FirstName,
                            LastName = x.LastName,
                            IdNumber = x.IdNumber,
                            Email = x.Email
                        }).ToListAsync();
                }
                else
                {
                    clients = await db.Contacts
                        .Where(c => (c.FirstName == nameSearch) || (c.LastName == nameSearch) || (c.IdNumber == nameSearch))
                        .Take(10)
                        .Select(x => new BasicContactResponse()
                        {
                            Id = x.Id,
                            FirstName = x.FirstName,
                            LastName = x.LastName,
                            IdNumber = x.IdNumber,
                            Email = x.Email
                        }).ToListAsync();
                }

                if (clients.Count == 0)
                {
                    return NotFound();
                }

                return Ok(clients);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // GET: api/Contacts/nameSearch
        [Route("Contacts/Advisor/{nameSearch}")]
        public async Task<IHttpActionResult> GetAdvisorContact(string nameSearch)
        {
            if (nameSearch == "undefined")
            {
                return NotFound();
            }

            List<AdvisorContactResponse> advisors = new List<AdvisorContactResponse>();

            if (nameSearch == "")
            {
                advisors = await db.Contacts
                    .Take(10)
                    .Select(x => new AdvisorContactResponse()
                    {
                        ID = x.Id,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        IdNumber = x.IdNumber,
                        EmployerName = x.EmployerName,
                        JobTitle = x.JobTitle,
                        Email = x.Email,
                        Cell1 = x.Cell1,
                        Tel1 = x.Tel1,
                        CreatedDate = x.CreatedDate.GetValueOrDefault(),
                        ModifiedDate = x.ModifiedDate.GetValueOrDefault()
                    }).ToListAsync();
            }
            else
            {
                advisors = await db.Contacts
                    .Where(c => (c.FirstName.Contains(nameSearch)) || (c.LastName.Contains(nameSearch)) || (c.IdNumber.Contains(nameSearch)))
                    .Take(10)
                    .Select(x => new AdvisorContactResponse()
                    {
                        ID = x.Id,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        IdNumber = x.IdNumber,
                        EmployerName = x.EmployerName,
                        JobTitle = x.JobTitle,
                        Email = x.Email,
                        Cell1 = x.Cell1,
                        Tel1 = x.Tel1,
                        CreatedDate = x.CreatedDate,
                        ModifiedDate = x.ModifiedDate
                    }).ToListAsync();
            }

            if (advisors.Count == 0)
            {
                return NotFound();
            }

            return Ok(advisors);
        }

        // GET: api/Contacts/nameSearch
        [Route("Contacts/Details/{id}")]
        public async Task<IHttpActionResult> GetContactDetails(int id)
        {
            var clients = await db.Contacts
                            .Select(contact => new
                            {
                                contact.Id,
                                contact.FirstName,
                                contact.MiddleName,
                                contact.LastName,
                                contact.CreatedDate,
                                contact.EmployerName,
                                contact.IdNumber,
                                contact.Email,
                                contact.Tel1,
                                contact.JobTitle,
                                contact.ModifiedDate,
                            }).Where(contact => contact.Id == id).FirstOrDefaultAsync();

            if (clients == null)
            {
                return NotFound();
            }

            return Ok(clients);
        }

        
        // PUT: api/Advisor/5
        [Route("ContactsClients/put/{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutContactClient(int id, ClientsResponse advisor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contact = db.Contacts.Find(id);

            if (contact != null)
            {
                contact.ModifiedDate = DateTime.Today;
                db.Entry(contact).CurrentValues.SetValues(advisor);
            };

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
                throw ex; 
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        // PUT: api/Contacts/5
        [ResponseType(typeof(void))]
        [Route("Contacts/{id}")]
        public async Task<IHttpActionResult> PutContact(int id, Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != contact.Id)
            {
                return BadRequest();
            }

            var existingContact = db.Contacts.Find(contact.Id);

            if (existingContact != null)
            {
                contact.ModifiedDate = DateTime.Today;
                db.Entry(existingContact).CurrentValues.SetValues(contact);

                var title = await db.ContactTitles.FindAsync(contact.ContactTitle.Id);
                if (title != null)
                {
                    existingContact.ContactTitle = title;
                }

                var originalAddresses = db.Addresses.Include(a => a.AddressType)
                    .Include(a => a.Province)
                    .Include(a => a.Country)
                    .Where(c => c.Contact.Id == existingContact.Id);

                foreach (Address originalAddress in originalAddresses)
                {
                    var address = contact.Addresses.First(c => c.Id == originalAddress.Id);

                    if (address != null)
                    {
                        db.Entry(originalAddress).CurrentValues.SetValues(address);
                        db.Entry(existingContact).State = EntityState.Modified;
                    }
                }

            }
            else
            {
                db.Entry(existingContact).State = EntityState.Added;
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id))
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

        // POST: api/Contacts
        [ResponseType(typeof(Contact))]
        public async Task<IHttpActionResult> PostContact(Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (contact.Addresses != null)
            {
                var addressList = contact.Addresses.ToList();

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
                    if (address.AddressType == null)
                    {
                        var advContactAddressType = await db.AddressTypes.FindAsync(1);
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
                    if (address.Province == null)
                    {
                        var advContactAddressProvince = await db.Provinces.FindAsync(1);
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

                    if (address.Country == null)
                    {
                        var advContactAddressCountry = await db.Countries.FindAsync(1);
                        if (advContactAddressCountry != null)
                        {
                            address.Country = advContactAddressCountry;
                        }
                    }
                }
            }
            
            //Title
            if (contact.ContactTitle != null)
            {
                var title = await db.ContactTitles.FindAsync(contact.ContactTitle.Id);
                if (title != null)
                {
                    contact.ContactTitle = title;
                }
            }

            if (contact.ContactType == null)
            {
                //Use default contact type
                var type = await db.ContactTypes.FindAsync(1);
                if (type != null)
                {
                    contact.ContactType = type;
                }
            }
            else
            {
                //Use given contact type
                var type = await db.ContactTypes.FindAsync(contact.ContactType);
                if (type != null)
                {
                    contact.ContactType = type;
                }
            }
            
            contact.CreatedDate = DateTime.Today;

            db.Contacts.Add(contact);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw e;
            }

            return CreatedAtRoute("DefaultApi", new { id = contact.Id }, contact);
        }

        // DELETE: api/Contacts/5
        [ResponseType(typeof(Contact))]
        public async Task<IHttpActionResult> DeleteContact(int id)
        {
            Contact contact = await db.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            contact.Deleted = true;
            db.Entry(contact).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return Ok(contact);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ContactExists(int id)
        {
            return db.Contacts.Count(e => e.Id == id) > 0;
        }
    }
}
