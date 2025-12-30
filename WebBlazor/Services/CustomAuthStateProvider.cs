//using Microsoft.AspNetCore.Mvc;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace QLVN_Blazor.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public CustomAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));  // Unauthenticated
            }

            var handler = new JwtSecurityTokenHandler();

            try
            {
                var jwtToken = handler.ReadJwtToken(token);

                // Kiểm tra thời hạn token (ValidTo) — nếu hết hạn => xoá token và trả về Unauthenticated
                if (jwtToken.ValidTo < DateTime.UtcNow)
                {
                    await _localStorage.RemoveItemAsync("authToken");
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
                var user = new ClaimsPrincipal(identity);

                return new AuthenticationState(user);  // Authenticated
            }
            catch
            {
                // Nếu token không hợp lệ (parse error) => xoá và trả về Unauthenticated
                await _localStorage.RemoveItemAsync("authToken");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        // Method để notify state thay đổi sau login/logout
        public void NotifyUserAuthentication(string token)
        {
            var authenticatedUser = BuildClaimsPrincipal(token);
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogout()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        private ClaimsPrincipal BuildClaimsPrincipal(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
            return new ClaimsPrincipal(identity);
        }

        // Thêm 2 phương thức tương thích vì AuthService hiện đang gọi MarkUserAsAuthenticated/MarkUserAsLoggedOut
        public void MarkUserAsAuthenticated(string token) => NotifyUserAuthentication(token);
        public void MarkUserAsLoggedOut() => NotifyUserLogout();
    }
}