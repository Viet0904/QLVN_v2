using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Service.Interfaces
{
    public interface IDvsdService
    {
        Task<IEnumerable<object>> GetAllAsync();
        Task CreateAsync(object request);
    }
}
