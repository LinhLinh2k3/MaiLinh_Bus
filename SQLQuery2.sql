Create database QL_NhaXe3
use QL_NhaXe3
CREATE TABLE [dbo].[AspNetUsers] (
    [Id]                   NVARCHAR (450)     NOT NULL,
    [UserName]             NVARCHAR (256)     NULL,
    [NormalizedUserName]   NVARCHAR (256)     NULL,
    [Email]                NVARCHAR (256)     NULL,
    [NormalizedEmail]      NVARCHAR (256)     NULL,
    [EmailConfirmed]       BIT                NOT NULL,
    [PasswordHash]         NVARCHAR (MAX)     NULL,
    [SecurityStamp]        NVARCHAR (MAX)     NULL,
    [ConcurrencyStamp]     NVARCHAR (MAX)     NULL,
    [PhoneNumber]          NVARCHAR (MAX)     NULL,
    [PhoneNumberConfirmed] BIT                NOT NULL,
    [TwoFactorEnabled]     BIT                NOT NULL,
    [LockoutEnd]           DATETIMEOFFSET (7) NULL,
    [LockoutEnabled]       BIT                NOT NULL,
    [AccessFailedCount]    INT                NOT NULL,
    [Address]              NVARCHAR (MAX)     NULL,
    [Name]                 NVARCHAR (MAX)     NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);
INSERT INTO [dbo].[AspNetUsers] 
    ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], 
     [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], 
     [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], 
     [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Address], [Name])
VALUES 
    ('2da7114c-0fc8-4524-8bf4-df9a33e3b001', NULL, NULL, NULL, NULL, 
     0, NULL, NULL, NULL, NULL, 0, 0, NULL, 1, 0, NULL, NULL);
select * from AspNetUsers

CREATE TABLE [dbo].[KhachHangs] (
    [KhachHangID]   NVARCHAR (450) NOT NULL,
    [HoTen]         NVARCHAR (MAX) NULL,
    [DiaChi]        NVARCHAR (MAX) NULL,
    [SDT]           NVARCHAR (MAX) NULL,
    [Email]         NVARCHAR (MAX) NULL,
    [CCCD]          NVARCHAR (MAX) NULL,
    [HangThanhVien] NVARCHAR (MAX) NULL,
    [AppUserId]     NVARCHAR (450) NULL,
    CONSTRAINT [PK_KhachHangs] PRIMARY KEY CLUSTERED ([KhachHangID] ASC),
    CONSTRAINT [FK_KhachHangs_AspNetUsers_AppUserId] FOREIGN KEY ([AppUserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);
INSERT INTO [dbo].[KhachHangs] 
    ([KhachHangID], [HoTen], [DiaChi], [SDT], [Email], 
     [CCCD], [HangThanhVien], [AppUserId])
VALUES 
    ('64cf82d3-71aa-4edf-ac8c-c574c8355e8b', 'Khach Hang User', NULL, NULL, 
     'khachhang@gmail.com', NULL, NULL, '2da7114c-0fc8-4524-8bf4-df9a33e3b001');
select * from KhachHangs

CREATE TABLE [dbo].[TuyenDuongs] (
    [TuyenDuongID]  NVARCHAR (450) NOT NULL,
    [TenTuyenDuong] NVARCHAR (MAX) NOT NULL,
    [DiemDi]        NVARCHAR (MAX) NOT NULL,
    [DiemDen]       NVARCHAR (MAX) NOT NULL,
    [QuangDuong]    REAL           NOT NULL,
    CONSTRAINT [PK_TuyenDuongs] PRIMARY KEY CLUSTERED ([TuyenDuongID] ASC)
);
CREATE TABLE [dbo].[LoaiXe] (
    [LoaiXeID]  NVARCHAR (450) NOT NULL,
    [TenLoaiXe] NVARCHAR (MAX) NOT NULL,
    [HangXe]    NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_LoaiXe] PRIMARY KEY CLUSTERED ([LoaiXeID] ASC)
);
CREATE TABLE [dbo].[Xes] (
    [XeID]      INT            IDENTITY (1, 1) NOT NULL,
    [BienSo]    NVARCHAR (MAX) NOT NULL,
    [SoGhe]     INT            NOT NULL,
    [TinhTrang] NVARCHAR (MAX) NOT NULL,
    [LoaiXeId]  NVARCHAR (450) DEFAULT (N'') NOT NULL,
    CONSTRAINT [PK_Xes] PRIMARY KEY CLUSTERED ([XeID] ASC),
    CONSTRAINT [FK_Xes_LoaiXe_LoaiXeId] FOREIGN KEY ([LoaiXeId]) REFERENCES [dbo].[LoaiXe] ([LoaiXeID]) ON DELETE CASCADE
);


CREATE TABLE [dbo].[LichTrinhs] (
    [LichTrinhId]    NVARCHAR (450) NOT NULL,
    [XeId]           INT            NOT NULL,
    [TuyenDuongId]   NVARCHAR (450) NOT NULL,
    [GioKhoiHanh]    TIME (7)       NOT NULL,
    [GioDen]         TIME (7)       NOT NULL,
    [DieuChinhGiaVe] INT            DEFAULT ((0)) NOT NULL,
    [GiaVe]          REAL           DEFAULT (CONVERT([real],(0))) NOT NULL,
    [NgayDen]        DATE           DEFAULT ('0001-01-01') NOT NULL,
    [NgayKhoiHanh]   DATE           DEFAULT ('0001-01-01') NOT NULL,
    CONSTRAINT [PK_LichTrinhs] PRIMARY KEY CLUSTERED ([LichTrinhId] ASC),
    CONSTRAINT [FK_LichTrinhs_TuyenDuongs_TuyenDuongId] FOREIGN KEY ([TuyenDuongId]) REFERENCES [dbo].[TuyenDuongs] ([TuyenDuongID]) ON DELETE CASCADE,
    CONSTRAINT [FK_LichTrinhs_Xes_XeId] FOREIGN KEY ([XeId]) REFERENCES [dbo].[Xes] ([XeID]) ON DELETE CASCADE
);

select * from LichTrinhs
CREATE TABLE [dbo].[VeXes] (
    [VeID]        NVARCHAR (450)  NOT NULL,
    [KhachHangID] NVARCHAR (450)  NOT NULL,
    [LichTrinhID] NVARCHAR (450)  NOT NULL,
    [TongGiaVe]   DECIMAL (18, 2) NOT NULL,
    CONSTRAINT [PK_VeXes] PRIMARY KEY CLUSTERED ([VeID] ASC),
    CONSTRAINT [FK_VeXes_KhachHangs_KhachHangID] FOREIGN KEY ([KhachHangID]) REFERENCES [dbo].[KhachHangs] ([KhachHangID]) ON DELETE CASCADE,
    CONSTRAINT [FK_VeXes_LichTrinhs_LichTrinhID] FOREIGN KEY ([LichTrinhID]) REFERENCES [dbo].[LichTrinhs] ([LichTrinhId]) ON DELETE CASCADE
);


CREATE TABLE [dbo].[Ghes] (
    [GheID]     INT            NOT NULL,
    [XeID]      INT            NOT NULL,
    [TenGhe]    NVARCHAR (MAX) NOT NULL,
    [TrangThai] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Ghes] PRIMARY KEY CLUSTERED ([GheID] ASC, [XeID] ASC),
    CONSTRAINT [FK_Ghes_Xes_XeID] FOREIGN KEY ([XeID]) REFERENCES [dbo].[Xes] ([XeID]) ON DELETE CASCADE
);


CREATE TABLE [dbo].[ChiTietVeDats] (
    [ChiTietVeID]  NVARCHAR (450)  NOT NULL,
    [VeID]         NVARCHAR (450)  NOT NULL,
    [XeID]         INT             NOT NULL,
    [GiaGhe]       DECIMAL (18, 2) NOT NULL,
    [TinhTrangGhe] NVARCHAR (MAX)  NOT NULL,
    [GheID]        INT             DEFAULT ((0)) NOT NULL,
    [NgayDat]      DATETIME2 (7)   NULL,
    CONSTRAINT [PK_ChiTietVeDats] PRIMARY KEY CLUSTERED ([ChiTietVeID] ASC),
    CONSTRAINT [FK_ChiTietVeDats_VeXes_VeID] FOREIGN KEY ([VeID]) REFERENCES [dbo].[VeXes] ([VeID]),
    CONSTRAINT [FK_ChiTietVeDats_Xes_XeID] FOREIGN KEY ([XeID]) REFERENCES [dbo].[Xes] ([XeID]) ON DELETE CASCADE
);




select * from ChiTietVeDats


SELECT name 
FROM sys.default_constraints 
WHERE parent_object_id = OBJECT_ID('dbo.ChiTietVeDats') 
AND parent_column_id = COLUMNPROPERTY(OBJECT_ID('dbo.ChiTietVeDats'), 'GheID1', 'ColumnID');

ALTER TABLE [dbo].[ChiTietVeDats]
DROP CONSTRAINT [DF__ChiTietVe__GheID__5812160E];
ALTER TABLE [dbo].[ChiTietVeDats]
DROP COLUMN [GheID1];
ALTER TABLE [dbo].[ChiTietVeDats]
DROP COLUMN [GheXeID];


SELECT name 
FROM sys.default_constraints 
WHERE parent_object_id = OBJECT_ID('dbo.ChiTietVeDats') 
AND parent_column_id = COLUMNPROPERTY(OBJECT_ID('dbo.ChiTietVeDats'), 'GheXeID', 'ColumnID');

ALTER TABLE [dbo].[ChiTietVeDats]
DROP CONSTRAINT [DF__ChiTietVe__GheXe__59063A47];
ALTER TABLE [dbo].[ChiTietVeDats]
DROP COLUMN [GheXeID];


EXEC sp_help 'dbo.ChiTietVeDats';



