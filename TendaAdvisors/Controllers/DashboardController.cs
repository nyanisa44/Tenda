using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using TendaAdvisors.Models;
using TendaAdvisors.Models.DTO;
using TendaAdvisors.Providers;

namespace TendaAdvisors.Controllers
{
    //[Authorize]
    public class DashboardController : BaseApiController
    {
       // private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Overview
        [ResponseType(typeof(DashboardDTO))]
        public async Task<IHttpActionResult> GetUserDashboard()
        {
            ApplicationUser user = HttpContext.Current.GetOwinContext()
                .GetUserManager<ApplicationUserManager>()
                .FindById(HttpContext.Current.User.Identity.GetUserId());

            if (user == null || user.AdvisorId == 0)
            {
                return NotFound();
            }

            var dashboardDto = new DashboardDTO();
            dashboardDto.ID = user.AdvisorId;

            //User could have more then one roll

            string superId = "";
            string advisorId = "";
            using (var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext())))
            {

                if (rm.RoleExists("Supervisor") == true)
                {
                    superId = rm.FindByName("Supervisor").Id;
                    //rm.Roles. = true;

                }

                if (rm.RoleExists("Advisor") == true)
                {
                    advisorId = rm.FindByName("Advisor").Id;
                    //rm.Roles. = true;
                }
            }
            //Check if the user is supervisor
            dashboardDto.Role = false;
            dashboardDto.advisorRole = false;
            foreach (var item in user.Roles)
            {
                if (item.RoleId == superId)
                {
                    dashboardDto.Role = true;
                }
                if (item.RoleId == advisorId)
                {
                    dashboardDto.advisorRole = true;
                }




            }


            var advisor = db.Advisors.Find(user.AdvisorId); 
            //var advisorShare = db.AdvisorShareUnderSupervisions.Find(user.AdvisorId);
            //await db.Entry(advisor).Collection(a => a.Applications).LoadAsync();
            //await db.Entry(advisor).Reference(a => a.Company).LoadAsync();
            //await db.Entry(advisor).Collection(c => c.AdvisorDocuments).LoadAsync();
            await db.Entry(advisor).Reference(a => a.Contact).LoadAsync();
            //await db.Entry(advisor).Collection(c => c.AdvisorDocuments).LoadAsync();




            dashboardDto.Advisor = advisor;
            /* dashboardDto.ApplicationStatuses= db.ApplicationStatuses.Select(a => new ApplicationStatusDTO()
             {
                 Id = a.Id,
                 Status = a.Status
             }).ToList();



             dashboardDto.AdvisorShareUnderSupervisions = db.AdvisorShareUnderSupervisions.Select(a => new AdvisorShareUnderSupervisionDTO()
             {
                 AdvisorId = a.AdvisorId,
                 LicenseTypeId =a.LicenseTypeId,
                 supplier=a.supplier,
                 product=a.product,
                 Share=a.Share,
                 underSupervision=a.underSupervision,
                 Advisor=a.Advisor,
                 validCommissionFromDate=a.validCommissionFromDate,
                 validCommissionToDate=a.validCommissionToDate,
                 ValidFromDate=a.ValidFromDate,
                 ValidToDate=a.ValidToDate

             }).ToList();


             foreach (ApplicationStatusDTO appStat in dashboardDto.ApplicationStatuses)
             {
                 appStat.Count = 0;
                 try
                 {
                     appStat.Count = db.Applications
                         .Where(a => a.ApplicationStatus.Id == appStat.Id && a.Advisor.Id == advisor.Id)
                         .Count();
                 }
                 catch (Exception ex)
                 {
                     var a = ex;
                 }
             }
             */


            //Comment out Sets Commission file status
            //var ImportFileList = db.ImportFiles.Where(e => e.ImportTypeId == 1).ToList();
            //foreach (var item in ImportFileList)
            //{
            //    var file = db.CommissionFileStatus.Where(e => e.ImportFileId == item.Id).ToList().FirstOrDefault();
            //    if (file == null)
            //    {
            //        db.CommissionFileStatus.Add(new CommissionFileStatus
            //        {
            //            UserUid = item.UserUid,
            //            FileName = item.FileName,
            //            Location = item.Location,
            //            Size = item.Size,
            //            Status = "Imported",
            //            ImportFileId = item.Id
            //        });
            //    }
            //    else
            //    {

            //        file.UserUid = item.UserUid;
            //        file.FileName = item.FileName;
            //        file.Location = item.Location;
            //        file.Size = item.Size;
            //        file.Status = "Imported";
            //        file.ImportFileId = item.Id;
            //        await db.SaveChangesAsync();
            //    }



            //}
            //await db.SaveChangesAsync();

            //Comment out Sets effective date
            //var AdvisorsList = db.Advisors.ToList();
            //foreach (var AdviList in AdvisorsList)
            //{
            //    AdviList.EffectiveStartDate = AdviList.CreatedDate;
            //}
            //await db.SaveChangesAsync();

            return Ok(dashboardDto);
        }

    }
}
