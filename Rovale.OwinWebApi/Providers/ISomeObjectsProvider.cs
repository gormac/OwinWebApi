using System.Collections.Generic;
using Rovale.OwinWebApi.Models;

namespace Rovale.OwinWebApi.Providers
{
    public interface ISomeObjectsProvider
    {
        IEnumerable<SomeObject> GetAll();
    }
}