using System.Collections.Generic;

namespace Rovale.OwinWebApi.Controllers
{
    public interface IValuesProvider
    {
        IEnumerable<string> GetAll();
    }
}