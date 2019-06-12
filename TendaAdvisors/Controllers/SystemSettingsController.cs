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

namespace TendaAdvisors.Controllers
{
  
    public class SystemSettingsController : BaseApiController
    {
       // private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/SystemSettings
        public IQueryable<SystemSetting> GetSystemSettings()
        {
            return db.SystemSettings;
        }

        // GET: api/SystemSettings/5
        [ResponseType(typeof(SystemSetting))]
        public async Task<IHttpActionResult> GetSystemSetting(int id)
        {
            SystemSetting systemSetting = await db.SystemSettings.FindAsync(id);
            if (systemSetting == null)
            {
                return NotFound();
            }

            return Ok(systemSetting);
        }

        // PUT: api/SystemSettings/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSystemSetting(int id, SystemSetting systemSetting)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != systemSetting.SystemSettingId)
            {
                return BadRequest();
            }

            db.Entry(systemSetting).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SystemSettingExists(id))
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

        // POST: api/SystemSettings
        [ResponseType(typeof(SystemSetting))]
        public async Task<IHttpActionResult> PostSystemSetting(SystemSetting systemSetting)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SystemSettings.Add(systemSetting);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = systemSetting.SystemSettingId }, systemSetting);
        }

        // DELETE: api/SystemSettings/5
        [ResponseType(typeof(SystemSetting))]
        public async Task<IHttpActionResult> DeleteSystemSetting(int id)
        {
            SystemSetting systemSetting = await db.SystemSettings.FindAsync(id);
            if (systemSetting == null)
            {
                return NotFound();
            }

            db.SystemSettings.Remove(systemSetting);
            await db.SaveChangesAsync();

            return Ok(systemSetting);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SystemSettingExists(int id)
        {
            return db.SystemSettings.Count(e => e.SystemSettingId == id) > 0;
        }
    }
}