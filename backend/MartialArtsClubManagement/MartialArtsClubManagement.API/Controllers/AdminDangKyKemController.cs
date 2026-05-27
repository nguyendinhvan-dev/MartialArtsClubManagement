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
    [Route("api/admin/dangkykem")]
    [ApiController]
    // [Authorize(Roles = "Admin")]
    public class AdminDangKyKemController : ControllerBase
    {
        private readonly QuanLyCLBVoThuatContext _context;

        public AdminDangKyKemController(QuanLyCLBVoThuatContext context)
        {
            _context = context;
        }

        // GET: api/admin/dangkykem
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.DangKyKems
                .Include(d => d.MaHocVienNavigation).ThenInclude(hv => hv.MaTaiKhoanNavigation)
                .Include(d => d.MaGoiNavigation)
                .Include(d => d.MaHlvNavigation).ThenInclude(h => h.MaTaiKhoanNavigation)
                .Select(d => new AdminDangKyKemDto
                {
                    MaDangKyKem = d.MaDangKyKem,
                    MaHocVien = d.MaHocVien,
                    TenHocVien = d.MaHocVienNavigation.MaTaiKhoanNavigation != null 
                        ? d.MaHocVienNavigation.MaTaiKhoanNavigation.HoTen : null,
                    MaGoi = d.MaGoi,
                    TenGoi = d.MaGoiNavigation.TenGoi,
                    SoBuoi = d.MaGoiNavigation.SoBuoi,
                    HocPhi = d.MaGoiNavigation.HocPhi,
                    MaHlv = d.MaHlv,
                    TenHuanLuyenVien = d.MaHlvNavigation != null && d.MaHlvNavigation.MaTaiKhoanNavigation != null 
                        ? d.MaHlvNavigation.MaTaiKhoanNavigation.HoTen : null,
                    NgayDangKy = d.NgayDangKy,
                    TrangThaiPhanCong = d.TrangThaiPhanCong,
                    NgayBatDau = d.NgayBatDau,
                    TrangThaiThanhToan = d.TrangThaiThanhToan,
                    NgayThanhToan = d.NgayThanhToan
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<AdminDangKyKemDto>>
            {
                Success = true,
                Message = "Lấy danh sách đăng ký dạy kèm thành công",
                Data = list
            });
        }

        // GET: api/admin/dangkykem/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var d = await _context.DangKyKems
                .Include(d => d.MaHocVienNavigation).ThenInclude(hv => hv.MaTaiKhoanNavigation)
                .Include(d => d.MaGoiNavigation)
                .Include(d => d.MaHlvNavigation).ThenInclude(h => h.MaTaiKhoanNavigation)
                .FirstOrDefaultAsync(d => d.MaDangKyKem == id);

            if (d == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy đăng ký dạy kèm" });
            }

            var dto = new AdminDangKyKemDto
            {
                MaDangKyKem = d.MaDangKyKem,
                MaHocVien = d.MaHocVien,
                TenHocVien = d.MaHocVienNavigation.MaTaiKhoanNavigation?.HoTen,
                MaGoi = d.MaGoi,
                TenGoi = d.MaGoiNavigation?.TenGoi,
                SoBuoi = d.MaGoiNavigation != null ? d.MaGoiNavigation.SoBuoi : 0,
                HocPhi = d.MaGoiNavigation != null ? d.MaGoiNavigation.HocPhi : 0,
                MaHlv = d.MaHlv,
                TenHuanLuyenVien = d.MaHlvNavigation?.MaTaiKhoanNavigation?.HoTen,
                NgayDangKy = d.NgayDangKy,
                TrangThaiPhanCong = d.TrangThaiPhanCong,
                NgayBatDau = d.NgayBatDau,
                TrangThaiThanhToan = d.TrangThaiThanhToan,
                NgayThanhToan = d.NgayThanhToan
            };

            return Ok(new ApiResponse<AdminDangKyKemDto>
            {
                Success = true,
                Message = "Lấy thông tin đăng ký dạy kèm thành công",
                Data = dto
            });
        }

        // POST: api/admin/dangkykem
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAdminDangKyKemDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            var hocVienExists = await _context.HocViens.AnyAsync(hv => hv.MaHocVien == dto.MaHocVien);
            if (!hocVienExists)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Học viên không tồn tại" });
            }

            var goiExists = await _context.GoiKemRiengs.AnyAsync(g => g.MaGoi == dto.MaGoi);
            if (!goiExists)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Gói dạy kèm không tồn tại" });
            }

            if (dto.MaHlv.HasValue)
            {
                var hlvExists = await _context.HuanLuyenViens.AnyAsync(h => h.MaHlv == dto.MaHlv.Value);
                if (!hlvExists)
                {
                    return BadRequest(new ApiResponse<object> { Success = false, Message = "Huấn luyện viên chỉ định không tồn tại" });
                }
            }

            var newDangKy = new DangKyKem
            {
                MaHocVien = dto.MaHocVien,
                MaGoi = dto.MaGoi,
                MaHlv = dto.MaHlv,
                NgayDangKy = dto.NgayDangKy,
                TrangThaiPhanCong = dto.MaHlv.HasValue ? "DaPhanCong" : dto.TrangThaiPhanCong,
                NgayBatDau = dto.NgayBatDau,
                TrangThaiThanhToan = dto.TrangThaiThanhToan
            };

            _context.DangKyKems.Add(newDangKy);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Tạo đăng ký dạy kèm thành công",
                Data = newDangKy.MaDangKyKem
            });
        }

        // PUT: api/admin/dangkykem/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAdminDangKyKemDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            var d = await _context.DangKyKems.FindAsync(id);
            if (d == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy đăng ký dạy kèm" });
            }

            if (dto.MaHlv.HasValue)
            {
                var hlvExists = await _context.HuanLuyenViens.AnyAsync(h => h.MaHlv == dto.MaHlv.Value);
                if (!hlvExists)
                {
                    return BadRequest(new ApiResponse<object> { Success = false, Message = "Huấn luyện viên chỉ định không tồn tại" });
                }
            }

            d.MaHlv = dto.MaHlv;
            d.TrangThaiPhanCong = dto.TrangThaiPhanCong;
            d.NgayBatDau = dto.NgayBatDau;
            d.TrangThaiThanhToan = dto.TrangThaiThanhToan;
            d.NgayThanhToan = dto.NgayThanhToan;

            _context.DangKyKems.Update(d);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Cập nhật đăng ký dạy kèm thành công"
            });
        }

        // PUT: api/admin/dangkykem/{id}/phancong
        [HttpPut("{id}/phancong")]
        public async Task<IActionResult> AssignTutor(int id, [FromBody] AssignTutorDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            var d = await _context.DangKyKems.FindAsync(id);
            if (d == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy đăng ký dạy kèm" });
            }

            var hlvExists = await _context.HuanLuyenViens.AnyAsync(h => h.MaHlv == dto.MaHlv);
            if (!hlvExists)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Huấn luyện viên không tồn tại" });
            }

            d.MaHlv = dto.MaHlv;
            d.TrangThaiPhanCong = "DaPhanCong";

            _context.DangKyKems.Update(d);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Phân công huấn luyện viên dạy kèm thành công"
            });
        }

        // GET: api/admin/dangkykem/chophancong
        [HttpGet("chophancong")]
        public async Task<IActionResult> GetChoPhanCong()
        {
            var list = await _context.VDangKyKemChoPhanCongs
                .Select(d => new DangKyKemChoPhanCongDto
                {
                    MaDangKyKem = d.MaDangKyKem,
                    TenHocVien = d.TenHocVien,
                    SoDienThoai = d.SoDienThoai,
                    TenGoi = d.TenGoi,
                    SoBuoi = d.SoBuoi,
                    HocPhi = d.HocPhi,
                    TenKhoaHoc = d.TenKhoaHoc,
                    NgayDangKy = d.NgayDangKy
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<DangKyKemChoPhanCongDto>>
            {
                Success = true,
                Message = "Lấy danh sách đăng ký dạy kèm chờ phân công thành công",
                Data = list
            });
        }

        // DELETE: api/admin/dangkykem/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var d = await _context.DangKyKems.FindAsync(id);
            if (d == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy đăng ký dạy kèm" });
            }

            _context.DangKyKems.Remove(d);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Xóa đăng ký dạy kèm thành công"
            });
        }
    }
}
