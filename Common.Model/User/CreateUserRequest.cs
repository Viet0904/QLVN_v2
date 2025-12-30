using System.ComponentModel.DataAnnotations;

namespace Common.Model.User;

public class CreateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string GroupId { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int? Gender { get; set; }
    public string? Cmnd { get; set; }
    public string? Address { get; set; }
    public string? Note { get; set; }
    public int RowStatus { get; set; } = 1; // Trạng thái mặc định là Active
}