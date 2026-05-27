using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MartialArtsClubManagement.API.Models.Entities;
using MartialArtsClubManagement.API.Models.DTOs;

namespace MartialArtsClubManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HocVienController : ControllerBase
    {
        private readonly QuanLyCLBVoThuatContext _context;

        public HocVienController(QuanLyCLBVoThuatContext context)
        {
            _context = context;
        }

        // GET: api/HocVien
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<VHocVienVaCapDai>>>> GetHocViens()
        {
            try
            {
                // Truy vấn trực tiếp từ View nghiệp vụ V_HocVienVaCapDai
                var hocViens = await _context.VHocVienVaCapDais.ToListAsync();
                return Ok(ApiResponse<IEnumerable<VHocVienVaCapDai>>.SuccessResponse(hocViens, "Lấy danh sách học viên kèm cấp đai thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<VHocVienVaCapDai>>.ErrorResponse($"Lỗi máy chủ: {ex.Message}"));
            }
        }
    }
}
