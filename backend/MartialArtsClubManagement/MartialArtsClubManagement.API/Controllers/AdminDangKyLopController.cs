using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MartialArtsClubManagement.API.Models.DTOs;
using MartialArtsClubManagement.API.Models.Entities;

namespace MartialArtsClubManagement.API.Controllers
{
    [Route("api/admin/dangkylop")]
    [ApiController]
    // [Authorize(Roles = "Admin")]
    public class AdminDangKyLopController : ControllerBase
    {
        private readonly QuanLyCLBVoThuatContext _context;

        public AdminDangKyLopController(QuanLyCLBVoThuatContext context)
        {
            _context = context;
        }

        // GET: api/admin/dangkylop
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.DangKyLops
                .Include(d => d.MaHocVienNavigation).ThenInclude(hv => hv.MaTaiKhoanNavigation)
                .Include(d => d.MaLopNavigation).ThenInclude(l => l.MaKhoaHocNavigation)
                .Select(d => new AdminDangKyLopDto
                {
                    MaDangKy = d.MaDangKy,
                    MaHocVien = d.MaHocVien,
                    TenHocVien = d.MaHocVienNavigation.MaTaiKhoanNavigation != null
                        ? d.MaHocVienNavigation.MaTaiKhoanNavigation.HoTen : null,
                    MaLop = d.MaLop,
                    TenLop = d.MaLopNavigation.MaKhoaHocNavigation != null
                        ? d.MaLopNavigation.MaKhoaHocNavigation.TenKhoaHoc + " - " + d.MaLopNavigation.LichHoc
                        : d.MaLopNavigation.LichHoc,
                    NgayDangKy = d.NgayDangKy,
                    TrangThaiThanhToan = d.TrangThaiThanhToan,
                    NgayThanhToan = d.NgayThanhToan
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<AdminDangKyLopDto>>
            {
                Success = true,
                Message = "Lấy danh sách đăng ký lớp thành công",
                Data = list
            });
        }

        // GET: api/admin/dangkylop/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var d = await _context.DangKyLops
                .Include(x => x.MaHocVienNavigation).ThenInclude(hv => hv.MaTaiKhoanNavigation)
                .Include(x => x.MaLopNavigation).ThenInclude(l => l.MaKhoaHocNavigation)
                .FirstOrDefaultAsync(x => x.MaDangKy == id);

            if (d == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy đăng ký lớp" });

            return Ok(new ApiResponse<AdminDangKyLopDto>
            {
                Success = true,
                Message = "Lấy thông tin đăng ký lớp thành công",
                Data = new AdminDangKyLopDto
                {
                    MaDangKy = d.MaDangKy,
                    MaHocVien = d.MaHocVien,
                    TenHocVien = d.MaHocVienNavigation?.MaTaiKhoanNavigation?.HoTen,
                    MaLop = d.MaLop,
                    TenLop = d.MaLopNavigation?.MaKhoaHocNavigation != null
                        ? d.MaLopNavigation.MaKhoaHocNavigation.TenKhoaHoc + " - " + d.MaLopNavigation.LichHoc
                        : d.MaLopNavigation?.LichHoc,
                    NgayDangKy = d.NgayDangKy,
                    TrangThaiThanhToan = d.TrangThaiThanhToan,
                    NgayThanhToan = d.NgayThanhToan
                }
            });
        }

        // GET: api/admin/dangkylop/by-lop/{maLop} - Lấy danh sách đăng ký theo lớp
        [HttpGet("by-lop/{maLop}")]
        public async Task<IActionResult> GetByLop(int maLop)
        {
            var list = await _context.DangKyLops
                .Where(d => d.MaLop == maLop)
                .Include(d => d.MaHocVienNavigation).ThenInclude(hv => hv.MaTaiKhoanNavigation)
                .Select(d => new AdminDangKyLopDto
                {
                    MaDangKy = d.MaDangKy,
                    MaHocVien = d.MaHocVien,
                    TenHocVien = d.MaHocVienNavigation.MaTaiKhoanNavigation != null
                        ? d.MaHocVienNavigation.MaTaiKhoanNavigation.HoTen : null,
                    MaLop = d.MaLop,
                    NgayDangKy = d.NgayDangKy,
                    TrangThaiThanhToan = d.TrangThaiThanhToan,
                    NgayThanhToan = d.NgayThanhToan
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<AdminDangKyLopDto>>
            {
                Success = true,
                Message = "Lấy danh sách đăng ký theo lớp thành công",
                Data = list
            });
        }

        // POST: api/admin/dangkylop
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAdminDangKyLopDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });

            // Kiểm tra học viên tồn tại
            var hocVienExists = await _context.HocViens.AnyAsync(h => h.MaHocVien == dto.MaHocVien);
            if (!hocVienExists)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Học viên không tồn tại" });

            // Kiểm tra lớp tồn tại
            var lopExists = await _context.LopHocs.AnyAsync(l => l.MaLop == dto.MaLop);
            if (!lopExists)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Lớp học không tồn tại" });

            // Kiểm tra đăng ký trùng
            var alreadyRegistered = await _context.DangKyLops
                .AnyAsync(d => d.MaHocVien == dto.MaHocVien && d.MaLop == dto.MaLop);
            if (alreadyRegistered)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Học viên đã đăng ký lớp này rồi" });

            var newDangKy = new DangKyLop
            {
                MaHocVien = dto.MaHocVien,
                MaLop = dto.MaLop,
                NgayDangKy = dto.NgayDangKy,
                TrangThaiThanhToan = dto.TrangThaiThanhToan
            };

            _context.DangKyLops.Add(newDangKy);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Đăng ký lớp thành công",
                Data = newDangKy.MaDangKy
            });
        }

        // PUT: api/admin/dangkylop/{id} - Cập nhật trạng thái thanh toán
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAdminDangKyLopDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });

            var dangKy = await _context.DangKyLops.FindAsync(id);
            if (dangKy == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy đăng ký lớp" });

            dangKy.TrangThaiThanhToan = dto.TrangThaiThanhToan;
            dangKy.NgayThanhToan = dto.NgayThanhToan;

            _context.DangKyLops.Update(dangKy);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object> { Success = true, Message = "Cập nhật đăng ký lớp thành công" });
        }

        // DELETE: api/admin/dangkylop/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dangKy = await _context.DangKyLops.FindAsync(id);
            if (dangKy == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy đăng ký lớp" });

            _context.DangKyLops.Remove(dangKy);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object> { Success = true, Message = "Xóa đăng ký lớp thành công" });
        }

        // ==================== ĐIỂM DANH ====================

        // GET: api/admin/dangkylop/{maDangKy}/diemdanh - Lấy danh sách điểm danh của 1 đăng ký
        [HttpGet("{maDangKy}/diemdanh")]
        public async Task<IActionResult> GetDiemDanh(int maDangKy)
        {
            var dangKyExists = await _context.DangKyLops
                .Include(d => d.MaHocVienNavigation).ThenInclude(hv => hv.MaTaiKhoanNavigation)
                .FirstOrDefaultAsync(d => d.MaDangKy == maDangKy);

            if (dangKyExists == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy đăng ký lớp" });

            var list = await _context.DiemDanhs
                .Where(dd => dd.MaDangKy == maDangKy)
                .OrderByDescending(dd => dd.NgayHoc)
                .Select(dd => new AdminDiemDanhDto
                {
                    MaDiemDanh = dd.MaDiemDanh,
                    MaDangKy = dd.MaDangKy,
                    TenHocVien = dangKyExists.MaHocVienNavigation.MaTaiKhoanNavigation != null
                        ? dangKyExists.MaHocVienNavigation.MaTaiKhoanNavigation.HoTen : null,
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

        // POST: api/admin/dangkylop/{maDangKy}/diemdanh - Thêm điểm danh
        [HttpPost("{maDangKy}/diemdanh")]
        public async Task<IActionResult> CreateDiemDanh(int maDangKy, [FromBody] CreateAdminDiemDanhDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });

            var dangKyExists = await _context.DangKyLops.AnyAsync(d => d.MaDangKy == maDangKy);
            if (!dangKyExists)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy đăng ký lớp" });

            try
            {
                // Use raw SQL to avoid potential trigger/cascade issues
                var sql = @"
                    IF EXISTS (SELECT 1 FROM DiemDanh WHERE MaDangKy = @maDangKy AND NgayHoc = @ngayHoc)
                        UPDATE DiemDanh 
                        SET TrangThai = @trangThai, GhiChu = @ghiChu
                        WHERE MaDangKy = @maDangKy AND NgayHoc = @ngayHoc
                    ELSE
                        INSERT INTO DiemDanh (MaDangKy, NgayHoc, TrangThai, GhiChu)
                        VALUES (@maDangKy, @ngayHoc, @trangThai, @ghiChu)";

                var parameters = new[]
                {
                    new Microsoft.Data.SqlClient.SqlParameter("@maDangKy", maDangKy),
                    new Microsoft.Data.SqlClient.SqlParameter("@ngayHoc", dto.NgayHoc),
                    new Microsoft.Data.SqlClient.SqlParameter("@trangThai", dto.TrangThai),
                    new Microsoft.Data.SqlClient.SqlParameter("@ghiChu", (object?)dto.GhiChu ?? DBNull.Value)
                };

                await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                return Ok(new ApiResponse<int>
                {
                    Success = true,
                    Message = "Thêm điểm danh thành công",
                    Data = 0
                });
            }
            catch (Exception ex)
            {
                // Log the error and return a more detailed message
                return BadRequest(new ApiResponse<object> { Success = false, Message = $"Lỗi khi lưu điểm danh: {ex.Message}" });
            }
        }
    }
}
