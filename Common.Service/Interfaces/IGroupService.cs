using Common.Model.Group;

namespace Common.Service.Interfaces
{
    public interface IGroupService
    {
        Task<IEnumerable<GroupDto>> GetAllAsync();
    }
}
