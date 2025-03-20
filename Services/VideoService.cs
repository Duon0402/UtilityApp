using System.Diagnostics;
using System.IO;

namespace UtilityApp.Services
{
    public static class VideoService
    {
        public static async Task FlipVideoAsync(string inputPath, string outputPath, bool isHorizontal)
        {
            string flipFilter = isHorizontal ? "hflip" : "vflip";

            string ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmbeddedResource", "ffmpeg.exe");

            if (!File.Exists(ffmpegPath))
                throw new FileNotFoundException($"Không tìm thấy ffmpeg.exe tại: {ffmpegPath}");

            var processInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = $"-i \"{inputPath}\" -vf \"{flipFilter}\" \"{outputPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = processInfo })
            {
                process.Start();
                await process.WaitForExitAsync();
                if (process.ExitCode != 0)
                {
                    string error = await process.StandardError.ReadToEndAsync();
                    throw new Exception($"Lỗi xử lý video: {error}");
                }
            }
        }
    }
}
