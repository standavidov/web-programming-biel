using System.Collections.Generic;
using System.Threading.Tasks;
using GT.Models;

namespace GT.Client.Data
{
    public interface IGroceryDataAccess
    {
        Task<Grocery> AddAsync(Grocery item);
        Task<bool> DeleteAsync(Grocery item);
        Task<IEnumerable<Grocery>> GetAsync(bool showAll);
        Task<Grocery> UpdateAsync(Grocery item);
    }
}
