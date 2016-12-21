using System.Collections.Generic;
using Rovale.OwinWebApi.Controllers;

namespace Rovale.OwinWebApi.Test
{
    public class TestValuesProvider : IValuesProvider
    {
        public IEnumerable<string> GetAll()
        {
            return new[] { "test value1", "test value2" };
        }
    }
}