using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MartialArtsClubManagement.API.Models.DTOs;
using MartialArtsClubManagement.API.Models.Entities;

namespace MartialArtsClubManagement.API.Controllers
{
    [Route("api/admin/huanluyenvien")]
    [ApiController]
    // [Authorize(Roles = "Admin")] // Uncomment this when roles are fully implemented in DB
    public class AdminHuanLuyenVienController : ControllerBase
    {
        private readonly QuanLyCLBVoThuatContext _context;

        public AdminHuanLuyenVienController(QuanLyCLBVoThuatContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHuanLuyenVien()
        {
            var hlvs = await _context.HuanLuyenViens
                .Include(h => h.MaTaiKhoanNavigation)
                .Select(h => new AdminHuanLuyenVienDto
                {
                    MaHlv = h.MaHlv,
                    MaTaiKhoan = h.MaTaiKhoan,
                    TenHuanLuyenVien = h.MaTaiKhoanNavigation != null ? h.MaTaiKhoanNavigation.HoTen : null,
                    Email = h.MaTaiKhoanNavigation != null ? h.MaTaiKhoanNavigation.Email : null,
                    SoDienThoai = h.SoDienThoai,
                    ChuyenMon = h.ChuyenMon,
                    NgayVaoClb = h.NgayVaoClb,
                    DangHoatDong = h.DangHoatDong
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<AdminHuanLuyenVienDto>>
            {
                Success = true,
                Message = "Lấy danh sách HLV thành công",
                Data = hlvs
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHuanLuyenVienById(int id)
        {
            var h = await _context.HuanLuyenViens
                .Include(h => h.MaTaiKhoanNavigation)
                .FirstOrDefaultAsync(h => h.MaHlv == id);

            if (h == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Không tìm thấy Huấn luyện viên"
                });
            }

            var dto = new AdminHuanLuyenVienDto
            {
                MaHlv = h.MaHlv,
                MaTaiKhoan = h.MaTaiKhoan,
                TenHuanLuyenVien = h.MaTaiKhoanNavigation?.HoTen,
                Email = h.MaTaiKhoanNavigation?.Email,
                SoDienThoai = h.SoDienThoai,
                ChuyenMon = h.ChuyenMon,
                NgayVaoClb = h.NgayVaoClb,
                DangHoatDong = h.DangHoatDong
            };

            return Ok(new ApiResponse<AdminHuanLuyenVienDto>
            {
                Success = true,
                Message = "Lấy thông tin HLV thành công",
                Data = dto
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateHuanLuyenVien([FromBody] CreateAdminHuanLuyenVienDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            // Check if TaiKhoan exists
            var taiKhoanExists = await _context.TaiKhoans.AnyAsync(t => t.MaTaiKhoan == dto.MaTaiKhoan);
            if (!taiKhoanExists)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Tài khoản không tồn tại" });
            }

            var newHlv = new HuanLuyenVien
            {
                MaTaiKhoan = dto.MaTaiKhoan,
                SoDienThoai = dto.SoDienThoai,
                ChuyenMon = dto.ChuyenMon,
                NgayVaoClb = dto.NgayVaoClb,
                DangHoatDong = dto.DangHoatDong
            };

            _context.HuanLuyenViens.Add(newHlv);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Thêm HLV thành công",
                Data = newHlv.MaHlv
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHuanLuyenVien(int id, [FromBody] UpdateAdminHuanLuyenVienDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            var hlv = await _context.HuanLuyenViens.Include(h => h.MaTaiKhoanNavigation).FirstOrDefaultAsync(h => h.MaHlv == id);
            if (hlv == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy Huấn luyện viên" });
            }

            // Update HuanLuyenVien fields
            hlv.SoDienThoai = dto.SoDienThoai;
            hlv.ChuyenMon = dto.ChuyenMon;
            hlv.NgayVaoClb = dto.NgayVaoClb;
            hlv.DangHoatDong = dto.DangHoatDong;

            // Update TaiKhoan.HoTen if provided
            if (!string.IsNullOrEmpty(dto.HoTen) && hlv.MaTaiKhoanNavigation != null)
            {
                hlv.MaTaiKhoanNavigation.HoTen = dto.HoTen;
                _context.TaiKhoans.Update(hlv.MaTaiKhoanNavigation);
            }

            _context.HuanLuyenViens.Update(hlv);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Cập nhật HLV thành công"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHuanLuyenVien(int id)
        {
            var hlv = await _context.HuanLuyenViens.FindAsync(id);
            if (hlv == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy Huấn luyện viên" });
            }

            _context.HuanLuyenViens.Remove(hlv);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Xóa HLV thành công"
            });
        }
    }
}
