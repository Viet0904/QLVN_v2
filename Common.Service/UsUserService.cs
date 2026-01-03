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

        public async Task<IEnumerable<UsUserViewModel>> GetAll()
        {
            var result = await DbContext.UsUsers.Where(x => x.RowStatus == RowStatusConstant.Active).ToListAsync();
            return Mapper.Map<List<UsUserViewModel>>(result);
        }

        public async Task<IEnumerable<UsUserViewModel>> GetFull()
        {
            var result = await DbContext.UsUsers.ToListAsync();
            return Mapper.Map<List<UsUserViewModel>>(result);
        }

        public async Task<UsUserViewModel> GetById(string id)
        {
            var result = await DbContext.UsUsers.Where(x => x.Id == id).FirstOrDefaultAsync();
            return Mapper.Map<UsUserViewModel>(result);
        }

        public async Task<UsUserViewModel> Login(string userName, string password)
        {
            password = PasswordHelper.CreatePassword(password);

            var result = await DbContext.UsUsers.Where(x => x.UserName == userName && x.Password == password && x.RowStatus == RowStatusConstant.Active).FirstOrDefaultAsync();
            if (result != null) return Mapper.Map<UsUserViewModel>(result);
            
            throw new Exception(MessageConstant.NOT_CORRECT);
        }

        public async Task<UsUserViewModel> Create(UsUserCreateModel model)
        {
            var result = await DbContext.UsUsers.Where(x => x.UserName == model.UserName && x.RowStatus == RowStatusConstant.Active).FirstOrDefaultAsync();
            if (result != null)
            {
                throw new Exception(MessageConstant.EXIST);
            }

            var strategy = DbContext.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await DbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var item = Mapper.Map<UsUser>(model);

                    generateId:
                        item.Id = item.GroupId + GenerateId(DefaultCodeConstant.UsUser.Name, DefaultCodeConstant.UsUser.Length);

                        var resultIdExist = await DbContext.UsUsers.Where(x => x.Id == item.Id).FirstOrDefaultAsync();
                        if (resultIdExist != null)
                            goto generateId;

                        item.Password = CryptorEngineHelper.Encrypt(model.Password);
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
                    Mapper.Map(model, result);
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
        public async Task<PaginatedResponse<UsUserViewModel>> GetPaginated(PaginatedRequest request)
        {
            var allResult = await GetAll();
            var query = allResult.AsQueryable();

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
                .ToList();

            return new PaginatedResponse<UsUserViewModel>
            {
                Items = items,
                TotalRecords = totalRecords,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

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
        }
    }
}
