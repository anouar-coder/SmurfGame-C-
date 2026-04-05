using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using SmurfBL.Entities;
using SmurfBL.Interfaces;
using SmurfDAL.Managers;

namespace SmurfUI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IGameEngine _gameEngine;
        private readonly IGameManager _gameManager;

        private int _smurfHealth;
        private int _goldenSarsaparillasCollected;
        private int _enemiesDefeated;
        private string _smurfPosition = "(0, 0)";
        private string _lastActionMessage = string.Empty;
        private Visibility _gameOverlayVisibility = Visibility.Collapsed;
        private string _gameOverlayMessage = string.Empty;
        private List<GridCell> _gridCells = new();

        public MainViewModel(IGameEngine gameEngine, IGameManager gameManager)
        {
            _gameEngine = gameEngine;
            _gameManager = gameManager;

            _gameEngine.GameStateChanged += OnGameStateChanged;

            MoveUpCommand = new RelayCommand(_ => TryMove(0, -1));
            MoveDownCommand = new RelayCommand(_ => TryMove(0, 1));
            MoveLeftCommand = new RelayCommand(_ => TryMove(-1, 0));
            MoveRightCommand = new RelayCommand(_ => TryMove(1, 0));
            NewGameCommand = new RelayCommand(_ => NewGame());
            SaveGameCommand = new RelayCommand(_ => SaveGame());
            LoadGameCommand = new RelayCommand(_ => LoadGame(), _ => _gameManager.HasSavedGame());

            LoadLastSavedGame();
        }

        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }
        public ICommand MoveLeftCommand { get; }
        public ICommand MoveRightCommand { get; }
        public ICommand NewGameCommand { get; }
        public ICommand SaveGameCommand { get; }
        public ICommand LoadGameCommand { get; }

        public int SmurfHealth
        {
            get => _smurfHealth;
            set { _smurfHealth = value; OnPropertyChanged(); }
        }

        public int GoldenSarsaparillasCollected
        {
            get => _goldenSarsaparillasCollected;
            set { _goldenSarsaparillasCollected = value; OnPropertyChanged(); }
        }

        public int EnemiesDefeated
        {
            get => _enemiesDefeated;
            set { _enemiesDefeated = value; OnPropertyChanged(); }
        }

        public string SmurfPosition
        {
            get => _smurfPosition;
            set { _smurfPosition = value; OnPropertyChanged(); }
        }

        public string LastActionMessage
        {
            get => _lastActionMessage;
            set { _lastActionMessage = value; OnPropertyChanged(); }
        }

        public Visibility GameOverlayVisibility
        {
            get => _gameOverlayVisibility;
            set { _gameOverlayVisibility = value; OnPropertyChanged(); }
        }

        public string GameOverlayMessage
        {
            get => _gameOverlayMessage;
            set { _gameOverlayMessage = value; OnPropertyChanged(); }
        }

        public List<GridCell> GridCells
        {
            get => _gridCells;
            set { _gridCells = value; OnPropertyChanged(); }
        }

        private void LoadLastSavedGame()
        {
            try
            {
                if (_gameManager.HasSavedGame())
                {
                    var result = MessageBox.Show(
                        "Une partie sauvegardée existe. Voulez-vous la charger ?",
                        "Chargement automatique",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        var forest = _gameManager.LoadLastGame();
                        if (forest != null)
                        {
                            _gameEngine.LoadGame(forest);
                            UpdateUI();
                            LastActionMessage = $"Bienvenue ! Partie chargée : {forest.Name}";
                            GameOverlayVisibility = Visibility.Collapsed;
                            return;
                        }
                    }
                }

                NewGame();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement automatique :\n{ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                NewGame();
            }
        }

        private void NewGame()
        {
            _gameEngine.InitializeGame();
            UpdateUI();
            GameOverlayVisibility = Visibility.Collapsed;
            LastActionMessage = "Nouvelle partie ! Bonne chance Schtroumpf !";
        }

        private void SaveGame()
        {
            if (_gameEngine.CurrentForest == null)
            {
                MessageBox.Show("Aucune partie en cours à sauvegarder.",
                                "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                LastActionMessage = "Sauvegarde en cours...";
                OnPropertyChanged(nameof(LastActionMessage));

                if (string.IsNullOrEmpty(_gameEngine.CurrentForest.Name))
                {
                    _gameEngine.CurrentForest.Name = $"Partie du {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                }

                _gameManager.SaveGame(_gameEngine.CurrentForest);

                LastActionMessage = _gameEngine.LastActionMessage + "\n💾 Partie sauvegardée !";
                OnPropertyChanged(nameof(LastActionMessage));

                MessageBox.Show("Partie sauvegardée avec succès !",
                                "Sauvegarde", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                string errorMessage = $"❌ ERREUR LORS DE LA SAUVEGARDE ❌\n\n" +
                                      $"Message : {ex.Message}\n\n" +
                                      $"Type d'erreur : {ex.GetType().Name}\n\n";

                if (ex.InnerException != null)
                {
                    errorMessage += $"Détail interne : {ex.InnerException.Message}\n\n";
                }

                errorMessage += $"Stack Trace :\n{ex.StackTrace}";

                MessageBox.Show(errorMessage, "Erreur de sauvegarde",
                                MessageBoxButton.OK, MessageBoxImage.Error);

                LastActionMessage = _gameEngine.LastActionMessage + "\n❌ Échec de la sauvegarde !";
                OnPropertyChanged(nameof(LastActionMessage));
            }
        }

        private void LoadGame()
        {
            try
            {
                var forest = _gameManager.LoadLastGame();

                if (forest == null)
                {
                    MessageBox.Show("Aucune partie sauvegardée trouvée.",
                                    "Charger", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                _gameEngine.LoadGame(forest);
                UpdateUI();
                GameOverlayVisibility = Visibility.Collapsed;
                LastActionMessage = $"Partie chargée : {forest.Name}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement :\n{ex.Message}",
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TryMove(int deltaX, int deltaY)
        {
            if (_gameEngine.IsGameOver || _gameEngine.IsVictory)
                return;

            _gameEngine.TryMoveSmurf(deltaX, deltaY);
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_gameEngine.ActiveSmurf != null)
                SmurfHealth = _gameEngine.ActiveSmurf.Health;

            GoldenSarsaparillasCollected = _gameEngine.GoldenSarsaparillasCollected;
            EnemiesDefeated = _gameEngine.EnemiesDefeated;

            var smurf = _gameEngine.ActiveSmurf;
            if (smurf != null)
                SmurfPosition = $"({smurf.X}, {smurf.Y})";

            LastActionMessage = _gameEngine.LastActionMessage;

            UpdateGrid();

            if (_gameEngine.IsGameOver)
            {
                GameOverlayMessage =
                    "💀 GAME OVER 💀\n\n" +
                    "Le Schtroumpf n'a pas survécu...\n\n" +
                    "Cliquez sur NOUVELLE PARTIE pour rejouer";
                GameOverlayVisibility = Visibility.Visible;
            }
            else if (_gameEngine.IsVictory)
            {
                GameOverlayMessage =
                    "🎉 VICTOIRE ! 🎉\n\n" +
                    "Tu as sauvé le village avec les 3 Salsepareilles Dorées !\n\n" +
                    $"⭐ Salsepareilles : 3/3\n" +
                    $"🗡️ Ennemis vaincus : {_gameEngine.EnemiesDefeated}\n" +
                    $"❤️ Santé finale : {_gameEngine.ActiveSmurf?.Health}/500\n\n" +
                    "Cliquez sur NOUVELLE PARTIE pour rejouer";
                GameOverlayVisibility = Visibility.Visible;

                SaveGame();
            }
        }

        private void UpdateGrid()
        {
            var cells = new List<GridCell>();

            for (int y = -10; y <= 10; y++)
            {
                for (int x = -10; x <= 10; x++)
                {
                    var cell = new GridCell { X = x, Y = y, ImageName = "forest" };

                    if (_gameEngine.ActiveSmurf?.X == x && _gameEngine.ActiveSmurf?.Y == y)
                    {
                        cell.ImageName = "smurf";
                    }
                    else
                    {
                        var creatures = _gameEngine.GetCreaturesAt(x, y);
                        foreach (var creature in creatures)
                        {
                            if (creature is Spider)
                                cell.ImageName = "spider";
                            else if (creature is BzzFly)
                                cell.ImageName = "bzzfly";
                        }
                    }

                    if (cell.ImageName == "forest")
                    {
                        var items = _gameEngine.GetItemsAt(x, y);
                        foreach (var item in items)
                        {
                            if (item is Berry)
                                cell.ImageName = "berry";
                            else if (item is RedPotion)
                                cell.ImageName = "redpotion";
                            else if (item is BluePotion)
                                cell.ImageName = "bluepotion";
                            else if (item is Sarsaparilla sarsa && !sarsa.IsCollected)
                                cell.ImageName = "sarsaparilla";
                        }
                    }

                    cells.Add(cell);
                }
            }

            GridCells = cells;
        }

        private void OnGameStateChanged(object? sender, EventArgs e)
        {
            UpdateUI();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class GridCell : INotifyPropertyChanged
    {
        private string _imageName = "forest";

        public int X { get; set; }
        public int Y { get; set; }

        public string ImageName
        {
            get => _imageName;
            set
            {
                _imageName = value;
                OnPropertyChanged();
            }
        }

        public string Symbol
        {
            get
            {
                return ImageName switch
                {
                    "smurf" => "🔵",
                    "spider" => "🕷️",
                    "bzzfly" => "🪰",
                    "berry" => "🍓",
                    "redpotion" => "❤️",
                    "bluepotion" => "💙",
                    "sarsaparilla" => "⭐",
                    _ => "⬛"
                };
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}