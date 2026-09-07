using BDBlio.Desktop.ViewModels;
using System.Windows.Controls;

namespace BDBlio.Desktop
{
    public partial class LibraryPage : Page
    {
        private readonly LibraryViewModel _viewModel;

        public LibraryPage()
        {
            InitializeComponent();
            _viewModel = new LibraryViewModel();
            DataContext = _viewModel;
        }

        private async void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            await _viewModel.LoadBooksCommand.ExecuteAsync(null);
        }
    }
}
