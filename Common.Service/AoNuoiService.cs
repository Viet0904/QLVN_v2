using AutoMapper;
using Common.Database.Data;
using Common.Database.Entities;
using Common.Library.Constant;
using Common.Model.AoNuoi;
using Common.Service.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Common.Service;

public class AoNuoiService : BaseService
{
    public AoNuoiService(QLVN_DbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(dbContext, mapper, httpContextAccessor)
    {

    }

    // GET All 
    public async Task<IEnumerable<AoNuoiViewModel>> GetAll()
    {
        try
        {
            var results = await DbContext.DbAoNuois.Where(x => x.RowStatus == RowStatusConstant.Active).ToListAsync();
            return Mapper.Map<List<AoNuoiViewModel>>(results);
        }
        catch
        {
            throw new Exception();
        }
    }

    // get full
    public async Task<IEnumerable<AoNuoiViewModel>> GetFull()
    {
        try
        {
            var results = await DbContext.DbAoNuois.ToListAsync();
            return Mapper.Map<List<AoNuoiViewModel>>(results);
        }
        catch
        {
            throw new Exception();
        }
    }


    // get by id
    public async Task<AoNuoiViewModel> GetById(string Ma)
    {
        try
        {
            var existed = await DbContext.DbAoNuois.FirstOrDefaultAsync(x => x.Ma == Ma && x.RowStatus == RowStatusConstant.Active);
            if (existed != null)
            {
                return Mapper.Map<AoNuoiViewModel>(existed);
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


    // Get by ma dvsd
    public async Task<IEnumerable<AoNuoiViewModel>> GetByDvsdMa(string dvsdMa)
    {
        try
        {
            var results = await DbContext.DbAoNuois.Where(x => x.DvsdMa == dvsdMa && x.RowStatus == RowStatusConstant.Active).ToListAsync();
            return Mapper.Map<List<AoNuoiViewModel>>(results);
        }
        catch
        {
            throw new Exception();
        }
    }

    // Get by mã ao
    public async Task<AoNuoiViewModel> GetByMaSo(string Maso)
    {
        try
        {
            var existed = await DbContext.DbAoNuois.FirstOrDefaultAsync(x => x.MaSo == Maso && x.RowStatus == RowStatusConstant.Active);
            if (existed == null)
            {
                throw new Exception(MessageConstant.NOT_EXIST);
            }
            else
            {
                return Mapper.Map<AoNuoiViewModel>(existed);
            }
        }
        catch
        {
            throw new Exception();
        }
    }

    // get by Tên
    public async Task<AoNuoiViewModel> GetByTen(string Ten)
    {
        try
        {
            var existed = await DbContext.DbAoNuois.FirstOrDefaultAsync(x => x.Ten == Ten && x.RowStatus == RowStatusConstant.Active);
            if (existed == null)
            {
                throw new Exception(MessageConstant.NOT_EXIST);
            }
            else
            {
                return Mapper.Map<AoNuoiViewModel>(existed);
            }
        }
        catch
        {
            throw new Exception();
        }
    }


    // get by ngày cấp
    public async Task<IEnumerable<AoNuoiViewModel>> GetByNgayCap(DateOnly ngayCap)
    {
        try
        {
            var results = await DbContext.DbAoNuois
                .Where(x => x.NgayCap.HasValue && x.NgayCap.Value == ngayCap && x.RowStatus == RowStatusConstant.Active)
                .ToListAsync();

            return Mapper.Map<List<AoNuoiViewModel>>(results);
        }
        catch
        {
            throw new Exception();
        }
    }


    // get by ngày thu hoach
    public async Task<IEnumerable<AoNuoiViewModel>> GetByNgayThuHoach(DateOnly ngayThuHoach)
    {
        try
        {
            var results = await DbContext.DbAoNuois
                .Where(x => x.NgayThuHoach.HasValue && x.NgayThuHoach.Value == ngayThuHoach && x.RowStatus == RowStatusConstant.Active).ToListAsync();
            return Mapper.Map<List<AoNuoiViewModel>>(results);
        }
        catch
        {
            throw new Exception();
        }
    }

    // get By số hợp đồng
    public async Task<AoNuoiViewModel> GetBySoHD(string soHd)
    {
        try
        {
            var existed = await DbContext.DbAoNuois.FirstOrDefaultAsync(x => x.SoHd == soHd && x.RowStatus == RowStatusConstant.Active);
            if (existed == null)
            {
                throw new Exception(MessageConstant.NOT_EXIST);
            }
            else
            {
                return Mapper.Map<AoNuoiViewModel>(existed);
            }
        }
        catch
        {
            throw new Exception();
        }
    }

    // Get by mã nhân viên giám sát
    public async Task<IEnumerable<AoNuoiViewModel>> GetByNhanVienGsma(string NhanVienGsma)
    {
        try
        {
            var results = await DbContext.DbAoNuois
                .Where(x => x.NhanVienGsma == NhanVienGsma && x.RowStatus == RowStatusConstant.Active).ToListAsync();
            return Mapper.Map<List<AoNuoiViewModel>>(results);
        }
        catch
        {
            throw new Exception();
        }
    }

    // get by tình trạng ao
    public async Task<IEnumerable<AoNuoiViewModel>> GetByTinhTrang(bool TinhTrang)
    {
        try
        {
            var results = await DbContext.DbAoNuois
                .Where(x => x.TinhTrang == TinhTrang && x.RowStatus == RowStatusConstant.Active).ToListAsync();
            return Mapper.Map<List<AoNuoiViewModel>>(results);
        }
        catch
        {
            throw new Exception();
        }
    }


    // get by mã khách hàng
    public async Task<IEnumerable<AoNuoiViewModel>> GetByKhachHangMa(string KhachHangMa)
    {
        try
        {
            var results = await DbContext.DbAoNuois
                .Where(x => x.KhachHangMa == KhachHangMa && x.RowStatus == RowStatusConstant.Active).ToListAsync();
            return Mapper.Map<List<AoNuoiViewModel>>(results);
        }
        catch
        {
            throw new Exception();
        }
    }

    // create

    public async Task<AoNuoiViewModel> Create(AoNuoiCreateModel model)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(model.MaSo) ||
                string.IsNullOrWhiteSpace(model.Ten)
            )
            {
                throw new Exception("Thông tin không được trống");
            }

            var existed = await DbContext.DbAoNuois.FirstOrDefaultAsync(
                x => x.MaSo == model.MaSo && x.RowStatus == RowStatusConstant.Active
            );

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
                            var item = new DbAoNuoi();

                            item.DvsdMa = model.DvsdMa;
                            item.MaSo = model.MaSo;
                            item.Ten = model.Ten;
                            item.NgayCap = model.NgayCap;
                            item.DiaChi = model.DiaChi;
                            item.DienTich = model.DienTich;
                            item.NgayThuHoach = model.NgayThuHoach;
                            item.SlduKien = model.SlduKien;
                            item.SoHd = model.SoHd;
                            item.NgayHd = model.NgayHd;
                            item.GiaGiaCong = model.GiaGiaCong;
                            item.NhanVienGsma = model.NhanVienGsma;
                            item.CongXuatNuoi = model.CongXuatNuoi;
                            item.TinhTrang = model.TinhTrang;
                            item.KhachHangMa = model.KhachHangMa;
                            item.KhuVucMa = model.KhuVucMa;
                            item.GoogleMap = model.GoogleMap;
                            item.Note = model.Note;
                            item.RowStatus = RowStatusConstant.Active;

                        generateId:
                            string randomPart = GenerateId(DefaultCodeConstant.DbAoNuoi.Name, DefaultCodeConstant.DbAoNuoi.Length);
                            string generatedId = item.MaSo + randomPart;

                            if (generatedId.Length > 8)
                            {
                                generatedId = generatedId.Substring(generatedId.Length - 8);
                            }
                            item.Ma = generatedId;
                            var resultIdExist = await DbContext.DbAoNuois.AsNoTracking().Where(x => x.Ma == item.Ma).FirstOrDefaultAsync();
                            if (resultIdExist != null)
                                goto generateId;

                            item.CreatedAt = DateTime.Now;
                            item.UpdatedAt = DateTime.Now;

                            var userId = GetCurrentUserId();

                            item.CreatedBy = userId;
                            item.UpdatedBy = userId;

                            await DbContext.DbAoNuois.AddAsync(item);
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

    public async Task<AoNuoiViewModel> Update(AoNuoiUpdateModel model)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(model.MaSo) ||
                string.IsNullOrWhiteSpace(model.Ten)
            )
            {
                throw new Exception("Thông tin không được trống");
            }

            var existed = await DbContext.DbAoNuois.FirstOrDefaultAsync(
                x => x.Ma == model.Ma && x.RowStatus == RowStatusConstant.Active
            );

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

                            existed.DvsdMa = model.DvsdMa;
                            existed.MaSo = model.MaSo;
                            existed.Ten = model.Ten;
                            existed.NgayCap = model.NgayCap;
                            existed.DiaChi = model.DiaChi;
                            existed.DienTich = model.DienTich;
                            existed.NgayThuHoach = model.NgayThuHoach;
                            existed.SlduKien = model.SlduKien;
                            existed.SoHd = model.SoHd;
                            existed.NgayHd = model.NgayHd;
                            existed.GiaGiaCong = model.GiaGiaCong;
                            existed.NhanVienGsma = model.NhanVienGsma;
                            existed.CongXuatNuoi = model.CongXuatNuoi;
                            existed.TinhTrang = model.TinhTrang;
                            existed.KhachHangMa = model.KhachHangMa;
                            existed.KhuVucMa = model.KhuVucMa;
                            existed.GoogleMap = model.GoogleMap;
                            existed.Note = model.Note;
                            existed.RowStatus = model.RowStatus;

                            existed.UpdatedAt = DateTime.Now;

                            var userId = GetCurrentUserId();
                            existed.UpdatedBy = userId;

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


    // Delete 
    public async Task<bool> Delete(string Ma)
    {
        try
        {
            var existed = await DbContext.DbAoNuois.FirstOrDefaultAsync(x => x.Ma == Ma);
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
        catch
        {
            throw new Exception();
        }
    }
}