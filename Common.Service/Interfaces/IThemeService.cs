using Common.Model.SysTheme;

namespace Common.Service.Interfaces
{
    public interface IThemeService
    {
        // Lấy cấu hình theme của user hiện tại
        Task<ThemeSettingsViewModel> GetThemeSettingsAsync(string userId);

        // Lưu cấu hình theme (nhận vào object ThemeSettings)
        Task SaveThemeSettingsAsync(string userId, ThemeSettingsUpdateModel settings);
    }
}
