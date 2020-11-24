using SII2.ViewModels;
using System.Windows;

namespace SII2.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();

            DataContext = new ApplicationViewModel();
        }

        private void LoadTreeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CalcDistanceButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
