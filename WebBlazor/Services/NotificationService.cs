using Microsoft.JSInterop;

namespace WebBlazor.Services
{
    public class NotificationService
    {
        private readonly IJSRuntime _jsRuntime;

        public NotificationService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        /// <summary>
        /// Hiển thị notification sử dụng bootstrap-growl
        /// </summary>
        /// <param name="message">Nội dung thông báo</param>
        /// <param name="type">Loại: success, info, warning, danger, primary, inverse</param>
        /// <param name="position">Vị trí: top, bottom</param>
        /// <param name="align">Căn lề: left, center, right</param>
        /// <param name="delay">Thời gian hiển thị (ms)</param>
        /// <param name="icon">Icon CSS class (Font Awesome)</param>
        public async Task ShowAsync(
            string message,
            string type = "info",
            string position = "top",
            string align = "right",
            int delay = 3000,
            string? icon = null)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("notificationManager.show",
                    message, type, position, align, delay, icon);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Notification Error: {ex.Message}");
            }
        }

        public async Task ShowSuccessAsync(string message, string? icon = null, int delay = 3000)
        {
            await ShowAsync(message, "success", "top", "right", delay, icon ?? "fa fa-check");
        }

        public async Task ShowErrorAsync(string message, string? icon = null, int delay = 5000)
        {
            await ShowAsync(message, "danger", "top", "right", delay, icon ?? "fa fa-times");
        }

        public async Task ShowWarningAsync(string message, string? icon = null, int delay = 4000)
        {
            await ShowAsync(message, "warning", "top", "right", delay, icon ?? "fa fa-exclamation");
        }

        public async Task ShowInfoAsync(string message, string? icon = null, int delay = 3000)
        {
            await ShowAsync(message, "info", "top", "right", delay, icon ?? "fa fa-info");
        }

        public async Task ShowPrimaryAsync(string message, string? icon = null, int delay = 3000)
        {
            await ShowAsync(message, "primary", "top", "right", delay, icon ?? "fa fa-star");
        }

        public async Task ShowInverseAsync(string message, string? icon = null, int delay = 3000)
        {
            await ShowAsync(message, "inverse", "top", "right", delay, icon ?? "fa fa-bell");
        }
    }
}