


using AutoMapper;
using Common.Database.Data;
using Common.Database.Entities;
using Common.Library.Constant;
using Common.Model.Common;
using Common.Model.UsGroup;
using Common.Model.UsUser;
using Common.Service.Common;

namespace Common.Service
{
    public class UsGroupService : BaseService
    {
        // ✅ Constructor nhận DbContext và IMapper
        public UsGroupService(QLVN_DbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        /// <summary>
        /// Lấy tất cả nhóm người dùng Active
        /// </summary>
        public Task<IEnumerable<UsGroupViewModel>> GetAllAsync()
        {
            return Task.Run(() =>
            {
                var groups = DbContext.UsGroups
                    .Where(g => g.RowStatus == RowStatusConstant.Active)
                    .ToList();
                return Mapper.Map<IEnumerable<UsGroupViewModel>>(groups);
            });
        }
        //public Task<UsGroupViewModel?> GetById(string id)
        //{
        //    return Task.Run(() =>
        //    {
        //        var group = DbContext.UsGroups
        //            .FirstOrDefault(g => g.Id == id && g.RowStatus == RowStatusConstant.Active);
        //        return Mapper.Map<UsGroupViewModel?>(group);
        //    });
        //}

        public ResModel<UsGroupViewModel> GetById(string id)
        {
            ResModel<UsGroupViewModel> res = new ResModel<UsGroupViewModel>();

            var result = DbContext.UsUsers.Where(x => x.Id == id).FirstOrDefault();
            res.Data = Mapper.Map<UsGroupViewModel>(result);

            return res;
        }


        public ResModel<UsGroupViewModel> Create(UsGroupViewModel model)
        {
            ResModel<UsGroupViewModel> res = new ResModel<UsGroupViewModel>();

            var result = DbContext.UsGroups.Where(x => x.Id == model.Id && x.RowStatus == RowStatusConstant.Active).FirstOrDefault();
            if (result == null)
            {
                try
                {
                    DbContext.Database.BeginTransaction();

                    var item = Mapper.Map<UsGroup>(model);

                generateId:
                    item.Id = item.Id + GenerateId(DefaultCodeConstant.UsGroup.Name, DefaultCodeConstant.UsGroup.Length);

                    var resultIdExist = DbContext.UsGroups.Where(x => x.Id == item.Id).FirstOrDefault();
                    if (resultIdExist != null)
                        goto generateId;


                    DbContext.SaveChanges();

                    DbContext.Database.CommitTransaction();

                    res = GetById(item.Id);
                }
                catch (Exception)
                {
                    DbContext.Database.RollbackTransaction();
                    throw;
                }
            }
            else
            {
                res.ErrorMessage = "Tên đăng nhập đã tồn tại.";
            }

            return res;
        }



    }
}