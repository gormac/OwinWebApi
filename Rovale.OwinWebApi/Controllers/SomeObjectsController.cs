﻿using System.Collections.Generic;
using System.Web.Http;
using Rovale.OwinWebApi.Models;
using Rovale.OwinWebApi.Providers;

namespace Rovale.OwinWebApi.Controllers
{
    public class SomeObjectsController : ApiController
    {
        private readonly ISomeObjectsProvider _someObjectsProvider;

        public SomeObjectsController(ISomeObjectsProvider someObjectsProvider)
        {
            _someObjectsProvider = someObjectsProvider;
        }

        [Route("api/someObjects")]
        public IEnumerable<SomeObject> Get()
        {
            return _someObjectsProvider.GetAll();
        }

        [Route("api/someObjects/{id}")]
        public IHttpActionResult Get(int id)
        {
            var someObject = _someObjectsProvider.Find(id);

            if (someObject == null)
            {
                return NotFound();
            }

            return Ok(someObject);
        }
    }
}
