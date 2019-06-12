using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace TendaAdvisors.Controllers
{
    [Authorize]
    [RoutePrefix("values")]
    public class ValuesController : BaseApiController
    {
        static int id = 1;
        static ConcurrentDictionary<int, string> _store = new ConcurrentDictionary<int, string>();

        public IEnumerable<ValueModel> Get()
        {
            return _store.Select(x => new ValueModel { Id = x.Key, Value = x.Value });
        }

        public ValueModel Get(int id)
        {
            string value;
            if (!_store.TryGetValue(id, out value)) throw new HttpException(404, "Not Found");
            return new ValueModel { Id = id, Value = value };
        }

        public ValueModel Post(ValueModel request)
        {
            var key = Interlocked.Increment(ref id);
            _store[key] = request.Value;

            return new ValueModel
            {
                Id = key,
                Value = request.Value
            };
        }

        public void Put(int id, ValueModel request)
        {
            string old;
            if (!_store.TryGetValue(id, out old)) throw new HttpException(404, "Not Found");
            if (!_store.TryUpdate(id, request.Value, old)) throw new HttpException(500, "Concurrency failure");
        }

        public void Delete(int id)
        {
            string old;
            if (!_store.TryRemove(id, out old)) throw new HttpException(404, "Not Found");
        }

        //this shows how to make an action with a specific route
        [Route("clear")]
        [HttpPost]
        public void Clear()
        {
            _store.Clear();
        }

        public class ValueModel
        {
            public int Id { get; set; }
            public string Value { get; set; }
        }
    }
}