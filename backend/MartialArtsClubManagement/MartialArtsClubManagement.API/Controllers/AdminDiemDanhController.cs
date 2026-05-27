using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MartialArtsClubManagement.API.Models.DTOs;
using MartialArtsClubManagement.API.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartialArtsClubManagement.API.Controllers
{
    [Route("api/admin/diemdanh")]
    [ApiController]
    // [Authorize(Roles = "Admin")]
    public class AdminDiemDanhController : ControllerBase
    {
        private readonly QuanLyCLBVoThuatContext _context;

        public AdminDiemDanhController(QuanLyCLBVoThuatContext context)
        {
            _context = context;
        }

        // GET: api/admin/diemdanh
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? maLop = null,
            [FromQuery] DateOnly? ngayHoc = null,
            [FromQuery] int? maHocVien = null)
        {
            var query = _context.DiemDanhs
                .Include(dd => dd.MaDangKyNavigation)
                    .ThenInclude(d => d.MaHocVienNavigation)
                        .ThenInclude(hv => hv.MaTaiKhoanNavigation)
                .Include(dd => dd.MaDangKyNavigation)
                    .ThenInclude(d => d.MaLopNavigation)
                        .ThenInclude(l => l.MaKhoaHocNavigation)
                .AsQueryable();

            if (maLop.HasValue)
            {
                query = query.Where(dd => dd.MaDangKyNavigation.MaLop == maLop.Value);
            }

            if (ngayHoc.HasValue)
            {
                query = query.Where(dd => dd.NgayHoc == ngayHoc.Value);
            }

            if (maHocVien.HasValue)
            {
                query = query.Where(dd => dd.MaDangKyNavigation.MaHocVien == maHocVien.Value);
            }

            var list = await query
                .OrderByDescending(dd => dd.NgayHoc)
                .Select(dd => new AdminDiemDanhDto
                {
                    MaDiemDanh = dd.MaDiemDanh,
                    MaDangKy = dd.MaDangKy,
                    TenHocVien = dd.MaDangKyNavigation.MaHocVienNavigation.MaTaiKhoanNavigation != null 
                        ? dd.MaDangKyNavigation.MaHocVienNavigation.MaTaiKhoanNavigation.HoTen : null,
                    NgayHoc = dd.NgayHoc,
                    TrangThai = dd.TrangThai,
                    GhiChu = dd.GhiChu
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<AdminDiemDanhDto>>
            {
                Success = true,
                Message = "Lấy danh sách điểm danh thành công",
                Data = list
            });
        }

        // GET: api/admin/diemdanh/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dd = await _context.DiemDanhs
                .Include(d => d.MaDangKyNavigation)
                    .ThenInclude(d => d.MaHocVienNavigation)
                        .ThenInclude(hv => hv.MaTaiKhoanNavigation)
                .FirstOrDefaultAsync(d => d.MaDiemDanh == id);

            if (dd == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Không tìm thấy bản ghi điểm danh"
                });
            }

            var dto = new AdminDiemDanhDto
            {
                MaDiemDanh = dd.MaDiemDanh,
                MaDangKy = dd.MaDangKy,
                TenHocVien = dd.MaDangKyNavigation.MaHocVienNavigation.MaTaiKhoanNavigation?.HoTen,
                NgayHoc = dd.NgayHoc,
                TrangThai = dd.TrangThai,
                GhiChu = dd.GhiChu
            };

            return Ok(new ApiResponse<AdminDiemDanhDto>
            {
                Success = true,
                Message = "Lấy thông tin điểm danh thành công",
                Data = dto
            });
        }

        // POST: api/admin/diemdanh
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAdminDiemDanhDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            var dangKyExists = await _context.DangKyLops.AnyAsync(d => d.MaDangKy == dto.MaDangKy);
            if (!dangKyExists)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Thông tin đăng ký lớp không tồn tại" });
            }

            // Kiểm tra điểm danh trùng trong ngày
            var isDuplicated = await _context.DiemDanhs
                .AnyAsync(dd => dd.MaDangKy == dto.MaDangKy && dd.NgayHoc == dto.NgayHoc);
            if (isDuplicated)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Học viên đã được điểm danh trong ngày này rồi" });
            }

            var newDiemDanh = new DiemDanh
            {
                MaDangKy = dto.MaDangKy,
                NgayHoc = dto.NgayHoc,
                TrangThai = dto.TrangThai,
                GhiChu = dto.GhiChu
            };

            _context.DiemDanhs.Add(newDiemDanh);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Điểm danh học viên thành công",
                Data = newDiemDanh.MaDiemDanh
            });
        }

        // POST: api/admin/diemdanh/bulk
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulk([FromBody] BulkDiemDanhDto dto)
        {
            if (dto == null || dto.Attendances == null || !dto.Attendances.Any())
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Danh sách điểm danh trống" });
            }

            var createdCount = 0;
            var duplicatedCount = 0;

            foreach (var item in dto.Attendances)
            {
                var dangKyExists = await _context.DangKyLops.AnyAsync(d => d.MaDangKy == item.MaDangKy);
                if (!dangKyExists) continue;

                var isDuplicated = await _context.DiemDanhs
                    .AnyAsync(dd => dd.MaDangKy == item.MaDangKy && dd.NgayHoc == dto.NgayHoc);
                if (isDuplicated)
                {
                    // Nếu đã có điểm danh, chúng ta cập nhật trạng thái
                    var existing = await _context.DiemDanhs
                        .FirstOrDefaultAsync(dd => dd.MaDangKy == item.MaDangKy && dd.NgayHoc == dto.NgayHoc);
                    if (existing != null)
                    {
                        existing.TrangThai = item.TrangThai;
                        existing.GhiChu = item.GhiChu;
                        _context.DiemDanhs.Update(existing);
                        duplicatedCount++;
                    }
                    continue;
                }

                var newDiemDanh = new DiemDanh
                {
                    MaDangKy = item.MaDangKy,
                    NgayHoc = dto.NgayHoc,
                    TrangThai = item.TrangThai,
                    GhiChu = item.GhiChu
                };

                _context.DiemDanhs.Add(newDiemDanh);
                createdCount++;
            }

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = $"Điểm danh hàng loạt hoàn tất. Đã thêm mới: {createdCount}, đã cập nhật: {duplicatedCount}"
            });
        }

        // PUT: api/admin/diemdanh/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAdminDiemDanhStatusDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            var dd = await _context.DiemDanhs.FindAsync(id);
            if (dd == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy bản ghi điểm danh" });
            }

            dd.TrangThai = dto.TrangThai;
            dd.GhiChu = dto.GhiChu;

            _context.DiemDanhs.Update(dd);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Cập nhật trạng thái điểm danh thành công"
            });
        }

        // DELETE: api/admin/diemdanh/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dd = await _context.DiemDanhs.FindAsync(id);
            if (dd == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy bản ghi điểm danh" });
            }

            _context.DiemDanhs.Remove(dd);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Xóa bản ghi điểm danh thành công"
            });
        }
    }

    public class BulkDiemDanhDto
    {
        public DateOnly NgayHoc { get; set; }
        public List<StudentAttendanceItemDto> Attendances { get; set; } = new();
    }

    public class StudentAttendanceItemDto
    {
        public int MaDangKy { get; set; }
        public string TrangThai { get; set; } = "CóMặt";
        public string? GhiChu { get; set; }
    }

    public class UpdateAdminDiemDanhStatusDto
    {
        public string TrangThai { get; set; } = null!;
        public string? GhiChu { get; set; }
    }
}
