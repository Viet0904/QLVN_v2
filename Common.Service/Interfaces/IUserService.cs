using Common.Model.Auth;
using Common.Model.Common;
using Common.Model.User;

namespace Common.Service.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        //  Thêm method mới cho pagination
        Task<PaginatedResponse<UserDto>> GetPaginatedAsync(PaginatedRequest request);
        Task<UserDto?> GetByIdAsync(string id);
        Task<UserDto> CreateAsync(CreateUserRequest request);
        Task<UserDto> UpdateAsync(string id, UpdateUserRequest request);
        Task DeleteAsync(string id);
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}