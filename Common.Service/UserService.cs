using AutoMapper;
using Common.Database.Data;
using Common.Library.Constant;
using Common.Library.Helper;
using Common.Model.Auth;
using Common.Model.Common;
using Common.Model.User;
using Common.Model.UsUser;
using Common.Service.Common;
using Common.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Common.Service
{
    /// <summary>
    /// Service cho User - wrap UsUserService để implement IUserService với async methods
    /// </summary>
    public class UserService : BaseService, IUserService
    {
        private readonly IConfiguration _config;
        private readonly UsUserService _usUserService;

        // ✅ Constructor nhận DbContext, IMapper và IConfiguration
        public UserService(QLVN_DbContext dbContext, IMapper mapper, IConfiguration config)
            : base(dbContext, mapper)
        {
            _config = config;
            _usUserService = new UsUserService(dbContext, mapper); // ✅ Truyền dependencies
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            return await Task.Run(() =>
            {
                var result = _usUserService.GetAll();
                if (result.IsSuccess && result.Data != null)
                {
                    return result.Data.Select(x => new UserDto
                    {
                        Id = x.Id ?? string.Empty,
                        GroupId = x.GroupId ?? string.Empty,
                        Name = x.Name ?? string.Empty,
                        Gender = x.Gender,
                        UserName = x.UserName ?? string.Empty,
                        Email = x.Email,
                        Phone = x.Phone,
                        Cmnd = x.CMND,
                        Address = x.Address,
                        Image = x.Image,
                        Note = x.Note,
                        RowStatus = x.RowStatus,
                        CreatedAt = x.CreatedAt,
                        CreatedBy = x.CreatedBy ?? string.Empty,
                        UpdatedAt = x.UpdatedAt,
                        UpdatedBy = x.UpdatedBy ?? string.Empty
                    }).ToList();
                }
                return new List<UserDto>();
            });
        }

        public async Task<UserDto?> GetByIdAsync(string id)
        {
            return await Task.Run(() =>
            {
                var result = _usUserService.GetById(id);
                if (result.IsSuccess && result.Data != null)
                {
                    var x = result.Data;
                    return new UserDto
                    {
                        Id = x.Id ?? string.Empty,
                        GroupId = x.GroupId ?? string.Empty,
                        Name = x.Name ?? string.Empty,
                        Gender = x.Gender,
                        UserName = x.UserName ?? string.Empty,
                        Email = x.Email,
                        Phone = x.Phone,
                        Cmnd = x.CMND,
                        Address = x.Address,
                        Image = x.Image,
                        Note = x.Note,
                        RowStatus = x.RowStatus,
                        CreatedAt = x.CreatedAt,
                        CreatedBy = x.CreatedBy ?? string.Empty,
                        UpdatedAt = x.UpdatedAt,
                        UpdatedBy = x.UpdatedBy ?? string.Empty
                    };
                }
                return null;
            });
        }

        public async Task<PaginatedResponse<UserDto>> GetPaginatedAsync(PaginatedRequest request)
        {
            return await Task.Run(() =>
            {
                var allResult = _usUserService.GetAll();
                if (!allResult.IsSuccess || allResult.Data == null)
                {
                    return new PaginatedResponse<UserDto>
                    {
                        Items = new List<UserDto>(),
                        TotalRecords = 0,
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize
                    };
                }

                var query = allResult.Data.AsQueryable();

                // Search
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchLower = request.SearchTerm.ToLower();
                    query = query.Where(u =>
                        (u.Name != null && u.Name.ToLower().Contains(searchLower)) ||
                        (u.UserName != null && u.UserName.ToLower().Contains(searchLower)) ||
                        (u.Email != null && u.Email.ToLower().Contains(searchLower)) ||
                        (u.Phone != null && u.Phone.ToLower().Contains(searchLower))
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
                    _ => query.OrderBy(u => u.Id)
                };

                var totalRecords = query.Count();

                // Pagination
                var items = query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => new UserDto
                    {
                        Id = x.Id ?? string.Empty,
                        GroupId = x.GroupId ?? string.Empty,
                        Name = x.Name ?? string.Empty,
                        Gender = x.Gender,
                        UserName = x.UserName ?? string.Empty,
                        Email = x.Email,
                        Phone = x.Phone,
                        Cmnd = x.CMND,
                        Address = x.Address,
                        Image = x.Image,
                        Note = x.Note,
                        RowStatus = x.RowStatus,
                        CreatedAt = x.CreatedAt,
                        CreatedBy = x.CreatedBy ?? string.Empty,
                        UpdatedAt = x.UpdatedAt,
                        UpdatedBy = x.UpdatedBy ?? string.Empty
                    })
                    .ToList();

                return new PaginatedResponse<UserDto>
                {
                    Items = items,
                    TotalRecords = totalRecords,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };
            });
        }

        public async Task<UserDto> CreateAsync(CreateUserRequest request)
        {
            return await Task.Run(() =>
            {
                var model = new UsUserCreateModel
                {
                    GroupId = request.GroupId,
                    Name = request.Name,
                    Gender = request.Gender,
                    UserName = request.UserName,
                    Password = request.Password,
                    Email = request.Email,
                    Phone = request.Phone,
                    CMND = request.Cmnd,
                    Address = request.Address,
                    Note = request.Note
                };

                var result = _usUserService.Create(model);
                
                if (!result.IsSuccess)
                {
                    throw new InvalidOperationException(result.ErrorMessage);
                }

                if (result.Data == null)
                {
                    throw new InvalidOperationException("Tạo người dùng thất bại");
                }

                var x = result.Data;
                return new UserDto
                {
                    Id = x.Id ?? string.Empty,
                    GroupId = x.GroupId ?? string.Empty,
                    Name = x.Name ?? string.Empty,
                    Gender = x.Gender,
                    UserName = x.UserName ?? string.Empty,
                    Email = x.Email,
                    Phone = x.Phone,
                    Cmnd = x.CMND,
                    Address = x.Address,
                    Image = x.Image,
                    Note = x.Note,
                    RowStatus = x.RowStatus,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy ?? string.Empty,
                    UpdatedAt = x.UpdatedAt,
                    UpdatedBy = x.UpdatedBy ?? string.Empty
                };
            });
        }

        public async Task<UserDto> UpdateAsync(string id, UpdateUserRequest request)
        {
            return await Task.Run(() =>
            {
                var model = new UsUserUpdateModel
                {
                    Id = id,
                    GroupId = request.GroupId,
                    Name = request.Name,
                    Gender = request.Gender,
                    Email = request.Email,
                    Phone = request.Phone,
                    CMND = request.Cmnd,
                    Address = request.Address,
                    Image = request.Image,
                    Note = request.Note
                };

                var result = _usUserService.Update(model);
                
                if (!result.IsSuccess)
                {
                    if (result.ErrorMessage == MessageConstant.NOT_EXIST)
                    {
                        throw new KeyNotFoundException($"Không tìm thấy người dùng với ID: {id}");
                    }
                    throw new InvalidOperationException(result.ErrorMessage);
                }

                // Lấy lại thông tin sau khi update
                var getResult = _usUserService.GetById(id);
                if (getResult.IsSuccess && getResult.Data != null)
                {
                    var x = getResult.Data;
                    return new UserDto
                    {
                        Id = x.Id,
                        GroupId = x.GroupId,
                        Name = x.Name,
                        Gender = x.Gender,
                        UserName = x.UserName,
                        Email = x.Email,
                        Phone = x.Phone,
                        Cmnd = x.CMND,
                        Address = x.Address,
                        Image = x.Image,
                        Note = x.Note,
                        RowStatus = x.RowStatus,
                        CreatedAt = x.CreatedAt,
                        CreatedBy = x.CreatedBy,
                        UpdatedAt = x.UpdatedAt,
                        UpdatedBy = x.UpdatedBy
                    };
                }

                throw new InvalidOperationException("Cập nhật thất bại");
            });
        }

        public async Task DeleteAsync(string id)
        {
            await Task.Run(() =>
            {
                var result = _usUserService.Delete(id);
                
                if (!result.IsSuccess)
                {
                    if (result.ErrorMessage == MessageConstant.NOT_EXIST)
                    {
                        throw new KeyNotFoundException($"Không tìm thấy người dùng với ID: {id}");
                    }
                    throw new InvalidOperationException(result.ErrorMessage);
                }
            });
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            return await Task.Run(() =>
            {
                var result = _usUserService.Login(request.UserName, request.Password);
                
                if (!result.IsSuccess || result.Data == null)
                {
                    throw new InvalidOperationException(result.ErrorMessage ?? MessageConstant.USERNAME_PASSWORD_NOT_CORRECT);
                }

                var user = result.Data;

                // Tạo JWT Token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_config["JwtSettings:Key"] ?? throw new InvalidOperationException("JWT Key not configured"));
                
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim("FullName", user.Name ?? string.Empty),
                        new Claim("GroupId", user.GroupId ?? string.Empty)
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    Issuer = _config["JwtSettings:Issuer"],
                    Audience = _config["JwtSettings:Audience"],
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return new LoginResponse
                {
                    Token = tokenString,
                    UserId = user.Id ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    FullName = user.Name ?? string.Empty,
                    GroupId = user.GroupId ?? string.Empty
                };
            });
        }
    }
}

