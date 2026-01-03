using Common.Model.Common;


namespace Common.Model.UsGroup
{
    public class UsGroupUpdateModel : BaseViewModel
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Note { get; set; }
    }
}
