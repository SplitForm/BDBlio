using System.Windows;
using BDBlio.Desktop.ViewModels;

namespace BDBlio.Desktop
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialiser la base de données
            InitializeDatabase();

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void InitializeDatabase()
        {
            try
            {
                var context = new BDBlio.Data.BDBlioDatabaseContext();
                context.Database.EnsureCreated();
                context.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur d'initialisation de la base de données: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
