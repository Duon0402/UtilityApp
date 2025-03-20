using Microsoft.Playwright;
using System.IO;
using System.Net.Http;
using UtilityApp.Models;

namespace UtilityApp.Services
{
    public class TikTokService
    {
        private readonly HttpClient _httpClient;
        private readonly PlaywrightService _playwrightService;

        public TikTokService()
        {
            _httpClient = new HttpClient();
            _playwrightService = new PlaywrightService();
        }

        public async Task DownloadVideoTikTokByUrlAsync(string url, string savePath = "video.mp4")
        {
            try
            {
                // Khởi tạo trình duyệt với tùy chọn Headless (đang dùng false để debug, có thể chuyển thành true khi deploy)
                var browserTypeLaunchOptions = new BrowserTypeLaunchOptions
                {
                    Headless = false
                };

                // Tạo trình duyệt và trang mới từ PlaywrightService
                var browser = await _playwrightService.CreateBrowserAsync(BrowserOptions.Chromium, browserTypeLaunchOptions);
                var page = await _playwrightService.CreateNewPageAsync(browser);

                Console.WriteLine("🔍 Đang truy cập TikTok...");
                await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.Load });

                // Chờ thẻ <source> bên trong <video> được thêm vào DOM (không cần visible)
                await page.WaitForSelectorAsync("video source", new PageWaitForSelectorOptions { State = WaitForSelectorState.Attached });

                // Lấy tất cả các thẻ <source> bên trong <video>
                var sources = await page.QuerySelectorAllAsync("video source");
                Console.WriteLine($"📌 Số lượng thẻ <source> tìm thấy: {sources.Count}");

                // Nếu có ít nhất một thẻ <source>, sử dụng phần tử đầu tiên
                if (sources.Count > 0)
                {
                    var videoUrl = await sources[2].EvaluateAsync<string>("el => el.src");
                    Console.WriteLine($"🎯 Video URL: {videoUrl}");

                    if (!string.IsNullOrEmpty(videoUrl))
                    {
                        try
                        {
                            using (HttpClient client = new HttpClient())
                            {
                                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

                                Console.WriteLine("📥 Đang tải video...");
                                using (HttpResponseMessage response = await client.GetAsync(videoUrl, HttpCompletionOption.ResponseHeadersRead))
                                {
                                    response.EnsureSuccessStatusCode();

                                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                                                   fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                                    {
                                        await contentStream.CopyToAsync(fileStream);
                                    }
                                }
                            }

                            Console.WriteLine($"✅ Video đã tải xong: {savePath}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"⚠️ Lỗi tải video: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("⚠️ Video URL rỗng.");
                    }
                }
                else
                {
                    Console.WriteLine("⚠️ Không tìm thấy thẻ <source> nào.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Lỗi: {ex.Message}");
                throw;
            }
        }
    }
}