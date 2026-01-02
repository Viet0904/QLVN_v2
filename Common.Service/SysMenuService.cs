using AutoMapper;
using Common.Database.Data;
using Common.Database.Entities;
using Common.Library.Constant;
using Common.Library.Helper;
using Common.Model.Common;
using Common.Model.SysMenu;
using Common.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Service
{
    public class SysMenuService : BaseService
    {
        public SysMenuService(QLVN_DbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        /// <summary>
        /// Lấy tất cả menu đang Active
        /// </summary>
        public ResModel<List<SysMenuViewModel>> GetAll()
        {
            ResModel<List<SysMenuViewModel>> res = new ResModel<List<SysMenuViewModel>>();

            var result = DbContext.SysMenus
                .Where(x => x.IsActive == 1)
                .OrderBy(x => x.ParentMenu)
                .ThenBy(x => x.Name)
                .ToList();
            
            res.Data = Mapper.Map<List<SysMenuViewModel>>(result);

            return res;
        }

        /// <summary>
        /// Lấy tất cả menu (bao gồm cả Inactive)
        /// </summary>
        public ResModel<List<SysMenuViewModel>> GetFull()
        {
            ResModel<List<SysMenuViewModel>> res = new ResModel<List<SysMenuViewModel>>();

            var result = DbContext.SysMenus
                .OrderBy(x => x.ParentMenu)
                .ThenBy(x => x.Name)
                .ToList();
            
            res.Data = Mapper.Map<List<SysMenuViewModel>>(result);

            return res;
        }

        /// <summary>
        /// Lấy menu theo Name (Primary Key)
        /// </summary>
        public ResModel<SysMenuViewModel> GetById(string name)
        {
            ResModel<SysMenuViewModel> res = new ResModel<SysMenuViewModel>();

            var result = DbContext.SysMenus
                .Where(x => x.Name == name)
                .FirstOrDefault();

            if (result != null)
                res.Data = Mapper.Map<SysMenuViewModel>(result);
            else
                res.ErrorMessage = MessageConstant.NOT_EXIST;

            return res;
        }

        /// <summary>
        /// Lấy danh sách menu con theo ParentMenu
        /// </summary>
        public ResModel<List<SysMenuViewModel>> GetByParent(string? parentMenu)
        {
            ResModel<List<SysMenuViewModel>> res = new ResModel<List<SysMenuViewModel>>();

            var result = DbContext.SysMenus
                .Where(x => x.ParentMenu == parentMenu && x.IsActive == 1)
                .OrderBy(x => x.Name)
                .ToList();
            
            res.Data = Mapper.Map<List<SysMenuViewModel>>(result);

            return res;
        }

        /// <summary>
        /// Lấy menu cấp 1 (ParentMenu = null)
        /// </summary>
        public ResModel<List<SysMenuViewModel>> GetRootMenus()
        {
            ResModel<List<SysMenuViewModel>> res = new ResModel<List<SysMenuViewModel>>();

            var result = DbContext.SysMenus
                .Where(x => x.ParentMenu == null && x.IsActive == 1)
                .OrderBy(x => x.Name)
                .ToList();
            
            res.Data = Mapper.Map<List<SysMenuViewModel>>(result);

            return res;
        }

        /// <summary>
        /// Tạo menu mới
        /// </summary>
        public ResModel<SysMenuViewModel> Create(SysMenuCreateModel model)
        {
            ResModel<SysMenuViewModel> res = new ResModel<SysMenuViewModel>();

            // Validate Name
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                res.ErrorMessage = MessageConstant.EXIST;
                return res;
            }

            // Validate Note
            if (string.IsNullOrWhiteSpace(model.Note))
            {
                res.ErrorMessage = MessageConstant.EXIST;
                return res;
            }

            // Kiểm tra trùng Name
            var existingMenu = DbContext.SysMenus
                .Where(x => x.Name == model.Name)
                .FirstOrDefault();

            if (existingMenu != null)
            {
                res.ErrorMessage = MessageConstant.EXIST;
                return res;
            }

            // Kiểm tra ParentMenu có tồn tại không
            if (!string.IsNullOrWhiteSpace(model.ParentMenu))
            {
                var parentMenu = DbContext.SysMenus
                    .Where(x => x.Name == model.ParentMenu)
                    .FirstOrDefault();

                if (parentMenu == null)
                {
                    res.ErrorMessage = MessageConstant.EXIST;
                    return res;
                }
            }

            try
            {
                var item = Mapper.Map<SysMenu>(model);

                // Set default IsActive = 1 nếu null
                if (!item.IsActive.HasValue)
                {
                    item.IsActive = 1;
                }

                DbContext.SysMenus.Add(item);
                DbContext.SaveChanges();

                res = GetById(item.Name);
                res.Message = MessageConstant.CREATED_SUCCESS;
            }
            catch (Exception e)
            {
                res.ErrorMessage = e.Message;
                ExceptionHelper.HandleException(e);
            }

            return res;
        }

        /// <summary>
        /// Cập nhật menu
        /// </summary>
        public ResModel<bool> Update(SysMenuUpdateModel model)
        {
            ResModel<bool> res = new ResModel<bool>();

            // Validate Name
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                res.ErrorMessage = MessageConstant.EXIST;
                return res;
            }

            // Validate Note
            if (string.IsNullOrWhiteSpace(model.Note))
            {
                res.ErrorMessage = MessageConstant.EXIST;
                return res;
            }

            try
            {
                var result = DbContext.SysMenus
                    .Where(x => x.Name == model.Name)
                    .FirstOrDefault();

                if (result != null)
                {
                    // Kiểm tra ParentMenu có tồn tại không
                    if (!string.IsNullOrWhiteSpace(model.ParentMenu))
                    {
                        var parentMenu = DbContext.SysMenus
                            .Where(x => x.Name == model.ParentMenu)
                            .FirstOrDefault();

                        if (parentMenu == null)
                        {
                            res.ErrorMessage = MessageConstant.EXIST;
                            return res;
                        }
                    }

                    // Map chỉ những field được phép update
                    result.ParentMenu = model.ParentMenu;
                    result.Note = model.Note;
                    result.Icon = model.Icon;
                    result.IsActive = model.IsActive;

                    DbContext.SaveChanges();
                    res.Data = true;
                    res.Message = MessageConstant.UPDATED_SUCCESS;
                }
                else
                {
                    res.ErrorMessage = MessageConstant.NOT_EXIST;
                }
            }
            catch (Exception e)
            {
                res.ErrorMessage = e.Message;
                ExceptionHelper.HandleException(e);
            }

            return res;
        }

        /// <summary>
        /// Xóa menu (Set IsActive = 0)
        /// </summary>
        public ResModel<bool> Delete(string name)
        {
            ResModel<bool> res = new ResModel<bool>();

            var result = DbContext.SysMenus
                .Where(x => x.Name == name)
                .FirstOrDefault();

            if (result != null)
            {
                // Kiểm tra có menu con không
                var hasChildren = DbContext.SysMenus
                    .Any(x => x.ParentMenu == name && x.IsActive == 1);

                if (hasChildren)
                {
                    res.ErrorMessage = "Không thể xóa menu có menu con.";
                    return res;
                }

                // Soft delete: Set IsActive = 0
                result.IsActive = 0;
                DbContext.SaveChanges();
                res.Data = true;
                res.Message = MessageConstant.DELETED_SUCCESS;
            }
            else
            {
                res.ErrorMessage = MessageConstant.NOT_EXIST;
            }

            return res;
        }

        /// <summary>
        /// Lấy cây menu (Parent + Children)
        /// </summary>
        public ResModel<List<SysMenuViewModel>> GetMenuTree()
        {
            ResModel<List<SysMenuViewModel>> res = new ResModel<List<SysMenuViewModel>>();

            var allMenus = DbContext.SysMenus
                .Where(x => x.IsActive == 1)
                .OrderBy(x => x.ParentMenu)
                .ThenBy(x => x.Name)
                .ToList();

            res.Data = Mapper.Map<List<SysMenuViewModel>>(allMenus);

            return res;
        }
    }
}

