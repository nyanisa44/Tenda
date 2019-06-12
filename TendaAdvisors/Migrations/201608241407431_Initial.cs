namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Note = c.String(maxLength: 30),
                        Advisor_Id = c.Int(),
                        Supplier_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Advisors", t => t.Advisor_Id)
                .ForeignKey("dbo.Suppliers", t => t.Supplier_Id)
                .Index(t => t.Advisor_Id)
                .Index(t => t.Supplier_Id);

            
                

            CreateTable(
                "dbo.Advisors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsActive = c.Boolean(nullable: false),
                        IsKeyIndividual = c.Boolean(),
                        Deleted = c.Boolean(),
                        LastLoginDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        CreatedDate = c.DateTime(),
                        FsbCode = c.String(),
                        CmsCode = c.String(),
                        ContactId = c.Int(nullable: false),
                        AdvisorType_Id = c.Int(),
                        AdvisorStatus_Id = c.Int(),
                        Company_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AdvisorTypes", t => t.AdvisorType_Id)
                .ForeignKey("dbo.AdvisorStatus", t => t.AdvisorStatus_Id)
                .ForeignKey("dbo.Companies", t => t.Company_Id)
                .ForeignKey("dbo.Contacts", t => t.ContactId, cascadeDelete: true)
                .Index(t => t.ContactId)
                .Index(t => t.AdvisorType_Id)
                .Index(t => t.AdvisorStatus_Id)
                .Index(t => t.Company_Id);
            
            CreateTable(
                "dbo.AdvisorDocuments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Location = c.String(),
                        IsRequired = c.Boolean(nullable: false),
                        IsExpired = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        Uploaded = c.Boolean(nullable: false),
                        File = c.String(),
                        OriginalFileName = c.String(),
                        ValidFromDate = c.DateTime(),
                        ValidToDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        CreatedDate = c.DateTime(),
                        DocumentTypeId = c.Int(nullable: false),
                        Advisor_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Advisors", t => t.Advisor_Id)
                .ForeignKey("dbo.DocumentTypes", t => t.DocumentTypeId, cascadeDelete: true)
                .Index(t => t.DocumentTypeId)
                .Index(t => t.Advisor_Id);
            
            CreateTable(
                "dbo.DocumentTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        BaseFileStoragePath = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AdvisorTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AdvisorStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Applications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationNumber = c.String(),
                        AdvisorCode = c.String(),
                        Deleted = c.Boolean(),
                        ModifiedDate = c.DateTime(),
                        CreatedDate = c.DateTime(),
                        Advisor_Id = c.Int(),
                        ApplicationStatus_Id = c.Int(),
                        Client_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Advisors", t => t.Advisor_Id)
                .ForeignKey("dbo.ApplicationStatus", t => t.ApplicationStatus_Id)
                .ForeignKey("dbo.Contacts", t => t.Client_Id)
                .Index(t => t.Advisor_Id)
                .Index(t => t.ApplicationStatus_Id)
                .Index(t => t.Client_Id);
            
            CreateTable(
                "dbo.ApplicationDocuments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Location = c.String(),
                        File = c.String(),
                        OriginalFileName = c.String(),
                        IsRequired = c.Boolean(nullable: false),
                        IsExpired = c.Boolean(nullable: false),
                        Deleted = c.Boolean(),
                        ValidFromDate = c.DateTime(),
                        ValidToDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        CreatedDate = c.DateTime(),
                        ProductId = c.Int(nullable: false),
                        ApplicationId = c.Int(nullable: false),
                        DocumentTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Applications", t => t.ApplicationId, cascadeDelete: true)
                .ForeignKey("dbo.DocumentTypes", t => t.DocumentTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.ApplicationId)
                .Index(t => t.DocumentTypeId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 30),
                        ModifiedDate = c.DateTime(),
                        CreatedDate = c.DateTime(),
                        LicenseTypeId = c.Int(nullable: false),
                        Supplier_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LicenseTypes", t => t.LicenseTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Suppliers", t => t.Supplier_Id)
                .Index(t => t.LicenseTypeId)
                .Index(t => t.Supplier_Id);
            
            CreateTable(
                "dbo.LicenseTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LicenseCategoryId = c.Int(nullable: false),
                        SubCategory = c.String(maxLength: 2),
                        Description = c.String(maxLength: 128),
                        Company_Id = c.Int(),
                        Advisor_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.Company_Id)
                .ForeignKey("dbo.Advisors", t => t.Advisor_Id)
                .Index(t => t.Company_Id)
                .Index(t => t.Advisor_Id);
            
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 30),
                        Deleted = c.Boolean(),
                        MainContact_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contacts", t => t.MainContact_Id)
                .Index(t => t.MainContact_Id);
            
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 10),
                        FirstName = c.String(nullable: false),
                        Initial = c.String(),
                        MiddleName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        Email2 = c.String(),
                        Tel1 = c.String(),
                        Tel2 = c.String(),
                        Tel3 = c.String(),
                        TelWork1 = c.String(),
                        TelWork2 = c.String(),
                        TelWork3 = c.String(),
                        Cell1 = c.String(),
                        Cell2 = c.String(),
                        Cell3 = c.String(),
                        BirthDate = c.DateTime(),
                        SchemeRegisterDate = c.DateTime(),
                        SchemeTerminatedDate = c.DateTime(),
                        LinkDate = c.DateTime(),
                        IdNumber = c.String(),
                        MemberNumber = c.String(),
                        GroupCode = c.String(),
                        JobTitle = c.String(),
                        EmployerName = c.String(),
                        Language = c.String(),
                        Network = c.String(),
                        BenefitCode = c.String(),
                        SalaryCode = c.String(),
                        SalaryScale = c.String(),
                        MonthlyFeeCurrent = c.Decimal(precision: 18, scale: 2),
                        BusinessUnit = c.String(),
                        photoUrl = c.String(maxLength: 255),
                        Upsell = c.Boolean(),
                        ChronicPMB = c.Boolean(),
                        DependantsAdult = c.Int(),
                        DependantsChild = c.Int(),
                        Deleted = c.Boolean(),
                        ModifiedDate = c.DateTime(),
                        CreatedDate = c.DateTime(),
                        ContactType_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ContactTypes", t => t.ContactType_Id)
                .Index(t => t.ContactType_Id);
            
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 100),
                        Street1 = c.String(maxLength: 50),
                        Street2 = c.String(maxLength: 50),
                        Street3 = c.String(maxLength: 50),
                        Suburb = c.String(maxLength: 30),
                        PostalCode = c.String(maxLength: 50),
                        City = c.String(maxLength: 30),
                        MapUrl = c.String(),
                        Deleted = c.Boolean(),
                        AddressType_Id = c.Int(),
                        Contact_Id = c.Int(),
                        Country_Id = c.Int(),
                        Province_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AddressTypes", t => t.AddressType_Id)
                .ForeignKey("dbo.Contacts", t => t.Contact_Id)
                .ForeignKey("dbo.Countries", t => t.Country_Id)
                .ForeignKey("dbo.Provinces", t => t.Province_Id)
                .Index(t => t.AddressType_Id)
                .Index(t => t.Contact_Id)
                .Index(t => t.Country_Id)
                .Index(t => t.Province_Id);
            
            CreateTable(
                "dbo.AddressTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 30),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 30),
                        Code = c.String(maxLength: 5),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Provinces",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 30),
                        Code = c.String(maxLength: 30),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ContactTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 30),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ApplicationStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Status = c.String(maxLength: 30),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Queries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserUID = c.String(maxLength: 30),
                        Note = c.String(),
                        Deleted = c.Boolean(nullable: false),
                        QuerySent = c.DateTime(),
                        QueryName = c.String(),
                        Advisor_Id = c.Int(),
                        Application_Id = c.Int(),
                        QueryType_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Advisors", t => t.Advisor_Id)
                .ForeignKey("dbo.Applications", t => t.Application_Id)
                .ForeignKey("dbo.QueryTypes", t => t.QueryType_Id)
                .Index(t => t.Advisor_Id)
                .Index(t => t.Application_Id)
                .Index(t => t.QueryType_Id);
            
            CreateTable(
                "dbo.QueryTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QueryName = c.String(maxLength: 30),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 30),
                        VatNumber = c.String(maxLength: 30),
                        FsbNumber = c.String(),
                        ImageUrl = c.String(),
                        ContactDetails_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contacts", t => t.ContactDetails_Id)
                .Index(t => t.ContactDetails_Id);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DebitAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreditAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Note = c.String(),
                        TransactionDate = c.DateTime(nullable: false),
                        AccountId = c.Int(nullable: false),
                        ContraAccountId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AccountId, cascadeDelete: true)
                .Index(t => t.AccountId);
            
            CreateTable(
                "dbo.AdvisorSupplierCodes",
                c => new
                    {
                        SupplierId = c.Int(nullable: false),
                        AdvisorId = c.Int(nullable: false),
                        AdvisorCode = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SupplierId, t.AdvisorId });
            
            CreateTable(
                "dbo.CommissionStatements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MemberGroupCode = c.String(),
                        Surname = c.String(),
                        Initial = c.String(),
                        MemberNumber = c.String(),
                        EnrollmentDate = c.DateTime(),
                        TerminationDate = c.DateTime(),
                        TransactionDate = c.DateTime(),
                        SubscriptionDue = c.Decimal(precision: 18, scale: 2),
                        SubscriptionReceived = c.Decimal(precision: 18, scale: 2),
                        Commission = c.Decimal(precision: 18, scale: 2),
                        Vat = c.Decimal(precision: 18, scale: 2),
                        CommissionTotal = c.Decimal(precision: 18, scale: 2),
                        AdvisorId = c.Int(nullable: false),
                        ClientId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ImportFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserUid = c.String(),
                        FileName = c.String(),
                        Location = c.String(),
                        Size = c.Int(nullable: false),
                        Data = c.String(),
                        BinaryData = c.Binary(),
                        CreatedDate = c.DateTime(nullable: false),
                        DateImported = c.DateTime(),
                        ImportSuccess = c.Boolean(nullable: false),
                        ImportTypeId = c.Int(nullable: false),
                        AdvisorId = c.Int(nullable: false),
                        FieldMap_FirstWorksheetName = c.String(),
                        FieldMap_TotalRows = c.Int(nullable: false),
                        FieldMap_TotalColumns = c.Int(nullable: false),
                        FieldMap_SkipLines = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ImportTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        ActionUrl = c.String(),
                        SupportedFileTypes = c.String(),
                        Enabled = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LicenseCategories",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(maxLength: 60),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        LastLoginDate = c.DateTime(),
                        JoinDate = c.DateTime(),
                        AdvisorId = c.Int(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Advisors", t => t.AdvisorId, cascadeDelete: true)
                .Index(t => t.AdvisorId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AdvisorTypeDocumentTypes",
                c => new
                    {
                        AdvisorType_Id = c.Int(nullable: false),
                        DocumentType_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.AdvisorType_Id, t.DocumentType_Id })
                .ForeignKey("dbo.AdvisorTypes", t => t.AdvisorType_Id, cascadeDelete: true)
                .ForeignKey("dbo.DocumentTypes", t => t.DocumentType_Id, cascadeDelete: true)
                .Index(t => t.AdvisorType_Id)
                .Index(t => t.DocumentType_Id);
            
            CreateTable(
                "dbo.ProductApplications",
                c => new
                    {
                        Product_Id = c.Int(nullable: false),
                        Application_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Product_Id, t.Application_Id })
                .ForeignKey("dbo.Products", t => t.Product_Id, cascadeDelete: true)
                .ForeignKey("dbo.Applications", t => t.Application_Id, cascadeDelete: true)
                .Index(t => t.Product_Id)
                .Index(t => t.Application_Id);
            
            CreateTable(
                "dbo.SupplierLicenseTypes",
                c => new
                    {
                        Supplier_Id = c.Int(nullable: false),
                        LicenseType_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Supplier_Id, t.LicenseType_Id })
                .ForeignKey("dbo.Suppliers", t => t.Supplier_Id, cascadeDelete: true)
                .ForeignKey("dbo.LicenseTypes", t => t.LicenseType_Id, cascadeDelete: true)
                .Index(t => t.Supplier_Id)
                .Index(t => t.LicenseType_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "AdvisorId", "dbo.Advisors");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Transactions", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.LicenseTypes", "Advisor_Id", "dbo.Advisors");
            DropForeignKey("dbo.Advisors", "ContactId", "dbo.Contacts");
            DropForeignKey("dbo.LicenseTypes", "Company_Id", "dbo.Companies");
            DropForeignKey("dbo.Companies", "ContactDetails_Id", "dbo.Contacts");
            DropForeignKey("dbo.Advisors", "Company_Id", "dbo.Companies");
            DropForeignKey("dbo.Queries", "QueryType_Id", "dbo.QueryTypes");
            DropForeignKey("dbo.Queries", "Application_Id", "dbo.Applications");
            DropForeignKey("dbo.Queries", "Advisor_Id", "dbo.Advisors");
            DropForeignKey("dbo.Applications", "Client_Id", "dbo.Contacts");
            DropForeignKey("dbo.Applications", "ApplicationStatus_Id", "dbo.ApplicationStatus");
            DropForeignKey("dbo.ApplicationDocuments", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "Supplier_Id", "dbo.Suppliers");
            DropForeignKey("dbo.Suppliers", "MainContact_Id", "dbo.Contacts");
            DropForeignKey("dbo.Contacts", "ContactType_Id", "dbo.ContactTypes");
            DropForeignKey("dbo.Addresses", "Province_Id", "dbo.Provinces");
            DropForeignKey("dbo.Addresses", "Country_Id", "dbo.Countries");
            DropForeignKey("dbo.Addresses", "Contact_Id", "dbo.Contacts");
            DropForeignKey("dbo.Addresses", "AddressType_Id", "dbo.AddressTypes");
            DropForeignKey("dbo.SupplierLicenseTypes", "LicenseType_Id", "dbo.LicenseTypes");
            DropForeignKey("dbo.SupplierLicenseTypes", "Supplier_Id", "dbo.Suppliers");
            DropForeignKey("dbo.Accounts", "Supplier_Id", "dbo.Suppliers");
            DropForeignKey("dbo.Products", "LicenseTypeId", "dbo.LicenseTypes");
            DropForeignKey("dbo.ProductApplications", "Application_Id", "dbo.Applications");
            DropForeignKey("dbo.ProductApplications", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.ApplicationDocuments", "DocumentTypeId", "dbo.DocumentTypes");
            DropForeignKey("dbo.ApplicationDocuments", "ApplicationId", "dbo.Applications");
            DropForeignKey("dbo.Applications", "Advisor_Id", "dbo.Advisors");
            DropForeignKey("dbo.Advisors", "AdvisorStatus_Id", "dbo.AdvisorStatus");
            DropForeignKey("dbo.AdvisorDocuments", "DocumentTypeId", "dbo.DocumentTypes");
            DropForeignKey("dbo.AdvisorTypeDocumentTypes", "DocumentType_Id", "dbo.DocumentTypes");
            DropForeignKey("dbo.AdvisorTypeDocumentTypes", "AdvisorType_Id", "dbo.AdvisorTypes");
            DropForeignKey("dbo.Advisors", "AdvisorType_Id", "dbo.AdvisorTypes");
            DropForeignKey("dbo.AdvisorDocuments", "Advisor_Id", "dbo.Advisors");
            DropForeignKey("dbo.Accounts", "Advisor_Id", "dbo.Advisors");
            DropIndex("dbo.SupplierLicenseTypes", new[] { "LicenseType_Id" });
            DropIndex("dbo.SupplierLicenseTypes", new[] { "Supplier_Id" });
            DropIndex("dbo.ProductApplications", new[] { "Application_Id" });
            DropIndex("dbo.ProductApplications", new[] { "Product_Id" });
            DropIndex("dbo.AdvisorTypeDocumentTypes", new[] { "DocumentType_Id" });
            DropIndex("dbo.AdvisorTypeDocumentTypes", new[] { "AdvisorType_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "AdvisorId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Transactions", new[] { "AccountId" });
            DropIndex("dbo.Companies", new[] { "ContactDetails_Id" });
            DropIndex("dbo.Queries", new[] { "QueryType_Id" });
            DropIndex("dbo.Queries", new[] { "Application_Id" });
            DropIndex("dbo.Queries", new[] { "Advisor_Id" });
            DropIndex("dbo.Addresses", new[] { "Province_Id" });
            DropIndex("dbo.Addresses", new[] { "Country_Id" });
            DropIndex("dbo.Addresses", new[] { "Contact_Id" });
            DropIndex("dbo.Addresses", new[] { "AddressType_Id" });
            DropIndex("dbo.Contacts", new[] { "ContactType_Id" });
            DropIndex("dbo.Suppliers", new[] { "MainContact_Id" });
            DropIndex("dbo.LicenseTypes", new[] { "Advisor_Id" });
            DropIndex("dbo.LicenseTypes", new[] { "Company_Id" });
            DropIndex("dbo.Products", new[] { "Supplier_Id" });
            DropIndex("dbo.Products", new[] { "LicenseTypeId" });
            DropIndex("dbo.ApplicationDocuments", new[] { "DocumentTypeId" });
            DropIndex("dbo.ApplicationDocuments", new[] { "ApplicationId" });
            DropIndex("dbo.ApplicationDocuments", new[] { "ProductId" });
            DropIndex("dbo.Applications", new[] { "Client_Id" });
            DropIndex("dbo.Applications", new[] { "ApplicationStatus_Id" });
            DropIndex("dbo.Applications", new[] { "Advisor_Id" });
            DropIndex("dbo.AdvisorDocuments", new[] { "Advisor_Id" });
            DropIndex("dbo.AdvisorDocuments", new[] { "DocumentTypeId" });
            DropIndex("dbo.Advisors", new[] { "Company_Id" });
            DropIndex("dbo.Advisors", new[] { "AdvisorStatus_Id" });
            DropIndex("dbo.Advisors", new[] { "AdvisorType_Id" });
            DropIndex("dbo.Advisors", new[] { "ContactId" });
            DropIndex("dbo.Accounts", new[] { "Supplier_Id" });
            DropIndex("dbo.Accounts", new[] { "Advisor_Id" });
            DropTable("dbo.SupplierLicenseTypes");
            DropTable("dbo.ProductApplications");
            DropTable("dbo.AdvisorTypeDocumentTypes");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.LicenseCategories");
            DropTable("dbo.ImportTypes");
            DropTable("dbo.ImportFiles");
            DropTable("dbo.CommissionStatements");
            DropTable("dbo.AdvisorSupplierCodes");
            DropTable("dbo.Transactions");
            DropTable("dbo.Companies");
            DropTable("dbo.QueryTypes");
            DropTable("dbo.Queries");
            DropTable("dbo.ApplicationStatus");
            DropTable("dbo.ContactTypes");
            DropTable("dbo.Provinces");
            DropTable("dbo.Countries");
            DropTable("dbo.AddressTypes");
            DropTable("dbo.Addresses");
            DropTable("dbo.Contacts");
            DropTable("dbo.Suppliers");
            DropTable("dbo.LicenseTypes");
            DropTable("dbo.Products");
            DropTable("dbo.ApplicationDocuments");
            DropTable("dbo.Applications");
            DropTable("dbo.AdvisorStatus");
            DropTable("dbo.AdvisorTypes");
            DropTable("dbo.DocumentTypes");
            DropTable("dbo.AdvisorDocuments");
            DropTable("dbo.Advisors");
            DropTable("dbo.Accounts");
        }
    }
}
