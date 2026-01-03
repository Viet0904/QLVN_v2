using AutoMapper;
using Common.Database.Data;
using Common.Database.Entities;
using Common.Library.Extension;
using Common.Library.Helper;
using Common.Model.UsUser;
using Common.Model.SysMenu;
using Common.Model.UsGroup;
using System.Linq;

namespace Common.Service.Common
{
    public class MapperConfig
    {
        // Giữ lại cho compatibility với code cũ
        public static IMapper BuildMapper(BaseEntity baseEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                ConfigureMappings(cfg, baseEntity);
            });
            return config.CreateMapper();
        }

        // ✅ METHOD MỚI - Dùng cho DI với QLVN_DbContext
        public static IMapper BuildMapper(QLVN_DbContext dbContext)
        {
            var config = new MapperConfiguration(cfg =>
            {
                ConfigureMappings(cfg, dbContext);
            });
            return config.CreateMapper();
        }

        // ✅ METHOD CHUNG - Định nghĩa mapping
        private static void ConfigureMappings(IMapperConfigurationExpression cfg, object context)
        {
            #region UsUser
            
            cfg.CreateMap<UsUser, UsUserViewModel>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTimeHelper.ToString(src.CreatedAt)))
                .ForMember(dest => dest.CreatedName, opt => opt.MapFrom(src => FormatUser(src.CreatedBy ?? string.Empty, context)))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => DateTimeHelper.ToString(src.UpdatedAt)))
                .ForMember(dest => dest.UpdatedName, opt => opt.MapFrom(src => FormatUser(src.UpdatedBy ?? string.Empty, context)));

            cfg.CreateMap<UsUserCreateModel, UsUser>()
                .IgnoreAllNonExisting();

            cfg.CreateMap<UsUserUpdateModel, UsUser>()
                .IgnoreAllNonExisting();

            #endregion

            #region SysMenu
            
            cfg.CreateMap<SysMenu, SysMenuViewModel>();

            cfg.CreateMap<SysMenuCreateModel, SysMenu>()
                .IgnoreAllNonExisting();

            cfg.CreateMap<SysMenuUpdateModel, SysMenu>()
                .IgnoreAllNonExisting();

            #endregion

            #region UsGroup
            
            cfg.CreateMap<UsGroup, UsGroupViewModel>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTimeHelper.ToString(src.CreatedAt)))
                .ForMember(dest => dest.CreatedName, opt => opt.MapFrom(src => FormatUser(src.CreatedBy ?? string.Empty, context)))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => DateTimeHelper.ToString(src.UpdatedAt)))
                .ForMember(dest => dest.UpdatedName, opt => opt.MapFrom(src => FormatUser(src.UpdatedBy ?? string.Empty, context)));

            cfg.CreateMap<UsGroupCreateModel, UsGroup>()
                .IgnoreAllNonExisting();

            cfg.CreateMap<UsGroupUpdateModel, UsGroup>()
                .IgnoreAllNonExisting();
            #endregion


        }

        // Helper method - Format user name
        private static string FormatUser(string userId, object context)
        {
            if (string.IsNullOrEmpty(userId)) return string.Empty;

            try
            {
                if (context is BaseEntity baseEntity)
                {
                    var user = baseEntity.UsUsers.FirstOrDefault(x => x.Id == userId);
                    return user?.Name ?? userId;
                }
                else if (context is QLVN_DbContext dbContext)
                {
                    var user = dbContext.UsUsers.FirstOrDefault(x => x.Id == userId);
                    return user?.Name ?? userId;
                }
            }
            catch
            {
                // Nếu có lỗi, trả về userId
            }

            return userId;
        }
    }
}