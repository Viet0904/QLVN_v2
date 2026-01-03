using Common.Model.Common;


namespace Common.Model.UsGroup
{
    public class UsGroupUpdateModel
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Note { get; set; }
        public int RowStatus { get; set; }
    }
}
