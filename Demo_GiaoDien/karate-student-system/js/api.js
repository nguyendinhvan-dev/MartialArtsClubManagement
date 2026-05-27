/**
 * API Service cho Giao diện Học viên
 * Cung cấp các hàm mô phỏng (mock) quá trình gọi API đến Backend.
 * Sau khi có Backend thực tế, chỉ cần thay đổi nội dung các hàm này thành fetch/axios.
 */

const API_BASE_URL = 'http://localhost:8080/api'; // Thay đổi URL này khi có backend

// --- MOCK DATA ---
const mockUser = {
    id: "SV001",
    name: "Nguyễn Văn A",
    email: "nguyenvana@gmail.com",
    phone: "0123456789",
    belt: "Đai Trắng",
    attendance: [
        { date: "2023-10-01", status: "present" },
        { date: "2023-10-03", status: "late" },
        { date: "2023-10-05", status: "absent" }
    ],
    exams: [
        { date: "2023-09-15", result: "Đạt", belt_achieved: "Đai Trắng" }
    ],
    tuition: [
        { month: "10/2023", amount: "500,000đ", status: "paid" },
        { month: "11/2023", amount: "500,000đ", status: "unpaid" }
    ],
    schedule: [
        { day: "Thứ 3", time: "18:00 - 19:30", location: "Sân tập A" },
        { day: "Thứ 5", time: "18:00 - 19:30", location: "Sân tập A" }
    ],
    notifications: [
        { id: 1, title: "Nghỉ tập ngày 20/11", content: "Chào mừng ngày nhà giáo VN, CLB nghỉ tập.", date: "2023-11-18" },
        { id: 2, title: "Đóng học phí tháng 11", content: "Yêu cầu hoàn thành trước 10/11.", date: "2023-11-01" }
    ]
};

// Hàm tiện ích để mô phỏng độ trễ mạng (Network delay)
const delay = (ms) => new Promise(resolve => setTimeout(resolve, ms));

const AuthService = {
    login: async (username, password) => {
        // TODO: Thay bằng fetch API thực tế
        await delay(1000); 
        if (username === 'admin' && password === '123') {
            localStorage.setItem('studentUser', JSON.stringify(mockUser));
            return { success: true, data: mockUser, token: "fake-jwt-token" };
        }
        return { success: false, message: 'Sai tài khoản hoặc mật khẩu' };
    },
    logout: () => {
        localStorage.removeItem('studentUser');
    },
    getCurrentUser: () => {
        return JSON.parse(localStorage.getItem('studentUser'));
    }
};

const StudentService = {
    getProfile: async () => {
        await delay(500);
        return { success: true, data: AuthService.getCurrentUser() };
    },
    updateProfile: async (profileData) => {
        await delay(1000);
        let user = AuthService.getCurrentUser();
        if(user) {
            user = { ...user, ...profileData };
            localStorage.setItem('studentUser', JSON.stringify(user));
            return { success: true, data: user, message: "Cập nhật thành công!" };
        }
        return { success: false, message: "Không tìm thấy người dùng" };
    }
};

const AttendanceService = {
    getAttendanceSummary: async () => {
        await delay(500);
        const user = AuthService.getCurrentUser();
        return { success: true, data: user ? user.attendance : [] };
    }
};

const ExamService = {
    getExamResults: async () => {
        await delay(500);
        const user = AuthService.getCurrentUser();
        return { success: true, data: user ? user.exams : [] };
    }
};

const TuitionService = {
    getTuitionHistory: async () => {
        await delay(500);
        const user = AuthService.getCurrentUser();
        return { success: true, data: user ? user.tuition : [] };
    }
};

const ScheduleService = {
    getSchedule: async () => {
        await delay(500);
        const user = AuthService.getCurrentUser();
        return { success: true, data: user ? user.schedule : [] };
    }
};

const NotificationService = {
    getNotifications: async () => {
        await delay(500);
        const user = AuthService.getCurrentUser();
        return { success: true, data: user ? user.notifications : [] };
    }
};

window.API = {
    AuthService,
    StudentService,
    AttendanceService,
    ExamService,
    TuitionService,
    ScheduleService,
    NotificationService
};
