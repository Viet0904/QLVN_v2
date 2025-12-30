using System.ComponentModel.DataAnnotations;

namespace QLVN_Blazor.Models;

public class UserFormModel
{
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tên")]
    [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
    [StringLength(10, ErrorMessage = "Tên đăng nhập không được vượt quá 10 ký tự")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn nhóm")]
    public string GroupId { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string? Email { get; set; }

    public string? Phone { get; set; }
    public int? Gender { get; set; }
    public string? Cmnd { get; set; }
    public string? Address { get; set; }
    public string? Image { get; set; }
    public string? Note { get; set; }
    public int RowStatus { get; set; } = 1;
    
    // Thêm hỗ trợ đổi password
    public bool ChangePassword { get; set; } = false;
}