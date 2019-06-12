using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using TendaAdvisors.Models;
using TendaAdvisors.Providers;

namespace TendaAdvisors.Controllers
{
    //TODO Refactor controller
    public class ClientsDTO
    {
        public List<Contact> Clients { get; set; }
        public int Count { get; set; }
    }
    
    public class ClientsController : BaseApiController
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

        // GET: api/Clients
        [Route("Clients/{advisorId}/Advisor/{pageIndex}/{pageSize}")]
        public ClientsDTO GetClients(int advisorId = 0, int? pageIndex = 1, int? pageSize = 10)
        {
            if (_user != null && !User.IsInRole("Admin"))
            {
                advisorId = _user.AdvisorId;
            }

            pageIndex = Math.Abs((int)pageIndex);
            pageSize = Math.Abs((int)pageSize);
            pageSize = pageSize > 1000 ? 1000 : pageSize;

            ClientsDTO clientsDTO = new ClientsDTO();

            int totalPages = (int)Math.Ceiling((decimal)(int)clientsDTO.Count / (int)pageSize);

            pageIndex = pageIndex > totalPages ? totalPages : pageIndex;

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    clientsDTO.Count = db.Advisors
                        .Where(o => o.Id == advisorId)
                        .SelectMany(o => o.Applications)
                        .Select(c => c.Client)
                        .Count();

                    var advisor = db.Advisors
                        .Where(a => a.Id == advisorId)
                        .Include(a => a.Applications.Select(c => c.Client))
                        .FirstOrDefault();

                    IEnumerable<Contact> clients;

                    if (advisor != null)
                    {
                        clients = advisor.Applications.Select(a => a.Client);

                        clients.Select(a =>
                         new Contact()
                         {
                             Id = a.Id,
                             FirstName = a.FirstName,
                             LastName = a.LastName,
                             Cell1 = a.Cell1,
                             IdNumber = a.IdNumber,
                             Email = a.Email,
                             Tel1 = a.Tel1,
                             ContactTitle = a.ContactTitle
                         });
                    }
                    else
                    {
                        clients = db.Contacts.Where(x => !db.Advisors.Any(y => y.ContactId == x.Id));
                    }

                    clientsDTO.Clients = clients.ToList();
                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                }
            }

            return clientsDTO;
        }
        // : api/Advisor/1/CommissionStatement/1      
        [AcceptVerbs("GET", "POST")]
        [Route("Clients/{clientId}/CommissionStatement/{CommisionStatementId}")]
        public CommissionStatement ClientToCommision(int? clientId = null, int? CommisionStatementId = null)
        {
            CommissionStatement commToUpdate;
            if (clientId > 0)
            {
                try
                {
                    commToUpdate = db.CommissionStatement.Where(c => c.Id == CommisionStatementId).FirstOrDefault();
                    if (commToUpdate != null)
                    {
                        commToUpdate.ClientId = clientId;
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
        // DELETE: api/Applications/5
        [ResponseType(typeof(Application))]
        public async Task<IHttpActionResult> DeleteApplication(int id)
        {
            Application application = await db.Applications.FindAsync(id);
            if (application == null)
            {
                return NotFound();
            }

            application.Deleted = true;
            db.Entry(application).State = EntityState.Deleted;
            await db.SaveChangesAsync();

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

        [Route("Clients/All")]
        public List<ClientsListItem> GetAllClients()
        {
            var cleints = db.Applications.Select(e => e.Client);

            var client = cleints
            .Where( e => e.Id != null)
            .Select(a => new ClientsListItem()
            {
                Id = a.Id,
                Name = (a.FirstName + " " + a.LastName + " ( Id: " + a.IdNumber + ")"),
            })
            .Distinct()
            .OrderBy(e => e.Name)
            .ToList();

            return client;
        }

        [Route("Clients/CommissionPayed/{id}/{dateFrom}/{dateTo}")]
        [HttpGet]
        [ResponseType(typeof(MemberCommissionPayed))]
        public IHttpActionResult CommissionPayed(int id, DateTime dateFrom, DateTime dateTo)
        {
            var adviHist = (from x in db.ApplicationAdvisorHistory
                            .Where(e => e.DateStarted > dateFrom &&
                                 ((e.DateEnded != null) ? e.DateEnded <= dateTo : true))
                                .OrderBy(e => e.DateStarted)
                            join y in db.Applications.Where(e => e.Client_Id == id) on x.Application_Id equals y.Id
                            join o in db.CommissionStatement on y.Client_Id equals o.ClientId
                            join w in db.Products on y.Product_Id equals w.Id
                            join z in db.Suppliers on w.SupplierId equals z.Id
                            select new
                            {
                                prodId = y.Product_Id,
                                SupId = z.Id,
                                clientId = y.Client_Id,
                                AdviId = x.New_Advisor
                            })
                            .Distinct()
                            .AsEnumerable();

            var suppliersShares = (from x in db.AdvisorShareUnderSupervisions
                                   join y in db.Suppliers on x.SupplierId equals y.Id
                                   select new
                                   {
                                       share = x.Share,
                                       SupId = y.Id,
                                       AdviId = x.AdvisorId
                                   })
                                   .Where(e => e.share != null)
                                   .Distinct()
                                   .GroupBy(e => e.SupId)
                                   .AsEnumerable();


            var CommissionPayed = (from c in db.CommissionStatement.Where(e => e.ClientId == id)
                                   join d in suppliersShares on c.SupplierId equals d.Key
                                   join h in adviHist on c.ClientId equals h.clientId
                                   join sup in db.Suppliers on c.SupplierId equals sup.Id
                                   join mem in db.Contacts on c.ClientId equals mem.Id                           
                                   join advi in db.Advisors on c.AdvisorId equals advi.Id
                                   join adviCont in db.Contacts on advi.ContactId equals adviCont.Id
                                   select new MemberCommissionPayed()
                                   {
                                       ClientName = mem.FirstName+" "+mem.LastName,
                                       Adviser = adviCont.FirstName+" "+adviCont.LastName,
                                       ClientIdNumber = mem.IdNumber,
                                       Supplier = sup.Name,
                                       TransactionDate = (DateTime)c.TransactionDate,
                                       CommAmmount = ((double)c.CommissionExclVAT * (1 - (d.FirstOrDefault(e => e.AdviId == advi.Id).share == null ? 0: d.FirstOrDefault(e => e.AdviId == advi.Id).share) / 100))
                                   })
                                    .Where(c => c.TransactionDate < dateTo && c.TransactionDate >= dateFrom)
                                    .OrderBy(c => c.TransactionDate)
                                    .Distinct()
                                    .ToList();

            return Ok(CommissionPayed);
        }
    }
}
