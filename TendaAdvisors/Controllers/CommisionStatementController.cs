using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TendaAdvisors.Controllers
{
    public class CommisionStatementController : BaseApiController
    {
        // GET: api/CommisionStatement
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/CommisionStatement/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/CommisionStatement
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/CommisionStatement/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/CommisionStatement/5
        public void Delete(int id)
        {
        }
    }
}
