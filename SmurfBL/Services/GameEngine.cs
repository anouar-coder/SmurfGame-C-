using SmurfBL.Entities;
using SmurfBL.Interfaces;

namespace SmurfBL.Services
{
    public class GameEngine : IGameEngine
    {
        public Forest? CurrentForest { get; private set; }
        public Smurf? ActiveSmurf { get; private set; }
        public int GoldenSarsaparillasCollected { get; private set; }
        public int EnemiesDefeated { get; private set; }
        public bool IsGameOver { get; private set; }
        public bool IsVictory { get; private set; }
        public string LastActionMessage { get; private set; } = string.Empty;

        public event EventHandler? GameStateChanged;

        private readonly Random _random = new();

        // ─────────────────────────────────────────────────────────────────
        public void InitializeGame()
        {
            // Id = 0 → EF auto-génère l'Id réel lors du SaveChanges()
            CurrentForest = new Forest
            {
                Id = 0,
                Name = "Forêt des Murmures",
                Creatures = new List<Creature>(),
                Items = new List<Item>()
            };

            // ForestId = 0 ici aussi : EF met à jour la FK automatiquement
            // via la propriété de navigation Forest.Creatures au moment du Save.
            ActiveSmurf = new Smurf
            {
                Name = "Schtroumpf Héros",
                Health = 100,
                MagicLevel = 1,
                X = 0,
                Y = 0,
                ForestId = CurrentForest.Id   // dynamique, pas hardcodé à 1
            };
            CurrentForest.Creatures.Add(ActiveSmurf);

            PlaceGoldenSarsaparillas();
            GenerateBugs(8);
            GenerateItems(6);

            GoldenSarsaparillasCollected = 0;
            EnemiesDefeated = 0;
            IsGameOver = false;
            IsVictory = false;
            LastActionMessage = "Bienvenue ! Trouve les 3 Salsepareilles Dorées !";

            OnGameStateChanged();
        }

        // ─────────────────────────────────────────────────────────────────
        public void LoadGame(Forest forest)
        {
            CurrentForest = forest;
            ActiveSmurf = forest.Creatures.OfType<Smurf>().FirstOrDefault();

            if (ActiveSmurf == null) return;

            GoldenSarsaparillasCollected = forest.Items
                .OfType<Sarsaparilla>()
                .Count(s => s.IsCollected);

            IsGameOver = false;
            IsVictory = false;
            LastActionMessage = "Partie chargée !";

            OnGameStateChanged();
        }

        // ─────────────────────────────────────────────────────────────────
        private void PlaceGoldenSarsaparillas()
        {
            // ForestId dynamique → EF résout la FK via la navigation property
            int fid = CurrentForest!.Id;

            var positions = new List<(int x, int y)>
            {
                (5, 5), (-5, -5), (3, -4)
            };

            foreach (var pos in positions)
            {
                CurrentForest.Items.Add(new Sarsaparilla
                {
                    X = pos.x,
                    Y = pos.y,
                    ForestId = fid
                });
            }
        }

        // ─────────────────────────────────────────────────────────────────
        private void GenerateBugs(int count)
        {
            int fid = CurrentForest!.Id;

            for (int i = 0; i < count; i++)
            {
                Bug bug;
                if (_random.Next(2) == 0)
                {
                    bug = new Spider
                    {
                        Name = $"Araignée {i + 1}",
                        Health = 40,
                        Damage = _random.Next(8, 15),
                        WebStrength = _random.Next(1, 5)
                    };
                }
                else
                {
                    bug = new BzzFly
                    {
                        Name = $"Bzz Fly {i + 1}",
                        Health = 25,
                        Damage = _random.Next(5, 10),
                        Speed = _random.Next(1, 4)
                    };
                }

                do
                {
                    bug.X = _random.Next(-10, 11);
                    bug.Y = _random.Next(-10, 11);
                } while (bug.X == 0 && bug.Y == 0);

                bug.ForestId = fid;      // ← dynamique
                CurrentForest.Creatures.Add(bug);
            }
        }

        // ─────────────────────────────────────────────────────────────────
        private void GenerateItems(int count)
        {
            int fid = CurrentForest!.Id;

            for (int i = 0; i < count; i++)
            {
                Item item = _random.Next(3) switch
                {
                    0 => new Berry { Name = "Baie Sauvage", HealthBoost = 15 },
                    1 => new RedPotion { Name = "Potion Rouge", BoostAmount = 30 },
                    _ => new BluePotion { Name = "Potion Bleue", GlobalHealAmount = 10 }
                };

                item.X = _random.Next(-10, 11);
                item.Y = _random.Next(-10, 11);
                item.ForestId = fid;     // ← dynamique
                CurrentForest.Items.Add(item);
            }
        }

        // ─────────────────────────────────────────────────────────────────
        public bool TryMoveSmurf(int deltaX, int deltaY)
        {
            if (IsGameOver || IsVictory || ActiveSmurf == null)
                return false;

            if (ActiveSmurf.Health <= 0)
            {
                IsGameOver = true;
                LastActionMessage = "GAME OVER - Le Schtroumpf est mort !";
                OnGameStateChanged();
                return false;
            }

            ActiveSmurf.X += deltaX;
            ActiveSmurf.Y += deltaY;

            LastActionMessage = $"Schtroumpf se déplace vers ({ActiveSmurf.X}, {ActiveSmurf.Y})";

            MoveAllBugs();
            ProcessCollisions();
            CheckGameState();

            OnGameStateChanged();
            return true;
        }

        // ─────────────────────────────────────────────────────────────────
        private void MoveAllBugs()
        {
            if (CurrentForest == null) return;

            foreach (var creature in CurrentForest.Creatures.ToList())
            {
                if (creature is Bug bug && bug.Health > 0)
                {
                    bug.X += _random.Next(-1, 2);
                    bug.Y += _random.Next(-1, 2);
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────
        private void ProcessCollisions()
        {
            if (CurrentForest == null || ActiveSmurf == null) return;

            foreach (var item in GetItemsAt(ActiveSmurf.X, ActiveSmurf.Y).ToList())
            {
                ApplyItemEffect(item);
                CurrentForest.Items.Remove(item);
            }

            var bugsHere = GetCreaturesAt(ActiveSmurf.X, ActiveSmurf.Y)
                              .OfType<Bug>()
                              .Where(b => b.Health > 0)
                              .ToList();

            foreach (var bug in bugsHere)
            {
                Fight(ActiveSmurf, bug);

                if (bug.Health <= 0)
                {
                    EnemiesDefeated++;
                    CurrentForest.Creatures.Remove(bug);
                    LastActionMessage += $"\n{bug.Name} vaincu !";
                }

                if (ActiveSmurf.Health <= 0)
                {
                    IsGameOver = true;
                    LastActionMessage += "\nLe Schtroumpf est mort...";
                    break;
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────
        private void ApplyItemEffect(Item item)
        {
            if (ActiveSmurf == null) return;

            switch (item)
            {
                case Berry berry:
                    ActiveSmurf.Health += berry.HealthBoost;
                    LastActionMessage += $"\nBaie mangée ! +{berry.HealthBoost} PV";
                    break;

                case RedPotion red:
                    // La Potion Rouge n'affecte que les Schtroumpfs (énoncé du sujet)
                    ActiveSmurf.Health += red.BoostAmount;
                    LastActionMessage += $"\nPotion rouge bue ! +{red.BoostAmount} PV";
                    break;

                case BluePotion blue:
                    if (CurrentForest != null)
                        foreach (var c in CurrentForest.Creatures)
                            c.Health += blue.GlobalHealAmount;
                    LastActionMessage += $"\nPotion bleue ! Tout le monde +{blue.GlobalHealAmount} PV";
                    break;

                case Sarsaparilla sarsa when !sarsa.IsCollected:
                    sarsa.IsCollected = true;
                    GoldenSarsaparillasCollected++;
                    ActiveSmurf.Health += 50;
                    LastActionMessage += $"\nSALSEPAREILLE DORÉE #{GoldenSarsaparillasCollected}/3 ! +50 PV";
                    break;
            }

            if (ActiveSmurf.Health > 500)
                ActiveSmurf.Health = 500;
        }

        // ─────────────────────────────────────────────────────────────────
        private void Fight(Smurf smurf, Bug bug)
        {
            LastActionMessage += $"\nCOMBAT : {smurf.Name} vs {bug.Name} !";

            smurf.Health -= bug.Damage;
            LastActionMessage += $"\n   {bug.Name} inflige {bug.Damage} dégâts !";

            if (smurf.Health <= 0)
            {
                LastActionMessage += $"\n   {smurf.Name} n'a plus de vie !";
                return;
            }

            int smurfDmg = 10 + smurf.MagicLevel * 5;
            bug.Health -= smurfDmg;
            LastActionMessage += $"\n   {smurf.Name} contre-attaque : {smurfDmg} dégâts !";
            LastActionMessage += bug.Health <= 0
                ? $"\n   {bug.Name} est vaincu !"
                : $"\n   {bug.Name} : {bug.Health} PV restants";

            LastActionMessage += $"\n   {smurf.Name} : {smurf.Health} PV";
        }

        // ─────────────────────────────────────────────────────────────────
        private void CheckGameState()
        {
            if (ActiveSmurf == null) return;

            if (ActiveSmurf.Health <= 0)
            {
                IsGameOver = true;
                LastActionMessage = "GAME OVER - Le Schtroumpf est mort !";
                return;
            }

            if (GoldenSarsaparillasCollected >= 3 && ActiveSmurf.X == 0 && ActiveSmurf.Y == 0)
            {
                IsVictory = true;
                LastActionMessage = "VICTOIRE ! Tu as sauvé le village avec les 3 Salsepareilles Dorées !";
            }
            else if (GoldenSarsaparillasCollected >= 3)
            {
                LastActionMessage += "\n⭐ Tu as les 3 Salsepareilles ! Retourne au village (0,0) !";
            }
        }

        // ─────────────────────────────────────────────────────────────────
        public List<Creature> GetCreaturesAt(int x, int y)
        {
            if (CurrentForest == null) return new List<Creature>();
            return CurrentForest.Creatures.Where(c => c.X == x && c.Y == y).ToList();
        }

        public List<Item> GetItemsAt(int x, int y)
        {
            if (CurrentForest == null) return new List<Item>();
            return CurrentForest.Items.Where(i => i.X == x && i.Y == y).ToList();
        }

        private void OnGameStateChanged() =>
            GameStateChanged?.Invoke(this, EventArgs.Empty);
    }
}