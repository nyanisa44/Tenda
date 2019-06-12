using Microsoft.AspNet.Identity.Owin;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TendaAdvisors.Models;
using TendaAdvisors.Providers;

namespace TendaAdvisors.Controllers
{
    public class BaseApiController : ApiController
    {
        //Code removed from brevity
        private ApplicationRoleManager _AppRoleManager = null;
        [Dependency]
        public ApplicationDbContext db { get; set; }
       

        protected ApplicationRoleManager AppRoleManager
        {
            get
            {
                return _AppRoleManager ?? Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
        }
    }
}
