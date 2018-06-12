using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExampleFunctions.Interfaces
{
    public interface IDataService<T>
    {
        Task<IEnumerable<T>> FetchAllAsync();
    }
}
