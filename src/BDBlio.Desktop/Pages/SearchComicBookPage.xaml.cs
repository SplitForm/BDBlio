using BDBlio.Desktop.ViewModels;
using System.Windows.Controls;

namespace BDBlio.Desktop
{
    public partial class SearchComicBookPage : Page
    {
        public SearchComicBookPage()
        {
            InitializeComponent();
            DataContext = new SearchComicBookViewModel();
        }
    }
}
