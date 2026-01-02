using Common.Model.UsGroup;

namespace Common.Service.Interfaces
{
    public interface IGroupService
    {
        Task<IEnumerable<UsGroupViewModel>> GetAllAsync();
    }
}
