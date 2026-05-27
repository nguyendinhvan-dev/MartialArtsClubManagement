using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MartialArtsClubManagement.API.Models.DTOs;
using MartialArtsClubManagement.API.Models.Entities;

namespace MartialArtsClubManagement.API.Controllers
{
    [Route("api/admin/thongbao")]
    [ApiController]
    // [Authorize(Roles = "Admin")]
    public class AdminThongBaoController : ControllerBase
    {
        private readonly QuanLyCLBVoThuatContext _context;

        public AdminThongBaoController(QuanLyCLBVoThuatContext context)
        {
            _context = context;
        }

        // GET: api/admin/thongbao
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.ThongBaos
                .Include(t => t.MaTaiKhoanTaoNavigation)
                .OrderByDescending(t => t.NgayDang)
                .Select(t => new AdminThongBaoDto
                {
                    MaThongBao = t.MaThongBao,
                    MaTaiKhoanTao = t.MaTaiKhoanTao,
                    TenNguoiTao = t.MaTaiKhoanTaoNavigation != null ? t.MaTaiKhoanTaoNavigation.HoTen : null,
                    TieuDe = t.TieuDe,
                    NoiDung = t.NoiDung,
                    LoaiThongBao = t.LoaiThongBao,
                    NgayDang = t.NgayDang
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<AdminThongBaoDto>>
            {
                Success = true,
                Message = "Lấy danh sách thông báo thành công",
                Data = list
            });
        }

        // GET: api/admin/thongbao/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var t = await _context.ThongBaos
                .Include(x => x.MaTaiKhoanTaoNavigation)
                .FirstOrDefaultAsync(x => x.MaThongBao == id);

            if (t == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy thông báo" });

            return Ok(new ApiResponse<AdminThongBaoDto>
            {
                Success = true,
                Message = "Lấy thông tin thông báo thành công",
                Data = new AdminThongBaoDto
                {
                    MaThongBao = t.MaThongBao,
                    MaTaiKhoanTao = t.MaTaiKhoanTao,
                    TenNguoiTao = t.MaTaiKhoanTaoNavigation?.HoTen,
                    TieuDe = t.TieuDe,
                    NoiDung = t.NoiDung,
                    LoaiThongBao = t.LoaiThongBao,
                    NgayDang = t.NgayDang
                }
            });
        }

        // POST: api/admin/thongbao
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAdminThongBaoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });

            var taiKhoanExists = await _context.TaiKhoans.AnyAsync(t => t.MaTaiKhoan == dto.MaTaiKhoanTao);
            if (!taiKhoanExists)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Tài khoản người tạo không tồn tại" });

            var newThongBao = new ThongBao
            {
                MaTaiKhoanTao = dto.MaTaiKhoanTao,
                TieuDe = dto.TieuDe,
                NoiDung = dto.NoiDung,
                LoaiThongBao = dto.LoaiThongBao,
                NgayDang = DateTime.UtcNow
            };

            _context.ThongBaos.Add(newThongBao);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Tạo thông báo thành công",
                Data = newThongBao.MaThongBao
            });
        }

        // PUT: api/admin/thongbao/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAdminThongBaoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });

            var thongBao = await _context.ThongBaos.FindAsync(id);
            if (thongBao == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy thông báo" });

            thongBao.TieuDe = dto.TieuDe;
            thongBao.NoiDung = dto.NoiDung;
            thongBao.LoaiThongBao = dto.LoaiThongBao;

            _context.ThongBaos.Update(thongBao);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object> { Success = true, Message = "Cập nhật thông báo thành công" });
        }

        // DELETE: api/admin/thongbao/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var thongBao = await _context.ThongBaos.FindAsync(id);
            if (thongBao == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy thông báo" });

            _context.ThongBaos.Remove(thongBao);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object> { Success = true, Message = "Xóa thông báo thành công" });
        }
    }
}
