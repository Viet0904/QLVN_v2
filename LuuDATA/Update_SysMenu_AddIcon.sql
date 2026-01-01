-- =============================================================================
-- CẬP NHẬT BẢNG SysMenu - THÊM CỘT Icon
-- =============================================================================
USE [IDI_QLVN]
GO

-- Kiểm tra và thêm cột Icon nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SysMenu]') AND name = 'Icon')
BEGIN
    ALTER TABLE [dbo].[SysMenu] ADD [Icon] NVARCHAR(100) NULL;
    PRINT 'Đã thêm cột Icon vào bảng SysMenu';
END
ELSE
BEGIN
    PRINT 'Cột Icon đã tồn tại trong bảng SysMenu';
END
GO

-- =============================================================================
-- SEED DATA - MENU MẪU CHO HỆ THỐNG QUẢN LÝ VÙNG NUÔI
-- Icons sử dụng: Feather Icons, Themify Icons, Icofont Icons
-- =============================================================================

-- Xóa dữ liệu cũ (nếu có)
DELETE FROM [dbo].[SysMenu];
GO

-- ===========================
-- 1. MENU CHÍNH - DASHBOARD
-- ===========================
INSERT INTO [dbo].[SysMenu] ([Name], [ParentMenu], [Note], [Icon], [IsActive])
VALUES 
('dashboard', NULL, N'Trang chủ', 'feather icon-home', 1);

-- ===========================
-- 2. MENU QUẢN TRỊ HỆ THỐNG
-- ===========================
INSERT INTO [dbo].[SysMenu] ([Name], [ParentMenu], [Note], [Icon], [IsActive])
VALUES 
('system', NULL, N'Quản trị hệ thống', 'feather icon-settings', 1),
('system_user', 'system', N'Quản lý người dùng', 'feather icon-users', 1),
('system_group', 'system', N'Quản lý nhóm quyền', 'feather icon-shield', 1),
('system_permission', 'system', N'Phân quyền', 'feather icon-key', 1),
('system_setting', 'system', N'Cài đặt hệ thống', 'feather icon-sliders', 1),
('system_log', 'system', N'Lịch sử hoạt động', 'feather icon-clock', 1);

-- ===========================
-- 3. MENU DANH MỤC
-- ===========================
INSERT INTO [dbo].[SysMenu] ([Name], [ParentMenu], [Note], [Icon], [IsActive])
VALUES 
('category', NULL, N'Danh mục', 'feather icon-list', 1),
('category_dvsd', 'category', N'Đơn vị sử dụng', 'feather icon-briefcase', 1),
('category_khuvuc', 'category', N'Khu vực', 'feather icon-map', 1),
('category_khachhang', 'category', N'Khách hàng', 'feather icon-user-check', 1),
('category_nhacungcap', 'category', N'Nhà cung cấp', 'feather icon-truck', 1),
('category_hoachat', 'category', N'Hóa chất', 'icofont icofont-test-tube-alt', 1),
('category_loaica', 'category', N'Loại cá', 'icofont icofont-fish-2', 1),
('category_loaibenh', 'category', N'Loại bệnh', 'icofont icofont-virus', 1),
('category_sizenl', 'category', N'Size nuôi lớn', 'ti-ruler-alt', 1),
('category_tangtrong', 'category', N'Tăng trọng', 'feather icon-trending-up', 1);

-- ===========================
-- 4. MENU QUẢN LÝ AO NUÔI
-- ===========================
INSERT INTO [dbo].[SysMenu] ([Name], [ParentMenu], [Note], [Icon], [IsActive])
VALUES 
('aonuoi', NULL, N'Quản lý ao nuôi', 'icofont icofont-water-drop', 1),
('aonuoi_list', 'aonuoi', N'Danh sách ao nuôi', 'feather icon-grid', 1),
('aonuoi_nhaptg', 'aonuoi', N'Nhập thả giống', 'feather icon-download', 1),
('aonuoi_nhapsl', 'aonuoi', N'Nhập sản lượng', 'feather icon-package', 1),
('aonuoi_nhapkc', 'aonuoi', N'Nhập kiểm cân', 'icofont icofont-ui-calculator', 1),
('aonuoi_nhapmt', 'aonuoi', N'Nhập môi trường', 'icofont icofont-thermometer', 1),
('aonuoi_nhapta', 'aonuoi', N'Nhập thức ăn', 'icofont icofont-ui-food', 1),
('aonuoi_nhaphc', 'aonuoi', N'Nhập hóa chất', 'icofont icofont-laboratory', 1),
('aonuoi_nhapkhac', 'aonuoi', N'Nhập thông tin khác', 'feather icon-more-horizontal', 1),
('aonuoi_nhapth', 'aonuoi', N'Nhập thu hoạch', 'ti-package', 1),
('aonuoi_nhapcahao', 'aonuoi', N'Nhập cá hao', 'ti-na', 1);

-- ===========================
-- 5. MENU KHÁNG SINH
-- ===========================
INSERT INTO [dbo].[SysMenu] ([Name], [ParentMenu], [Note], [Icon], [IsActive])
VALUES 
('khangsinh', NULL, N'Kháng sinh', 'icofont icofont-pills', 1),
('khangsinh_yeucau', 'khangsinh', N'Yêu cầu kiểm tra', 'feather icon-file-plus', 1),
('khangsinh_ketqua', 'khangsinh', N'Kết quả kiểm tra', 'feather icon-check-square', 1);

-- ===========================
-- 6. MENU BÁO CÁO
-- ===========================
INSERT INTO [dbo].[SysMenu] ([Name], [ParentMenu], [Note], [Icon], [IsActive])
VALUES 
('report', NULL, N'Báo cáo', 'feather icon-bar-chart-2', 1),
('report_aonuoi', 'report', N'Báo cáo ao nuôi', 'feather icon-bar-chart', 1),
('report_sanluong', 'report', N'Báo cáo sản lượng', 'feather icon-trending-up', 1),
('report_hoachat', 'report', N'Báo cáo hóa chất', 'icofont icofont-chart-bar-graph', 1),
('report_thucan', 'report', N'Báo cáo thức ăn', 'icofont icofont-chart-line', 1),
('report_doanhso', 'report', N'Báo cáo doanh số', 'feather icon-dollar-sign', 1),
('report_khangsinh', 'report', N'Báo cáo kháng sinh', 'icofont icofont-prescription', 1);

-- ===========================
-- 7. MENU TỔNG HỢP
-- ===========================
INSERT INTO [dbo].[SysMenu] ([Name], [ParentMenu], [Note], [Icon], [IsActive])
VALUES 
('summary', NULL, N'Tổng hợp', 'feather icon-layers', 1),
('summary_dashboard', 'summary', N'Bảng điều khiển', 'feather icon-activity', 1),
('summary_aonuoi', 'summary', N'Tổng hợp ao nuôi', 'ti-stats-up', 1),
('summary_khachhang', 'summary', N'Tổng hợp khách hàng', 'ti-id-badge', 1);

GO



-- =============================================================================
-- HIỂN THỊ KẾT QUẢ
-- =============================================================================
SELECT 
    Name AS [Tên Menu],
    ParentMenu AS [Menu Cha],
    Note AS [Mô tả],
    Icon AS [Icon],
    IsActive AS [Trạng thái]
FROM [dbo].[SysMenu]
ORDER BY 
    CASE WHEN ParentMenu IS NULL THEN 0 ELSE 1 END,
    ParentMenu,
    Name;
GO
