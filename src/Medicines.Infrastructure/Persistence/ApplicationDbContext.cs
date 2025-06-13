using Microsoft.EntityFrameworkCore;
using Medicines.Core.Entities;

namespace Medicines.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Data> Datas { get; set; }
    }
}
