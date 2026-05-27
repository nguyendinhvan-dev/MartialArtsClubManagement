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
    [Route("api/admin/hocphi")]
    [ApiController]
    // [Authorize(Roles = "Admin")]
    public class AdminHocPhiController : ControllerBase
    {
        private readonly QuanLyCLBVoThuatContext _context;

        public AdminHocPhiController(QuanLyCLBVoThuatContext context)
        {
            _context = context;
        }

        // GET: api/admin/hocphi/lop
        [HttpGet("lop")]
        public async Task<IActionResult> GetLopHocPhi([FromQuery] string? trangThai = null)
        {
            var query = _context.DangKyLops
                .Include(d => d.MaHocVienNavigation).ThenInclude(hv => hv.MaTaiKhoanNavigation)
                .Include(d => d.MaLopNavigation).ThenInclude(l => l.MaKhoaHocNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(trangThai))
            {
                query = query.Where(d => d.TrangThaiThanhToan == trangThai);
            }

            var list = await query
                .Select(d => new AdminLopHocPhiDto
                {
                    MaDangKy = d.MaDangKy,
                    MaHocVien = d.MaHocVien,
                    TenHocVien = d.MaHocVienNavigation.MaTaiKhoanNavigation != null 
                        ? d.MaHocVienNavigation.MaTaiKhoanNavigation.HoTen : null,
                    MaLop = d.MaLop,
                    TenKhoaHoc = d.MaLopNavigation.MaKhoaHocNavigation != null 
                        ? d.MaLopNavigation.MaKhoaHocNavigation.TenKhoaHoc : null,
                    LichHoc = d.MaLopNavigation.LichHoc,
                    HocPhi = d.MaLopNavigation.MaKhoaHocNavigation != null 
                        ? d.MaLopNavigation.MaKhoaHocNavigation.HocPhi : 0,
                    NgayDangKy = d.NgayDangKy,
                    TrangThaiThanhToan = d.TrangThaiThanhToan,
                    NgayThanhToan = d.NgayThanhToan
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<AdminLopHocPhiDto>>
            {
                Success = true,
                Message = "Lấy danh sách học phí lớp học thành công",
                Data = list
            });
        }

        // GET: api/admin/hocphi/kem
        [HttpGet("kem")]
        public async Task<IActionResult> GetKemHocPhi([FromQuery] string? trangThai = null)
        {
            var query = _context.DangKyKems
                .Include(d => d.MaHocVienNavigation).ThenInclude(hv => hv.MaTaiKhoanNavigation)
                .Include(d => d.MaGoiNavigation)
                .Include(d => d.MaHlvNavigation).ThenInclude(h => h.MaTaiKhoanNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(trangThai))
            {
                query = query.Where(d => d.TrangThaiThanhToan == trangThai);
            }

            var list = await query
                .Select(d => new AdminKemHocPhiDto
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
                    TrangThaiThanhToan = d.TrangThaiThanhToan,
                    NgayThanhToan = d.NgayThanhToan
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<AdminKemHocPhiDto>>
            {
                Success = true,
                Message = "Lấy danh sách học phí dạy kèm riêng thành công",
                Data = list
            });
        }

        // PUT: api/admin/hocphi/lop/{id}/thanhtoan
        [HttpPut("lop/{id}/thanhtoan")]
        public async Task<IActionResult> PayLopHocPhi(int id, [FromBody] UpdateHocPhiStatusDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            var dangKy = await _context.DangKyLops.FindAsync(id);
            if (dangKy == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy đăng ký lớp" });
            }

            dangKy.TrangThaiThanhToan = dto.TrangThaiThanhToan;
            dangKy.NgayThanhToan = DateTime.Now;

            _context.DangKyLops.Update(dangKy);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Xác nhận thanh toán học phí lớp thành công"
            });
        }

        // PUT: api/admin/hocphi/kem/{id}/thanhtoan
        [HttpPut("kem/{id}/thanhtoan")]
        public async Task<IActionResult> PayKemHocPhi(int id, [FromBody] UpdateHocPhiStatusDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            var dangKyKem = await _context.DangKyKems.FindAsync(id);
            if (dangKyKem == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy đăng ký dạy kèm" });
            }

            dangKyKem.TrangThaiThanhToan = dto.TrangThaiThanhToan;
            dangKyKem.NgayThanhToan = DateTime.Now;

            _context.DangKyKems.Update(dangKyKem);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Xác nhận thanh toán học phí dạy kèm riêng thành công"
            });
        }

        // GET: api/admin/hocphi/thongke
        [HttpGet("thongke")]
        public async Task<IActionResult> GetHocPhiStats()
        {
            // Class registration fees stats
            var listLop = await _context.DangKyLops
                .Include(d => d.MaLopNavigation).ThenInclude(l => l.MaKhoaHocNavigation)
                .ToListAsync();

            var totalLopPaid = listLop.Where(l => l.TrangThaiThanhToan == "Đã thanh toán").Sum(l => l.MaLopNavigation.MaKhoaHocNavigation?.HocPhi ?? 0);
            var totalLopUnpaid = listLop.Where(l => l.TrangThaiThanhToan != "Đã thanh toán").Sum(l => l.MaLopNavigation.MaKhoaHocNavigation?.HocPhi ?? 0);

            // Tutoring packages stats
            var listKem = await _context.DangKyKems
                .Include(d => d.MaGoiNavigation)
                .ToListAsync();

            var totalKemPaid = listKem.Where(k => k.TrangThaiThanhToan == "Đã thanh toán" || k.TrangThaiThanhToan == "DaThanhToan").Sum(k => k.MaGoiNavigation?.HocPhi ?? 0);
            var totalKemUnpaid = listKem.Where(k => k.TrangThaiThanhToan != "Đã thanh toán" && k.TrangThaiThanhToan != "DaThanhToan").Sum(k => k.MaGoiNavigation?.HocPhi ?? 0);

            var stats = new
            {
                TongHocPhiLopDaThu = totalLopPaid,
                TongHocPhiLopChuaThu = totalLopUnpaid,
                TongHocPhiKemDaThu = totalKemPaid,
                TongHocPhiKemChuaThu = totalKemUnpaid,
                TongDoanhThuDaThu = totalLopPaid + totalKemPaid,
                TongDoanhThuChuaThu = totalLopUnpaid + totalKemUnpaid
            };

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Thống kê học phí thành công",
                Data = stats
            });
        }
    }
}
