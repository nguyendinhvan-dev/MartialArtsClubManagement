using Microsoft.AspNetCore.Authorization;
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
    [Route("api/trainer")]
    [ApiController]
    [Authorize(Roles = "HuanLuyenVien")]
    public class TrainerController : ControllerBase
    {
        private readonly QuanLyCLBVoThuatContext _context;

        public TrainerController(QuanLyCLBVoThuatContext context)
        {
            _context = context;
        }

        // GET: api/trainer/dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            // TODO: Get current trainer ID from JWT token
            // For now, using a hardcoded ID for demo
            int currentHlvId = 1;

            // Get classes taught by this trainer
            var soLopDangDay = await _context.LopHocs
                .Where(l => l.MaHlv == currentHlvId)
                .CountAsync();

            // Get total students in trainer's classes
            var lopIds = await _context.LopHocs
                .Where(l => l.MaHlv == currentHlvId)
                .Select(l => l.MaLop)
                .ToListAsync();

            var tongHocVien = await _context.DangKyLops
                .Where(dk => lopIds.Contains(dk.MaLop))
                .Select(dk => dk.MaHocVien)
                .Distinct()
                .CountAsync();

            // Calculate attendance rate
            var totalAttendance = await _context.DiemDanhs
                .Join(_context.DangKyLops,
                    dd => dd.MaDangKy,
                    dk => dk.MaDangKy,
                    (dd, dk) => new { dd, dk })
                .Where(x => lopIds.Contains(x.dk.MaLop))
                .CountAsync();

            var presentAttendance = await _context.DiemDanhs
                .Join(_context.DangKyLops,
                    dd => dd.MaDangKy,
                    dk => dk.MaDangKy,
                    (dd, dk) => new { dd, dk })
                .Where(x => lopIds.Contains(x.dk.MaLop) && x.dd.TrangThai == "CóMặt")
                .CountAsync();

            double tyLeChuyenCan = totalAttendance > 0 ? (double)presentAttendance / totalAttendance * 100 : 0;

            // Get upcoming exams
            var kyThiSapToi = await _context.KyThiThangDais
                .Where(k => k.TrangThai == "SapDienRa")
                .CountAsync();

            return Ok(new ApiResponse<TrainerDashboardDto>
            {
                Success = true,
                Message = "Lấy thống kê dashboard thành công",
                Data = new TrainerDashboardDto
                {
                    SoLopDangDay = soLopDangDay,
                    TongHocVien = tongHocVien,
                    TyLeChuyenCan = Math.Round(tyLeChuyenCan, 2),
                    KyThiSapToi = kyThiSapToi
                }
            });
        }

        // GET: api/trainer/classes
        [HttpGet("classes")]
        public async Task<IActionResult> GetClasses()
        {
            // TODO: Get current trainer ID from JWT token
            int currentHlvId = 1;

            var lops = await _context.LopHocs
                .Where(l => l.MaHlv == currentHlvId)
                .Include(l => l.MaKhoaHocNavigation)
                .Include(l => l.MaCapDaiNavigation)
                .Select(l => new TrainerLopHocDto
                {
                    MaLop = l.MaLop,
                    MaKhoaHoc = l.MaKhoaHoc,
                    TenKhoaHoc = l.MaKhoaHocNavigation != null ? l.MaKhoaHocNavigation.TenKhoaHoc : null,
                    MaCapDai = l.MaCapDai,
                    TenCapDai = l.MaCapDaiNavigation != null ? l.MaCapDaiNavigation.TenCapDai : null,
                    LichHoc = l.LichHoc,
                    PhongTap = l.PhongTap,
                    SoLuongToiDa = l.SoLuongToiDa,
                    SoHocVienHienTai = _context.DangKyLops.Count(dk => dk.MaLop == l.MaLop)
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<TrainerLopHocDto>>
            {
                Success = true,
                Message = "Lấy danh sách lớp học thành công",
                Data = lops
            });
        }

        // GET: api/trainer/classes/{id}
        [HttpGet("classes/{id}")]
        public async Task<IActionResult> GetClassById(int id)
        {
            // TODO: Get current trainer ID from JWT token
            int currentHlvId = 1;

            var l = await _context.LopHocs
                .Include(l => l.MaKhoaHocNavigation)
                .Include(l => l.MaCapDaiNavigation)
                .FirstOrDefaultAsync(l => l.MaLop == id && l.MaHlv == currentHlvId);

            if (l == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Không tìm thấy lớp học"
                });
            }

            var dto = new TrainerLopHocDto
            {
                MaLop = l.MaLop,
                MaKhoaHoc = l.MaKhoaHoc,
                TenKhoaHoc = l.MaKhoaHocNavigation?.TenKhoaHoc,
                MaCapDai = l.MaCapDai,
                TenCapDai = l.MaCapDaiNavigation?.TenCapDai,
                LichHoc = l.LichHoc,
                PhongTap = l.PhongTap,
                SoLuongToiDa = l.SoLuongToiDa,
                SoHocVienHienTai = _context.DangKyLops.Count(dk => dk.MaLop == l.MaLop)
            };

            return Ok(new ApiResponse<TrainerLopHocDto>
            {
                Success = true,
                Message = "Lấy thông tin lớp học thành công",
                Data = dto
            });
        }

        // POST: api/trainer/classes
        [HttpPost("classes")]
        public async Task<IActionResult> CreateClass([FromBody] CreateTrainerLopHocDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            // TODO: Get current trainer ID from JWT token
            int currentHlvId = 1;

            var khoaHocExists = await _context.KhoaHocs.AnyAsync(k => k.MaKhoaHoc == dto.MaKhoaHoc);
            if (!khoaHocExists)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Khóa học không tồn tại" });
            }

            var newLopHoc = new LopHoc
            {
                MaKhoaHoc = dto.MaKhoaHoc,
                MaCapDai = dto.MaCapDai,
                MaHlv = currentHlvId,
                LichHoc = dto.LichHoc,
                SoLuongToiDa = dto.SoLuongToiDa,
                PhongTap = dto.PhongTap
            };

            _context.LopHocs.Add(newLopHoc);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Thêm lớp học thành công",
                Data = newLopHoc.MaLop
            });
        }

        // PUT: api/trainer/classes/{id}
        [HttpPut("classes/{id}")]
        public async Task<IActionResult> UpdateClass(int id, [FromBody] UpdateTrainerLopHocDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            // TODO: Get current trainer ID from JWT token
            int currentHlvId = 1;

            var lopHoc = await _context.LopHocs
                .FirstOrDefaultAsync(l => l.MaLop == id && l.MaHlv == currentHlvId);

            if (lopHoc == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy lớp học" });
            }

            lopHoc.MaKhoaHoc = dto.MaKhoaHoc;
            lopHoc.MaCapDai = dto.MaCapDai;
            lopHoc.LichHoc = dto.LichHoc;
            lopHoc.SoLuongToiDa = dto.SoLuongToiDa;
            lopHoc.PhongTap = dto.PhongTap;

            _context.LopHocs.Update(lopHoc);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Cập nhật lớp học thành công"
            });
        }

        // DELETE: api/trainer/classes/{id}
        [HttpDelete("classes/{id}")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            // TODO: Get current trainer ID from JWT token
            int currentHlvId = 1;

            var lopHoc = await _context.LopHocs
                .FirstOrDefaultAsync(l => l.MaLop == id && l.MaHlv == currentHlvId);

            if (lopHoc == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy lớp học" });
            }

            _context.LopHocs.Remove(lopHoc);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Xóa lớp học thành công"
            });
        }

        // GET: api/trainer/students
        [HttpGet("students")]
        public async Task<IActionResult> GetStudents([FromQuery] int? maLop = null)
        {
            // TODO: Get current trainer ID from JWT token
            int currentHlvId = 1;

            var lopIds = await _context.LopHocs
                .Where(l => l.MaHlv == currentHlvId)
                .Select(l => l.MaLop)
                .ToListAsync();

            var query = _context.DangKyLops
                .Include(dk => dk.MaHocVienNavigation)
                    .ThenInclude(hv => hv.MaTaiKhoanNavigation)
                .Include(dk => dk.MaLopNavigation)
                .Where(dk => lopIds.Contains(dk.MaLop));

            if (maLop.HasValue)
            {
                query = query.Where(dk => dk.MaLop == maLop.Value);
            }

            var students = await query
                .Select(dk => new TrainerHocVienDto
                {
                    MaHocVien = dk.MaHocVien,
                    TenHocVien = dk.MaHocVienNavigation.MaTaiKhoanNavigation != null 
                        ? dk.MaHocVienNavigation.MaTaiKhoanNavigation.HoTen : null,
                    Email = dk.MaHocVienNavigation.MaTaiKhoanNavigation != null 
                        ? dk.MaHocVienNavigation.MaTaiKhoanNavigation.Email : null,
                    SoDienThoai = dk.MaHocVienNavigation.SoDienThoai,
                    MaCapDaiHienTai = dk.MaHocVienNavigation.MaCapDaiHienTai,
                    TenCapDai = dk.MaHocVienNavigation.MaCapDaiHienTaiNavigation != null 
                        ? dk.MaHocVienNavigation.MaCapDaiHienTaiNavigation.TenCapDai : null,
                    MaLop = dk.MaLop,
                    TenLop = dk.MaLopNavigation != null ? $"Lớp {dk.MaLop}" : null
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<TrainerHocVienDto>>
            {
                Success = true,
                Message = "Lấy danh sách học viên thành công",
                Data = students
            });
        }

        // GET: api/trainer/students/{id}
        [HttpGet("students/{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            // TODO: Get current trainer ID from JWT token
            int currentHlvId = 1;

            var lopIds = await _context.LopHocs
                .Where(l => l.MaHlv == currentHlvId)
                .Select(l => l.MaLop)
                .ToListAsync();

            var student = await _context.DangKyLops
                .Include(dk => dk.MaHocVienNavigation)
                    .ThenInclude(hv => hv.MaTaiKhoanNavigation)
                .Include(dk => dk.MaLopNavigation)
                .Include(dk => dk.MaHocVienNavigation.MaCapDaiHienTaiNavigation)
                .Where(dk => dk.MaHocVien == id && lopIds.Contains(dk.MaLop))
                .FirstOrDefaultAsync();

            if (student == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Không tìm thấy học viên"
                });
            }

            var dto = new TrainerHocVienDto
            {
                MaHocVien = student.MaHocVien,
                TenHocVien = student.MaHocVienNavigation.MaTaiKhoanNavigation?.HoTen,
                Email = student.MaHocVienNavigation.MaTaiKhoanNavigation?.Email,
                SoDienThoai = student.MaHocVienNavigation.SoDienThoai,
                MaCapDaiHienTai = student.MaHocVienNavigation.MaCapDaiHienTai,
                TenCapDai = student.MaHocVienNavigation.MaCapDaiHienTaiNavigation?.TenCapDai,
                MaLop = student.MaLop,
                TenLop = student.MaLopNavigation != null ? $"Lớp {student.MaLop}" : null
            };

            return Ok(new ApiResponse<TrainerHocVienDto>
            {
                Success = true,
                Message = "Lấy thông tin học viên thành công",
                Data = dto
            });
        }

        // GET: api/trainer/attendance
        [HttpGet("attendance")]
        public async Task<IActionResult> GetAttendance([FromQuery] int? maLop = null, [FromQuery] DateOnly? ngayHoc = null)
        {
            // TODO: Get current trainer ID from JWT token
            int currentHlvId = 1;

            var lopIds = await _context.LopHocs
                .Where(l => l.MaHlv == currentHlvId)
                .Select(l => l.MaLop)
                .ToListAsync();

            var query = _context.DiemDanhs
                .Include(dd => dd.MaDangKyNavigation)
                    .ThenInclude(d => d.MaHocVienNavigation)
                        .ThenInclude(hv => hv.MaTaiKhoanNavigation)
                .Where(dd => lopIds.Contains(dd.MaDangKyNavigation.MaLop));

            if (maLop.HasValue)
            {
                query = query.Where(dd => dd.MaDangKyNavigation.MaLop == maLop.Value);
            }

            if (ngayHoc.HasValue)
            {
                query = query.Where(dd => dd.NgayHoc == ngayHoc.Value);
            }

            var attendance = await query
                .Select(dd => new TrainerDiemDanhDto
                {
                    MaDiemDanh = dd.MaDiemDanh,
                    MaDangKy = dd.MaDangKy,
                    TenHocVien = dd.MaDangKyNavigation.MaHocVienNavigation.MaTaiKhoanNavigation != null 
                        ? dd.MaDangKyNavigation.MaHocVienNavigation.MaTaiKhoanNavigation.HoTen : null,
                    NgayHoc = dd.NgayHoc,
                    TrangThai = dd.TrangThai,
                    GhiChu = dd.GhiChu
                })
                .OrderByDescending(dd => dd.NgayHoc)
                .ToListAsync();

            return Ok(new ApiResponse<List<TrainerDiemDanhDto>>
            {
                Success = true,
                Message = "Lấy danh sách điểm danh thành công",
                Data = attendance
            });
        }

        // POST: api/trainer/attendance/bulk
        [HttpPost("attendance/bulk")]
        public async Task<IActionResult> CreateBulkAttendance([FromBody] BulkTrainerDiemDanhDto dto)
        {
            if (dto == null || dto.Attendances == null || !dto.Attendances.Any())
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Danh sách điểm danh trống" });
            }

            // TODO: Get current trainer ID from JWT token
            int currentHlvId = 1;

            var lopIds = await _context.LopHocs
                .Where(l => l.MaHlv == currentHlvId)
                .Select(l => l.MaLop)
                .ToListAsync();

            var createdCount = 0;
            var updatedCount = 0;

            foreach (var item in dto.Attendances)
            {
                // Verify the registration belongs to trainer's class
                var dangKy = await _context.DangKyLops
                    .FirstOrDefaultAsync(d => d.MaDangKy == item.MaDangKy && lopIds.Contains(d.MaLop));
                
                if (dangKy == null) continue;

                var isDuplicated = await _context.DiemDanhs
                    .AnyAsync(dd => dd.MaDangKy == item.MaDangKy && dd.NgayHoc == dto.NgayHoc);
                
                if (isDuplicated)
                {
                    // Update existing
                    var existing = await _context.DiemDanhs
                        .FirstOrDefaultAsync(dd => dd.MaDangKy == item.MaDangKy && dd.NgayHoc == dto.NgayHoc);
                    if (existing != null)
                    {
                        existing.TrangThai = item.TrangThai;
                        existing.GhiChu = item.GhiChu;
                        _context.DiemDanhs.Update(existing);
                        updatedCount++;
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
                Message = $"Điểm danh hàng loạt hoàn tất. Đã thêm mới: {createdCount}, đã cập nhật: {updatedCount}"
            });
        }

        // GET: api/trainer/exams
        [HttpGet("exams")]
        public async Task<IActionResult> GetExams()
        {
            // TODO: Get current trainer ID from JWT token
            int currentHlvId = 1;

            var lopIds = await _context.LopHocs
                .Where(l => l.MaHlv == currentHlvId)
                .Select(l => l.MaLop)
                .ToListAsync();

            var khoaHocIds = await _context.LopHocs
                .Where(l => l.MaHlv == currentHlvId)
                .Select(l => l.MaKhoaHoc)
                .Distinct()
                .ToListAsync();

            var exams = await _context.KyThiThangDais
                .Include(k => k.MaKhoaHocNavigation)
                .Where(k => khoaHocIds.Contains(k.MaKhoaHoc))
                .Select(k => new TrainerKyThiDto
                {
                    MaKyThi = k.MaKyThi,
                    TenKyThi = k.TenKyThi,
                    NgayThi = k.NgayThi,
                    MaKhoaHoc = k.MaKhoaHoc,
                    TenKhoaHoc = k.MaKhoaHocNavigation != null ? k.MaKhoaHocNavigation.TenKhoaHoc : null,
                    TrangThai = k.TrangThai,
                    MoTa = k.MoTa
                })
                .OrderByDescending(k => k.NgayThi)
                .ToListAsync();

            return Ok(new ApiResponse<List<TrainerKyThiDto>>
            {
                Success = true,
                Message = "Lấy danh sách kỳ thi thành công",
                Data = exams
            });
        }

        // POST: api/trainer/exams
        [HttpPost("exams")]
        public async Task<IActionResult> CreateExam([FromBody] CreateTrainerKyThiDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            // TODO: Get current trainer ID from JWT token
            int currentHlvId = 1;

            // Verify the course belongs to trainer's classes
            var khoaHocIds = await _context.LopHocs
                .Where(l => l.MaHlv == currentHlvId)
                .Select(l => l.MaKhoaHoc)
                .Distinct()
                .ToListAsync();

            if (!khoaHocIds.Contains(dto.MaKhoaHoc))
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Khóa học không thuộc quyền quản lý" });
            }

            var newKyThi = new KyThiThangDai
            {
                TenKyThi = dto.TenKyThi,
                NgayThi = dto.NgayThi,
                MaKhoaHoc = dto.MaKhoaHoc,
                TrangThai = "SapDienRa",
                MoTa = dto.MoTa
            };

            _context.KyThiThangDais.Add(newKyThi);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Tạo kỳ thi thành công",
                Data = newKyThi.MaKyThi
            });
        }

        // PUT: api/trainer/exams/{id}
        [HttpPut("exams/{id}")]
        public async Task<IActionResult> UpdateExam(int id, [FromBody] UpdateTrainerKyThiDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            // TODO: Get current trainer ID from JWT token
            int currentHlvId = 1;

            var khoaHocIds = await _context.LopHocs
                .Where(l => l.MaHlv == currentHlvId)
                .Select(l => l.MaKhoaHoc)
                .Distinct()
                .ToListAsync();

            var kyThi = await _context.KyThiThangDais
                .FirstOrDefaultAsync(k => k.MaKyThi == id && khoaHocIds.Contains(k.MaKhoaHoc));

            if (kyThi == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy kỳ thi" });
            }

            kyThi.TenKyThi = dto.TenKyThi;
            kyThi.NgayThi = dto.NgayThi;
            kyThi.MaKhoaHoc = dto.MaKhoaHoc;
            kyThi.TrangThai = dto.TrangThai;
            kyThi.MoTa = dto.MoTa;

            _context.KyThiThangDais.Update(kyThi);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Cập nhật kỳ thi thành công"
            });
        }

        // DELETE: api/trainer/exams/{id}
        [HttpDelete("exams/{id}")]
        public async Task<IActionResult> DeleteExam(int id)
        {
            // TODO: Get current trainer ID from JWT token
            int currentHlvId = 1;

            var khoaHocIds = await _context.LopHocs
                .Where(l => l.MaHlv == currentHlvId)
                .Select(l => l.MaKhoaHoc)
                .Distinct()
                .ToListAsync();

            var kyThi = await _context.KyThiThangDais
                .FirstOrDefaultAsync(k => k.MaKyThi == id && khoaHocIds.Contains(k.MaKhoaHoc));

            if (kyThi == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy kỳ thi" });
            }

            _context.KyThiThangDais.Remove(kyThi);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Xóa kỳ thi thành công"
            });
        }

        // POST: api/trainer/exam-results
        [HttpPost("exam-results")]
        public async Task<IActionResult> CreateExamResults([FromBody] BulkTrainerKetQuaThiDto dto)
        {
            if (dto == null || dto.Results == null || !dto.Results.Any())
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Danh sách kết quả trống" });
            }

            // TODO: Get current trainer ID from JWT token
            int currentHlvId = 1;

            var khoaHocIds = await _context.LopHocs
                .Where(l => l.MaHlv == currentHlvId)
                .Select(l => l.MaKhoaHoc)
                .Distinct()
                .ToListAsync();

            // Verify exam belongs to trainer's courses
            var kyThi = await _context.KyThiThangDais
                .FirstOrDefaultAsync(k => k.MaKyThi == dto.MaKyThi && khoaHocIds.Contains(k.MaKhoaHoc));

            if (kyThi == null)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Kỳ thi không thuộc quyền quản lý" });
            }

            var createdCount = 0;
            var updatedCount = 0;

            foreach (var item in dto.Results)
            {
                var isDuplicated = await _context.KetQuaThis
                    .AnyAsync(kq => kq.MaKyThi == dto.MaKyThi && kq.MaHocVien == item.MaHocVien);
                
                if (isDuplicated)
                {
                    // Update existing
                    var existing = await _context.KetQuaThis
                        .FirstOrDefaultAsync(kq => kq.MaKyThi == dto.MaKyThi && kq.MaHocVien == item.MaHocVien);
                    if (existing != null)
                    {
                        existing.MaCapDaiMoi = item.MaCapDaiMoi;
                        existing.DiemSo = item.DiemSo ?? 0;
                        _context.KetQuaThis.Update(existing);
                        updatedCount++;
                    }
                    continue;
                }

                var newKetQua = new KetQuaThi
                {
                    MaKyThi = dto.MaKyThi,
                    MaHocVien = item.MaHocVien,
                    MaCapDaiMoi = item.MaCapDaiMoi,
                    DiemSo = item.DiemSo ?? 0
                };

                _context.KetQuaThis.Add(newKetQua);
                createdCount++;
            }

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = $"Nhập kết quả thi hoàn tất. Đã thêm mới: {createdCount}, đã cập nhật: {updatedCount}"
            });
        }

        // GET: api/trainer/events
        [HttpGet("events")]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.ThongBaos
                .Include(t => t.MaTaiKhoanTaoNavigation)
                .Where(t => t.LoaiThongBao == "SuKien" || t.LoaiThongBao == "ThongBao")
                .OrderByDescending(d => d.NgayDang)
                .Select(t => new TrainerThongBaoDto
                {
                    MaThongBao = t.MaThongBao,
                    TieuDe = t.TieuDe,
                    NoiDung = t.NoiDung,
                    NgayDang = DateOnly.FromDateTime(t.NgayDang),
                    LoaiThongBao = t.LoaiThongBao
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<TrainerThongBaoDto>>
            {
                Success = true,
                Message = "Lấy danh sách sự kiện thành công",
                Data = events
            });
        }

        // POST: api/trainer/events
        [HttpPost("events")]
        public async Task<IActionResult> CreateEvent([FromBody] CreateTrainerThongBaoDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            // TODO: Get current trainer ID from JWT token
            int currentHlvId = 1;

            var taiKhoan = await _context.HuanLuyenViens
                .Include(h => h.MaTaiKhoanNavigation)
                .FirstOrDefaultAsync(h => h.MaHlv == currentHlvId);

            if (taiKhoan == null)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Không tìm thấy tài khoản HLV" });
            }

            var newThongBao = new ThongBao
            {
                TieuDe = dto.TieuDe,
                NoiDung = dto.NoiDung,
                NgayDang = DateTime.Now,
                LoaiThongBao = dto.LoaiThongBao,
                MaTaiKhoanTao = taiKhoan.MaTaiKhoan
            };

            _context.ThongBaos.Add(newThongBao);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Tạo sự kiện thành công",
                Data = newThongBao.MaThongBao
            });
        }

        // GET: api/trainer/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            // TODO: Get current trainer ID from JWT token
            int currentHlvId = 1;

            var hlv = await _context.HuanLuyenViens
                .Include(h => h.MaTaiKhoanNavigation)
                .FirstOrDefaultAsync(h => h.MaHlv == currentHlvId);

            if (hlv == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Không tìm thấy thông tin HLV"
                });
            }

            var dto = new TrainerProfileDto
            {
                MaHlv = hlv.MaHlv,
                HoTen = hlv.MaTaiKhoanNavigation?.HoTen,
                Email = hlv.MaTaiKhoanNavigation?.Email,
                SoDienThoai = hlv.SoDienThoai,
                ChuyenMon = hlv.ChuyenMon,
                NgayVaoClb = hlv.NgayVaoClb,
                DangHoatDong = hlv.DangHoatDong
            };

            return Ok(new ApiResponse<TrainerProfileDto>
            {
                Success = true,
                Message = "Lấy hồ sơ HLV thành công",
                Data = dto
            });
        }

        // PUT: api/trainer/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateTrainerProfileDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Dữ liệu không hợp lệ" });
            }

            // TODO: Get current trainer ID from JWT token
            int currentHlvId = 1;

            var hlv = await _context.HuanLuyenViens
                .FirstOrDefaultAsync(h => h.MaHlv == currentHlvId);

            if (hlv == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Không tìm thấy thông tin HLV" });
            }

            hlv.SoDienThoai = dto.SoDienThoai;
            hlv.ChuyenMon = dto.ChuyenMon;

            _context.HuanLuyenViens.Update(hlv);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Cập nhật hồ sơ HLV thành công"
            });
        }
    }
}
