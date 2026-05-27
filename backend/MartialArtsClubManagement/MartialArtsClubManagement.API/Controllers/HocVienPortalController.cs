using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MartialArtsClubManagement.API.Models.Entities;
using MartialArtsClubManagement.API.Models.DTOs;
using System.Security.Claims;
using BCrypt.Net;

namespace MartialArtsClubManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "HocVien")] // Chỉ Học viên mới được gọi API này
    public class HocVienPortalController : ControllerBase
    {
        private readonly QuanLyCLBVoThuatContext _context;

        public HocVienPortalController(QuanLyCLBVoThuatContext context)
        {
            _context = context;
        }

        private int GetCurrentTaiKhoanId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("Không thể xác định người dùng");
        }

        private async Task<HocVien?> GetCurrentHocVienAsync()
        {
            int maTaiKhoan = GetCurrentTaiKhoanId();
            return await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);
        }

        // GET: api/HocVienPortal/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                int maTaiKhoan = GetCurrentTaiKhoanId();
                var thongTin = await _context.HocViens
                    .Include(h => h.MaTaiKhoanNavigation)
                    .Include(h => h.MaCapDaiHienTaiNavigation)
                    .Where(h => h.MaTaiKhoan == maTaiKhoan)
                    .Select(h => new
                    {
                        h.MaHocVien,
                        TenHocVien = h.MaTaiKhoanNavigation.HoTen,
                        Email = h.MaTaiKhoanNavigation.Email,
                        h.SoDienThoai,
                        h.DiaChi,
                        h.NgaySinh,
                        h.GioiTinh,
                        h.NgayGiaNhap,
                        CapDaiHienTai = h.MaCapDaiHienTaiNavigation != null ? h.MaCapDaiHienTaiNavigation.TenCapDai : "Chưa có"
                    })
                    .FirstOrDefaultAsync();

                if (thongTin == null) return NotFound(new { Success = false, Message = "Không tìm thấy thông tin học viên" });

                return Ok(new { Success = true, Data = thongTin });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // PUT: api/HocVienPortal/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO dto)
        {
            try
            {
                var hocVien = await GetCurrentHocVienAsync();
                if (hocVien == null) return NotFound(new { Success = false, Message = "Không tìm thấy học viên" });

                hocVien.SoDienThoai = dto.SoDienThoai;
                hocVien.DiaChi = dto.DiaChi;

                _context.HocViens.Update(hocVien);
                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Cập nhật thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // GET: api/HocVienPortal/attendance
        [HttpGet("attendance")]
        public async Task<IActionResult> GetAttendance()
        {
            try
            {
                var hocVien = await GetCurrentHocVienAsync();
                if (hocVien == null) return NotFound(new { Success = false, Message = "Không tìm thấy học viên" });

                var diemDanhs = await _context.DiemDanhs
                    .Include(d => d.MaDangKyNavigation)
                        .ThenInclude(dk => dk.MaLopNavigation)
                    .Where(d => d.MaDangKyNavigation.MaHocVien == hocVien.MaHocVien)
                    .OrderByDescending(d => d.NgayDiemDanh)
                    .Select(d => new
                    {
                        d.MaDiemDanh,
                        d.NgayDiemDanh,
                        d.TrangThai,
                        LopHoc = d.MaDangKyNavigation.MaLopNavigation.TenLop
                    })
                    .ToListAsync();

                return Ok(new { Success = true, Data = diemDanhs });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // GET: api/HocVienPortal/exams
        [HttpGet("exams")]
        public async Task<IActionResult> GetExams()
        {
            try
            {
                var hocVien = await GetCurrentHocVienAsync();
                if (hocVien == null) return NotFound(new { Success = false, Message = "Không tìm thấy học viên" });

                var ketQua = await _context.KetQuaThis
                    .Include(k => k.MaKyThiNavigation)
                    .Where(k => k.MaHocVien == hocVien.MaHocVien)
                    .OrderByDescending(k => k.MaKyThiNavigation.NgayThi)
                    .Select(k => new
                    {
                        k.MaKetQua,
                        k.MaKyThiNavigation.TenKyThi,
                        k.MaKyThiNavigation.NgayThi,
                        k.DiemLyThuyet,
                        k.DiemThucHanh,
                        k.TrangThai,
                        k.GhiChu
                    })
                    .ToListAsync();

                return Ok(new { Success = true, Data = ketQua });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // GET: api/HocVienPortal/schedule
        [HttpGet("schedule")]
        public async Task<IActionResult> GetSchedule()
        {
            try
            {
                var hocVien = await GetCurrentHocVienAsync();
                if (hocVien == null) return NotFound(new { Success = false, Message = "Không tìm thấy học viên" });

                var lopHocs = await _context.DangKyLops
                    .Include(d => d.MaLopNavigation)
                    .Where(d => d.MaHocVien == hocVien.MaHocVien)
                    .Select(d => new
                    {
                        Loai = "Lớp học",
                        Ten = d.MaLopNavigation.TenLop,
                        Lich = d.MaLopNavigation.LichHoc,
                        NgayDangKy = d.NgayDangKy
                    })
                    .ToListAsync();

                var kemRiengs = await _context.DangKyKems
                    .Include(d => d.MaGoiNavigation)
                    .Where(d => d.MaHocVien == hocVien.MaHocVien)
                    .Select(d => new
                    {
                        Loai = "Kèm riêng",
                        Ten = d.MaGoiNavigation.TenGoi,
                        Lich = "Thỏa thuận với HLV",
                        NgayDangKy = d.NgayBatDau
                    })
                    .ToListAsync();

                return Ok(new { Success = true, Data = lopHocs.Concat(kemRiengs) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // GET: api/HocVienPortal/tuition
        [HttpGet("tuition")]
        public async Task<IActionResult> GetTuition()
        {
            try
            {
                var hocVien = await GetCurrentHocVienAsync();
                if (hocVien == null) return NotFound(new { Success = false, Message = "Không tìm thấy học viên" });

                var lopHocs = await _context.DangKyLops
                    .Include(d => d.MaLopNavigation)
                    .Where(d => d.MaHocVien == hocVien.MaHocVien)
                    .Select(d => new
                    {
                        Loai = "Lớp học",
                        NoiDung = d.MaLopNavigation.TenLop,
                        SoTien = d.MaLopNavigation.HocPhi,
                        TrangThaiThanhToan = d.TrangThaiThanhToan,
                        NgayThanhToan = d.NgayThanhToan
                    })
                    .ToListAsync();

                var kemRiengs = await _context.DangKyKems
                    .Include(d => d.MaGoiNavigation)
                    .Where(d => d.MaHocVien == hocVien.MaHocVien)
                    .Select(d => new
                    {
                        Loai = "Kèm riêng",
                        NoiDung = d.MaGoiNavigation.TenGoi,
                        SoTien = d.MaGoiNavigation.HocPhi,
                        TrangThaiThanhToan = d.TrangThaiThanhToan,
                        NgayThanhToan = d.NgayThanhToan
                    })
                    .ToListAsync();

                return Ok(new { Success = true, Data = lopHocs.Concat(kemRiengs) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // GET: api/HocVienPortal/notifications
        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            try
            {
                var thongBaos = await _context.ThongBaos
                    .Where(t => t.DoiTuongNhan == "HocVien" || t.DoiTuongNhan == "TatCa")
                    .OrderByDescending(t => t.NgayDang)
                    .Select(t => new
                    {
                        t.MaThongBao,
                        t.TieuDe,
                        t.NoiDung,
                        t.NgayDang
                    })
                    .ToListAsync();

                return Ok(new { Success = true, Data = thongBaos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // PUT: api/HocVienPortal/change-password
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            try
            {
                int maTaiKhoan = GetCurrentTaiKhoanId();
                var taiKhoan = await _context.TaiKhoans.FindAsync(maTaiKhoan);
                
                if (taiKhoan == null) return NotFound(new { Success = false, Message = "Tài khoản không tồn tại" });

                if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, taiKhoan.MatKhauHash))
                {
                    return BadRequest(new { Success = false, Message = "Mật khẩu hiện tại không đúng" });
                }

                taiKhoan.MatKhauHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                _context.TaiKhoans.Update(taiKhoan);
                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Đổi mật khẩu thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
    }

    public class UpdateProfileDTO
    {
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
    }

    public class ChangePasswordDTO
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
