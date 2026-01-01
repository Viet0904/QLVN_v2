using AutoMapper;
using Common.Database.Data;
using Common.Database.Entities;
using Common.Library.Constant;
using Common.Library.Helper;
using Common.Model.Common;
using Common.Model.UsUser;
using Common.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Service
{
    public class UsUserService : BaseService
    {
        // ✅ THAY ĐỔI: Constructor nhận DbContext và IMapper
        public UsUserService(QLVN_DbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public ResModel<List<UsUserViewModel>> GetAll()
        {
            ResModel<List<UsUserViewModel>> res = new ResModel<List<UsUserViewModel>>();

            var result = DbContext.UsUsers.Where(x => x.RowStatus == RowStatusConstant.Active).ToList();
            res.Data = Mapper.Map<List<UsUserViewModel>>(result);

            return res;
        }

        public ResModel<List<UsUserViewModel>> GetFull()
        {
            ResModel<List<UsUserViewModel>> res = new ResModel<List<UsUserViewModel>>();

            var result = DbContext.UsUsers.ToList();
            res.Data = Mapper.Map<List<UsUserViewModel>>(result);

            return res;
        }

        public ResModel<UsUserViewModel> GetById(string id)
        {
            ResModel<UsUserViewModel> res = new ResModel<UsUserViewModel>();

            var result = DbContext.UsUsers.Where(x => x.Id == id).FirstOrDefault();
            res.Data = Mapper.Map<UsUserViewModel>(result);

            return res;
        }

        public ResModel<UsUserViewModel> Login(string userName, string password)
        {
            ResModel<UsUserViewModel> res = new ResModel<UsUserViewModel>();

            var result = DbContext.UsUsers.Where(x => x.UserName == userName && x.RowStatus == RowStatusConstant.Active).FirstOrDefault();

            if (result != null)
            {
                string decryptedPassword = CryptorEngineHelper.Decrypt(result.Password);

                if (decryptedPassword == password)
                {
                    res.Data = Mapper.Map<UsUserViewModel>(result);
                }
                else
                {
                    res.ErrorMessage = MessageConstant.USERNAME_PASSWORD_NOT_CORRECT;
                }
            }
            else
            {
                res.ErrorMessage = MessageConstant.USERNAME_PASSWORD_NOT_CORRECT;
            }

            return res;
        }

        public ResModel<UsUserViewModel> Create(UsUserCreateModel model)
        {
            ResModel<UsUserViewModel> res = new ResModel<UsUserViewModel>();

            var result = DbContext.UsUsers.Where(x => x.UserName == model.UserName && x.RowStatus == RowStatusConstant.Active).FirstOrDefault();
            if (result == null)
            {
                try
                {
                    DbContext.Database.BeginTransaction();

                    var item = Mapper.Map<UsUser>(model);

                generateId:
                    item.Id = item.GroupId + GenerateId(DefaultCodeConstant.UsUser.Name, DefaultCodeConstant.UsUser.Length);

                    var resultIdExist = DbContext.UsUsers.Where(x => x.Id == item.Id).FirstOrDefault();
                    if (resultIdExist != null)
                        goto generateId;

                    item.Password = CryptorEngineHelper.Encrypt(model.Password);
                    DbContext.UsUsers.Add(item);
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

        public ResModel<bool> Update(UsUserUpdateModel model)
        {
            ResModel<bool> res = new ResModel<bool>();
            try
            {
                var result = DbContext.UsUsers.Where(x => x.Id == model.Id).FirstOrDefault();
                if (result != null)
                {
                    Mapper.Map(model, result);
                    DbContext.SaveChanges();
                    res.Data = true;
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

        public ResModel<bool> Delete(string id)
        {
            ResModel<bool> res = new ResModel<bool>();

            var result = DbContext.UsUsers.Where(x => x.Id == id).FirstOrDefault();
            if (result != null)
            {
                result.RowStatus = RowStatusConstant.Deleted;
                DbContext.SaveChanges();
                res.Data = true;
            }
            else
            {
                res.ErrorMessage = MessageConstant.NOT_EXIST;
            }

            return res;
        }
    }
}