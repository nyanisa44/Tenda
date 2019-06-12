using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models.Admin
{
    
    public class ApplicationWithMemberDTO
    {
        public Contact Member { get; set; }
        public Application Application { get; set; }

        public ApplicationWithMemberDTO(Application application, Contact member, ApplicationDbContext db)
        {
            Application originalApplication = db.Applications.Where(c => c.ApplicationNumber == application.ApplicationNumber).FirstOrDefault();
            Contact originalMember = new Contact();

            if (originalApplication != null)
            {
                db.Entry(originalApplication).Reference(m => m.Client).Load();
                db.Entry(originalApplication.Client).Reference(a => a.Addresses).Load();
                originalMember = application.Client;
                db.Entry(originalApplication).CurrentValues.SetValues(application);
                db.Entry(originalMember).CurrentValues.SetValues(member);

                if (application.Advisor == null && application.AdvisorCode != null)
                {
                    db.AdvisorSupplierCodes.First(code => code.AdvisorCode == application.AdvisorCode);

                }
            }
            else
            {
                originalApplication = application;
                originalMember = db.Contacts.Where(c => (c.IdNumber == member.IdNumber)).First();

                if (originalMember != null)
                {
                    db.Entry(originalMember).Reference(m => m.Addresses).Load();
                    db.Entry(originalMember).CurrentValues.SetValues(member);
                }
            }
            
            this.Member = member;
            this.Application = application;
        }
        
    }
}