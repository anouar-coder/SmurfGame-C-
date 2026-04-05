using SmurfBL.Entities;

namespace SmurfDAL.Repositories
{
    public interface IItemRepository : IRepository<Item>
    {
        IEnumerable<Item> GetItemsByForest(int forestId);
        Task<List<Item>> GetItemsByForestAsync(int forestId);
        IEnumerable<T> GetItemsByType<T>() where T : Item;
        Task<List<T>> GetItemsByTypeAsync<T>() where T : Item;
        IEnumerable<Item> GetItemsAtPosition(int x, int y);
        Task<List<Item>> GetItemsAtPositionAsync(int x, int y);
        IEnumerable<Sarsaparilla> GetGoldenSarsaparillas(int forestId);
        Task<List<Sarsaparilla>> GetGoldenSarsaparillasAsync(int forestId);
    }
}