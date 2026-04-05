using SmurfBL.Entities;

namespace SmurfBL.Interfaces
{
    public interface IGameEngine
    {
        Forest? CurrentForest { get; }
        Smurf? ActiveSmurf { get; }
        int GoldenSarsaparillasCollected { get; }
        int EnemiesDefeated { get; }
        bool IsGameOver { get; }
        bool IsVictory { get; }
        string LastActionMessage { get; }

        void InitializeGame();
        void LoadGame(Forest forest);
        bool TryMoveSmurf(int deltaX, int deltaY);
        List<Creature> GetCreaturesAt(int x, int y);
        List<Item> GetItemsAt(int x, int y);

        event EventHandler? GameStateChanged;
    }
}