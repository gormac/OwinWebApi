using System.Collections.Generic;
using System.Linq;
using Rovale.OwinWebApi.Models;

namespace Rovale.OwinWebApi.Providers
{
    public class SomeObjectsProvider : ISomeObjectsProvider
    {
        private readonly List<SomeObject> _someObjects = new List<SomeObject>();

        public IEnumerable<SomeObject> GetAll()
        {
            return _someObjects;
        }

        public SomeObject Find(int id)
        {
            return _someObjects.SingleOrDefault(o => o.Id == id);
        }

        public void Add(SomeObject someObject)
        {
            _someObjects.Add(someObject);
        }

        public void Delete(SomeObject someObject)
        {
            _someObjects.Remove(someObject);
        }
    }
}