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
    [Route("api/admin/kythithangdai")]
    [ApiController]
    // [Authorize(Roles = "Admin")]
    public class AdminKyThiThangDaiController : ControllerBase
    {
        private readonly QuanLyCLBVoThuatContext _context;

        public AdminKyThiThangDaiController(QuanLyCLBVoThuatContext context)
        {
            _context = context;
        }

        // GET: api/admin/kythithangdai
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.KyThiThangDais
                .Include(k => k.MaKhoaHocNavigation)
                .OrderByDescending(k => k.NgayThi)
                .Select(k => new AdminKyThiThangDaiDto
                {
                    MaKyThi = k.MaKyThi,
                    MaKhoaHoc = k.MaKhoaHoc,
                    TenKhoaHoc = k.MaKhoaHocNavigation.TenKhoaHoc,
                    NgayThi = k.NgayThi,
                    MoTa = k.MoTa,
                    TrangThai = k.TrangThai
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<AdminKyThiThangDaiDto>>
            {
                Success = true,
                Message = "Lấy danh sách kỳ thi thăng đai thành công",
                Data = list
            });
        }

        // GET: api/admin/kythithangdai/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var kyThi = await _context.KyThiThangDais
                .Include(k => k.MaKhoaHocNavigation)
                .FirstOrDefaultAsync(k => k.MaKyThi == id);

            if (kyThi == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy kỳ thi" });

            return Ok(new ApiResponse<AdminKyThiThangDaiDto>
            {
                Success = true,
                Message = "Lấy thông tin kỳ thi thành công",
                Data = new AdminKyThiThangDaiDto
                {
                    MaKyThi = kyThi.MaKyThi,
                    MaKhoaHoc = kyThi.MaKhoaHoc,
                    TenKhoaHoc = kyThi.MaKhoaHocNavigation.TenKhoaHoc,
                    NgayThi = kyThi.NgayThi,
                    MoTa = kyThi.MoTa,
                    TrangThai = kyThi.TrangThai
                }
            });
        }

        // POST: api/admin/kythithangdai
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAdminKyThiThangDaiDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });

            var khoaHocExists = await _context.KhoaHocs.AnyAsync(k => k.MaKhoaHoc == dto.MaKhoaHoc);
            if (!khoaHocExists)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Khóa học không tồn tại" });

            var newKyThi = new KyThiThangDai
            {
                MaKhoaHoc = dto.MaKhoaHoc,
                NgayThi = dto.NgayThi,
                MoTa = dto.MoTa,
                TrangThai = dto.TrangThai
            };

            _context.KyThiThangDais.Add(newKyThi);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Tạo kỳ thi thăng đai thành công",
                Data = newKyThi.MaKyThi
            });
        }

        // PUT: api/admin/kythithangdai/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAdminKyThiThangDaiDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });

            var kyThi = await _context.KyThiThangDais.FindAsync(id);
            if (kyThi == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy kỳ thi" });

            kyThi.NgayThi = dto.NgayThi;
            kyThi.MoTa = dto.MoTa;
            kyThi.TrangThai = dto.TrangThai;

            _context.KyThiThangDais.Update(kyThi);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Cập nhật kỳ thi thăng đai thành công"
            });
        }

        // DELETE: api/admin/kythithangdai/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var kyThi = await _context.KyThiThangDais.FindAsync(id);
            if (kyThi == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy kỳ thi" });

            _context.KyThiThangDais.Remove(kyThi);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Xóa kỳ thi thăng đai thành công"
            });
        }

        // GET: api/admin/kythithangdai/{id}/ketqua
        [HttpGet("{id}/ketqua")]
        public async Task<IActionResult> GetKetQuaByKyThi(int id)
        {
            var list = await _context.KetQuaThis
                .Include(k => k.MaHocVienNavigation)
                    .ThenInclude(hv => hv.MaTaiKhoanNavigation)
                .Include(k => k.MaCapDaiMoiNavigation)
                .Where(k => k.MaKyThi == id)
                .Select(k => new AdminKetQuaThiDto
                {
                    MaKetQua = k.MaKetQua,
                    MaKyThi = k.MaKyThi,
                    MaHocVien = k.MaHocVien,
                    TenHocVien = k.MaHocVienNavigation.MaTaiKhoanNavigation != null 
                        ? k.MaHocVienNavigation.MaTaiKhoanNavigation.HoTen : null,
                    DiemSo = k.DiemSo,
                    DaDat = k.DaDat,
                    MaCapDaiMoi = k.MaCapDaiMoi,
                    TenCapDaiMoi = k.MaCapDaiMoiNavigation != null ? k.MaCapDaiMoiNavigation.TenCapDai : null
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<AdminKetQuaThiDto>>
            {
                Success = true,
                Message = "Lấy danh sách kết quả thi thành công",
                Data = list
            });
        }

        // POST: api/admin/kythithangdai/{id}/ketqua
        [HttpPost("{id}/ketqua")]
        public async Task<IActionResult> CreateKetQua(int id, [FromBody] CreateAdminKetQuaThiDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });

            var kyThiExists = await _context.KyThiThangDais.AnyAsync(k => k.MaKyThi == id);
            if (!kyThiExists)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Kỳ thi không tồn tại" });

            var hocVienExists = await _context.HocViens.AnyAsync(hv => hv.MaHocVien == dto.MaHocVien);
            if (!hocVienExists)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Học viên không tồn tại" });

            var existingKetQua = await _context.KetQuaThis
                .FirstOrDefaultAsync(k => k.MaKyThi == id && k.MaHocVien == dto.MaHocVien);

            if (existingKetQua != null)
            {
                existingKetQua.DiemSo = dto.DiemSo;
                existingKetQua.DaDat = dto.DaDat;
                existingKetQua.MaCapDaiMoi = dto.MaCapDaiMoi;
                _context.KetQuaThis.Update(existingKetQua);
            }
            else
            {
                var newKetQua = new KetQuaThi
                {
                    MaKyThi = id,
                    MaHocVien = dto.MaHocVien,
                    DiemSo = dto.DiemSo,
                    DaDat = dto.DaDat,
                    MaCapDaiMoi = dto.MaCapDaiMoi
                };
                _context.KetQuaThis.Add(newKetQua);
            }

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Lưu kết quả thi thành công"
            });
        }

        // GET: api/admin/kythithangdai/hocvien
        [HttpGet("hocvien")]
        public async Task<IActionResult> GetAllHocVien()
        {
            var list = await _context.HocViens
                .Include(hv => hv.MaTaiKhoanNavigation)
                .Include(hv => hv.MaCapDaiHienTaiNavigation)
                .Select(hv => new AdminHocVienSimpleDto
                {
                    MaHocVien = hv.MaHocVien,
                    HoTen = hv.MaTaiKhoanNavigation != null ? hv.MaTaiKhoanNavigation.HoTen : null,
                    MaCapDaiHienTai = hv.MaCapDaiHienTai,
                    TenCapDaiHienTai = hv.MaCapDaiHienTaiNavigation != null ? hv.MaCapDaiHienTaiNavigation.TenCapDai : null
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<AdminHocVienSimpleDto>>
            {
                Success = true,
                Message = "Lấy danh sách học viên thành công",
                Data = list
            });
        }
    }
}
