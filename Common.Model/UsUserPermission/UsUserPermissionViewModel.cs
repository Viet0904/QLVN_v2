using System;

namespace Common.Model.UsUserPermission
{
    public class UsUserPermissionViewModel
    {
        public string UserId { get; set; } = null!;
        public string MenuId { get; set; } = null!;
        public string? MenuName { get; set; }
        public bool Xem { get; set; }
        public bool Them { get; set; }
        public bool Sua { get; set; }
        public bool SuaHangLoat { get; set; }
        public bool Xoa { get; set; }
        public bool XoaHangLoat { get; set; }
        public bool XuatDuLieu { get; set; }
        public bool Khac { get; set; }
    }
}
