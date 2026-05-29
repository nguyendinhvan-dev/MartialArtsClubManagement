using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MartialArtsClubManagement.API.Models.DTOs;
using MartialArtsClubManagement.API.Models.Entities;

namespace MartialArtsClubManagement.API.Controllers
{
    [Route("api/admin/khoahoc")]
    [ApiController]
    // [Authorize(Roles = "Admin")] // Uncomment this when roles are fully implemented in DB
    public class AdminKhoaHocController : ControllerBase
    {
        private readonly QuanLyCLBVoThuatContext _context;

        public AdminKhoaHocController(QuanLyCLBVoThuatContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllKhoaHoc()
        {
            var khoaHocs = await _context.KhoaHocs
                .Select(k => new AdminKhoaHocDto
                {
                    MaKhoaHoc = k.MaKhoaHoc,
                    TenKhoaHoc = k.TenKhoaHoc,
                    NgayKhaiGiang = k.NgayKhaiGiang,
                    NgayKetThuc = k.NgayKetThuc,
                    HocPhi = k.HocPhi,
                    SoLuongToiDa = k.SoLuongToiDa,
                    TrangThai = k.TrangThai
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<AdminKhoaHocDto>>
            {
                Success = true,
                Message = "Lấy danh sách khóa học thành công",
                Data = khoaHocs
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetKhoaHocById(int id)
        {
            var k = await _context.KhoaHocs.FindAsync(id);

            if (k == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Không tìm thấy khóa học"
                });
            }

            var dto = new AdminKhoaHocDto
            {
                MaKhoaHoc = k.MaKhoaHoc,
                TenKhoaHoc = k.TenKhoaHoc,
                NgayKhaiGiang = k.NgayKhaiGiang,
                NgayKetThuc = k.NgayKetThuc,
                HocPhi = k.HocPhi,
                SoLuongToiDa = k.SoLuongToiDa,
                TrangThai = k.TrangThai
            };

            return Ok(new ApiResponse<AdminKhoaHocDto>
            {
                Success = true,
                Message = "Lấy thông tin khóa học thành công",
                Data = dto
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateKhoaHoc([FromBody] CreateAdminKhoaHocDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            var newKhoaHoc = new KhoaHoc
            {
                TenKhoaHoc = dto.TenKhoaHoc,
                NgayKhaiGiang = dto.NgayKhaiGiang,
                NgayKetThuc = dto.NgayKetThuc,
                HocPhi = dto.HocPhi,
                SoLuongToiDa = dto.SoLuongToiDa,
                TrangThai = dto.TrangThai
            };

            _context.KhoaHocs.Add(newKhoaHoc);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Thêm khóa học thành công",
                Data = newKhoaHoc.MaKhoaHoc
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateKhoaHoc(int id, [FromBody] UpdateAdminKhoaHocDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            var khoaHoc = await _context.KhoaHocs.FindAsync(id);
            if (khoaHoc == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy khóa học" });
            }

            khoaHoc.TenKhoaHoc = dto.TenKhoaHoc;
            khoaHoc.NgayKhaiGiang = dto.NgayKhaiGiang;
            khoaHoc.NgayKetThuc = dto.NgayKetThuc;
            khoaHoc.HocPhi = dto.HocPhi;
            khoaHoc.SoLuongToiDa = dto.SoLuongToiDa;
            khoaHoc.TrangThai = dto.TrangThai;

            _context.KhoaHocs.Update(khoaHoc);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Cập nhật khóa học thành công"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKhoaHoc(int id)
        {
            var khoaHoc = await _context.KhoaHocs.FindAsync(id);
            if (khoaHoc == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy khóa học" });
            }

            // Check if there are related classes
            var hasRelatedClasses = await _context.LopHocs.AnyAsync(l => l.MaKhoaHoc == id);
            if (hasRelatedClasses)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Không thể xóa khóa học này vì có lớp học liên kết. Vui lòng xóa các lớp học trước." });
            }

            _context.KhoaHocs.Remove(khoaHoc);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Xóa khóa học thành công"
            });
        }
    }
}
