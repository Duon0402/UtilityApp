using Microsoft.Playwright;
using Microsoft.Win32;
using System.IO;
using UtilityApp.Models;

namespace UtilityApp.Services
{
    public class PlaywrightService : IAsyncDisposable
    {
        private static readonly Lazy<Task<IPlaywright>> LazyPlaywright = new(() => Playwright.CreateAsync());

        private static Task<IPlaywright> PlaywrightInstance => LazyPlaywright.Value;

        public PlaywrightService()
        {
        }

        public async Task<IBrowser> CreateBrowserAsync(BrowserOptions options = BrowserOptions.Chromium, BrowserTypeLaunchOptions? browserTypeLaunchOptions = null)
        {
            var playwright = await PlaywrightInstance;
            switch (options)
            {
                case BrowserOptions.Chromium:
                    var chromePath = GetChromeExecutablePathFromRegistry();
                    if (!string.IsNullOrEmpty(chromePath))
                    {
                        if(browserTypeLaunchOptions != null)
                        {
                            browserTypeLaunchOptions.ExecutablePath = chromePath;
                            return await playwright.Chromium.LaunchAsync(browserTypeLaunchOptions);
                        }

                        return await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                        {
                            Headless = false,
                            ExecutablePath = chromePath
                        });
                    }
                    else
                    {
                        throw new FileNotFoundException("Không tìm thấy Chrome được cài đặt trên máy.");
                    }
                // Các trường hợp khác (Edge, Firefox, WebKit) có thể xử lý tương tự...
                default:
                    throw new ArgumentException("Unsupported browser option");
            }
        }

        public async Task<IPage> CreateNewPageAsync(IBrowser? browser = null)
        {
            if (browser == null)
            {
                browser = await CreateBrowserAsync();
            }
            return await browser.NewPageAsync();
        }

        public async Task GotoAsync(string url, IPage? page = null)
        {
            if (page == null)
            {
                page = await CreateNewPageAsync();
            }
            await page.GotoAsync(url);
        }


        // TODO: 
        public async Task ScreenshotAsync(IPage? page, bool isFullPage = false)
        {
            if (page == null)
            {
                page = await CreateNewPageAsync();
            }

            await page.ScreenshotAsync(new PageScreenshotOptions
            {
                FullPage = isFullPage
            });
        }

        public async ValueTask DisposeAsync()
        {
            var playwright = await PlaywrightInstance;
            if (playwright is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else
            {
                playwright.Dispose();
            }
        }

        private string? GetChromeExecutablePathFromRegistry()
        {
            const string registryKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe";
            string? path = GetRegistryValue(Registry.LocalMachine, registryKeyPath);
            if (!string.IsNullOrEmpty(path))
                return path;
            return GetRegistryValue(Registry.CurrentUser, registryKeyPath);
        }

        private string? GetRegistryValue(RegistryKey baseKey, string registryKeyPath)
        {
            using var key = baseKey.OpenSubKey(registryKeyPath);
            return key?.GetValue("")?.ToString();
        }
    }
}