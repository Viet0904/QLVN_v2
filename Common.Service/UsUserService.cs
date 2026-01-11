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
                        if (string.IsNullOrEmpty(model.GroupId))
                        {
                            throw new Exception(MessageConstant.GROUP_NOT_NULL);
                        }

                        var item = new UsUser();
                        item.Name = model.Name;
                        item.UserName = model.UserName;
                        item.GroupId = model.GroupId;
                        item.Email = model.Email;
                        item.Phone = model.Phone;
                        item.Gender = model.Gender;
                        item.CMND = model.CMND;
                        item.Address = model.Address;
                        item.Note = model.Note;
                        item.RowStatus = model.RowStatus;
                        // FIX: Khởi tạo Theme mặc định để tránh lỗi NOT NULL
                        item.Theme = string.Empty;

                    generateId:
                        string randomPart = GenerateId(DefaultCodeConstant.UsUser.Name, DefaultCodeConstant.UsUser.Length);
                        string generatedId = item.GroupId + randomPart;

                        // nếu Id dài hơn 8 ký tự thì cắt bớt
                        if (generatedId.Length > 8)
                        {
                            
                            generatedId = generatedId.Substring(generatedId.Length - 8);
                        }
                        item.Id = generatedId;

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
                    result.CMND = model.CMND;
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
        //public async Task<PaginatedResponse<UsUserViewModel>> GetPaginated(PaginatedRequest request)
        //{
        //    // Sử dụng IQueryable để query trực tiếp tại DB, tránh load toàn bộ vào RAM
        //    // Thêm AsNoTracking để tăng tốc độ thực thi
        //    var query = DbContext.UsUsers.AsNoTracking()
        //        .Where(x => x.RowStatus == RowStatusConstant.Active);

        //    // Search nâng cao: Tìm kiếm trên cả User và tên Group
        //    if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        //    {
        //        var searchLower = request.SearchTerm.ToLower();
        //        query = query.Where(u =>
        //            (u.Id != null && u.Id.Contains(searchLower)) ||
        //            (u.Name != null && u.Name.ToLower().Contains(searchLower)) ||
        //            (u.UserName != null && u.UserName.ToLower().Contains(searchLower)) ||
        //            (u.Email != null && u.Email.ToLower().Contains(searchLower)) ||
        //            (u.Phone != null && u.Phone.ToLower().Contains(searchLower)) ||
        //            (u.Group.Name != null && u.Group.Name.ToLower().Contains(searchLower)) // Search trực tiếp trên bảng Join

        //        );
        //    }

        //    // Sorting
        //    query = request.SortColumn?.ToLower() switch
        //    {
        //        "id" => request.SortDirection == "desc" ? query.OrderByDescending(u => u.Id) : query.OrderBy(u => u.Id),
        //        "name" => request.SortDirection == "desc" ? query.OrderByDescending(u => u.Name) : query.OrderBy(u => u.Name),
        //        "username" => request.SortDirection == "desc" ? query.OrderByDescending(u => u.UserName) : query.OrderBy(u => u.UserName),
        //        "email" => request.SortDirection == "desc" ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
        //        "groupname" => request.SortDirection == "desc" ? query.OrderByDescending(u => u.Group.Name) : query.OrderBy(u => u.Group.Name),
        //        "createdat" => request.SortDirection == "desc" ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
        //        "createdby" => request.SortDirection == "desc" ? query.OrderByDescending(u => u.CreatedBy) : query.OrderBy(u => u.CreatedBy),
        //        "updatedat" => request.SortDirection == "desc" ? query.OrderByDescending(u => u.UpdatedAt) : query.OrderBy(u => u.UpdatedAt),
        //        "updatedby" => request.SortDirection == "desc" ? query.OrderByDescending(u => u.UpdatedBy) : query.OrderBy(u => u.UpdatedBy),
        //        "GroupId" => request.SortDirection == "desc" ? query.OrderByDescending(u => u.GroupId) : query.OrderBy(u => u.GroupId),


        //        _ => query.OrderByDescending(u => u.CreatedAt)
        //    };

        //    var totalRecords = await query.CountAsync();

        //    // Projection: Chỉ lấy những gì cần thiết nhất để giảm tải (bỏ qua Image lớn)
        //    var items = await query
        //        .Skip((request.PageNumber - 1) * request.PageSize)
        //        .Take(request.PageSize)
        //        .Select(u => new UsUserViewModel
        //        {
        //            Id = u.Id,
        //            Name = u.Name,
        //            UserName = u.UserName,
        //            Email = u.Email,
        //            Phone = u.Phone,
        //            CMND = u.CMND,
        //            Address = u.Address,
        //            Note = u.Note,
        //            Gender = u.Gender,
        //            GroupId = u.GroupId,
        //            GroupName = u.Group.Name, // EF Core tự động sinh INNER JOIN cực tối ưu
        //            RowStatus = u.RowStatus,
        //            CreatedAt = u.CreatedAt,
        //            CreatedBy = u.CreatedBy,
        //            UpdatedBy = u.UpdatedBy,
        //            UpdatedAt = u.UpdatedAt
        //        })
        //        .ToListAsync();

        //    // Xử lý mapping tên người tạo/sửa (Vẫn giữ logic tối ưu này)
        //    if (items.Any())
        //    {

        //        // Lấy ra danh sách userId duy nhất từ CreatedBy và UpdatedBy
        //        var userIds = items.SelectMany(u => new[] { u.CreatedBy, u.UpdatedBy })
        //                           .Where(id => !string.IsNullOrEmpty(id))
        //                           .Distinct()
        //                           .ToList();
        //        // Lấy tên người dùng từ bảng UsUsers dựa trên danh sách userId
        //        if (userIds.Any())
        //        {
        //            // Tạo dictionary để tra cứu nhanh
        //            var userNames = await DbContext.UsUsers.AsNoTracking()
        //                                   .Where(u => userIds.Contains(u.Id))
        //                                   .Select(u => new { u.Id, u.Name })
        //                                   .ToDictionaryAsync(u => u.Id, u => u.Name);


        //            // Gán tên người tạo và người cập nhật vào từng item
        //            foreach (var item in items)
        //            {
        //                if (!string.IsNullOrEmpty(item.CreatedBy) && userNames.TryGetValue(item.CreatedBy, out var cName))
        //                    item.CreatedName = cName;

        //                if (!string.IsNullOrEmpty(item.UpdatedBy) && userNames.TryGetValue(item.UpdatedBy, out var uName))
        //                    item.UpdatedName = uName;
        //            }
        //        }
        //    }

        //    return new PaginatedResponse<UsUserViewModel>
        //    {
        //        Items = items,
        //        TotalRecords = totalRecords,
        //        PageNumber = request.PageNumber,
        //        PageSize = request.PageSize
        //    };
        //}
        public async Task<PaginatedResponse<UsUserViewModel>> GetPaginated(PaginatedRequest request)
        {
            // 1. Khởi tạo query với AsNoTracking để tối ưu hiệu năng đọc
            var query = DbContext.UsUsers.AsNoTracking()
                .Where(x => x.RowStatus == RowStatusConstant.Active);

            // 2. Search nâng cao - Tối ưu hóa với EF.Functions.Like
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchPattern = $"%{request.SearchTerm.Trim()}%";
                query = query.Where(u =>
                    EF.Functions.Like(u.Id, searchPattern) ||
                    EF.Functions.Like(u.Name, searchPattern) ||
                    EF.Functions.Like(u.UserName, searchPattern) ||
                    (u.Email != null && EF.Functions.Like(u.Email, searchPattern)) ||
                    (u.Phone != null && EF.Functions.Like(u.Phone, searchPattern)) ||
                    EF.Functions.Like(u.Group.Name, searchPattern)
                );
            }

            // 3. Sorting (Sử dụng switch expression - C# 8.0+)
            query = request.SortColumn?.ToLower() switch
            {
                "id" => request.SortDirection == "desc" ? query.OrderByDescending(u => u.Id) : query.OrderBy(u => u.Id),
                "name" => request.SortDirection == "desc" ? query.OrderByDescending(u => u.Name) : query.OrderBy(u => u.Name),
                "username" => request.SortDirection == "desc" ? query.OrderByDescending(u => u.UserName) : query.OrderBy(u => u.UserName),
                "email" => request.SortDirection == "desc" ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                "groupname" => request.SortDirection == "desc" ? query.OrderByDescending(u => u.Group.Name) : query.OrderBy(u => u.Group.Name),
                "createdat" => request.SortDirection == "desc" ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
                "updatedat" => request.SortDirection == "desc" ? query.OrderByDescending(u => u.UpdatedAt) : query.OrderBy(u => u.UpdatedAt),
                "groupid" => request.SortDirection == "desc" ? query.OrderByDescending(u => u.GroupId) : query.OrderBy(u => u.GroupId),
                _ => query.OrderByDescending(u => u.CreatedAt) // Mặc định sắp xếp theo ngày tạo mới nhất
            };

            // 4. Lấy tổng số bản ghi trước khi phân trang
            var totalRecords = await query.CountAsync();

            // 5. Projection & Pagination
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
                    CMND = u.CMND,
                    Address = u.Address,
                    Note = u.Note,
                    Gender = u.Gender,
                    GroupId = u.GroupId,
                    GroupName = u.Group != null ? u.Group.Name : string.Empty,
                    RowStatus = u.RowStatus,
                    CreatedAt = u.CreatedAt,
                    CreatedBy = u.CreatedBy,
                    UpdatedBy = u.UpdatedBy,
                    UpdatedAt = u.UpdatedAt
                })
                .ToListAsync();

            // 6. Xử lý Batch Mapping cho CreatedName và UpdatedName
            //if (items.Count != 0)
            //{
            //    // Gom tất cả ID người tạo và người sửa vào 1 HashSet để lọc trùng và loại bỏ null
            //    var userIdsForLookup = items.Select(u => u.CreatedBy)
            //        .Concat(items.Select(u => u.UpdatedBy))
            //        .Where(id => !string.IsNullOrEmpty(id))
            //        .Distinct()
            //        .ToList();

            //    if (userIdsForLookup.Count != 0)
            //    {
            //        // Truy vấn 1 lần duy nhất để lấy Name mapping với ID
            //        var userNamesDict = await DbContext.UsUsers.AsNoTracking()
            //            .Where(u => userIdsForLookup.Contains(u.Id))
            //            .Select(u => new { u.Id, u.Name })
            //            .ToDictionaryAsync(u => u.Id, u => u.Name);

                    
            //        foreach (var item in items)
            //        {
            //            if (!string.IsNullOrEmpty(item.CreatedBy) && userNamesDict.TryGetValue(item.CreatedBy, out var createdName))
            //            {
            //                item.CreatedName = createdName;
            //            }

            //            if (!string.IsNullOrEmpty(item.UpdatedBy) && userNamesDict.TryGetValue(item.UpdatedBy, out var updatedName))
            //            {
            //                item.UpdatedName = updatedName;
            //            }
            //        }
            //    }
            //}

            // 7. Trả về kết quả
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
