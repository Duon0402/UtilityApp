using Microsoft.Playwright;

namespace UtilityApp.Services
{
    public class PlaywrightFacebookService
    {
        private readonly PlaywrightService _playwrightService;

        public PlaywrightFacebookService()
        {
            _playwrightService = new PlaywrightService();
        }

        public async Task LoginFacebookAsync(string email, string password)
        {
            var browser = await _playwrightService.CreateBrowserAsync();
            var facebookPage = await _playwrightService.CreateNewPageAsync(browser);
            await facebookPage.GotoAsync("https://www.facebook.com/");

            await facebookPage.WaitForSelectorAsync("input#email");

            await facebookPage.FillAsync("input#email", email);
            await facebookPage.FillAsync("input#pass", password);

            await facebookPage.ClickAsync("button[name='login']");
        }
    }
}
