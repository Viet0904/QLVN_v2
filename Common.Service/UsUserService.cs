using Common.Database.Entities;
using Common.Library.Constant;
using Common.Library.Helper;
using Common.Model.Common;
using Common.Model.UsUser;
using Common.Service.Common;
using System.Data;

namespace Common.Service
{
    public class UsUserService : BaseService
    {
        public ResModel<List<UsUserViewModel>> GetAll()
        {
            ResModel<List<UsUserViewModel>> res = new ResModel<List<UsUserViewModel>>();

            var results = DbContext.UsUsers.Where(x => x.RowStatus == RowStatusConstant.Active).ToList();
            res.Data = Mapper.Map<List<UsUserViewModel>>(results);

            return res;
        }
        public ResModel<List<UsUserViewModel>> GetFull()
        {
            ResModel<List<UsUserViewModel>> res = new ResModel<List<UsUserViewModel>>();

            var results = DbContext.UsUsers.ToList();
            res.Data = Mapper.Map<List<UsUserViewModel>>(results);

            return res;
        }

        public ResModel<UsUserViewModel> GetById(string id)
        {
            ResModel<UsUserViewModel> res = new ResModel<UsUserViewModel>();

            var result = DbContext.UsUsers.Where(x => x.Id == id && x.RowStatus == RowStatusConstant.Active).FirstOrDefault();
            if (result != null) res.Data = Mapper.Map<UsUserViewModel>(result);
            else
            {
                res.ErrorMessage = MessageConstant.NOT_EXIST;
            }

            return res;
        }

        public ResModel<UsUserViewModel> Login(string userName, string password)
        {
            ResModel<UsUserViewModel> res = new ResModel<UsUserViewModel>();

            // Lấy user theo username
            var result = DbContext.UsUsers.Where(x => x.UserName == userName && x.RowStatus == RowStatusConstant.Active).FirstOrDefault();
            
            if (result != null)
            {
                // Decrypt password từ database
                string decryptedPassword = CryptorEngineHelper.Decrypt(result.Password);
                
                // So sánh với password user nhập
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


        public ResModel<bool> ChangePassword(string userId, string oldPassword, string newPassword)
        {
            ResModel<bool> res = new ResModel<bool>();

            var result = DbContext.UsUsers.Where(x => x.Id == userId).FirstOrDefault();
            if (result != null)
            {
                // Decrypt password hiện tại từ database
                string currentPassword = CryptorEngineHelper.Decrypt(result.Password);
                
                if (currentPassword == oldPassword)
                {
                    // Encrypt password mới trước khi lưu
                    result.Password = CryptorEngineHelper.Encrypt(newPassword);
                    DbContext.SaveChanges();
                    res.Data = true;
                }
                else
                {
                    res.ErrorMessage = "Mật khẩu cũ không đúng!";
                }
            }
            else
            {
                res.ErrorMessage = MessageConstant.NOT_EXIST;
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
                    UnitOfWork.Ins.TransactionOpen();

                    var item = Mapper.Map<UsUser>(model);

                generateId:
                    item.Id = item.GroupId + GenerateId(DefaultCodeConstant.UsUser.Name, DefaultCodeConstant.UsUser.Length);

                    var resultIdExist = DbContext.UsUsers.Where(x => x.Id == item.Id).FirstOrDefault();
                    if (resultIdExist != null)
                        goto generateId;

                    // Encrypt password trước khi lưu vào database
                    item.Password = CryptorEngineHelper.Encrypt(model.Password);
                    DbContext.UsUsers.Add(item);
                    DbContext.SaveChanges();

                    UnitOfWork.Ins.TransactionCommit();
                    UnitOfWork.Ins.RenewDB();

                    res = GetById(item.Id);
                }
                catch (Exception)
                {
                    UnitOfWork.Ins.TransactionRollback();
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
