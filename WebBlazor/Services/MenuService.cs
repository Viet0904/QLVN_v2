using Common.Model.Common;
using Common.Model.SysMenu;
using System.Net.Http.Json;

namespace WebBlazor.Services
{
    public class MenuService
    {
        private readonly HttpClient _http;

        public MenuService(HttpClient http)
        {
            _http = http;
        }

        /// <summary>
        /// Lấy tất cả menu Active
        /// </summary>
        public async Task<List<SysMenuViewModel>> GetAllMenusAsync()
        {
            try
            {
                var response = await _http.GetFromJsonAsync<ResModel<List<SysMenuViewModel>>>("api/menu");
                return response?.Data ?? new List<SysMenuViewModel>();
            }
            catch
            {
                return new List<SysMenuViewModel>();
            }
        }

        /// <summary>
        /// Lấy menu gốc (ParentMenu = null)
        /// </summary>
        public async Task<List<SysMenuViewModel>> GetRootMenusAsync()
        {
            try
            {
                var response = await _http.GetFromJsonAsync<ResModel<List<SysMenuViewModel>>>("api/menu/root");
                return response?.Data ?? new List<SysMenuViewModel>();
            }
            catch
            {
                return new List<SysMenuViewModel>();
            }
        }

        /// <summary>
        /// Lấy menu con theo ParentMenu
        /// </summary>
        public async Task<List<SysMenuViewModel>> GetChildMenusAsync(string parentName)
        {
            try
            {
                var response = await _http.GetFromJsonAsync<ResModel<List<SysMenuViewModel>>>($"api/menu/parent/{parentName}");
                return response?.Data ?? new List<SysMenuViewModel>();
            }
            catch
            {
                return new List<SysMenuViewModel>();
            }
        }

        /// <summary>
        /// Lấy cây menu đầy đủ
        /// </summary>
        public async Task<List<SysMenuViewModel>> GetMenuTreeAsync()
        {
            try
            {
                var response = await _http.GetFromJsonAsync<ResModel<List<SysMenuViewModel>>>("api/menu/tree");
                return response?.Data ?? new List<SysMenuViewModel>();
            }
            catch
            {
                return new List<SysMenuViewModel>();
            }
        }
    }
}

