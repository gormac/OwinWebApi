using System.Collections.Generic;
using Rovale.OwinWebApi.Models;

namespace Rovale.OwinWebApi.Providers
{
    public interface ISomeObjectsProvider
    {
        IEnumerable<SomeObject> GetAll();
        SomeObject Find(int id);
        void Add(SomeObject someObject);
        void Delete(SomeObject someObject);
    }
}