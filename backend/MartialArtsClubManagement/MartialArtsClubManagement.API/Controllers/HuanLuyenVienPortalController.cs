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
    [Authorize(Roles = "HuanLuyenVien")]
    public class HuanLuyenVienPortalController : ControllerBase
    {
        private readonly QuanLyCLBVoThuatContext _context;

        public HuanLuyenVienPortalController(QuanLyCLBVoThuatContext context)
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

        private async Task<HuanLuyenVien?> GetCurrentHuanLuyenVienAsync()
        {
            int maTaiKhoan = GetCurrentTaiKhoanId();
            return await _context.HuanLuyenViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);
        }

        // GET: api/HuanLuyenVienPortal/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                int maTaiKhoan = GetCurrentTaiKhoanId();
                var thongTin = await _context.HuanLuyenViens
                    .Include(h => h.MaTaiKhoanNavigation)
                    .Where(h => h.MaTaiKhoan == maTaiKhoan)
                    .Select(h => new
                    {
                        h.MaHlv,
                        TenHLV = h.MaTaiKhoanNavigation.HoTen,
                        Email = h.MaTaiKhoanNavigation.Email,
                        h.SoDienThoai,
                        h.ChuyenMon,
                        h.NgayVaoClb,
                        h.DangHoatDong
                    })
                    .FirstOrDefaultAsync();

                if (thongTin == null) return NotFound(new { Success = false, Message = "Không tìm thấy thông tin huấn luyện viên" });

                return Ok(new { Success = true, Data = thongTin });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // PUT: api/HuanLuyenVienPortal/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateHLVProfileDTO dto)
        {
            try
            {
                var hlv = await GetCurrentHuanLuyenVienAsync();
                if (hlv == null) return NotFound(new { Success = false, Message = "Không tìm thấy huấn luyện viên" });

                hlv.SoDienThoai = dto.SoDienThoai;
                hlv.ChuyenMon = dto.ChuyenMon;

                _context.HuanLuyenViens.Update(hlv);
                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Cập nhật thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // GET: api/HuanLuyenVienPortal/schedule
        [HttpGet("schedule")]
        public async Task<IActionResult> GetSchedule()
        {
            try
            {
                var hlv = await GetCurrentHuanLuyenVienAsync();
                if (hlv == null) return NotFound(new { Success = false, Message = "Không tìm thấy huấn luyện viên" });

                var lopHocs = await _context.LopHocs
                    .Include(l => l.MaKhoaHocNavigation)
                    .Include(l => l.MaCapDaiNavigation)
                    .Where(l => l.MaHlv == hlv.MaHlv)
                    .Select(l => new
                    {
                        l.MaLop,
                        TenKhoaHoc = l.MaKhoaHocNavigation.TenKhoaHoc,
                        TenCapDai = l.MaCapDaiNavigation.TenCapDai,
                        l.LichHoc,
                        l.SoLuongToiDa,
                        l.PhongTap
                    })
                    .ToListAsync();

                var kemRiengs = await _context.DangKyKems
                    .Include(d => d.MaGoiNavigation)
                    .Include(d => d.MaHocVienNavigation)
                    .Where(d => d.MaHlv == hlv.MaHlv && d.TrangThaiPhanCong == "DaPhanCong")
                    .Select(d => new
                    {
                        d.MaDangKyKem,
                        TenGoi = d.MaGoiNavigation.TenGoi,
                        TenHocVien = d.MaHocVienNavigation.MaTaiKhoanNavigation.HoTen,
                        d.NgayBatDau,
                        d.NgayDangKy
                    })
                    .ToListAsync();

                return Ok(new { Success = true, Data = new { LopHocs = lopHocs, KemRiengs = kemRiengs } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // GET: api/HuanLuyenVienPortal/students/{maLop}
        [HttpGet("students/{maLop}")]
        public async Task<IActionResult> GetStudentsInClass(int maLop)
        {
            try
            {
                var hlv = await GetCurrentHuanLuyenVienAsync();
                if (hlv == null) return NotFound(new { Success = false, Message = "Không tìm thấy huấn luyện viên" });

                // Verify the class belongs to this HLV
                var lopHoc = await _context.LopHocs.FirstOrDefaultAsync(l => l.MaLop == maLop && l.MaHlv == hlv.MaHlv);
                if (lopHoc == null) return BadRequest(new { Success = false, Message = "Lớp học không thuộc quyền quản lý" });

                var students = await _context.DangKyLops
                    .Include(d => d.MaHocVienNavigation)
                        .ThenInclude(h => h.MaTaiKhoanNavigation)
                    .Include(d => d.MaHocVienNavigation)
                        .ThenInclude(h => h.MaCapDaiHienTaiNavigation)
                    .Where(d => d.MaLop == maLop)
                    .Select(d => new
                    {
                        d.MaDangKy,
                        TenHocVien = d.MaHocVienNavigation.MaTaiKhoanNavigation.HoTen,
                        SoDienThoai = d.MaHocVienNavigation.SoDienThoai,
                        CapDai = d.MaHocVienNavigation.MaCapDaiHienTaiNavigation != null 
                            ? d.MaHocVienNavigation.MaCapDaiHienTaiNavigation.TenCapDai 
                            : "Chưa có",
                        d.TrangThaiThanhToan,
                        d.NgayDangKy
                    })
                    .ToListAsync();

                return Ok(new { Success = true, Data = students });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // POST: api/HuanLuyenVienPortal/attendance
        [HttpPost("attendance")]
        public async Task<IActionResult> TakeAttendance([FromBody] CreateAttendanceDTO dto)
        {
            try
            {
                var hlv = await GetCurrentHuanLuyenVienAsync();
                if (hlv == null) return NotFound(new { Success = false, Message = "Không tìm thấy huấn luyện viên" });

                // Verify the class belongs to this HLV
                var lopHoc = await _context.LopHocs.FirstOrDefaultAsync(l => l.MaLop == dto.MaLop && l.MaHlv == hlv.MaHlv);
                if (lopHoc == null) return BadRequest(new { Success = false, Message = "Lớp học không thuộc quyền quản lý" });

                var diemDanh = new DiemDanh
                {
                    MaDangKy = dto.MaDangKy,
                    NgayHoc = dto.NgayHoc,
                    TrangThai = dto.TrangThai,
                    GhiChu = dto.GhiChu
                };

                _context.DiemDanhs.Add(diemDanh);
                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Điểm danh thành công", Data = diemDanh });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // GET: api/HuanLuyenVienPortal/exams
        [HttpGet("exams")]
        public async Task<IActionResult> GetExams()
        {
            try
            {
                var hlv = await GetCurrentHuanLuyenVienAsync();
                if (hlv == null) return NotFound(new { Success = false, Message = "Không tìm thấy huấn luyện viên" });

                // Get exams for classes taught by this HLV
                var kyThis = await _context.LopHocs
                    .Where(l => l.MaHlv == hlv.MaHlv)
                    .SelectMany(l => _context.KyThiThangDais
                        .Where(k => k.MaKhoaHoc == l.MaKhoaHoc))
                    .Distinct()
                    .OrderByDescending(k => k.NgayThi)
                    .Select(k => new
                    {
                        k.MaKyThi,
                        k.TenKyThi,
                        k.NgayThi,
                        k.MoTa,
                        k.TrangThai
                    })
                    .ToListAsync();

                return Ok(new { Success = true, Data = kyThis });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // GET: api/HuanLuyenVienPortal/notifications
        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            try
            {
                var thongBaos = await _context.ThongBaos
                    .OrderByDescending(t => t.NgayDang)
                    .Select(t => new
                    {
                        t.MaThongBao,
                        t.TieuDe,
                        t.NoiDung,
                        t.NgayDang,
                        t.LoaiThongBao
                    })
                    .ToListAsync();

                return Ok(new { Success = true, Data = thongBaos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // PUT: api/HuanLuyenVienPortal/change-password
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

    public class UpdateHLVProfileDTO
    {
        public string? SoDienThoai { get; set; }
        public string? ChuyenMon { get; set; }
    }

    public class CreateAttendanceDTO
    {
        public int MaLop { get; set; }
        public int MaDangKy { get; set; }
        public DateOnly NgayHoc { get; set; }
        public string TrangThai { get; set; } = "CóMặt";
        public string? GhiChu { get; set; }
    }
}
