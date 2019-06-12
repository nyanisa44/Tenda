using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using OfficeOpenXml;
using TendaAdvisors.Models;
using TendaAdvisors.Models.Admin;
using TendaAdvisors.Providers;
using TendaAdvisors.ViewModels;
using TendaAdvisors.Models.Response;
using System.Data.SqlTypes;
using System.Net;

namespace TendaAdvisors.Controllers
{
    public class FieldMap
    {
        public string FirstWorksheetName { get; set; }
        public int TotalRows { get; set; }
        public int TotalColumns { get; set; }
        public int SkipLines { get; set; }
        public string[] DbFields { get; set; }
        public string[][] FileFields { get; set; }
        public string[][] MappingData { get; set; }
    }

    public class AdminController : BaseApiController
    {
        private DateTime vatEffectiveDate = new DateTime(2018, 4, 1);
        private readonly string workingFolder = HttpRuntime.AppDomainAppPath + @"Uploads\ImportFiles";
        private readonly string exportFolder = HttpRuntime.AppDomainAppPath + @"Uploads\ExportFiles";

        private decimal calVat(decimal excludingVat, decimal includingVat)
        {
            if (excludingVat == 0) return 0;

            decimal difference = includingVat - excludingVat;
            decimal vat = difference / excludingVat * 100;
            decimal vatRate = (Math.Round(vat));

            return vatRate;
        }

        public AdminController(ApplicationDbContext dbcontext)
        {
            db = dbcontext;
        }

        private async Task<ApplicationUser> GetUser()
        {
            return await HttpContext.Current.GetOwinContext()
                .GetUserManager<ApplicationUserManager>()
                .FindByIdAsync(HttpContext.Current.User.Identity.GetUserId());
        }

        private AdminController()
        {
            if (!Directory.Exists(this.workingFolder))
            {
                Directory.CreateDirectory(this.workingFolder);
            }

            if (!Directory.Exists(this.exportFolder))
            {
                Directory.CreateDirectory(this.exportFolder);
            }
        }

        private async Task<ImportFile> GetCSVMeta(String location)
        {
            ImportFile result = new ImportFile();
            try
            {
                using (StreamReader sr = File.OpenText(location))
                {
                    string str = String.Empty;
                    var _user = await GetUser();
                    string userId = "unknown";
                    int advisorId = 0;
                    if (_user != null)
                    {
                        userId = _user.Id;
                        advisorId = _user.AdvisorId;
                    }

                    if ((str = sr.ReadLine()) != null)
                    {
                        str = str.Replace(@"\", "").Replace("\"", "");
                        string[] headers = str.Split(',');
                        string[] contactFields = typeof(Contact).GetProperties()
                                    .Select(property => property.Name + " (" + typeof(Contact).Name + ')')
                                    .ToArray();

                        string[] applicationFields = typeof(Application).GetProperties()
                                    .Select(property => property.Name + " (" + typeof(Application).Name + ')')
                                    .ToArray();

                        string[] addressFields = typeof(Address).GetProperties()
                                    .Select(property => property.Name + " (" + typeof(Address).Name + ')')
                                    .ToArray();

                        FieldMap map = new FieldMap();
                        map.FirstWorksheetName = "1";
                        map.TotalRows = 0;
                        map.TotalColumns = headers.Count();
                        //Join DB mapping all options
                        map.DbFields = contactFields;
                        map.DbFields = addressFields.Concat(map.DbFields).ToArray();
                        map.DbFields = applicationFields.Concat(map.DbFields).ToArray();

                        var end = headers.Count();
                        map.FileFields = new string[15][];

                        for (int row = 0; row <= 0; row++)
                        { // Row by row...
                            //var obj = worksheet.Cells[row, 1, row, end.Column];
                            map.FileFields[row] = new string[1];
                            for (int col = 0; col < headers.Count(); col++)
                            {
                                map.FileFields[row][col] = headers[col]; // This got me the actual value I needed.
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }

        // GET: api/Admin/Process/1
        [Route("Admin/Process/{importTypeId}")]
        [ResponseType(typeof(IEnumerable<ImportFile>))]
        public async Task<IHttpActionResult> GetImportFileListByImportTypeId(int importTypeId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (importTypeId == 0)
            {
                return Ok();
            }

            List<ImportFile> importFiles = await db.ImportFiles
                .Where(i => i.ImportTypeId == importTypeId)
                .Where(i => i.ImportSuccess != true)
                .ToListAsync();

            if (importFiles != null)
            {
                string[] contactFields = typeof(Contact)
                    .GetProperties()
                    .Select(property => property.Name + " (" + typeof(Contact).Name + ')')
                    .ToArray();

                string[] applicationFields = typeof(Application)
                    .GetProperties()
                    .Select(property => property.Name + " (" + typeof(Application).Name + ')')
                    .ToArray();

                string[] addressFields = typeof(Address).GetProperties()
                    .Select(property => property.Name + " (" + typeof(Address).Name + ')')
                    .ToArray();

                foreach (var importFile in importFiles)
                {
                    try
                    {
                        FileInfo file = new FileInfo(importFile.Location);
                        if (!file.Exists)
                            continue;

                        if (file.Extension != ".csv")
                        {
                            using (ExcelPackage excelFile = new ExcelPackage(file))
                            {
                                ExcelWorksheet worksheet = excelFile.Workbook.Worksheets.First();

                                FieldMap map = new FieldMap()
                                {
                                    FirstWorksheetName = worksheet.Name,
                                    TotalColumns = worksheet.Dimension.Columns,
                                    TotalRows = worksheet.Dimension.Rows,
                                };
                                
                                //Join DB mapping all options
                                map.DbFields = contactFields;
                                map.DbFields = addressFields.Concat(map.DbFields).ToArray();
                                map.DbFields = applicationFields.Concat(map.DbFields).ToArray();

                                var start = worksheet.Dimension.Start;
                                var end = worksheet.Dimension.End;
                                map.FileFields = new string[15][];

                                for (int row = start.Row; row <= 15; row++)
                                { // Row by row...
                                    map.FileFields[row - 1] = new string[end.Column];
                                    for (int col = start.Column; col <= end.Column; col++)
                                    { // ... Cell by cell...
                                        map.FileFields[row - 1][col - 1] = worksheet.Cells[row, col].Text; // This got me the actual value I needed.
                                    }
                                }

                                importFile.FieldMap = map;
                            }
                        }
                        else
                        {
                            using (StreamReader sr = File.OpenText(importFile.Location))
                            {
                                string str = String.Empty;

                                int row = 0;
                                int colCount = 0;

                                FieldMap map = new FieldMap();
                                map.FirstWorksheetName = importFile.FileName;
                                map.FileFields = new string[15][];

                                //Join DB mapping all options
                                map.DbFields = contactFields;
                                map.DbFields = addressFields.Concat(map.DbFields).ToArray();
                                map.DbFields = applicationFields.Concat(map.DbFields).ToArray();

                                while ((str = sr.ReadLine()) != null && row <= 15)
                                {
                                    if (row < 1)
                                    {
                                        row++;
                                        str = sr.ReadLine();
                                    }

                                    str = str.Replace(@"\", "").Replace("\"", "");
                                    string[] item = str.Split(',');

                                    colCount = (item.Length > colCount) ? item.Length : colCount;

                                    map.FileFields[row - 1] = new string[item.Length];

                                    for (int col = 0; col < item.Length; col++)
                                    {
                                        map.FileFields[row - 1][col] = item[col];
                                    }

                                    row++;
                                }

                                map.TotalRows = row;
                                map.TotalColumns = colCount;

                                importFile.FieldMap = map;
                            }
                        }
                    }
                    catch
                    {
                        return BadRequest();
                    }
                }

                if (importFiles.Count() > 0)
                {
                    var ImportFilesStatus = importFiles
                        .Join(db.CommissionFileStatus.Where(e => e.Status != "Completed"),
                        imp => imp.Id,
                        comm => comm.ImportFileId,
                        (imp, comm) => new ImportFileResponseStatus
                        {
                            Id = imp.Id,
                            UserUid = imp.UserUid,
                            FileName = imp.FileName,
                            Location = imp.Location,
                            Size = imp.Size,
                            Data = imp.Data,
                            BinaryData = imp.BinaryData,
                            CreatedDate = imp.CreatedDate,
                            DateImported = imp.DateImported,
                            ImportSuccess = imp.ImportSuccess,
                            ImportTypeId = imp.ImportTypeId,
                            AdvisorId = imp.AdvisorId,
                            status = comm.Status
                        }).ToList();

                    return Ok(ImportFilesStatus);
                }
                else
                {
                    return NotFound();
                }
            }
            return Ok();
        }

        // TODO: Here we will be adding the fucntionality to pull from the db
        // GET: api/Admin/MemberListException
        [Route("Admin/MemberListExceptionFile/")]
        public async Task<HttpResponseMessage> MemberListExceptionFile()
        {
            List<MemberListException> memberListExceptionsEntries = null;
            memberListExceptionsEntries = await db.MemberListExceptions.ToListAsync();

            if (memberListExceptionsEntries != null)
            {
                string[] memberListExceptionFields = typeof(MemberListException).GetProperties()
                    .Select(property => property.Name + " (" + typeof(MemberListException).Name + ')')
                    .ToArray();

                FileInfo excelFile;

                using (ExcelPackage excel = new ExcelPackage())
                {
                    excel.Workbook.Worksheets.Add("MemberListException");
                    var headerRow = new List<string[]>()
                    {
                        new string[] { "MemberNumber", "IdNumber", "Initials", "Surname", "Date Created", "Reason", "Linked Advisor Name"},
                    };

                    // Determine the header range (e.g. A1:D1)
                    string headerRange = "A1:" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + "1";

                    // Target a worksheet
                    var worksheet = excel.Workbook.Worksheets["MemberListException"];

                    // Populate header row data
                    worksheet.Cells[headerRange].LoadFromArrays(headerRow);

                    string fileName = (
                        exportFolder + "\\"
                        + "MemberListException_"
                        + (DateTime.Now.Ticks.ToString()).Replace(@"/", "-")
                        + ".xlsx"
                    );

                    excelFile = new FileInfo(fileName);
                    excel.SaveAs(excelFile);
                }

                try
                {
                    if (excelFile != null)
                    {
                        using (ExcelPackage excel = new ExcelPackage(excelFile))
                        {
                            ExcelWorksheet worksheet = excel.Workbook.Worksheets["MemberListException"];
                            var row = worksheet.Dimension.Start.Row + 1;
                            var linkedAdvisorName = "Not Found";

                            foreach (MemberListException member in memberListExceptionsEntries)
                            {
                                if (member.MemberId != null)
                                {
                                    Application application = db.Applications.Where(e => e.ApplicationNumber == member.MemberId).FirstOrDefault();
                                    if (application != null)
                                    {
                                        var linkedAdvisorId = application.Advisor_Id ?? 0;
                                        
                                        if (linkedAdvisorId != 0)
                                        {
                                            Advisor advisorContact = db.Advisors
                                                .Where(advisor => advisor.Id == linkedAdvisorId)
                                                .FirstOrDefault();

                                            linkedAdvisorName = advisorContact.User.FullName;
                                        }
                                    }
                                }
                                var cellData = new List<string[]>()
                                {
                                    new string[] {
                                        member.MemberId,
                                        member.IdNumber,
                                        member.Initials,
                                        member.MemberSurname,
                                        member.DateCreated.ToShortDateString(),
                                        member.Reason,
                                        linkedAdvisorName
                                    }
                                };
                                string cellRangeStart = "A" + row.ToString();
                                string cellRange = cellRangeStart + ":" + Char.ConvertFromUtf32(cellData[0].Length + 64) + row.ToString();
                                worksheet.Cells[cellRange].LoadFromArrays(cellData);
                                row++;
                            }
                            excel.Save();
                        }

                        var stream = new FileStream(excelFile.FullName, FileMode.Open, FileAccess.Read);

                        var result = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StreamContent(stream)
                        };

                        result.Content.Headers.ContentDisposition =
                            new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                            {
                                FileName = excelFile.Name
                            };
                        result.Content.Headers.ContentType =
                            new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                        return result;
                    }
                    else
                    {
                        var result = new HttpResponseMessage(HttpStatusCode.NotFound);
                        return result;
                    }

                }
                catch(Exception e)
                {
                    var result = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    return result;
                }
            }
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
        
        [HttpPost]
        [Route("Admin/MemberListExceptionTableTruncation")]
        public async Task<HttpResponseMessage> MemberListExceptionTableTruncation()
        {
            try {
                List<MemberListException> listOfEntries = await db.MemberListExceptions.ToListAsync();
                db.MemberListExceptions.RemoveRange(listOfEntries);
                await db.SaveChangesAsync();

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch(Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        [Route("Admin/CommissionExceptionTableTruncation")]
        public async Task<HttpResponseMessage> CommissionExceptionTableTruncation()
        {
            try
            {
                List<UnmatchedCommissions> listOfEntries = await db.UnmatchedCommissions.ToListAsync();
                db.UnmatchedCommissions.RemoveRange(listOfEntries);
                await db.SaveChangesAsync();
            }
            catch(Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        // PUT: api/Admin/Save/FieldMap/5
        // Save/Update a file upload field map.
        [Route("Admin/Save/FieldMap/{importFileId}")]
        [ResponseType(typeof(ImportFile))]
        public async Task<IHttpActionResult> PutFieldMap(int importFileId, FieldMap fieldMapping)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ImportFile fileEntity = db.ImportFiles.Find(importFileId);

            if (fileEntity == null)
            {
                return NotFound();
            }

            fileEntity.FieldMap = fieldMapping;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                return NotFound();
            }

            return Ok(importFileId);
        }

        // PUT: api/Admin/UpdateFileOwner/5
        // Finds an already uploaded comission .csv and imports into CommissionStatement Table.
        [Route("Admin/UpdateFileOwner/{importFileId}/{advisorId}")]
        public async Task<IHttpActionResult> UpdateImportFileOwner(int importFileId, int advisorId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ImportFile fileEntity = db.ImportFiles.Find(importFileId);

            if (fileEntity == null)
            {
                return NotFound();
            }

            fileEntity.AdvisorId = advisorId;
            await db.SaveChangesAsync();

            return Ok(fileEntity);
        }

        // PUT: api/Admin/Process/Commission/5
        // Finds an already uploaded comission .csv and imports into CommissionStatement Table.
        [HttpPut]
        [Route("Admin/Process/Commission/{importFileId}/{selectedSupplierID}")]
        [ResponseType(typeof(ImportFile))]
        public async Task<IHttpActionResult> PutProcessCommission(int importFileId, int selectedSupplierID)
        {

            //Note: 
            //Commission date according to csv file 
            SqlDateTime SQLMinDate = SqlDateTime.MinValue;
            DateTime commissionDateFromSVC = (DateTime)SQLMinDate;
            DateTime endDate = (DateTime)SQLMinDate;
            DateTime transactionDate = (DateTime)SQLMinDate;

            var mySupplier = db.Suppliers.Include("Licenses").SingleOrDefault(a => a.Id == selectedSupplierID);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ImportFile fileEntity = db.ImportFiles.Find(importFileId);

            if (fileEntity == null)
            {
                return NotFound();
            }

            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.Currency;
            culture.NumberFormat.CurrencySymbol = "R";
            List<CommissionStatement> rows = new List<CommissionStatement>();
            List<CommissionStatement> rowsNotAdded = new List<CommissionStatement>();
            List<UnmatchedCommissions> unmatchedCommissionList = new List<UnmatchedCommissions>();
            List<string> skippedRows = new List<string>();
            int advisorId = fileEntity.AdvisorId;

            var suppliersShares = await (from x in db.Suppliers
                                   join y in db.AdvisorShareUnderSupervisions
                                   on x.Name equals y.supplier
                                   select new
                                   {
                                       AdviserId = y.AdvisorId,
                                       SupplierId = x.Id,
                                       y.validCommissionFromDate,
                                       y.validCommissionToDate
                                   })
                                   .ToListAsync();

            List<int> Products = await db.Products
                                        .Where(x => x.SupplierId == selectedSupplierID)
                                        .Select(x => x.Id)
                                        .ToListAsync();

            var ext = Path.GetExtension(fileEntity.FileName);
            if (ext != ".csv")
            { 
                try
                {
                    FileInfo file = new FileInfo(fileEntity.Location);

                    using (ExcelPackage excelFile = new ExcelPackage(file))
                    {
                        ExcelWorksheet worksheet = excelFile.Workbook.Worksheets.First();

                        var start = worksheet.Dimension.Start;
                        var end = worksheet.Dimension.End;
                        DateTime runDate = DateTime.Now;
                        // Todo: get logged in user name;
                        string runUser = "CurrentUser";

                        for (int row = start.Row + 1; row <= end.Row; row++)
                        {   
                            // Row by row...
                            DateTime enrollmentDate = (DateTime)SQLMinDate;
                            DateTime terminationDate = (DateTime)SQLMinDate;
                            
                            decimal subscriptionDue = 0.00m;
                            decimal subscriptionReceived = 0.00m;
                            decimal commissionInclVat = 0.00m;
                            decimal commissionExclVat = 0.00m;

                            CommissionStatement commissionStatement = new CommissionStatement();
                            try
                            {
                                // Search by ID number, Member Number or Policy Number
                                Application app = null;

                                string worksheetIdNumber = mySupplier.IDNumberColumn == 0
                                    ? ""
                                    : worksheet.Cells[row, mySupplier.IDNumberColumn].Text.Normalize().Trim();

                                string worksheetMemberNumber = mySupplier.MemberNumberColumn == 0
                                    ? ""
                                    : worksheet.Cells[row, mySupplier.MemberNumberColumn].Text.Normalize().Trim();

                                string worksheetPolicyNumber = mySupplier.PolicyNumberColumn == 0
                                    ? ""
                                    : worksheet.Cells[row, mySupplier.MemberNumberColumn].Text.Normalize();

                                string worksheetPreCompInclun = mySupplier.CommissionInclVatColumn == 0
                                    ? "0"
                                    : worksheet.Cells[row, mySupplier.CommissionInclVatColumn].Text.Normalize();

                                string worksheetPreCompExclun = mySupplier.CommissionExclVatColumn == 0
                                    ? "0"
                                    : worksheet.Cells[row, mySupplier.CommissionExclVatColumn].Text.Normalize();

                                string worksheetSurname = mySupplier.SurnameColumn == 0
                                    ? ""
                                    : worksheet.Cells[row, mySupplier.SurnameColumn].Text.Normalize();

                                string worksheetInitial = mySupplier.InitialColumn == 0
                                    ? ""
                                    : worksheet.Cells[row, mySupplier.InitialColumn].Text.Normalize();
                                
                                DateTime.TryParse(worksheet.Cells[row, mySupplier.TransactionDateColumn].Text, out transactionDate);
                                transactionDate = transactionDate < (DateTime)SQLMinDate ? (DateTime)SQLMinDate : transactionDate;

                                DateTime ? worksheetEnrollmentDate = mySupplier.EnrollmentDateColumn == 0
                                ? (DateTime)SQLMinDate
                                : DateTime.TryParse(worksheet.Cells[row, mySupplier.EnrollmentDateColumn].Text, out enrollmentDate)
                                    ? (DateTime?)enrollmentDate
                                    : (DateTime)SQLMinDate;

                                DateTime? worksheetTerminationDate = mySupplier.TerminationDateColumn == 0
                                    ? (DateTime)SQLMinDate
                                    : DateTime.TryParse(worksheet.Cells[row, mySupplier.TerminationDateColumn].Text, out terminationDate)
                                        ? (DateTime?)terminationDate
                                        : (DateTime)SQLMinDate;

                                decimal? worksheetSubscriptionDue = mySupplier.SubscriptionDueColumn == 0
                                    ? 0m
                                    : decimal.TryParse(worksheet.Cells[row, mySupplier.SubscriptionDueColumn].Text, out subscriptionDue)
                                        ? (decimal?)subscriptionDue
                                        : null;

                                decimal? worksheetSubscriptionReceived = mySupplier.SubscriptionReceivedColumn == 0
                                    ? 0m
                                    : decimal.TryParse(worksheet.Cells[row, mySupplier.SubscriptionReceivedColumn].Text, style, culture, out subscriptionReceived)
                                        ? (decimal?)subscriptionReceived
                                        : null;

                                decimal? worksheetCommissionInclVAT = mySupplier.CommissionInclVatColumn == 0
                                    ? 0m
                                    : (decimal.TryParse(worksheetPreCompInclun, style, culture, out commissionInclVat)
                                        ? (decimal?)commissionInclVat
                                        : null);

                                decimal? worksheetCommissionExclVAT = mySupplier.CommissionExclVatColumn == 0
                                    ? 0m
                                    : decimal.TryParse(worksheetPreCompExclun, style, culture, out commissionExclVat)
                                        ? (decimal?)commissionExclVat
                                        : null;

                                decimal advisorTaxRate = calVat(
                                    Convert.ToDecimal(commissionStatement.CommissionExclVAT),
                                    Convert.ToDecimal(commissionStatement.CommissionInclVAT));

                                if (worksheetPreCompInclun.Contains(',') && worksheetPreCompInclun.Contains('.') == false)
                                {
                                    worksheetPreCompInclun = worksheetPreCompInclun.Replace(',', '.');
                                }

                                if (worksheetPreCompExclun.Contains(',') && worksheetPreCompExclun.Contains('.') == false)
                                {
                                    worksheetPreCompExclun = worksheetPreCompExclun.Replace(',', '.');
                                }

                                commissionStatement.MemberSearchKey = null;

                                // Fall through search types
                                if (worksheetIdNumber != "")
                                {
                                    // Strip search non-numerics
                                    commissionStatement.MemberSearchValue = worksheetIdNumber;
                                    commissionStatement.MemberSearchKey = "IDNumber";
                                }

                                if (worksheetMemberNumber != "" &&
                                    commissionStatement.MemberSearchKey == null)
                                {
                                    commissionStatement.MemberSearchValue = worksheetMemberNumber;
                                    commissionStatement.MemberSearchKey = "MemberNumber";
                                }

                                if (transactionDate > (new DateTime(2016, 01, 01)) && commissionDateFromSVC == (DateTime)SQLMinDate)
                                {
                                    commissionDateFromSVC = transactionDate;

                                    // Set end date 
                                    endDate = new DateTime(
                                        transactionDate.Year,
                                        transactionDate.Month,
                                        DateTime.DaysInMonth(transactionDate.Year, transactionDate.Month)
                                    );
                                }

                                // If advisor status ID is NOT 5 i.e status not complete
                                Advisor advisor = await db.Advisors
                                    .Where(e => e.Id == advisorId)
                                    .FirstOrDefaultAsync();

                                Contact contact = null;

                                string advisorName = "";

                                if (advisor != null)
                                {
                                    contact = await db.Contacts.FirstOrDefaultAsync(e => e.Id == advisor.ContactId);
                                    advisorName = contact.FirstName + " " + contact.LastName;
                                }

                                if (commissionStatement.MemberSearchKey == "IDNumber")
                                {
                                    if (transactionDate == (DateTime)SQLMinDate)
                                    {
                                        transactionDate = commissionDateFromSVC;
                                    }

                                    contact = await db.Contacts
                                        .Where(x => x.IdNumber == commissionStatement.MemberSearchValue)
                                        .FirstOrDefaultAsync();
                                }
                                else if (commissionStatement.MemberSearchKey == "MemberNumber")
                                {
                                    if (transactionDate == (DateTime)SQLMinDate)
                                    {
                                        transactionDate = commissionDateFromSVC;
                                    }

                                    var clientId = db.Applications
                                        .Where(match => match.ApplicationNumber == commissionStatement.MemberSearchValue)
                                        .Select(entry => entry.Client_Id)
                                        .FirstOrDefault();

                                    if (clientId == null)
                                    {
                                        if (transactionDate == (DateTime)SQLMinDate)
                                        {
                                            transactionDate = commissionDateFromSVC;
                                        }

                                        UnmatchedCommissions unmatchedMember = new UnmatchedCommissions
                                        {
                                            Reasons = "Member number not linked to a user in database",
                                            AdvisorName = advisorName,
                                            MemberSearchValue = commissionStatement.MemberSearchValue,
                                            MemberSearchKey = commissionStatement.MemberSearchKey,
                                            CommisionRunDate = runDate,
                                            CommisionRunUser = runUser,
                                            MemberNumber = worksheetMemberNumber,
                                            supplierName = mySupplier.Name,
                                            ImportFileId = importFileId,
                                            SupplierId = selectedSupplierID,
                                            Surname = worksheetSurname,
                                            Initial = worksheetInitial,
                                            EnrollmentDate = worksheetEnrollmentDate,
                                            TerminationDate = worksheetTerminationDate,
                                            TransactionDate = transactionDate,
                                            SubscriptionDue = worksheetSubscriptionDue,
                                            SubscriptionReceived = worksheetSubscriptionReceived,
                                            CommissionInclVAT = worksheetCommissionInclVAT,
                                            CommissionExclVAT = worksheetCommissionExclVAT,
                                        };

                                        bool duplicateInsertCheck = unmatchedCommissionList
                                                .Where(e => e.MemberSearchValue == unmatchedMember.MemberSearchValue)
                                                .Any();

                                        if (!duplicateInsertCheck)
                                        {
                                            unmatchedCommissionList.Add(unmatchedMember);
                                        }
                                    }
                                    else
                                    {
                                        contact = await db.Contacts
                                        .Where(x => x.Id == clientId)
                                        .FirstOrDefaultAsync();
                                    }
                                }

                                UnmatchedCommissions commissionUnmatched = null;

                                UnmatchedCommissions matchedCommissionCheck = await db.UnmatchedCommissions
                                    .FirstOrDefaultAsync(e =>
                                        e.MemberSearchValue == commissionStatement.MemberSearchValue &&
                                        e.TransactionDate.Value.Month == commissionDateFromSVC.Month);

                                if (contact != null)
                                {
                                    app = await db.Applications
                                        .Where(x => x.Client_Id == contact.Id && Products.Contains(x.Product_Id.Value))
                                        .FirstOrDefaultAsync();

                                    if (!Equals(matchedCommissionCheck, null))
                                    {
                                        commissionUnmatched = await db.UnmatchedCommissions
                                            .FirstOrDefaultAsync(e =>
                                                e.MemberSearchValue == commissionStatement.MemberSearchValue &&
                                                e.TransactionDate.Value.Month == commissionDateFromSVC.Month);
                                    }

                                    // Check if client's application is in commission 
                                    if (Equals(app, null))
                                    {
                                        if (Equals(matchedCommissionCheck, null))
                                        {
                                            UnmatchedCommissions unmatchedApplication = new UnmatchedCommissions
                                            {
                                                AdvisorName = "",
                                                Reasons = "No application assigned to client",
                                                MemberSearchValue = commissionStatement.MemberSearchValue,
                                                MemberSearchKey = commissionStatement.MemberSearchKey,
                                                CommisionRunDate = runDate,
                                                CommisionRunUser = runUser,
                                                TransactionDate = transactionDate,
                                                MemberNumber = worksheetMemberNumber,
                                                supplierName = mySupplier.Name,
                                                ImportFileId = importFileId,
                                                SupplierId = selectedSupplierID,
                                                Surname = worksheetSurname,
                                                Initial = worksheetInitial,
                                                EnrollmentDate = worksheetEnrollmentDate,
                                                TerminationDate = worksheetTerminationDate,
                                                SubscriptionDue = worksheetSubscriptionDue,
                                                SubscriptionReceived = worksheetSubscriptionReceived,
                                                CommissionInclVAT = worksheetCommissionInclVAT,
                                                CommissionExclVAT = worksheetCommissionExclVAT,
                                            };

                                            bool duplicateInsertCheck = unmatchedCommissionList
                                                .Where(e => e.MemberSearchValue == unmatchedApplication.MemberSearchValue)
                                                .Any();

                                            if (!duplicateInsertCheck)
                                            {
                                                unmatchedCommissionList.Add(unmatchedApplication);
                                            }
                                        }
                                        else
                                        {
                                            // Group the applications into one 
                                            commissionUnmatched.CommissionExclVAT += worksheetCommissionExclVAT;
                                            commissionUnmatched.CommissionInclVAT += worksheetCommissionInclVAT;
                                        }
                                    }
                                    else
                                    {
                                        if(advisor != null)
                                        {
                                            if (advisor.AdvisorStatus_Id != 5)
                                            {
                                                UnmatchedCommissions memberSearchValue = await db.UnmatchedCommissions
                                                    .FirstOrDefaultAsync(e => e.MemberSearchValue == commissionStatement.MemberSearchValue);

                                                if (!Equals(memberSearchValue, null))
                                                {
                                                    commissionUnmatched = await db.UnmatchedCommissions
                                                        .FirstOrDefaultAsync(e => e.MemberSearchValue == commissionStatement.MemberSearchValue);
                                                }

                                                if (Equals(commissionUnmatched, null))
                                                {
                                                    UnmatchedCommissions unmatchedAdvisor = new UnmatchedCommissions
                                                    {
                                                        Reasons = " Advisor not complete ",
                                                        AdvisorName = advisorName,
                                                        MemberSearchValue = commissionStatement.MemberSearchValue,
                                                        MemberSearchKey = commissionStatement.MemberSearchKey,
                                                        CommisionRunDate = runDate,
                                                        CommisionRunUser = runUser,
                                                        TransactionDate = transactionDate,
                                                        Surname = worksheetSurname,
                                                        Initial = worksheetInitial,
                                                        EnrollmentDate = worksheetEnrollmentDate,
                                                        TerminationDate = worksheetTerminationDate,
                                                        SubscriptionDue = worksheetSubscriptionDue,
                                                        SubscriptionReceived = worksheetSubscriptionReceived,
                                                        MemberNumber = worksheetMemberNumber,
                                                        supplierName = mySupplier.Name,
                                                        ImportFileId = importFileId,
                                                        SupplierId = selectedSupplierID,
                                                        CommissionInclVAT = worksheetCommissionInclVAT,
                                                        CommissionExclVAT = worksheetCommissionExclVAT,
                                                    };
                                                    
                                                    bool duplicateInsertCheck = unmatchedCommissionList
                                                        .Where(e => e.MemberSearchValue == unmatchedAdvisor.MemberSearchValue)
                                                        .Any();

                                                    if (!duplicateInsertCheck)
                                                    {
                                                        unmatchedCommissionList.Add(unmatchedAdvisor);
                                                    }
                                                }
                                                else
                                                {
                                                    // Group the applications into one 
                                                    commissionUnmatched.CommissionExclVAT += worksheetCommissionExclVAT;
                                                    commissionUnmatched.CommissionInclVAT += worksheetCommissionInclVAT;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            bool validCommission = false;
                                            if(app.Advisor != null)
                                            {
                                                if (app.Advisor.EffectiveEndDate != null && app.Advisor.EffectiveEndDate <= endDate)
                                                {
                                                    validCommission = true;
                                                }

                                                // Get commission runDate and test with adviser's effective start date 
                                                if (app.Advisor.EffectiveStartDate > endDate || validCommission)
                                                {

                                                    if (!Equals(matchedCommissionCheck, null))
                                                    {
                                                        commissionUnmatched = matchedCommissionCheck;
                                                    }

                                                    if (Equals(commissionUnmatched, null))
                                                    {
                                                        UnmatchedCommissions unmatchedDates = new UnmatchedCommissions
                                                        {
                                                            Reasons = " Adviser Effective date not in range ",
                                                            AdvisorName = advisorName,
                                                            MemberSearchValue = commissionStatement.MemberSearchValue,
                                                            MemberSearchKey = commissionStatement.MemberSearchKey,
                                                            CommisionRunDate = runDate,
                                                            CommisionRunUser = runUser,
                                                            TransactionDate = transactionDate,
                                                            Surname = worksheetSurname,
                                                            Initial = worksheetInitial,
                                                            EnrollmentDate = worksheetEnrollmentDate,
                                                            TerminationDate = worksheetTerminationDate,
                                                            SubscriptionDue = worksheetSubscriptionDue,
                                                            SubscriptionReceived = worksheetSubscriptionReceived,
                                                            MemberNumber = worksheetMemberNumber,
                                                            supplierName = mySupplier.Name,
                                                            ImportFileId = importFileId,
                                                            SupplierId = selectedSupplierID,
                                                            CommissionInclVAT = worksheetCommissionInclVAT,
                                                            CommissionExclVAT = worksheetCommissionExclVAT,
                                                        };

                                                        bool duplicateInsertCheck = unmatchedCommissionList
                                                        .Where(e => e.MemberSearchValue == unmatchedDates.MemberSearchValue)
                                                        .Any();

                                                        if (!duplicateInsertCheck)
                                                        {
                                                            unmatchedCommissionList.Add(unmatchedDates);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        commissionUnmatched.CommissionExclVAT += worksheetCommissionExclVAT;
                                                        commissionUnmatched.CommissionInclVAT += worksheetCommissionInclVAT;
                                                    }
                                                }

                                                // Get effective date for the application 
                                                var appEffect = await db.ApplicationAdvisorHistory
                                                    .Where(e => e.Application_Id == app.Id && e.New_Advisor == app.Advisor.Id)
                                                    .ToArrayAsync();

                                                var applicationEffectiveDate = appEffect.Count() > 0 ? appEffect.Last() : null;

                                                // Get commission run date and test with application's effective start date .
                                                if (!Equals(applicationEffectiveDate, null))
                                                {
                                                    if (applicationEffectiveDate.DateStarted >= endDate)
                                                    {
                                                        if (!Equals(matchedCommissionCheck, null))
                                                        {
                                                            commissionUnmatched = matchedCommissionCheck;
                                                        }

                                                        if (Equals(commissionUnmatched, null))
                                                        {
                                                            UnmatchedCommissions unmatchedDates = new UnmatchedCommissions
                                                            {
                                                                Reasons = "Application's Effective date not in range",
                                                                AdvisorName = advisorName,
                                                                MemberSearchValue = commissionStatement.MemberSearchValue,
                                                                MemberSearchKey = commissionStatement.MemberSearchKey,
                                                                CommisionRunDate = runDate,
                                                                CommisionRunUser = runUser,
                                                                TransactionDate = transactionDate,
                                                                Surname = worksheetSurname,
                                                                Initial = worksheetInitial,
                                                                EnrollmentDate = worksheetEnrollmentDate,
                                                                TerminationDate = worksheetTerminationDate,
                                                                SubscriptionDue = worksheetSubscriptionDue,
                                                                SubscriptionReceived = worksheetSubscriptionReceived,
                                                                MemberNumber = worksheetMemberNumber,
                                                                supplierName = mySupplier.Name,
                                                                ImportFileId = importFileId,
                                                                SupplierId = selectedSupplierID,
                                                                CommissionInclVAT = worksheetCommissionInclVAT,
                                                                CommissionExclVAT = worksheetCommissionExclVAT,
                                                            };

                                                            bool duplicateInsertCheck = unmatchedCommissionList
                                                                .Where(e => e.MemberSearchValue == unmatchedDates.MemberSearchValue)
                                                                .Any();

                                                            if (!duplicateInsertCheck)
                                                            {
                                                                unmatchedCommissionList.Add(unmatchedDates);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            commissionUnmatched.CommissionExclVAT += worksheetCommissionExclVAT;
                                                            commissionUnmatched.CommissionInclVAT += worksheetCommissionInclVAT;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    UnmatchedCommissions unmatchedCommissions = new UnmatchedCommissions
                                                    {
                                                        AdvisorName = advisorName,
                                                        Reasons = "Check Adviser history and refresh unmatched commissions",
                                                        MemberSearchValue = commissionStatement.MemberSearchValue,
                                                        MemberSearchKey = commissionStatement.MemberSearchKey,
                                                        CommisionRunDate = runDate,
                                                        CommisionRunUser = runUser,
                                                        TransactionDate = transactionDate,
                                                        Surname = worksheetSurname,
                                                        Initial = worksheetInitial,
                                                        EnrollmentDate = worksheetEnrollmentDate,
                                                        TerminationDate = worksheetTerminationDate,
                                                        SubscriptionDue = worksheetSubscriptionDue,
                                                        SubscriptionReceived = worksheetSubscriptionReceived,
                                                        MemberNumber = worksheetMemberNumber,
                                                        supplierName = mySupplier.Name,
                                                        ImportFileId = importFileId,
                                                        SupplierId = selectedSupplierID,
                                                        CommissionInclVAT = worksheetCommissionInclVAT,
                                                        CommissionExclVAT = worksheetCommissionExclVAT,
                                                    };

                                                    bool duplicateInsertCheck = unmatchedCommissionList
                                                                .Where(e => e.MemberSearchValue == unmatchedCommissions.MemberSearchValue)
                                                                .Any();

                                                    if (!duplicateInsertCheck)
                                                    {
                                                        unmatchedCommissionList.Add(unmatchedCommissions);
                                                    }
                                                }
                                            }
                                        }
                                        
                                        // If application linked to the advisor is not valid expired
                                        var applinked = db.Applications
                                            .Where(e => e.Id == app.Id && e.Advisor_Id == app.Advisor_Id)
                                            .Any();

                                        if (!applinked)
                                        {
                                            if (!Equals(matchedCommissionCheck, null))
                                            {
                                                commissionUnmatched = matchedCommissionCheck;
                                            }

                                            if (Equals(commissionUnmatched, null))
                                            {
                                                UnmatchedCommissions unmatchedAdvisor = new UnmatchedCommissions
                                                {
                                                    AdvisorName = advisorName,
                                                    Reasons = "Advisor not linked to application",
                                                    MemberSearchValue = commissionStatement.MemberSearchValue,
                                                    MemberSearchKey = commissionStatement.MemberSearchKey,
                                                    CommisionRunDate = runDate,
                                                    CommisionRunUser = runUser,
                                                    TransactionDate = transactionDate,
                                                    Surname = worksheetSurname,
                                                    Initial = worksheetInitial,
                                                    EnrollmentDate = worksheetEnrollmentDate,
                                                    TerminationDate = worksheetTerminationDate,
                                                    SubscriptionDue = worksheetSubscriptionDue,
                                                    SubscriptionReceived = worksheetSubscriptionReceived,
                                                    MemberNumber = worksheetMemberNumber,
                                                    supplierName = mySupplier.Name,
                                                    ImportFileId = importFileId,
                                                    SupplierId = selectedSupplierID,
                                                    CommissionInclVAT = worksheetCommissionInclVAT,
                                                    CommissionExclVAT = worksheetCommissionExclVAT,
                                                    AdvisorTaxRate = advisorTaxRate,
                                                };

                                                bool duplicateInsertCheck = unmatchedCommissionList
                                                    .Where(e => e.MemberSearchValue == unmatchedAdvisor.MemberSearchValue)
                                                    .Any();

                                                if (!duplicateInsertCheck)
                                                {
                                                    unmatchedCommissionList.Add(unmatchedAdvisor);
                                                }
                                            }
                                            else
                                            {
                                                commissionUnmatched.CommissionExclVAT += worksheetCommissionExclVAT;
                                                commissionUnmatched.CommissionInclVAT += worksheetCommissionInclVAT;
                                            }
                                        }

                                        // If BR number in advisor documents is expired 
                                        var expired = db.AdvisorDocuments
                                            .Where(e => e.DocumentTypeId == 10 && e.ValidFromDate >= DateTime.Now && e.ValidToDate <= DateTime.Now)
                                            .Any();

                                        if (expired)
                                        {
                                            var tempSearchAndDateValue = await db.UnmatchedCommissions
                                                .FirstOrDefaultAsync(e => e.MemberSearchValue == commissionStatement.MemberSearchValue && e.TransactionDate.Value.Month == commissionDateFromSVC.Month);

                                            if (!Equals(tempSearchAndDateValue, null))
                                            {
                                                commissionUnmatched = await db.UnmatchedCommissions
                                                    .FirstOrDefaultAsync(e => e.MemberSearchValue == commissionStatement.MemberSearchValue && e.TransactionDate.Value.Month == commissionDateFromSVC.Month);
                                            }

                                            if (Equals(commissionUnmatched, null))
                                            {
                                                UnmatchedCommissions unmatchedDocuments = new UnmatchedCommissions
                                                {
                                                    Reasons = "CMS (BR) document expired",
                                                    AdvisorName = advisorName,
                                                    MemberSearchValue = commissionStatement.MemberSearchValue,
                                                    MemberSearchKey = commissionStatement.MemberSearchKey,
                                                    CommisionRunDate = runDate,
                                                    CommisionRunUser = runUser,
                                                    TransactionDate = transactionDate,
                                                    Surname = worksheetSurname,
                                                    Initial = worksheetInitial,
                                                    EnrollmentDate = worksheetEnrollmentDate,
                                                    TerminationDate = worksheetTerminationDate,
                                                    SubscriptionDue = worksheetSubscriptionDue,
                                                    SubscriptionReceived = worksheetSubscriptionReceived,
                                                    MemberNumber = worksheetMemberNumber,
                                                    supplierName = mySupplier.Name,
                                                    ImportFileId = importFileId,
                                                    SupplierId = selectedSupplierID,
                                                    CommissionInclVAT = worksheetCommissionInclVAT,
                                                    CommissionExclVAT = worksheetCommissionExclVAT,
                                                };

                                                bool duplicateInsertCheck = unmatchedCommissionList
                                                    .Where(e => e.MemberSearchValue == unmatchedDocuments.MemberSearchValue)
                                                    .Any();

                                                if (!duplicateInsertCheck)
                                                {
                                                    unmatchedCommissionList.Add(unmatchedDocuments);
                                                }
                                            }
                                            else
                                            {
                                                commissionUnmatched.CommissionExclVAT += worksheetCommissionExclVAT;
                                                commissionUnmatched.CommissionInclVAT += worksheetCommissionInclVAT;
                                            }
                                        }

                                        // Supplier Licence
                                        var supplierLicence = suppliersShares
                                            .Where(e => e.AdviserId == app.Advisor_Id && e.SupplierId == selectedSupplierID)
                                            .Any();

                                        if (!supplierLicence)
                                        {
                                            if (!Equals(matchedCommissionCheck, null))
                                            {
                                                commissionUnmatched = matchedCommissionCheck;
                                            }

                                            if (Equals(commissionUnmatched, null))
                                            {
                                                UnmatchedCommissions unmatchedSupplier = new UnmatchedCommissions
                                                {
                                                    Reasons = "No Supplier Licence For This Application",
                                                    AdvisorName = advisorName,
                                                    MemberSearchValue = commissionStatement.MemberSearchValue,
                                                    MemberSearchKey = commissionStatement.MemberSearchKey,
                                                    CommisionRunDate = runDate,
                                                    CommisionRunUser = runUser,
                                                    TransactionDate = transactionDate,
                                                    MemberNumber = worksheetMemberNumber,
                                                    supplierName = mySupplier.Name,
                                                    ImportFileId = importFileId,
                                                    SupplierId = selectedSupplierID,
                                                    Surname = worksheetSurname,
                                                    Initial = worksheetInitial,
                                                    EnrollmentDate = worksheetEnrollmentDate,
                                                    TerminationDate = worksheetTerminationDate,
                                                    SubscriptionDue = worksheetSubscriptionDue,
                                                    SubscriptionReceived = worksheetSubscriptionReceived,
                                                    CommissionInclVAT = worksheetCommissionInclVAT,
                                                    CommissionExclVAT = worksheetCommissionExclVAT,
                                                };

                                                bool duplicateInsertCheck = unmatchedCommissionList
                                                    .Where(e => e.MemberNumber == unmatchedSupplier.MemberNumber)
                                                    .Any(); 

                                                if (!duplicateInsertCheck)
                                                {
                                                    unmatchedCommissionList.Add(unmatchedSupplier);
                                                }
                                            }
                                            else
                                            {
                                                commissionUnmatched.CommissionExclVAT += worksheetCommissionExclVAT;
                                                commissionUnmatched.CommissionInclVAT += worksheetCommissionInclVAT;
                                            }
                                        }

                                        // If appliation status is not 6 (status == 'Complete')
                                        if (app.ApplicationStatus_Id != 6)
                                        {
                                            if (!Equals(matchedCommissionCheck, null))
                                            {
                                                commissionUnmatched = matchedCommissionCheck;
                                            }

                                            if (Equals(commissionUnmatched, null))
                                            {
                                                UnmatchedCommissions unmatchedApplication = new UnmatchedCommissions
                                                {
                                                    Reasons = "Application status not complete",
                                                    AdvisorName = advisorName,
                                                    MemberSearchValue = commissionStatement.MemberSearchValue,
                                                    MemberSearchKey = commissionStatement.MemberSearchKey,
                                                    CommisionRunDate = runDate,
                                                    CommisionRunUser = runUser,
                                                    TransactionDate = transactionDate,
                                                    Surname = worksheetSurname,
                                                    Initial = worksheetInitial,
                                                    EnrollmentDate = worksheetEnrollmentDate,
                                                    TerminationDate = worksheetTerminationDate,
                                                    SubscriptionDue = worksheetSubscriptionDue,
                                                    SubscriptionReceived = worksheetSubscriptionReceived,
                                                    MemberNumber = worksheetMemberNumber,
                                                    supplierName = mySupplier.Name,
                                                    ImportFileId = importFileId,
                                                    SupplierId = selectedSupplierID,
                                                    CommissionInclVAT = worksheetCommissionInclVAT,
                                                    CommissionExclVAT = worksheetCommissionExclVAT,
                                                };

                                                bool duplicateInsertCheck = unmatchedCommissionList
                                                    .Where(e => e.MemberSearchValue == unmatchedApplication.MemberSearchValue)
                                                    .Any();

                                                if (!duplicateInsertCheck)
                                                {
                                                    unmatchedCommissionList.Add(unmatchedApplication);
                                                }
                                            }
                                            else
                                            {
                                                commissionUnmatched.CommissionExclVAT += worksheetCommissionExclVAT;
                                                commissionUnmatched.CommissionInclVAT += worksheetCommissionInclVAT;
                                            }
                                        }

                                        var CommissionDateValid = db.AdvisorShareUnderSupervisions
                                            .Where(e =>
                                                e.AdvisorId == app.Advisor_Id &&
                                                e.validCommissionFromDate <= DateTime.Now &&
                                                (e.validCommissionToDate == null ? false : e.validCommissionToDate >= DateTime.Now))
                                            .Any();

                                        if (CommissionDateValid)
                                        {
                                            if (!Equals(matchedCommissionCheck, null))
                                            {
                                                commissionUnmatched = matchedCommissionCheck;
                                            }

                                            if (Equals(commissionUnmatched, null))
                                            {
                                                UnmatchedCommissions unmatchedSplit = new UnmatchedCommissions
                                                {
                                                    Reasons = "Commission split date not valid",
                                                    AdvisorName = advisorName,
                                                    MemberSearchValue = commissionStatement.MemberSearchValue,
                                                    MemberSearchKey = commissionStatement.MemberSearchKey,
                                                    CommisionRunDate = runDate,
                                                    CommisionRunUser = runUser,
                                                    TransactionDate = transactionDate,
                                                    MemberNumber = worksheetMemberNumber,
                                                    supplierName = mySupplier.Name,
                                                    ImportFileId = importFileId,
                                                    SupplierId = selectedSupplierID,
                                                    Surname = worksheetSurname,
                                                    Initial = worksheetInitial,
                                                    EnrollmentDate = worksheetEnrollmentDate,
                                                    TerminationDate = worksheetTerminationDate,
                                                    SubscriptionDue = worksheetSubscriptionDue,
                                                    SubscriptionReceived = worksheetSubscriptionReceived,
                                                    CommissionInclVAT = worksheetCommissionInclVAT,
                                                    CommissionExclVAT = worksheetCommissionExclVAT,
                                                };

                                                bool duplicateInsertCheck = unmatchedCommissionList
                                                    .Where(e => e.MemberSearchValue == unmatchedSplit.MemberSearchValue)
                                                    .Any();

                                                if (!duplicateInsertCheck)
                                                {
                                                    unmatchedCommissionList.Add(unmatchedSplit);
                                                }
                                            }
                                            else
                                            {
                                                commissionUnmatched.CommissionExclVAT += worksheetCommissionExclVAT;
                                                commissionUnmatched.CommissionInclVAT += worksheetCommissionInclVAT;
                                            }
                                        }

                                        if (app != null)
                                        {
                                            commissionStatement.ClientId = app.Client.Id;
                                        }
                                    }
                                }
                                else if (commissionStatement.MemberSearchValue != "")
                                {
                                    var searchAndMonthValueMatch = await db.UnmatchedCommissions
                                        .FirstOrDefaultAsync(e =>
                                            e.MemberSearchValue == commissionStatement.MemberSearchValue &&
                                            e.TransactionDate.Value.Month == commissionDateFromSVC.Month);

                                    if (Equals(searchAndMonthValueMatch, null))
                                    {
                                        UnmatchedCommissions unmatchedClient = new UnmatchedCommissions
                                        {
                                            AdvisorName = advisorName,
                                            Reasons = "Client does not exist",
                                            MemberSearchValue = commissionStatement.MemberSearchValue,
                                            MemberSearchKey = commissionStatement.MemberSearchKey,
                                            CommisionRunDate = runDate,
                                            CommisionRunUser = runUser,
                                            TransactionDate = transactionDate,
                                            Surname = worksheetSurname,
                                            Initial = worksheetInitial,
                                            EnrollmentDate = worksheetEnrollmentDate,
                                            TerminationDate = worksheetTerminationDate,
                                            SubscriptionDue = worksheetSubscriptionDue,
                                            SubscriptionReceived = worksheetSubscriptionReceived,
                                            MemberNumber = worksheetMemberNumber,
                                            supplierName = mySupplier.Name,
                                            ImportFileId = importFileId,
                                            SupplierId = selectedSupplierID,
                                            CommissionInclVAT = worksheetCommissionInclVAT,
                                            CommissionExclVAT = worksheetCommissionExclVAT,
                                        };

                                        bool memberDuplicateCheck = unmatchedCommissionList
                                            .Where(e => e.MemberNumber.Contains(unmatchedClient.MemberNumber))
                                            .Any();

                                        if (!memberDuplicateCheck)
                                        {
                                            unmatchedCommissionList.Add(unmatchedClient);
                                        }
                                    }
                                    else
                                    {
                                        commissionUnmatched.CommissionExclVAT += worksheetCommissionExclVAT;
                                        commissionUnmatched.CommissionInclVAT += worksheetCommissionInclVAT;
                                    }
                                }

                                // Confirm link
                                Product linkedProduct = null;
                                if (app != null)
                                {
                                    linkedProduct = app.Product;
                                    commissionStatement.ProductId = linkedProduct == null
                                        ? 0
                                        : linkedProduct.Id;
                                }

                                string transactionDateColumnValue = worksheet.Cells[row, mySupplier.TransactionDateColumn].Text.Normalize();

                                DateTime.TryParse(transactionDateColumnValue, out transactionDate);
                                if (transactionDate == (DateTime)SQLMinDate)
                                {
                                    transactionDate = commissionDateFromSVC;
                                }

                                int statementLineCount = 0;
                                if (linkedProduct != null)
                                {
                                    statementLineCount = db.CommissionStatement
                                        .Where(cs =>
                                            cs.AdvisorId == advisorId &&
                                            cs.ProductId == linkedProduct.Id &&
                                            cs.TransactionDate == transactionDate)
                                        .Count();
                                }


                                if (statementLineCount > 0)
                                {
                                    // If the transaction has been listed skip;
                                    continue;
                                }

                                if (app != null)
                                {
                                    if (app.Advisor_Id.HasValue)
                                    {
                                        advisorId = app.Advisor_Id.Value;
                                    }

                                    if (app.Product_Id.HasValue)
                                    {
                                        commissionStatement.ProductId = app.Product_Id.Value;
                                    }
                                    else
                                    {
                                        // TODO: Set so default advisor 
                                        contact = await db.Contacts.FirstOrDefaultAsync(x => x.IdNumber == "6411205084083");
                                        advisorId = (await db.Advisors.FirstOrDefaultAsync(x => x.ContactId == contact.Id)).Id;
                                    }

                                    commissionStatement.CommisionRunDate = runDate;
                                    commissionStatement.CommisionRunUser = runUser;
                                    commissionStatement.TransactionDate = transactionDate;
                                    commissionStatement.AdvisorId = advisorId;
                                    commissionStatement.MemberNumber = worksheetMemberNumber;
                                    commissionStatement.ImportFileId = importFileId;
                                    commissionStatement.ApprovalStatus = "REFRESHED";
                                    commissionStatement.SupplierId = selectedSupplierID;
                                    commissionStatement.Surname = worksheetSurname;
                                    commissionStatement.Initial = worksheetInitial;
                                    commissionStatement.EnrollmentDate = worksheetEnrollmentDate;
                                    commissionStatement.TerminationDate = worksheetTerminationDate;
                                    commissionStatement.SubscriptionDue = worksheetSubscriptionDue;
                                    commissionStatement.SubscriptionReceived = worksheetSubscriptionReceived;
                                    commissionStatement.CommissionInclVAT = worksheetCommissionInclVAT;
                                    commissionStatement.CommissionExclVAT = worksheetCommissionExclVAT;
                                    commissionStatement.AdvisorTaxRate = advisorTaxRate;

                                    if ((advisorId > 0) && (linkedProduct != null))
                                    {
                                        // Compute commision as per product liscense
                                        var A2LT = await db.AdvisorShareUnderSupervisions
                                            .Where(a => a.LicenseTypeId == linkedProduct.LicenseTypeId && a.AdvisorId == advisorId)
                                            .FirstOrDefaultAsync();

                                        if (A2LT != null)
                                        {
                                            decimal advisorshare = (decimal)(A2LT.Share) / 100m;
                                            decimal companyshare = (Math.Abs((decimal)(A2LT.Share) - 100m)) / 100m;
                                            commissionStatement.AdvisorCommission = commissionStatement.CommissionExclVAT * advisorshare;
                                            commissionStatement.CompanyCommission = commissionStatement.CommissionExclVAT * companyshare;
                                            commissionStatement.AdvisorTaxRate = advisor.TaxDirectiveRate;

                                            if (commissionStatement.AdvisorTaxRate == 0)
                                            {
                                                SystemSetting ss = db.SystemSettings.Where(s => s.SettingName == "PersonalTaxIncomeRate").FirstOrDefault();
                                                decimal TaxRate = 0m;
                                                commissionStatement.AdvisorTaxRate = ss.SettingValue == ""
                                                    ? 0m
                                                    : decimal.TryParse(ss.SettingValue, out TaxRate)
                                                        ? (decimal?)TaxRate
                                                        : 25m;
                                            }

                                            commissionStatement.AdvisorTax = commissionStatement.AdvisorCommission * (commissionStatement.AdvisorTaxRate / 100m);
                                        }
                                    }

                                    if (commissionStatement.ClientId == null)
                                    {
                                        commissionStatement.ClientId = 0;
                                    }

                                    var adviStatus2 = db.Advisors.Where(e => e.Id == app.Advisor_Id.Value).FirstOrDefault().AdvisorStatus_Id;
                                    var applinked2 = db.ApplicationAdvisorHistory
                                        .Where(e => e.Application_Id == app.Id && e.New_Advisor == app.Advisor_Id)
                                        .Any();

                                    var expired2 = db.AdvisorDocuments
                                        .Where(e => e.DocumentTypeId == 10 && e.ValidFromDate >= DateTime.Now && e.ValidToDate <= DateTime.Now)
                                        .Any();

                                    var CommissionDateValid2 = !db.AdvisorShareUnderSupervisions
                                        .Where(e =>
                                            e.AdvisorId == app.Advisor_Id &&
                                            e.validCommissionFromDate <= DateTime.Now && e.validCommissionToDate >= DateTime.Now)
                                        .Any();

                                    var SupplierLicence2 = suppliersShares
                                        .Where(e => e.AdviserId == app.Advisor_Id && e.SupplierId == selectedSupplierID)
                                        .Any();

                                    // Test adviser's  effective start date and end date 
                                    var validEffectiveDate = true;

                                    if (((app.Advisor.EffectiveEndDate != null) && (app.Advisor.EffectiveEndDate < commissionDateFromSVC)) || (app.Advisor.EffectiveStartDate > commissionDateFromSVC))
                                    {
                                        validEffectiveDate = false;
                                    }

                                    // Test application's effective date 
                                    // Get effective date for the application 
                                    var applicationsEffectiveDate = db.ApplicationAdvisorHistory
                                        .Where(e => e.Application_Id == app.Id && e.New_Advisor == app.Advisor.Id).ToArray().Last();

                                    //Thabang: Add to commission statement if all requirements are met, also test against Effective start date and effective end date 
                                    if (adviStatus2 == 5
                                        && applinked2
                                        && !expired2
                                        && app.ApplicationStatus_Id == 6
                                        && CommissionDateValid2
                                        && SupplierLicence2
                                        && validEffectiveDate
                                        && applicationsEffectiveDate.DateStarted <= endDate)
                                    {

                                        // Group same application entries 
                                        // Check if the application is already in the commission stament 

                                        if (rows.Where(e =>
                                            e.MemberSearchValue == commissionStatement.MemberSearchValue &&
                                            e.SupplierId == commissionStatement.SupplierId &&
                                            e.TransactionDate.Value.Year == commissionStatement.TransactionDate.Value.Year).Count() > 0)
                                        {
                                            // Get the application and alter the amounts 
                                            var comm = rows.Where(e => e.MemberSearchValue == commissionStatement.MemberSearchValue).FirstOrDefault();

                                            // Sum all the applications 
                                            comm.CommissionExclVAT += commissionStatement.CommissionExclVAT;
                                            comm.CommissionInclVAT += commissionStatement.CommissionInclVAT;
                                        }
                                        else
                                        {
                                            rows.Add(commissionStatement);
                                        }
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            catch(Exception)
                            {
                                skippedRows.Add("");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    if (rows.Count > 0)
                    {
                        return BadRequest(e.Message);
                    }
                }
            }
            
            //for CSV using product mappings
            if (rows.Count == 0)
            {
                try
                {
                    using (StreamReader sr = File.OpenText(fileEntity.Location))
                    {
                        string str = String.Empty;

                        DateTime runDate = DateTime.Now;
                        // Todo: get logged in user name;
                        string runUser = "CurrentUser";
                        int line = 0;

                        while ((str = sr.ReadLine()) != null)
                        {
                            if (line < 1)
                            {
                                line++;
                                str = sr.ReadLine();
                            }

                            str = str.Replace(@"\", "").Replace("\"", "");
                            string[] item = str.Split(',');
                            
                            DateTime enrollmentDate = (DateTime)SQLMinDate;
                            DateTime terminationDate = (DateTime)SQLMinDate;

                            decimal subscriptionDue = 0.00m;
                            decimal subscriptionReceived = 0.00m;
                            decimal commissionInclVat = 0.00m;
                            decimal commissionExclVat = 0.00m;

                            CommissionStatement commissionStatement = new CommissionStatement();
                            try
                            {
                                Application app = null;
                                Advisor advisor = await db.Advisors.FirstOrDefaultAsync(e => e.Id == advisorId); ;
                                Contact contact = null;
                                string advisorName = "";

                                string itemIdNumber = mySupplier.IDNumberColumn == 0
                                    ? ""
                                    : item[mySupplier.IDNumberColumn].Normalize().Trim();

                                string itemMemberNumber = mySupplier.MemberNumberColumn == 0
                                    ? ""
                                    : item[mySupplier.MemberNumberColumn].Normalize().Trim();

                                string itemPolicyNumber = mySupplier.PolicyNumberColumn == 0
                                    ? ""
                                    : item[mySupplier.MemberNumberColumn].Normalize();

                                string itemPreCompInclun = mySupplier.CommissionInclVatColumn == 0
                                    ? "0"
                                    : item[mySupplier.CommissionInclVatColumn].Normalize();

                                string itemPreCompExclun = mySupplier.CommissionExclVatColumn == 0
                                    ? "0"
                                    : item[mySupplier.CommissionExclVatColumn].Normalize();

                                string itemSurname = mySupplier.SurnameColumn == 0
                                    ? ""
                                    : item[mySupplier.SurnameColumn].Normalize();

                                string itemInitial = mySupplier.InitialColumn == 0
                                    ? ""
                                    : item[mySupplier.InitialColumn].Normalize();

                                string transactionDateValFromCSV = item[mySupplier.TransactionDateColumn].Normalize();

                                DateTime? itemEnrollmentDate = mySupplier.EnrollmentDateColumn == 0
                                    ? (DateTime)SQLMinDate
                                    : DateTime.TryParse(item[mySupplier.EnrollmentDateColumn], out enrollmentDate)
                                        ? (DateTime?)enrollmentDate
                                        : null;

                                DateTime? itemTerminationDate = mySupplier.TerminationDateColumn == 0
                                    ? (DateTime)SQLMinDate
                                    : DateTime.TryParse(item[mySupplier.TerminationDateColumn], out terminationDate)
                                        ? (DateTime?)terminationDate
                                        : null;

                                decimal? itemSubscriptionDue = mySupplier.SubscriptionDueColumn == 0
                                    ? 0m
                                    : decimal.TryParse(item[mySupplier.SubscriptionDueColumn], out subscriptionDue)
                                        ? (decimal?)subscriptionDue
                                        : null;

                                decimal? itemSubscriptionReceived = mySupplier.SubscriptionReceivedColumn == 0
                                    ? 0m
                                    : decimal.TryParse(item[mySupplier.SubscriptionReceivedColumn], style, culture, out subscriptionReceived)
                                        ? (decimal?)subscriptionReceived
                                        : null;

                                decimal? itemCommissionInclVAT = mySupplier.CommissionInclVatColumn == 0
                                    ? 0m
                                    : (decimal.TryParse(itemPreCompInclun, style, culture, out commissionInclVat)
                                        ? (decimal?)commissionInclVat
                                        : null);

                                decimal? itemCommissionExclVAT = mySupplier.CommissionExclVatColumn == 0
                                    ? 0m
                                    : decimal.TryParse(itemPreCompExclun, style, culture, out commissionExclVat)
                                        ? (decimal?)commissionExclVat
                                        : null;

                                decimal advisorTaxRate = calVat(
                                    Convert.ToDecimal(commissionStatement.CommissionExclVAT),
                                    Convert.ToDecimal(commissionStatement.CommissionInclVAT));

                                if (advisor != null)
                                {
                                    
                                    contact = await db.Contacts.FirstOrDefaultAsync(e => e.Id == advisor.ContactId);
                                    advisorName = contact.FirstName + " " + contact.LastName;
                                }

                                // Fall through search types
                                if (itemIdNumber != "")
                                {
                                    // Strip search non-numerics
                                    commissionStatement.MemberSearchValue = new string(itemIdNumber.Where(c => char.IsDigit(c)).ToArray());
                                    commissionStatement.MemberSearchKey = "IDNumber";
                                }

                                if ((itemPolicyNumber) != "" && commissionStatement.MemberSearchKey == "")
                                {
                                    commissionStatement.MemberSearchValue = itemPolicyNumber;
                                    commissionStatement.MemberSearchKey = "PolicyNumber";
                                }

                                if ((itemMemberNumber) != ""
                                    && ((commissionStatement.MemberSearchKey == null)
                                    || (commissionStatement.MemberSearchKey == "")))
                                {
                                    commissionStatement.MemberSearchValue = itemMemberNumber;
                                    commissionStatement.MemberSearchKey = "MemberNumber";
                                }

                                if (commissionStatement.ClientId == null)
                                {
                                    // Can not be linked to client
                                    commissionStatement.ClientId = 0;
                                }

                                var matchedCommissionsCheck = await db.UnmatchedCommissions
                                    .FirstOrDefaultAsync(e =>
                                        e.MemberSearchValue == commissionStatement.MemberSearchValue &&
                                        e.TransactionDate.Value.Month == commissionDateFromSVC.Month);

                                //The next applies to value like '98,21': NumberStyles parse computes to '9821'
                                
                                if (itemPreCompInclun.Contains(',') && itemPreCompInclun.Contains('.') == false)
                                {
                                    itemPreCompInclun = itemPreCompInclun.Replace(',', '.');
                                }
                                
                                if (itemPreCompExclun.Contains(',') && itemPreCompExclun.Contains('.') == false)
                                {
                                    itemPreCompExclun = itemPreCompExclun.Replace(',', '.');
                                }

                                if (commissionStatement.MemberSearchKey == "IDNumber")
                                {
                                    Contact searchContact = await db.Contacts
                                        .Where(x => x.IdNumber == commissionStatement.MemberSearchValue)
                                        .FirstOrDefaultAsync();

                                    if (contact != null)
                                    {
                                        app = await db.Applications
                                            .Where(x => x.Client_Id == contact.Id && Products.Contains(x.Product_Id.Value))
                                            .FirstOrDefaultAsync();

                                        if (app != null)
                                        {
                                            commissionStatement.ClientId = app.Client.Id;
                                        }
                                    }
                                    else if (commissionStatement.MemberSearchValue != "")
                                    {
                                        if (Equals(matchedCommissionsCheck, null))
                                        {
                                            UnmatchedCommissions unmatchedApplication = new UnmatchedCommissions
                                            {
                                                Reasons = "No application assigned to client",
                                                AdvisorName = advisorName,
                                                MemberSearchValue = commissionStatement.MemberSearchValue,
                                                MemberSearchKey = commissionStatement.MemberSearchKey,
                                                CommisionRunDate = runDate,
                                                CommisionRunUser = runUser,
                                                TransactionDate = transactionDate,
                                                MemberNumber = itemMemberNumber,
                                                supplierName = mySupplier.Name,
                                                ImportFileId = importFileId,
                                                SupplierId = selectedSupplierID,
                                                Surname = itemSurname,
                                                Initial = itemInitial,
                                                EnrollmentDate = itemEnrollmentDate,
                                                TerminationDate = itemTerminationDate,
                                                SubscriptionDue = itemSubscriptionDue,
                                                SubscriptionReceived = itemSubscriptionReceived,
                                                CommissionInclVAT = itemCommissionInclVAT,
                                                CommissionExclVAT = itemCommissionExclVAT,
                                                AdvisorTaxRate = advisorTaxRate,
                                            };

                                            bool duplicateInsertCheck = unmatchedCommissionList
                                                    .Where(e => e.MemberSearchValue == unmatchedApplication.MemberSearchValue)
                                                    .Any();

                                            if (!duplicateInsertCheck)
                                            {
                                                unmatchedCommissionList.Add(unmatchedApplication);
                                            }
                                        }
                                        else
                                        {
                                            UnmatchedCommissions memberSearchValue = await db.UnmatchedCommissions
                                                .FirstOrDefaultAsync(e => e.MemberSearchValue == commissionStatement.MemberSearchValue);

                                            bool memberSearchValueContains = memberSearchValue.Reasons.Contains("No application assigned to client");

                                            if (!memberSearchValueContains)
                                            {
                                                memberSearchValue.Reasons = memberSearchValue.Reasons + ", No application assigned to client ";
                                            }
                                        }
                                    }
                                }

                                if (commissionStatement.MemberSearchKey == "MemberNumber")
                                {
                                    UnmatchedCommissions memberSearchValue = await db.UnmatchedCommissions
                                                .FirstOrDefaultAsync(e => e.MemberSearchValue == commissionStatement.MemberSearchValue);

                                    if (Equals(memberSearchValue, null))
                                    {
                                        UnmatchedCommissions unmatchedMember = new UnmatchedCommissions
                                        {
                                            Reasons = "Member number not on statement",
                                            AdvisorName = advisorName,
                                            MemberSearchValue = commissionStatement.MemberSearchValue,
                                            MemberSearchKey = commissionStatement.MemberSearchKey,
                                            CommisionRunDate = runDate,
                                            CommisionRunUser = runUser,
                                            TransactionDate = transactionDate,
                                            MemberNumber = itemMemberNumber,
                                            supplierName = mySupplier.Name,
                                            ImportFileId = importFileId,
                                            SupplierId = selectedSupplierID,
                                            Surname = itemSurname,
                                            Initial = itemInitial,
                                            EnrollmentDate = itemEnrollmentDate,
                                            TerminationDate = itemTerminationDate,
                                            SubscriptionDue = itemSubscriptionDue,
                                            SubscriptionReceived = itemSubscriptionReceived,
                                            CommissionInclVAT = itemCommissionInclVAT,
                                            CommissionExclVAT = itemCommissionExclVAT,
                                            AdvisorTaxRate = advisorTaxRate,
                                        };

                                        bool duplicateInsertCheck = unmatchedCommissionList
                                            .Where(e => e.MemberSearchValue == unmatchedMember.MemberSearchValue)
                                            .Any();

                                        if (!duplicateInsertCheck)
                                        {
                                            unmatchedCommissionList.Add(unmatchedMember);
                                        }
                                    }
                                }

                                // Confirm link
                                Product linkedProduct = null;

                                if (app != null)
                                {
                                    linkedProduct = app.Product;
                                    commissionStatement.ProductId = linkedProduct == null 
                                        ? 0
                                        : linkedProduct.Id;
                                }

                                // Map col is 1 based. item is 0 based
                                string transactionDateString = (mySupplier.TransactionDateColumn) == 0
                                    ? SQLMinDate.ToString()
                                    : item[mySupplier.TransactionDateColumn];

                                if (transactionDateString == "")
                                {
                                    skippedRows.Add(str);
                                    continue;
                                }

                                DateTime.TryParse(transactionDateString, out transactionDate);
                                if (transactionDate != (DateTime)SQLMinDate)
                                {
                                    DateTime transactionDateMagic = (DateTime)SQLMinDate;
                                    if (transactionDate == (DateTime)SQLMinDate)
                                    {
                                        transactionDateString = transactionDateString.Replace('/', ' ').Replace('-', ' ');
                                        int transactionDateYear = int.Parse(transactionDateString.Substring(0, 4));
                                        int transactionDateMonth = int.Parse(transactionDateString.Substring(4, 2));
                                        transactionDateMagic = new DateTime(transactionDateYear, transactionDateMonth, 1);
                                    }
                                    if (transactionDateMagic == (DateTime)SQLMinDate)
                                    {
                                        // If you cant find the transaction date the row is useless;
                                        skippedRows.Add(str);
                                        continue;
                                    }
                                }

                                int statementLineCount = 0;
                                if (linkedProduct != null)
                                {
                                    statementLineCount = db.CommissionStatement
                                        .Where(cs => 
                                            cs.AdvisorId == advisorId &&
                                            cs.ProductId == linkedProduct.Id &&
                                            cs.TransactionDate == transactionDate)
                                        .Count();
                                }


                                if (statementLineCount > 0)
                                {
                                    // If the transaction has been listed skip;
                                    continue;
                                }


                                if (app != null)
                                {
                                    if (app.Product_Id.HasValue)
                                    {
                                        commissionStatement.ProductId = app.Product_Id.Value;
                                    }
                                    else
                                    {
                                        // TODO: Set so default advisor
                                        contact = await db.Contacts.FirstOrDefaultAsync(x => x.IdNumber == "6411205084083");
                                        advisorId = (await db.Advisors.FirstOrDefaultAsync(x => x.ContactId == contact.Id)).Id;
                                    }
                                }
                                else
                                {
                                    continue;
                                }

                                commissionStatement.CommisionRunDate = runDate;
                                commissionStatement.CommisionRunUser = runUser;
                                commissionStatement.Surname = itemSurname;
                                commissionStatement.Initial = itemInitial;
                                commissionStatement.EnrollmentDate = itemEnrollmentDate;
                                commissionStatement.TerminationDate = itemTerminationDate;
                                commissionStatement.SubscriptionDue = itemSubscriptionDue;
                                commissionStatement.SubscriptionReceived = itemSubscriptionReceived;
                                commissionStatement.CommissionInclVAT = itemCommissionInclVAT;
                                commissionStatement.CommissionExclVAT = itemCommissionExclVAT;
                                commissionStatement.AdvisorId = advisorId;
                                commissionStatement.MemberNumber = itemMemberNumber;
                                commissionStatement.ImportFileId = importFileId;
                                commissionStatement.ApprovalStatus = "REFRESHED";
                                commissionStatement.SupplierId = selectedSupplierID;

                                if (advisorId > 0 && linkedProduct != null)
                                {
                                    //compute commision as per product liscense
                                    Advisor thisAdvisor = db.Advisors.FirstOrDefault(a => a.Id == advisorId);
                                    var A2LT = await db.AdvisorShareUnderSupervisions
                                        .Where(a => a.LicenseTypeId == linkedProduct.LicenseTypeId && a.AdvisorId == advisorId)
                                        .FirstOrDefaultAsync();

                                    if (A2LT != null)
                                    {
                                        decimal advisorshare = (decimal)(A2LT.Share) / 100m;
                                        decimal companyshare = (Math.Abs((decimal)(A2LT.Share) - 100m)) / 100m;
                                        commissionStatement.AdvisorCommission = commissionStatement.CommissionExclVAT * advisorshare;
                                        commissionStatement.CompanyCommission = commissionStatement.CommissionExclVAT * companyshare;
                                        commissionStatement.AdvisorTaxRate = thisAdvisor.TaxDirectiveRate;

                                        if (commissionStatement.AdvisorTaxRate == 0)
                                        {
                                            SystemSetting ss = await db.SystemSettings.Where(s => s.SettingName == "PersonalTaxIncomeRate").FirstOrDefaultAsync();
                                            decimal TaxRate = 0m;
                                            commissionStatement.AdvisorTaxRate = ss.SettingValue == "" ? 25m : decimal.TryParse(ss.SettingValue, out TaxRate) ? (decimal?)TaxRate : 25m;
                                        }

                                        commissionStatement.AdvisorTax = commissionStatement.AdvisorCommission * (commissionStatement.AdvisorTaxRate / 100m);
                                    }
                                }

                                if (commissionStatement.ClientId == null)
                                {
                                    commissionStatement.ClientId = 0;
                                }

                                var adviStatus3 = db.Advisors
                                    .Where(e => e.Id == app.Advisor_Id.Value)
                                    .FirstOrDefault()
                                    .AdvisorStatus_Id;

                                var applinked3 = db.ApplicationAdvisorHistory
                                    .Where(e => e.Application_Id == app.Id && e.New_Advisor == app.Advisor_Id)
                                    .Any();

                                var expired3 = db.AdvisorDocuments
                                    .Where(e => e.DocumentTypeId == 10 && e.ValidFromDate >= DateTime.Now && e.ValidToDate <= DateTime.Now)
                                    .Any();

                                var CommissionDateValid3 = db.AdvisorShareUnderSupervisions
                                    .Where(e => 
                                        e.AdvisorId == app.Advisor_Id &&
                                        e.validCommissionFromDate <= DateTime.Now &&
                                        e.validCommissionToDate >= DateTime.Now)
                                    .Any();

                                var SupplierLicence3 = suppliersShares
                                    .Where(e => e.AdviserId == app.Advisor_Id && e.SupplierId == selectedSupplierID).Any();

                                if (adviStatus3 == 5
                                    && !applinked3
                                    && !expired3
                                    && app.ApplicationStatus_Id == 6
                                    || CommissionDateValid3
                                    && SupplierLicence3)
                                {
                                    rows.Add(commissionStatement);
                                }
                                else
                                {
                                    var anyMemberMatch = db.UnmatchedCommissions
                                        .Where(e => e.MemberSearchValue.Equals(commissionStatement.MemberSearchValue))
                                        .Any();
                                    if (!anyMemberMatch)
                                    {
                                        Console.WriteLine(" " + commissionStatement.MemberSearchValue);
                                    }
                                }
                            }
                            catch (Exception exception)
                            {
                                skippedRows.Add(str);
                                System.Diagnostics.Debug.WriteLine("Exception in PutCommission: ", exception);
                            }
                            line++;
                        }
                    }
                }
                catch (Exception exception)
                {
                    return BadRequest(exception.ToString()); ;
                }
            }

            try
            {
                if (rows.Count > 0)
                {
                    db.CommissionStatement.AddRange(rows);
                    fileEntity.DateImported = DateTime.Now;
                }

                if (unmatchedCommissionList.Count > 0)
                {
                    db.UnmatchedCommissions.AddRange(unmatchedCommissionList.Distinct());
                }

                var commissionfilestatus = await db.CommissionFileStatus.FirstOrDefaultAsync(e => e.ImportFileId == importFileId);
                db.CommissionFileStatus.Attach(commissionfilestatus);
                commissionfilestatus.Status = "Processed";
                await db.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }

            return Ok(fileEntity);
        }

        // PUT: api/Admin/Process/Members/5
        // This method finds file with importFileId and loads the file, 
        // assumes it's a list of members and applications parses the 
        // data and imports it into the database.
        [Route("Admin/Process/ApplicationsMembers/{importFileId}")]
        [ResponseType(typeof(ImportFile))]
        public async Task<IHttpActionResult> PutProcessMembers(int importFileId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ImportFile importFile = db.ImportFiles.Find(importFileId);

            if (importFile == null)
            {
                return NotFound();
            }
            
            if (importFile.FieldMap == null)
            {
                return new System.Web.Http.Results.BadRequestErrorMessageResult("No field mapping found in uploaded file.", this);
            }

            FileInfo file = new FileInfo(importFile.Location);
            List<ApplicationWithMemberDTO> importList = new List<ApplicationWithMemberDTO>();
            
            if (file.Exists && file.Extension.StartsWith(".xls"))
            {
                ExcelPackage excelFile = new ExcelPackage(file);

                var mappingData = importFile.FieldMap.MappingData;
                var header = importFile.FieldMap.FileFields;
                var dbheader = importFile.FieldMap.DbFields;

                var worksheet = excelFile.Workbook.Worksheets[1];
                for (int i = importFile.FieldMap.SkipLines + 1; i <= worksheet.Dimension.Rows; i++)
                {
                    var rowData = worksheet.Cells[i, 1, i, worksheet.Dimension.Columns - 1];
                    var advisorCode = rowData.ElementAt(33).Value;
                    if (advisorCode != null)
                    {
                        var codes = db.AdvisorSupplierCodes.Where(c => c.AdvisorCode == (string)advisorCode).FirstOrDefault();
                        if (codes != null)
                        {
                            int advisorId = codes.AdvisorId;
                        }
                    }
                }

                Advisor advisor = new Advisor()
                {
                    Id = 1,
                };

            }

            importFile.DateImported = DateTime.Now;
            importFile.ImportSuccess = true;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }

            return Ok(importFile);
        }

        // POST: api/Admin/
        // Saves an uploaded file to disk. (takes importFile containing base64 file content and decodes...)
        [Route("Admin/ImportFile")]
        [ResponseType(typeof(ImportFile))]
        public async Task<IHttpActionResult> PostFileImport(ImportFile importFile)
        {
            if (string.IsNullOrEmpty(importFile.Data))
            {
                return BadRequest("No file data received.");
            }

            if (db.ImportFiles.Where(e => e.FileName == importFile.FileName).Any())
            {
                return BadRequest("File Already Imported");
            }

            try
            {
                var stripHeader = importFile.Data.Split(',');

                if (stripHeader.Length < 2)
                {
                    return BadRequest("The file data is not properly formatted");
                }

                importFile.BinaryData = Convert.FromBase64String(stripHeader[1]);

                string userId = "unknown";
                var _user = await GetUser();
                if (_user != null)
                {
                    userId = _user.Id;
                }

                //TODO: Add Aplication & Product Folders
                string fileName = (
                    workingFolder + "\\"
                    + userId + "\\"
                    + (DateTime.Now.Ticks.ToString()).Replace(@"/", "-")
                    + importFile.FileName
                    );

                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }

                if (File.Exists(fileName))
                {
                    Console.WriteLine(fileName + " already exists!");
                    return BadRequest("File already exists.");
                }

                FileStream fs = new FileStream(fileName.Replace(@"\\", @"\"), FileMode.CreateNew);
                await fs.WriteAsync(importFile.BinaryData, 0, importFile.BinaryData.Length);
                fs.Close();

                importFile.Location = fileName;
                importFile.Data = "";
                importFile.Size = importFile.BinaryData.Length;
                importFile.BinaryData = null;
                importFile.UserUid = userId;
                importFile.CreatedDate = DateTime.Now;
                importFile.DateImported = DateTime.Now;
                importFile.FieldMap = new FieldMap();
                
                var ext = Path.GetExtension(fileName);
                if (ext != ".csv")
                {
                    //Prepare and set the fieldmap...
                    FileInfo file = new FileInfo(fileName);
                    ExcelPackage excelFile = new ExcelPackage(file);
                    ExcelWorksheet worksheet = excelFile.Workbook.Worksheets[1];

                    string[] modelFields;
                    List<MemberListException> memberListProblems = new List<MemberListException>();

                    importFile.FieldMap.FirstWorksheetName = worksheet.Name;
                    importFile.FieldMap.TotalRows = worksheet.Dimension.Rows;
                    importFile.FieldMap.TotalColumns = worksheet.Dimension.Columns;

                    var start = worksheet.Dimension.Start;
                    var end = worksheet.Dimension.End;

                    importFile.FieldMap.FileFields = new string[end.Row][];

                    switch (importFile.ImportTypeId)
                    {
                        // For Applications with Memberlist 
                        case 2:

                            // importFile.Id has been repurposed to act as the supplierId just for this special case.
                            // This list will provide as a check to make sure the application we are looking at are of the correct supplier 
                            List<Product> allProducts = db.Products.ToList();
                            List<Product> validProductCodes = db.Products.Where(product => product.SupplierId == importFile.Id).ToList();
                             
                            modelFields = typeof(Contact).GetProperties()
                                    .Select(property => typeof(Contact).Name + '.' + property.Name)
                                    .ToArray();

                            // Join DB mapping all options
                            importFile.FieldMap.DbFields = modelFields;
                            for (int row = start.Row + 1; row <= end.Row; row++)
                            {
                                string memberNumber = worksheet.Cells[row, 1].Text.Replace("\"", "");
                                string memberInitials = worksheet.Cells[row, 3].Text.Replace("\"", "");
                                string memberSurname = worksheet.Cells[row, 4].Text.Replace("\"", "");
                                string memberIdNumber = worksheet.Cells[row, 5].Text.Replace("\"", "");
                                
                                bool idNumberNull = string.IsNullOrEmpty(memberIdNumber);
                                bool memberIdNull = string.IsNullOrEmpty(memberNumber);

                                // No RSA ID number present, treat as new application / contact 
                                if (idNumberNull)
                                {
                                    if (memberIdNull) // create new contact variable and add to exception list
                                    {
                                        try
                                        {
                                            MemberListException problemMember = new MemberListException()
                                            {
                                                MemberId = memberNumber,
                                                Initials = memberInitials,
                                                MemberSurname = memberSurname,
                                                IdNumber = memberIdNumber,
                                                DateCreated = DateTime.Now,
                                                Reason = "No RSA ID or member number to match on",
                                            };

                                            memberListProblems.Add(problemMember);   
                                        }
                                        catch(Exception e)
                                        {
                                            throw e;
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            MemberListException problemMember = new MemberListException()
                                            {
                                                MemberId = memberNumber,
                                                Initials = memberInitials,
                                                MemberSurname = memberSurname,
                                                IdNumber = memberIdNumber,
                                                DateCreated = DateTime.Now,
                                                Reason = "No RSA ID number to match on",
                                            };

                                            memberListProblems.Add(problemMember);
                                        }
                                        catch (Exception e)
                                        {
                                            throw e;
                                        }
                                    }
                                } 
                                else if (!memberIdNull)
                                {
                                    Contact existingContact = new Contact();
                                    try
                                    {
                                        existingContact = db.Contacts.Where(contact => contact.IdNumber == memberIdNumber).FirstOrDefault();
                                        if (existingContact != null)
                                        {
                                            // Client promised that a client can only have one appliction with a supplier 
                                            List<Application> allExistingApplicationsForClient = db.Applications
                                                .Where(application => application.Client_Id == existingContact.Id)
                                                .ToList();

                                            Application existingApplication = null;
                                            string productCodeReason = "These product arent not associated with choosen supplier: ";

                                            if (allExistingApplicationsForClient != null)
                                            {
                                                foreach(Application app in allExistingApplicationsForClient)
                                                {
                                                    if (validProductCodes.Contains(app.Product))
                                                    {
                                                        existingApplication = app;
                                                    }
                                                    else
                                                    {
                                                        string productName = allProducts
                                                            .Where(e => e.Id == app.Product_Id)
                                                            .Select(result => result.Name)
                                                            .FirstOrDefault();

                                                        productCodeReason += "-- " + productName + " ";
                                                    }
                                                }
                                            }

                                            if (existingApplication != null)
                                            {

                                                existingApplication.ApplicationNumber = memberNumber;
                                                db.Entry(existingApplication).Property("ApplicationNumber").IsModified = true;
                                            }
                                            else
                                            {
                                                MemberListException problemMember = new MemberListException()
                                                {
                                                    MemberId = memberNumber,
                                                    Initials = memberInitials,
                                                    MemberSurname = memberSurname,
                                                    IdNumber = memberIdNumber,
                                                    DateCreated = DateTime.Now,
                                                    Reason = productCodeReason,
                                                };

                                                memberListProblems.Add(problemMember);
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        throw e;
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        MemberListException problemMember = new MemberListException()
                                        {
                                            MemberId = memberNumber,
                                            Initials = memberInitials,
                                            MemberSurname = memberSurname,
                                            IdNumber = memberIdNumber,
                                            DateCreated = DateTime.Now,
                                            Reason = "Entry does not have a member number to update the the database",
                                        };

                                        memberListProblems.Add(problemMember);
                                    }
                                    catch (Exception e)
                                    {
                                        throw e;
                                    }
                                }
                            }

                            foreach(MemberListException member in memberListProblems)
                            {
                                MemberListException existingMember = db.MemberListExceptions.Where(dbentry => dbentry.MemberId == member.MemberId).FirstOrDefault();

                                if (existingMember == null)
                                {
                                    db.MemberListExceptions.Add(member);
                                }
                            }

                            await db.SaveChangesAsync();
                            break;

                        // For contacts
                        case 3:
                            modelFields = typeof(Contact).GetProperties()
                                    .Select(property => typeof(Contact).Name + '.' + property.Name)
                                    .ToArray();

                            // Join DB mapping all options
                            importFile.FieldMap.DbFields = modelFields;

                            // Parse each row in the spreadsheet and persist
                            for (int row = start.Row + 1; row <= end.Row; row++)
                            {
                                // This prevents reading empty rows. If an empty id number is encountered, reading stops.
                                if (!string.IsNullOrEmpty(worksheet.Cells[row, 20].Text))
                                {

                                    // Before attempting to insert this record, ensure that there is no duplicate ID already exsting.
                                    string idNumber = worksheet.Cells[row, 20].Text;
                                    string memberId = worksheet.Cells[row, 38].Text;
                                    bool idNumberNull = string.IsNullOrEmpty(idNumber);
                                    bool memberIdNull = string.IsNullOrEmpty(memberId);

                                    if ((idNumberNull) || (memberIdNull))
                                    {
                                        break;
                                    }
                                    else 
                                    {
                                        Contact existingContact = new Contact();
                                        if (idNumberNull)
                                        {
                                            Application applicationClientId = db.Applications.FirstOrDefault(i => i.ApplicationNumber == memberId);
                                            existingContact = db.Contacts.FirstOrDefault(i => i.Id == applicationClientId.Client_Id);
                                        }
                                        else
                                        {
                                            existingContact = db.Contacts.FirstOrDefault(i => i.IdNumber == idNumber);
                                        }
                                        
                                        if (existingContact == null)
                                        {
                                            // Continue to insert the record iff there is no duplicate record

                                            DateTime? birthdate = null;
                                            if (!string.IsNullOrEmpty(worksheet.Cells[row, 16].Text))
                                            {
                                                birthdate = Convert.ToDateTime(worksheet.Cells[row, 16].Text);
                                            }

                                            DateTime? schemeRegDate = null;
                                            if (!string.IsNullOrEmpty(worksheet.Cells[row, 17].Text))
                                            {
                                                schemeRegDate = Convert.ToDateTime(worksheet.Cells[row, 17].Text);
                                            }

                                            DateTime? schemeTermDate = null;
                                            if (!string.IsNullOrEmpty(worksheet.Cells[row, 18].Text))
                                            {
                                                schemeTermDate = Convert.ToDateTime(worksheet.Cells[row, 18].Text);
                                            }

                                            DateTime? linkDate = null;
                                            if (!string.IsNullOrEmpty(worksheet.Cells[row, 19].Text))
                                            {
                                                linkDate = Convert.ToDateTime(worksheet.Cells[row, 19].Text);
                                            }

                                            Decimal? monthlyFeeCurrent = null;
                                            if (!string.IsNullOrEmpty(worksheet.Cells[row, 29].Text))
                                            {
                                                monthlyFeeCurrent = System.Convert.ToDecimal(worksheet.Cells[row, 29].Text);
                                            }

                                            bool upsell = Boolean.TryParse(worksheet.Cells[row, 32].Text, out upsell);
                                            bool ChronicPMB = Boolean.TryParse(worksheet.Cells[row, 33].Text, out ChronicPMB);

                                            int? dependantAdult = null;
                                            if (!string.IsNullOrEmpty(worksheet.Cells[row, 34].Text))
                                            {
                                                dependantAdult = Int32.Parse(worksheet.Cells[row, 34].Text);
                                            }

                                            int? dependantsChild = null;
                                            if (!string.IsNullOrEmpty(worksheet.Cells[row, 35].Text))
                                            {
                                                dependantsChild = Int32.Parse(worksheet.Cells[row, 35].Text);
                                            }

                                            int? contactTypeId = null;
                                            ContactType contactType = null;
                                            if (!string.IsNullOrEmpty(worksheet.Cells[row, 36].Text))
                                            {
                                                contactTypeId = Int32.Parse(worksheet.Cells[row, 36].Text);
                                                contactType = db.ContactTypes.FirstOrDefault(i => i.Id == contactTypeId);
                                            }

                                            int? contactTitleId = null;
                                            ContactTitles contactTitle = null;
                                            if (!string.IsNullOrEmpty(worksheet.Cells[row, 37].Text))
                                            {
                                                contactTitleId = Int32.Parse(worksheet.Cells[row, 37].Text);
                                                contactTitle = db.ContactTitles.FirstOrDefault(i => i.Id == contactTitleId);
                                            }

                                            Contact contactsObject = new Contact
                                            {
                                                IdNumber = idNumber,
                                                FirstName = worksheet.Cells[row, 1].Text,
                                                Initial = worksheet.Cells[row, 2].Text,
                                                MiddleName = worksheet.Cells[row, 3].Text,
                                                LastName = worksheet.Cells[row, 4].Text,
                                                Email = worksheet.Cells[row, 5].Text,
                                                Email2 = worksheet.Cells[row, 6].Text,
                                                Tel1 = worksheet.Cells[row, 7].Text,
                                                Tel2 = worksheet.Cells[row, 8].Text,
                                                Tel3 = worksheet.Cells[row, 9].Text,
                                                TelWork1 = worksheet.Cells[row, 10].Text,
                                                TelWork2 = worksheet.Cells[row, 11].Text,
                                                TelWork3 = worksheet.Cells[row, 12].Text,
                                                Cell1 = worksheet.Cells[row, 13].Text,
                                                Cell2 = worksheet.Cells[row, 14].Text,
                                                Cell3 = worksheet.Cells[row, 15].Text,
                                                BirthDate = birthdate,
                                                SchemeRegisterDate = schemeRegDate,
                                                SchemeTerminatedDate = schemeTermDate,
                                                LinkDate = linkDate,
                                                GroupCode = worksheet.Cells[row, 21].Text,
                                                JobTitle = worksheet.Cells[row, 22].Text,
                                                EmployerName = worksheet.Cells[row, 23].Text,
                                                Language = worksheet.Cells[row, 24].Text,
                                                Network = worksheet.Cells[row, 25].Text,
                                                BenefitCode = worksheet.Cells[row, 26].Text,
                                                SalaryCode = worksheet.Cells[row, 27].Text,
                                                SalaryScale = worksheet.Cells[row, 28].Text,
                                                MonthlyFeeCurrent = monthlyFeeCurrent,
                                                BusinessUnit = worksheet.Cells[row, 30].Text,
                                                photoUrl = worksheet.Cells[row, 31].Text,
                                                Upsell = upsell,
                                                ChronicPMB = ChronicPMB,
                                                DependantsAdult = dependantAdult,
                                                DependantsChild = dependantsChild,
                                                Deleted = false,
                                                ModifiedDate = DateTime.Now,
                                                CreatedDate = DateTime.Now,
                                                ContactType_Id = contactTypeId,
                                                ContactType = contactType,
                                                ContactTitle_Id = contactTitleId,
                                                ContactTitle = contactTitle,
                                            };
                                            
                                            // save the object
                                            db.Contacts.Add(contactsObject);

                                            // After each row, force flush the db to prevent duplicates within the same spreadsheet
                                            try
                                            {
                                                await db.SaveChangesAsync();
                                            }
                                            catch (Exception exception)
                                            {
                                                // @TODO: concurrency exception?
                                                throw exception;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }

                            break;
                        case 4: // Case its Applications
                            modelFields = typeof(Application).GetProperties()
                                    .Select(property => typeof(Application).Name + '.' + property.Name)
                                    .ToArray();

                            // Join DB mapping all options
                            importFile.FieldMap.DbFields = modelFields;

                            // Parse each row in the spreadsheet and persist
                            for (int row = start.Row + 1; row <= end.Row; row++)
                            {
                                // This prevents reading empty rows. If an empty name is encountered, reading stops.
                                if (!string.IsNullOrEmpty(worksheet.Cells[row, 1].Text))
                                {
                                    Application applicationObject = new Application();
                                    applicationObject.ApplicationNumber = worksheet.Cells[row, 1].Text;
                                    applicationObject.AdvisorCode = worksheet.Cells[row, 2].Text;
                                    applicationObject.Deleted = false;
                                    applicationObject.ModifiedDate = DateTime.Now;
                                    applicationObject.CreatedDate = DateTime.Now;

                                    var advIdValid = worksheet.Cells[row, 4].Text;
                                    if (!string.IsNullOrEmpty(advIdValid))
                                    {
                                        int advId = Int32.Parse(advIdValid);
                                        var adv = db.Advisors.FirstOrDefault(i => i.Id == advId);
                                        if (adv != null)
                                        {
                                            // Advisor advisor = db.Advisors.FirstOrDefault(i => i.Id == Int32.Parse(worksheet.Cells[row, 6].Text));
                                            applicationObject.Advisor = adv;
                                        }
                                    }

                                    var appStatusIdValid = worksheet.Cells[row, 5].Text;
                                    if (!string.IsNullOrEmpty(worksheet.Cells[row, 5].Text))
                                    {
                                        int appStatusId = Int32.Parse(worksheet.Cells[row, 5].Text);
                                        var appStatus = db.ApplicationStatuses.FirstOrDefault(i => i.Id == appStatusId);
                                        if (appStatus != null)
                                        {
                                            applicationObject.ApplicationStatus = appStatus;
                                        }
                                    }

                                    var clientIdValid = worksheet.Cells[row, 6].Text;
                                    if (!string.IsNullOrEmpty(clientIdValid))
                                    {
                                        int clientId = Int32.Parse(clientIdValid);
                                        var client = db.Contacts.FirstOrDefault(i => i.Id == clientId);
                                        if (client != null)
                                        {
                                            applicationObject.Client = client;
                                        }
                                    }

                                    var appTypeValid = worksheet.Cells[row, 7].Text;
                                    if (!string.IsNullOrEmpty(appTypeValid))
                                    {
                                        int appType = Int32.Parse(appTypeValid);
                                        var applicationType = db.ApplicationTypes.FirstOrDefault(i => i.Id == appType);
                                        if (applicationType != null)
                                        {
                                            applicationObject.ApplicationType = applicationType;
                                        }
                                    }

                                    var productIdValid = worksheet.Cells[row, 8].Text;
                                    if (!string.IsNullOrEmpty(productIdValid))
                                    {
                                        int productId = Int32.Parse(productIdValid);
                                        var product = db.Products.FirstOrDefault(i => i.Id == productId);
                                        if (product != null)
                                        {
                                            applicationObject.Product = product;
                                        }
                                    }
                                     
                                    // save the object
                                    db.Applications.Add(applicationObject);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            break;
                        case 5: // Product-Applications 
                            modelFields = typeof(Product).GetProperties()
                                    .Select(property => typeof(Product).Name + '.' + property.Name)
                                    .ToArray();

                            // Join DB mapping all options
                            importFile.FieldMap.DbFields = modelFields;

                            // Parse each row in the spreadsheet and persist
                            for (int row = start.Row + 1; row <= end.Row; row++)
                            {
                                // This prevents reading empty rows. If an empty application id is encountered, reading stops.
                                var applicationId = worksheet.Cells[row, 2].Text;
                                var productId = worksheet.Cells[row, 1].Text;
                                if (!string.IsNullOrEmpty(applicationId) && !string.IsNullOrEmpty(productId))
                                {
                                    int appId = Int32.Parse(applicationId);
                                    var application = db.Applications.FirstOrDefault(i => i.Id == appId);

                                    if (application != null)
                                    {
                                        int prodId = Int32.Parse(productId);
                                        var product = db.Products.FirstOrDefault(i => i.Id == prodId);
                                        if (product != null)
                                        {
                                            if (product.Applications == null)
                                            {
                                                product.Applications = new List<Application>();
                                            }

                                            product.Applications.Add(application);
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            break;
                        case 6: // Application-Suppliers
                            modelFields = typeof(ApplicationSupplier).GetProperties()
                                    .Select(property => typeof(ApplicationSupplier).Name + '.' + property.Name)
                                    .ToArray();

                            // Join DB mapping all options
                            importFile.FieldMap.DbFields = modelFields;

                            // Parse each row in the spreadsheet and persist
                            for (int row = start.Row + 1; row <= end.Row; row++)
                            {
                                // This prevents reading empty rows. If an empty application id is encountered, reading stops.
                                ApplicationSupplier applicationSupplier = new ApplicationSupplier();
                                var applicationId = worksheet.Cells[row, 1].Text;
                                var supplierId = worksheet.Cells[row, 2].Text;
                                var memberNumber = worksheet.Cells[row, 3].Text;

                                if (!string.IsNullOrEmpty(applicationId) && !string.IsNullOrEmpty(supplierId))
                                {
                                    int appId = Int32.Parse(applicationId);
                                    var application = db.Applications.FirstOrDefault(i => i.Id == appId);

                                    if (application != null)
                                    {
                                        int supId = Int32.Parse(supplierId);
                                        var supplier = db.Suppliers.FirstOrDefault(i => i.Id == supId);

                                        if (supplier != null)
                                        {
                                            applicationSupplier.Application = application;
                                            applicationSupplier.Supplier = supplier;
                                            applicationSupplier.MemberNumber = memberNumber;
                                        }
                                    }

                                    // save the object
                                    db.ApplicationSuppliers.Add(applicationSupplier);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            break;
                    }
                }

                // Persist the import files object
                db.ImportFiles.Add(importFile);

                try
                {
                    await db.SaveChangesAsync();
                    if (importFile.ImportTypeId == 1)
                    {
                        CommissionFileStatus comissionfilestatus = new CommissionFileStatus
                        {
                            UserUid = importFile.UserUid,
                            FileName = importFile.FileName,
                            Location = importFile.Location,
                            Size = importFile.Size,
                            Status = "Imported",
                            ImportFileId = importFile.Id

                        };
                        db.CommissionFileStatus.Add(comissionfilestatus);
                    }
                    await db.SaveChangesAsync();
                }
                catch (Exception exception)
                {
                    //@TODO: concurrency exception?
                    throw exception;
                }

                return Ok("{ \"File\":" + importFile.FileName + ", \"message\" : \"File saved successfully.\", \"success\":true }");
            }
            catch (Exception exception)
            {
                return BadRequest("The file data is not properly formatted" + exception.Message);
            }
        }

        // GET: api/Admin/GetUsers
        [Route("Admin/GetUsers")]
        [ResponseType(typeof(IEnumerable<User>))]
        public async Task<IHttpActionResult> GetUsers()
        {
            var ctx = Request.GetOwinContext();
            var userManager = ctx.Get<ApplicationUserManager>();
            var rolesManager = ctx.GetUserManager<ApplicationRoleManager>();
            List<User> users = new List<ViewModels.User>();
            
            var context = new ApplicationDbContext();

            try
            {
                var db_users = (from c in context.Users
                                select c).ToList();

                foreach (var tmpuser in db_users)
                {
                    var luser = new ViewModels.User
                    {
                        FirstName = tmpuser.UserName,
                        LastName = "",
                        Email = tmpuser.Email,
                        AdvisorId = tmpuser.AdvisorId,
                    };

                    //For those that have advisorids populate
                    if (tmpuser.AdvisorId != null)
                    {

                        var advisor = db.Advisors
                            .Where(c => c.Id == tmpuser.AdvisorId)
                            .FirstOrDefault();

                        var cntk = db.Contacts.Where(c => c.Id == advisor.ContactId).FirstOrDefault();

                        if (luser.FirstName == null)
                        {
                            luser.FirstName = cntk.FirstName;
                        }

                        if (luser.LastName == null)
                        {
                            luser.LastName = cntk.LastName;
                        }
                            
                        if (luser.Email == null)
                        {
                            luser.Email = cntk.Email;
                        }    
                    }

                    ApplicationUser userLogin = await userManager.FindByEmailAsync(luser.Email);

                    if (userLogin != null)
                    {
                        IdentityRole adminRole = rolesManager.FindByName("Admin");
                        IdentityUserRole userRole = userLogin.Roles.FirstOrDefault();

                        luser.Username = userLogin.UserName;
                        luser.Password = "********";
                        luser.IsAdmin = userLogin.Advisor.User.IsAdmin;
                    }
                    users.Add(luser);
                }
            }
            catch
            {
                throw;
            }

            return Ok(users);
        }

        // POST: api/Admin/AddOrUpdateUser
        [Route("Admin/AddOrUpdateUser")]
        public async Task<IHttpActionResult> PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var ctx = Request.GetOwinContext();
                var userManager = ctx.Get<ApplicationUserManager>();
                var rolesManager = ctx.GetUserManager<ApplicationRoleManager>();
                var _user = userManager.FindById(HttpContext.Current.User.Identity.GetUserId());
                AdvisorType advisorType = db.AdvisorTypes.Where(match => match.Title == "Admin").FirstOrDefault();
                AdvisorStatus advisorStatus = db.AdvisorStatuses.Where(match => match.Name == "Pending").FirstOrDefault();

                if (_user != null && !User.IsInRole("Supervisor"))
                {
                    return BadRequest(ModelState);
                }

                //Find the system id for supervisor role, 
                // we need to create a helper class for this
                string superId = "";
                using (var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext())))
                {
                    if (rm.RoleExists("Supervisor") == true)
                    {
                        superId = rm.FindByName("Supervisor").Id;
                    }
                }


                if (user.AdvisorId == 0)
                {
                    Contact contact = new Contact()
                    {
                        Email = user.Email,
                        FirstName = user.Username,
                        LastName = "AdminUser",
                        IdNumber = "8181818181810",
                    };

                    contact.ContactType = db.ContactTypes.Where(c => c.Name == "Private Individual").FirstOrDefault();
                    
                    db.Contacts.Add(contact);
                    await db.SaveChangesAsync();

                    Advisor advisor = new Advisor {
                        Contact = contact,
                        IsKeyIndividual = false,
                        Deleted = false,
                        LastLoginDate = null,
                        ModifiedDate = null,
                        CreatedDate = DateTime.Now,
                        User = user,
                        Company_Id = 1,
                        AdvisorType_Id = advisorType.Id,
                        AdvisorStatus_Id = advisorStatus.Id,
                    };

                    if (advisor.User.FirstName == null)
                    {
                        advisor.User.FirstName = user.Username;
                    }

                    advisor.User.Password = user.Password;
                    advisor.User.IsAdmin = true;

                    //Create an empty advisor
                    db.Advisors.Add(advisor);
                    await db.SaveChangesAsync();
                    user.AdvisorId = advisor.Id;
                }

                ApplicationUser userLogin = await userManager.FindByEmailAsync(user.Email);
                ApplicationDbContext context = new ApplicationDbContext();
                IdentityRole adminRole = rolesManager.FindByName("Admin");
                IdentityRole advisorRole = rolesManager.FindByName("Advisor");
                AdvisorType adminTypeId = context.AdvisorTypes.Where(c => c.Title == "Admin").FirstOrDefault();
                AdvisorType advisorTypeId = context.AdvisorTypes.Where(c => c.Title == "Adviser").FirstOrDefault();

                if (userLogin != null)
                {
                    IdentityUserRole userRole = userLogin.Roles.FirstOrDefault();
                    userLogin.Advisor.User.Email = user.Email;
                    userLogin.Advisor.User.Username = user.Username;

                    if (userLogin.Advisor.User.IsAdmin != user.IsAdmin)
                    {
                        userLogin.Advisor.User.IsAdmin = user.IsAdmin;

                        try
                        {
                            // User has become an Admin
                            if (user.IsAdmin && (userRole.RoleId != adminRole.Id))
                            {
                                userLogin.Advisor.AdvisorType_Id = adminTypeId.Id;
                                userManager.RemoveFromRole(userRole.UserId, advisorRole.Name);
                                userManager.AddToRole(userRole.UserId, adminRole.Name);
                            }
                            // User is now an Advisor 
                            else if (!user.IsAdmin && (userRole.RoleId != advisorRole.Id))
                            {
                                userLogin.Advisor.AdvisorType_Id = advisorTypeId.Id;
                                userManager.RemoveFromRole(userRole.UserId, adminRole.Name);
                                userManager.AddToRole(userRole.UserId, advisorRole.Name);
                            }

                            userLogin = await userManager.FindByNameAsync(user.Username);
                            var userRoles = userLogin.Roles.Where(c => c.RoleId == superId).Count() > 0;
                            // now check if he need to be supervisor

                            if (user.IsAdmin)
                            {
                                // If the user is not supervisor add this role
                                if (!userRoles)
                                {
                                    userManager.AddToRole(userLogin.Id, "Admin");
                                }
                            }
                            else
                            {   // If the user is no longer supervisor check that the role has been removed
                                if (userRoles)
                                {
                                    userManager.RemoveFromRole(userLogin.Id, "Admin");
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e);
                        }

                        db.Entry(userRole).Property("RoleId").IsModified = true;
                        db.SaveChanges();
                    }

                    //userLogin.PasswordHash = userManager.PasswordHasher.HashPassword(user.Password);
                    if (user.ConfirmPassword == user.Password)
                    {
                        string resetToken = await userManager.GeneratePasswordResetTokenAsync(userLogin.Id);
                        IdentityResult passwordChangeResult = await userManager.ResetPasswordAsync(userLogin.Id, resetToken, user.Password);
                    }

                    ////persist user changes
                    await userManager.UpdateAsync(userLogin);
                }
                else
                {
                    try
                    {
                        ApplicationUser us = new ApplicationUser()
                        {
                            AdvisorId = user.AdvisorId,
                            UserName = user.Username,
                            Email = user.Email,
                            JoinDate = DateTime.Now,
                            LockoutEnabled = false
                        };
                        IdentityResult res = await userManager.CreateAsync(us, user.Password);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // POST: api/Admin/ApproveImport
        [Route("Admin/approveImport/{Id}/{SupplierId}/{FromDate}/{ToDate}")]
        public async Task<IHttpActionResult> ApproveImport(int Id, int SupplierId, DateTime? FromDate, DateTime? ToDate)
        {
            if (Equals(FromDate, null) || Equals(ToDate, null))
            {
                return NotFound();
            }

            try
            {
                FromDate = DateTime.Parse(String.Format("{0:yyyy/MM/dd}", FromDate));
                ToDate = DateTime.Parse(String.Format("{0:yyyy/MM/dd}", ToDate));

                var comissionfilestatus = db.CommissionFileStatus.
                     FirstOrDefault(e => e.ImportFileId == Id);


                comissionfilestatus.Status = "Completed";
                comissionfilestatus.ComissionRunDateFrom = (DateTime)FromDate;
                comissionfilestatus.ComissionRunDateTo = (DateTime)ToDate;
                comissionfilestatus.SupplierId = SupplierId;
                db.SaveChanges();


                var UnmatchedList = await db.UnmatchedCommissions.ToListAsync();

                var newClient = new Contact();
                var newComission = new CommissionStatement();
                var newApp = new Application();
                var newAdvi = new Advisor();


                var suppliersShares = (from x in db.Suppliers
                                       join y in db.AdvisorShareUnderSupervisions
                                       on x.Name equals y.supplier
                                       select new
                                       {
                                           AdviserId = y.AdvisorId,
                                           SupplierId = x.Id
                                       })
                            .ToList();

                var ApplicationSupplier =
                        (from Apps in db.Applications
                         join Prod in db.Products on Apps.Product_Id equals Prod.Id
                         join Suppliers in db.Suppliers on Prod.SupplierId equals Suppliers.Id
                         select new
                         {
                             AppId = Apps.Id,
                             SuppliersId = Suppliers.Id
                         }).ToList();

                foreach (var UnmatchedCommission in UnmatchedList)
                {
                    try
                    {
                        newComission.Surname = UnmatchedCommission.Surname;
                        newComission.MemberSearchValue = UnmatchedCommission.MemberSearchValue;
                        if (UnmatchedCommission.MemberSearchKey == "IDNumber")
                        {
                            // TODO: clean up these db calls 
                            var contactExists = db.Contacts.FirstOrDefault(e => e.IdNumber == UnmatchedCommission.MemberSearchValue);
                            if (!Equals(contactExists, null))
                            {
                                newClient = db.Contacts.FirstOrDefault(e => e.IdNumber == UnmatchedCommission.MemberSearchValue);
                                newApp = db.Applications.FirstOrDefault(e => e.Client_Id == newClient.Id && e.Deleted == false);
                                var clientApp = db.Applications.FirstOrDefault(e => e.Client_Id == newClient.Id);

                                if (newApp != null && !Equals(clientApp, null))
                                {
                                    newAdvi = db.Advisors.FirstOrDefault(e => e.Id == newApp.Advisor_Id);

                                    //Check if adviser's effective date is in range 
                                    var validEffectiveDate = true;

                                    if ((newAdvi.EffectiveEndDate != null && newAdvi.EffectiveEndDate < ToDate) || (newAdvi.EffectiveStartDate > ToDate))
                                    {
                                        validEffectiveDate = false;
                                    }

                                    if (validEffectiveDate)
                                    {
                                        if (newAdvi.AdvisorStatus_Id == 5)
                                        {
                                            var appExists = db.Applications
                                                .Where(e => e.Id == newApp.Id && e.Advisor_Id == newApp.Advisor_Id).Any();
                                            if (appExists)
                                            {
                                                var advisorDocumentCheck = db.AdvisorDocuments
                                                    .Where(e => e.Advisor_Id == newApp.Advisor_Id && e.DocumentTypeId == 10 && e.ValidFromDate >= DateTime.Now && e.ValidToDate <= DateTime.Now)
                                                    .Any();

                                                if (!advisorDocumentCheck)
                                                {
                                                    var supplierSharesCheck = suppliersShares
                                                        .Where(e => e.AdviserId == newApp.Advisor_Id && e.SupplierId == UnmatchedCommission.SupplierId)
                                                        .Any();

                                                    if (supplierSharesCheck)
                                                    {
                                                        var appEffect = db.ApplicationAdvisorHistory
                                                            .Where(e => e.Application_Id == newApp.Id && e.New_Advisor == newApp.Advisor.Id)
                                                            .ToArray();
                                                        var effectiveDate = appEffect.Count() > 0 ? appEffect.Last() : null;

                                                        if (!Equals(effectiveDate,null))
                                                        {
                                                            if (effectiveDate.DateStarted <= ToDate)
                                                                {
                                                                    if (newApp.ApplicationStatus_Id == 6)
                                                                    {
                                                                        if (!db.AdvisorShareUnderSupervisions.Where(
                                                                             e => e.AdvisorId == newApp.Advisor_Id &&
                                                                             e.validCommissionFromDate <= DateTime.Now &&
                                                                             (e.validCommissionToDate == null ? false : e.validCommissionToDate >= DateTime.Now)).Any())
                                                                        {
                                                                            newComission.TransactionDate = UnmatchedCommission.TransactionDate;
                                                                            newComission.SubscriptionDue = UnmatchedCommission.SubscriptionDue;
                                                                            newComission.SubscriptionReceived = UnmatchedCommission.SubscriptionReceived;
                                                                            newComission.AdvisorId = newApp.Advisor_Id;
                                                                            newComission.ClientId = newApp.Client_Id;
                                                                            newComission.ProductId = (int)newApp.Product_Id;
                                                                            newComission.MemberNumber = UnmatchedCommission.MemberNumber;
                                                                            newComission.MemberSearchKey = UnmatchedCommission.MemberSearchKey;
                                                                            newComission.MemberSearchValue = UnmatchedCommission.MemberSearchValue;
                                                                            newComission.CommisionRunDate = UnmatchedCommission.CommisionRunDate;
                                                                            newComission.CommisionRunUser = UnmatchedCommission.CommisionRunUser;
                                                                            newComission.CommissionInclVAT = UnmatchedCommission.CommissionInclVAT;
                                                                            newComission.CommissionExclVAT = UnmatchedCommission.CommissionExclVAT;
                                                                            newComission.AdvisorTaxRate = calVat(Convert.ToDecimal(newComission.CommissionExclVAT), Convert.ToDecimal(newComission.CommissionInclVAT));
                                                                            newComission.ImportFileId = UnmatchedCommission.ImportFileId;
                                                                            newComission.SupplierId = UnmatchedCommission.SupplierId;
                                                                            newComission.ApprovalStatus = "APPROVED";
                                                                            newComission.ApprovalDateTo = ToDate;
                                                                            newComission.ApprovalDateFrom = FromDate;

                                                                            db.CommissionStatement.Add(newComission);
                                                                            db.UnmatchedCommissions.Remove(UnmatchedCommission);
                                                                            db.SaveChanges();
                                                                        }
                                                                        else
                                                                        {
                                                                            UnmatchedCommission.Reasons = " Commission split date not valid ";
                                                                            continue;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        UnmatchedCommission.Reasons = " Application status not complete ";
                                                                        continue;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    UnmatchedCommission.Reasons = " Application's effective date not in range ";
                                                                    continue;
                                                                }
                                                        }
                                                        else
                                                        {
                                                            UnmatchedCommission.Reasons = " Check Adviser history and refresh unmatched commissions ";
                                                            continue;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        UnmatchedCommission.Reasons = " No Supplier Licence For This Application ";
                                                        continue;
                                                    }
                                                }
                                                else
                                                {
                                                    UnmatchedCommission.Reasons = " CMS (BR) document expired ";
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                UnmatchedCommission.Reasons = " Advisor not linked to application ";
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                             UnmatchedCommission.Reasons = " Advisor not complete  ";
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        UnmatchedCommission.Reasons = " Adviser Effective date not in range ";
                                    }
                                }
                                else
                                {
                                    UnmatchedCommission.Reasons = " No application assigned to client  ";
                                    continue;
                                }
                            }
                            else
                            {
                                UnmatchedCommission.Reasons = " Client does not exist  ";
                                continue;
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.Write("Error : " + ex.Message);
                    }
                }

                var CommissionsRefreshed = db.CommissionStatement.Where(e => e.ApprovalStatus == "REFRESHED");
                foreach (var comm in CommissionsRefreshed)
                {
                    comm.ApprovalStatus = "APPROVED";
                    comm.ApprovalDateTo = ToDate;
                    comm.ApprovalDateFrom = FromDate;
                }

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.Write("Error : " + ex.Message);
            }

            return Ok();
        }

        // POST: api/Admin/ApproveImport
        [Route("Admin/refreshUnmatched/")]
        public async Task<IHttpActionResult> RefreshUnmatched()
        {
            try
            {
                var UnmatchedList = await db.UnmatchedCommissions.ToListAsync();
                var newComission = new CommissionStatement();
                var newApplication = new Application();
                var newAdvi = new Advisor();

                var suppliersShares =
                   (from x in db.Suppliers
                    join y in db.AdvisorShareUnderSupervisions
                    on x.Id equals y.SupplierId
                    select new
                    {
                        AdviserId = y.AdvisorId,
                        SupplierId = x.Id
                    })
                    .ToList();

                var ApplicationSupplier =
                    (from Apps in db.Applications
                     join Prod in db.Products on Apps.Product_Id equals Prod.Id
                     join Suppliers in db.Suppliers on Prod.SupplierId equals Suppliers.Id
                     select new
                     {
                         AppId = Apps.Id,
                         SuppliersId = Suppliers.Id
                     }).ToList();
                
                // Get Commission date from unmacthed commissions
                SqlDateTime SQLMinDate = SqlDateTime.MinValue;
                DateTime? commissionDateFromSCV = (DateTime)SQLMinDate;
                DateTime endDate = (DateTime)SQLMinDate;

                if (UnmatchedList.Count() > 1)
                {
                    commissionDateFromSCV = UnmatchedList.LastOrDefault().TransactionDate;
                    //Set end date 
                    endDate = new DateTime(
                        commissionDateFromSCV.Value.Year,
                        commissionDateFromSCV.Value.Month,
                        DateTime.DaysInMonth(commissionDateFromSCV.Value.Year, commissionDateFromSCV.Value.Month));
                }
                else if (db.CommissionStatement.Count() > 1)
                {
                    commissionDateFromSCV = db.CommissionStatement.LastOrDefault().TransactionDate;
                    endDate = new DateTime(
                        commissionDateFromSCV.Value.Year,
                        commissionDateFromSCV.Value.Month,
                        DateTime.DaysInMonth(commissionDateFromSCV.Value.Year, commissionDateFromSCV.Value.Month));
                }

                foreach (var UnmatchedCommission in UnmatchedList)
                {
                    newComission.Surname = UnmatchedCommission.Surname;
                    newComission.MemberSearchValue = UnmatchedCommission.MemberSearchValue;

                    try
                    {
                        Contact client = null;
                        if (UnmatchedCommission.MemberSearchKey == "IDNumber")
                        {
                            client = db.Contacts
                                .FirstOrDefault(e => e.IdNumber == UnmatchedCommission.MemberSearchValue);

                        }
                        else if (UnmatchedCommission.MemberSearchKey == "MemberNumber")
                        {
                            Application clientApplication = db.Applications
                                .FirstOrDefault(e => e.ApplicationNumber == UnmatchedCommission.MemberSearchValue);

                            if(!Equals(clientApplication, null))
                            {
                                client = db.Contacts
                                .FirstOrDefault(e => e.Id == clientApplication.Client_Id);
                            }
                        }
                        if (!Equals(client, null))
                        {
                            try
                            {
                                newApplication = db.Applications
                                    .FirstOrDefault(e 
                                        => (e.Client_Id == client.Id) 
                                        && (e.Deleted == false) 
                                        && (UnmatchedCommission.SupplierId == e.ApplicationSuppliers.FirstOrDefault().SupplierId));

                                //Application clientApplicationExists = db.Applications
                                //    .FirstOrDefault(e => e.Client_Id == client.Id);
                                // && !Equals(clientApplicationExists, null)

                                if (!Equals(newApplication, null))
                                {
                                    newAdvi = db.Advisors.FirstOrDefault(e => e.Id == newApplication.Advisor_Id);

                                    if(UnmatchedCommission.AdvisorName == "")
                                    {
                                        UnmatchedCommission.AdvisorName = newAdvi.User.FullName;
                                    }
                                    
                                    // Check if the advisor effective date is within date range
                                    var validEffectiveDate = true;

                                    if ((newAdvi.EffectiveEndDate != null && newAdvi.EffectiveEndDate < endDate) ||
                                        (newAdvi.EffectiveStartDate > endDate))
                                    {
                                        validEffectiveDate = false;
                                    }

                                    if (!validEffectiveDate)
                                    {
                                        UnmatchedCommission.Reasons = " Adviser Effective date not in range ";
                                        continue;
                                    }
                                    else
                                    {
                                        if (newAdvi.AdvisorStatus_Id == 5)
                                        {
                                            bool applicationWithAdvisor = db.Applications
                                                .Where(e => e.Id == newApplication.Id && e.Advisor_Id == newApplication.Advisor_Id)
                                                .Any();

                                            if (applicationWithAdvisor)
                                            {
                                                bool validDocumentation = db.AdvisorDocuments
                                                    .Where(e => e.Advisor_Id == newApplication.Advisor_Id
                                                        && e.DocumentTypeId == 10
                                                        && e.ValidFromDate >= DateTime.Now
                                                        && e.ValidToDate <= DateTime.Now)
                                                    .Any();

                                                if (!validDocumentation)
                                                {
                                                    bool supplierAdvisorMatch = suppliersShares
                                                        .Where(e => e.AdviserId == newAdvi.Id && e.SupplierId == UnmatchedCommission.SupplierId)
                                                        .Any();

                                                    if (supplierAdvisorMatch)
                                                    {
                                                        // Get effective date for the application 
                                                        var effectiveDate = db.ApplicationAdvisorHistory
                                                            .Where(e => e.Application_Id == newApplication.Id && e.New_Advisor == newAdvi.Id)
                                                            .Last();

                                                        if (effectiveDate != null && effectiveDate.DateStarted  <= endDate)
                                                        {

                                                            if (newApplication.ApplicationStatus_Id == 6)
                                                            {
                                                                bool validCommissionSplit = db.AdvisorShareUnderSupervisions
                                                                    .Where(
                                                                        e => e.AdvisorId == newApplication.Advisor_Id
                                                                        && e.validCommissionFromDate <= DateTime.Now
                                                                        && (e.validCommissionToDate == null ? false : e.validCommissionToDate >= DateTime.Now))
                                                                    .Any();

                                                                if (!validCommissionSplit)
                                                                {
                                                                    CommissionStatement newCommissionEntry = new CommissionStatement
                                                                    {
                                                                        TransactionDate = UnmatchedCommission.TransactionDate,
                                                                        SubscriptionDue = UnmatchedCommission.SubscriptionDue,
                                                                        SubscriptionReceived = UnmatchedCommission.SubscriptionReceived,
                                                                        AdvisorId = newApplication.Advisor_Id,
                                                                        ClientId = newApplication.Client_Id,
                                                                        ProductId = (int)newApplication.Product_Id,
                                                                        MemberSearchKey = UnmatchedCommission.MemberSearchKey,
                                                                        MemberNumber = UnmatchedCommission.MemberNumber,
                                                                        MemberSearchValue = UnmatchedCommission.MemberSearchValue,
                                                                        Initial = UnmatchedCommission.Initial,
                                                                        Surname = UnmatchedCommission.Surname,
                                                                        CommisionRunDate = UnmatchedCommission.CommisionRunDate,
                                                                        CommisionRunUser = UnmatchedCommission.CommisionRunUser,
                                                                        CommissionInclVAT = UnmatchedCommission.CommissionInclVAT,
                                                                        CommissionExclVAT = UnmatchedCommission.CommissionExclVAT,
                                                                        ImportFileId = UnmatchedCommission.ImportFileId,
                                                                        SupplierId = UnmatchedCommission.SupplierId,
                                                                        ApprovalStatus = "REFRESHED"
                                                                    };

                                                                    newCommissionEntry.AdvisorTaxRate = calVat(
                                                                        Convert.ToDecimal(newCommissionEntry.CommissionExclVAT),
                                                                        Convert.ToDecimal(newCommissionEntry.CommissionInclVAT));
                                                                        
                                                                    db.CommissionStatement.Add(newCommissionEntry);
                                                                    db.UnmatchedCommissions.Remove(UnmatchedCommission);
                                                                }
                                                                else
                                                                {
                                                                    UnmatchedCommission.Reasons = " Commission split date not valid ";
                                                                    continue;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                UnmatchedCommission.Reasons = " Application status not complete ";
                                                                continue;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            UnmatchedCommission.Reasons = " Application's effective date not in range ";
                                                            continue;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        UnmatchedCommission.Reasons = " No Supplier Licence For This Application ";
                                                        continue;
                                                    }
                                                }
                                                else
                                                {
                                                    UnmatchedCommission.Reasons = " CMS (BR) document expired ";
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                UnmatchedCommission.Reasons = " Advisor not linked to application ";
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            UnmatchedCommission.Reasons = " Advisor status not complete ";
                                            continue;
                                        }
                                    }
                                }
                                else
                                {
                                    UnmatchedCommission.Reasons = " No application assigned to client ";
                                    continue;
                                }
                            }
                            catch(Exception ex) {
                                Console.Write("Error : " + ex.Message);
                                UnmatchedCommission.Reasons = "Check Adviser history and refresh unmatched commissions";
                            }
                        }
                        else
                        {
                            UnmatchedCommission.Reasons = " Client does not exist ";
                            continue;
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.Write("Error : " + ex.Message);
                    }
                }

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.Write("Error : " + ex.Message);
            }

            return Ok();
        }

        // POST: api/Admin/ApproveImport
        [Route("Admin/CheckComissionRun/{SupplierId}/{datefrom}/{dateto}")]
        public IHttpActionResult CheckComissionRun(int SupplierId, DateTime? datefrom, DateTime? dateto)
        {
            try
            {
                var CommissionRun = db.CommissionFileStatus
                    .Where(e => e.ComissionRunDateFrom <= datefrom 
                        && e.ComissionRunDateTo >= dateto
                        && e.SupplierId == SupplierId
                        && e.Status == "Completed")
                    .Any();

                if (CommissionRun)
                {
                    return NotFound();
                }
            }
            catch
            {
                throw;
            }

            return Ok();
        }
    }
}
