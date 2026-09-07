using BDBlio.Desktop.ViewModels;
using System.Windows.Controls;

namespace BDBlio.Desktop
{
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            DataContext = new SettingsViewModel();
        }
    }
}
