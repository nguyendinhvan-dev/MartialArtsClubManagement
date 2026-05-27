using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MartialArtsClubManagement.API.Models.DTOs;
using MartialArtsClubManagement.API.Models.Entities;

namespace MartialArtsClubManagement.API.Controllers
{
    [Route("api/admin/lophoc")]
    [ApiController]
    // [Authorize(Roles = "Admin")] // Uncomment this when roles are fully implemented in DB
    public class AdminLopHocController : ControllerBase
    {
        private readonly QuanLyCLBVoThuatContext _context;

        public AdminLopHocController(QuanLyCLBVoThuatContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLopHoc()
        {
            var lops = await _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .Include(l => l.MaCapDaiNavigation)
                .Include(l => l.MaHlvNavigation)
                    .ThenInclude(h => h.MaTaiKhoanNavigation)
                .Select(l => new AdminLopHocDto
                {
                    MaLop = l.MaLop,
                    MaKhoaHoc = l.MaKhoaHoc,
                    TenKhoaHoc = l.MaKhoaHocNavigation != null ? l.MaKhoaHocNavigation.TenKhoaHoc : null,
                    MaCapDai = l.MaCapDai,
                    TenCapDai = l.MaCapDaiNavigation != null ? l.MaCapDaiNavigation.TenCapDai : null,
                    MaHlv = l.MaHlv,
                    TenHuanLuyenVien = l.MaHlvNavigation != null && l.MaHlvNavigation.MaTaiKhoanNavigation != null 
                                       ? l.MaHlvNavigation.MaTaiKhoanNavigation.HoTen : null,
                    LichHoc = l.LichHoc,
                    SoLuongToiDa = l.SoLuongToiDa,
                    PhongTap = l.PhongTap
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<AdminLopHocDto>>
            {
                Success = true,
                Message = "Lấy danh sách lớp học thành công",
                Data = lops
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLopHocById(int id)
        {
            var l = await _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .Include(l => l.MaCapDaiNavigation)
                .Include(l => l.MaHlvNavigation)
                    .ThenInclude(h => h.MaTaiKhoanNavigation)
                .FirstOrDefaultAsync(l => l.MaLop == id);

            if (l == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Không tìm thấy lớp học"
                });
            }

            var dto = new AdminLopHocDto
            {
                MaLop = l.MaLop,
                MaKhoaHoc = l.MaKhoaHoc,
                TenKhoaHoc = l.MaKhoaHocNavigation?.TenKhoaHoc,
                MaCapDai = l.MaCapDai,
                TenCapDai = l.MaCapDaiNavigation?.TenCapDai,
                MaHlv = l.MaHlv,
                TenHuanLuyenVien = l.MaHlvNavigation?.MaTaiKhoanNavigation?.HoTen,
                LichHoc = l.LichHoc,
                SoLuongToiDa = l.SoLuongToiDa,
                PhongTap = l.PhongTap
            };

            return Ok(new ApiResponse<AdminLopHocDto>
            {
                Success = true,
                Message = "Lấy thông tin lớp học thành công",
                Data = dto
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateLopHoc([FromBody] CreateAdminLopHocDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            // Optional: validate KhoaHoc, CapDai, HLV exists
            var khoaHocExists = await _context.KhoaHocs.AnyAsync(k => k.MaKhoaHoc == dto.MaKhoaHoc);
            if (!khoaHocExists) return BadRequest(new ApiResponse<object> { Success = false, Message = "Khóa học không tồn tại" });

            var newLopHoc = new LopHoc
            {
                MaKhoaHoc = dto.MaKhoaHoc,
                MaCapDai = dto.MaCapDai,
                MaHlv = dto.MaHlv,
                LichHoc = dto.LichHoc,
                SoLuongToiDa = dto.SoLuongToiDa,
                PhongTap = dto.PhongTap
            };

            _context.LopHocs.Add(newLopHoc);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Thêm lớp học thành công",
                Data = newLopHoc.MaLop
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLopHoc(int id, [FromBody] UpdateAdminLopHocDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            var lopHoc = await _context.LopHocs.FindAsync(id);
            if (lopHoc == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy lớp học" });
            }

            lopHoc.MaKhoaHoc = dto.MaKhoaHoc;
            lopHoc.MaCapDai = dto.MaCapDai;
            lopHoc.MaHlv = dto.MaHlv;
            lopHoc.LichHoc = dto.LichHoc;
            lopHoc.SoLuongToiDa = dto.SoLuongToiDa;
            lopHoc.PhongTap = dto.PhongTap;

            _context.LopHocs.Update(lopHoc);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Cập nhật lớp học thành công"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLopHoc(int id)
        {
            var lopHoc = await _context.LopHocs.FindAsync(id);
            if (lopHoc == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy lớp học" });
            }

            _context.LopHocs.Remove(lopHoc);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Xóa lớp học thành công"
            });
        }
    }
}
