using Common.Database;
using Common.Library.Constant;
using Common.Model.Common;
using Common.Model.Group;
using Common.Service.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Service
{
    public class UsGroupService : BaseService
    {
        /// <summary>
        /// Lấy tất cả nhóm người dùng Active
        /// </summary>
        public Task<IEnumerable<GroupDto>> GetAllAsync()
        {
            return Task.Run(() =>
            {
                var groups = DbContext.UsGroups
                    .Where(g => g.RowStatus == RowStatusConstant.Active)
                    .ToList();
                return Mapper.Map<IEnumerable<GroupDto>>(groups);
            });
        }
    }
}