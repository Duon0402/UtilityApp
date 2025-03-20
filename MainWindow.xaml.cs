using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using UtilityApp.Services;
using WindowsAPICodePack.Dialogs;

namespace UtilityApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _videoPath = "";
        private string _saveFolder = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectVideo_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Video Files (*.mp4;*.avi;*.mov)|*.mp4;*.avi;*.mov",
                Title = "Chọn Video"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _videoPath = openFileDialog.FileName;
                VideoPath.Text = $"📂 {_videoPath}";
            }
        }

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Title = "Chọn thư mục để lưu video";

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                _saveFolder = dialog.FileName;
                SavePath.Text = $"📁 {_saveFolder}";
            }
        }


        private async void FlipVideo_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_videoPath) || string.IsNullOrEmpty(_saveFolder))
            {
                Status.Text = "⚠️ Vui lòng chọn video và thư mục lưu!";
                return;
            }

            string flipOption = (FlipType.SelectedItem as ComboBoxItem)?.Content.ToString();
            bool isHorizontal = flipOption == "Lật Ngang";

            Status.Text = "⏳ Đang xử lý...";
            try
            {
                string outputFile = Path.Combine(_saveFolder, $"Flipped_{Path.GetFileName(_videoPath)}");
                await VideoService.FlipVideoAsync(_videoPath, outputFile, isHorizontal);
                Status.Text = $"✅ Video đã lật thành công!\nLưu tại: {outputFile}";
            }
            catch (Exception ex)
            {
                Status.Text = $"❌ Lỗi: {ex.Message}";
            }
        }
    }
}