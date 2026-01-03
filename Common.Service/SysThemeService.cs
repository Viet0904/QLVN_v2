using AutoMapper;
using Common.Database;
using Common.Database.Data;
using Common.Library.Constant;
using Common.Library.Helper;
using Common.Model.Common;
using Common.Model.SysTheme;
using Common.Service.Common;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Common.Service
{
    public class SysThemeService : BaseService
    {
        // ✅ Constructor nhận DbContext và IMapper
        public SysThemeService(QLVN_DbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(dbContext, mapper, httpContextAccessor)
        {
        }

        /// <summary>
        /// Lấy cấu hình theme của user
        /// </summary>
        public Task<ThemeSettingsViewModel> GetThemeSettingsAsync(string userId)
        {
            return Task.Run(() =>
            {
                var result = DbContext.UsUsers
                    .Where(x => x.Id == userId && x.RowStatus == RowStatusConstant.Active)
                    .FirstOrDefault();

                if (result != null && !string.IsNullOrEmpty(result.Theme))
                {
                    try
                    {
                        return JsonSerializer.Deserialize<ThemeSettingsViewModel>(result.Theme) ?? new ThemeSettingsViewModel();
                    }
                    catch
                    {
                        return new ThemeSettingsViewModel();
                    }
                }

                return new ThemeSettingsViewModel();
            });
        }

        /// <summary>
        /// Lưu cấu hình theme
        /// </summary>
        public Task SaveThemeSettingsAsync(string userId, ThemeSettingsUpdateModel settings)
        {
            return Task.Run(() =>
            {
                var user = DbContext.UsUsers.Where(x => x.Id == userId).FirstOrDefault();
                
                if (user != null)
                {
                    var jsonString = JsonSerializer.Serialize(settings);
                    user.Theme = jsonString;
                    user.UpdatedAt = DateTime.Now;
                    user.UpdatedBy = GetCurrentUserId();
                    DbContext.SaveChanges();
                }
            });
        }

        /// <summary>
        /// Lấy theme theo UserId - ResModel version
        /// </summary>
        public ResModel<ThemeSettingsViewModel> GetById(string userId)
        {
            ResModel<ThemeSettingsViewModel> res = new ResModel<ThemeSettingsViewModel>();

            var result = DbContext.UsUsers
                .Where(x => x.Id == userId && x.RowStatus == RowStatusConstant.Active)
                .FirstOrDefault();
                
            if (result != null && !string.IsNullOrEmpty(result.Theme))
            {
                try
                {
                    res.Data = JsonSerializer.Deserialize<ThemeSettingsViewModel>(result.Theme) ?? new ThemeSettingsViewModel();
                }
                catch
                {
                    res.Data = new ThemeSettingsViewModel();
                }
            }
            else
            {
                res.Data = new ThemeSettingsViewModel();
            }

            return res;
        }

        /// <summary>
        /// Cập nhật theme - ResModel version
        /// </summary>
        public ResModel<bool> Update(ThemeSettingsUpdateModel model)
        {
            ResModel<bool> res = new ResModel<bool>();
            try
            {
                var result = DbContext.UsUsers.Where(x => x.Id == model.UserId).FirstOrDefault();
                if (result != null)
                {
                    var jsonString = JsonSerializer.Serialize(model);
                    result.Theme = jsonString;
                    result.UpdatedAt = DateTime.Now;
                    result.UpdatedBy = GetCurrentUserId();
                    DbContext.SaveChanges();
                    res.Data = true;
                }
                else
                {
                    res.ErrorMessage = MessageConstant.NOT_EXIST;
                }
            }
            catch (Exception e)
            {
                res.ErrorMessage = e.Message;
                ExceptionHelper.HandleException(e);
            }

            return res;
        }
    }
}
