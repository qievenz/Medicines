using Medicines.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicines.Core.Repositories
{
    public interface IIngestionProcessRepository
    {
        Task<IngestionProcess> GetById(Guid id);
        Task Add(IngestionProcess process);
        Task Update(IngestionProcess process);
    }
}
