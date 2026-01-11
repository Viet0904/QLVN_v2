using AutoMapper;
using Common.Database.Data;
using Common.Database.Entities;
using Common.Library.Constant;
using Common.Model.NhaCungCap;
using Common.Service.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Common.Service;

public class NhaCungCapService : BaseService
{
    public NhaCungCapService(QLVN_DbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(dbContext, mapper, httpContextAccessor)
    {

    }

    // get all
    public async Task<IEnumerable<NhaCungCapViewModel>> GetAll()
    {
        var results = await DbContext.DbNhaCungCaps.Where(x => x.RowStatus == RowStatusConstant.Active).ToListAsync();
        return Mapper.Map<List<NhaCungCapViewModel>>(results);
    }

    // get full
    public async Task<IEnumerable<NhaCungCapViewModel>> GetFull()
    {
        return Mapper.Map<List<NhaCungCapViewModel>>(
            await DbContext.DbNhaCungCaps.ToListAsync()
        );
    }

    // get by ID
    public async Task<NhaCungCapViewModel> GetById(string Ma)
    {
        try
        {
            var result = await DbContext.DbNhaCungCaps.FirstOrDefaultAsync(x => x.Ma == Ma && x.RowStatus == RowStatusConstant.Active);
            if (result != null)
            {
                return Mapper.Map<NhaCungCapViewModel>(result);
            }
            else
            {
                throw new Exception(MessageConstant.NOT_EXIST);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    public async Task<NhaCungCapViewModel> GetByTen(string Ten)
    {
        try
        {
            var result = await DbContext.DbNhaCungCaps.FirstOrDefaultAsync(x => x.Ten == Ten && x.RowStatus == RowStatusConstant.Active);
            if (result != null)
            {
                return Mapper.Map<NhaCungCapViewModel>(result);
            }
            else
            {
                throw new Exception(MessageConstant.NOT_EXIST);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    public async Task<NhaCungCapViewModel> GetByPhone(string Phone)
    {
        try
        {
            var result = await DbContext.DbNhaCungCaps.FirstOrDefaultAsync(x => x.Phone == Phone && x.RowStatus == RowStatusConstant.Active);
            if (result != null)
            {
                return Mapper.Map<NhaCungCapViewModel>(result);
            }
            else
            {
                throw new Exception(MessageConstant.NOT_EXIST);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    public async Task<NhaCungCapViewModel> GetByCCCD(string cccd)
    {
        try
        {
            var result = await DbContext.DbNhaCungCaps.FirstOrDefaultAsync(x => x.Cccd == cccd && x.RowStatus == RowStatusConstant.Active);
            if (result != null)
            {
                return Mapper.Map<NhaCungCapViewModel>(result);
            }
            else
            {
                throw new Exception(MessageConstant.NOT_EXIST);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    // create
    public async Task<NhaCungCapViewModel> Create(NhaCungCapCreateModel model)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(model.Ten) ||
                string.IsNullOrWhiteSpace(model.DiaChi) ||
                string.IsNullOrWhiteSpace(model.Phone) ||
                string.IsNullOrWhiteSpace(model.Cccd)
                )
            {
                throw new Exception("Dữ liệu không được trống");
            }
            var existed = await DbContext.DbNhaCungCaps.FirstOrDefaultAsync(x =>
                (x.Ten == model.Ten ||
                 x.Cccd == model.Cccd ||
                 x.Phone == model.Phone ||
                 x.Stk == model.Stk) && x.RowStatus == RowStatusConstant.Active
            );

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
                        var item = new DbNhaCungCap();
                        item.Ten = model.Ten;
                        item.DiaChi = model.DiaChi;
                        item.Phone = model.Phone;
                        item.Cccd = model.Cccd;
                        item.TenNganHang = model.TenNganHang;
                        item.Stk = model.Stk;
                        item.GoogleMap = model.GoogleMap;
                        item.Note = model.Note;
                        item.DvsdMa = model.DvsdMa;
                        item.RowStatus = RowStatusConstant.Active;

                    generateId:
                        string randomPart = GenerateId(DefaultCodeConstant.DbNhaCungCap.Name, DefaultCodeConstant.DbNhaCungCap.Length);
                        string generatedId = item.Phone + randomPart;

                        if (generatedId.Length > 8)
                        {
                            generatedId = generatedId.Substring(generatedId.Length - 8);
                        }
                        item.Ma = generatedId;
                        var resultIdExist = await DbContext.DbNhaCungCaps.AsNoTracking().Where(x => x.Ma == item.Ma).FirstOrDefaultAsync();
                        if (resultIdExist != null)
                            goto generateId;

                        item.CreatedAt = DateTime.Now;
                        item.UpdatedAt = DateTime.Now;
                        string currentUserId = GetCurrentUserId();
                        item.CreatedBy = currentUserId;
                        item.UpdatedBy = currentUserId;

                        await DbContext.DbNhaCungCaps.AddAsync(item);
                        await DbContext.SaveChangesAsync();

                        await transaction.CommitAsync();
                        return await GetById(item.Ma);
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            });
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    // update
    public async Task<NhaCungCapViewModel> Update(NhaCungCapUpdateMode model)
    {
        try
        {
            var existed = await DbContext.DbNhaCungCaps.FirstOrDefaultAsync(x => x.Ma == model.Ma);
            if (existed == null)
            {
                throw new Exception(MessageConstant.NOT_EXIST);
            }
            else
            {
                var strategy = DbContext.Database.CreateExecutionStrategy();
                return await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = await DbContext.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            existed.Ten = model.Ten;
                            existed.DiaChi = model.DiaChi;
                            existed.Phone = model.Phone;
                            existed.Cccd = model.Cccd;
                            existed.TenNganHang = model.TenNganHang;
                            existed.Stk = model.Stk;
                            existed.GoogleMap = model.GoogleMap;
                            existed.Note = model.Note;
                            existed.DvsdMa = model.DvsdMa;

                            existed.UpdatedBy = GetCurrentUserId();
                            existed.UpdatedAt = DateTime.Now;

                            await DbContext.SaveChangesAsync();

                            await transaction.CommitAsync();
                            return await GetById(existed.Ma);

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
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    public async Task<bool> Delete(string Ma)
    {
        try
        {
            var existed = await DbContext.DbNhaCungCaps.FirstOrDefaultAsync(x => x.Ma == Ma);
            if (existed == null)
            {
                throw new Exception(MessageConstant.NOT_EXIST);
            }
            else
            {
                var strategy = DbContext.Database.CreateExecutionStrategy();
                return await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = await DbContext.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            existed.RowStatus = RowStatusConstant.Deleted;
                            existed.UpdatedAt = DateTime.Now;
                            existed.UpdatedBy = GetCurrentUserId();
                            await DbContext.SaveChangesAsync();
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
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

}