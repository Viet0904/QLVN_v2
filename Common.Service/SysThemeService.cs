using Common.Database;
using Common.Library.Constant;
using Common.Library.Helper;
using Common.Model.Common;
using Common.Model.SysTheme;
using Common.Service.Common;
using System;

using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common.Service
{
    public class SysThemeService : BaseService
    {
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

        //public async Task<ThemeSettings> GetThemeSettingsAsync(string userId)
        //{
        //    var config = await _unitOfWork.Repository<SysTheme>().GetByIdAsync(userId);

        //    if (config == null || string.IsNullOrEmpty(config.Settings))
        //    {
        //        // Nếu chưa có cấu hình, trả về mặc định (theo class bạn định nghĩa)
        //        return new ThemeSettings();
        //    }

        //    try
        //    {
        //        // Deserialize JSON từ DB ra Object
        //        return JsonSerializer.Deserialize<ThemeSettings>(config.Settings) ?? new ThemeSettings();
        //    }
        //    catch
        //    {
        //        return new ThemeSettings();
        //    }
        //}

        //public async Task SaveThemeSettingsAsync(string userId, ThemeSettings settings)
        //{
        //    var repo = _unitOfWork.Repository<UsUserConfig>();
        //    var config = await repo.GetByIdAsync(userId);

        //    // Chuyển Object C# thành chuỗi JSON để lưu DB
        //    var jsonString = JsonSerializer.Serialize(settings);

        //    if (config == null || string.IsNullOrEmpty(config.Settings))
        //    {
        //        config = new UsUserConfig
        //        {
        //            UserId = userId,
        //            Settings = jsonString,
        //            UpdatedAt = DateTime.Now
        //        };
        //        await repo.AddAsync(config);
        //    }
        //    else
        //    {
        //        config.Settings = jsonString;
        //        config.UpdatedAt = DateTime.Now;
        //        repo.Update(config);
        //    }
        //    await _unitOfWork.SaveChangesAsync();
        //}


        //public async Task ResetThemeSettingsAsync(string userId)
        //{
        //    var repo = _unitOfWork.Repository<UsUserConfig>();
        //    var config = await repo.GetByIdAsync(userId);
        //    if (config != null)
        //    {
        //        repo.Delete(config);
        //        await _unitOfWork.SaveChangesAsync();
        //    }
        //}
    }


}
