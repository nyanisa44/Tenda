namespace TendaAdvisors.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;

    internal sealed class Configuration : DbMigrationsConfiguration<TendaAdvisors.Models.ApplicationDbContext>
    {
        private readonly bool _pendingMigrations;
        private static object _seedlock = new object();

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
           // AutomaticMigrationDataLossAllowed = true;
            var migrator = new DbMigrator(this);
            _pendingMigrations = migrator.GetPendingMigrations().Any();
        }

        private bool _runSeed = false;
        public void runSeed(TendaAdvisors.Models.ApplicationDbContext context)
        {
            _runSeed = true;
            Seed(context);
            _runSeed = false;
        }

        protected override void Seed(TendaAdvisors.Models.ApplicationDbContext context)
        {
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();

            if (!_pendingMigrations && !_runSeed) return;
            if (!_runSeed) return;
            lock(_seedlock)
            {
                SeedData(context);
            }
        }

        public static void SeedData(TendaAdvisors.Models.ApplicationDbContext context)
        {
#if DEBUG
            //run seed here
#endif

            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();

            var addrtype1 = new AddressType() { Id = 1, Name = "Home" };
            var addrtype2 = new AddressType() { Id = 2, Name = "Postal" };
            var addrtype3 = new AddressType() { Id = 3, Name = "Office" };

            

            var advstat1 = new AdvisorStatus() { Id = 1, Name = "Invalid" };
            var advstat2 = new AdvisorStatus() { Id = 2, Name = "Not Registered" };
            //var advstat3 = new AdvisorStatus() { Id = 3, Name = "Under Supervision" };
            var advstat4 = new AdvisorStatus() { Id = 4, Name = "Active" };
            var advstat5 = new AdvisorStatus() { Id = 5, Name = "Disabled" };
            var advstat6 = new AdvisorStatus() { Id = 6, Name = "Complete" };
            var advstat7 = new AdvisorStatus() { Id = 7, Name = "Pending" };
            var advstat8 = new AdvisorStatus() { Id = 8, Name = "Inactive" };



            var doctype1 = new DocumentType() { Id = 1, Name = "FAIS Certificate" };
            var doctype2 = new DocumentType() { Id = 2, Name = "ID Document" };
            var doctype3 = new DocumentType() { Id = 3, Name = "Formal Qualification" };
            var doctype4 = new DocumentType() { Id = 4, Name = "Proof of Residence" };
            var doctype5 = new DocumentType() { Id = 5, Name = "FICA Supporting Document" };
            var doctype6 = new DocumentType() { Id = 6, Name = "Broker Advice" };
            var doctype7 = new DocumentType() { Id = 7, Name = "Broker Note" };
            var doctype8 = new DocumentType() { Id = 8, Name = "Commission Statement" };

            var doctype9 = new DocumentType() { Id = 9, Name = "CMS Application" };
            var doctype10 = new DocumentType() { Id = 10, Name = "CMS (BR) Certificate" };
            var doctype11 = new DocumentType() { Id = 11, Name = "ID Document" };
            var doctype12 = new DocumentType() { Id = 12, Name = "Grade 12 Certificate" };
            var doctype13 = new DocumentType() { Id = 13, Name = "Banking Details" };
            var doctype14 = new DocumentType() { Id = 14, Name = "Supervision Plan" };
            var doctype15 = new DocumentType() { Id = 15, Name = "Supervision Agreement" };
            var doctype16 = new DocumentType() { Id = 16, Name = "FSB Declaration" };
            var doctype17 = new DocumentType() { Id = 17, Name = "Letter of Intent" };
            var doctype18 = new DocumentType() { Id = 18, Name = "Brokerage Appointment Contract" };
            var doctype19 = new DocumentType() { Id = 19, Name = "New User Form" };
            var doctype20 = new DocumentType() { Id = 20, Name = "Quarterly Fit and Proper declaration(Every 3 months)" };
            var doctype21 = new DocumentType() { Id = 21, Name = "Qualifictions(Adviser Training Page)" };
            var doctype22 = new DocumentType() { Id = 22, Name = "Qualification(In Progress)-(Adviser Training Page)-Must obtain within 6 years from appointment date" };
            var doctype23 = new DocumentType() { Id = 23, Name = "Quarterly Fit and Proper declaration(Every 3 months)" };
            var doctype24 = new DocumentType() { Id = 24, Name = "RE" };
            var doctype25 = new DocumentType() { Id = 25, Name = "RE(In Progress)-Must obtain within 2 years from appointment date" };

            var doctype26 = new DocumentType() { Id = 26, Name = "FSB Certificate" };
            var doctype27 = new DocumentType() { Id = 27, Name = "CMS Certificate(ORG)" };
            var doctype28 = new DocumentType() { Id = 28, Name = "Cipro Document" };
            var doctype29 = new DocumentType() { Id = 29, Name = "PI Cover" };
            var doctype30 = new DocumentType() { Id = 30, Name = "SARS Document" }; 
            var doctype31 = new DocumentType() { Id = 31, Name = "Vat Certificate" };
            var doctype32 = new DocumentType() { Id = 32, Name = "Risk Management Plan" };
            var doctype33 = new DocumentType() { Id = 33, Name = "Contracts" };
            var doctype34 = new DocumentType() { Id = 34, Name = "Complaints Resolution Policy" };
            var doctype35 = new DocumentType() { Id = 35, Name = "FICA Internal Rules" };
            var doctype36 = new DocumentType() { Id = 36, Name = "Conflict of Interest" };

            var doctype37 = new DocumentType() { Id = 37, Name = "Application Form" };
            var doctype38 = new DocumentType() { Id = 38, Name = "Advice Record" };
            var doctype39 = new DocumentType() { Id = 39, Name = "Disclosure Document" };
            var doctype40 = new DocumentType() { Id = 40, Name = "Group Appointment" };

            var doctype41 = new DocumentType() { Id = 41, Name = "No documents required,add if need be:" };
            var doctype42 = new DocumentType() { Id = 42, Name = "Voice logged script" };

            var advtype1 = new AdvisorType() { Id = 1, Title = "Key Individual", DocumentTypes = new List<DocumentType>() };
            var advtype2 = new AdvisorType() { Id = 2, Title = "Adviser", DocumentTypes = new List<DocumentType>() };
            var advtype3 = new AdvisorType() { Id = 3, Title = "Compliance Officer", DocumentTypes = new List<DocumentType>() };
            var advtype4 = new AdvisorType() { Id = 4, Title = "Support", DocumentTypes = new List<DocumentType>() };
            var advtype5 = new AdvisorType() { Id = 5, Title = "Company-Private", DocumentTypes = new List<DocumentType>() };
            var advtype6 = new AdvisorType() { Id = 6, Title = "Admin", DocumentTypes = new List<DocumentType>() };
            var advtype7 = new AdvisorType() { Id = 7, Title = "Director", DocumentTypes = new List<DocumentType>() };
            

            var apptype1 = new ApplicationType() { Id = 1, Title = "New Application", DocumentTypes = new List<DocumentType>() };
            var apptype2 = new ApplicationType() { Id = 2, Title = "Broker Note", DocumentTypes = new List<DocumentType>() };
            var apptype3 = new ApplicationType() { Id = 3, Title = "Group Appointment", DocumentTypes = new List<DocumentType>() };
            var apptype4 = new ApplicationType() { Id = 4, Title = "Incomplete", DocumentTypes = new List<DocumentType>() };
            var apptype5 = new ApplicationType() { Id = 5, Title = "Voice logged appointment", DocumentTypes = new List<DocumentType>() };
            var apptype6= new ApplicationType() { Id = 6, Title = "New Application-Solidarity", DocumentTypes = new List<DocumentType>() };
            var apptype7 = new ApplicationType() { Id =7, Title = "New Broker Note-Solidarity", DocumentTypes = new List<DocumentType>() };

            var title1 = new ContactTitles() { Id = 1, Name = "Mr", Contacts = new List<Contact>() };
            var title2 = new ContactTitles() { Id = 2, Name = "Ms", Contacts = new List<Contact>() };
            var title3 = new ContactTitles() { Id = 3, Name = "Miss", Contacts = new List<Contact>() };
            var title4 = new ContactTitles() { Id = 4, Name = "Mrs", Contacts = new List<Contact>() };

            var bank1 = new BankNames() { Id = 1, Name = "Standard Bank"};
            var bank2 = new BankNames() { Id = 2, Name = "FNB" };
            var bank3 = new BankNames() { Id = 3, Name = "ABSA" };
            var bank4 = new BankNames() { Id = 4, Name = "Nedbank" };
            var bank5 = new BankNames() { Id = 5, Name = "Capitec Bank" };

            var bank1Branch = new BankBranchCodes() { Id = 1, Name = "051 001" };
            var bank2Branch = new BankBranchCodes() { Id = 2, Name = "250 655" };
            var bank3Branch = new BankBranchCodes() { Id = 3, Name = "632 005" };
            var bank4Branch = new BankBranchCodes() { Id = 4, Name = "198 765" };
            var bank5Branch  = new BankBranchCodes() { Id = 5, Name = "	470 010" };



            apptype1.DocumentTypes.Add(doctype37);
            apptype1.DocumentTypes.Add(doctype38);
            apptype1.DocumentTypes.Add(doctype39);
            apptype6.DocumentTypes.Add(doctype37);
            apptype6.DocumentTypes.Add(doctype38);
            apptype6.DocumentTypes.Add(doctype39);
            apptype2.DocumentTypes.Add(doctype7);
            apptype7.DocumentTypes.Add(doctype7);
            apptype3.DocumentTypes.Add(doctype40);
            apptype4.DocumentTypes.Add(doctype37);
            apptype5.DocumentTypes.Add(doctype42);


            advtype1.DocumentTypes.Add(doctype1);
            advtype1.DocumentTypes.Add(doctype2);
            advtype1.DocumentTypes.Add(doctype3);


            //Add Advisor (2) to documentType (9-25)

            advtype2.DocumentTypes = new List<DocumentType>();
            advtype2.DocumentTypes.Add(doctype9);
            advtype2.DocumentTypes.Add(doctype10);
            advtype2.DocumentTypes.Add(doctype11);
            advtype2.DocumentTypes.Add(doctype12);
            advtype2.DocumentTypes.Add(doctype13);
            advtype2.DocumentTypes.Add(doctype14);
            advtype2.DocumentTypes.Add(doctype15);
            advtype2.DocumentTypes.Add(doctype16);
            advtype2.DocumentTypes.Add(doctype17);
            advtype2.DocumentTypes.Add(doctype18);
            advtype2.DocumentTypes.Add(doctype19);
            advtype2.DocumentTypes.Add(doctype20);
            advtype2.DocumentTypes.Add(doctype21);
            advtype2.DocumentTypes.Add(doctype22);
            advtype2.DocumentTypes.Add(doctype23);
            advtype2.DocumentTypes.Add(doctype24);
            advtype2.DocumentTypes.Add(doctype25);

            advtype5.DocumentTypes.Add(doctype26);
            advtype5.DocumentTypes.Add(doctype27);
            advtype5.DocumentTypes.Add(doctype28);
            advtype5.DocumentTypes.Add(doctype29);
            advtype5.DocumentTypes.Add(doctype30);
            advtype5.DocumentTypes.Add(doctype31);
            advtype5.DocumentTypes.Add(doctype32);
            advtype5.DocumentTypes.Add(doctype33);
            advtype5.DocumentTypes.Add(doctype34);
            advtype5.DocumentTypes.Add(doctype35);
            advtype5.DocumentTypes.Add(doctype36);

            advtype4.DocumentTypes.Add(doctype41);
            advtype3.DocumentTypes.Add(doctype41);
            advtype6.DocumentTypes.Add(doctype41);
            advtype7.DocumentTypes.Add(doctype41);



            var applstat1 = new ApplicationStatus() { Id = 1, Status = "New" };
            var applstat2 = new ApplicationStatus() { Id = 2, Status = "Pending" };
            var applstat3 = new ApplicationStatus() { Id = 3, Status = "Expired" };
            var applstat4 = new ApplicationStatus() { Id = 4, Status = "Cancelled" };
            var applstat5 = new ApplicationStatus() { Id = 5, Status = "Payment Failed" };
            var applstat6 = new ApplicationStatus() { Id = 6, Status = "Complete" };


            var conttype1 = new ContactType() { Id = 1, Name = "Private Individual" };
            var conttype2 = new ContactType() { Id = 2, Name = "Company" };
            var conttype3 = new ContactType() { Id = 3, Name = "Government Employee" };

            var country1 = new Country() { Id = 1, Name = "South Africa", Code = "ZA" };

            var prov1 = new Province() { Id = 1, Name = "Gauteng", Code = "GP" };
            var prov2 = new Province() { Id = 2, Name = "NorthWest", Code = "NW" };
            var prov3 = new Province() { Id = 3, Name = "Mpumulanga", Code = "MP" };
            var prov4 = new Province() { Id = 4, Name = "Limpopo", Code = "LP" };
            var prov5 = new Province() { Id = 5, Name = "KwaZulu-Natal", Code = "KZN" };
            var prov6 = new Province() { Id = 6, Name = "Free State", Code = "FS" };
            var prov7 = new Province() { Id = 7, Name = "Northern Cape", Code = "NC" };
            var prov8 = new Province() { Id = 8, Name = "Eastern Cape", Code = "EC" };
            var prov9 = new Province() { Id = 9, Name = "Western Cape", Code = "WC" };

            var importType1 = new ImportType() { Id = 1, Type = "Commission Statement", ActionUrl = "Commission", Enabled = true };
            var importType2 = new ImportType() { Id = 2, Type = "Applications with Members List", ActionUrl = "ApplicationsMembers", Enabled = true };
            var importType4 = new ImportType() { Id = 2, Type = "Clients", ActionUrl = "Clients", Enabled = true };
            var importType5 = new ImportType() { Id = 2, Type = "Applications", ActionUrl = "Applications", Enabled = true };
            var importType3 = new ImportType() { Id = 2, Type = "Product-Applications", ActionUrl = "Product-Applications", Enabled = true };
            var importType6 = new ImportType() { Id = 2, Type = "Application-Suppliers", ActionUrl = "Application-Suppliers", Enabled = true };

            var company1 = new Company() { Name = "RetroRabbit", ImageUrl = "http://retrorabbit.co.za/Resources/retrorabbit-logo.png", VatNumber = "123456789Umm" };
            var company2 = new Company() { Name = "TendaHealth (Pty) Ltd", ImageUrl = "/img/Tendahealth%20logo.png", VatNumber = "123456789", FsbNumber = "44680" };

            //Medical
            var supplier1 = new Supplier() { Id = 1, Name = "MediHelp", Deleted = false, Licenses = new List<LicenseType>() };
            var supplier2 = new Supplier() { Id = 2, Name = "Feadhealth", Deleted = false, Licenses = new List<LicenseType>() };
            var supplier3 = new Supplier() { Id = 3, Name = "Discovery", Deleted = false, Licenses = new List<LicenseType>() };
            var supplier4 = new Supplier() { Id = 4, Name = "Bonitas", Deleted = false, Licenses = new List<LicenseType>() };
            var supplier5 = new Supplier() { Id = 5, Name = "Bestmed", Deleted = false, Licenses = new List<LicenseType>() };
            var supplier6= new Supplier() { Id = 6, Name = "Momentum", Deleted = false, Licenses = new List<LicenseType>() };
            var supplier7 = new Supplier() { Id = 7, Name = "Keyhealth", Deleted = false, Licenses = new List<LicenseType>() };
            var supplier8 = new Supplier() { Id = 8, Name = "Topmed", Deleted = false, Licenses = new List<LicenseType>() };

            //Short Term
            var supplier9 = new Supplier() { Id = 9, Name = "SANTAM", Deleted = false, Licenses = new List<LicenseType>() };
            var supplier10= new Supplier() { Id = 10, Name = "Auto and General", Deleted = false, Licenses = new List<LicenseType>() };
            var supplier11= new Supplier() { Id = 11, Name = "Mutual and Federal", Deleted = false, Licenses = new List<LicenseType>() };
            var supplier12 = new Supplier() { Id = 12, Name = "Hollard", Deleted = false, Licenses = new List<LicenseType>() };
            var supplier13 = new Supplier() { Id = 13, Name = "Discovery Insure", Deleted = false, Licenses = new List<LicenseType>() };
            var supplier14 = new Supplier() { Id =14, Name = "Discovery  Invest", Deleted = false, Licenses = new List<LicenseType>() };
            var supplier15 = new Supplier() { Id = 15, Name = "Discovery Live", Deleted = false, Licenses = new List<LicenseType>() };
            var supplier16 = new Supplier() { Id = 16, Name = "Guardrisk", Deleted = false, Licenses = new List<LicenseType>() };
            var supplier17= new Supplier() { Id = 17, Name = "Stratum", Deleted = false, Licenses = new List<LicenseType>() };
            var supplier18 = new Supplier() { Id = 18, Name = "Tumberry", Deleted = false, Licenses = new List<LicenseType>() };
            var supplier19 = new Supplier() { Id = 19, Name = "DRUM Dental", Deleted = false, Licenses = new List<LicenseType>() };
            var supplier20 = new Supplier() { Id = 20, Name = "FSP Solutions", Deleted = false, Licenses = new List<LicenseType>() };
            var supplier21 = new Supplier() { Id = 21, Name = "Legacy", Deleted = false, Licenses = new List<LicenseType>() };


            var licCategory1 = new LicenseCategory() { Id = 1, Name = "CATEGORY I" };
            var licCategory2 = new LicenseCategory() { Id = 2, Name = "CATEGORY II - Discretionary FSP" };
            var licCategory3 = new LicenseCategory() { Id = 3, Name = "CATEGORY III - Administrative FSP" };
            var licCategory4 = new LicenseCategory() { Id = 4, Name = "CATEGORY IV Assistance business FSP" };

            var licTypeC1S1 = new LicenseType() { Id = 1, LicenseCategoryId = 1, SubCategory = "1", Description = "Long - term Insurance: Category A", };
            var licTypeC1S2 = new LicenseType() { Id = 2, LicenseCategoryId = 1, SubCategory = "2", Description = "Short - Term Insurance: Personal Lines", };
            var licTypeC1S3 = new LicenseType() { Id = 3, LicenseCategoryId = 1, SubCategory = "3", Description = "Long - Term Insurance: Category B1", };
            var licTypeC1S20 = new LicenseType() { Id = 4, LicenseCategoryId = 1, SubCategory = "20", Description = "Long - Term Insurance: Category B2", };
            var licTypeC1S4 = new LicenseType() { Id = 5, LicenseCategoryId = 1, SubCategory = "4", Description = "Long - Term Insurance: Category C", };
            var licTypeC1S5 = new LicenseType() { Id = 6, LicenseCategoryId = 1, SubCategory = "5", Description = "Retail Pension Benefits", };
            var licTypeC1S6 = new LicenseType() { Id = 7, LicenseCategoryId = 1, SubCategory = "6", Description = "Short - Term Insurance: Commercial Lines", };
            var licTypeC1S7 = new LicenseType() { Id = 8, LicenseCategoryId = 1, SubCategory = "7", Description = "Pension Funds Benefits(excluding retail)", };
            var licTypeC1S8 = new LicenseType() { Id = 9, LicenseCategoryId = 1, SubCategory = "8", Description = "Securities and Instruments: Shares", };
            var licTypeC1S9 = new LicenseType() { Id = 10, LicenseCategoryId = 1, SubCategory = "9", Description = "Securities and Instruments: Money market instruments", };
            var licTypeC1S10 = new LicenseType() { Id = 11, LicenseCategoryId = 1, SubCategory = "10", Description = "Securities and Instruments: Debentures and securitised debt", };
            var licTypeC1S11 = new LicenseType() { Id = 12, LicenseCategoryId = 1, SubCategory = "11", Description = "Securities and Instruments: Warrants, certificates and other instruments", };
            var licTypeC1S12 = new LicenseType() { Id = 13, LicenseCategoryId = 1, SubCategory = "12", Description = "Securities and Instruments: Bonds", };
            var licTypeC1S13 = new LicenseType() { Id = 14, LicenseCategoryId = 1, SubCategory = "13", Description = "Securities and Instruments: Derivative instruments", };
            var licTypeC1S14 = new LicenseType() { Id = 15, LicenseCategoryId = 1, SubCategory = "14", Description = "Participatory interests in Collective Investment Schemes", };
            var licTypeC1S15 = new LicenseType() { Id = 16, LicenseCategoryId = 1, SubCategory = "15", Description = "Foreign currency denominated investment instruments", };
            var licTypeC1S16 = new LicenseType() { Id = 17, LicenseCategoryId = 1, SubCategory = "16", Description = "Health Service Benefits", };
            var licTypeC1S17 = new LicenseType() { Id = 18, LicenseCategoryId = 1, SubCategory = "17", Description = "Deposits Defined in the Banks Act - exceeding 12 months", };
            var licTypeC1S18 = new LicenseType() { Id = 19, LicenseCategoryId = 1, SubCategory = "18", Description = "Deposits defined in the Banks act - 12 months or less", };
            var licTypeC1S19 = new LicenseType() { Id = 20, LicenseCategoryId = 1, SubCategory = "19", Description = "Friendly Society Benefits", };

            var licTypeC2S1 = new LicenseType() { Id = 21, LicenseCategoryId = 2, SubCategory = "1", Description = "Long - Term Insurance: Category B1", };
            var licTypeC2S16 = new LicenseType() { Id = 22, LicenseCategoryId = 2, SubCategory = "16", Description = "Long - Term Insurance: Category B2", };
            var licTypeC2S2 = new LicenseType() { Id = 23, LicenseCategoryId = 2, SubCategory = "2", Description = "Long - Term Insurance: Category C", };
            var licTypeC2S3 = new LicenseType() { Id = 24, LicenseCategoryId = 2, SubCategory = "3", Description = "Retail Pension Benefits", };
            var licTypeC2S4 = new LicenseType() { Id = 25, LicenseCategoryId = 2, SubCategory = "4", Description = "Pension Funds Benefits(excluding retail pension benefits)", };
            var licTypeC2S5 = new LicenseType() { Id = 26, LicenseCategoryId = 2, SubCategory = "5", Description = "Securities and Instruments: Shares", };
            var licTypeC2S6 = new LicenseType() { Id = 27, LicenseCategoryId = 2, SubCategory = "6", Description = "Securities and Instruments: Money market instruments", };
            var licTypeC2S7 = new LicenseType() { Id = 28, LicenseCategoryId = 2, SubCategory = "7", Description = "Securities and Instruments: Debentures and securitised debt", };
            var licTypeC2S8 = new LicenseType() { Id = 29, LicenseCategoryId = 2, SubCategory = "8", Description = "Securities and Instruments: Warrants, certificates and other instruments", };
            var licTypeC2S9 = new LicenseType() { Id = 30, LicenseCategoryId = 2, SubCategory = "9", Description = "Securities and Instruments: Bonds", };
            var licTypeC2S10 = new LicenseType() { Id = 31, LicenseCategoryId = 2, SubCategory = "10", Description = "Securities and Instruments: Derivative instruments", };
            var licTypeC2S11 = new LicenseType() { Id = 32, LicenseCategoryId = 2, SubCategory = "11", Description = "Participatory interests in Collective Investment Schemes", };
            var licTypeC2S12 = new LicenseType() { Id = 33, LicenseCategoryId = 2, SubCategory = "12", Description = "Foreign currency denominated investment instruments", };
            var licTypeC2S13 = new LicenseType() { Id = 34, LicenseCategoryId = 2, SubCategory = "13", Description = "Long - term Deposits", };
            var licTypeC2S14 = new LicenseType() { Id = 35, LicenseCategoryId = 2, SubCategory = "14", Description = "Short - term Deposits", };
            var licTypeC2SA = new LicenseType() { Id = 36, LicenseCategoryId = 2, SubCategory = "A", Description = "Hedge Fund Manager", };

            var licTypeC3S1 = new LicenseType() { Id = 37, LicenseCategoryId = 3, SubCategory = "1", Description = "Long - Term Insurance: Category B1", };
            var licTypeC3S15 = new LicenseType() { Id = 38, LicenseCategoryId = 3, SubCategory = "15", Description = "Long - Term Insurance: Category B2", };
            var licTypeC3S2 = new LicenseType() { Id = 39, LicenseCategoryId = 3, SubCategory = "2", Description = "Long - Term Insurance: Category C", };
            var licTypeC3S3 = new LicenseType() { Id = 40, LicenseCategoryId = 3, SubCategory = "3", Description = "Retail Pension Benefits", };
            var licTypeC3S4 = new LicenseType() { Id = 41, LicenseCategoryId = 3, SubCategory = "4", Description = "Pension Funds Benefits(excluding retail pension benefits)", };
            var licTypeC3S5 = new LicenseType() { Id = 42, LicenseCategoryId = 3, SubCategory = "5", Description = "Securities and Instruments: Shares", };
            var licTypeC3S6 = new LicenseType() { Id = 43, LicenseCategoryId = 3, SubCategory = "6", Description = "Securities and Instruments: Money market instruments", };
            var licTypeC3S7 = new LicenseType() { Id = 44, LicenseCategoryId = 3, SubCategory = "7", Description = "Securities and Instruments: Debentures and securitised debt", };
            var licTypeC3S8 = new LicenseType() { Id = 45, LicenseCategoryId = 3, SubCategory = "8", Description = "Securities and Instruments: Warrents, certificates and other instruments", };
            var licTypeC3S9 = new LicenseType() { Id = 46, LicenseCategoryId = 3, SubCategory = "9", Description = "Securities and Instruments: Bonds", };
            var licTypeC3S10 = new LicenseType() { Id = 47, LicenseCategoryId = 3, SubCategory = "10", Description = "Securities and Instruments: Derivative instruments", };
            var licTypeC3S11 = new LicenseType() { Id = 48, LicenseCategoryId = 3, SubCategory = "11", Description = "Participatory interests in Collective Investment Schemes", };
            var licTypeC3S12 = new LicenseType() { Id = 49, LicenseCategoryId = 3, SubCategory = "12", Description = "Foreign currency denominated investment instruments", };
            var licTypeC3S13 = new LicenseType() { Id = 50, LicenseCategoryId = 3, SubCategory = "13", Description = "Long - term Deposits", };
            var licTypeC3S14 = new LicenseType() { Id = 51, LicenseCategoryId = 3, SubCategory = "14", Description = "Short - term Deposits", };

            var licTypeC4S1 = new LicenseType() { Id = 52, LicenseCategoryId = 4, SubCategory = "1", Description = "Assistance business FSP", };


            var address1 = new Address()
            {
                City = "Pretoria",
                Country = country1,
                Description = "Ring bell?",
                PostalCode = "0184",
                Province = prov2,
                Street1 = "171 Clifford Rd",
                Suburb = "Murrayfield Ext. 1",
                MapUrl = "http://maps.google.com/",
                AddressType = addrtype1
            };
            var address2 = new Address()
            {
                City = "Pretoria",
                Country = country1,
                Description = "Please knock 2 and then turn in a circle and knock again three times.",
                PostalCode = "0040",
                Province = prov2,
                Street1 = "116 Camellia Ave",
                Suburb = "Murrayfield Ext. 1",
                MapUrl = "https://www.google.co.za/maps/place/Retro+Rabbit/@-25.7598715,28.2932057,18.43z/data=!4m5!3m4!1s0x0:0x688d465f509c535d!8m2!3d-25.7593317!4d28.2930317",
                AddressType = addrtype2
            };
            var address3 = new Address()
            {
                City = "Johannesburg",
                Country = country1,
                Description = "Please knock once and then jump up and down and knock again two times.",
                PostalCode = "2090",
                Province = prov2,
                Street1 = "90 Grayston Ave",
                Suburb = "Blairegowry",
                MapUrl = "",
                AddressType = addrtype2
            };


            Contact contact1 = new Contact()
            {
                Id = 1,
                ContactType = conttype1,
                EmployerName = "Tenda Health",
                JobTitle = "Advisor",
                Tel1 = "0124240435",
                ContactTitle = title1,
                FirstName = "Jaco",
                Initial = "JJ",
                MiddleName = "Jaco",
                LastName = "Klopper",
                Email = "jaco@tendaonline.co.za",
                Cell1 = "0722267418",
                IdNumber = "8181818181810",
                Addresses = new Collection<Address>(),
                ModifiedDate = DateTime.Now,
                CreatedDate = DateTime.Now,
                BenefitCode = "Fed1",
                BirthDate = DateTime.Parse("1981/04/02"),
                BusinessUnit = "13",
                ChronicPMB = false,
                Deleted = false,
                DependantsAdult = 1,
                DependantsChild = 0,
                GroupCode = "8161 - PRIVATE INDIVIDUALS: ADVANCE",
                Language = "Afrikaans",
                LinkDate = DateTime.Parse("1981/04/03"),              
                MonthlyFeeCurrent = 3900.01m,
                Network = "Non newtork",
                photoUrl = "img/Eddie-01.png",
                SalaryCode = "A",
                SalaryScale = "1 - 99999",
                SchemeRegisterDate = DateTime.Parse("1981/04/04"),
                SchemeTerminatedDate = DateTime.Parse("1981/04/01"),
                Upsell = false
            };

            Contact contact2 = new Contact()
            {
                Id = 2,
                ContactType = conttype1,
                EmployerName = "RetroRabbit",
                JobTitle = "Bunny",
                Tel1 = "+27 12 803 3068",
                ContactTitle = title1,
                FirstName = "Stacy",
                LastName = "Jose",
                Email = "stacy@retrorabbit.co.za",
                Cell1 = "+27 76 9656891",
                IdNumber = "8104025058084",
                Addresses = new Collection<Address>() { address3 },
                ModifiedDate = DateTime.Now,
                CreatedDate = DateTime.Now
            };

            var advisor1 = new Advisor()
            {
                Id = 1,
                IsActive = true,
                IsKeyIndividual = false,
                Deleted = false,
                LastLoginDate = null,
                ModifiedDate = DateTime.Now,
                CreatedDate = DateTime.Now,
                Company = company2,
                FsbCode = "0083",
                CmsCode = "1123",
                RegNumber = "3456",
                //DateAuthorized = null,
                ComplianceOfficer = "jefff",
                ComplianceOffficerNumber = "07896523",
                Contact = contact1,
                BankName=bank1,
                BranchCode= bank1Branch,
                AccountType="Student Cheque Account",
                AccountNumber="12365478965412",
                AdvisorStatus = advstat1,
                AdvisorType = advtype1,
                AdvisorDocuments = new Collection<AdvisorDocument>(),
                Applications = new Collection<Application>(),
                User = new ViewModels.User
                {
                    FirstName = "Test1",
                    LastName = "Some2"
                }


            };

            var advisor2 = new Advisor()
            {
                Id = 2,
                IsActive = true,
                IsKeyIndividual = false,
                Deleted = false,
                LastLoginDate = null,
                ModifiedDate = DateTime.Now,
                CreatedDate = DateTime.Now,
                Company = company1,
                FsbCode = "5569",
                CmsCode = "7456",
                RegNumber = "123654789",
                //DateAuthorized = null,
                //ComplianceOfficer = "Jessi Markums",
                //ComplianceOffficerNumber = "0729491978",
                Contact = contact2,
                AdvisorStatus = advstat1,
                AdvisorType = advtype2,
                BankName = bank2,
                BranchCode = bank2Branch,
                AccountType = "Student Cheque Account",
                AccountNumber = "12365478965412",
                AdvisorDocuments = new Collection<AdvisorDocument>(),
                Applications = new Collection<Application>(),
                User = new ViewModels.User
                {
                    FirstName = "Test2",
                    LastName = "Some3"
                }
            };

            var advisor3 = new Advisor()
            {
                Id = 3,
                IsActive = true,
                IsKeyIndividual = false,
                Deleted = false,
                LastLoginDate = null,
                ModifiedDate = DateTime.Now,
                CreatedDate = DateTime.Now,
                Company = company1,
                FsbCode = "5569",
                CmsCode = "7456",
                RegNumber = "156329",
                //DateAuthorized = null,
                ComplianceOfficer= "molly",
                ComplianceOffficerNumber= "03251978",
                Contact = contact2,
                AdvisorStatus = advstat4,
                AdvisorType = advtype5,
                BankName = bank3,
                BranchCode = bank3Branch,
                AccountType = "Student Cheque Account",
                AccountNumber = "12365478965412",
                AdvisorDocuments = new Collection<AdvisorDocument>(),
                Applications = new Collection<Application>(),
                User = new ViewModels.User
                {
                    FirstName = "Test3",
                    LastName = "Some"
                }
            };


            company1.ContactDetails = contact2;
            company2.ContactDetails = contact1;

            var querytype1 = new QueryType() { Id = 1, QueryName = "Complaint" };
            var querytype2 = new QueryType() { Id = 2, QueryName = "Update" };

            var query1 = new Query() { Id = 1, UserUID = "Steve Thompson", Note = "Amount is incorrect", Deleted = false, QueryType = querytype1, Advisor = advisor1 };
            var query2 = new Query() { Id = 2, UserUID = "Bob Van Der Walt", Note = "Update Address", Deleted = false, QueryType = querytype2, Advisor = advisor1 };
            var query3 = new Query() { Id = 3, UserUID = "James Gobay", Note = "Wrong Policy", Deleted = false, QueryType = querytype1, Advisor = advisor1 };

            var transaction1 = new Transaction() { Id = 1, TransactionDate = DateTime.Parse("2016-01-13 14:00:04 PM"), AccountId = 1, ContraAccountId = 2, DebitAmount = 0m, CreditAmount = 500.001m, Note = "Payment" };
            var transaction2 = new Transaction() { Id = 2, TransactionDate = DateTime.Parse("2016-01-13 14:00:06 PM"), AccountId = 2, ContraAccountId = 1, DebitAmount = 500.001m, CreditAmount = 0m, Note = "BC:1141577" };

            var account1 = new Account() { Id = 1, Note = "Final Payment", Advisor = advisor1, Supplier = supplier2, Transactions = new Collection<Transaction>() { transaction1 } };
            var account2 = new Account() { Id = 2, Note = "Allowance", Advisor = advisor1, Supplier = supplier1, Transactions = new Collection<Transaction>() { transaction2 } };

            var advdoc1 = new AdvisorDocument()
            {
                Advisor = advisor1,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now.AddDays(25),
                Deleted = false,
                Id = 1,
                IsExpired = false,
                IsRequired = true,
                Uploaded = true,
                Location = "C:\\Users\\sjj98\\Desktop\\TH1\\TendaAdvisors\\TendaAdvisors\\Uploads\\AdvisorDocuments\\iddoc1.pdf",
                OriginalFileName="iddoc1.pdf",
                Title = "ID Document 1",
                ValidFromDate = DateTime.Now.AddDays(-366),
                ValidToDate = DateTime.Now.AddDays(75),
                DocumentType = doctype2
            };

            var advdoc2 = new AdvisorDocument()
            {
                Advisor = advisor1,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now.AddDays(25),
                Deleted = false,
                Id = 1,
                IsExpired = false,
                IsRequired = true,
                Uploaded = true,
                Location = "C:\\Users\\sjj98\\Desktop\\TH1\\TendaAdvisors\\TendaAdvisors\\Uploads\\AdvisorDocuments\\iddoc2.pdf",
                OriginalFileName = "iddoc2.pdf",
                Title = "ID Document 2",
                ValidFromDate = DateTime.Now.AddDays(-366),
                ValidToDate = DateTime.Now.AddDays(75),
                DocumentType = doctype2
            };

            var advdoc3 = new AdvisorDocument()
            {
                Advisor = advisor1,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now.AddDays(25),
                Deleted = false,
                Id = 1,
                IsExpired = false,
                IsRequired = true,
                Uploaded = true,
                Location = "C:\\Users\\sjj98\\Desktop\\TH1\\TendaAdvisors\\TendaAdvisors\\Uploads\\AdvisorDocuments\\iddoc3.pdf",
                OriginalFileName = "iddoc3.pdf",
                Title = "ID Document 3",
                ValidFromDate = DateTime.Now.AddDays(-366),
                ValidToDate = DateTime.Now.AddDays(75),
                DocumentType = doctype2
            };


            //var client1 = new Client()
            //{
            //    Id = 2
            //};

            var client1 = new Contact
            {
                Id = 2,
                ContactType = conttype2,
                EmployerName = "KPMG",
                JobTitle = "Consultant",
                Tel1 = "+27 11 1234 123",
                ContactTitle = title4,
                FirstName = "Stacey",
                LastName = "da Silva",
                Email = "sdasilva@kpmg.co.za",
                Cell1 = "+27 76 567 7778",
                IdNumber = "8505035068081",
                Addresses = new Collection<Address>() { address1, address2 },
                ModifiedDate = DateTime.Now,
                CreatedDate = DateTime.Now
            };

            //var client2 = new Client()
            //{
            //    Id = 2,
            //};

            var client2 = new Contact
            {
                Id = 3,
                ContactType = conttype3,
                EmployerName = "PSG Consulting",
                JobTitle = "Manager",
                Tel1 = "+27 13 2254 823",
                ContactTitle = title1,
                FirstName = "Peter",
                LastName = "Peterson",
                Email = "ppeterson@psg.co.za",
                Cell1 = "+27 79 565 7278",
                IdNumber = "7505036068084",
                Addresses = new Collection<Address>() { address3 },
                ModifiedDate = DateTime.Now,
                CreatedDate = DateTime.Now
            };


            ////Next Product Medihelp


            var producttest = new Product()
            {
                Id = 123,
                Name = "test",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S1,
                Supplier = supplier1,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product1 = new Product()
            {
                Id = 1,
                Name = "Dimension Prime 1",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier1,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product2 = new Product()
            {
                Id = 2,
                Name = "Dimension Prime 1 Network",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier1,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product3 = new Product()
            {
                Id = 3,
                Name = "Dimension Prime 2",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier1,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product4 = new Product()
            {
                Id = 4,
                Name = "Dimension Prime 2 Network",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier1,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product5 = new Product()
            {
                Id = 5,
                Name = "Dimension Prime 3",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier1,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product6 = new Product()
            {
                Id = 6,
                Name = "Dimension Prime 3 Network",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier1,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product7 = new Product()
            {
                Id = 7,
                Name = "Dimension Elite",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier1,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };


            var product8 = new Product()
            {
                Id = 8,
                Name = "Medihelp Plus",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier1,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };


            var product9 = new Product()
            {
                Id = 9,
                Name = "Unify",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier1,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product10 = new Product()
            {
                Id = 10,
                Name = "Necesse",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier1,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product11 = new Product()
            {
                Id = 11,
                Name = "Necesse Student",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier1,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };


            //Next Product Discovery
            var product12 = new Product()
            {
                Id = 12,
                Name = "Classic Comprehensive",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier3,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product13 = new Product()
            {
                Id = 13,
                Name = "Classic Comprehensive Zero",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier3,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product14 = new Product()
            {
                Id = 14,
                Name = "Classic Core",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier3,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product15 = new Product()
            {
                Id = 15,
                Name = "Classic Delta Comprehensive",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier3,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product16 = new Product()
            {
                Id = 16,
                Name = "Classic Delta Core",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier3,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product17 = new Product()
            {
                Id = 17,
                Name = "Classic Delta Saver",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier3,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product18= new Product()
            {
                Id = 18,
                Name = "Classic Priority",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier3,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product19 = new Product()
            {
                Id = 19,
                Name = "Classic Saver",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier3,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product20 = new Product()
            {
                Id = 20,
                Name = "Coastal Core",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier3,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product21 = new Product()
            {
                Id = 21,
                Name = "Coastal Saver",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier3,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product22 = new Product()
            {
                Id = 22,
                Name = "Essential Comprehensive",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier3,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product23 = new Product()
            {
                Id = 23,
                Name = "Essential Core",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier3,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

           
            var product25 = new Product()
            {
                Id = 25,
                Name = "Essential Delta Core",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier3,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product26 = new Product()
            {
                Id = 26,
                Name = "Essential Delta Comprehensive",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier3,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product27 = new Product()
            {
                Id = 27,
                Name = "Essential Delta Saver",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier3,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product28 = new Product()
            {
                Id = 28,
                Name = "Essential Priority",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier3,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product29 = new Product()
            {
                Id = 29,
                Name = "Essential Saver",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier3,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product30 = new Product()
            {
                Id = 30,
                Name = "Executive",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier3,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product31 = new Product()
            {
                Id = 31,
                Name = "Key Care Access",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier3,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product32 = new Product()
            {
                Id = 32,
                Name = "Key Care Core",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier3,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product33 = new Product()
            {
                Id = 33,
                Name = "Key Care Plus",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier3,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product34 = new Product()
            {
                Id = 34,
                Name = "Smart Plan",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier3,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            //Next Product  Bonitas
            var product24 = new Product()
            {
                Id = 24,
                Name = "Standard",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier4,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product35 = new Product()
            {
                Id = 35,
                Name = "Standard Select",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier4,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product36 = new Product()
            {
                Id = 36,
                Name = "Primary",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier4,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product37 = new Product()
            {
                Id = 37,
                Name = "BonComprehensive",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier4,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product38 = new Product()
            {
                Id = 38,
                Name = "BonClassic",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier4,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product39 = new Product()
            {
                Id = 39,
                Name = "BonSave",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier4,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product40 = new Product()
            {
                Id = 40,
                Name = "BonFit",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier4,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product41 = new Product()
            {
                Id = 41,
                Name = "BonEssential",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier4,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            //Next Product  BestMed
            var product42 = new Product()
            {
                Id = 42,
                Name = "Beat 1",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier5,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product43 = new Product()
            {
                Id = 43,
                Name = "Beat 1 Network",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier5,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product44 = new Product()
            {
                Id = 44,
                Name = "Beat 2",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier5,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product45 = new Product()
            {
                Id = 45,
                Name = "Beat 2 Network",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier5,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product46 = new Product()
            {
                Id = 46,
                Name = "Beat 3",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier5,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product47 = new Product()
            {
                Id = 47,
                Name = "Beat 3 Network",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier5,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product48 = new Product()
            {
                Id = 48,
                Name = "Beat 4",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier5,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product49 = new Product()
            {
                Id = 49,
                Name = "Pulse 1",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier5,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product50 = new Product()
            {
                Id = 50,
                Name = "Pulse 2",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier5,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product51 = new Product()
            {
                Id = 51,
                Name = "Pace 1",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier5,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product52 = new Product()
            {
                Id = 52,
                Name = "Pace 2",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier5,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product53 = new Product()
            {
                Id = 53,
                Name = "Pace 3",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier5,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product54 = new Product()
            {
                Id = 54,
                Name = "Pace 4",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier5,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            //New product Momentum

            var product55 = new Product()
            {
                Id = 55,
                Name = "Summit",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier6,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product56 = new Product()
            {
                Id = 56,
                Name = "Extender Ass. Hospital/Any chronic provider",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier6,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product57 = new Product()
            {
                Id = 57,
                Name = "Extender Ass. Hospital/Ass. chronic provider",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier6,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product58 = new Product()
            {
                Id = 58,
                Name = "Extender Ass. Hospital/State provider",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier6,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product59 = new Product()
            {
                Id = 59,
                Name = "Extender Any Hospital/ Any chronic provider",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier6,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product60 = new Product()
            {
                Id = 60,
                Name = "Extender Any Hospital/Ass. chronic provider",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier6,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product61 = new Product()
            {
                Id = 61,
                Name = "Extender Any Hospital/State provider",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier6,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product62 = new Product()
            {
                Id = 62,
                Name = "Custom Ass. Hospital/ Any chronic provider",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier6,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product63 = new Product()
            {
                Id = 63,
                Name = "Custom Ass. Hospital/Ass. chronic provider",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier6,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product64 = new Product()
            {
                Id = 64,
                Name = "Custom Ass. Hospital/State provider",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier6,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product65 = new Product()
            {
                Id = 65,
                Name = "Custom Any Hospital/ Any chronic provider",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier6,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product66 = new Product()
            {
                Id = 66,
                Name = "Custom Any Hospital/Ass. chronic provider",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier6,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product67 = new Product()
            {
                Id = 67,
                Name = "Custom Any Hospital/State provider",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier6,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product68 = new Product()
            {
                Id = 68,
                Name = "Incentive Ass. Hospital/ Any chronic provider",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier6,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product69 = new Product()
            {
                Id = 69,
                Name = "Incentive Ass. Hospital/Ass. chronic provider",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier6,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product70 = new Product()
            {
                Id = 70,
                Name = "Incentive Ass. Hospital/State provider",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier6,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product71 = new Product()
            {
                Id = 71,
                Name = "Incentive Any Hospital/ Any chronic provider",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier6,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product72 = new Product()
            {
                Id = 72,
                Name = "Incentive Any Hospital/Ass. chronic provider",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier6,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product73 = new Product()
            {
                Id = 73,
                Name = "Incentive Any Hospital/State provider",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier6,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product74 = new Product()
            {
                Id = 74,
                Name = "Ingwe Any Hospital",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier6,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product75 = new Product()
            {
                Id = 75,
                Name = "Ingwe Ass. Hospital",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier6,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product76 = new Product()
            {
                Id = 76,
                Name = "Ingwe State Hospital",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier6,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product77 = new Product()
            {
                Id = 77,
                Name = "Access",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier6,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            //Next product Feadhealth

            var product78 = new Product()
            {
                Id = 78,
                Name = "Ultimax",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier2,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product79 = new Product()
            {
                Id = 79,
                Name = "Maxima Plus",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier2,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product80 = new Product()
            {
                Id = 80,
                Name = "Maxima Exec",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier2,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product81 = new Product()
            {
                Id = 81,
                Name = "Maxcima Standard",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier2,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product82 = new Product()
            {
                Id = 82,
                Name = "Maxcima Standard Elect",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier2,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product83 = new Product()
            {
                Id = 83,
                Name = "Maxcima Core",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier2,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product84 = new Product()
            {
                Id = 84,
                Name = "Maxcima Entryzone",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier2,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product85 = new Product()
            {
                Id = 85,
                Name = "Maxima Saver",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier2,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product86 = new Product()
            {
                Id = 86,
                Name = "Maxcima Entry Saver",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier2,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product87 = new Product()
            {
                Id = 87,
                Name = "Ultima 200",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier2,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product88 = new Product()
            {
                Id = 88,
                Name = "Maxima Basis",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier2,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product89 = new Product()
            {
                Id = 89,
                Name = "Maxima Entry Zone",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier2,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product90 = new Product()
            {
                Id = 90,
                Name = "Blue Door Plus",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier2,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };


            //New Product Topmed

            var product91 = new Product()
            {
                Id = 91,
                Name = "Topmed Rainbow Comprehensive",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier8,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product92 = new Product()
            {
                Id = 92,
                Name = "Topmed Paladin Comprehensive",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier8,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product93 = new Product()
            {
                Id = 93,
                Name = "Topmed Professional",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier8,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product94 = new Product()
            {
                Id = 94,
                Name = "Topmed Limited",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier8,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product95= new Product()
            {
                Id = 95,
                Name = "Topmed Hospital",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier8,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product96 = new Product()
            {
                Id = 96,
                Name = "Topmed Savings",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier8,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product97 = new Product()
            {
                Id = 97,
                Name = "Topmed Active Saver",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier8,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product98= new Product()
            {
                Id = 98,
                Name = "Topmed Network",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier8,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            //Netx product Keyhealth

            var product99= new Product()
            {
                Id = 99,
                Name = "Origin",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier7,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product100 = new Product()
            {
                Id = 100,
                Name = "Platinum",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier7,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product101 = new Product()
            {
                Id = 101,
                Name = "Silver",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier7,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product102 = new Product()
            {
                Id = 102,
                Name = "Equilibtium",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier7,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product103 = new Product()
            {
                Id = 103,
                Name = "Essence",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier7,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product104 = new Product()
            {
                Id = 104,
                Name = "Gold",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier7,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            
            //others

              var product105 = new Product()
              {
                  Id = 105,
                  Name = "Santam product personal lines",
                  Applications = new Collection<Application>(),
                  LicenseType = licTypeC1S2,
                  Supplier = supplier9,
                  CreatedDate = DateTime.Now,
                  ModifiedDate = DateTime.Now,
              };
            var product106 = new Product()
            {
                Id = 106,
                Name = "Santam product commecial lines",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S6,
                Supplier = supplier9,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product107 = new Product()
            {
                Id = 107,
                Name = "Auto product personal lines",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S2,
                Supplier = supplier10,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product108 = new Product()
            {
                Id = 108,
                Name = "Auto product commecial lines",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S6,
                Supplier = supplier10,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product109= new Product()
            {
                Id = 109,
                Name = "Mutual product personal lines",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S2,
                Supplier = supplier11,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product110 = new Product()
            {
                Id = 110,
                Name = "Mutual product commecial lines",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S6,
                Supplier = supplier11,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };



            var product111 = new Product()
            {
                Id = 111,
                Name = "Hollard product personal lines",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S2,
                Supplier = supplier12,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product112 = new Product()
            {
                Id = 112,
                Name = "Hollar product commecial lines",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S6,
                Supplier = supplier12,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product113 = new Product()
            {
                Id = 113,
                Name = "Discovery Insure product personal lines",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S2,
                Supplier = supplier13,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product114 = new Product()
            {
                Id = 114,
                Name = "Discovery Invest Retail pension benefits",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S5,
                Supplier = supplier14,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product115 = new Product()
            {
                Id = 115,
                Name = "Discovery Live pension fund",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S7,
                Supplier = supplier15,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product116= new Product()
            {
                Id = 116,
                Name = "Discovery Live participatory interests",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S14,
                Supplier = supplier15,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product117 = new Product()
            {
                Id = 117,
                Name = "Admed gap",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S2,
                Supplier = supplier16,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product118 = new Product()
            {
                Id = 118,
                Name = "Stratum gap",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S2,
                Supplier = supplier17,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product119 = new Product()
            {
                Id = 119,
                Name = "Tumberry gap",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S2,
                Supplier = supplier18,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            var product120 = new Product()
            {
                Id = 120,
                Name = "Drum dental product",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S2,
                Supplier = supplier19,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product121 = new Product()
            {
                Id = 121,
                Name = "FSP Solutions personal product",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S2,
                Supplier = supplier20,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product122 = new Product()
            {
                Id = 122,
                Name = "FSP Solutions commercial product",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S6,
                Supplier = supplier20,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };


            var product123 = new Product()
            {
                Id = 123,
                Name = "Legacy personal product",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S2,
                Supplier = supplier21,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product124 = new Product()
            {
                Id = 124,
                Name = "Legacy commercial product",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S6,
                Supplier = supplier21,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            var product125 = new Product()
            {
                Id = 125,
                Name = "BONCAP OPTION",
                Applications = new Collection<Application>(),
                LicenseType = licTypeC1S16,
                Supplier = supplier4,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };















            var application1 = new Application()
            {
                ApplicationStatus = applstat6,
                //ApplicationType = appType37,
                ApplicationDocuments = new Collection<ApplicationDocument>() {
                    new ApplicationDocument() {
                        Id = 1,
                        Title = "Document title of a document in an application.",
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        IsExpired = true,
                        IsRequired = true,
                        Location = "C:\\Users\\sjj98\\Desktop\\TH1\\TendaAdvisors\\TendaAdvisors\\Uploads\\ApplicationDocuments\\iddoc1.pdf",
                        OriginalFileName="iddoc1.pdf",
                        DocumentType = doctype1,
                        Product = product1,
                        ValidFromDate = DateTime.Now,
                        ValidToDate = DateTime.Now.AddDays(30),
                    },
                },

                Product = product1,
                Client = client1,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                Deleted = false,
                Queries = new Collection<Query>()
            };
            

            var a2l1 = new Advisor2LicenseTypes { AdvisorId = 1, LicenseTypeId = 1, Share = 5, underSupervision = false,
                validCommissionFromDate = DateTime.Now.AddDays(-366), validCommissionToDate = DateTime.Now.AddDays(75),
                ValidFromDate = DateTime.Now.AddDays(-366), ValidToDate= DateTime.Now.AddDays(75)
            };
            var a2l2 = new Advisor2LicenseTypes { AdvisorId = 1, LicenseTypeId = 10, Share = 10, underSupervision = true,
                Advisor = 2 , validCommissionFromDate = DateTime.Now.AddDays(-366),validCommissionToDate = DateTime.Now.AddDays(75),
                ValidFromDate= DateTime.Now.AddDays(-366),ValidToDate= DateTime.Now.AddDays(75)
            };
            var a2l3 = new Advisor2LicenseTypes { AdvisorId = 1, LicenseTypeId = 11, Share = 15, underSupervision = false,
                validCommissionFromDate = DateTime.Now.AddDays(-366),validCommissionToDate = DateTime.Now.AddDays(75),
                ValidFromDate= DateTime.Now.AddDays(-366),ValidToDate= DateTime.Now.AddDays(75)
            };

            context.Products.AddOrUpdate(
                p => p.Id,
                product1, product2, product3, product4, product5, product6, product7, product8, product9, product10,
                product11, product12, product13, product14, product15, product16, product17, product18, product19, product20,
                product21, product22, product23, product24, product25, product26, product27, product28, product29, product30,
                product31, product32, product33, product34, product35, product36, product37, product38, product39, product40, 
                product41,product42, product43, product44, product45, product46, product47, product48, product49, product50,
                product51, product52, product53, product54, product55, product56, product57, product58, product59, product60,
                product61, product62, product63, product64, product65, product66, product67, product68, product69, product70,
                product71, product72, product73, product74, product75, product76, product77, product78, product79, product80,
                product81, product82, product83, product84, product85, product86, product87, product88, product89, product90,
                product91, product92, product93, product94, product95, product96, product97, product98, product99, product100,
                product101, product102, product103, product104, producttest,

                product105, product106, product107, product108, product109, product110, product111, product112, product113, product114,
                product115, product116, product117, product118, product119, product120, product121, product122, product123, product124, 
                product125

        );

            context.AddressTypes.AddOrUpdate(
                p => p.Id,
                addrtype1, addrtype2, addrtype3
                );

            context.ApplicationTypes.AddOrUpdate(
                p => p.Id,
                apptype1, apptype2, apptype3, apptype4, apptype5, apptype6, apptype7
                );

            context.AdvisorStatuses.AddOrUpdate(
                p => p.Id,
                advstat1, advstat2, advstat4, advstat5, advstat6, advstat7
            );

            context.AdvisorTypes.AddOrUpdate(
                p => p.Id,
                advtype1, advtype2, advtype3, advtype4, advtype5, advtype6, advtype7
                );


            context.ContactTitles.AddOrUpdate(
               p => p.Id,
               title1, title2, title3, title4
               );

            context.BankName.AddOrUpdate(
              p => p.Id,
            bank1, bank2, bank3, bank4,bank5
              );

            context.BankBranchCodes.AddOrUpdate(
            p => p.Id,
          bank1Branch, bank2Branch, bank3Branch, bank4Branch, bank5Branch
            );






            context.ApplicationStatuses.AddOrUpdate(
                p => p.Id,
            applstat1, applstat2, applstat3, applstat4, applstat5, applstat6
            );

            context.ContactTypes.AddOrUpdate(
                p => p.Id,
                conttype1, conttype2, conttype3
                );

            context.Countries.AddOrUpdate(
                p => p.Id,
                country1
                );

            context.DocumentTypes.AddOrUpdate(
                p => p.Id,
                doctype1, doctype2, doctype3, doctype4, doctype5,
                doctype6, doctype7, doctype8, doctype9, doctype10,
                doctype11, doctype12, doctype13, doctype14, doctype15,
                doctype16, doctype17, doctype18, doctype19, doctype20,
                doctype21, doctype22, doctype23, doctype24, doctype25,
                doctype26, doctype27, doctype28, doctype29, doctype30,
                doctype31, doctype32, doctype33, doctype34, doctype35,
                doctype36, doctype37, doctype38, doctype39, doctype40,
                doctype41, doctype42

                );

            context.LicenseCategories.AddOrUpdate(c => c.Id,
                licCategory1,
                licCategory2,
                licCategory3,
                licCategory4
                );

            context.LicenseTypes.AddOrUpdate(
                c => c.Id,
                licTypeC1S1, licTypeC1S2, licTypeC1S3, licTypeC1S20, licTypeC1S4, licTypeC1S5, licTypeC1S6, licTypeC1S7,
                licTypeC1S8, licTypeC1S9, licTypeC1S10, licTypeC1S11, licTypeC1S12, licTypeC1S13, licTypeC1S14, licTypeC1S15,
                licTypeC1S16, licTypeC1S17, licTypeC1S18, licTypeC1S19, licTypeC2S1, licTypeC2S16, licTypeC2S2, licTypeC2S3,
                licTypeC2S4, licTypeC2S5, licTypeC2S6, licTypeC2S7, licTypeC2S8, licTypeC2S9, licTypeC2S10, licTypeC2S11,
                licTypeC2S12, licTypeC2S13, licTypeC2S14, licTypeC2SA, licTypeC3S1, licTypeC3S15, licTypeC3S2, licTypeC3S3,
                licTypeC3S4, licTypeC3S5, licTypeC3S6, licTypeC3S7, licTypeC3S8, licTypeC3S9, licTypeC3S10, licTypeC3S11,
                licTypeC3S12, licTypeC3S13, licTypeC3S14, licTypeC4S1
                );

            context.Provinces.AddOrUpdate(
                p => p.Id,
                prov1, prov2, prov3, prov4, prov5, prov6, prov7, prov8, prov9
                );

            context.Companies.AddOrUpdate(
                p => p.Id,
                company1, company2
                );

            supplier1.Licenses.Add(licTypeC1S16);
            supplier1.Licenses.Add(licTypeC1S1);
            supplier2.Licenses.Add(licTypeC1S16);
            supplier3.Licenses.Add(licTypeC1S16);
            supplier4.Licenses.Add(licTypeC1S16);
            supplier5.Licenses.Add(licTypeC1S16);
            supplier6.Licenses.Add(licTypeC1S16);
            supplier7.Licenses.Add(licTypeC1S16);
            supplier8.Licenses.Add(licTypeC1S16);
            supplier9.Licenses.Add(licTypeC1S2);
            supplier9.Licenses.Add(licTypeC1S6);
            supplier10.Licenses.Add(licTypeC1S2);
            supplier10.Licenses.Add(licTypeC1S6);
            supplier11.Licenses.Add(licTypeC1S2);
            supplier11.Licenses.Add(licTypeC1S6);
            supplier12.Licenses.Add(licTypeC1S2);
            supplier12.Licenses.Add(licTypeC1S6);
            supplier13.Licenses.Add(licTypeC1S2);
            supplier14.Licenses.Add(licTypeC1S5);
            supplier15.Licenses.Add(licTypeC1S7);
            supplier15.Licenses.Add(licTypeC1S14);

            supplier16.Licenses.Add(licTypeC1S2);
            supplier17.Licenses.Add(licTypeC1S2);
            supplier18.Licenses.Add(licTypeC1S2);
            supplier19.Licenses.Add(licTypeC1S2);
            supplier20.Licenses.Add(licTypeC1S2);
            supplier20.Licenses.Add(licTypeC1S6);
            supplier21.Licenses.Add(licTypeC1S2);
            supplier21.Licenses.Add(licTypeC1S6);


            context.Suppliers.AddOrUpdate(
                p => p.Id,
                supplier1, supplier2, supplier3, supplier4, supplier5, supplier6, supplier7, supplier8,
                supplier9, supplier10, supplier11, supplier12, supplier13, supplier14, supplier15, supplier16,
                supplier17, supplier18, supplier19, supplier20, supplier21
                );

            context.Advisors.AddOrUpdate(
              p => p.Id,
              advisor1, advisor2,advisor3
            );

            context.Accounts.AddOrUpdate(
                p => p.Id,
                account1, account2
                );

            context.ImportTypes.AddOrUpdate(i => i.Id,
                importType1, importType2, importType4, importType5, importType3, importType6
                );

            //context.Transactions.AddOrUpdate(
            //    p => p.Id,
            //    transaction1, transaction2
            //    );

            //context.Queries.AddOrUpdate(
            //    p => p.Id,
            //    query1, query2, query3
            //    );

            context.QueryTypes.AddOrUpdate(
                p => p.Id,
                querytype1, querytype2
                );



            //context.Clients.AddOrUpdate(
            //    p => p.Id,
            //    client1, client2
            //);
            application1.Queries.Add(query1);
            application1.Queries.Add(query2);
            application1.Queries.Add(query3);

            advisor1.Applications.Add(
                application1
            );

            advisor1.AdvisorDocuments.Add(advdoc1);
            advisor1.AdvisorDocuments.Add(advdoc2);
            advisor1.AdvisorDocuments.Add(advdoc3);

            context.Advisors.AddOrUpdate(advisor1);
            context.Advisors.AddOrUpdate(advisor2);
            context.Advisors.AddOrUpdate(advisor3);
            

            context.AdvisorLicenseTypes.AddOrUpdate( a => a.AdvisorId, a2l1, a2l2, a2l3);
            
            
            //advisor1.Contact.Addresses.Add(address1);
            //advisor1.Contact.Addresses.Add(address2);

            //application1.Products.Add(product1);
            //application1.Products.Add(product2);
            //application1.Products.Add(product3);

            //context.Products.AddOrUpdate(
            //    p => p.Id,
            //    product1, product2, product3
            //    );



            context.SaveChanges();

            List<string> userRoles = new List<string>();

            userRoles.Add("Admin");
            userRoles.Add("Advisor");
            userRoles.Add("Supervisor");

            using (var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext())))
            {
                using (var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
                {

                    foreach (var item in userRoles)
                    {
                        if (!rm.RoleExists(item))
                        {
                            var roleResult = rm.Create(new IdentityRole(item));
                            if (!roleResult.Succeeded)
                                throw new ApplicationException("Creating role " + item + "failed with error(s): " + roleResult.Errors);
                        }
                    }

                    if (um.FindByName("admin") == null)
                    {
                        ApplicationUser user = new ApplicationUser
                        {
                            UserName = "admin",
                            Email = context.Advisors.First().Contact.Email,
                            JoinDate = DateTime.Now,
                            LastLoginDate = null,
                            AdvisorId = context.Advisors.First().Id,
                            EmailConfirmed = true
                        };

                        IdentityResult result = um.Create(user, "admin123");
                        result.ThrowOnFailure();

                        if (!um.IsInRole(user.Id, userRoles[0]))
                        {
                            var userResult = um.AddToRole(user.Id, userRoles[0]);
                            if (!userResult.Succeeded)
                                throw new ApplicationException("Adding user '" + user.Id + "' to '" + userRoles[0] + "' role failed with error(s): " + userResult.Errors);
                        }


                    }

                    if (um.FindByName("supervisor") == null)
                    {
                        ApplicationUser user = new ApplicationUser
                        {
                            UserName = "sup",
                            Email = context.Advisors.First().Contact.Email,
                            JoinDate = DateTime.Now,
                            LastLoginDate = null,
                            AdvisorId = context.Advisors.First().Id,
                            EmailConfirmed = true
                        };

                        IdentityResult result = um.Create(user, "sup123");
                        result.ThrowOnFailure();

                        if (!um.IsInRole(user.Id, userRoles[2]))
                        {
                            var userResult = um.AddToRole(user.Id, userRoles[2]);
                            if (!userResult.Succeeded)
                                throw new ApplicationException("Adding user '" + user.Id + "' to '" + userRoles[2] + "' role failed with error(s): " + userResult.Errors);
                        }


                    }

                    if (um.FindByName("Advisor") == null)
                    {
                        ApplicationUser user = new ApplicationUser
                        {
                            UserName = "add",
                            Email = context.Advisors.First().Contact.Email,
                            JoinDate = DateTime.Now,
                            LastLoginDate = null,
                            AdvisorId = context.Advisors.First().Id,
                            EmailConfirmed = true
                        };

                        IdentityResult result = um.Create(user, "add123");
                        result.ThrowOnFailure();


                        if (!um.IsInRole(user.Id, userRoles[1]))
                        {
                            var userResult = um.AddToRole(user.Id, userRoles[1]);
                            if (!userResult.Succeeded)
                                throw new ApplicationException("Adding user '" + user.Id + "' to '" + userRoles[1] + "' role failed with error(s): " + userResult.Errors);
                        }


                    }
                }
            }
        }
    }
}
