

namespace Common.Model.SysTheme
{
    public class ThemeSettingsUpdateModel
    {
        public string? UserId { get; set; }
        // 1. Layout Options
        public bool IsFixedSidebar { get; set; } = true; // id="sidebar-position"
        public bool IsFixedHeader { get; set; } = true;  // id="header-position"

        // 2. Sidebar Settings
        public string MainLayout { get; set; } = "theme1"; // theme1, themelight1

        public string MenuType { get; set; } = "st6"; // st5 (color icon), st6 (simple icon)
        public string SidebarEffect { get; set; } = "shrink"; // shrink, overlay, push
        public string BorderStyle { get; set; } = "none"; // No Border, Style1-> Style3
        public string DropdownIcon { get; set; } = "style1"; // style1 -> style3
        public string SubItemIcon { get; set; } = "style1"; // style1 -> style6

        // 3. Colors
        public string HeaderBrandColor { get; set; } = "theme1"; // .logo-theme (theme1 -> theme5)
        public string HeaderColor { get; set; } = "theme1"; // .header- (theme1 -> theme6)
        public string ActiveLinkColor { get; set; } = "theme1"; // .active-item-theme  (theme1 -> theme12)
        public string MenuCaptionColor { get; set; } = "theme5"; // .pcoded-navigatio-lavel (theme1 -> theme6)
    }
}
