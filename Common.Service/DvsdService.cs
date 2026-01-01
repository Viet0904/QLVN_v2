using AutoMapper;
using Common.Database.Data;
using Common.Database.Entities;
using Common.Library.Constant;
using Common.Library.Helper;
using Common.Model.Common;
using Common.Service.Common;
using Common.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Service
{
    public class DvsdService : BaseService
    {
        // ✅ Constructor nhận DbContext và IMapper
        public DvsdService(QLVN_DbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
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
    }
}