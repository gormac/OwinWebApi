using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Rovale.OwinWebApi.Models;
using Rovale.OwinWebApi.Providers;

namespace Rovale.OwinWebApi.Controllers
{
    [Route("api/someObjects")]
    public class SomeObjectsController : ApiController
    {
        private readonly ISomeObjectsProvider _someObjectsProvider;

        public SomeObjectsController(ISomeObjectsProvider someObjectsProvider)
        {
            _someObjectsProvider = someObjectsProvider;
        }

        public IEnumerable<SomeObject> Get()
        {
            return _someObjectsProvider.GetAll();
        }

        [Route("api/someObjects/{id}", Name = "getById")]
        public IHttpActionResult Get(int id)
        {
            var someObject = _someObjectsProvider.Find(id);

            if (someObject == null)
            {
                return NotFound();
            }

            return Ok(someObject);
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] SomeObject someObject)
        {
            if (someObject == null)
            {
                return BadRequest("Invalid passed data");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_someObjectsProvider.Find(someObject.Id) != null)
            {
                return Conflict();
            }

            _someObjectsProvider.Add(someObject);

            return CreatedAtRoute("getById", new { id = someObject.Id }, someObject);
        }

        [HttpDelete]
        [Route("api/someObjects/{id}")]
        public IHttpActionResult Delete(int id)
        {
            var someObject = _someObjectsProvider.Find(id);

            if (someObject == null)
            {
                return NotFound();
            }

            _someObjectsProvider.Delete(someObject);

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
