using System.Collections.Generic;
using Rovale.OwinWebApi.Models;

namespace Rovale.OwinWebApi.Providers
{
    public class SomeObjectsProvider : ISomeObjectsProvider
    {
        public IEnumerable<SomeObject> GetAll()
        {
            return new [] { new SomeObject {SomeText = "Some text 1"}, new SomeObject { SomeText = "Some text 2" } };
        }
    }
}