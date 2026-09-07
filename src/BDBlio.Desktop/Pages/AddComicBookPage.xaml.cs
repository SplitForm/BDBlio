using BDBlio.Desktop.ViewModels;
using System.Windows.Controls;

namespace BDBlio.Desktop
{
    public partial class AddComicBookPage : Page
    {
        public AddComicBookPage()
        {
            InitializeComponent();
            DataContext = new AddComicBookViewModel();
        }
    }
}
