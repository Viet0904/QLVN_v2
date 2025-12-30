using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Common.Model.Auth;
using System.Net.Http.Json;
using System.Text.Json;

namespace QLVN_Blazor.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly NotificationService _notificationService;

        public AuthService(
            HttpClient httpClient,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authStateProvider,
            NotificationService notificationService)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
            _notificationService = notificationService;
        }

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginRequest);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                // Lưu token vào LocalStorage
                await _localStorage.SetItemAsync("authToken", result!.Token);

                // Thông Báo cho Blazor biết đã đăng nhập 
                ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.Token);

                return result;
            }
            else
            {
                // Đọc nội dung lỗi trả về từ API
                var errorContent = await response.Content.ReadAsStringAsync();

                // Cố gắng parse JSON để lấy message nếu có
                string serverMessage = errorContent ?? string.Empty;
                try
                {
                    var doc = JsonDocument.Parse(errorContent);
                    if (doc.RootElement.ValueKind == JsonValueKind.Object)
                    {
                        if (doc.RootElement.TryGetProperty("message", out var m) && m.ValueKind == JsonValueKind.String)
                            serverMessage = m.GetString() ?? serverMessage;
                        else if (doc.RootElement.TryGetProperty("Message", out var m2) && m2.ValueKind == JsonValueKind.String)
                            serverMessage = m2.GetString() ?? serverMessage;
                    }
                }
                catch
                {
                    // ignore parse errors
                }

                // Chuẩn hoá thông báo cho người dùng 
                string userMessage;
                if (!string.IsNullOrWhiteSpace(serverMessage) &&
                    (serverMessage.IndexOf("tài khoản", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     serverMessage.IndexOf("tài khoản không tồn tại", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     serverMessage.IndexOf("mật khẩu", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     serverMessage.IndexOf("mật khẩu không chính xác", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    userMessage = "Đăng nhập thất bại: Tài khoản hoặc mật khẩu không chính xác.";
                }
                else
                {
                    userMessage = "Đăng nhập thất bại. Vui lòng thử lại.";
                }

                // Hiển thị notification lỗi
                await _notificationService.ShowErrorAsync(userMessage);

                Console.WriteLine($"AuthService.Login error (server): {serverMessage}");

                throw new Exception(userMessage);
            }
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
            
        }
    }
}