-- Bảng LoaiXe
INSERT INTO LoaiXes (LoaiXeID, TenLoaiXe, HangXe) VALUES
(N'LX001', N'Xe Giường Nằm', N'Hyundai'),
(N'LX002', N'Xe Ghế Ngồi', N'Toyota'),
(N'LX003', N'Xe Limousine', N'Ford');

-- Bảng TaiXes
INSERT INTO TaiXes (TaiXeID, HoTen, SDT, CCCD, BangLaiXe) VALUES
(N'TX001', N'Nguyen Van A', N'0912345678', N'123456789', N'B2'),
(N'TX002', N'Tran Thi B', N'0912345679', N'987654321', N'C'),
(N'TX003', N'Le Van C', N'0912345680', N'456789123', N'D');

-- Bảng TuyenDuongs
INSERT INTO TuyenDuongs (TuyenDuongID, TenTuyenDuong, DiemDi, DiemDen, QuangDuong) VALUES
(N'TD001', N'Tuyen Sai Gon - Ha Noi', N'Sai Gon', N'Ha Noi', 1700),
(N'TD002', N'Tuyen Da Nang - Hue', N'Da Nang', N'Hue', 100),
(N'TD003', N'Tuyen Can Tho - Sai Gon', N'Can Tho', N'Sai Gon', 200);

-- Bảng Xes
SET IDENTITY_INSERT Xes ON;
INSERT INTO Xes (XeID, BienSo, TinhTrang, LoaiXeID) VALUES
(123, N'51A-12345', N'Hoat Dong', N'LX001'),
(234, N'51B-54321', N'Bao Tri', N'LX002'),
(345, N'51C-98765', N'San Sang', N'LX003'),
(456, N'52A-45678', N'Hoạt Động', N'LX001'),
(567, N'52B-56789', N'Bảo Trì', N'LX002'),
(678, N'52C-67890', N'Sẵn Sàng', N'LX003'),
(789, N'53A-78901', N'Hoạt Động', N'LX001'),
(890, N'53B-89012', N'Bảo Trì', N'LX002'),
(901, N'53C-90123', N'Sẵn Sàng', N'LX003'),
(912, N'54A-91234', N'Hoạt Động', N'LX001'),
(923, N'54B-92345', N'Bảo Trì', N'LX002'),
(934, N'54C-93456', N'Sẵn Sàng', N'LX003'),
(945, N'55A-94567', N'Hoạt Động', N'LX001');
SET IDENTITY_INSERT Xes OFF;


-- Bảng NhanViens
INSERT INTO NhanViens (NhanVienID, HoTen, NgaySinh, GioiTinh, DiaChi, SDT, Email, CCCD, NgayVaoLam, ChucVu) VALUES
(N'NV001', N'Nguyen Van A', '1990-01-01', N'M', N'Ha Noi', '0123456789', 'a@example.com', '123456789', '2023-01-01', N'Nhân viên'),
(N'NV002', N'Le Thi B', '1992-02-02', N'F', N'Hai Phong', '0987654321', 'b@example.com', '987654321', '2022-01-01', N'Quản lý'),
(N'NV003', N'Tran Van C', '1985-03-03', N'M', N'Da Nang', '0123456789', 'c@example.com', '456789123', '2020-01-01', N'Trưởng phòng');

-- Bảng KhachHangs
INSERT INTO KhachHangs (KhachHangID, HoTen, DiaChi, SDT, Email, CCCD, HangThanhVien, AppUserId) VALUES
(1, N'Nguyen Van A', N'Ha Noi', '0123456789', 'a@example.com', '123456789', 1, '064ba27f-6514-4096-8626-61057add323a'),
(2, N'Le Thi B', N'Hai Phong', '0987654321', 'b@example.com', '987654321', 2, '80e25164-001c-4b28-b071-451057d48ade'),
(3, N'Tran Van C', N'Da Nang', '0123456789', 'c@example.com', '456789123', 1, 'f4843e5c-f443-4866-b5d6-dc69c4d4d988');

-- Bảng LichTrinhs
INSERT INTO LichTrinhs (LichTrinhID, XeID, TuyenDuongID, GioKhoiHanh, GioDen, DieuChinhGiaVe, GiaVe, NgayDen, NgayKhoiHanh) VALUES 
(1, 123, 'TD001', '08:00:00', '10:00:00', 0, 100000, '2024-10-08', '2024-10-08'),
(2, 234, 'TD002', '09:00:00', '11:00:00', 0, 120000, '2024-10-09', '2024-10-09'),
(3, 345, 'TD003', '10:00:00', '12:00:00', 0, 150000, '2024-10-10', '2024-10-10'),
(4, 456, 'TD001', '06:00:00', '08:00:00', 0, 150000, '2024-11-01', '2024-11-01'),
(5, 567, 'TD002', '07:00:00', '09:00:00', 0, 170000, '2024-11-02', '2024-11-02'),
(6, 678, 'TD003', '08:00:00', '10:00:00', 0, 180000, '2024-11-03', '2024-11-03'),
(7, 789, 'TD001', '09:00:00', '11:00:00', 0, 200000, '2024-11-04', '2024-11-04'),
(8, 890, 'TD002', '10:00:00', '12:00:00', 0, 220000, '2024-11-05', '2024-11-05'),
(9, 901, 'TD003', '11:00:00', '13:00:00', 0, 240000, '2024-11-06', '2024-11-06'),
(10, 912, 'TD001', '12:00:00', '14:00:00', 0, 250000, '2024-11-07', '2024-11-07'),
(11, 923, 'TD002', '13:00:00', '15:00:00', 0, 260000, '2024-11-08', '2024-11-08'),
(12, 934, 'TD003', '14:00:00', '16:00:00', 0, 280000, '2024-11-09', '2024-11-09'),
(13, 945, 'TD001', '15:00:00', '17:00:00', 0, 300000, '2024-11-10', '2024-11-10');

-- Bảng Ghes
INSERT INTO Ghes (GheID, XeID, TenGhe, TrangThai) VALUES
(1, 123, N'A01', N'Trống'), 
(2, 234, N'A02', N'Trống'), 
(3, 345, N'A03', N'Trống'),
(4, 456, N'A01', N'Trống'),
(5, 456, N'A02', N'Trống'),
(6, 567, N'B01', N'Trống'),
(7, 567, N'B02', N'Trống'),
(8, 678, N'C01', N'Trống'),
(9, 678, N'C02', N'Trống'),
(10, 789, N'D01', N'Trống'),
(11, 789, N'D02', N'Trống'),
(12, 890, N'E01', N'Trống'),
(13, 890, N'E02', N'Trống'),
(14, 901, N'F01', N'Trống'),
(15, 901, N'F02', N'Trống'),
(16, 912, N'G01', N'Trống'),
(17, 912, N'G02', N'Trống'),
(18, 923, N'H01', N'Trống'),
(19, 923, N'H02', N'Trống'),
(20, 934, N'I01', N'Trống'),
(21, 934, N'I02', N'Trống'),
(22, 945, N'J01', N'Trống'),
(23, 945, N'J02', N'Trống');

-- Bảng KhuyenMais
INSERT INTO KhuyenMais (KhuyenMaiID, TenKhuyenMai, LoaiKhuyenMai, NgayBatDau, NgayKetThuc, GiaTriGiam, DieuKienApDung, TrangThaiThanhToan) 
VALUES
(N'KM004', N'Giảm 10% cho vé cuối tuần', N'Theo phần trăm', '2024-12-15', '2024-12-31', 10.00, 50000, N'Đang áp dụng'),
(N'KM005', N'Giảm 100.000 VNĐ cho chuyến xa', N'Theo số tiền', '2024-12-20', '2024-12-31', 100000.00, 1000000, N'Đang áp dụng'),
(N'KM006', N'Mua 1 vé tặng 1 vé', N'Khuyến mãi đặc biệt', '2024-12-25', '2025-01-05', 0.00, 0, N'Sắp áp dụng');

-- Bảng KhuyenMai_KHcs
INSERT INTO KhuyenMai_KHcs (KhuyenMaiID, KhachHangID) 
VALUES
(N'KM004', N'1'),
(N'KM005', N'2'),
(N'KM006', N'3');


-- Bảng VeXes
INSERT INTO VeXes (VeID, KhachHangID, LichTrinhID, TongGiaVe, KhuyenMaiID)
VALUES
(N'VX001', N'1', N'1', 200000, N'KM004'),
(N'VX002', N'2', N'2', 250000, N'KM005'),
(N'VX003', N'3', N'3', 300000, N'KM006'),
(N'VX004', N'1', N'4', 220000, N'KM004'),
(N'VX005', N'2', N'5', 230000, N'KM005'),
(N'VX006', N'3', N'6', 240000, N'KM006'),
(N'VX007', N'1', N'7', 250000, N'KM004'),
(N'VX008', N'2', N'8', 260000, N'KM005'),
(N'VX009', N'3', N'9', 270000, N'KM006'),
(N'VX010', N'1', N'10', 280000, N'KM004');



-- Bảng LichSuGiaoDichs
INSERT INTO LichSuGiaoDichs (GiaoDichID, VeID, LoaiGiaoDich, ChiTiet, NgayGiaoDich, NhanVienID, TrangThaiGiaoDich)
VALUES
(1, 'VX001', N'Thanh Toán', N'Thanh toán vé xe', '2024-10-01', 'NV001', N'Thành công'),
(2, 'VX002', N'Hủy Vé', N'Hủy vé đã đặt', '2024-10-02', 'NV002', N'Thành công'),
(3, 'VX003', N'Thanh Toán', N'Thanh toán vé xe', '2024-10-03', 'NV003', N'Thành công');


-- Bảng HoaDons
SET IDENTITY_INSERT HoaDons ON;
INSERT INTO HoaDons (HoaDonID, VeID, NhanVienID, NgayLap, TongTien, PhuongThucThanhToan, TrangThaiThanhToan) VALUES
(1, 'VX001', N'NV001', '2024-10-01', 500000, N'Tiền mặt', N'Đã thanh toán'),
(2, 'VX002', N'NV002', '2024-10-02', 600000, N'Chuyển khoản', N'Chưa thanh toán'),
(3, 'VX003', N'NV003', '2024-10-03', 700000, N'Tiền mặt', N'Đã thanh toán');
SET IDENTITY_INSERT HoaDons OFF;


--Bảng ChiTietVeDats
INSERT INTO ChiTietVeDats (ChiTietVeID, VeID, XeID, GiaGhe, TinhTrangGhe, SoGhe, NgayDat)
VALUES
(N'CTD001', N'VX001', 123, 200000, N'Trống', 1, GETDATE()),
(N'CTD002', N'VX002', 234, 250000, N'Đã đặt', 2, GETDATE()),
(N'CTD003', N'VX003', 345, 300000, N'Trống', 3, GETDATE()),
(N'CTD004', N'VX001', 123, 200000, N'Đã đặt', 1, GETDATE()),
(N'CTD005', N'VX002', 234, 250000, N'Đã đặt', 2, GETDATE()),
(N'CTD006', N'VX003', 345, 300000, N'Đã đặt', 3, GETDATE()),
(N'CTD007', N'VX004', 456, 180000, N'Trống', 4, GETDATE()),
(N'CTD008', N'VX004', 456, 180000, N'Trống', 5, GETDATE()),
(N'CTD009', N'VX005', 567, 190000, N'Trống', 6, GETDATE()),
(N'CTD010', N'VX005', 567, 190000, N'Trống', 7, GETDATE()),
(N'CTD011', N'VX006', 678, 220000, N'Đã đặt', 8, GETDATE()),
(N'CTD012', N'VX006', 678, 220000, N'Trống', 9, GETDATE()),
(N'CTD013', N'VX007', 789, 250000, N'Đã đặt', 10, GETDATE());


-- Bảng NhanXetKhachHangs
SET IDENTITY_INSERT NhanXetKhachHangs ON;
INSERT INTO NhanXetKhachHangs (NhanXetID, KhachHangID, LichTrinhID, DanhGia, NhanXet) VALUES
(1, 1, 1, 5, N'Dịch vụ rất tốt, tôi rất hài lòng!'),
(2, 2, 2, 4, N'Thời gian chạy xe rất đúng giờ, nhân viên thân thiện.'),
(3, 3, 3, 3, N'Tôi cảm thấy cần cải thiện chất lượng xe.');
SET IDENTITY_INSERT NhanXetKhachHangs OFF;

-- Bảng LichSuDoiGhes
INSERT INTO LichSuDoiGhes (ChiTietVeID, GheCu, GheMoi, NgayDoi, LyDoDoi, ChenhLechGia, NhanVienID, GiaoDichID) 
VALUES
(N'CTD001', 1, 2, GETDATE(), N'Khách đổi ghế', 0, N'NV001', 1),
(N'CTD002', 3, 4, GETDATE(), N'Khách đổi ghế', 0, N'NV002', 2),
(N'CTD003', 5, 6, GETDATE(), N'Khách đổi ghế', 0, N'NV003', 3);

-- Bảng TinTucs
INSERT INTO TinTucs (TinTucId, TieuDe, NoiDung, ThoiGian, Url, AnhBia, NhanVienID) 
VALUES 
(N'TT001', N'Ưu đãi mùa lễ hội', N'Chương trình ưu đãi giảm giá vé trong mùa lễ hội', SYSDATETIME(), NULL, NULL, N'NV001'),
(N'TT002', N'Chuyến xe mới', N'Chúng tôi vừa thêm tuyến đường mới từ Hà Nội đến Hải Phòng.', SYSDATETIME(), NULL, NULL, N'NV002'),
(N'TT003', N'Giới thiệu ứng dụng đặt vé', N'Tải ứng dụng đặt vé của chúng tôi để nhận nhiều ưu đãi hấp dẫn.', SYSDATETIME(), NULL, NULL, N'NV003');

-- Bảng FileTinTuc
INSERT INTO [dbo].[FileTinTuc] (FileID, TinTucId, TenFile, Loai, FilePath) VALUES 
(N'FT001', N'TT001', N'UuDai.png', N'Image', N'/uploads/UUDAI.png'),
(N'FT002', N'TT002', N'TuyenMoi.pdf', N'Document', N'/uploads/TuyenMoi.pdf'),
(N'FT003', N'TT003', N'AppGioiThieu.jpg', N'Image', N'/uploads/AppGioiThieu.jpg');

DELETE FROM FileTinTuc;
DELETE FROM ChiTietVeDats;
DELETE FROM LichSuDoiGhes;
DELETE FROM NhanXetKhachHangs;
DELETE FROM LichSuGiaoDichs;
DELETE FROM HoaDons;
DELETE FROM KhuyenMai_KHcs;
DELETE FROM VeXes;
DELETE FROM KhuyenMais;
DELETE FROM TinTucs;
DELETE FROM Ghes;
DELETE FROM LichTrinhs;
DELETE FROM TaiXes;
DELETE FROM NhanViens;
DELETE FROM KhachHangs;
DELETE FROM Xes;
DELETE FROM LoaiXes;
DELETE FROM TuyenDuongs;


SELECT * FROM FileTinTuc;
SELECT * FROM ChiTietVeDats;
SELECT * FROM LichSuDoiGhes;
SELECT * FROM NhanXetKhachHangs;
SELECT * FROM LichSuGiaoDichs;
SELECT * FROM HoaDons;
SELECT * FROM KhuyenMai_KHcs;
SELECT * FROM VeXes;
SELECT * FROM KhuyenMais;
SELECT * FROM TinTucs;
SELECT * FROM Ghes;
SELECT * FROM LichTrinhs;
SELECT * FROM TaiXes;
SELECT * FROM NhanViens;
SELECT * FROM KhachHangs;
SELECT * FROM Xes;
SELECT * FROM LoaiXes;
SELECT * FROM TuyenDuongs;
