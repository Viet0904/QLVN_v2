USE [IDI_QLVN]; -- Thay tên DB của bạn vào đây
GO

-- 1. Xử lý Index cho GroupId trên bảng UsUser
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UsUser_GroupId' AND object_id = OBJECT_ID('UsUser'))
BEGIN
    CREATE INDEX IX_UsUser_GroupId ON UsUser(GroupId);
    PRINT 'Đã tạo thành công IX_UsUser_GroupId';
END
ELSE
BEGIN
    PRINT 'Index IX_UsUser_GroupId đã tồn tại, bỏ qua tạo mới.';
END
GO

-- 2. Xử lý Index cho UserName (thường dùng để Login/Search)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UsUser_UserName' AND object_id = OBJECT_ID('UsUser'))
BEGIN
    CREATE INDEX IX_UsUser_UserName ON UsUser(UserName);
    PRINT 'Đã tạo thành công IX_UsUser_UserName';
END
GO

-- 3. Xử lý Index phức hợp cho RowStatus và CreatedAt (Tối ưu cho trang UserList phân trang)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UsUser_RowStatus_CreatedAt' AND object_id = OBJECT_ID('UsUser'))
BEGIN
    CREATE INDEX IX_UsUser_RowStatus_CreatedAt ON UsUser(RowStatus, CreatedAt DESC);
    PRINT 'Đã tạo thành công IX_UsUser_RowStatus_CreatedAt';
END
GO

-- 4. Xử lý Index cho bảng UsGroup để tìm kiếm tên nhanh
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UsGroup_Name' AND object_id = OBJECT_ID('UsGroup'))
BEGIN
    CREATE INDEX IX_UsGroup_Name ON UsGroup(Name);
    PRINT 'Đã tạo thành công IX_UsGroup_Name';
END
GO