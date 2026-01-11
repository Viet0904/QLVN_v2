using Blazored.LocalStorage;
using Blazored.Typeahead;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Http;
using WebBlazor.Handlers;
using WebBlazor.Services;
using MudBlazor.Services;
using System;

namespace WebBlazor
{
    public partial class App : ComponentBase { }
    
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5084")
            });

            // Cấu hình HttpClient có sử dụng JwtAuthorizationHandler
            builder.Services.AddHttpClient("Common.API", client =>
            {
                client.BaseAddress = new Uri("http://localhost:5084");
            })
            .AddHttpMessageHandler<JwtAuthorizationHandler>();

            // Thay đổi cách đăng ký HttpClient mặc định
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Common.API"));

            builder.Services.AddScoped<UserApiClient>();
            builder.Services.AddScoped<NotificationService>();

            // Đăng ký Handler
            builder.Services.AddScoped<JwtAuthorizationHandler>();
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
            builder.Services.AddScoped<ThemeService>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<MenuService>();

            // Thêm MudBlazor Services
            builder.Services.AddMudServices();

            await builder.Build().RunAsync();
        }
    }
}
