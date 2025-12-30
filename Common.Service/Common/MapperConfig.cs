using AutoMapper;
using Common.Database.Entities;
using Common.Library.Extension;
using Common.Library.Helper;
using Common.Model.UsUser;

namespace Common.Service.Common
{
    //public class MapperConfig : Profile
    //{
    //    public MapperConfig()
    //    {
    //        // Map 2 chiều giữa Entity và DTO
    //        CreateMap<DbDvsd, DvsdDto>().ReverseMap();
    //        CreateMap<CreateDvsdRequest, DbDvsd>();

    //        CreateMap<UsUser, UserDto>().ReverseMap();
    //        CreateMap<CreateUserRequest, UsUser>();

    //        CreateMap<UsGroup, GroupDto>().ReverseMap();
    //        CreateMap<UsGroup, GroupDto>();
    //    }
    //}
    public class MapperConfig
    {
        public static IMapper BuildMapper(BaseEntity baseEntity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                // #region UsGroup

                // cfg.CreateMap<UsGroup, UsGroupViewModel>()
                // .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTimeHelper.ToString(src.CreatedAt)))
                // .ForMember(dest => dest.CreatedName, opt => opt.MapFrom(src => FormatUser(src.CreatedBy, baseEntity)))
                // .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => DateTimeHelper.ToString(src.UpdatedAt)))
                // .ForMember(dest => dest.UpdatedName, opt => opt.MapFrom(src => FormatUser(src.UpdatedBy, baseEntity)));

                // cfg.CreateMap<UsGroup, UsGroupModel>();

                // cfg.CreateMap<UsGroupCreateModel, UsGroup>()
                // .IgnoreAllNonExisting();

                // cfg.CreateMap<UsGroupUpdateModel, UsGroup>()
                // .IgnoreAllNonExisting();

                // #endregion UsGroup

                // #region UsUserPermission
                // cfg.CreateMap<UsUserPermission, UsUserPermissionViewModel>();
                // cfg.CreateMap<UsUserPermissionCreateModel, UsUserPermission>()
                // .IgnoreAllNonExisting();

                // #endregion UsUserPermission

                #region User

                cfg.CreateMap<UsUser, UsUserViewModel>()
                // .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.us.UsGroup != null ? src.UsGroup.Name : string.Empty))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTimeHelper.ToString(src.CreatedAt)))
                 .ForMember(dest => dest.CreatedName, opt => opt.MapFrom(src => FormatUser(src.CreatedBy, baseEntity)))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => DateTimeHelper.ToString(src.UpdatedAt)))
                .ForMember(dest => dest.UpdatedName, opt => opt.MapFrom(src => FormatUser(src.UpdatedBy, baseEntity)));

                // cfg.CreateMap<UsUser, UsUserModel>();

                cfg.CreateMap<UsUserCreateModel, UsUser>()
                .IgnoreAllNonExisting();

                cfg.CreateMap<UsUserUpdateModel, UsUser>()
                .IgnoreAllNonExisting();

                #endregion User

                // #region UserLog

                // cfg.CreateMap<vUserLog, ViewUserLogModel>()
                //.ForMember(dest => dest.ActionDate, opt => opt.MapFrom(src => DateTimeHelper.ToString(src.ActionDate)));

                // cfg.CreateMap<UsUserLogCreateModel, UsUserLog>()
                // .IgnoreAllNonExisting();

                // #endregion UserLog


            });

            return config.CreateMapper();
        }

        private static string FormatUser(string userId, BaseEntity baseEntity)
        {
            var user = baseEntity.UsUsers.Where(x => x.Id == userId).FirstOrDefault();
            return user != null ? user.Name : string.Empty;
        }
    }
}