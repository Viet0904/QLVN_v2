using System.Xml.Linq;
using AutoMapper;
using Common.Database.Data;
using Common.Database.Entities;
using Common.Library.Constant;
using Common.Model.KhachHang;
using Common.Service.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Common.Service;

public class KHService : BaseService
{
    public KHService(QLVN_DbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(dbContext, mapper, httpContextAccessor)
    {

    }

    // GET All 
    public async Task<IEnumerable<KHViewModel>> GetAll()
    {
        try
        {
            var results = await DbContext.DbKhachHangs.Where(x => x.RowStatus == RowStatusConstant.Active).ToListAsync();
            return Mapper.Map<List<KHViewModel>>(results);
        }
        catch
        {
            throw new Exception();
        }
    }

    // get full
    public async Task<IEnumerable<KHViewModel>> GetFull()
    {
        try
        {
            var results = await DbContext.DbKhachHangs.ToListAsync();
            return Mapper.Map<List<KHViewModel>>(results);
        }
        catch
        {
            throw new Exception();
        }
    }


    // get by id
    public async Task<KHViewModel> GetById(string Ma)
    {
        try
        {
            var existed = await DbContext.DbKhachHangs.FirstOrDefaultAsync(x => x.Ma == Ma);
            if (existed != null)
            {
                return Mapper.Map<KHViewModel>(existed);
            }
            else
            {
                throw new Exception(MessageConstant.NOT_EXIST);
            }
        }
        catch
        {
            throw new Exception();
        }
    }


    public async Task<KHViewModel> GetByTen(string Ten)
    {
        try
        {
            var existed = await DbContext.DbKhachHangs.FirstOrDefaultAsync(x => x.Ten == Ten);
            if (existed != null)
            {
                return Mapper.Map<KHViewModel>(existed);
            }
            else
            {
                throw new Exception(MessageConstant.NOT_EXIST);
            }
        }
        catch
        {
            throw new Exception();
        }
    }


    public async Task<KHViewModel> GetByPhone(string Phone)
    {
        try
        {
            var existed = await DbContext.DbKhachHangs.FirstOrDefaultAsync(x => x.Phone == Phone);
            if (existed != null)
            {
                return Mapper.Map<KHViewModel>(existed);
            }
            else
            {
                throw new Exception(MessageConstant.NOT_EXIST);
            }
        }
        catch
        {
            throw new Exception();
        }
    }


    public async Task<KHViewModel> GetByCccd(string Cccd)
    {
        try
        {
            var existed = await DbContext.DbKhachHangs.FirstOrDefaultAsync(x => x.Cccd == Cccd);
            if (existed != null)
            {
                return Mapper.Map<KHViewModel>(existed);
            }
            else
            {
                throw new Exception(MessageConstant.NOT_EXIST);
            }
        }
        catch
        {
            throw new Exception();
        }
    }

    // create

    public async Task<KHViewModel> Create(KHCreateModel model)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(model.Ten) ||
                string.IsNullOrWhiteSpace(model.Cccd) ||
                string.IsNullOrWhiteSpace(model.Phone) ||
                string.IsNullOrWhiteSpace(model.DvsdMa)
                )
            {
                throw new Exception("Dữ liệu không hợp lệ");
            }

            var existed = await DbContext.DbKhachHangs.FirstOrDefaultAsync(x => (
                x.Ten == model.Ten || x.Cccd == model.Cccd || x.Phone == model.Phone
            ) && x.RowStatus == RowStatusConstant.Active);

            if (existed != null)
            {
                throw new Exception(MessageConstant.EXIST);
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
                            var item = new DbKhachHang();
                            item.Ten = model.Ten;
                            item.DiaChi = model.DiaChi;
                            item.Phone = model.Phone;
                            item.Cccd = model.Cccd;
                            item.TenNganHang = model.TenNganHang;
                            item.Stk = model.Stk;
                            item.GoogleMap = model.GoogleMap;
                            item.Note = model.Note;
                            item.DvsdMa = model.DvsdMa;

                        generateId:
                            string randomPart = GenerateId(DefaultCodeConstant.DbKhachHang.Name, DefaultCodeConstant.DbKhachHang.Length);
                            string generatedId = item.Phone + randomPart;

                            if (generatedId.Length > 8)
                            {
                                generatedId = generatedId.Substring(generatedId.Length - 8);
                            }
                            item.Ma = generatedId;
                            var resultIdExist = await DbContext.DbKhachHangs.AsNoTracking().Where(x => x.Ma == item.Ma).FirstOrDefaultAsync();
                            if (resultIdExist != null)
                                goto generateId;

                            item.CreatedBy = GetCurrentUserId();
                            item.UpdatedBy = GetCurrentUserId();

                            item.CreatedAt = DateTime.Now;
                            item.UpdatedAt = DateTime.Now;

                            await DbContext.DbKhachHangs.AddAsync(item);
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

        }
        catch
        {
            throw new Exception();
        }
    }



    // update
    public async Task<KHViewModel> Update(KHUpdateModel model)
    {
        try
        {
            var existed = await DbContext.DbKhachHangs.FirstOrDefaultAsync(x => x.Ma == model.Ma);
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
                            existed.RowStatus = model.RowStatus;

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
        catch
        {
            throw new Exception();
        }
    }


    // delete
    public async Task<bool> Delete (string Ma)
    {
        try
        {
            var existed = await DbContext.DbKhachHangs.FirstOrDefaultAsync(x => x.Ma == Ma);
            if (existed == null)
            {
                throw new Exception(MessageConstant.NOT_EXIST);
            } else
            {
                var strategy = DbContext.Database.CreateExecutionStrategy();
                return await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = await DbContext.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            existed.RowStatus = RowStatusConstant.Deleted;
                            await DbContext.SaveChangesAsync();
                            return true;

                        } catch (Exception)
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                });
            }
        } catch
        {
            throw new Exception();
        }
    }
}