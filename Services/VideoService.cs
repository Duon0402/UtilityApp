using System.Diagnostics;
using System.IO;

namespace UtilityApp.Services
{
    public static class VideoService
    {
        public static async Task FlipVideoAsync(string inputPath, string outputPath, bool isHorizontal)
        {
            string flipFilter = isHorizontal ? "hflip" : "vflip";
            string ffmpegPath = "D:/MyData/MyProjects/UtilityApp/EmbeddedResource/ffmpeg.exe";

            if (!File.Exists(ffmpegPath))
                throw new FileNotFoundException($"Không tìm thấy ffmpeg.exe tại: {ffmpegPath}");

            var processInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = $"-hwaccel auto -i \"{inputPath}\" -vf \"{flipFilter}\" -c:v libx264 -preset ultrafast -crf 18 -threads 4 \"{outputPath}\"",
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
