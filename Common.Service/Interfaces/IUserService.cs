using Common.Model.Auth;
using Common.Model.Common;
using Common.Model.UsUser;

namespace Common.Service.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UsUserViewModel>> GetAllAsync();
        //  Thêm method mới cho pagination
        Task<PaginatedResponse<UsUserViewModel>> GetPaginatedAsync(PaginatedRequest request);
        Task<UsUserViewModel?> GetByIdAsync(string id);
        Task<UsUserViewModel> CreateAsync(UsUserCreateModel request);
        Task<UsUserViewModel> UpdateAsync(string id, UsUserUpdateModel request);
        Task DeleteAsync(string id);
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}