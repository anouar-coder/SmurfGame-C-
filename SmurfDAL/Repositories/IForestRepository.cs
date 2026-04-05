using SmurfBL.Entities;

namespace SmurfDAL.Repositories
{
    public interface IForestRepository : IRepository<Forest>
    {
        Forest? GetForestWithAllData(int id);
        Task<Forest?> GetForestWithAllDataAsync(int id);
        Forest? GetForestWithCreatures(int id);
        Task<Forest?> GetForestWithCreaturesAsync(int id);
        Forest? GetForestWithItems(int id);
        Task<Forest?> GetForestWithItemsAsync(int id);
    }
}