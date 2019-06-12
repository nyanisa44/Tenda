using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace TendaAdvisors.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/roles")]
    public class RolesController : BaseApiController
    {

        // GET: api/roles/5
        [Route("{id}")]
        public async Task<IHttpActionResult> GetRole(string Id)
        {
            var role = await AppRoleManager.FindByIdAsync(Id);

            if (role != null)
            {
                return Ok(role);
            }

            return NotFound();
        }

        [Route("")]
        public IHttpActionResult GetAllRoles()
        {
            var roles = AppRoleManager.Roles;

            return Ok(roles);
        }


        public async Task<IHttpActionResult> PostRole(string role)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var identityRole = new IdentityRole { Name = role };

            var result = await AppRoleManager.CreateAsync(identityRole);

            if (!result.Succeeded)
            {
                return BadRequest($"Creating role, {role}, was unsuccessful. Errors: { string.Join(", ", result.Errors.Select(e => e.ToString()).ToArray()) }");
            }

            Uri locationHeader = new Uri(Url.Link("GetRoleById", new { id = identityRole.Id }));

            return Created(locationHeader, role);

        }

        [Route("{id:guid}")]
        public async Task<IHttpActionResult> DeleteRole(string Id)
        {

            var role = await AppRoleManager.FindByIdAsync(Id);

            if (role != null)
            {
                IdentityResult result = await AppRoleManager.DeleteAsync(role);

                if (!result.Succeeded)
                {
                    return BadRequest($"Deleting role, {role}, was unsuccessful. Errors: { string.Join(", ", result.Errors.Select(e => e.ToString()).ToArray()) }");
                }

                return Ok();
            }

            return NotFound();
        }
    }
}
