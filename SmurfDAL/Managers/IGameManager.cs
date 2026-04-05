using SmurfBL.Entities;

namespace SmurfDAL.Managers
{
    public interface IGameManager
    {
        // Synchrones
        void SaveGame(Forest forest);
        Forest? LoadGame(int forestId);
        Forest? LoadLastGame();
        void DeleteGame(int forestId);
        bool HasSavedGame();
        GameStatistics GetGameStatistics(int forestId);
        IEnumerable<Forest> GetAllForests();

        // Asynchrones
        Task SaveGameAsync(Forest forest);
        Task<Forest?> LoadGameAsync(int forestId);
        Task<Forest?> LoadLastGameAsync();
        Task DeleteGameAsync(int forestId);
        Task<bool> HasSavedGameAsync();
        Task<IEnumerable<Forest>> GetAllForestsAsync();
    }

    public class GameStatistics
    {
        public int ForestId { get; set; }
        public string ForestName { get; set; } = string.Empty;
        public int TotalCreatures { get; set; }
        public int TotalItems { get; set; }
        public int SmurfHealth { get; set; }
        public int GoldenSarsaparillasFound { get; set; }
        public DateTime LastPlayed { get; set; }
    }
}