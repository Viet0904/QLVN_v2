using Common.Database.Entities;
using Common.Library.Constant;
using Common.Library.Helper;
using Common.Model.Common;
using Common.Service.Common;
using Common.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Service
{
    public class DvsdService : BaseService
    {
        /// <summary>
        /// Lấy tất cả đơn vị sản xuất Active
        /// </summary>
        public async Task<IEnumerable<object>> GetAllAsync()
        {
            var list = DbContext.DbDvsds
                .Where(x => x.RowStatus == RowStatusConstant.Active)
                .ToList();
            return list;
        }

        /// <summary>
        /// Tạo mới đơn vị sản xuất
        /// </summary>
        //public async Task CreateAsync(object request)
        //{
        //    try
        //    {
        //        UnitOfWork.Ins.TransactionOpen();

        //        var entity = Mapper.Map<DbDvsd>(request);
                
        //        // Generate ID
        //        generateId:
        //        entity.Ma = GenerateId(DefaultCodeConstant.DbDvsd.Name, DefaultCodeConstant.DbDvsd.Length);
                
        //        var existId = DbContext.DbDvsds.Where(x => x.Ma == entity.Ma).FirstOrDefault();
        //        if (existId != null) goto generateId;

        //        entity.CreatedAt = DateTime.Now;
        //        entity.RowStatus = RowStatusConstant.Active;

        //        DbContext.DbDvsds.Add(entity);
        //        DbContext.SaveChanges();

        //        UnitOfWork.Ins.TransactionCommit();
        //        UnitOfWork.Ins.RenewDB();
        //    }
        //    catch (Exception ex)
        //    {
        //        UnitOfWork.Ins.TransactionRollback();
        //        ExceptionHelper.HandleException(ex);
        //        throw;
        //    }
        //}
    }
}