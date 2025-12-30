/*
--------------------------------------------------------------------------------
HỆ THỐNG QUẢN LÝ NUÔI TRỒNG - IDI_QLVN
Script tối ưu hóa: Hợp nhất Theme vào bảng UsUser
Ngày cập nhật: 29/12/2025
--------------------------------------------------------------------------------
*/

USE [IDI_QLVN];
GO

-- =============================================================================
-- 1. NHÓM BẢNG HỆ THỐNG (SYSTEM & SECURITY)
-- =============================================================================

-- Danh mục Nhóm người dùng
IF OBJECT_ID('[dbo].[UsGroup]', 'U') IS NULL
CREATE TABLE [dbo].[UsGroup] (
    [Id]         NVARCHAR(3)   NOT NULL,
    [Name]       NVARCHAR(100) NOT NULL,
    [Note]       NVARCHAR(200) NULL,
    [RowStatus]  INT           NOT NULL,
    [CreatedAt]  DATETIME      NOT NULL DEFAULT (GETDATE()),
    [CreatedBy]  NVARCHAR(8)   NULL,
    [UpdatedAt]  DATETIME      NOT NULL DEFAULT (GETDATE()),
    [UpdatedBy]  NVARCHAR(8)   NULL,
    CONSTRAINT [PK_UsGroup] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- Danh mục Người dùng 
IF OBJECT_ID('[dbo].[UsUser]', 'U') IS NULL
CREATE TABLE [dbo].[UsUser] (
    [Id]         NVARCHAR(8)   NOT NULL,
    [GroupId]    NVARCHAR(3)   NOT NULL,
    [Name]       NVARCHAR(100) NOT NULL,
    [Gender]     INT           NULL,
    [UserName]   NVARCHAR(10)  NOT NULL,
    [Password]   NVARCHAR(50)  NOT NULL,
    [Email]      NVARCHAR(30)  NULL,
    [Phone]      NVARCHAR(50)  NULL,
    [CMND]       NVARCHAR(20)  NULL,
    [Address]    NVARCHAR(200) NULL,
    [Image]      NVARCHAR(200) NULL,
    [Theme]      NVARCHAR(MAX) NULL, -- Thuộc tính cấu hình giao diện người dùng
    [Note]       NVARCHAR(100) NULL,
    [RowStatus]  INT           NOT NULL,
    [CreatedAt]  DATETIME      NOT NULL DEFAULT (GETDATE()),
    [CreatedBy]  NVARCHAR(8)   NOT NULL,
    [UpdatedAt]  DATETIME      NOT NULL DEFAULT (GETDATE()),
    [UpdatedBy]  NVARCHAR(8)   NOT NULL,
    CONSTRAINT [PK_UsUser] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UsUser_UsGroup] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[UsGroup] ([Id])
);
GO

-- Log hoạt động người dùng
CREATE TABLE [dbo].[UsUserLog] (
    [Id]           NVARCHAR(30)   NOT NULL,
    [UserId]       NVARCHAR(8)    NOT NULL,
    [Menu]         NVARCHAR(50)   NOT NULL,
    [ComputerName] NVARCHAR(50)   NOT NULL,
    [ActionName]   NVARCHAR(100)  NOT NULL,
    [Data]         NVARCHAR(3000) NULL,
    [Note]         NVARCHAR(2000) NULL,
    [ActionDate]   DATETIME       NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_UsUserLog] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UsUserLog_UsUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

-- Quản lý Menu hệ thống
CREATE TABLE [dbo].[SysMenu] (
    [Name]       NVARCHAR(50)  NOT NULL,
    [ParentMenu] NVARCHAR(50)  NULL,
    [Note]       NVARCHAR(100) NOT NULL,
    [IsActive]   INT           NULL,
    CONSTRAINT [PK_SysMenu] PRIMARY KEY CLUSTERED ([Name] ASC)
);
GO

-- Phân quyền chi tiết
CREATE TABLE [dbo].[UsUserPermission] (
    [UserId]       NVARCHAR(8)  NOT NULL,
    [MenuId]       NVARCHAR(50) NOT NULL,
    [Xem]          BIT          DEFAULT 0,
    [Them]         BIT          DEFAULT 0,
    [Sua]          BIT          DEFAULT 0,
    [SuaHangLoat]  BIT          DEFAULT 0,
    [Xoa]          BIT          DEFAULT 0,
    [XoaHangLoat]  BIT          DEFAULT 0,
    [XuatDuLieu]   BIT          DEFAULT 0,
    [Khac]         BIT          DEFAULT 0,
    [UpdatedAt]    DATETIME     NOT NULL DEFAULT (GETDATE()),
    [UpdatedBy]    NVARCHAR(8)  NULL,
    CONSTRAINT [PK_UsUserPermission] PRIMARY KEY CLUSTERED ([UserId] ASC, [MenuId] ASC),
    CONSTRAINT [FK_UsUserPermission_UsUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_UsUserPermission_SysMenu] FOREIGN KEY ([MenuId]) REFERENCES [dbo].[SysMenu] ([Name])
);
GO

-- Bảng quản lý ID tự động sinh
CREATE TABLE [dbo].[SysIdGenerated] (
    [TableName]  NVARCHAR(50) NOT NULL,
    [CurrentId]  INT          NOT NULL DEFAULT 0,
    CONSTRAINT [PK_SysIdGenerated] PRIMARY KEY CLUSTERED ([TableName] ASC)
);
GO

-- =============================================================================
-- 2. NHÓM DANH MỤC DÙNG CHUNG (MASTER DATA)
-- =============================================================================

CREATE TABLE [dbo].[DbDvsd] (
    [Ma]        NVARCHAR(5)   NOT NULL,
    [Ten]       NVARCHAR(50)  NOT NULL,
    [DiaChi]    NVARCHAR(500) NULL,
    [Phone]     NVARCHAR(50)  NULL,
    [CCCD]      NVARCHAR(50)  NULL,
    [MST]       NVARCHAR(500) NULL,
    [STK]       NVARCHAR(50)  NULL,
    [Note]      NVARCHAR(100) NULL,
    [RowStatus] INT           NOT NULL,
    [CreatedAt] DATETIME      NOT NULL,
    [CreatedBy] NVARCHAR(8)   NOT NULL,
    [UpdatedAt] DATETIME      NOT NULL,
    [UpdatedBy] NVARCHAR(8)   NOT NULL,
    CONSTRAINT [PK_DbDvsd] PRIMARY KEY CLUSTERED ([Ma] ASC)
);
GO

CREATE TABLE [dbo].[DbKhuVuc] (
    [Ma]        NVARCHAR(5)   NOT NULL,
    [Ten]       NVARCHAR(50)  NOT NULL,
    [TinhThanh] NVARCHAR(500) NULL,
    [Note]      NVARCHAR(100) NULL,
    [DvsdMa]    NVARCHAR(5)   NOT NULL,
    [RowStatus] INT           NOT NULL,
    [CreatedAt] DATETIME      NOT NULL,
    [CreatedBy] NVARCHAR(8)   NOT NULL,
    [UpdatedAt] DATETIME      NOT NULL,
    [UpdatedBy] NVARCHAR(8)   NOT NULL,
    CONSTRAINT [PK_DbKhuVuc] PRIMARY KEY CLUSTERED ([Ma] ASC)
);
GO

-- =============================================================================
-- 3. NHÓM QUẢN LÝ AO NUÔI VÀ NGHIỆP VỤ
-- =============================================================================

CREATE TABLE [dbo].[DbAoNuoi] (
    [Ma]            NVARCHAR(5)     NOT NULL,
    [DvsdMa]        NVARCHAR(5)     NOT NULL,
    [MaSo]          NVARCHAR(50)    NULL,
    [Ten]           NVARCHAR(50)    NOT NULL,
    [NgayCap]       DATE            NULL,
    [DienTich]      NUMERIC(18, 0)  NOT NULL DEFAULT 0,
    [SLDuKien]      NUMERIC(18, 0)  NOT NULL DEFAULT 0,
    [TinhTrang]     BIT             NOT NULL,
    [KhachHangMa]   NVARCHAR(50)    NOT NULL,
    [KhuVucMa]      NVARCHAR(5)     NOT NULL,
    [Note]          NVARCHAR(100)   NULL,
    [RowStatus]     INT             NOT NULL,
    [CreatedAt]     DATETIME        NOT NULL,
    [CreatedBy]     NVARCHAR(8)     NOT NULL,
    [UpdatedAt]     DATETIME        NOT NULL,
    [UpdatedBy]     NVARCHAR(8)     NOT NULL,
    CONSTRAINT [PK_DbAoNuoi] PRIMARY KEY CLUSTERED ([Ma] ASC),
    CONSTRAINT [FK_DbAoNuoi_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_DbAoNuoi_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[DbAoNuoi_NhapTA] (
    [Ma]        NVARCHAR(5)    NOT NULL,
    [AoNuoiMa]  NVARCHAR(5)    NOT NULL,
    [Ngay]      DATE           NOT NULL,
    [TA_2mm]    NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [TA_3mm]    NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [TA_5mm]    NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [RowStatus] INT            NOT NULL,
    [CreatedAt] DATETIME       NOT NULL,
    [CreatedBy] NVARCHAR(8)    NOT NULL,
    [UpdatedAt] DATETIME       NOT NULL,
    [UpdatedBy] NVARCHAR(8)    NOT NULL,
    CONSTRAINT [PK_DbAoNuoi_NhapTA] PRIMARY KEY CLUSTERED ([Ma] ASC),
    CONSTRAINT [FK_DbAoNuoi_NhapTA_AoNuoi] FOREIGN KEY ([AoNuoiMa]) REFERENCES [dbo].[DbAoNuoi] ([Ma])
);
GO

-- =============================================================================
-- 4. VIEW & STORED PROCEDURES
-- =============================================================================

-- View log 
CREATE OR ALTER VIEW [dbo].[vUserLog]
AS
SELECT 
    G.Id AS GroupId, 
    G.Name AS GroupName, 
    U.Id AS UserId, 
    U.Name AS TenNV,  
    L.Id, 
    L.Menu, 
    L.ComputerName, 
    L.ActionName, 
    L.ActionDate
FROM dbo.UsGroup G
INNER JOIN dbo.UsUser U ON G.Id = U.GroupId 
INNER JOIN dbo.UsUserLog L ON U.Id = L.UserId;
GO

-- SP: Quản lý quyền
CREATE OR ALTER PROCEDURE [dbo].[SpR_UserPermission] 
    @UserId NVARCHAR(8)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @CountMenu INT = (SELECT COUNT(*) FROM [dbo].[SysMenu]);
    DECLARE @CountUserPerm INT = (SELECT COUNT(*) FROM [dbo].[UsUserPermission] WHERE UserId = @UserId);

    IF (@CountUserPerm != @CountMenu)
    BEGIN
        DELETE FROM [dbo].[UsUserPermission] WHERE UserId = @UserId;

        INSERT INTO [dbo].[UsUserPermission] (
            UserId, MenuId, Xem, Them, Sua, Xoa, SuaHangLoat, XoaHangLoat, XuatDuLieu, Khac, UpdatedAt, UpdatedBy
        )
        SELECT 
            @UserId, 
            Name, 
            CASE WHEN @UserId = '00100001' THEN 1 ELSE 0 END,
            CASE WHEN @UserId = '00100001' THEN 1 ELSE 0 END,
            CASE WHEN @UserId = '00100001' THEN 1 ELSE 0 END,
            CASE WHEN @UserId = '00100001' THEN 1 ELSE 0 END,
            CASE WHEN @UserId = '00100001' THEN 1 ELSE 0 END,
            CASE WHEN @UserId = '00100001' THEN 1 ELSE 0 END,
            CASE WHEN @UserId = '00100001' THEN 1 ELSE 0 END,
            CASE WHEN @UserId = '00100001' THEN 1 ELSE 0 END,
            GETDATE(), 
            'SYSTEM'
        FROM SysMenu;
    END

    SELECT * FROM [dbo].[UsUserPermission] WHERE UserId = @UserId;
END;
GO