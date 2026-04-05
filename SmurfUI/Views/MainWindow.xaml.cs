using System.Windows;
using System.Windows.Input;

namespace SmurfUI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // Activer les touches clavier
            this.KeyDown += MainWindow_KeyDown;
        }
        
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            var viewModel = DataContext as ViewModels.MainViewModel;
            if (viewModel == null) return;
            
            // Touches directionnelles
            switch (e.Key)
            {
                case Key.Up:
                case Key.Z:
                    viewModel.MoveUpCommand.Execute(null);
                    e.Handled = true;
                    break;
                case Key.Down:
                case Key.S:
                    viewModel.MoveDownCommand.Execute(null);
                    e.Handled = true;
                    break;
                case Key.Left:
                case Key.Q:
                    viewModel.MoveLeftCommand.Execute(null);
                    e.Handled = true;
                    break;
                case Key.Right:
                case Key.D:
                    viewModel.MoveRightCommand.Execute(null);
                    e.Handled = true;
                    break;
                case Key.N:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        viewModel.NewGameCommand.Execute(null);
                        e.Handled = true;
                    }
                    break;
                case Key.Escape:
                    this.Close();
                    break;
            }
        }
    }
}