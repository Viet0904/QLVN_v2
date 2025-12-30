using System.ComponentModel.DataAnnotations;

namespace Common.Model.User
{
    public class UserDto
    {
        [Required]
        public string Id { get; set; } = null!;
        public string GroupId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int? Gender { get; set; }
        public string UserName { get; set; } = null!;
        // Password không được trả về
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Cmnd { get; set; }
        public string? Address { get; set; }
        public string? Image { get; set; }
        public string? Note { get; set; }
        public int RowStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = null!;
    }
}
