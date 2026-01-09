using AutoMapper;
using Common.Database.Data;
using Common.Database.Entities;
using Common.Library.Constant;
using Common.Library.Helper;
using Common.Model.Common;
using Common.Model.Dvsd;
using Common.Service.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Service
{
    public class DvsdService : BaseService
    {
        public DvsdService(QLVN_DbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(dbContext, mapper, httpContextAccessor)
        {
        }


        public async Task<IEnumerable<DvsdViewModel>> GetFullAsync()
        {
            var result = await DbContext.DbDvsds.ToListAsync();
            return Mapper.Map<List<DvsdViewModel>>(result);
        }

        /// <summary>
        /// Lấy tất cả đơn vị sd Active
        /// </summary>
        public async Task<IEnumerable<object>> GetAllAsync()
        {
            return await Task.Run(() =>
            {
                var list = DbContext.DbDvsds
                    .Where(x => x.RowStatus == RowStatusConstant.Active)
                    .ToList();
                return list;
            });
        }

        // Tìm bằng mã
        public async Task<DvsdViewModel> GetByMaAsync(string Ma)
        {
            var result = await DbContext.DbDvsds.Where(x => x.Ma == Ma).FirstOrDefaultAsync();
            return Mapper.Map<DvsdViewModel>(result);
        }

        // Tìm bằng tên
        public async Task<DvsdViewModel> GetByTen(String Ten)
        {
            var result = DbContext.DbDvsds.Where(x => x.Ten == Ten).FirstOrDefaultAsync();
            return Mapper.Map<DvsdViewModel>(result);
        }



        // Tạo đơn vị sử dụng
        public async Task<DvsdViewModel> CreateDvsd(DvsdCreateModel dvsdCreateModel)
        {
            var existed = await DbContext.DbDvsds.Where(x => (x.Ten == dvsdCreateModel.Ten ||
                                                             x.Cccd == dvsdCreateModel.Cccd ||
                                                             x.Mst == dvsdCreateModel.Mst) && x.RowStatus == RowStatusConstant.Active
                                                        ).FirstOrDefaultAsync();
            if (existed != null)
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
                        var item = new DbDvsd();
                        item.Ten = dvsdCreateModel.Ten;
                        item.DiaChi = dvsdCreateModel.DiaChi;
                        item.Phone = dvsdCreateModel.Phone;
                        item.Cccd = dvsdCreateModel.Cccd;
                        item.Mst = dvsdCreateModel.Mst;
                        item.Stk = dvsdCreateModel.Stk;
                        item.Note = dvsdCreateModel.Note;
                        item.RowStatus = RowStatusConstant.Active;
                    generateId:
                        string randomPart = GenerateId(DefaultCodeConstant.DbDvsd.Name, DefaultCodeConstant.DbDvsd.Length);
                        string generatedId = item.Phone + randomPart;

                        if (generatedId.Length > 8)
                        {

                            generatedId = generatedId.Substring(generatedId.Length - 8);
                        }
                        item.Ma = generatedId;
                        var resultIdExist = await DbContext.DbDvsds.AsNoTracking().Where(x => x.Ma == item.Ma).FirstOrDefaultAsync();
                        if (resultIdExist != null)
                            goto generateId;

                        item.CreatedAt = DateTime.Now;
                        item.UpdatedAt = DateTime.Now;
                        string currentUserId = GetCurrentUserId();
                        item.CreatedBy = currentUserId;
                        item.UpdatedBy = currentUserId;

                        await DbContext.DbDvsds.AddAsync(item);
                        await DbContext.SaveChangesAsync();

                        await transaction.CommitAsync();
                        return await GetByMaAsync(item.Ma);

                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            });
        }

        // Update dvsd
        public async Task<DvsdViewModel> UpdateDvsd(DvsdUpdateModel dvsdUpdateModel)
        {
            try
            {
                var existed = await DbContext.DbDvsds.Where(x => (x.Cccd == dvsdUpdateModel.Cccd ||
                                                              x.Mst == dvsdUpdateModel.Mst ||
                                                              x.Ten == dvsdUpdateModel.Ten) && x.RowStatus == RowStatusConstant.Active)
                .FirstOrDefaultAsync();
                if (existed != null)
                {
                    var strategy = DbContext.Database.CreateExecutionStrategy();

                    

                    existed.Ten = dvsdUpdateModel.Ten;
                    existed.DiaChi = dvsdUpdateModel.DiaChi;
                    existed.Phone = dvsdUpdateModel.Phone;
                    existed.Cccd = dvsdUpdateModel.Cccd;
                    existed.Mst = dvsdUpdateModel.Mst;
                    existed.Stk = dvsdUpdateModel.Stk;
                    existed.RowStatus = dvsdUpdateModel.RowStatus;

                    existed.UpdatedAt = DateTime.Now;
                    existed.UpdatedBy = GetCurrentUserId();

                    await DbContext.SaveChangesAsync();

                    return await GetByMaAsync(existed.Ma);
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


        // delete 
        public async Task<bool> DeleteDvsd (string Ma)
        {
            var existed = await DbContext.DbDvsds.FirstOrDefaultAsync(x => x.Ma == Ma);
            if (existed != null)
            {
                existed.RowStatus = RowStatusConstant.Deleted;
                await DbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new Exception(MessageConstant.NOT_EXIST);
            }
        }


    }
}