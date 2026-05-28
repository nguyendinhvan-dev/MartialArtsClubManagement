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
                    .OrderByDescending(d => d.NgayHoc)
                    .Select(d => new
                    {
                        MaDiemDanh = d.MaDiemDanh,
                        NgayHoc = d.NgayHoc,
                        TrangThai = d.TrangThai,
                        LopHoc = "Lớp " + d.MaDangKyNavigation.MaLopNavigation.MaLop
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
                        k.DiemSo,
                        k.DaDat,
                        CapDaiMoi = k.MaCapDaiMoiNavigation != null ? k.MaCapDaiMoiNavigation.TenCapDai : null
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
                        .ThenInclude(l => l.MaKhoaHocNavigation)
                    .Where(d => d.MaHocVien == hocVien.MaHocVien)
                    .Select(d => new
                    {
                        Loai = "Lớp học",
                        Ten = d.MaLopNavigation.MaKhoaHocNavigation.TenKhoaHoc,
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
                        NgayDangKy = d.NgayBatDau ?? DateOnly.FromDateTime(DateTime.Now)
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
                        .ThenInclude(l => l.MaKhoaHocNavigation)
                    .Where(d => d.MaHocVien == hocVien.MaHocVien)
                    .Select(d => new
                    {
                        Loai = "Lớp học",
                        NoiDung = d.MaLopNavigation.MaKhoaHocNavigation.TenKhoaHoc,
                        SoTien = d.MaLopNavigation.MaKhoaHocNavigation.HocPhi,
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

        // ==========================================
        // CÁC API ĐĂNG KÝ VÀ THANH TOÁN (MODULE HỌC VIÊN)
        // ==========================================

        // GET: api/HocVienPortal/classes-available
        [HttpGet("classes-available")]
        public async Task<IActionResult> GetAvailableClasses()
        {
            try
            {
                var hocVien = await GetCurrentHocVienAsync();
                if (hocVien == null) return NotFound(new { Success = false, Message = "Không tìm thấy học viên" });

                // Lấy các lớp học chưa đăng ký và thuộc khóa học đang mở, đồng thời cấp đai của lớp nhỏ hơn hoặc bằng cấp đai học viên (hoặc không bắt buộc)
                var lopHocs = await _context.LopHocs
                    .Include(l => l.MaKhoaHocNavigation)
                    .Include(l => l.MaHlvNavigation)
                    .Include(l => l.MaCapDaiNavigation)
                    .Where(l => l.MaKhoaHocNavigation.TrangThai == "DangMo")
                    .Select(l => new
                    {
                        l.MaLop,
                        l.TenLop,
                        l.LichHoc,
                        l.PhongTap,
                        l.HocPhi,
                        l.SoLuongToiDa,
                        SoLuongDaDangKy = l.DangKyLops.Count(),
                        KhoaHoc = l.MaKhoaHocNavigation.TenKhoaHoc,
                        HuanLuyenVien = l.MaHlvNavigation.MaTaiKhoanNavigation.HoTen,
                        YeuCauCapDai = l.MaCapDaiNavigation.TenCapDai
                    })
                    .ToListAsync();

                return Ok(new { Success = true, Data = lopHocs });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // POST: api/HocVienPortal/enroll
        [HttpPost("enroll")]
        public async Task<IActionResult> EnrollClass([FromBody] DangKyLopRequestDTO dto)
        {
            try
            {
                var hocVien = await GetCurrentHocVienAsync();
                if (hocVien == null) return NotFound(new { Success = false, Message = "Không tìm thấy học viên" });

                // Kiểm tra xem đã đăng ký chưa
                var daDangKy = await _context.DangKyLops.AnyAsync(d => d.MaHocVien == hocVien.MaHocVien && d.MaLop == dto.MaLop);
                if (daDangKy) return BadRequest(new { Success = false, Message = "Bạn đã đăng ký lớp này rồi" });

                var dangKyMoi = new DangKyLop
                {
                    MaHocVien = hocVien.MaHocVien,
                    MaLop = dto.MaLop,
                    TrangThaiThanhToan = "ChuaThanhToan"
                };

                _context.DangKyLops.Add(dangKyMoi);
                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Đăng ký lớp thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // GET: api/HocVienPortal/private-packages-available
        [HttpGet("private-packages-available")]
        public async Task<IActionResult> GetPrivatePackages()
        {
            try
            {
                var gois = await _context.GoiKemRiengs
                    .Where(g => g.DangSuDung == true)
                    .Select(g => new
                    {
                        g.MaGoi,
                        g.TenGoi,
                        g.SoBuoi,
                        g.HocPhi
                    })
                    .ToListAsync();

                var hlvs = await _context.HuanLuyenViens
                    .Include(h => h.MaTaiKhoanNavigation)
                    .Where(h => h.DangHoatDong == true)
                    .Select(h => new
                    {
                        h.MaHlv,
                        HoTen = h.MaTaiKhoanNavigation.HoTen,
                        h.ChuyenMon
                    })
                    .ToListAsync();

                return Ok(new { Success = true, Packages = gois, Trainers = hlvs });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // POST: api/HocVienPortal/book-private
        [HttpPost("book-private")]
        public async Task<IActionResult> BookPrivateTraining([FromBody] DangKyKemRequestDTO dto)
        {
            try
            {
                var hocVien = await GetCurrentHocVienAsync();
                if (hocVien == null) return NotFound(new { Success = false, Message = "Không tìm thấy học viên" });

                var dangKyMoi = new DangKyKem
                {
                    MaHocVien = hocVien.MaHocVien,
                    MaGoi = dto.MaGoi,
                    MaHlv = dto.MaHlv,
                    TrangThaiPhanCong = dto.MaHlv.HasValue ? "DaPhanCong" : "ChoPhanCong",
                    TrangThaiThanhToan = "ChuaThanhToan"
                };

                _context.DangKyKems.Add(dangKyMoi);
                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Đăng ký gói kèm riêng thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // GET: api/HocVienPortal/events-available
        [HttpGet("events-available")]
        public async Task<IActionResult> GetAvailableEvents()
        {
            try
            {
                // Sự kiện được lưu trong bảng Thông Báo với loại là Sự Kiện
                var suKiens = await _context.ThongBaos
                    .Where(t => t.LoaiThongBao == "SuKien" && t.NgayDang >= DateTime.Now.AddMonths(-1))
                    .Select(t => new
                    {
                        t.MaThongBao,
                        t.TieuDe,
                        t.NoiDung,
                        t.NgayDang
                    })
                    .ToListAsync();

                return Ok(new { Success = true, Data = suKiens });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // POST: api/HocVienPortal/register-event
        [HttpPost("register-event")]
        public async Task<IActionResult> RegisterEvent([FromBody] DangKySuKienRequestDTO dto)
        {
            try
            {
                var hocVien = await GetCurrentHocVienAsync();
                if (hocVien == null) return NotFound(new { Success = false, Message = "Không tìm thấy học viên" });

                var daDangKy = await _context.DangKySuKiens.AnyAsync(d => d.MaHocVien == hocVien.MaHocVien && d.MaThongBao == dto.MaThongBao);
                if (daDangKy) return BadRequest(new { Success = false, Message = "Bạn đã đăng ký sự kiện này rồi" });

                var dangKyMoi = new DangKySuKien
                {
                    MaHocVien = hocVien.MaHocVien,
                    MaThongBao = dto.MaThongBao
                };

                _context.DangKySuKiens.Add(dangKyMoi);
                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Đăng ký sự kiện thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        // POST: api/HocVienPortal/pay
        [HttpPost("pay")]
        public async Task<IActionResult> PayTuition([FromBody] ThanhToanRequestDTO dto)
        {
            try
            {
                var hocVien = await GetCurrentHocVienAsync();
                if (hocVien == null) return NotFound(new { Success = false, Message = "Không tìm thấy học viên" });

                if (dto.LoaiThanhToan == "LopHoc")
                {
                    var dk = await _context.DangKyLops.FirstOrDefaultAsync(d => d.MaDangKy == dto.MaDangKy && d.MaHocVien == hocVien.MaHocVien);
                    if (dk == null) return NotFound(new { Success = false, Message = "Không tìm thấy thông tin đăng ký" });
                    
                    dk.TrangThaiThanhToan = "DaThanhToan";
                    dk.NgayThanhToan = DateTime.Now;
                    _context.DangKyLops.Update(dk);
                }
                else if (dto.LoaiThanhToan == "KemRieng")
                {
                    var dk = await _context.DangKyKems.FirstOrDefaultAsync(d => d.MaDangKyKem == dto.MaDangKy && d.MaHocVien == hocVien.MaHocVien);
                    if (dk == null) return NotFound(new { Success = false, Message = "Không tìm thấy thông tin đăng ký" });

                    dk.TrangThaiThanhToan = "DaThanhToan";
                    dk.NgayThanhToan = DateTime.Now;
                    _context.DangKyKems.Update(dk);
                }
                else
                {
                    return BadRequest(new { Success = false, Message = "Loại thanh toán không hợp lệ" });
                }

                await _context.SaveChangesAsync();
                return Ok(new { Success = true, Message = $"Thanh toán thành công bằng {dto.PhuongThucThanhToan ?? "tiền mặt"}" });
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
