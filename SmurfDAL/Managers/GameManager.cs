using Microsoft.EntityFrameworkCore;
using SmurfBL.Entities;
using SmurfDAL.Repositories;

namespace SmurfDAL.Managers
{
    public class GameManager : IGameManager
    {
        private readonly SmurfDbContext _context;
        private readonly IForestRepository _forestRepository;
        private readonly ICreatureRepository _creatureRepository;
        private readonly IItemRepository _itemRepository;

        public GameManager(SmurfDbContext context)
        {
            _context = context;
            _forestRepository = new ForestRepository(context);
            _creatureRepository = new CreatureRepository(context);
            _itemRepository = new ItemRepository(context);
        }

        private void DetachAllTrackedEntities()
        {
            var tracked = _context.ChangeTracker.Entries().ToList();
            foreach (var entry in tracked)
            {
                entry.State = EntityState.Detached;
            }
        }

        public void SaveGame(Forest forest)
        {
            try
            {
                // Vérifier si la forêt existe déjà
                var existingForest = _context.Forests
                    .AsNoTracking()
                    .FirstOrDefault(f => f.Id == forest.Id);

                if (existingForest == null)
                {
                    // Nouvelle forêt : ajout simple
                    _context.Forests.Add(forest);
                }
                else
                {
                    // Mise à jour : attacher et modifier
                    _context.Entry(forest).State = EntityState.Modified;

                    foreach (var creature in forest.Creatures)
                    {
                        creature.ForestId = forest.Id;
                        var existingCreature = _context.Creatures
                            .AsNoTracking()
                            .FirstOrDefault(c => c.Id == creature.Id);

                        if (existingCreature == null)
                            _context.Entry(creature).State = EntityState.Added;
                        else
                            _context.Entry(creature).State = EntityState.Modified;
                    }

                    foreach (var item in forest.Items)
                    {
                        item.ForestId = forest.Id;
                        var existingItem = _context.Items
                            .AsNoTracking()
                            .FirstOrDefault(i => i.Id == item.Id);

                        if (existingItem == null)
                            _context.Entry(item).State = EntityState.Added;
                        else
                            _context.Entry(item).State = EntityState.Modified;
                    }
                }

                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Erreur base de données : {ex.InnerException?.Message ?? ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la sauvegarde : {ex.Message}", ex);
            }
        }

        public async Task SaveGameAsync(Forest forest)
        {
            try
            {
                var existingForest = await _context.Forests
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Id == forest.Id);

                if (existingForest == null)
                {
                    await _context.Forests.AddAsync(forest);
                }
                else
                {
                    _context.Entry(forest).State = EntityState.Modified;

                    foreach (var creature in forest.Creatures)
                    {
                        creature.ForestId = forest.Id;
                        var existingCreature = await _context.Creatures
                            .AsNoTracking()
                            .FirstOrDefaultAsync(c => c.Id == creature.Id);

                        if (existingCreature == null)
                            _context.Entry(creature).State = EntityState.Added;
                        else
                            _context.Entry(creature).State = EntityState.Modified;
                    }

                    foreach (var item in forest.Items)
                    {
                        item.ForestId = forest.Id;
                        var existingItem = await _context.Items
                            .AsNoTracking()
                            .FirstOrDefaultAsync(i => i.Id == item.Id);

                        if (existingItem == null)
                            _context.Entry(item).State = EntityState.Added;
                        else
                            _context.Entry(item).State = EntityState.Modified;
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Erreur base de données (async) : {ex.InnerException?.Message ?? ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la sauvegarde (async) : {ex.Message}", ex);
            }
        }

        public Forest? LoadGame(int forestId)
        {
            try
            {
                return _context.Forests
                    .Include(f => f.Creatures)
                    .Include(f => f.Items)
                    .AsNoTracking()
                    .FirstOrDefault(f => f.Id == forestId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du chargement : {ex.Message}", ex);
            }
        }

        public async Task<Forest?> LoadGameAsync(int forestId)
        {
            try
            {
                return await _context.Forests
                    .Include(f => f.Creatures)
                    .Include(f => f.Items)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Id == forestId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du chargement (async) : {ex.Message}", ex);
            }
        }

        public Forest? LoadLastGame()
        {
            try
            {
                var lastForest = _context.Forests
                    .OrderByDescending(f => f.Id)
                    .AsNoTracking()
                    .FirstOrDefault();

                return lastForest != null ? LoadGame(lastForest.Id) : null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du chargement de la dernière partie : {ex.Message}", ex);
            }
        }

        public async Task<Forest?> LoadLastGameAsync()
        {
            try
            {
                var lastForest = await _context.Forests
                    .OrderByDescending(f => f.Id)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                return lastForest != null ? await LoadGameAsync(lastForest.Id) : null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du chargement de la dernière partie (async) : {ex.Message}", ex);
            }
        }

        public void DeleteGame(int forestId)
        {
            try
            {
                var forest = _context.Forests.Find(forestId);
                if (forest != null)
                {
                    _context.Forests.Remove(forest);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la suppression : {ex.Message}", ex);
            }
        }

        public async Task DeleteGameAsync(int forestId)
        {
            try
            {
                var forest = await _context.Forests.FindAsync(forestId);
                if (forest != null)
                {
                    _context.Forests.Remove(forest);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la suppression (async) : {ex.Message}", ex);
            }
        }

        public bool HasSavedGame()
        {
            try
            {
                return _context.Forests.Any();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la vérification des sauvegardes : {ex.Message}", ex);
            }
        }

        public async Task<bool> HasSavedGameAsync()
        {
            try
            {
                return await _context.Forests.AnyAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la vérification des sauvegardes (async) : {ex.Message}", ex);
            }
        }

        public IEnumerable<Forest> GetAllForests()
        {
            try
            {
                return _context.Forests
                    .AsNoTracking()
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des forêts : {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Forest>> GetAllForestsAsync()
        {
            try
            {
                return await _context.Forests
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des forêts (async) : {ex.Message}", ex);
            }
        }

        public GameStatistics GetGameStatistics(int forestId)
        {
            try
            {
                var forest = LoadGame(forestId);
                if (forest == null) return new GameStatistics();

                var smurf = forest.Creatures.OfType<Smurf>().FirstOrDefault();
                var goldenCount = forest.Items.OfType<Sarsaparilla>().Count(s => s.IsCollected);

                return new GameStatistics
                {
                    ForestId = forest.Id,
                    ForestName = forest.Name,
                    TotalCreatures = forest.Creatures.Count,
                    TotalItems = forest.Items.Count,
                    SmurfHealth = smurf?.Health ?? 0,
                    GoldenSarsaparillasFound = goldenCount,
                    LastPlayed = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'obtention des statistiques : {ex.Message}", ex);
            }
        }
    }
}