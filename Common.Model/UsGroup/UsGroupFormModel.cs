namespace Common.Model.UsGroup
{
    public class UsGroupFormModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Note { get; set; }
        public int RowStatus { get; set; } = 1;
    }
}
