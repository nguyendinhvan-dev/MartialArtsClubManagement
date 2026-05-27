const API_BASE_URL = 'http://localhost:5260/api';

// Hàm helper để gửi request có kèm token
async function fetchWithAuth(url, options = {}) {
    const token = localStorage.getItem('jwtToken');
    
    if (!token) {
        throw new Error("Không có token đăng nhập");
    }

    const headers = {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
        ...(options.headers || {})
    };

    const response = await fetch(`${API_BASE_URL}${url}`, {
        ...options,
        headers
    });

    if (response.status === 401) {
        // Token hết hạn hoặc không hợp lệ
        localStorage.removeItem('jwtToken');
        window.location.href = '../pages/login.html';
        throw new Error("Phiên đăng nhập đã hết hạn");
    }

    return response.json();
}

window.API = {
    AuthService: {
        login: async (username, password) => {
            const response = await fetch(`${API_BASE_URL}/Auth/login`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email: username, password: password })
            });
            return response.json();
        },
        logout: () => {
            localStorage.removeItem('jwtToken');
            window.location.href = '../pages/login.html';
        },
        isAuthenticated: () => {
            return !!localStorage.getItem('jwtToken');
        }
    },
    StudentService: {
        getProfile: async () => {
            return fetchWithAuth('/HocVienPortal/profile');
        },
        updateProfile: async (profileData) => {
            return fetchWithAuth('/HocVienPortal/profile', {
                method: 'PUT',
                body: JSON.stringify(profileData)
            });
        }
    },
    AttendanceService: {
        getAttendanceSummary: async () => {
            return fetchWithAuth('/HocVienPortal/attendance');
        }
    },
    ExamService: {
        getExamResults: async () => {
            return fetchWithAuth('/HocVienPortal/exams');
        }
    },
    ScheduleService: {
        getSchedule: async () => {
            return fetchWithAuth('/HocVienPortal/schedule');
        }
    },
    TuitionService: {
        getTuitionHistory: async () => {
            return fetchWithAuth('/HocVienPortal/tuition');
        }
    },
    NotificationService: {
        getNotifications: async () => {
            return fetchWithAuth('/HocVienPortal/notifications');
        }
    },
    SettingService: {
        changePassword: async (currentPassword, newPassword) => {
            return fetchWithAuth('/HocVienPortal/change-password', {
                method: 'PUT',
                body: JSON.stringify({ currentPassword, newPassword })
            });
        }
    }
};
