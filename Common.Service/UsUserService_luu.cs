using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
//using QLVN_Application.Interfaces;
using QLVN_Contracts.Dtos.Auth;
using QLVN_Contracts.Dtos.Common;
using QLVN_Contracts.Dtos.User;
//using QLVN_Domain.Entities;
//using QLVN_Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QLVN_Application;

public class UsUserService_luu : IUserService
{
    private readonly IConfiguration _config;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private static readonly object _lockObject = new object();

    public UsUserService_luu(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _config = config;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _unitOfWork.Repository<UsUser>().GetAllAsync();
        var activeUsers = users.Where(u => u.RowStatus == 1).OrderBy(u => u.Id);
        return _mapper.Map<IEnumerable<UserDto>>(activeUsers);
    }
    
    public async Task<UserDto?> GetByIdAsync(string id)
    {
        var user = await _unitOfWork.Repository<UsUser>().GetByIdAsync(id);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }
    public async Task<PaginatedResponse<UserDto>> GetPaginatedAsync(PaginatedRequest request)
    {
        var allUsers = await _unitOfWork.Repository<UsUser>().GetAllAsync();

        // Filter active users
        var query = allUsers.Where(u => u.RowStatus == 1).AsQueryable();

        // Search
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchLower = request.SearchTerm.ToLower();
            query = query.Where(u =>
                u.Name != null && u.Name.ToLower().Contains(searchLower) ||
                u.UserName != null && u.UserName.ToLower().Contains(searchLower) ||
                u.Email != null && u.Email.ToLower().Contains(searchLower) ||
                u.Phone != null && u.Phone.ToLower().Contains(searchLower)
            );
        }

        // Sort
        query = request.SortColumn?.ToLower() switch
        {
            "name" => request.SortDirection == "desc"
                ? query.OrderByDescending(u => u.Name)
                : query.OrderBy(u => u.Name),
            "username" => request.SortDirection == "desc"
                ? query.OrderByDescending(u => u.UserName)
                : query.OrderBy(u => u.UserName),
            "email" => request.SortDirection == "desc"
                ? query.OrderByDescending(u => u.Email)
                : query.OrderBy(u => u.Email),
            "createdat" => request.SortDirection == "desc"
                ? query.OrderByDescending(u => u.CreatedAt)
                : query.OrderBy(u => u.CreatedAt),
            _ => query.OrderBy(u => u.Id) // Default sort by Id
        };

        // Total count
        var totalRecords = query.Count();

        // Pagination
        var items = query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var userDtos = _mapper.Map<List<UserDto>>(items);

        return new PaginatedResponse<UserDto>
        {
            Items = userDtos,
            TotalRecords = totalRecords,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
    public async Task<UserDto> CreateAsync(CreateUserRequest request)
    {
        // Kiểm tra username đã tồn tại chưa
        var allUsers = await _unitOfWork.Repository<UsUser>().GetAllAsync();
        var existingUser = allUsers.FirstOrDefault(u =>
            u.UserName.Equals(request.UserName, StringComparison.OrdinalIgnoreCase));

        if (existingUser != null)
        {
            throw new InvalidOperationException($"Tên đăng nhập '{request.UserName}' đã tồn tại!");
        }

        // Tạo ID mới
        var newId = await GenerateNextUserIdAsync();

        var userEntity = _mapper.Map<UsUser>(request);
        userEntity.Id = newId;
        userEntity.CreatedAt = DateTime.Now;
        userEntity.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
        userEntity.CreatedBy = "SYSTEM";
        userEntity.UpdatedAt = DateTime.Now;
        userEntity.UpdatedBy = "SYSTEM";
        userEntity.RowStatus = request.RowStatus > 0 ? request.RowStatus : 1;

        await _unitOfWork.Repository<UsUser>().AddAsync(userEntity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<UserDto>(userEntity);
    }

    public async Task<UserDto> UpdateAsync(string id, UpdateUserRequest request)
    {
        var user = await _unitOfWork.Repository<UsUser>().GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"Không tìm thấy người dùng với ID: {id}");

        // Cập nhật các trường
        user.Name = !string.IsNullOrEmpty(request.Name) ? request.Name : user.Name;
        user.GroupId = !string.IsNullOrEmpty(request.GroupId) ? request.GroupId : user.GroupId;
        user.Email = request.Email;
        user.Phone = request.Phone;
        user.Address = request.Address;
        user.Cmnd = request.Cmnd;
        user.Note = request.Note;
        user.Image = request.Image;
        user.Gender = request.Gender;
        user.RowStatus = request.RowStatus;
        
        // Xử lý đổi password nếu có
        if (request.ChangePassword && !string.IsNullOrWhiteSpace(request.Password))
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
        }
        
        user.UpdatedAt = DateTime.Now;
        user.UpdatedBy = "SYSTEM";

        _unitOfWork.Repository<UsUser>().Update(user);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }

    public async Task DeleteAsync(string id)
    {
        var user = await _unitOfWork.Repository<UsUser>().GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"Không tìm thấy người dùng với ID: {id}");

        // Soft delete - set RowStatus = 0
        user.RowStatus = 0;
        user.UpdatedAt = DateTime.Now;
        user.UpdatedBy = "SYSTEM";

        _unitOfWork.Repository<UsUser>().Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    /// <summary>
    /// Tạo ID người dùng mới
    /// Lấy MAX ID hiện có + 1
    /// </summary>
    private async Task<string> GenerateNextUserIdAsync()
    {
        var allUsers = await _unitOfWork.Repository<UsUser>().GetAllAsync();

        // Lọc các ID có format 001XXXXX (8 ký tự, bắt đầu bằng 001)
        var userIds = allUsers
            .Where(u => u.Id.Length == 8 && u.Id.StartsWith("001"))
            .Select(u =>
            {
                if (int.TryParse(u.Id, out int idNum))
                    return idNum;
                return 0;
            })
            .Where(id => id > 0)
            .ToList();

        int nextNumber;
        if (userIds.Any())
        {
            var maxId = userIds.Max();
            nextNumber = maxId + 1;
        }
        else
        {
            // Bắt đầu từ 00100001
            nextNumber = 100001;
        }

        // Format thành 8 ký tự: 00100001, 00100002, ...
        return nextNumber.ToString("D8");
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var users = await _unitOfWork.Repository<UsUser>().GetAllAsync();
        var user = users.FirstOrDefault(x => x.UserName == request.UserName && x.RowStatus == 1);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            throw new UnauthorizedAccessException("Tài khoản hoặc mật khẩu không chính xác");
        }

        var token = GenerateJwtToken(user);

        return new LoginResponse
        {
            Token = token,
            FullName = user.Name
        };
    }

    private string GenerateJwtToken(UsUser user)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName),
            new("FullName", user.Name),
            new("GroupId", user.GroupId)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(8),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}