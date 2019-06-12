using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false){}

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Advisor> Advisors { get; set; }
        public DbSet<AdvisorDocument> AdvisorDocuments { get; set; }
        public DbSet<AdvisorStatus> AdvisorStatuses { get; set; }
        public DbSet<AdvisorType> AdvisorTypes { get; set; }
        public DbSet<ApplicationType> ApplicationTypes { get; set; }
        public DbSet<ContactTitles> ContactTitles { get; set; }
        public DbSet<BankNames> BankName { get; set; }
        public DbSet<BankBranchCodes> BankBranchCodes { get; set; }
        public DbSet<CommissionFileStatus> CommissionFileStatus { get; set; }
        public DbSet<AdvisorShareUnderSupervision> AdvisorShareUnderSupervisions { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AddressType> AddressTypes { get; set; }
        public DbSet<ApplicationAdvisorHistory> ApplicationAdvisorHistory { get; set; }
        public DbSet<ApplicationAdvisorEditHistory> ApplicationAdvisorEditHistory { get; set; }
        public DbSet<ApplicationDocument> ApplicationDocuments { get; set; }
        public DbSet<ApplicationStatus> ApplicationStatuses { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactType> ContactTypes { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Query> Querys { get; set; }
        public DbSet<QueryType> QueryTypes { get; set; }
        public DbSet<ImportFile> ImportFiles { get; set; }
        public DbSet<ImportType> ImportTypes { get; set; }
        public DbSet<CommissionStatement> CommissionStatement { get; set; }
        public DbSet<AdvisorSupplierCode> AdvisorSupplierCodes { get; set; }
        public DbSet<LicenseType> LicenseTypes { get; set; }
        public DbSet<LicenseCategory> LicenseCategories { get; set; }
        public DbSet<Advisor2LicenseTypes> AdvisorLicenseTypes { get; set; }
        public DbSet<AnnualTaxBracket> AnnualTaxBrackets { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<ApplicationSupplier> ApplicationSuppliers { get; set; }
        public DbSet<UnmatchedCommissions> UnmatchedCommissions { get; set; }
        public DbSet<MemberListException> MemberListExceptions { get; set; }
    }
}
