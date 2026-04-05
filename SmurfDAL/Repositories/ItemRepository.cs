using Microsoft.EntityFrameworkCore;
using SmurfBL.Entities;

namespace SmurfDAL.Repositories
{
    public class ItemRepository : Repository<Item>, IItemRepository
    {
        public ItemRepository(SmurfDbContext context) : base(context)
        {
        }

        public IEnumerable<Item> GetItemsByForest(int forestId)
        {
            return _dbSet.Where(i => i.ForestId == forestId).ToList();
        }

        public async Task<List<Item>> GetItemsByForestAsync(int forestId)
        {
            return await _dbSet.Where(i => i.ForestId == forestId).ToListAsync();
        }

        public IEnumerable<T> GetItemsByType<T>() where T : Item
        {
            return _dbSet.OfType<T>().ToList();
        }

        public async Task<List<T>> GetItemsByTypeAsync<T>() where T : Item
        {
            return await _dbSet.OfType<T>().ToListAsync();
        }

        public IEnumerable<Item> GetItemsAtPosition(int x, int y)
        {
            return _dbSet.Where(i => i.X == x && i.Y == y).ToList();
        }

        public async Task<List<Item>> GetItemsAtPositionAsync(int x, int y)
        {
            return await _dbSet.Where(i => i.X == x && i.Y == y).ToListAsync();
        }

        public IEnumerable<Sarsaparilla> GetGoldenSarsaparillas(int forestId)
        {
            return _dbSet.OfType<Sarsaparilla>()
                .Where(s => s.ForestId == forestId && s.IsGolden)
                .ToList();
        }

        public async Task<List<Sarsaparilla>> GetGoldenSarsaparillasAsync(int forestId)
        {
            return await _dbSet.OfType<Sarsaparilla>()
                .Where(s => s.ForestId == forestId && s.IsGolden)
                .ToListAsync();
        }
    }
}