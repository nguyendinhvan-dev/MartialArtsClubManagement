using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MartialArtsClubManagement.API.Models.DTOs;
using MartialArtsClubManagement.API.Models.Entities;

namespace MartialArtsClubManagement.API.Controllers
{
    [Route("api/admin/taikhoan")]
    [ApiController]
    // [Authorize(Roles = "Admin")]
    public class AdminTaiKhoanController : ControllerBase
    {
        private readonly QuanLyCLBVoThuatContext _context;

        public AdminTaiKhoanController(QuanLyCLBVoThuatContext context)
        {
            _context = context;
        }

        // GET: api/admin/taikhoan
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.TaiKhoans
                .Select(t => new AdminTaiKhoanDto
                {
                    MaTaiKhoan = t.MaTaiKhoan,
                    HoTen = t.HoTen,
                    Email = t.Email,
                    VaiTro = t.VaiTro,
                    DangHoatDong = t.DangHoatDong,
                    NgayTao = t.NgayTao
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<AdminTaiKhoanDto>>
            {
                Success = true,
                Message = "Lấy danh sách tài khoản thành công",
                Data = list
            });
        }

        // GET: api/admin/taikhoan/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var t = await _context.TaiKhoans.FindAsync(id);
            if (t == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy tài khoản" });

            return Ok(new ApiResponse<AdminTaiKhoanDto>
            {
                Success = true,
                Message = "Lấy thông tin tài khoản thành công",
                Data = new AdminTaiKhoanDto
                {
                    MaTaiKhoan = t.MaTaiKhoan,
                    HoTen = t.HoTen,
                    Email = t.Email,
                    VaiTro = t.VaiTro,
                    DangHoatDong = t.DangHoatDong,
                    NgayTao = t.NgayTao
                }
            });
        }

        // POST: api/admin/taikhoan
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAdminTaiKhoanDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });

            // Check duplicate email
            var emailExists = await _context.TaiKhoans.AnyAsync(t => t.Email == dto.Email);
            if (emailExists)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Email đã tồn tại" });

            // Hash mật khẩu đơn giản bằng BCrypt (hoặc dùng SHA256 tùy cấu hình)
            var newTaiKhoan = new TaiKhoan
            {
                HoTen = dto.HoTen,
                Email = dto.Email,
                MatKhauHash = BCrypt.Net.BCrypt.HashPassword(dto.MatKhau),
                VaiTro = dto.VaiTro,
                DangHoatDong = dto.DangHoatDong,
                NgayTao = DateTime.UtcNow
            };

            _context.TaiKhoans.Add(newTaiKhoan);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Tạo tài khoản thành công",
                Data = newTaiKhoan.MaTaiKhoan
            });
        }

        // PUT: api/admin/taikhoan/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAdminTaiKhoanDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });

            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy tài khoản" });

            taiKhoan.HoTen = dto.HoTen;
            taiKhoan.VaiTro = dto.VaiTro;
            taiKhoan.DangHoatDong = dto.DangHoatDong;

            _context.TaiKhoans.Update(taiKhoan);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object> { Success = true, Message = "Cập nhật tài khoản thành công" });
        }

        // DELETE: api/admin/taikhoan/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy tài khoản" });

            _context.TaiKhoans.Remove(taiKhoan);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object> { Success = true, Message = "Xóa tài khoản thành công" });
        }

        // PATCH: api/admin/taikhoan/{id}/toggle-status - Bật/Tắt trạng thái tài khoản
        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy tài khoản" });

            taiKhoan.DangHoatDong = !taiKhoan.DangHoatDong;
            await _context.SaveChangesAsync();

            var status = taiKhoan.DangHoatDong ? "kích hoạt" : "vô hiệu hóa";
            return Ok(new ApiResponse<object> { Success = true, Message = $"Tài khoản đã được {status}" });
        }
    }
}
