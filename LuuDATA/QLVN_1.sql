USE [IDI_QLVN]
GO

-- =============================================================================
-- 1. NHÓM USER VÀ QUYỀN
-- =============================================================================

CREATE TABLE [dbo].[UsGroup] (
    [Id]          NVARCHAR(3)   NOT NULL,
    [Name]        NVARCHAR(100) NOT NULL,
    [Note]        NVARCHAR(200) NULL,
    [RowStatus]   INT           NOT NULL,
    [CreatedAt]   DATETIME      NOT NULL,
    [CreatedBy]   NVARCHAR(8)   NULL,
    [UpdatedAt]   DATETIME      NOT NULL,
    [UpdatedBy]   NVARCHAR(8)   NULL,
    CONSTRAINT [PK_UsGroup] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[UsUser] (
    [Id]          NVARCHAR(8)   NOT NULL,
    [GroupId]     NVARCHAR(3)   NOT NULL,
    [Name]        NVARCHAR(100) NOT NULL,
    [Gender]      INT           NULL,
    [UserName]    NVARCHAR(10)  NOT NULL,
    [Password]    NVARCHAR(50)  NOT NULL,
    [Email]       NVARCHAR(30)  NULL,
    [Phone]       NVARCHAR(50)  NULL,
    [CMND]        NVARCHAR(20)  NULL,
    [Address]     NVARCHAR(200) NULL,
    [Image]       NVARCHAR(200) NULL,
    [Note]        NVARCHAR(100) NULL,
    [RowStatus]   INT           NOT NULL,
    [CreatedAt]   DATETIME      NOT NULL,
    [CreatedBy]   NVARCHAR(8)   NOT NULL,
    [UpdatedAt]   DATETIME      NOT NULL,
    [UpdatedBy]   NVARCHAR(8)   NOT NULL,
    [Theme]       NVARCHAR(MAX) NOT NULL,
    CONSTRAINT [PK_UsUser] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UsUser_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[UsGroup] ([Id]),
    CONSTRAINT [FK_UsUser_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_UsUser_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[UsUserLog] (
    [Id]           NVARCHAR(30)  NOT NULL,
    [UserId]       NVARCHAR(8)   NOT NULL,
    [Menu]         NVARCHAR(50)  NOT NULL,
    [ComputerName] NVARCHAR(50)  NOT NULL,
    [ActionName]   NVARCHAR(100) NOT NULL,
    [Data]         NVARCHAR(3000) NULL,
    [Note]         NVARCHAR(2000) NULL,
    [ActionDate]   DATETIME      NOT NULL,
    CONSTRAINT [PK_UsUserLog] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UsUserLog_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[UsUserPermission] (
    [UserId]      NVARCHAR(8)   NOT NULL,
    [MenuId]      NVARCHAR(50)  NOT NULL,
    [Xem]         BIT           NULL,
    [Them]        BIT           NULL,
    [Sua]         BIT           NULL,
    [SuaHangLoat] BIT           NULL,
    [Xoa]         BIT           NULL,
    [XoaHangLoat] BIT           NULL,
    [XuatDuLieu]  BIT           NULL,
    [Khac]        BIT           NULL,
    [UpdatedAt]   DATETIME      NOT NULL,
    [UpdatedBy]   NVARCHAR(8)   NULL,
    CONSTRAINT [PK_UsUserPermission] PRIMARY KEY CLUSTERED ([UserId] ASC, [MenuId] ASC)
);
GO

CREATE TABLE [dbo].[UsGridLayout] (
    [Id]        NVARCHAR(300) NOT NULL,
    [UserId]    NVARCHAR(8)   NOT NULL,
    [Layout]    NVARCHAR(MAX) NULL,
    [UpdatedAt] DATETIME      NULL,
    CONSTRAINT [PK_UsGridLayout] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UsGridLayout_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

-- =============================================================================
-- 2. NHÓM HỆ THỐNG
-- =============================================================================

CREATE TABLE [dbo].[SysMenu] (
    [Name]       NVARCHAR(50) NOT NULL,
    [ParentMenu] NVARCHAR(50) NULL,
    [Note]       NVARCHAR(100) NOT NULL,
    [IsActive]   INT          NULL,
    CONSTRAINT [PK_SysMenu] PRIMARY KEY CLUSTERED ([Name] ASC)
);
GO

CREATE TABLE [dbo].[SysSetting] (
    [Key]         NVARCHAR(30)  NOT NULL,
    [Description] NVARCHAR(100) NULL,
    [Value]       NVARCHAR(500) NOT NULL,
    [Type]        INT           NOT NULL,
    [UpdatedAt]   DATETIME      NOT NULL,
    [UpdatedBy]   NVARCHAR(8)   NOT NULL,
    CONSTRAINT [PK_SysSetting] PRIMARY KEY CLUSTERED ([Key] ASC),
    CONSTRAINT [FK_SysSetting_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[SysSystemInfo] (
    [CTYMa]          NVARCHAR(5)   NOT NULL,
    [CTYTen]         NVARCHAR(200) NULL,
    [CTYDiaChi]      NVARCHAR(500) NULL,
    [CTYMaSoThue]    NVARCHAR(20)  NULL,
    [CTYDienThoai]   NVARCHAR(30)  NULL,
    [CTYFax]         NVARCHAR(50)  NULL,
    [CTYEmail]       NVARCHAR(30)  NULL,
    [CTYSoTaiKhoan]  NVARCHAR(20)  NULL,
    [CTYTenNganHang] NVARCHAR(100) NULL,
    [VersionApp]     NVARCHAR(10)  NULL,
    [UpdatedAt]      DATETIME      NOT NULL,
    [UpdatedBy]      NVARCHAR(8)   NOT NULL,
    CONSTRAINT [PK_SysSystemInfo] PRIMARY KEY CLUSTERED ([CTYMa] ASC),
    CONSTRAINT [FK_SysSystemInfo_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[SysIdGenerated] (
    [Table]      NVARCHAR(20) NOT NULL,
    [TotalRows]  INT          NOT NULL,
    [UpdatedAt]  DATETIME     NOT NULL,
    [UpdatedBy]  NVARCHAR(8)  NOT NULL,
    CONSTRAINT [PK_SysIdGenerated] PRIMARY KEY CLUSTERED ([Table] ASC),
    CONSTRAINT [FK_SysIdGenerated_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

-- =============================================================================
-- 3. NHÓM QUẢN LÝ AO NUÔI VÀ NGHIỆP VỤ
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
    CONSTRAINT [PK_DbDvsd] PRIMARY KEY CLUSTERED ([Ma] ASC),
    CONSTRAINT [FK_DbDvsd_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_DbDvsd_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
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
    CONSTRAINT [PK_DbKhuVuc] PRIMARY KEY CLUSTERED ([Ma] ASC),
    CONSTRAINT [FK_DbKhuVuc_DvsdMa] FOREIGN KEY ([DvsdMa]) REFERENCES [dbo].[DbDvsd] ([Ma]),
    CONSTRAINT [FK_DbKhuVuc_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_DbKhuVuc_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[DbKhachHang] (
    [Ma]          NVARCHAR(5)    NOT NULL,
    [Ten]         NVARCHAR(50)   NOT NULL,
    [DiaChi]      NVARCHAR(500)  NULL,
    [Phone]       NVARCHAR(50)   NULL,
    [CCCD]        NVARCHAR(50)   NULL,
    [TenNganHang] NVARCHAR(500)  NULL,
    [STK]         NVARCHAR(50)   NULL,
    [GoogleMap]   NVARCHAR(MAX)  NULL,
    [Note]        NVARCHAR(100)  NULL,
    [DvsdMa]      NVARCHAR(5)    NOT NULL,
    [RowStatus]   INT            NOT NULL,
    [CreatedAt]   DATETIME       NOT NULL,
    [CreatedBy]   NVARCHAR(8)    NOT NULL,
    [UpdatedAt]   DATETIME       NOT NULL,
    [UpdatedBy]   NVARCHAR(8)    NOT NULL,
    CONSTRAINT [PK_DbKhachHang] PRIMARY KEY CLUSTERED ([Ma] ASC),
    CONSTRAINT [FK_DbKhachHang_DvsdMa] FOREIGN KEY ([DvsdMa]) REFERENCES [dbo].[DbDvsd] ([Ma]),
    CONSTRAINT [FK_DbKhachHang_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_DbKhachHang_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[DbAoNuoi] (
    [Ma]            NVARCHAR(5)    NOT NULL,
    [DvsdMa]        NVARCHAR(5)    NOT NULL,
    [MaSo]          NVARCHAR(50)   NULL,
    [Ten]           NVARCHAR(50)   NOT NULL,
    [NgayCap]       DATE           NULL,
    [DiaChi]        NVARCHAR(500)  NULL,
    [DienTich]      NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [NgayThuHoach]  DATE           NULL,
    [SLDuKien]      NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [SoHD]          NCHAR(10)      NULL,
    [NgayHD]        NVARCHAR(50)   NULL,
    [GiaGiaCong]    NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [NhanVienGSMa]  NVARCHAR(50)   NULL,
    [CongXuatNuoi]  NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [TinhTrang]     BIT            NOT NULL,
    [KhachHangMa]   NVARCHAR(5)    NOT NULL,
    [KhuVucMa]      NVARCHAR(5)    NOT NULL,
    [GoogleMap]     NVARCHAR(MAX)  NULL,
    [Note]          NVARCHAR(100)  NULL,
    [RowStatus]     INT            NOT NULL,
    [CreatedAt]     DATETIME       NOT NULL,
    [CreatedBy]     NVARCHAR(8)    NOT NULL,
    [UpdatedAt]     DATETIME       NOT NULL,
    [UpdatedBy]     NVARCHAR(8)    NOT NULL,
    CONSTRAINT [PK_DbAoNuoi] PRIMARY KEY CLUSTERED ([Ma] ASC),
    CONSTRAINT [FK_DbAoNuoi_DvsdMa] FOREIGN KEY ([DvsdMa]) REFERENCES [dbo].[DbDvsd] ([Ma]),
    CONSTRAINT [FK_DbAoNuoi_KhachHangMa] FOREIGN KEY ([KhachHangMa]) REFERENCES [dbo].[DbKhachHang] ([Ma]),
    CONSTRAINT [FK_DbAoNuoi_KhuVucMa] FOREIGN KEY ([KhuVucMa]) REFERENCES [dbo].[DbKhuVuc] ([Ma]),
    CONSTRAINT [FK_DbAoNuoi_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_DbAoNuoi_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[DbHoaChat] (
    [Ma]        NVARCHAR(5)   NOT NULL,
    [Ten]       NVARCHAR(50)  NOT NULL,
    [Note]      NVARCHAR(100) NULL,
    [DvsdMa]    NVARCHAR(5)   NOT NULL,
    [RowStatus] INT           NOT NULL,
    [CreatedAt] DATETIME      NOT NULL,
    [CreatedBy] NVARCHAR(8)   NOT NULL,
    [UpdatedAt] DATETIME      NOT NULL,
    [UpdatedBy] NVARCHAR(8)   NOT NULL,
    CONSTRAINT [PK_DbHoaChat] PRIMARY KEY CLUSTERED ([Ma] ASC),
    CONSTRAINT [FK_DbHoaChat_DvsdMa] FOREIGN KEY ([DvsdMa]) REFERENCES [dbo].[DbDvsd] ([Ma]),
    CONSTRAINT [FK_DbHoaChat_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_DbHoaChat_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[DbLoaiBenh] (
    [Ma]        NVARCHAR(5)   NOT NULL,
    [Ten]       NVARCHAR(50)  NOT NULL,
    [Note]      NVARCHAR(100) NULL,
    [DvsdMa]    NVARCHAR(5)   NOT NULL,
    [RowStatus] INT           NOT NULL,
    [CreatedAt] DATETIME      NOT NULL,
    [CreatedBy] NVARCHAR(8)   NOT NULL,
    [UpdatedAt] DATETIME      NOT NULL,
    [UpdatedBy] NVARCHAR(8)   NOT NULL,
    CONSTRAINT [PK_DbLoaiBenh] PRIMARY KEY CLUSTERED ([Ma] ASC),
    CONSTRAINT [FK_DbLoaiBenh_DvsdMa] FOREIGN KEY ([DvsdMa]) REFERENCES [dbo].[DbDvsd] ([Ma]),
    CONSTRAINT [FK_DbLoaiBenh_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_DbLoaiBenh_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[DbLoaiCa] (
    [Ma]        NVARCHAR(5)   NOT NULL,
    [Ten]       NVARCHAR(50)  NOT NULL,
    [Note]      NVARCHAR(100) NULL,
    [DvsdMa]    NVARCHAR(5)   NOT NULL,
    [RowStatus] INT           NOT NULL,
    [CreatedAt] DATETIME      NOT NULL,
    [CreatedBy] NVARCHAR(8)   NOT NULL,
    [UpdatedAt] DATETIME      NOT NULL,
    [UpdatedBy] NVARCHAR(8)   NOT NULL,
    CONSTRAINT [PK_DbLoaiCa] PRIMARY KEY CLUSTERED ([Ma] ASC),
    CONSTRAINT [FK_DbLoaiCa_DvsdMa] FOREIGN KEY ([DvsdMa]) REFERENCES [dbo].[DbDvsd] ([Ma]),
    CONSTRAINT [FK_DbLoaiCa_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_DbLoaiCa_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[DbSizeNL] (
    [Ma]        NVARCHAR(5)   NOT NULL,
    [Ten]       NVARCHAR(50)  NOT NULL,
    [Note]      NVARCHAR(100) NULL,
    [DvsdMa]    NVARCHAR(5)   NOT NULL,
    [RowStatus] INT           NOT NULL,
    [CreatedAt] DATETIME      NOT NULL,
    [CreatedBy] NVARCHAR(8)   NOT NULL,
    [UpdatedAt] DATETIME      NOT NULL,
    [UpdatedBy] NVARCHAR(8)   NOT NULL,
    CONSTRAINT [PK_DbSizeNL] PRIMARY KEY CLUSTERED ([Ma] ASC),
    CONSTRAINT [FK_DbSizeNL_DvsdMa] FOREIGN KEY ([DvsdMa]) REFERENCES [dbo].[DbDvsd] ([Ma]),
    CONSTRAINT [FK_DbSizeNL_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_DbSizeNL_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[DbTangTrong] (
    [Ma]        NVARCHAR(5)   NOT NULL,
    [Ten]       NVARCHAR(50)  NOT NULL,
    [Note]      NVARCHAR(100) NULL,
    [DvsdMa]    NVARCHAR(5)   NOT NULL,
    [RowStatus] INT           NOT NULL,
    [CreatedAt] DATETIME      NOT NULL,
    [CreatedBy] NVARCHAR(8)   NOT NULL,
    [UpdatedAt] DATETIME      NOT NULL,
    [UpdatedBy] NVARCHAR(8)   NOT NULL,
    CONSTRAINT [PK_DbTangTrong] PRIMARY KEY CLUSTERED ([Ma] ASC),
    CONSTRAINT [FK_DbTangTrong_DvsdMa] FOREIGN KEY ([DvsdMa]) REFERENCES [dbo].[DbDvsd] ([Ma]),
    CONSTRAINT [FK_DbTangTrong_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_DbTangTrong_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[DbNhaCungCap] (
    [Ma]          NVARCHAR(5)    NOT NULL,
    [Ten]         NVARCHAR(50)   NOT NULL,
    [DiaChi]      NVARCHAR(500)  NULL,
    [Phone]       NVARCHAR(50)   NULL,
    [CCCD]        NVARCHAR(50)   NULL,
    [TenNganHang] NVARCHAR(500)  NULL,
    [STK]         NVARCHAR(50)   NULL,
    [GoogleMap]   NVARCHAR(MAX)  NULL,
    [Note]        NVARCHAR(100)  NULL,
    [DvsdMa]      NVARCHAR(5)    NOT NULL,
    [RowStatus]   INT            NOT NULL,
    [CreatedAt]   DATETIME       NOT NULL,
    [CreatedBy]   NVARCHAR(8)    NOT NULL,
    [UpdatedAt]   DATETIME       NOT NULL,
    [UpdatedBy]   NVARCHAR(8)    NOT NULL,
    CONSTRAINT [PK_DbNhaCungCap] PRIMARY KEY CLUSTERED ([Ma] ASC),
    CONSTRAINT [FK_DbNhaCungCap_DvsdMa] FOREIGN KEY ([DvsdMa]) REFERENCES [dbo].[DbDvsd] ([Ma]),
    CONSTRAINT [FK_DbNhaCungCap_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_DbNhaCungCap_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[DbNhapTA] (
    [Ma]           NVARCHAR(5)    NOT NULL,
    [KhuVucMa]     NVARCHAR(5)    NOT NULL,
    [NhaCungCapMa] NVARCHAR(5)    NOT NULL,
    [Ngay]         DATE           NOT NULL,
    [TA_2mm]       NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [TA_3mm]       NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [TA_5mm]       NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [Note]         NVARCHAR(100)  NULL,
    [DvsdMa]       NVARCHAR(5)    NOT NULL,
    [RowStatus]    INT            NOT NULL,
    [CreatedAt]    DATETIME       NOT NULL,
    [CreatedBy]    NVARCHAR(8)    NOT NULL,
    [UpdatedAt]    DATETIME       NOT NULL,
    [UpdatedBy]    NVARCHAR(8)    NOT NULL,
    CONSTRAINT [PK_DbNhapTA] PRIMARY KEY CLUSTERED ([Ma] ASC),
    CONSTRAINT [FK_DbNhapTA_KhuVucMa] FOREIGN KEY ([KhuVucMa]) REFERENCES [dbo].[DbKhuVuc] ([Ma]),
    CONSTRAINT [FK_DbNhapTA_NhaCungCapMa] FOREIGN KEY ([NhaCungCapMa]) REFERENCES [dbo].[DbNhaCungCap] ([Ma]),
    CONSTRAINT [FK_DbNhapTA_DvsdMa] FOREIGN KEY ([DvsdMa]) REFERENCES [dbo].[DbDvsd] ([Ma]),
    CONSTRAINT [FK_DbNhapTA_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_DbNhapTA_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[DbAoNuoi_NhapCaHao] (
    [Ma]        NVARCHAR(5)    NOT NULL,
    [AoNuoiMa]  NVARCHAR(5)    NOT NULL,
    [Ngay]      DATE           NOT NULL,
    [SoCon]     NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [SoKg]      NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [Note]      NVARCHAR(100)  NULL,
    [RowStatus] INT            NOT NULL,
    [CreatedAt] DATETIME       NOT NULL,
    [CreatedBy] NVARCHAR(8)    NOT NULL,
    [UpdatedAt] DATETIME       NOT NULL,
    [UpdatedBy] NVARCHAR(8)    NOT NULL,
    CONSTRAINT [PK_DbAoNuoi_NhapCaHao] PRIMARY KEY CLUSTERED ([Ma] ASC),
    CONSTRAINT [FK_DbAoNuoi_NhapCaHao_AoNuoiMa] FOREIGN KEY ([AoNuoiMa]) REFERENCES [dbo].[DbAoNuoi] ([Ma]),
    CONSTRAINT [FK_DbAoNuoi_NhapCaHao_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_DbAoNuoi_NhapCaHao_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[DbAoNuoi_NhapHC] (
    [Ma]        NVARCHAR(5)    NOT NULL,
    [AoNuoiMa]  NVARCHAR(5)    NOT NULL,
    [Ngay]      DATE           NOT NULL,
    [HoaChatMa] NVARCHAR(5)    NOT NULL,
    [MaSoLo]    NVARCHAR(10)   NULL,
    [SoLuong]   NUMERIC(18, 2) NOT NULL DEFAULT 0,
    [Note]      NVARCHAR(100)  NULL,
    [RowStatus] INT            NOT NULL,
    [CreatedAt] DATETIME       NOT NULL,
    [CreatedBy] NVARCHAR(8)    NOT NULL,
    [UpdatedAt] DATETIME       NOT NULL,
    [UpdatedBy] NVARCHAR(8)    NOT NULL,
    CONSTRAINT [PK_DbAoNuoi_NhapHC] PRIMARY KEY CLUSTERED ([Ma] ASC),
    CONSTRAINT [FK_DbAoNuoi_NhapHC_AoNuoiMa] FOREIGN KEY ([AoNuoiMa]) REFERENCES [dbo].[DbAoNuoi] ([Ma]),
    CONSTRAINT [FK_DbAoNuoi_NhapHC_HoaChatMa] FOREIGN KEY ([HoaChatMa]) REFERENCES [dbo].[DbHoaChat] ([Ma]),
    CONSTRAINT [FK_DbAoNuoi_NhapHC_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_DbAoNuoi_NhapHC_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[DbAoNuoi_NhapKC] (
    [Ma]          NVARCHAR(5)    NOT NULL,
    [AoNuoiMa]    NVARCHAR(5)    NOT NULL,
    [NgayKiem]    DATE           NOT NULL,
    [LuongGiong]  NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [MauGiong]    NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [LuongThucAn] NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [Note]        NVARCHAR(100)  NULL,
    [RowStatus]   INT            NOT NULL,
    [CreatedAt]   DATETIME       NOT NULL,
    [CreatedBy]   NVARCHAR(8)    NOT NULL,
    [UpdatedAt]   DATETIME       NOT NULL,
    [UpdatedBy]   NVARCHAR(8)    NOT NULL,
    CONSTRAINT [PK_DbAoNuoi_NhapKC] PRIMARY KEY CLUSTERED ([Ma] ASC),
    CONSTRAINT [FK_DbAoNuoi_NhapKC_AoNuoiMa] FOREIGN KEY ([AoNuoiMa]) REFERENCES [dbo].[DbAoNuoi] ([Ma]),
    CONSTRAINT [FK_DbAoNuoi_NhapKC_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_DbAoNuoi_NhapKC_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[DbAoNuoi_NhapKhac] (
    [Ma]        NVARCHAR(5)    NOT NULL,
    [AoNuoiMa]  NVARCHAR(5)    NOT NULL,
    [Ngay]      DATE           NOT NULL,
    [ThayNuoc]  NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [TTBQ]      NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [CaBenhMa]  NVARCHAR(5)    NULL,
    [Note]      NVARCHAR(100)  NULL,
    [RowStatus] INT            NOT NULL,
    [CreatedAt] DATETIME       NOT NULL,
    [CreatedBy] NVARCHAR(8)    NOT NULL,
    [UpdatedAt] DATETIME       NOT NULL,
    [UpdatedBy] NVARCHAR(8)    NOT NULL,
    CONSTRAINT [PK_DbAoNuoi_NhapKhac] PRIMARY KEY CLUSTERED ([Ma] ASC),
    CONSTRAINT [FK_DbAoNuoi_NhapKhac_AoNuoiMa] FOREIGN KEY ([AoNuoiMa]) REFERENCES [dbo].[DbAoNuoi] ([Ma]),
    CONSTRAINT [FK_DbAoNuoi_NhapKhac_CaBenhMa] FOREIGN KEY ([CaBenhMa]) REFERENCES [dbo].[DbLoaiBenh] ([Ma]),
    CONSTRAINT [FK_DbAoNuoi_NhapKhac_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_DbAoNuoi_NhapKhac_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[DbAoNuoi_NhapMT] (
    [Ma]        NVARCHAR(5)    NOT NULL,
    [AoNuoiMa]  NVARCHAR(5)    NOT NULL,
    [Ngay]      DATE           NOT NULL,
    [NhietDo]   NUMERIC(18, 1) NOT NULL DEFAULT 0,
    [PH_Sang]   NUMERIC(18, 1) NOT NULL DEFAULT 0,
    [PH_Chieu]  NUMERIC(18, 1) NOT NULL DEFAULT 0,
    [O2_Sang]   NUMERIC(18, 1) NOT NULL DEFAULT 0,
    [O2_Chieu]  NUMERIC(18, 1) NOT NULL DEFAULT 0,
    [NH3]       NUMERIC(18, 2) NOT NULL DEFAULT 0,
    [H2S]       NUMERIC(18, 2) NOT NULL DEFAULT 0,
    [KH]        NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [Note]      NVARCHAR(100)  NULL,
    [RowStatus] INT            NOT NULL,
    [CreatedAt] DATETIME       NOT NULL,
    [CreatedBy] NVARCHAR(8)    NOT NULL,
    [UpdatedAt] DATETIME       NOT NULL,
    [UpdatedBy] NVARCHAR(8)    NOT NULL,
    CONSTRAINT [PK_DbAoNuoi_NhapMT] PRIMARY KEY CLUSTERED ([Ma] ASC),
    CONSTRAINT [FK_DbAoNuoi_NhapMT_AoNuoiMa] FOREIGN KEY ([AoNuoiMa]) REFERENCES [dbo].[DbAoNuoi] ([Ma]),
    CONSTRAINT [FK_DbAoNuoi_NhapMT_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_DbAoNuoi_NhapMT_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[DbAoNuoi_NhapSL] (
    [Ma]         NVARCHAR(5)    NOT NULL,
    [AoNuoiMa]   NVARCHAR(5)    NOT NULL,
    [NgayKiem]   DATE           NOT NULL,
    [LuongGiong] NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [Note]       NVARCHAR(100)  NULL,
    [RowStatus]  INT            NOT NULL,
    [CreatedAt]  DATETIME       NOT NULL,
    [CreatedBy]  NVARCHAR(8)    NOT NULL,
    [UpdatedAt]  DATETIME       NOT NULL,
    [UpdatedBy]  NVARCHAR(8)    NOT NULL,
    CONSTRAINT [PK_DbAoNuoi_NhapSL] PRIMARY KEY CLUSTERED ([Ma] ASC),
    CONSTRAINT [FK_DbAoNuoi_NhapSL_AoNuoiMa] FOREIGN KEY ([AoNuoiMa]) REFERENCES [dbo].[DbAoNuoi] ([Ma]),
    CONSTRAINT [FK_DbAoNuoi_NhapSL_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_DbAoNuoi_NhapSL_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[DbAoNuoi_NhapTA] (
    [Ma]        NVARCHAR(5)    NOT NULL,
    [AoNuoiMa]  NVARCHAR(5)    NOT NULL,
    [Ngay]      DATE           NOT NULL,
    [Ten]       NVARCHAR(MAX)  NULL,
    [MaLo]      NVARCHAR(10)   NULL,
    [TA_2mm]    NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [TA_3mm]    NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [TA_5mm]    NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [Note]      NVARCHAR(100)  NULL,
    [RowStatus] INT            NOT NULL,
    [CreatedAt] DATETIME       NOT NULL,
    [CreatedBy] NVARCHAR(8)    NOT NULL,
    [UpdatedAt] DATETIME       NOT NULL,
    [UpdatedBy] NVARCHAR(8)    NOT NULL,
    CONSTRAINT [PK_DbAoNuoi_NhapTA] PRIMARY KEY CLUSTERED ([Ma] ASC),
    CONSTRAINT [FK_DbAoNuoi_NhapTA_AoNuoiMa] FOREIGN KEY ([AoNuoiMa]) REFERENCES [dbo].[DbAoNuoi] ([Ma]),
    CONSTRAINT [FK_DbAoNuoi_NhapTA_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_DbAoNuoi_NhapTA_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[DbAoNuoi_NhapTG] (
    [Ma]          NVARCHAR(5)    NOT NULL,
    [AoNuoiMa]    NVARCHAR(5)    NOT NULL,
    [NgayThaGiong] DATE          NOT NULL,
    [LuongGiong]  NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [MauGiong]    NUMERIC(18, 0) NULL,
    [GiaGiong]    NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [Note]        NVARCHAR(100)  NULL,
    [RowStatus]   INT            NOT NULL,
    [CreatedAt]   DATETIME       NOT NULL,
    [CreatedBy]   NVARCHAR(8)    NOT NULL,
    [UpdatedAt]   DATETIME       NOT NULL,
    [UpdatedBy]   NVARCHAR(8)    NOT NULL,
    CONSTRAINT [PK_DbAoNuoi_NhapTG] PRIMARY KEY CLUSTERED ([Ma] ASC),
    CONSTRAINT [FK_DbAoNuoi_NhapTG_AoNuoiMa] FOREIGN KEY ([AoNuoiMa]) REFERENCES [dbo].[DbAoNuoi] ([Ma]),
    CONSTRAINT [FK_DbAoNuoi_NhapTG_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_DbAoNuoi_NhapTG_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[DbAoNuoi_NhapTH] (
    [Ma]        NVARCHAR(5)    NOT NULL,
    [AoNuoiMa]  NVARCHAR(5)    NOT NULL,
    [Ngay]      DATE           NOT NULL,
    [Size]      NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [SanLuong]  NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [TA_2mm]    NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [TA_3mm]    NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [TA_5mm]    NUMERIC(18, 0) NOT NULL DEFAULT 0,
    [Note]      NVARCHAR(100)  NULL,
    [RowStatus] INT            NOT NULL,
    [CreatedAt] DATETIME       NOT NULL,
    [CreatedBy] NVARCHAR(8)    NOT NULL,
    [UpdatedAt] DATETIME       NOT NULL,
    [UpdatedBy] NVARCHAR(8)    NOT NULL,
    CONSTRAINT [PK_DbAoNuoi_NhapTH] PRIMARY KEY CLUSTERED ([Ma] ASC),
    CONSTRAINT [FK_DbAoNuoi_NhapTH_AoNuoiMa] FOREIGN KEY ([AoNuoiMa]) REFERENCES [dbo].[DbAoNuoi] ([Ma]),
    CONSTRAINT [FK_DbAoNuoi_NhapTH_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_DbAoNuoi_NhapTH_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[DbKhangSinh_KQ] (
    [Ma]            NVARCHAR(5)    NOT NULL,
    [AoNuoiMa]      NVARCHAR(5)    NOT NULL,
    [MaKhangSinh]   NVARCHAR(50)   NOT NULL,
    [NgayKiem]      DATE           NOT NULL,
    [AOZ]           NVARCHAR(10)   NULL,
    [CAP]           NVARCHAR(10)   NULL,
    [FLU]           NVARCHAR(10)   NULL,
    [ENRO]          NVARCHAR(10)   NULL,
    [MGLMG]         NVARCHAR(10)   NULL,
    [TRF]           NVARCHAR(10)   NULL,
    [AMOZ]          NVARCHAR(10)   NULL,
    [AHD]           NVARCHAR(10)   NULL,
    [SEM]           NVARCHAR(10)   NULL,
    [TenNguoiKiem]  NVARCHAR(200)  NULL,
    [Note]          NVARCHAR(100)  NULL,
    [RowStatus]     INT            NOT NULL,
    [CreatedAt]     DATETIME       NOT NULL,
    [CreatedBy]     NVARCHAR(8)    NOT NULL,
    [UpdatedAt]     DATETIME       NOT NULL,
    [UpdatedBy]     NVARCHAR(8)    NOT NULL,
    CONSTRAINT [PK_DbKhangSinh_KQ] PRIMARY KEY CLUSTERED ([Ma] ASC),
    CONSTRAINT [FK_DbKhangSinh_KQ_AoNuoiMa] FOREIGN KEY ([AoNuoiMa]) REFERENCES [dbo].[DbAoNuoi] ([Ma]),
    CONSTRAINT [FK_DbKhangSinh_KQ_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_DbKhangSinh_KQ_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

CREATE TABLE [dbo].[DbKhangSinh_YC] (
    [Ma]           NVARCHAR(5)   NOT NULL,
    [AoNuoiMa]     NVARCHAR(5)   NOT NULL,
    [NoiDungKiem]  NVARCHAR(MAX) NULL,
    [NgayYeuCau]   DATE          NOT NULL,
    [AOZ]          BIT           NOT NULL DEFAULT 0,
    [CAP]          BIT           NOT NULL DEFAULT 0,
    [FLU]          BIT           NOT NULL DEFAULT 0,
    [ENRO]         BIT           NOT NULL DEFAULT 0,
    [MGLMG]        BIT           NOT NULL DEFAULT 0,
    [TRF]          BIT           NOT NULL DEFAULT 0,
    [AMOZ]         BIT           NOT NULL DEFAULT 0,
    [AHD]          BIT           NOT NULL DEFAULT 0,
    [SEM]          BIT           NOT NULL DEFAULT 0,
    [Note]         BIT           NOT NULL DEFAULT 0,
    [RowStatus]    INT           NOT NULL,
    [CreatedAt]    DATETIME      NOT NULL,
    [CreatedBy]    NVARCHAR(8)   NOT NULL,
    [UpdatedAt]    DATETIME      NOT NULL,
    [UpdatedBy]    NVARCHAR(8)   NOT NULL,
    CONSTRAINT [PK_DbKhangSinh_YC] PRIMARY KEY CLUSTERED ([Ma] ASC),
    CONSTRAINT [FK_DbKhangSinh_YC_AoNuoiMa] FOREIGN KEY ([AoNuoiMa]) REFERENCES [dbo].[DbAoNuoi] ([Ma]),
    CONSTRAINT [FK_DbKhangSinh_YC_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UsUser] ([Id]),
    CONSTRAINT [FK_DbKhangSinh_YC_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[UsUser] ([Id])
);
GO

-- =============================================================================
-- 4. VIEW & STORED PROCEDURES
-- =============================================================================

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
FROM 
    dbo.UsGroup G
INNER JOIN 
    dbo.UsUser U ON G.Id = U.GroupId
INNER JOIN 
    dbo.UsUserLog L ON U.Id = L.UserId;
GO

CREATE OR ALTER PROCEDURE [dbo].[INSERT_MENU]
AS
BEGIN
    SET NOCOUNT ON;
    -- Insert into [dbo].[SysMenu] ([Name],[ParentMenu],[Note],[IsActive])
    -- -- values ('barBaoCaoXKTangTrong', 'ribbonPageGroupBaoCaoXK',N'Khối lượng BTP theo loại tăng trọng, loại cá',1)
    -- INSERT INTO [dbo].[UsUserPermission]([UserId],[MenuId],[Xem],[Them],[Sua],[SuaHangLoat],[Xoa],[XoaHangLoat],[XuatDuLieu],[UpdatedAt],[UpdatedBy])
    -- SELECT DISTINCT [UserId],'barBaoCaoXKTangTrong',1,1,1,1,1,1,1,getdate(),'00100001' FROM [dbo].[UsUserPermission]
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_drop_constraint]
AS
BEGIN
    SET NOCOUNT ON;
    -- Insert statements for procedure here
    ALTER TABLE [dbo].[DbBoPhan] DROP CONSTRAINT [FK_DbBoPhan_UsUser];
    ALTER TABLE [dbo].[DbBoTriNhanVien] DROP CONSTRAINT [FK_DbBoTriNhanVien_DbLoCa],[FK_DbBoTriNhanVien_DbNhanVien],[FK_DbBoTriNhanVien_DbSizeCa],[FK_DbBoTriNhanVien_UsUserCreated],[FK_DbBoTriNhanVien_UsUserUpdated];
    ALTER TABLE [dbo].[DbCongDoan] DROP CONSTRAINT [FK_DbCongDoan_UsUser];
    ALTER TABLE [dbo].[DbDonGia] DROP CONSTRAINT [FK_DbDonGia_DbLoaiCa],[FK_DbDonGia_UsUserCreated],[FK_DbDonGia_UsUserUpdated];
    ALTER TABLE [dbo].[DbDonGiaDetail] DROP CONSTRAINT [FK_DbDonGiaDetail_DbCongDoan],[FK_DbDonGiaDetail_DbDonGia],[FK_DbDonGiaDetail_DbSizeCa];
    ALTER TABLE [dbo].DbDonGiaQuanLy DROP CONSTRAINT [FK_DbDonGiaQuanLy_UsUser_Created],[FK_DbDonGiaQuanLy_UsUser_Updated];
    ALTER TABLE [dbo].[DbDonGiaThoiVuFillet] DROP CONSTRAINT [FK_DbDonGiaThoiVuFillet_DbLoaiCa],[FK_DbDonGiaThoiVuFillet_UsUser],[FK_DbDonGiaThoiVuFillet_UsUser1];
    ALTER TABLE [dbo].[DbDonGiaThoiVuFilletDetail] DROP CONSTRAINT [FK_DbDonGiaThoiVuFilletDetail_DbCongDoan],[FK_DbDonGiaThoiVuFilletDetail_DbDonGiaThoiVuFillet],[FK_DbDonGiaThoiVuFilletDetail_DbDonGiaThoiVuFillet_LoaiSize],[FK_DbDonGiaThoiVuFilletDetail_DbSizeCa];
    ALTER TABLE [dbo].[DbDonGiaThoiVuSuaCa] DROP CONSTRAINT [FK_DbDonGiaThoiVu_DbLoaiCa],[FK_DbDonGiaThoiVu_UsUser_Created],[FK_DbDonGiaThoiVu_UsUserUpdated];
    ALTER TABLE [dbo].[DbDonGiaThoiVuSuaCaDetail] DROP CONSTRAINT [FK_DbDonGiaThoiVuDetail_DbCongDoan],[FK_DbDonGiaThoiVuDetail_DbDonGiaThoiVu];
    ALTER TABLE [dbo].[DbHistory] DROP CONSTRAINT [FK_DbHistory_UsUserCreated];
    ALTER TABLE [dbo].[DbLoaiCa] DROP CONSTRAINT [FK_DbLoaiCa_UsUser];
    ALTER TABLE [dbo].[DbLoaiRo] DROP CONSTRAINT [FK_DbLoaiRo_UsUser];
    ALTER TABLE [dbo].[DbLoCa] DROP CONSTRAINT [FK_DbLoCa_DbSizeCa],[FK_DbLoCa_UsUserCreated],[FK_DbLoCa_UsUserUpdated];
    ALTER TABLE [dbo].[DbLoCaDetail] DROP CONSTRAINT [FK_DbLoCaDetail_DbLoCa];
    ALTER TABLE [dbo].[DbMaTo] DROP CONSTRAINT [FK_DbMaTo_UsUser];
    ALTER TABLE [dbo].[DbNhanVien] DROP CONSTRAINT [FK_DbNhanVien_UsUserCreated],[FK_DbNhanVien_UsUserUpdated];
    ALTER TABLE [dbo].[DbNhomNhanVien] DROP CONSTRAINT [FK_DbNhomNhanVien_UsUser];
    ALTER TABLE [dbo].[DbSizeCa] DROP CONSTRAINT [FK_DbSizeCa_UsUser];
    ALTER TABLE [dbo].DbTheReader DROP CONSTRAINT [FK_DbTheReader_UsUserCreated],[FK_DbTheReader_UsUserUpdated];
    ALTER TABLE [dbo].DbXk_LoaiCa DROP CONSTRAINT [FK_DbXk_LoaiCa_UsUser_CreatedByName],[FK_DbXk_LoaiCa_UsUser_UpdatedByName];
    ALTER TABLE [dbo].DbXk_LoaiTangTrong DROP CONSTRAINT [FK_DbXk_LoaiTangTrong_UsUser_CreatedByName],[FK_DbXk_LoaiTangTrong_UsUser_UpdatedByName];
    ALTER TABLE [dbo].[DbXk_NhomGomCa] DROP CONSTRAINT [FK_DbXk_NhomGomCa_UsUser_CreateByName],[FK_DbXk_NhomGomCa_UsUser_UpdateByName];
    ALTER TABLE [dbo].[DbXk_SizeCa] DROP CONSTRAINT [FK_DbXk_SizeCa_UsUser_CreatedByName],[FK_DbXk_SizeCa_UsUser_UpdatedByName];
    ALTER TABLE [dbo].[SysEndpoint] DROP CONSTRAINT [FK_SysEndpoint_UsUserUpdated];
    ALTER TABLE [dbo].SysKhuVuc DROP CONSTRAINT [FK_SysKhuVuc_UsUserCreated],[FK_SysKhuVuc_UsUserUpdated];
    ALTER TABLE [dbo].[SysMayCan] DROP CONSTRAINT [FK_SysMayTinh_SysKhuVuc],[FK_SysMayTinh_UsUserCreated],[FK_SysMayTinh_UsUserUpdated];
    ALTER TABLE [dbo].[SysSetting] DROP CONSTRAINT [FK_SysSetting_UsUserUpdated];
    ALTER TABLE [dbo].UsUserPermission DROP CONSTRAINT [FK_UsUserPermission_SysMenu],[FK_UsUserPermission_UsUser];
    ALTER TABLE [dbo].UsUser DROP CONSTRAINT [FK_UsUser_UsGroup];
    ALTER TABLE [dbo].UsUserLog DROP CONSTRAINT [FK_UsUserLog_UsUser];
    ALTER TABLE [dbo].[SysIdGenerated] DROP CONSTRAINT [FK_SysIdGenerated_UsUserUpdated];
    ALTER TABLE [dbo].[SysOAuthClient] DROP CONSTRAINT [FK_SysOAuthClient_UsUserCreated],[FK_SysOAuthClient_UsUserUpdated];
    ALTER TABLE [dbo].[SysSystemInfo] DROP CONSTRAINT [FK_SysSystemInfo_UsUserUpdated];
    ALTER TABLE [dbo].[UsGridLayout] DROP CONSTRAINT [FK_UsGridLayout_UsUser];
    ALTER TABLE [dbo].[DbNL_KhoiLuongTaiHam] DROP CONSTRAINT [FK_DbNL_KhoiLuongTaiHam_DbNL_KhachHang];
    ALTER TABLE [dbo].[DbNL_HangHoaBuTruNguyenLieu] DROP CONSTRAINT [FK_DbNL_HangHoaBuTruNguyenLieu_DbNL_HangHoa];
    ALTER TABLE [dbo].[DbNL_HangHoaBuTru] DROP CONSTRAINT [FK_DbNL_HangHoaBuTru_DbNL_HangHoaBuTru],[FK_DbNL_HangHoaBuTru_DbNL_HangHoa];
    ALTER TABLE [dbo].[DbNL_HangHoa_DonGia_VanChuyen] DROP CONSTRAINT [FK_DbNL_HangHoa_DonGia_VanChuyen_DbNL_DonViVanChuyen],[FK_DbNL_HangHoa_DonGia_VanChuyen_DbNL_HangHoa];
    ALTER TABLE [dbo].[DbNL_HangHoa_DonGia_KhachHang] DROP CONSTRAINT [FK_DbNL_HangHoa_DonGia_KhachHang_DbNL_HangHoa],[FK_DbNL_HangHoa_DonGia_KhachHang_DbNL_KhachHang];
    ALTER TABLE [dbo].[DbNL_DonViVanChuyen] DROP CONSTRAINT [FK_DbNL_DonViVanChuyen_UsUserCreated],[FK_DbNL_DonViVanChuyen_UsUserUpdated];
    ALTER TABLE [dbo].[DbNL_KhachHang] DROP CONSTRAINT [FK_DbNL_KhachHang_UsUserCreated],[FK_DbNL_KhachHang_UsUserUpdated];
    ALTER TABLE [dbo].[DbNL_HangHoa] DROP CONSTRAINT [FK_DbNL_HangHoa_UsUser];
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SpIU_UserPermission]
    @Json NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @PermissionTable AS TABLE(
        UserId NVARCHAR(8),
        MenuId NVARCHAR(50),
        Xem BIT,
        Them BIT,
        Sua BIT,
        Xoa BIT,
        SuaHangLoat BIT,
        XoaHangLoat BIT,
        XuatDuLieu BIT,
        Khac BIT,
        UpdatedAt DATETIME,
        UpdatedBy NVARCHAR(50)
    );
    INSERT INTO @PermissionTable(
        UserId, MenuId, Xem, Them, Sua, Xoa, SuaHangLoat, XoaHangLoat, XuatDuLieu, Khac, UpdatedAt, UpdatedBy
    )
    SELECT UserId, MenuId, Xem, Them, Sua, Xoa, SuaHangLoat, XoaHangLoat, XuatDuLieu, Khac, UpdatedAt, UpdatedBy
    FROM OPENJSON(@Json)
    WITH (
        UserId NVARCHAR(8),
        MenuId NVARCHAR(50),
        Xem BIT,
        Them BIT,
        Sua BIT,
        Xoa BIT,
        SuaHangLoat BIT,
        XoaHangLoat BIT,
        XuatDuLieu BIT,
        Khac BIT,
        UpdatedAt DATETIME,
        UpdatedBy NVARCHAR(50)
    );
    -- Update
    UPDATE [dbo].[UsUserPermission] SET
        [dbo].[UsUserPermission].Xem = tb.Xem,
        [dbo].[UsUserPermission].Them = tb.Them,
        [dbo].[UsUserPermission].Sua = tb.Sua,
        [dbo].[UsUserPermission].Xoa = tb.Xoa,
        [dbo].[UsUserPermission].SuaHangLoat = tb.SuaHangLoat,
        [dbo].[UsUserPermission].XoaHangLoat = tb.XoaHangLoat,
        [dbo].[UsUserPermission].XuatDuLieu = tb.XuatDuLieu,
        [dbo].[UsUserPermission].Khac = tb.Khac,
        [dbo].[UsUserPermission].UpdatedAt = GETDATE(),
        [dbo].[UsUserPermission].UpdatedBy = tb.UpdatedBy
    FROM [dbo].[UsUserPermission] INNER JOIN @PermissionTable AS tb
    ON [dbo].[UsUserPermission].MenuId = tb.MenuId
    WHERE [dbo].[UsUserPermission].UserId = tb.UserId AND [dbo].[UsUserPermission].MenuId = tb.MenuId
END
GO

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
        SELECT @UserId, Name, 
            CASE WHEN @UserId = '00100001' THEN 1 ELSE 0 END,
            CASE WHEN @UserId = '00100001' THEN 1 ELSE 0 END,
            CASE WHEN @UserId = '00100001' THEN 1 ELSE 0 END,
            CASE WHEN @UserId = '00100001' THEN 1 ELSE 0 END,
            CASE WHEN @UserId = '00100001' THEN 1 ELSE 0 END,
            CASE WHEN @UserId = '00100001' THEN 1 ELSE 0 END,
            CASE WHEN @UserId = '00100001' THEN 1 ELSE 0 END,
            CASE WHEN @UserId = '00100001' THEN 1 ELSE 0 END,
            GETDATE(), 'SYSTEM' 
        FROM SysMenu;
    END
    SELECT * FROM [dbo].[UsUserPermission] WHERE UserId = @UserId;
END;
GO
