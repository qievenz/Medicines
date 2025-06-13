using Microsoft.EntityFrameworkCore;
using ReactTSWithNetCoreTemplate.Core.Entities;

namespace ReactTSWithNetCoreTemplate.Infrastructure.Persistence
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
