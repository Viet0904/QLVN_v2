using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Common.Database.Data;
using Common.Database.Entities;
using Common.Library.Constant;
using Common.Model.Common;
using Common.Model.UsGroup;
using Common.Model.UsUser;
using Common.Model.UsUserPermission;
using Common.Service.Common;
using Common.Library.Helper;
using Microsoft.AspNetCore.Http;
namespace Common.Service
{
    public class UsGroupService : BaseService
    {
        //  Constructor nhận DbContext và IMapper
        public UsGroupService(QLVN_DbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(dbContext, mapper, httpContextAccessor)
        {
        }

        // Lấy tất cả nhóm người dùng Active
        public async Task<IEnumerable<UsGroupViewModel>> GetAll()
        {
            return await Task.Run(() =>
            {
                var groups = DbContext.UsGroups
                    .Where(g => g.RowStatus == RowStatusConstant.Active)
                    .ToList();
                return Mapper.Map<IEnumerable<UsGroupViewModel>>(groups);
            });
        }
        // lấy ra tất cả nhóm người dùng cả Active và Deleted
        public async Task<IEnumerable<UsGroupViewModel>> GetFull(){
            return await Task.Run(() =>{
                var groups = DbContext.UsGroups.ToList();
                return Mapper.Map<IEnumerable<UsGroupViewModel>>(groups);
            });
        }

        // Lấy ra nhóm người dùng theo Id
        public async Task<UsGroupViewModel> GetById(string id){
            return await Task.Run(() =>{
                var group = DbContext.UsGroups.Where(g => g.Id == id).FirstOrDefault();
                return Mapper.Map<UsGroupViewModel>(group);
            });
        }
        // Tạo nhóm người dùng
        public async Task<UsGroupViewModel> Create(UsGroupCreateModel model)
        {
            var result = await DbContext.UsGroups.Where(x => x.Name == model.Name && x.RowStatus == RowStatusConstant.Active).FirstOrDefaultAsync();
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
                        var item = Mapper.Map<UsGroup>(model);
                        item.CreatedAt = DateTime.Now;
                        item.UpdatedAt = DateTime.Now;
                        string currentUserId = GetCurrentUserId();
                        item.CreatedBy = currentUserId;
                        item.UpdatedBy = currentUserId;
                        item.RowStatus = RowStatusConstant.Active;
                        int lengthGroup = 3;
                    generateId:
                        item.Id = GenerateId(DefaultCodeConstant.UsGroup.Name, lengthGroup);

                        var resultIdExist = await DbContext.UsGroups.Where(x => x.Id == item.Id).FirstOrDefaultAsync();
                        if (resultIdExist != null)
                            goto generateId;

                        await DbContext.UsGroups.AddAsync(item);
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
        // Update nhóm người dùng, trả về lại thông tin người dùng để Blazor cập nhật lại UI, mà không cần refresh lại trang và chỉ reload lại 1 dòng 
        public async Task<UsGroupViewModel> Update(UsGroupUpdateModel model)
        {
            try
            {
                var result = await DbContext.UsGroups.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
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

        // Delete nhóm người dùng , xoá mềm chuyển rowstatus thành deleted
         public async Task<bool> Delete (string id){
            try
            {
                var result = await DbContext.UsGroups.Where(x => x.Id == id).FirstOrDefaultAsync();
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
            catch (Exception e)
            {
                ExceptionHelper.HandleException(e);
                throw;
            }
         }
        // Lấy danh sách nhóm người dùng theo phân trang
        public async Task<PaginatedResponse<UsGroupViewModel>> GetPaginated(PaginatedRequest request)
        {
            var query = DbContext.UsGroups.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchLower = request.SearchTerm.ToLower();
                query = query.Where(g =>
                    g.Name.ToLower().Contains(searchLower) ||
                    (g.Note != null && g.Note.ToLower().Contains(searchLower))
                );
            }

            // Sort
            query = request.SortColumn?.ToLower() switch
            {
                "name" => request.SortDirection == "desc"
                    ? query.OrderByDescending(g => g.Name)
                    : query.OrderBy(g => g.Name),
                "createdat" => request.SortDirection == "desc"
                    ? query.OrderByDescending(g => g.CreatedAt)
                    : query.OrderBy(g => g.CreatedAt),
                _ => query.OrderBy(g => g.Id)
            };

            var totalRecords = await query.CountAsync();

            // Pagination
            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PaginatedResponse<UsGroupViewModel>
            {
                Items = Mapper.Map<List<UsGroupViewModel>>(items),
                TotalRecords = totalRecords,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
        // Lấy danh sách người dùng theo nhóm người dùng
        public async Task<IEnumerable<UsUserViewModel>> GetUsersByGroupId(string groupId)
        {
            var users = await DbContext.UsUsers
                .Where(u => u.GroupId == groupId && u.RowStatus == RowStatusConstant.Active)
                .ToListAsync();
            return Mapper.Map<List<UsUserViewModel>>(users);
        }
        // Lấy danh sách quyền của người dùng theo nhóm người dùng
        public async Task<IEnumerable<UsUserPermissionViewModel>> GetPermissionMatrix(string groupId)
        {
            var users = await DbContext.UsUsers
                .Where(u => u.GroupId == groupId && u.RowStatus == RowStatusConstant.Active)
                .ToListAsync();

            if (!users.Any()) return new List<UsUserPermissionViewModel>();

            var menus = await DbContext.SysMenus
                .Where(m => m.IsActive == 1)
                .ToListAsync();

            var userIds = users.Select(u => u.Id).ToList();
            var existingPermissions = await DbContext.UsUserPermissions
                .Where(p => userIds.Contains(p.UserId))
                .ToListAsync();

            var result = new List<UsUserPermissionViewModel>();

            foreach (var menu in menus)
            {
                foreach (var user in users)
                {
                    var p = existingPermissions.FirstOrDefault(x => x.UserId == user.Id && x.MenuId == menu.Name);
                    result.Add(new UsUserPermissionViewModel
                    {
                        UserId = user.Id,
                        MenuId = menu.Name,
                        MenuName = !string.IsNullOrEmpty(menu.Note) ? menu.Note : menu.Name,
                        Xem = p?.Xem ?? false,
                        Them = p?.Them ?? false,
                        Sua = p?.Sua ?? false,
                        SuaHangLoat = p?.SuaHangLoat ?? false,
                        Xoa = p?.Xoa ?? false,
                        XoaHangLoat = p?.XoaHangLoat ?? false,
                        XuatDuLieu = p?.XuatDuLieu ?? false,
                        Khac = p?.Khac ?? false
                    });
                }
            }

            return result;
        }
        // Cập nhật quyền của người dùng
        public async Task<bool> UpdatePermissions(IEnumerable<UsUserPermissionViewModel> models)
        {
            var strategy = DbContext.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await DbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        foreach (var model in models)
                        {
                            var permission = await DbContext.UsUserPermissions
                                .FirstOrDefaultAsync(p => p.UserId == model.UserId && p.MenuId == model.MenuId);

                            if (permission != null)
                            {
                                permission.Xem = model.Xem;
                                permission.Them = model.Them;
                                permission.Sua = model.Sua;
                                permission.SuaHangLoat = model.SuaHangLoat;
                                permission.Xoa = model.Xoa;
                                permission.XoaHangLoat = model.XoaHangLoat;
                                permission.XuatDuLieu = model.XuatDuLieu;
                                permission.Khac = model.Khac;
                                permission.UpdatedAt = DateTime.Now;
                            }
                        }

                        await DbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return true;
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            });
        }
    }
}