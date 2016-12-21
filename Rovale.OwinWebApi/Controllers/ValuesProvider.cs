using System.Collections.Generic;

namespace Rovale.OwinWebApi.Controllers
{
    public class ValuesProvider : IValuesProvider
    {
        public IEnumerable<string> GetAll()
        {
            return new[] { "value1", "value2" };
        }
    }
}