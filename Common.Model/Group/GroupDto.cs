
namespace Common.Model.Group
{
   

    public class GroupDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Note { get; set; }
        public int RowStatus { get; set; }
    }
}
