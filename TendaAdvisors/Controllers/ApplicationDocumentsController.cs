using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using TendaAdvisors.Models;
using TendaAdvisors.Models.Response;
using TendaAdvisors.Providers;


namespace TendaAdvisors.Controllers
{

    [Authorize]
    public class ApplicationDocumentsController : BaseApiController
    {
        private readonly string uploadsFolder = HttpRuntime.AppDomainAppPath + @"Uploads\ApplicationDocuments";

        public ApplicationDocumentsController()
        {

            if (!Directory.Exists(this.uploadsFolder))
            {
                try
                {
                    Directory.CreateDirectory(this.uploadsFolder);
                }
                catch (Exception ex)
                {
                    // Could not create folder.
                    throw;
                }
            }
        }

        private ApplicationDocumentsController(ApplicationDbContext dbcontext)
        {

            if (!Directory.Exists(this.uploadsFolder))
            {
                Directory.CreateDirectory(this.uploadsFolder);
            }
        }

        private string saveFile(string fileName, byte[] data)
        {
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            }

            if (File.Exists(fileName))
            {
                Console.WriteLine(fileName + " already exists!");
                return "";
            }

            FileStream fs = new FileStream(fileName.Replace(@"\\", @"\"), FileMode.CreateNew);
            fs.Write(data, 0, data.Length);
            fs.Close();

            return fileName;
        }

        private byte[] base64stringToByteArray(string base64Data)
        {
            var stripHeader = base64Data.Split(',');

            if (stripHeader.Length < 2)
            {
                return null;
            }

            byte[] data = Convert.FromBase64String(stripHeader[1]);
            //var dataString = Encoding.UTF8.GetString(data);
            return data;
        }


        // GET: api/ApplicationDocuments
        [ResponseType(typeof(IQueryable<DocumentTypeResponse>))]
        public async Task<IHttpActionResult> GetApplicationDocuments()
        {
            IEnumerable<DocumentTypeResponse> applicationDocuments =
                await (from doc in db.ApplicationDocuments
                       join app in db.Applications on doc.ApplicationId equals app.Id
                       join client in db.Contacts on app.Client_Id equals client.Id
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
                           IsExpired = doc.IsExpired,
                           ApplicationId = doc.ApplicationId,
                           client = new BasicContactResponse() {
                               FirstName = client.FirstName,
                               LastName = client.LastName,
                               IdNumber = client.IdNumber,
                               Id = client.Id
                           }
                       }).ToListAsync();
            return Ok(applicationDocuments);
        }

        // GET: api/ApplicationDocuments
        [Route("AdvisorDocumentsForAdvisor/{AdviId}")]
        [ResponseType(typeof(IQueryable<DocumentTypeResponse>))]
        public async Task<IHttpActionResult> GetAdvisorDocumentsForAdvisor(int AdviId)
        {
            IEnumerable<DocumentTypeResponse> applicationDocuments =
                await (from doc in db.ApplicationDocuments
                       join app in db.Applications on doc.ApplicationId equals app.Id
                       join client in db.Contacts on app.Client_Id equals client.Id
                       where app.Advisor_Id == AdviId
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
                           IsExpired = doc.IsExpired,
                           ApplicationId = doc.ApplicationId,
                           client = new BasicContactResponse()
                           {
                               FirstName = client.FirstName,
                               LastName = client.LastName,
                               IdNumber = client.IdNumber,
                               Id = client.Id
                           }
                       }).ToListAsync();
            return Ok(applicationDocuments);
        }

        // GET: api/ApplicationDocuments/5
        [ResponseType(typeof(ApplicationDocument))]
        public async Task<IHttpActionResult> GetApplicationDocument(int id)
        {
            ApplicationDocument applicationDocument = await db.ApplicationDocuments.FindAsync(id);
            if (applicationDocument == null)
            {
                return NotFound();
            }

            return Ok(applicationDocument);
        }

        // GET: api/ApplicationDocuments/5/File
        [Route("ApplicationDocuments/{id}/File")]
        public HttpResponseMessage GetApplicationDocumentFile(int id)
        {
            if (id <= 0)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var appDoc = db.ApplicationDocuments.Find(id);

            if (appDoc == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            ApplicationUser user = HttpContext.Current.GetOwinContext()
                .GetUserManager<ApplicationUserManager>()
                .FindById(HttpContext.Current.User.Identity.GetUserId());

            //@TODO: ROLES or CLAIMS!
            if (user == null)
            {
                return new HttpResponseMessage(HttpStatusCode.Forbidden);
            }

            if (appDoc != null)
            {
                db.Entry(appDoc).Reference(a => a.Application).Load();
                try
                {
                    var stream = new FileStream(appDoc.Location, FileMode.Open);
                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StreamContent(stream),
                    };

                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = appDoc.OriginalFileName
                    };

                    result.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(appDoc.OriginalFileName));
                    result.Content.Headers.ContentLength = stream.Length;
                    appDoc.Location = "";

                    return result;
                }
                catch (DirectoryNotFoundException directoryException)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                    throw;
                }
                catch (Exception exception)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                    throw;
                }
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.Forbidden);
            }
        }


        [Route("ApplicationDocuments/{appDocId}/{status}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutchangeIsExpired(int appDocId, bool status)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                ApplicationDocument at = db.ApplicationDocuments.Find(appDocId);
                if (at != null)
                {


                    at.IsExpired = status;

                    db.Entry(at).CurrentValues.SetValues(at);

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
                throw;

            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/ApplicationDocuments/5
        [ResponseType(typeof(void))]
        [Route("ApplicationDocuments/{applicationId}")]
        public async Task<IHttpActionResult> PutApplicationDocument(int applicationId, ApplicationDocument applicationDocument)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var originalApplication = db.Applications.Find(applicationId);
            var originalAppDoc = db.ApplicationDocuments.Find(applicationDocument.Id);
            var originalAppDocType = db.DocumentTypes.Find(applicationDocument.DocumentTypeId);
            if (originalAppDocType == null)
            {
                originalAppDocType = db.DocumentTypes.FirstOrDefault();
            }

            if (originalAppDoc == null)
            {
                applicationDocument.Application = originalApplication;
                applicationDocument.DocumentType = originalAppDocType;
                applicationDocument.DocumentTypeId = originalAppDocType.Id;
                applicationDocument.CreatedDate = DateTime.Now;
                applicationDocument.IsExpired = applicationDocument.ValidToDate < DateTime.Now;
                db.Entry(applicationDocument).State = EntityState.Added;
            } else
            {
                originalAppDoc.Application = originalApplication;
                applicationDocument.IsRequired = originalAppDoc.IsRequired;
                applicationDocument.IsExpired = applicationDocument.ValidToDate < DateTime.Now ? true : originalAppDoc.IsExpired;
                db.Entry(originalAppDoc).CurrentValues.SetValues(applicationDocument);
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!ApplicationDocumentExists(applicationId))
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
        
        // POST: api/ApplicationDocuments/5/Product/2
        [Route("ApplicationDocuments/{applicationId}/ProductId/{productId}/DocumentTypeId/{documentId}/{submitted}")]
        [ResponseType(typeof(ApplicationDocument))]
        public async Task<IHttpActionResult> PostApplicationDocument(int applicationId, int productId, int documentId, ApplicationDocument applicationDocument, bool submitted)
        {
            if (applicationId == 0 || productId == 0)
            {
                return BadRequest("Cannot locate Application or Product.");
            }
            var application = await db.Applications.FindAsync(applicationId);
            var product = await db.Products.FindAsync(productId);
            var documentType = await db.DocumentTypes.FindAsync(documentId);


            if (application == null || product == null)
            {
                return BadRequest("Cannot locate Application or Product.");
            }

            applicationDocument.Application = application;
            applicationDocument.Product = product;
            applicationDocument.DocumentType = documentType;
            applicationDocument.Uploaded = submitted;

            if (string.IsNullOrEmpty(applicationDocument.File))
            {
                return BadRequest("No file data received.");
            }

            try
            {
                //@TODO: Add Aplication & Product Folders
                string fileName = (
                    uploadsFolder + "\\"
                    + applicationId + "\\"
                    + productId + "\\AppDocFile"
                    + (DateTime.Now.Ticks.ToString()).Replace(@"/", "-"));

                byte[] data = base64stringToByteArray(applicationDocument.File);

                if (data == null)
                {
                    //could not save file.
                    return BadRequest();
                }

                fileName = saveFile(fileName, data);

                if (fileName == "")
                {
                    //could not save file.
                    return BadRequest();
                }

                applicationDocument.OriginalFileName = applicationDocument.Location;
                applicationDocument.Location = fileName;
                applicationDocument.CreatedDate = DateTime.Now;
                applicationDocument.ModifiedDate = applicationDocument.CreatedDate;
                applicationDocument.File = null;
                //applicationDocument.DocumentType = new DocumentType() { Id = 1 };

                db.ApplicationDocuments.Add(applicationDocument);

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception exception)
                {
                    //@TODO: concurrency exception?
                    throw exception;
                }
                //return Ok("{ \"applicationDocumentId\":" + applicationDocument.Id + ", \"message\" : \"File saved successfully.\", \"success\":true }");
                applicationDocument.File = null;
                DocumentTypeResponse ApplicationDocumentResponse = new DocumentTypeResponse()
                {
                    ApplicationId = applicationDocument.ApplicationId,
                    DocumentTypeId = applicationDocument.DocumentTypeId,
                    DocumentTypeName = applicationDocument.DocumentType.Name,
                    Id = applicationDocument.Id,
                    IsExpired = applicationDocument.IsExpired,
                    Name = applicationDocument.OriginalFileName,
                    Title = applicationDocument.Title,
                    Uploaded = applicationDocument.Uploaded,
                    ValidFromDate = applicationDocument.ValidFromDate,
                    ValidToDate = applicationDocument.ValidToDate
                };
                return Ok(ApplicationDocumentResponse);

            }
            catch (Exception exception)
            {
                return BadRequest("{ data:{\"applicationDocumentId\":" + applicationDocument.Id + "}, \"statusText\" : \"Error: File could not be saved to disk.\", \"status\":-1}");
                //throw;
            }

        }


        // DELETE: api/ApplicationDocuments/5
        [ResponseType(typeof(ApplicationDocument))]
        public async Task<IHttpActionResult> DeleteApplicationDocument(int id)
        {
            ApplicationDocument applicationDocument = await db.ApplicationDocuments.FindAsync(id);
            if (applicationDocument == null)
            {
                return NotFound();
            }

            applicationDocument.Deleted = true;
            var origAppDoc = await db.ApplicationDocuments.FindAsync(applicationDocument.Id);
            db.Entry(origAppDoc).CurrentValues.SetValues(applicationDocument);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return Ok(applicationDocument);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ApplicationDocumentExists(int id)
        {
            return db.ApplicationDocuments.Count(e => e.Id == id) > 0;
        }

        [Route("ApplicationDocuments/DeleteDupAppocument/{Id}/{AppId}/{DocType}")]
        public async Task<IHttpActionResult> DeleteDupAppocument(int Id, int AppId, int DocType)
        {
            try
            {
                int AppDocsCount = await db.ApplicationDocuments
                    .Where(e => e.DocumentTypeId == DocType && e.ApplicationId == AppId)
                    .CountAsync();

                if (AppDocsCount > 1)
                {
                    var AppDoc = await db.ApplicationDocuments.FirstOrDefaultAsync(e => e.Id == Id);
                    db.ApplicationDocuments.Remove(AppDoc);
                    await db.SaveChangesAsync();

                    return Ok("Deleted");
                }

                return NotFound();

            }
            catch (Exception ex)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }
    }
}