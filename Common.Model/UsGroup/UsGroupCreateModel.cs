using Common.Model.Common;


namespace Common.Model.UsGroup
{
    internal class UsGroupCreateModel : BaseViewModel
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Note { get; set; }
    }
}
