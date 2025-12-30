using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Service;
using Common.Model.SysTheme;

namespace Common.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // bắt buộc đăng nhập
    public class ThemeController : Controller
    {
        private readonly SysThemeService _themeService;

        public ThemeController(SysThemeService themeService)
        {
            _themeService = themeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTheme()
        {
            // Lấy UserId từ Token JWT
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var settings = await _themeService.GetThemeSettingsAsync(userId);
            return Ok(settings);
        }

        [HttpPost]
        public async Task<IActionResult> SaveTheme([FromBody] ThemeSettingsUpdateModel settings)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            settings.UserId = userId;
            await _themeService.SaveThemeSettingsAsync(userId, settings);
            return Ok(new { message = "Đã lưu cấu hình theme thành công" });
        }

        [HttpDelete]
        public async Task<IActionResult> ResetTheme()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            // Reset về mặc định bằng cách lưu ThemeSettingsUpdateModel mặc định
            await _themeService.SaveThemeSettingsAsync(userId, new ThemeSettingsUpdateModel { UserId = userId });
            return Ok(new { message = "Đã reset theme về mặc định" });
        }
    }
}
