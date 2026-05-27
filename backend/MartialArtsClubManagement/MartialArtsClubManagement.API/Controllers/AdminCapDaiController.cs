using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MartialArtsClubManagement.API.Models.DTOs;
using MartialArtsClubManagement.API.Models.Entities;

namespace MartialArtsClubManagement.API.Controllers
{
    [Route("api/admin/capdai")]
    [ApiController]
    // [Authorize(Roles = "Admin")]
    public class AdminCapDaiController : ControllerBase
    {
        private readonly QuanLyCLBVoThuatContext _context;

        public AdminCapDaiController(QuanLyCLBVoThuatContext context)
        {
            _context = context;
        }

        // GET: api/admin/capdai
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.CapDais
                .OrderBy(c => c.ThuTu)
                .Select(c => new AdminCapDaiDto
                {
                    MaCapDai = c.MaCapDai,
                    TenCapDai = c.TenCapDai,
                    MauSac = c.MauSac,
                    ThuTu = c.ThuTu,
                    ThoiGianToiThieuThang = c.ThoiGianToiThieuThang
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<AdminCapDaiDto>>
            {
                Success = true,
                Message = "Lấy danh sách cấp đai thành công",
                Data = list
            });
        }

        // GET: api/admin/capdai/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var c = await _context.CapDais.FindAsync(id);
            if (c == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy cấp đai" });

            return Ok(new ApiResponse<AdminCapDaiDto>
            {
                Success = true,
                Message = "Lấy thông tin cấp đai thành công",
                Data = new AdminCapDaiDto
                {
                    MaCapDai = c.MaCapDai,
                    TenCapDai = c.TenCapDai,
                    MauSac = c.MauSac,
                    ThuTu = c.ThuTu,
                    ThoiGianToiThieuThang = c.ThoiGianToiThieuThang
                }
            });
        }

        // POST: api/admin/capdai
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAdminCapDaiDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });

            var newCapDai = new CapDai
            {
                TenCapDai = dto.TenCapDai,
                MauSac = dto.MauSac,
                ThuTu = dto.ThuTu,
                ThoiGianToiThieuThang = dto.ThoiGianToiThieuThang
            };

            _context.CapDais.Add(newCapDai);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Thêm cấp đai thành công",
                Data = newCapDai.MaCapDai
            });
        }

        // PUT: api/admin/capdai/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAdminCapDaiDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });

            var capDai = await _context.CapDais.FindAsync(id);
            if (capDai == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy cấp đai" });

            capDai.TenCapDai = dto.TenCapDai;
            capDai.MauSac = dto.MauSac;
            capDai.ThuTu = dto.ThuTu;
            capDai.ThoiGianToiThieuThang = dto.ThoiGianToiThieuThang;

            _context.CapDais.Update(capDai);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object> { Success = true, Message = "Cập nhật cấp đai thành công" });
        }

        // DELETE: api/admin/capdai/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var capDai = await _context.CapDais.FindAsync(id);
            if (capDai == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy cấp đai" });

            // Kiểm tra xem cấp đai có đang được sử dụng không
            var isInUse = await _context.HocViens.AnyAsync(hv => hv.MaCapDaiHienTai == id);
            if (isInUse)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Không thể xóa cấp đai đang được sử dụng bởi học viên" });

            _context.CapDais.Remove(capDai);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object> { Success = true, Message = "Xóa cấp đai thành công" });
        }
    }
}
