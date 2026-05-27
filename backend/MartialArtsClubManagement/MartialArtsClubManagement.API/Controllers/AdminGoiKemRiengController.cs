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
    [Route("api/admin/goikemrieng")]
    [ApiController]
    // [Authorize(Roles = "Admin")]
    public class AdminGoiKemRiengController : ControllerBase
    {
        private readonly QuanLyCLBVoThuatContext _context;

        public AdminGoiKemRiengController(QuanLyCLBVoThuatContext context)
        {
            _context = context;
        }

        // GET: api/admin/goikemrieng
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.GoiKemRiengs
                .Include(g => g.MaKhoaHocNavigation)
                .Select(g => new AdminGoiKemRiengDto
                {
                    MaGoi = g.MaGoi,
                    TenGoi = g.TenGoi,
                    SoBuoi = g.SoBuoi,
                    HocPhi = g.HocPhi,
                    MaKhoaHoc = g.MaKhoaHoc,
                    TenKhoaHoc = g.MaKhoaHocNavigation != null ? g.MaKhoaHocNavigation.TenKhoaHoc : null,
                    DangSuDung = g.DangSuDung
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<AdminGoiKemRiengDto>>
            {
                Success = true,
                Message = "Lấy danh sách gói dạy kèm riêng thành công",
                Data = list
            });
        }

        // GET: api/admin/goikemrieng/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var g = await _context.GoiKemRiengs
                .Include(g => g.MaKhoaHocNavigation)
                .FirstOrDefaultAsync(g => g.MaGoi == id);

            if (g == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy gói dạy kèm riêng" });
            }

            var dto = new AdminGoiKemRiengDto
            {
                MaGoi = g.MaGoi,
                TenGoi = g.TenGoi,
                SoBuoi = g.SoBuoi,
                HocPhi = g.HocPhi,
                MaKhoaHoc = g.MaKhoaHoc,
                TenKhoaHoc = g.MaKhoaHocNavigation?.TenKhoaHoc,
                DangSuDung = g.DangSuDung
            };

            return Ok(new ApiResponse<AdminGoiKemRiengDto>
            {
                Success = true,
                Message = "Lấy thông tin gói dạy kèm riêng thành công",
                Data = dto
            });
        }

        // POST: api/admin/goikemrieng
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAdminGoiKemRiengDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            var khoaHocExists = await _context.KhoaHocs.AnyAsync(k => k.MaKhoaHoc == dto.MaKhoaHoc);
            if (!khoaHocExists)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Khóa học chỉ định không tồn tại" });
            }

            var newGoi = new GoiKemRieng
            {
                TenGoi = dto.TenGoi,
                SoBuoi = dto.SoBuoi,
                HocPhi = dto.HocPhi,
                MaKhoaHoc = dto.MaKhoaHoc,
                DangSuDung = dto.DangSuDung
            };

            _context.GoiKemRiengs.Add(newGoi);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Thêm gói dạy kèm riêng thành công",
                Data = newGoi.MaGoi
            });
        }

        // PUT: api/admin/goikemrieng/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAdminGoiKemRiengDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            var g = await _context.GoiKemRiengs.FindAsync(id);
            if (g == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy gói dạy kèm riêng" });
            }

            var khoaHocExists = await _context.KhoaHocs.AnyAsync(k => k.MaKhoaHoc == dto.MaKhoaHoc);
            if (!khoaHocExists)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Khóa học chỉ định không tồn tại" });
            }

            g.TenGoi = dto.TenGoi;
            g.SoBuoi = dto.SoBuoi;
            g.HocPhi = dto.HocPhi;
            g.MaKhoaHoc = dto.MaKhoaHoc;
            g.DangSuDung = dto.DangSuDung;

            _context.GoiKemRiengs.Update(g);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Cập nhật gói dạy kèm riêng thành công"
            });
        }

        // DELETE: api/admin/goikemrieng/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var g = await _context.GoiKemRiengs.FindAsync(id);
            if (g == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy gói dạy kèm riêng" });
            }

            // Để tránh lỗi ràng buộc toàn vẹn dữ liệu, chúng ta thực hiện ngưng kích hoạt thay vì xóa vật lý
            // nếu gói này đã có người đăng ký
            var hasRegistrations = await _context.DangKyKems.AnyAsync(d => d.MaGoi == id);
            if (hasRegistrations)
            {
                g.DangSuDung = false;
                _context.GoiKemRiengs.Update(g);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Gói dạy kèm này đã được sử dụng. Hệ thống đã chuyển trạng thái sang ngưng kích hoạt (DangSuDung = false)"
                });
            }

            _context.GoiKemRiengs.Remove(g);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Xóa gói dạy kèm riêng thành công"
            });
        }
    }
}
