using System.Diagnostics;
using System.IO;

namespace UtilityApp.Services
{
    public static class VideoService
    {
        private static readonly string FFMEPG_PATH = "D:/MyData/MyProjects/UtilityApp/EmbeddedResource/ffmpeg.exe";
        public static async Task FlipVideoAsync(string inputPath, string outputPath, bool isHorizontal)
        {
            string flipFilter = isHorizontal ? "hflip" : "vflip";

            if (!File.Exists(FFMEPG_PATH))
                throw new FileNotFoundException($"Không tìm thấy ffmpeg.exe tại: {FFMEPG_PATH}");

            var processInfo = new ProcessStartInfo
            {
                FileName = FFMEPG_PATH,
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

        public static async Task ConvertVideoToAudio(string inputPath, string outputPath)
        {
            string arguments = $"-i \"{inputPath}\" -q:a 0 -map a \"{outputPath}\"";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = FFMEPG_PATH,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi))
            {
                // Đợi cho quá trình xử lý hoàn tất
                await process!.WaitForExitAsync();

                // Kiểm tra mã kết thúc để xác định có lỗi không
                if (process.ExitCode != 0)
                {
                    string error = await process.StandardError.ReadToEndAsync();
                    throw new Exception($"FFmpeg lỗi: {error}");
                }
            }
        }
    }
}
