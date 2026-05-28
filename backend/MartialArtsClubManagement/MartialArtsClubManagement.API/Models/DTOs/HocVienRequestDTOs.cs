using System.ComponentModel.DataAnnotations;

namespace MartialArtsClubManagement.API.Models.DTOs
{
    public class DangKyLopRequestDTO
    {
        [Required(ErrorMessage = "Vui lòng chọn lớp học")]
        public int MaLop { get; set; }
    }

    public class DangKyKemRequestDTO
    {
        [Required(ErrorMessage = "Vui lòng chọn gói kèm riêng")]
        public int MaGoi { get; set; }
        public int? MaHlv { get; set; } // Có thể null nếu học viên để trung tâm tự phân công
    }

    public class DangKySuKienRequestDTO
    {
        [Required(ErrorMessage = "Vui lòng chọn sự kiện")]
        public int MaThongBao { get; set; }
    }

    public class ThanhToanRequestDTO
    {
        [Required(ErrorMessage = "Vui lòng chọn loại thanh toán")]
        public string LoaiThanhToan { get; set; } = null!; // "LopHoc" hoặc "KemRieng"

        [Required(ErrorMessage = "Vui lòng chọn mã đăng ký")]
        public int MaDangKy { get; set; }

        public string? PhuongThucThanhToan { get; set; } // MoMo, VNPay, Bank
    }
}
