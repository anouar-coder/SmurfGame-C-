using Microsoft.EntityFrameworkCore;
using SmurfBL.Entities;

namespace SmurfDAL.Repositories
{
    public class ForestRepository : Repository<Forest>, IForestRepository
    {
        public ForestRepository(SmurfDbContext context) : base(context)
        {
        }

        public Forest? GetForestWithAllData(int id)
        {
            return _context.Forests
                .Include(f => f.Creatures)
                .Include(f => f.Items)
                .FirstOrDefault(f => f.Id == id);
        }

        public async Task<Forest?> GetForestWithAllDataAsync(int id)
        {
            return await _context.Forests
                .Include(f => f.Creatures)
                .Include(f => f.Items)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public Forest? GetForestWithCreatures(int id)
        {
            return _context.Forests
                .Include(f => f.Creatures)
                .FirstOrDefault(f => f.Id == id);
        }

        public async Task<Forest?> GetForestWithCreaturesAsync(int id)
        {
            return await _context.Forests
                .Include(f => f.Creatures)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public Forest? GetForestWithItems(int id)
        {
            return _context.Forests
                .Include(f => f.Items)
                .FirstOrDefault(f => f.Id == id);
        }

        public async Task<Forest?> GetForestWithItemsAsync(int id)
        {
            return await _context.Forests
                .Include(f => f.Items)
                .FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}