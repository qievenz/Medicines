using Microsoft.EntityFrameworkCore;
using ReactTSWithNetCoreTemplate.Core.Entities;
using ReactTSWithNetCoreTemplate.Core.Repositories;
using ReactTSWithNetCoreTemplate.Infrastructure.Persistence;

namespace ReactTSWithNetCoreTemplate.Infrastructure.Repositories
{
    public class DataRepository : IDataRepository
    {
        private readonly ApplicationDbContext _context;

        public DataRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(Data data)
        {
            _context.Datas.Add(data);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var datas = await _context.Datas.FindAsync(id);
            if (datas != null)
            {
                _context.Datas.Remove(datas);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Data>> GetAll()
        {
            return await _context.Datas.ToListAsync();
        }

        public async Task<Data?> GetById(int id)
        {
            return await _context.Datas.FindAsync(id);
        }

        public async Task Update(Data data)
        {
            _context.Datas.Update(data);
            await _context.SaveChangesAsync();
        }
    }
}
