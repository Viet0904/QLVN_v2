using Common.Model.SysTheme;
using System.Net.Http.Json;

namespace QLVN_Blazor.Services
{
    public class ThemeService
    {
        private readonly HttpClient _http;

        public ThemeService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ThemeSettingsViewModel> GetThemeAsync()
        {
            try
            {
                // Gọi API lấy config, nếu lỗi hoặc chưa có thì trả về mặc định
                var result = await _http.GetFromJsonAsync<ThemeSettingsViewModel>("api/Theme");
                return result ?? new ThemeSettingsViewModel();
            }
            catch
            {
                return new ThemeSettingsViewModel();
            }
        }

        public async Task<bool> SaveThemeAsync(ThemeSettingsViewModel settings)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/Theme", settings);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ResetThemeAsync()
        {
            try
            {
                var response = await _http.DeleteAsync("api/Theme");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}