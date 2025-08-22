
-- Bảng LoaiXe
INSERT INTO LoaiXes (LoaiXeID, TenLoaiXe, HangXe) VALUES
(N'LX001', N'Xe Giường Nằm', N'Hyundai'),
(N'LX002', N'Xe Ghế Ngồi', N'Toyota'),
(N'LX003', N'Xe Limousine', N'Ford');

SET IDENTITY_INSERT Xes ON;
INSERT INTO Xes (XeID, BienSo, TinhTrang, LoaiXeId) VALUES
(1, N'51A-12345', N'Hoat Dong', N'LX001'),
(2, N'51B-54321', N'Bao Tri', N'LX002'),
(3, N'51C-98765', N'San Sang', N'LX003'),
(4, N'79A-12345', N'Hoat Dong', N'LX001');
SET IDENTITY_INSERT Xes OFF;

-- Bảng Ghes
INSERT INTO Ghes (GheID, XeID, TenGhe, TrangThai) VALUES
(1, 1, N'A01', N'trống'), 
(2, 2, N'A02', N'trống'), 
(3, 3, N'A03', N'trống'),
(4, 1, N'B01', N'mua'),
(5, 2, N'B02', N'trống'),
(6, 3, N'B03', N'mua'),
(7, 1, N'B02', N'mua'),
(8, 1, N'B03', N'mua'),
(9, 1, N'B04', N'trống'),
(10, 1, N'B05', N'mua'),
(11, 1, N'B06', N'trống');

INSERT INTO TuyenDuongs (TuyenDuongID, TenTuyenDuong, DiemDi, DiemDen, QuangDuong) VALUES
(N'TD001', N'Tuyen Sai Gon - Ha Noi', N'Sai Gon', N'Ha Noi', 1700),
(N'TD002', N'Tuyen Da Nang - Hue', N'Da Nang', N'Hue', 100),
(N'TD003', N'Tuyen Can Tho - Sai Gon', N'Can Tho', N'Sai Gon', 200),
(N'TD004', N'LocationA', N'LocationB', N'Tuyến A - B', 100);

INSERT INTO LichTrinhs (LichTrinhID, XeID, TuyenDuongID, GioKhoiHanh, GioDen, DieuChinhGiaVe, GiaVe, NgayDen, NgayKhoiHanh)
VALUES 
(1, 1, 'TD001', '08:00:00', '10:00:00', 0, 100000, '2024-10-08', '2024-10-08'),
(2, 2, 'TD002', '09:00:00', '11:00:00', 0, 120000, '2024-10-09', '2024-10-09'),
(3, 3, 'TD003', '10:00:00', '12:00:00', 0, 150000, '2024-10-10', '2024-10-10'),
(4, 4, 'TD004', '12:00:00', '14:00:00', 0, 0, '2024-10-10', '2024-10-10');

INSERT INTO KhuyenMais (KhuyenMaiID, TenKhuyenMai, LoaiKhuyenMai, NgayBatDau, NgayKetThuc, GiaTriGiam, DieuKienApDung, TrangThaiThanhToan)
VALUES
('NONE', N'Không có', N'Chả có', '', '', 0, '', '');

DELETE FROM LichSuDoiGhes;
DELETE FROM ChiTietVeDats;
DELETE FROM LichSuGiaoDichs;
DELETE FROM HoaDons;
DELETE FROM VeXes;
DELETE FROM Ghes;
DELETE FROM LichTrinhs;
DELETE FROM KhachHangs;
DELETE FROM TaiXes;
DELETE FROM NhanViens;
DELETE FROM Xes;
DELETE FROM TuyenDuongs;
DELETE FROM LoaiXe;
DELETE FROM AspNetRoleClaims;
DELETE FROM AspNetRoles;
DELETE FROM AspNetUserClaims;
DELETE FROM AspNetUserLogins;
DELETE FROM AspNetUserRoles;
DELETE FROM AspNetUsers;
DELETE FROM AspNetUserTokens;


select * from LichTrinhs 
where NgayKhoiHanh = '2024-12-19' and TuyenDuongId = (select TuyenDuongID 
						from TuyenDuongs
						where TenTuyenDuong=N'Sai Gon - Bac Lieu')

