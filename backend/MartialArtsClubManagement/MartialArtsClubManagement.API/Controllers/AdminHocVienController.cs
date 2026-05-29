using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MartialArtsClubManagement.API.Models.DTOs;
using MartialArtsClubManagement.API.Models.Entities;

namespace MartialArtsClubManagement.API.Controllers
{
    [Route("api/admin/hocvien")]
    [ApiController]
    // [Authorize(Roles = "Admin")] // Uncomment this when roles are fully implemented in DB
    public class AdminHocVienController : ControllerBase
    {
        private readonly QuanLyCLBVoThuatContext _context;

        public AdminHocVienController(QuanLyCLBVoThuatContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHocVien()
        {
            var hocViens = await _context.HocViens
                .Include(hv => hv.MaTaiKhoanNavigation)
                .Include(hv => hv.MaCapDaiHienTaiNavigation)
                .Select(hv => new AdminHocVienDto
                {
                    MaHocVien = hv.MaHocVien,
                    MaTaiKhoan = hv.MaTaiKhoan,
                    TenHocVien = hv.MaTaiKhoanNavigation != null ? hv.MaTaiKhoanNavigation.HoTen : null,
                    Email = hv.MaTaiKhoanNavigation != null ? hv.MaTaiKhoanNavigation.Email : null,
                    SoDienThoai = hv.SoDienThoai,
                    DiaChi = hv.DiaChi,
                    NgaySinh = hv.NgaySinh,
                    GioiTinh = hv.GioiTinh,
                    NgayGiaNhap = hv.NgayGiaNhap,
                    MaCapDaiHienTai = hv.MaCapDaiHienTai,
                    TenCapDai = hv.MaCapDaiHienTaiNavigation != null ? hv.MaCapDaiHienTaiNavigation.TenCapDai : null
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<AdminHocVienDto>>
            {
                Success = true,
                Message = "Lấy danh sách học viên thành công",
                Data = hocViens
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHocVienById(int id)
        {
            var hv = await _context.HocViens
                .Include(h => h.MaTaiKhoanNavigation)
                .Include(h => h.MaCapDaiHienTaiNavigation)
                .FirstOrDefaultAsync(h => h.MaHocVien == id);

            if (hv == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Không tìm thấy học viên"
                });
            }

            var dto = new AdminHocVienDto
            {
                MaHocVien = hv.MaHocVien,
                MaTaiKhoan = hv.MaTaiKhoan,
                TenHocVien = hv.MaTaiKhoanNavigation?.HoTen,
                Email = hv.MaTaiKhoanNavigation?.Email,
                SoDienThoai = hv.SoDienThoai,
                DiaChi = hv.DiaChi,
                NgaySinh = hv.NgaySinh,
                GioiTinh = hv.GioiTinh,
                NgayGiaNhap = hv.NgayGiaNhap,
                MaCapDaiHienTai = hv.MaCapDaiHienTai,
                TenCapDai = hv.MaCapDaiHienTaiNavigation?.TenCapDai
            };

            return Ok(new ApiResponse<AdminHocVienDto>
            {
                Success = true,
                Message = "Lấy thông tin học viên thành công",
                Data = dto
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateHocVien([FromBody] CreateAdminHocVienDto dto)
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

            var newHocVien = new HocVien
            {
                MaTaiKhoan = dto.MaTaiKhoan,
                SoDienThoai = dto.SoDienThoai,
                DiaChi = dto.DiaChi,
                NgaySinh = dto.NgaySinh,
                GioiTinh = dto.GioiTinh,
                NgayGiaNhap = dto.NgayGiaNhap,
                MaCapDaiHienTai = dto.MaCapDaiHienTai
            };

            _context.HocViens.Add(newHocVien);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Thêm học viên thành công",
                Data = newHocVien.MaHocVien
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHocVien(int id, [FromBody] UpdateAdminHocVienDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            var hocVien = await _context.HocViens.Include(h => h.MaTaiKhoanNavigation).FirstOrDefaultAsync(h => h.MaHocVien == id);
            if (hocVien == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy học viên" });
            }

            // Update HocVien fields
            hocVien.SoDienThoai = dto.SoDienThoai;
            hocVien.DiaChi = dto.DiaChi;
            hocVien.NgaySinh = dto.NgaySinh;
            hocVien.GioiTinh = dto.GioiTinh;
            hocVien.NgayGiaNhap = dto.NgayGiaNhap;
            hocVien.MaCapDaiHienTai = dto.MaCapDaiHienTai;

            // Update TaiKhoan.HoTen if provided
            if (!string.IsNullOrEmpty(dto.HoTen) && hocVien.MaTaiKhoanNavigation != null)
            {
                hocVien.MaTaiKhoanNavigation.HoTen = dto.HoTen;
                _context.TaiKhoans.Update(hocVien.MaTaiKhoanNavigation);
            }

            _context.HocViens.Update(hocVien);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Cập nhật học viên thành công"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHocVien(int id)
        {
            var hocVien = await _context.HocViens.FindAsync(id);
            if (hocVien == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy học viên" });
            }

            _context.HocViens.Remove(hocVien);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Xóa học viên thành công"
            });
        }
    }
}
