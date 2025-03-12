using System.Windows;
using UtilityApp.Services;

namespace UtilityApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnRunDemo_Click(object sender, RoutedEventArgs e)
        {
            var service = new PlaywrightFacebookService();
            //await service.LoginFacebookAsync("truongduong0402@gmail.com", "D@ngDuong04022002");
        }
    }
}