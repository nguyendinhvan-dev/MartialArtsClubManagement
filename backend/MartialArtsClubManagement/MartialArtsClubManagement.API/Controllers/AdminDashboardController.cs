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
    [Route("api/admin/dashboard")]
    [ApiController]
    // [Authorize(Roles = "Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly QuanLyCLBVoThuatContext _context;

        public AdminDashboardController(QuanLyCLBVoThuatContext context)
        {
            _context = context;
        }

        // GET: api/admin/dashboard/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var today = DateTime.Today;

            var tongHocVien = await _context.HocViens.CountAsync();
            var tongHlv = await _context.HuanLuyenViens.CountAsync(h => h.DangHoatDong == true);
            var tongLop = await _context.LopHocs.CountAsync();

            // Doanh thu lớp học trong tháng này
            var listLopThangNay = await _context.DangKyLops
                .Include(d => d.MaLopNavigation)
                    .ThenInclude(l => l.MaKhoaHocNavigation)
                .Where(d => d.TrangThaiThanhToan == "Đã thanh toán" 
                            && d.NgayThanhToan.HasValue 
                            && d.NgayThanhToan.Value.Month == today.Month 
                            && d.NgayThanhToan.Value.Year == today.Year)
                .ToListAsync();
            var doanhThuLop = listLopThangNay.Sum(d => d.MaLopNavigation.MaKhoaHocNavigation?.HocPhi ?? 0);

            // Doanh thu dạy kèm trong tháng này
            var listKemThangNay = await _context.DangKyKems
                .Include(d => d.MaGoiNavigation)
                .Where(d => (d.TrangThaiThanhToan == "Đã thanh toán" || d.TrangThaiThanhToan == "DaThanhToan")
                            && d.NgayThanhToan.HasValue 
                            && d.NgayThanhToan.Value.Month == today.Month 
                            && d.NgayThanhToan.Value.Year == today.Year)
                .ToListAsync();
            var doanhThuKem = listKemThangNay.Sum(d => d.MaGoiNavigation?.HocPhi ?? 0);

            var dto = new AdminDashboardStatsDto
            {
                TongHocVien = tongHocVien,
                TongHuanLuyenVien = tongHlv,
                TongLopHoc = tongLop,
                DoanhThuThangNay = doanhThuLop + doanhThuKem
            };

            return Ok(new ApiResponse<AdminDashboardStatsDto>
            {
                Success = true,
                Message = "Lấy số liệu thống kê tổng quan thành công",
                Data = dto
            });
        }

        // GET: api/admin/dashboard/doanhthu-hangthang
        [HttpGet("doanhthu-hangthang")]
        public async Task<IActionResult> GetMonthlyRevenue([FromQuery] int? nam = null)
        {
            var targetYear = nam ?? DateTime.Today.Year;

            // Fetch all paid class registrations for the year
            var allLop = await _context.DangKyLops
                .Include(d => d.MaLopNavigation).ThenInclude(l => l.MaKhoaHocNavigation)
                .Where(d => d.TrangThaiThanhToan == "Đã thanh toán" 
                            && d.NgayThanhToan.HasValue 
                            && d.NgayThanhToan.Value.Year == targetYear)
                .ToListAsync();

            // Fetch all paid tutoring registrations for the year
            var allKem = await _context.DangKyKems
                .Include(d => d.MaGoiNavigation)
                .Where(d => (d.TrangThaiThanhToan == "Đã thanh toán" || d.TrangThaiThanhToan == "DaThanhToan")
                            && d.NgayThanhToan.HasValue 
                            && d.NgayThanhToan.Value.Year == targetYear)
                .ToListAsync();

            var listRevenue = new List<MonthlyRevenueDto>();

            for (int month = 1; month <= 12; month++)
            {
                var revLop = allLop
                    .Where(d => d.NgayThanhToan!.Value.Month == month)
                    .Sum(d => d.MaLopNavigation.MaKhoaHocNavigation?.HocPhi ?? 0);

                var revKem = allKem
                    .Where(d => d.NgayThanhToan!.Value.Month == month)
                    .Sum(d => d.MaGoiNavigation?.HocPhi ?? 0);

                listRevenue.Add(new MonthlyRevenueDto
                {
                    Thang = month,
                    DoanhThuLopHoc = revLop,
                    DoanhThuKemRieng = revKem,
                    TongDoanhThu = revLop + revKem
                });
            }

            return Ok(new ApiResponse<List<MonthlyRevenueDto>>
            {
                Success = true,
                Message = $"Lấy biểu đồ doanh thu hàng tháng năm {targetYear} thành công",
                Data = listRevenue
            });
        }

        // GET: api/admin/dashboard/hocvien-theolop
        [HttpGet("hocvien-theolop")]
        public async Task<IActionResult> GetClassStudentCount()
        {
            var classes = await _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .Include(l => l.DangKyLops)
                .Select(l => new ClassStudentCountDto
                {
                    MaLop = l.MaLop,
                    TenKhoaHoc = l.MaKhoaHocNavigation != null ? l.MaKhoaHocNavigation.TenKhoaHoc : null,
                    LichHoc = l.LichHoc,
                    SoLuongHocVien = l.DangKyLops.Count,
                    SoLuongToiDa = l.SoLuongToiDa
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<ClassStudentCountDto>>
            {
                Success = true,
                Message = "Lấy thống kê số lượng học viên theo lớp thành công",
                Data = classes
            });
        }

        // GET: api/admin/dashboard/phanbo-capdai
        [HttpGet("phanbo-capdai")]
        public async Task<IActionResult> GetBeltDistribution()
        {
            var capDais = await _context.CapDais
                .Include(c => c.HocViens)
                .OrderBy(c => c.ThuTu)
                .Select(c => new BeltDistributionDto
                {
                    MaCapDai = c.MaCapDai,
                    TenCapDai = c.TenCapDai,
                    MauSac = c.MauSac,
                    SoLuongHocVien = c.HocViens.Count
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<BeltDistributionDto>>
            {
                Success = true,
                Message = "Lấy biểu đồ phân bố cấp đai học viên thành công",
                Data = capDais
            });
        }
    }
}
