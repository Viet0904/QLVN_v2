USE [IDI_QLVN]
GO

-- =============================================================================
-- 1. SINH DỮ LIỆU MẪU CHO BẢNG [UsGroup] 
-- Giới hạn NVARCHAR(3) nên chèn tối đa 999 dòng còn lại (trừ mã '001' đã có)
-- =============================================================================
PRINT 'Starting insert into UsGroup (Max 1000 records)...';

WITH 
    L0 AS (SELECT 1 AS c UNION ALL SELECT 1), -- 2
    L1 AS (SELECT 1 AS c FROM L0 AS T1 CROSS JOIN L0 AS T2), -- 4
    L2 AS (SELECT 1 AS c FROM L1 AS T1 CROSS JOIN L1 AS T2), -- 16
    L3 AS (SELECT 1 AS c FROM L2 AS T1 CROSS JOIN L2 AS T2), -- 256
    L4 AS (SELECT 1 AS c FROM L3 AS T1 CROSS JOIN L1 AS T2), -- 1024
    Nums AS (SELECT ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) - 1 AS n FROM L4) -- Bắt đầu từ 0
INSERT INTO [dbo].[UsGroup] (
    [Id], [Name], [Note], [RowStatus], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy]
)
SELECT 
    RIGHT('000' + CAST(n AS VARCHAR(3)), 3),
    N'Group Sample ' + RIGHT('000' + CAST(n AS VARCHAR(3)), 3),
    N'Dữ liệu mẫu cho Group ' + CAST(n AS NVARCHAR(10)),
    1, GETDATE(), N'00001001', GETDATE(), N'00001001'
FROM Nums
WHERE n <= 999 
  AND RIGHT('000' + CAST(n AS VARCHAR(3)), 3) <> '001'; -- Bỏ qua Id 001 đã tồn tại

PRINT 'Insert UsGroup completed.';
GO

-- =============================================================================
-- 2. SINH DỮ LIỆU MẪU CHO BẢNG [UsUser] (50,000 DÒNG)
-- Id là NVARCHAR(8), bắt đầu từ 00001002 trở đi
-- =============================================================================
PRINT 'Starting insert into UsUser (50,000 records)...';

WITH 
    L0 AS (SELECT 1 AS c UNION ALL SELECT 1),
    L1 AS (SELECT 1 AS c FROM L0 AS T1 CROSS JOIN L0 AS T2), -- 4
    L2 AS (SELECT 1 AS c FROM L1 AS T1 CROSS JOIN L1 AS T2), -- 16
    L3 AS (SELECT 1 AS c FROM L2 AS T1 CROSS JOIN L2 AS T2), -- 256
    L4 AS (SELECT 1 AS c FROM L3 AS T1 CROSS JOIN L3 AS T2), -- 65,536
    Nums AS (SELECT ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS n FROM L4)
INSERT INTO [dbo].[UsUser] (
    [Id], [GroupId], [Name], [Gender], [UserName], [Password], [Email], [Phone], 
    [CMND], [Address], [Image], [Note], [RowStatus], [CreatedAt], [CreatedBy], 
    [UpdatedAt], [UpdatedBy], [Theme]
)
SELECT 
    RIGHT('00000000' + CAST(n + 1001 AS VARCHAR(10)), 8), -- Sinh mã từ 00001002
    '001', -- Gán vào Group Administrator
    N'User Test ' + CAST(n AS NVARCHAR(10)),
    (n % 2), 
    'user' + CAST(n AS VARCHAR(10)),
    '1fe24dc8a2869a095c53d1cc77d14683d4f9ec5c', -- Pass mặc định
    'user' + CAST(n AS VARCHAR(10)) + '@idi.com.vn',
    '090' + RIGHT('0000000' + CAST(n AS VARCHAR(10)), 7),
    NULL, NULL, NULL, NULL, 1, GETDATE(), N'00001001', GETDATE(), N'00001001',
    N'{"UserId":"' + RIGHT('00000000' + CAST(n + 1001 AS VARCHAR(10)), 8) + '","MainLayout":"theme1"}'
FROM Nums
WHERE n <= 50000;

PRINT 'Insert UsUser completed.';
GO

-- =============================================================================
-- 3. KIỂM TRA TỔNG QUAN
-- =============================================================================
SELECT 
    (SELECT COUNT(*) FROM [dbo].[UsGroup]) AS Total_Groups,
    (SELECT COUNT(*) FROM [dbo].[UsUser]) AS Total_Users;