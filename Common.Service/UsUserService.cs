using AutoMapper;
using Common.Database.Data;
using Common.Database.Entities;
using Common.Library.Constant;
using Common.Library.Helper;
using Common.Model.Auth;
using Common.Model.Common;
using Common.Model.UsUser;
using Common.Service.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Common.Service
{
    public class UsUserService : BaseService
    {
        private readonly IConfiguration _config;

        //  Constructor nhận DbContext, IMapper và IConfiguration
        public UsUserService(QLVN_DbContext dbContext, IMapper mapper, IConfiguration config, IHttpContextAccessor httpContextAccessor)
            : base(dbContext, mapper, httpContextAccessor)
        {
            _config = config;
        }
        // Lấy tất cả người dùng đang hoạt động
        public async Task<IEnumerable<UsUserViewModel>> GetAll()
        {
            var result = await DbContext.UsUsers.Where(x => x.RowStatus == RowStatusConstant.Active).ToListAsync();
            return Mapper.Map<List<UsUserViewModel>>(result);
        }
        // Lấy tất cả người dùng cả đang hoạt động và ngừng hoạt động
        public async Task<IEnumerable<UsUserViewModel>> GetFull()
        {
            var result = await DbContext.UsUsers.ToListAsync();
            return Mapper.Map<List<UsUserViewModel>>(result);
        }
        // Lấy thông tin người dùng theo id
        public async Task<UsUserViewModel> GetById(string id)
        {
            var result = await DbContext.UsUsers.Where(x => x.Id == id).FirstOrDefaultAsync();
            return Mapper.Map<UsUserViewModel>(result);
        }
        // Login người dùng
        public async Task<UsUserViewModel> Login(string userName, string password)
        {
            password = PasswordHelper.CreatePassword(password);

            var result = await DbContext.UsUsers.Where(x => x.UserName == userName && x.Password == password && x.RowStatus == RowStatusConstant.Active).FirstOrDefaultAsync();
            if (result != null) return Mapper.Map<UsUserViewModel>(result);
            
            throw new Exception(MessageConstant.NOT_CORRECT);
        }
        // Tạo người dùng
        public async Task<UsUserViewModel> Create(UsUserCreateModel model)
        {
            var result = await DbContext.UsUsers.Where(x => x.UserName == model.UserName && x.RowStatus == RowStatusConstant.Active).FirstOrDefaultAsync();
            if (result != null)
            {
                throw new Exception(MessageConstant.EXIST);
            }
            // CreateExecutionStrategy là tính năng của EF Core để retry khi có lỗi xảy ra (Connection Resiliency)
            var strategy = DbContext.Database.CreateExecutionStrategy();
            // strategy.ExecuteAsync là phương thức để thực hiện transaction. nếu có lỗi (ví dụ: mất kết nối mạng) xảy ra thì sẽ retry lại
            return await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await DbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var item = Mapper.Map<UsUser>(model);

                    generateId:
                        item.Id = item.GroupId + GenerateId(DefaultCodeConstant.UsUser.Name, DefaultCodeConstant.UsUser.Length);

                        var resultIdExist = await DbContext.UsUsers.AsNoTracking().Where(x => x.Id == item.Id).FirstOrDefaultAsync();
                        if (resultIdExist != null)
                            goto generateId;

                        item.Password = PasswordHelper.CreatePassword(model.Password);
                        item.CreatedAt = DateTime.Now;
                        item.UpdatedAt = DateTime.Now;
                        string currentUserId = GetCurrentUserId();
                        item.CreatedBy = currentUserId;
                        item.UpdatedBy = currentUserId;

                        await DbContext.UsUsers.AddAsync(item);
                        await DbContext.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return await GetById(item.Id);
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            });
        }
        // Update người dùng, trả về lại thông tin người dùng để Blazor cập nhật lại UI, mà không cần refresh lại trang và chỉ reload lại 1 dòng 
        public async Task<UsUserViewModel> Update(UsUserUpdateModel model)
        {
            try
            {
                var result = await DbContext.UsUsers.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (result != null)
                {
                    // FIX: Manual mapping to avoid AutoMapper resetting CreatedAt/CreatedBy to null/default
                    result.Name = model.Name;
                    result.UserName = model.UserName;
                    result.GroupId = model.GroupId;
                    result.Email = model.Email;
                    result.Phone = model.Phone;
                    result.Gender = model.Gender;
                    result.Cmnd = model.CMND;
                    result.Address = model.Address;
                    result.Note = model.Note;
                    result.RowStatus = model.RowStatus;
                    
                    // Only update Image if it's provided (not null)
                    if (model.Image != null) 
                    {
                        result.Image = model.Image;
                    }

                    if (model.IsChangePassword == true && !string.IsNullOrEmpty(model.Password)) 
                    {
                        result.Password = PasswordHelper.CreatePassword(model.Password);
                    }

                    result.UpdatedAt = DateTime.Now;
                    result.UpdatedBy = GetCurrentUserId();
                    
                    await DbContext.SaveChangesAsync();
                    return await GetById(model.Id);
                }
                else
                {
                    throw new Exception(MessageConstant.NOT_EXIST);
                }
            }
            catch (Exception e)
            {
                ExceptionHelper.HandleException(e);
                throw;
            }
        }
        // Delete người dùng chuyển trạng thái thành ngừng hoạt động (RowStatus = 2)
        public async Task<bool> Delete(string id)
        {
            var result = await DbContext.UsUsers.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (result != null)
            {
                result.RowStatus = RowStatusConstant.Deleted;
                await DbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new Exception(MessageConstant.NOT_EXIST);
            }
        }
        // Lấy danh sách người dùng theo phân trang
        public async Task<PaginatedResponse<UsUserViewModel>> GetPaginated(PaginatedRequest request)
        {
            // Sử dụng IQueryable để query trực tiếp tại DB, tránh load toàn bộ vào RAM
            // Thêm AsNoTracking để tăng tốc độ read
            var query = DbContext.UsUsers.AsNoTracking()
                .Where(x => x.RowStatus == RowStatusConstant.Active);

            // Search - Thực hiện tại server (DB)
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

            // Sort - Thực hiện tại server (DB)
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
                _ => query.OrderByDescending(u => u.CreatedAt) // Mặc định sắp xếp theo ngày tạo mới nhất
            };

            var totalRecords = await query.CountAsync();

            // Pagination & Projection - Thực hiện tại server (DB)
            // QUAN TRỌNG: Sử dụng Select() trực tiếp để chỉ lấy các trường cần thiết, tránh N+1 và load dữ liệu thừa (Image, text dài...)
            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(u => new UsUserViewModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    UserName = u.UserName,
                    Email = u.Email,
                    Phone = u.Phone,
                    CMND = u.Cmnd,
                    Address = u.Address,
                    Note = u.Note,
                    Gender = u.Gender,
                    // Image = u.Image, // FIX: Don't load Image for list view to improve performance (20s -> <1s)
                    Theme = u.Theme,
                    GroupId = u.GroupId,
                    GroupName = u.Group.Name, // Join tự động qua Navigation Property
                    RowStatus = u.RowStatus,
                    CreatedAt = u.CreatedAt,
                    CreatedBy = u.CreatedBy,
                    UpdatedBy = u.UpdatedBy,
                    // Không cần map CreatedName/UpdatedName ở đây nếu không thực sự cần thiết cho List, 
                    // hoặc nếu cần thì phải join thêm bảng UsUsers (self-join)
                    
                    // Nếu cần CreatedName/UpdatedName, ta có thể xử lý sau khi lấy về memory 
                    // hoặc dùng sub-query (nhưng phức tạp hơn cho EF)
                })
                .ToListAsync();

            // Xử lý mapping tên người tạo/sửa (Optional - nếu cần thiết thì giữ logic cũ nhưng áp dụng lên items đã select)
            if (items.Any())
            {
                var userIds = items.SelectMany(u => new[] { u.CreatedBy, u.UpdatedBy })
                                   .Where(id => !string.IsNullOrEmpty(id))
                                   .Distinct()
                                   .ToList();

                if (userIds.Any())
                {
                    var userNames = await DbContext.UsUsers.AsNoTracking()
                                           .Where(u => userIds.Contains(u.Id))
                                           .Select(u => new { u.Id, u.Name })
                                           .ToDictionaryAsync(u => u.Id, u => u.Name);

                    foreach (var item in items)
                    {
                        if (!string.IsNullOrEmpty(item.CreatedBy) && userNames.TryGetValue(item.CreatedBy, out var cName))
                            item.CreatedName = cName;
                        if (!string.IsNullOrEmpty(item.UpdatedBy) && userNames.TryGetValue(item.UpdatedBy, out var uName))
                            item.UpdatedName = uName;
                    }
                }
            }

            return new PaginatedResponse<UsUserViewModel>
            {
                Items = items,
                TotalRecords = totalRecords,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
        // Login người dùng và tạo token
        public async Task<LoginResponse> LoginWithToken(LoginRequest request)
        {
            var user = await Login(request.UserName, request.Password);

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
                // Token hết hạn trong 1 giờ
                Expires = DateTime.UtcNow.AddHours(1),
                // Tạo token issuer
                Issuer = _config["JwtSettings:Issuer"],
                // Tạo người nhận token
                Audience = _config["JwtSettings:Audience"],
                // Tạo signature
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            // Tạo token
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
        }
    }
}
