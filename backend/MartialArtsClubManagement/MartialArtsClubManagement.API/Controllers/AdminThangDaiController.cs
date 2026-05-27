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
    [Route("api/admin/thangdai")]
    [ApiController]
    // [Authorize(Roles = "Admin")]
    public class AdminThangDaiController : ControllerBase
    {
        private readonly QuanLyCLBVoThuatContext _context;

        public AdminThangDaiController(QuanLyCLBVoThuatContext context)
        {
            _context = context;
        }

        // ==================== QUẢN LÝ KỲ THI ====================

        // GET: api/admin/thangdai/kythi
        [HttpGet("kythi")]
        public async Task<IActionResult> GetAllKyThi()
        {
            var list = await _context.KyThiThangDais
                .Include(k => k.MaKhoaHocNavigation)
                .Select(k => new AdminKyThiThangDaiDto
                {
                    MaKyThi = k.MaKyThi,
                    MaKhoaHoc = k.MaKhoaHoc,
                    TenKhoaHoc = k.MaKhoaHocNavigation != null ? k.MaKhoaHocNavigation.TenKhoaHoc : null,
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

        // GET: api/admin/thangdai/kythi/{id}
        [HttpGet("kythi/{id}")]
        public async Task<IActionResult> GetKyThiById(int id)
        {
            var k = await _context.KyThiThangDais
                .Include(k => k.MaKhoaHocNavigation)
                .FirstOrDefaultAsync(k => k.MaKyThi == id);

            if (k == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy kỳ thi" });
            }

            var dto = new AdminKyThiThangDaiDto
            {
                MaKyThi = k.MaKyThi,
                MaKhoaHoc = k.MaKhoaHoc,
                TenKhoaHoc = k.MaKhoaHocNavigation?.TenKhoaHoc,
                NgayThi = k.NgayThi,
                MoTa = k.MoTa,
                TrangThai = k.TrangThai
            };

            return Ok(new ApiResponse<AdminKyThiThangDaiDto>
            {
                Success = true,
                Message = "Lấy thông tin kỳ thi thành công",
                Data = dto
            });
        }

        // POST: api/admin/thangdai/kythi
        [HttpPost("kythi")]
        public async Task<IActionResult> CreateKyThi([FromBody] CreateAdminKyThiThangDaiDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            var khoaHocExists = await _context.KhoaHocs.AnyAsync(kh => kh.MaKhoaHoc == dto.MaKhoaHoc);
            if (!khoaHocExists)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Khóa học không tồn tại" });
            }

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

        // PUT: api/admin/thangdai/kythi/{id}
        [HttpPut("kythi/{id}")]
        public async Task<IActionResult> UpdateKyThi(int id, [FromBody] UpdateAdminKyThiThangDaiDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            var k = await _context.KyThiThangDais.FindAsync(id);
            if (k == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy kỳ thi" });
            }

            var khoaHocExists = await _context.KhoaHocs.AnyAsync(kh => kh.MaKhoaHoc == dto.MaKhoaHoc);
            if (!khoaHocExists)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Khóa học không tồn tại" });
            }

            k.MaKhoaHoc = dto.MaKhoaHoc;
            k.NgayThi = dto.NgayThi;
            k.MoTa = dto.MoTa;
            k.TrangThai = dto.TrangThai;

            _context.KyThiThangDais.Update(k);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Cập nhật kỳ thi thăng đai thành công"
            });
        }

        // DELETE: api/admin/thangdai/kythi/{id}
        [HttpDelete("kythi/{id}")]
        public async Task<IActionResult> DeleteKyThi(int id)
        {
            var k = await _context.KyThiThangDais.FindAsync(id);
            if (k == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy kỳ thi" });
            }

            // Kiểm tra xem kỳ thi đã có kết quả thi chưa
            var hasResults = await _context.KetQuaThis.AnyAsync(kq => kq.MaKyThi == id);
            if (hasResults)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Không thể xóa kỳ thi đã có kết quả thi của học viên" });
            }

            _context.KyThiThangDais.Remove(k);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Xóa kỳ thi thăng đai thành công"
            });
        }

        // ==================== QUẢN LÝ KẾT QUẢ THI & DUYỆT ĐAI ====================

        // GET: api/admin/thangdai/kythi/{maKyThi}/ketqua
        [HttpGet("kythi/{maKyThi}/ketqua")]
        public async Task<IActionResult> GetKetQuaThi(int maKyThi)
        {
            var kyThiExists = await _context.KyThiThangDais.AnyAsync(k => k.MaKyThi == maKyThi);
            if (!kyThiExists)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy kỳ thi" });
            }

            var list = await _context.KetQuaThis
                .Where(kq => kq.MaKyThi == maKyThi)
                .Include(kq => kq.MaHocVienNavigation).ThenInclude(hv => hv.MaTaiKhoanNavigation)
                .Include(kq => kq.MaCapDaiMoiNavigation)
                .Select(kq => new AdminKetQuaThiDto
                {
                    MaKetQua = kq.MaKetQua,
                    MaKyThi = kq.MaKyThi,
                    MaHocVien = kq.MaHocVien,
                    TenHocVien = kq.MaHocVienNavigation.MaTaiKhoanNavigation != null 
                        ? kq.MaHocVienNavigation.MaTaiKhoanNavigation.HoTen : null,
                    DiemSo = kq.DiemSo,
                    DaDat = kq.DaDat,
                    MaCapDaiMoi = kq.MaCapDaiMoi,
                    TenCapDaiMoi = kq.MaCapDaiMoiNavigation != null ? kq.MaCapDaiMoiNavigation.TenCapDai : null
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<AdminKetQuaThiDto>>
            {
                Success = true,
                Message = "Lấy danh sách kết quả thi thành công",
                Data = list
            });
        }

        // POST: api/admin/thangdai/kythi/{maKyThi}/ketqua
        [HttpPost("kythi/{maKyThi}/ketqua")]
        public async Task<IActionResult> AddKetQuaThi(int maKyThi, [FromBody] CreateAdminKetQuaThiDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            var kyThiExists = await _context.KyThiThangDais.AnyAsync(k => k.MaKyThi == maKyThi);
            if (!kyThiExists)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy kỳ thi" });
            }

            var hocVien = await _context.HocViens.FindAsync(dto.MaHocVien);
            if (hocVien == null)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Học viên không tồn tại" });
            }

            // Kiểm tra kết quả thi trùng
            var isDuplicated = await _context.KetQuaThis.AnyAsync(kq => kq.MaKyThi == maKyThi && kq.MaHocVien == dto.MaHocVien);
            if (isDuplicated)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Học viên đã có kết quả thi cho kỳ thi này" });
            }

            var newKetQua = new KetQuaThi
            {
                MaKyThi = maKyThi,
                MaHocVien = dto.MaHocVien,
                DiemSo = dto.DiemSo,
                DaDat = dto.DaDat,
                MaCapDaiMoi = dto.MaCapDaiMoi
            };

            // Nếu học viên ĐẠT kỳ thi và có chọn cấp đai mới, tự động thăng đai cho học viên
            if (dto.DaDat && dto.MaCapDaiMoi.HasValue)
            {
                hocVien.MaCapDaiHienTai = dto.MaCapDaiMoi.Value;
                _context.HocViens.Update(hocVien);
            }

            _context.KetQuaThis.Add(newKetQua);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Thêm kết quả thi thành công" + (dto.DaDat && dto.MaCapDaiMoi.HasValue ? " và đã cập nhật cấp đai mới cho học viên" : ""),
                Data = newKetQua.MaKetQua
            });
        }

        // PUT: api/admin/thangdai/ketqua/{id}
        [HttpPut("ketqua/{id}")]
        public async Task<IActionResult> UpdateKetQuaThi(int id, [FromBody] UpdateAdminKetQuaThiDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            var kq = await _context.KetQuaThis.FindAsync(id);
            if (kq == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy kết quả thi" });
            }

            // Trạng thái cũ
            var oldDaDat = kq.DaDat;

            kq.DiemSo = dto.DiemSo;
            kq.DaDat = dto.DaDat;
            kq.MaCapDaiMoi = dto.MaCapDaiMoi;

            _context.KetQuaThis.Update(kq);

            // Nếu từ Chưa Đạt sang Đạt và có cấp đai mới
            if (!oldDaDat && dto.DaDat && dto.MaCapDaiMoi.HasValue)
            {
                var hocVien = await _context.HocViens.FindAsync(kq.MaHocVien);
                if (hocVien != null)
                {
                    hocVien.MaCapDaiHienTai = dto.MaCapDaiMoi.Value;
                    _context.HocViens.Update(hocVien);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Cập nhật kết quả thi thành công"
            });
        }

        // DELETE: api/admin/thangdai/ketqua/{id}
        [HttpDelete("ketqua/{id}")]
        public async Task<IActionResult> DeleteKetQuaThi(int id)
        {
            var kq = await _context.KetQuaThis.FindAsync(id);
            if (kq == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy kết quả thi" });
            }

            _context.KetQuaThis.Remove(kq);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Xóa bản ghi kết quả thi thành công"
            });
        }

        // GET: api/admin/thangdai/dudieukien
        [HttpGet("dudieukien")]
        public async Task<IActionResult> GetHocVienDuDieuKienThi()
        {
            var list = await _context.VHocVienDuDieuKienThis
                .Select(hv => new HocVienDuDieuKienThiDto
                {
                    MaHocVien = hv.MaHocVien,
                    HoTen = hv.HoTen,
                    CapDaiHienTai = hv.CapDaiHienTai,
                    ThuTuDaiHienTai = hv.ThuTuDaiHienTai,
                    SoThangHienTai = hv.SoThangHienTai,
                    ThoiGianToiThieuThang = hv.ThoiGianToiThieuThang
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<HocVienDuDieuKienThiDto>>
            {
                Success = true,
                Message = "Lấy danh sách học viên đủ điều kiện thi thăng đai thành công",
                Data = list
            });
        }
    }
}
