using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TendaAdvisors.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.IO;
using System.Web;
using TendaAdvisors.Providers;
using System.Net.Http.Headers;
using TendaAdvisors.Models.Response;

namespace TendaAdvisors.Controllers
{
    //[Authorize]
    public class AdvisorDocumentsController : BaseApiController
    {
        //private ApplicationDbContext db;// = new ApplicationDbContext();
        private readonly string uploadsAdvFolder = HttpRuntime.AppDomainAppPath + @"Uploads\AdvisorDocuments";

        public AdvisorDocumentsController()
        {

            if (!Directory.Exists(this.uploadsAdvFolder))
            {
                try
                {
                    Directory.CreateDirectory(this.uploadsAdvFolder);
                }
                catch (Exception ex)
                {
                    //could not create folder.
                    throw ex;
                }
            }
        }
        private AdvisorDocumentsController(ApplicationDbContext dbcontext)
        {
           
            if (!Directory.Exists(this.uploadsAdvFolder))
            {
                try { 
                Directory.CreateDirectory(this.uploadsAdvFolder);
                } catch (Exception ex)
                {
                    //could not create folder.
                    throw ex;
                }
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

        // GET: api/AdvisorDocuments/  
        [Route("AdvisorDocuments/")]
        public async Task<IHttpActionResult> GetAdvisorDocument()
        {
            var documents = await db.AdvisorDocuments
                                   .Where(e => e.Deleted == false)
                                   .Select(x => new AdvisorDocumentsResponse()
                                   {
                                       DocumentId = x.Id,
                                       AdvisorId=x.Advisor_Id,
                                       DocumentTypeId = x.DocumentTypeId,
                                       DocumentTitle = x.Title,
                                       Uploaded = x.Uploaded,
                                       DocumentTypeName = x.DocumentType.Name,
                                       DocumenTypeLocation = x.Location,
                                       ValidFromDate = x.ValidFromDate.Value,
                                       ValidToDate = x.ValidToDate.Value,
                                       Expired = (x.ValidToDate.HasValue  && x.ValidToDate <= DateTime.Now)                                    
                                   }).ToListAsync();

            var advisorIds = documents.Select(x => x.AdvisorId).ToList();
            var advisors = await db.Advisors.Where(x => advisorIds.Contains(x.Id)).Select(x => new { AdvisorId = x.Id, x.Contact }).ToListAsync();

            foreach (var doc in documents)
            {
                doc.AdvisorName = advisors.FirstOrDefault(x => x.AdvisorId == doc.AdvisorId).Contact.FirstName;
                doc.AdvisorLastname = advisors.FirstOrDefault(x => x.AdvisorId == doc.AdvisorId).Contact.LastName;
                doc.AdvisorIdNumber = advisors.FirstOrDefault(x => x.AdvisorId == doc.AdvisorId).Contact.IdNumber;
            }
            return Ok(documents);
        }

        // GET: api/AdvisorDocuments/5
        [ResponseType(typeof(AdvisorDocument))]
        [Route("AdvisorDocuments/{id}")]
        public async Task<IHttpActionResult> GetAdvisorDocuments(int id)
        {
            AdvisorDocument advisorDocument = await db.AdvisorDocuments.FindAsync(id);
            if (advisorDocument == null)
            {
                return NotFound();
            }
            return Ok(advisorDocument);
        }

        // GET: api/AdvisorDocuments/5/File
        [Route("AdvisorDocuments/{id}/File")]
        public HttpResponseMessage GetAdvisorDocumentFile(int id)
        {
            if (id <= 0)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var appDoc = db.AdvisorDocuments.Find(id);

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
                 db.Entry(appDoc).Reference(a => a.Advisor).Load();
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
                         throw directoryException;
                     }
                     catch (Exception exception)
                     {
                         return new HttpResponseMessage(HttpStatusCode.NotFound);
                         throw exception;
                     }
             }
             else
             {
                 return new HttpResponseMessage(HttpStatusCode.Forbidden);
             }
        }


        [Route("AdvisorDocuments/{advisorDocId}/{status}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutchangeIsExpired(int advisorDocId, bool status)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                AdvisorDocument at = db.AdvisorDocuments.Find(advisorDocId);
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
                throw ex;

            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        // GET: api/Advisor/DocumentTypes/7
        //advisorTypeId is optional
        [Route("Advisor/DocumentTypes/{advisorId}")]
        public async Task<IHttpActionResult> GetAdvisorDocumentTypes(int advisorId)
        {
            try
            {
                var documentTypes = await db.Advisors.Where(x => x.Id == advisorId)
                                            .Select(x => x.AdvisorType)
                                            .Select(c => c.DocumentTypes.Select(p => new { p.Id, p.Name }))
                                            .FirstOrDefaultAsync();

                return Ok(documentTypes);
            }
            catch (Exception e)
            {
                return NotFound();
                throw e;
            }
        }

        // PUT: api/AdvisorDocuments/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAdvisorDocument(int id, AdvisorDocument advisorDocument)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != advisorDocument.Id)
            {
                return BadRequest();
            }

            db.Entry(advisorDocument).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdvisorDocumentExists(id))
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




        /*    // POST: api/ApplicationDocuments/5/Product/2
            [Route("ApplicationDocuments/{applicationId}/Product/{productId}")]
            [ResponseType(typeof(ApplicationDocument))]*/

        // POST: api/AdvisorDocuments/5/DocumentTypeId/1
        [Route("AdvisorDocuments/{advisorId}/DocumentTypeId/{docTypeId}/{submmited}")]
        [ResponseType(typeof(AdvisorDocument))]
        public async Task<IHttpActionResult> PostAdvisorDocument(int advisorId,int docTypeId,AdvisorDocument advisorDocument,bool submmited)
        {
            if (advisorId==0 || docTypeId == 0)
            {
                return BadRequest("Cannot locate Document.");
            }

            var advisor = await db.Advisors.FindAsync(advisorId);
            var docType = await db.DocumentTypes.FindAsync(docTypeId);


            if (advisor == null || docType == null)
            {
                return BadRequest("Cannot locate Document.");
            }

            advisorDocument.Advisor = advisor;
            advisorDocument.DocumentType = docType;
            advisorDocument.Uploaded = submmited;
            //applicationDocument.DocumentType = documentType;

            if (string.IsNullOrEmpty(advisorDocument.File))
            {
                return BadRequest("No file data received.");
            }

            try
            {

                string fileName = (
                    uploadsAdvFolder + "\\"
                    + advisorId + "\\"
                    + docTypeId + "\\AdvDocFile"
                    + (DateTime.Now.Ticks.ToString()).Replace(@"/", "-"));

                byte[] data = base64stringToByteArray(advisorDocument.File);

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

                advisorDocument.OriginalFileName = advisorDocument.Location;
                advisorDocument.Location = fileName;
                advisorDocument.CreatedDate = DateTime.Now;
                advisorDocument.ModifiedDate = advisorDocument.CreatedDate;
                advisorDocument.File = null;

                db.AdvisorDocuments.Add(advisorDocument);

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
                advisorDocument.File = null;
                return Ok(advisorDocument);

            }
            catch (Exception exception)
            {
                return BadRequest("{ data:{\"advisorDocumentId\":" + advisorDocument.Id + "}, \"statusText\" : \"Error: File could not be saved to disk.\", \"status\":-1}");
                throw exception;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AdvisorDocumentExists(int id)
        {
            return db.AdvisorDocuments.Count(e => e.Id == id) > 0;
        }

        // DELETE: api/AdvisorDocument/5
        [HttpDelete]
        [Route("AdvisorDocument/{id}")]
        [ResponseType(typeof(AdvisorDocument))]
        public async Task<IHttpActionResult> DeleteAdvisorDocument(int id)
        {
            AdvisorDocument advisorDocument = await db.AdvisorDocuments.FindAsync(id);
            if (advisorDocument == null)
            {
                return NotFound();
            }

            advisorDocument.Deleted = true;
            var origAppDoc = await db.AdvisorDocuments.FindAsync(advisorDocument.Id);
            db.Entry(origAppDoc).CurrentValues.SetValues(advisorDocument);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return Ok(advisorDocument);
        }
    }
}