using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmurfBL.Interfaces;
using SmurfBL.Services;
using SmurfDAL;
using SmurfDAL.Managers;
using SmurfUI.ViewModels;
using SmurfUI.Views;

namespace SmurfUI
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            // ── 1. DbContext en Scoped (pas Singleton) ──
            services.AddScoped<SmurfDbContext>(sp =>
            {
                var options = new DbContextOptionsBuilder<SmurfDbContext>()
                    .UseSqlServer(
                        "Server=(localdb)\\mssqllocaldb;" +
                        "Database=SmurfForestDB;" +
                        "Trusted_Connection=True;" +
                        "MultipleActiveResultSets=true")
                    .Options;
                return new SmurfDbContext(options);
            });

            // ── 2. Couche métier en Singleton ──
            services.AddSingleton<IGameEngine, GameEngine>();

            // ── 3. Couche données en Scoped ──
            services.AddScoped<IGameManager, GameManager>();

            // ── 4. ViewModel en Singleton ──
            services.AddSingleton<MainViewModel>();

            ServiceProvider = services.BuildServiceProvider();

            // ── 5. Créer la base si elle n'existe pas ──
            try
            {
                using (var scope = ServiceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<SmurfDbContext>();
                    dbContext.Database.EnsureCreated();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Impossible de créer/accéder à la base de données.\n\n" +
                    $"Vérifie que SQL Server LocalDB est installé.\n\n" +
                    $"Détail : {ex.Message}",
                    "Erreur base de données",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown();
                return;
            }

            // ── 6. Démarrer la fenêtre ──
            var mainWindow = new MainWindow();
            using (var scope = ServiceProvider.CreateScope())
            {
                mainWindow.DataContext = scope.ServiceProvider.GetRequiredService<MainViewModel>();
            }
            mainWindow.Show();
        }
    }
}