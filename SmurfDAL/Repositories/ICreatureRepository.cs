using SmurfBL.Entities;

namespace SmurfDAL.Repositories
{
    public interface ICreatureRepository : IRepository<Creature>
    {
        IEnumerable<Creature> GetCreaturesByForest(int forestId);
        Task<List<Creature>> GetCreaturesByForestAsync(int forestId);
        IEnumerable<T> GetCreaturesByType<T>() where T : Creature;
        Task<List<T>> GetCreaturesByTypeAsync<T>() where T : Creature;
        IEnumerable<Creature> GetCreaturesAtPosition(int x, int y);
        Task<List<Creature>> GetCreaturesAtPositionAsync(int x, int y);
        Smurf? GetActiveSmurf(int forestId);
        Task<Smurf?> GetActiveSmurfAsync(int forestId);
    }
}