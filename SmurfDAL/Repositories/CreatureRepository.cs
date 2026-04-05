using Microsoft.EntityFrameworkCore;
using SmurfBL.Entities;

namespace SmurfDAL.Repositories
{
    public class CreatureRepository : Repository<Creature>, ICreatureRepository
    {
        public CreatureRepository(SmurfDbContext context) : base(context)
        {
        }

        public IEnumerable<Creature> GetCreaturesByForest(int forestId)
        {
            return _dbSet.Where(c => c.ForestId == forestId).ToList();
        }

        public async Task<List<Creature>> GetCreaturesByForestAsync(int forestId)
        {
            return await _dbSet.Where(c => c.ForestId == forestId).ToListAsync();
        }

        public IEnumerable<T> GetCreaturesByType<T>() where T : Creature
        {
            return _dbSet.OfType<T>().ToList();
        }

        public async Task<List<T>> GetCreaturesByTypeAsync<T>() where T : Creature
        {
            return await _dbSet.OfType<T>().ToListAsync();
        }

        public IEnumerable<Creature> GetCreaturesAtPosition(int x, int y)
        {
            return _dbSet.Where(c => c.X == x && c.Y == y).ToList();
        }

        public async Task<List<Creature>> GetCreaturesAtPositionAsync(int x, int y)
        {
            return await _dbSet.Where(c => c.X == x && c.Y == y).ToListAsync();
        }

        public Smurf? GetActiveSmurf(int forestId)
        {
            return _dbSet.OfType<Smurf>().FirstOrDefault(s => s.ForestId == forestId);
        }

        public async Task<Smurf?> GetActiveSmurfAsync(int forestId)
        {
            return await _dbSet.OfType<Smurf>().FirstOrDefaultAsync(s => s.ForestId == forestId);
        }
    }
}