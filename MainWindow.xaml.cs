using System.Windows;
using UtilityApp.Models;
using UtilityApp.Services;

namespace UtilityApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TikTokService _tiktokService;

        public MainWindow()
        {
            InitializeComponent();
            _tiktokService = new TikTokService();
        }

        private async void btnRunDemo_Click(object sender, RoutedEventArgs e)
        {
            string url = txtInput.Text.Trim();
            string savePath = "video.mp4";

            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("Vui lòng nhập URL TikTok!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                MessageBox.Show("Đang tải video...", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                await _tiktokService.DownloadVideoTikTokByUrlAsync(url, savePath);
                MessageBox.Show($"Video đã lưu tại: {savePath}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}