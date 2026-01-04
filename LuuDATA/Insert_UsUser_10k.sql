USE [IDI_QLVN]
GO

-- =============================================================================
-- SCRIPT SINH 10,000 USERS - TỰ ĐỘNG TRÁNH TRÙNG LẶP ID ĐÃ CÓ
-- =============================================================================

BEGIN TRANSACTION;
BEGIN TRY
    -- 1. Lấy ID lớn nhất hiện tại trong hệ thống (ép kiểu về INT để tính toán)
    DECLARE @MaxId INT;
    SELECT @MaxId = ISNULL(MAX(CAST(Id AS INT)), 0) FROM [dbo].[UsUser];

    -- 2. Chuẩn bị danh sách GroupId hợp lệ (NVARCHAR(3))
    DECLARE @AvailableGroups TABLE (Id NVARCHAR(20));
    INSERT INTO @AvailableGroups (Id)
    SELECT Id FROM [dbo].[UsGroup] WHERE LEN(Id) <= 20;

    -- Nếu chưa có Group nào, tạo mặc định để không vi phạm FK_UsUser_GroupId
    IF NOT EXISTS (SELECT 1 FROM @AvailableGroups)
    BEGIN
        INSERT INTO [dbo].[UsGroup] ([Id], [Name], [Note], [RowStatus], [CreatedAt], [UpdatedAt])
        VALUES ('001', N'Administrator', N'Admin Group', 1, GETDATE(), GETDATE());
        INSERT INTO @AvailableGroups (Id) VALUES ('001');
    END

    -- 3. Sử dụng CTE để sinh 10,000 bản ghi bắt đầu từ (@MaxId + 1)
    ;WITH 
    L0 AS (SELECT 1 AS c UNION ALL SELECT 1),
    L1 AS (SELECT 1 AS c FROM L0 AS A CROSS JOIN L0 AS B),
    L2 AS (SELECT 1 AS c FROM L1 AS A CROSS JOIN L1 AS B),
    L3 AS (SELECT 1 AS c FROM L2 AS A CROSS JOIN L2 AS B),
    L4 AS (SELECT 1 AS c FROM L3 AS A CROSS JOIN L3 AS B),
    Nums AS (SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS RowNum FROM L4),
    DataGen AS (
        SELECT TOP 10000 (@MaxId + RowNum) AS NewIdNumber 
        FROM Nums
    )
    INSERT INTO [dbo].[UsUser] (
        [Id],
        [GroupId],
        [Name],
        [Gender],
        [UserName],
        [Password],
        [Email],
        [Phone],
        [CMND],
        [Address],
        [Image],
        [Note],
        [RowStatus],
        [CreatedAt],
        [CreatedBy],
        [UpdatedAt],
        [UpdatedBy],
        [Theme]
    )
    SELECT 
        -- Id: NVARCHAR(8) - Format thành 8 chữ số
        RIGHT('00000000' + CAST(NewIdNumber AS NVARCHAR(8)), 8),
        
        -- GroupId: Lấy ngẫu nhiên từ bảng tạm đã lọc
        (SELECT TOP 1 Id FROM @AvailableGroups ORDER BY NEWID()),
        
        -- Name
        N'User Test ' + CAST(NewIdNumber AS NVARCHAR(10)),
        
        -- Gender
        ABS(CHECKSUM(NEWID())) % 2,
        
        -- UserName: Cắt bớt để vừa NVARCHAR(10)
        LEFT('u' + CAST(NewIdNumber AS NVARCHAR(9)), 10),
        
        -- Password: Giả lập MD5
        'e10adc3949ba59abbe56e057f20f883e', -- Default '123456'
        
        -- Email
        'user' + CAST(NewIdNumber AS NVARCHAR(10)) + '@idi.com.vn',
        
        -- Phone
        '09' + RIGHT('00000000' + CAST(NewIdNumber AS NVARCHAR(10)), 8),
        
        -- CMND
        RIGHT('000000000' + CAST(NewIdNumber AS NVARCHAR(10)), 9),
        
        -- Address
        N'Khu vực nuôi số ' + CAST(NewIdNumber AS NVARCHAR(10)),
        
        -- Image
        NULL,
        
        -- Note
        N'Bulk Insert Performance Test',
        
        -- RowStatus
        1,
        
        -- CreatedAt
        GETDATE(),
        
        -- CreatedBy: Lấy lại User 00001001 đã có của bạn làm người tạo
        '00001001',
        
        -- UpdatedAt
        GETDATE(),
        
        -- UpdatedBy
        '00001001',
        
        -- Theme
        N'Default'
    FROM DataGen;

    COMMIT TRANSACTION;
    DECLARE @Count INT;
    SELECT @Count = COUNT(*) FROM [dbo].[UsUser];
    PRINT N'Thành công! Tổng số User hiện tại: ' + CAST(@Count AS NVARCHAR(10));

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    PRINT N'Lỗi xảy ra: ' + ERROR_MESSAGE();
END CATCH
GO