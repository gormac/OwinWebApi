using System.Collections.Generic;
using System.Web.Http;

namespace Rovale.OwinWebApi.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly IValuesProvider _valuesProvider;

        public ValuesController(IValuesProvider valuesProvider)
        {
            _valuesProvider = valuesProvider;
        }

        // GET: api/values
        [Route("api/values")]
        public IEnumerable<string> Get()
        {
            return _valuesProvider.GetAll();
        }
    }
}
